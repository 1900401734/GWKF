using MesDatas.Models;
using MesDatas.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MesDatas.Services
{
    /// <summary>
    /// 使用现有ChangeOrder表维护最近使用的工单。
    /// </summary>
    internal sealed class WorkOrderHistoryStore
    {
        public const int MaxRecentOrderCount = 10;

        private readonly AccessHelper _database;

        public WorkOrderHistoryStore(AccessHelper database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public Form3Entity GetLatestOrder()
        {
            DataTable rows = _database.Find("SELECT * FROM ChangeOrder ORDER BY id DESC");
            return rows.Rows.Cast<DataRow>()
                .Select(ToEntity)
                .FirstOrDefault(item => !string.IsNullOrWhiteSpace(item.GDH));
        }

        public List<Form3Entity> GetRecentOrders(string operatorId, string orderNoPrefix = null)
        {
            if (string.IsNullOrWhiteSpace(operatorId)) return new List<Form3Entity>();

            string sql = $"SELECT * FROM ChangeOrder WHERE Operator='{EscapeSql(operatorId.Trim())}' ORDER BY id DESC";
            IEnumerable<Form3Entity> orders = _database.Find(sql).Rows.Cast<DataRow>()
                .Select(ToEntity)
                .Where(item => !string.IsNullOrWhiteSpace(item.GDH));

            if (!string.IsNullOrWhiteSpace(orderNoPrefix))
            {
                orders = orders.Where(item => item.GDH.StartsWith(orderNoPrefix, StringComparison.OrdinalIgnoreCase));
            }

            return orders
                .GroupBy(item => item.GDH, StringComparer.OrdinalIgnoreCase)
                .Select(group => group.First())
                .Take(MaxRecentOrderCount)
                .ToList();
        }

        public bool SaveRecentOrder(string orderNo, string operatorId, int orderQuantity)
        {
            if (string.IsNullOrWhiteSpace(orderNo) || string.IsNullOrWhiteSpace(operatorId)) return false;

            string normalizedOrderNo = orderNo.Trim();
            string escapedOperator = EscapeSql(operatorId.Trim());
            string escapedOrderNo = EscapeSql(normalizedOrderNo);
            DataTable rows = _database.Find(
                $"SELECT * FROM ChangeOrder WHERE Operator='{escapedOperator}' ORDER BY id DESC");

            var keptIds = new List<int>();
            var keptOrderNos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (DataRow row in rows.Rows)
            {
                string existingOrderNo = row["OrderNo"].ToString().Trim();
                int id;
                if (string.IsNullOrWhiteSpace(existingOrderNo) ||
                    string.Equals(existingOrderNo, normalizedOrderNo, StringComparison.OrdinalIgnoreCase) ||
                    !keptOrderNos.Add(existingOrderNo) ||
                    !int.TryParse(row["id"].ToString(), out id))
                {
                    continue;
                }

                keptIds.Add(id);
                if (keptIds.Count == MaxRecentOrderCount - 1) break;
            }

            string deleteSql = keptIds.Count == 0
                ? $"DELETE FROM ChangeOrder WHERE Operator='{escapedOperator}'"
                : $"DELETE FROM ChangeOrder WHERE Operator='{escapedOperator}' AND id NOT IN ({string.Join(",", keptIds)})";
            string insertSql =
                $"INSERT INTO ChangeOrder (OrderNo,Operator,OrderNum) VALUES ('{escapedOrderNo}','{escapedOperator}',{orderQuantity})";

            return _database.ExecuteTransaction(deleteSql, insertSql);
        }

        public bool DeleteOrder(string orderNo, string operatorId, int orderQuantity)
        {
            if (string.IsNullOrWhiteSpace(orderNo) || string.IsNullOrWhiteSpace(operatorId)) return false;

            string sql =
                $"DELETE FROM ChangeOrder WHERE OrderNo='{EscapeSql(orderNo.Trim())}' " +
                $"AND Operator='{EscapeSql(operatorId.Trim())}' AND OrderNum={orderQuantity}";
            return _database.Del(sql);
        }

        private static Form3Entity ToEntity(DataRow row)
        {
            int quantity;
            int.TryParse(row["OrderNum"].ToString(), out quantity);

            return new Form3Entity
            {
                GDH = row["OrderNo"].ToString(),
                CZY = row["Operator"].ToString(),
                GDSL = quantity
            };
        }

        private static string EscapeSql(string value)
        {
            return (value ?? string.Empty).Replace("'", "''");
        }
    }
}

using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MesDatas.DataAcess
{
    public class DBConnSugClie
    {
        static string Dbtypele = "Access";
        static string path4 = System.AppDomain.CurrentDomain.BaseDirectory + "SystemDateBase.mdb";
        static string ConnnectString = " Provider = Microsoft.Jet.OLEDB.4.0; Data Source ={0}; Persist Security Info=True; Jet OLEDB:Database Password=byd; User Id=admin ";
        static bool Isfile = true;

        /// <summary>
        /// 获取通用数据库连接
        /// </summary>
        /// <returns></returns>
        public static SqlSugarClient GetDBConnection()
        {
            string conn = string.Format(ConnnectString, path4);
            using (var db = GetSQLSugarConnStr(Dbtypele, conn, Isfile, path4))
            {
                if (!File.Exists(path4))
                {
                    db.DbMaintenance.CreateDatabase();
                }
                return db;
            }
        }

        /// <summary>
        /// 自定义数据库连接
        /// </summary>
        /// <returns></returns>
        public static SqlSugarClient CustomDBConn()
        {
            string conn = string.Format(ConnnectString, path4);
            using (var db = GetSQLSugarConnStr(Dbtypele, conn, Isfile, path4))
            {
                if (!File.Exists(path4))
                {
                    db.DbMaintenance.CreateDatabase();
                }
                return db;
            }
        }

        private static SqlSugarClient GetSQLSugarConnStr(string Dbtypele, string ConnnectString, bool Isfile, string DbName)
        {
            if (ConnnectString == null || ConnnectString.Trim() == "")
            {
                throw new Exception("数据库连接字符串不能为空");
            }
            DbType dbType = SugType.GetDataBaseStringType(Dbtypele);

            var db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConnnectString,//设置数据库连接字符串
                DbType = dbType,//设置数据库类型
                //数据库类型 DbType = DbType.SqlServer,
                IsAutoCloseConnection = true,//自动释放数据务，如果存在事务，在事务结束后释放 
                InitKeyType = InitKeyType.Attribute //从实体特性中读取主键自增列信息 
            });
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                string log = $"{UtilMethods.GetNativeSql(sql, pars)}\n";
            };
            //创建数据库库
            if (Isfile)
            {
                if (!File.Exists(DbName))
                {
                    db.DbMaintenance.CreateDatabase();
                }
            }
            else
            {
                db.DbMaintenance.CreateDatabase();
            }
            //db.DbMaintenance.CreateDatabase();

            //初始化数据表，如果没有则创建
            // db.CodeFirst.InitTables(typeof());
            return db;
        }

        /// <summary>
        /// 创建表
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columns"></param>
        public static void GetInitTablesDynamic(string tableName, List<string> columns)
        {
            try
            {
                using (var db = GetDBConnection())
                {
                    var type = TypeSugarClass.GetListType(db, tableName, columns);
                    // if (db.DbMaintenance.IsAnyTable(tableName))
                    db.CodeFirst.InitTables(type);
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static System.Data.DataTable GetDataTableSql(string sql)
        {
            try
            {
                using (var db = GetDBConnection())
                {
                    return db.Ado.GetDataTable(sql);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取DataSet
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static System.Data.DataSet GetDataSetSql(string sql)
        {
            try
            {
                using (var db = GetDBConnection())
                {
                    return db.Ado.GetDataSetAll(sql);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取几行
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static int GetExecuteCommandSql(string sql)
        {
            try
            {
                using (var db = GetDBConnection())
                {
                    return db.Ado.ExecuteCommand(sql);
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// 添加失败重新生成表在添加
        /// 表名+(i+1)
        /// 最多重创建99表
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="listHead"></param>
        /// <param name="listTail"></param>
        /// <returns></returns>
        public static int GetInsertTableSql(string tableName, List<string> listHead, List<string> listTail)
        {
            try
            {
                int result = GetInsertListSql(tableName, listHead, listTail);
                if (result > 0) { return result; }
                else
                {
                    for (int i = 0; i < 99; i++)
                    { //最多重试99次
                        GetInitTablesDynamic(tableName + (i + 1), listHead);
                        result = GetInsertListSql(tableName + (i + 1), listHead, listTail);
                        if (result > 0) { return result; }
                    }
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
            return 0;
        }

        /// <summary>
        /// INSERT添加
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int GetInsertListSql(string tableName, List<string> listHead, List<string> listTail)
        {
            try
            {
                using (var db = GetDBConnection())
                {
                    return InsertListSqlSuk(tableName, listHead, listTail, db);
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="listHead"></param>
        /// <param name="listTail"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private static int InsertListSqlSuk(string tableName, List<string> listHead, List<string> listTail, SqlSugarClient db)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("INSERT INTO ");
            sql.Append(tableName);
            sql.Append("(");
            for (int i = 0; i < listHead.Count; i++)
            {
                sql.Append(listHead[i]);
                if (i < listHead.Count - 1) { sql.Append(","); }
            }
            sql.Append(") VALUES(");
            for (int i = 0; i < listTail.Count; i++)
            {
                sql.Append("'");
                sql.Append(listTail[i]);
                sql.Append("'");
                if (i < listTail.Count - 1) { sql.Append(","); }
            }
            sql.Append(")");
            return db.Ado.ExecuteCommand(sql.ToString());
        }

        public virtual int GetCountSql(string sql)
        {
            try
            {
                using (var db = GetDBConnection())
                {
                    return db.Ado.GetInt(sql);
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}

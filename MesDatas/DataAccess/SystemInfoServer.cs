using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace MesDatas.DataAcess
{
    public class SystemInfoServer
    {
        private const string SystemInfoTableName = "SystemInfo";
        private const string MesSaveResultTimeoutColumnName = "MesSaveResultTimeoutSeconds";
        private const string TorqueAckTimeoutModeColumnName = "TorqueAckTimeoutMode";
        private const string TorqueAckTimeoutSecondsColumnName = "TorqueAckTimeoutSeconds";

        // 初始化表格
        public static void InitTable()
        {
            try
            {
                using (var db = DBConnSugClie.GetDBConnection())
                {
                    db.CodeFirst.InitTables<SystemInfo>();
                    EnsureMesSaveResultTimeoutColumn(db);
                    EnsureTorqueAckTimeoutModeColumn(db);
                    EnsureTorqueAckTimeoutSecondsColumn(db);

                    SystemInfo plcInfo = SystemInfo.Initalize();
                    if (!db.Queryable<SystemInfo>().Where(it => it.ID == plcInfo.ID).Any())
                    {
                        db.Insertable(plcInfo).ExecuteCommand();
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 兼容现场旧版 Access 数据库，确保 MES 过站超时字段存在。
        /// <para>CodeFirst 对旧 Access 表结构的升级不一定稳定，所以这里做一次显式兜底。</para>
        /// </summary>
        private static void EnsureMesSaveResultTimeoutColumn(SqlSugarClient db)
        {
            try
            {
                if (db.DbMaintenance.IsAnyColumn(SystemInfoTableName, MesSaveResultTimeoutColumnName, false)) return;

                db.Ado.ExecuteCommand($"ALTER TABLE {SystemInfoTableName} ADD COLUMN {MesSaveResultTimeoutColumnName} TEXT(20)");
            }
            catch
            {
                // 字段已存在或现场数据库不支持结构变更时不阻断启动，后续保存仍会返回明确失败。
            }
        }

        /// <summary>
        /// 兼容现场旧版 Access 数据库，确保扭力 ACK 超时处理模式字段存在。
        /// </summary>
        private static void EnsureTorqueAckTimeoutModeColumn(SqlSugarClient db)
        {
            try
            {
                if (db.DbMaintenance.IsAnyColumn(SystemInfoTableName, TorqueAckTimeoutModeColumnName, false)) return;

                db.Ado.ExecuteCommand($"ALTER TABLE {SystemInfoTableName} ADD COLUMN {TorqueAckTimeoutModeColumnName} TEXT(50)");
            }
            catch
            {
                // 字段已存在或现场数据库不支持结构变更时不阻断启动，使用程序默认值继续运行。
            }
        }

        /// <summary>
        /// 兼容现场旧版 Access 数据库，确保扭力 ACK 超时时间字段存在。
        /// </summary>
        private static void EnsureTorqueAckTimeoutSecondsColumn(SqlSugarClient db)
        {
            try
            {
                if (db.DbMaintenance.IsAnyColumn(SystemInfoTableName, TorqueAckTimeoutSecondsColumnName, false)) return;

                db.Ado.ExecuteCommand($"ALTER TABLE {SystemInfoTableName} ADD COLUMN {TorqueAckTimeoutSecondsColumnName} TEXT(20)");
            }
            catch
            {
                // 字段已存在或现场数据库不支持结构变更时不阻断启动，使用程序默认3秒继续运行。
            }
        }

        // 保存
        public static bool SaveSystemInfo(SystemInfo systemInfo)
        {
            try
            {
                using (var db = DBConnSugClie.GetDBConnection())
                {
                    if (db.Queryable<SystemInfo>().Where(it => it.ID == systemInfo.ID).Any())
                    {
                        return db.Updateable(systemInfo).ExecuteCommand() > 0 ? true :false;
                    }
                    else
                    {
                        return db.Insertable(systemInfo).ExecuteCommand() > 0 ? true : false;
                    }
                }
            }
            catch (Exception)
            {

                return false;
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="systemInfo"></param>
        /// <returns></returns>
        public static string UpdateSystemInfo(SystemInfo systemInfo)
        {
            try
            {
                using (var db = DBConnSugClie.GetDBConnection())
                {
                    return db.Updateable(systemInfo).ExecuteCommand() > 0 ? "保存成功" : "保存失败";
                }
            }
            catch (Exception)
            {

                return "保存失败";
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="systemInfo"></param>
        /// <returns></returns>
        public static string DeleteSystemInfo(SystemInfo systemInfo)
        {
            try
            {
                using (var db = DBConnSugClie.GetDBConnection())
                {
                    return db.Deleteable(systemInfo).ExecuteCommand() > 0 ? "保存成功" : "保存失败";
                }
            }
            catch (Exception)
            {

                return "保存失败";
            }
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static SystemInfo GetSystemInfo(int id)
        {
            try
            {
                using (var db = DBConnSugClie.GetDBConnection())
                {
                    return db.Queryable<SystemInfo>().Where(it => it.ID == id).First();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取列表 
        /// </summary>
        /// <returns></returns>
        public static List<SystemInfo> GetSystemInfoList()
        {
            try
            {
                using (var db = DBConnSugClie.GetDBConnection())
                {
                    return db.Queryable<SystemInfo>().ToList();
                }
            }
            catch (Exception)
            {

                return null;
            }
        }

        /// <summary>
        /// 获取列表 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static List<SystemInfo> GetSystemInfoList(int ID)
        {
            try
            {
                using (var db = DBConnSugClie.GetDBConnection())
                {
                    return db.Queryable<SystemInfo>().Where(it => it.ID == ID).ToList();
                }
            }
            catch (Exception)
            {

                return null;
            }
        }

        /// <summary>
        /// 获取BindingList列表 
        /// </summary>
        /// <returns></returns>
        public static BindingList<SystemInfo> GetSystemInfoBindingList()
        {
            return new BindingList<SystemInfo>(GetSystemInfoList());
        }

        /// <summary>
        /// 获取{tableName} 
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static SystemInfo GetLangDeviceInformation(int ID)
        {
            try
            {
                using (var db = DBConnSugClie.GetDBConnection())
                {
                    return db.Queryable<SystemInfo>().Where(it => it.ID == ID).First();
                }
            }
            catch (Exception)
            {

                return null;
            }
        }

    }
}

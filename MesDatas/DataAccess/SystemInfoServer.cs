using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesDatas.DataAcess
{
    public class SystemInfoServer
    {
        // 初始化表格
        public static void InitTable()
        {
            try
            {
                using (var db = DBConnSugClie.GetDBConnection())
                {
                    db.CodeFirst.InitTables<SystemInfo>();

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

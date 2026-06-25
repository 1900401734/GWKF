using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesDatas.DataAcess
{
    public class PlcAddressServer
    {
        // 初始化表格
        public static void InitTable()
        {
            try
            {
                using (var db = DBConnSugClie.GetDBConnection())
                {
                    db.CodeFirst.InitTables<PlcAddressInfo>();

                    PlcAddressInfo plcInfo = PlcAddressInfo.DeviceInformationInitalize();
                    if (!db.Queryable<PlcAddressInfo>().Where(it => it.ID == plcInfo.ID).Any())
                    {
                        db.Insertable(plcInfo).ExecuteCommand();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        // 保存
        public static string GetPlcAddressInfoSave(PlcAddressInfo deviceinformation)
        {
            try
            {
                using (var db = DBConnSugClie.GetDBConnection())
                {
                    if (db.Queryable<PlcAddressInfo>().Where(it => it.ID == deviceinformation.ID).Any())
                    {
                        return db.Updateable(deviceinformation).ExecuteCommand() > 0 ? "保存成功" : "保存失败";
                    }
                    else
                    {
                        return db.Insertable(deviceinformation).ExecuteCommand() > 0 ? "保存成功" : "保存失败";
                    }
                }
            }
            catch (Exception ex)
            {

                return "保存失败";
            }
        }

        //修改DeviceInformation 
        public static string GetPlcAddressInfoUpdate(PlcAddressInfo deviceinformation)
        {
            try
            {
                using (var db = DBConnSugClie.GetDBConnection())
                {
                    return db.Updateable(deviceinformation).ExecuteCommand() > 0 ? "保存成功" : "保存失败";
                }
            }
            catch (Exception ex)
            {

                return "保存失败";
            }
        }

        //删除DeviceInformation 
        public static string GetPlcAddressInfoDelete(PlcAddressInfo deviceinformation)
        {
            try
            {
                using (var db = DBConnSugClie.GetDBConnection())
                {
                    return db.Deleteable(deviceinformation).ExecuteCommand() > 0 ? "保存成功" : "保存失败";
                }
            }
            catch (Exception ex)
            {

                return "保存失败";
            }
        }

        // 获取DeviceInformation 
        public static PlcAddressInfo GetPlcAddressInfo(int id)
        {
            try
            {
                using (var db = DBConnSugClie.GetDBConnection())
                {
                    return db.Queryable<PlcAddressInfo>().Where(it => it.ID == id).First();
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        // 获取DeviceInformation列表 
        public static List<PlcAddressInfo> GetPlcAddressInfoList()
        {
            try
            {
                using (var db = DBConnSugClie.GetDBConnection())
                {
                    return db.Queryable<PlcAddressInfo>().ToList();
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        //获取DeviceInformation列表 
        public static List<PlcAddressInfo> GetPlcAddressInfoList(int ID)
        {
            try
            {
                using (var db = DBConnSugClie.GetDBConnection())
                {
                    return db.Queryable<PlcAddressInfo>().Where(it => it.ID == ID).ToList();
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

        //获取BindingListDeviceInformation列表 
        public static BindingList<PlcAddressInfo> GetPlcAddressInfoBindingList()
        {
            return new BindingList<PlcAddressInfo>(GetPlcAddressInfoList());
        }

        // 获取{tableName} 
        public static PlcAddressInfo GetLangDeviceInformation(int ID)
        {
            try
            {
                using (var db = DBConnSugClie.GetDBConnection())
                {
                    return db.Queryable<PlcAddressInfo>().Where(it => it.ID == ID).First();
                }
            }
            catch (Exception ex)
            {

                return null;
            }
        }

    }
}

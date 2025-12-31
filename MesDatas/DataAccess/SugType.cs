using System;
using SqlSugar;

namespace MesDatas.DataAcess
{
    public class SugType
    {
        #region 获取数据库类型
        /// <summary>
        /// 获取数据库类型
        /// </summary>
        /// <param name="dataBaseType"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static DbType GetDataBaseStringType(string dataBaseType)
        {
            DbType dbType = DbType.MySql;
            switch (dataBaseType)
            {
                case "MySql":
                    dbType = DbType.MySql;//0//mysql数据库
                    break;
                case "SqlServer"://1
                    dbType = DbType.SqlServer;//sqlserver数据库
                    break;
                case "Sqlite"://2
                    dbType = DbType.Sqlite;//sqlite数据库
                    break;
                case "Oracle"://3"//3
                    dbType = DbType.Oracle;//oracle数据库
                    break;
                case "PostgreSQL"://4
                    dbType = DbType.PostgreSQL;
                    break;
                case "Dm"://5
                    dbType = DbType.Dm;
                    break;
                case "Kdbndp"://6
                    dbType = DbType.Kdbndp;
                    break;
                case "Oscar"://7
                    dbType = DbType.Oscar;
                    break;
                case "MySqlConnector"://8
                    dbType = DbType.MySqlConnector;
                    break;
                case "Access"://9
                    dbType = DbType.Access;
                    break;
                case "OpenGauss"://10
                    dbType = DbType.OpenGauss;
                    break;
                case "QuestDB"://11
                    dbType = DbType.QuestDB;
                    break;
                case "HG"://12
                    dbType = DbType.HG;
                    break;
                case "ClickHouse"://13
                    dbType = DbType.ClickHouse;
                    break;
                case "GBase"://14
                    dbType = DbType.GBase;
                    break;
                case "Odbc"://15
                    dbType = DbType.Odbc;
                    break;
                case "OceanBaseForOracle"://16
                    dbType = DbType.OceanBaseForOracle;
                    break;
                case "TDengine"://17
                    dbType = DbType.TDengine;
                    break;
                case "GaussDB"://18
                    dbType = DbType.GaussDB;
                    break;
                case "OceanBase"://19
                    dbType = DbType.OceanBase;
                    break;
                case "Tidb"://20
                    dbType = DbType.Tidb;
                    break;
                case "Vastbase"://21
                    dbType = DbType.Vastbase;
                    break;
                case "PolarDB"://22
                    dbType = DbType.PolarDB;
                    break;
                case "Doris"://23
                    dbType = DbType.Doris;
                    break;
                case "Custom"://900
                    dbType = DbType.Custom;
                    break;
                default:
                    throw new Exception("数据库类型错误");
            }
            return dbType;

        }
        #endregion
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace MesDatas.Utility
{
    class AccessHelper
    {
        //System.Reflection.Missing vtMissing = System.Reflection.Missing.Value;
        private OleDbConnection myConn;

        /// <summary>
        /// 初始化连接数据库
        /// </summary>
        /// <param name="address"></param>
        public AccessHelper(string address)
        {
            try
            {
                // 创建一个 OleDbConnection对象
                string strCon = $"Provider = Microsoft.Jet.OLEDB.4.0; Data Source = {address}; Persist Security Info = True; Jet OLEDB:Database Password=byd; User Id=admin";
                myConn = new OleDbConnection(strCon);
                myConn.Open();
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
        }

        /// <summary>
        /// 关闭数据连接
        /// </summary>
        public void CloseConnection()
        {
            myConn.Close();
        }

        /// <summary>
        /// 创建一个类型和datatable一致的空表
        /// </summary>
        /// <param name="tableName"></param>
        public void CreateTable(string tableName, DataTable dt)
        {
            try
            {
                string sql = "create table " + tableName;
                string tableAttribute = "";
                //for (int i = 0; i < dt.Columns.Count; i++)
                //{
                // tableAttribute = tableAttribute + dt.Columns[i].ColumnName + " " + GetType(dt.Columns[i].DataType.ToString()) + ",";
                //}
                //sql = sql + "(Num AUTOINCREMENT," + tableAttribute + "CONSTRAINT table_PK PRIMARYKEY(Num));";
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    tableAttribute = tableAttribute + dt.Columns[i].ColumnName + " " + GetType(dt.Columns[i].DataType.ToString());
                    if (i < dt.Columns.Count - 1)
                    {
                        tableAttribute = tableAttribute + ",";
                    }
                }
                sql = sql + "(" + tableAttribute + ");";
                OleDbCommand cmd = new OleDbCommand(sql, myConn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        /// <summary>
        /// 将datatable导入对应名字的表中
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="dt"></param>
        public int DatatableToMdb(string name, DataTable dt)
        {
            try
            {

                string strCom = string.Format("select * from {0}", name);
                OleDbDataAdapter da = new OleDbDataAdapter(strCom, myConn);
                //****
                OleDbCommandBuilder cb = new OleDbCommandBuilder(da);//这里的CommandBuilder对象一定不要忘了,一般就是写在DataAdapter定义的后面
                cb.QuotePrefix = "[";
                cb.QuoteSuffix = "]";
                DataSet midData = new DataSet();
                da.Fill(midData, name);
                foreach (DataRow dR in dt.Rows)
                {
                    DataRow dr = midData.Tables[name].NewRow();
                    dr.ItemArray = dR.ItemArray;//行复制
                    midData.Tables[name].Rows.Add(dr);
                }
                da.Update(midData, name);

                return 1;
            }
            catch (Exception ex)
            {
                return 0;
                throw (ex);
            }
        }

        /// <summary>
        /// 更新一行数据到mdb数据库
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sLine"></param>
        public void ListsLineDataToMdb(string name, List<string> sLine)
        {
            try
            {
                string strCom = string.Format("select * from {0}", name);
                OleDbDataAdapter da = new OleDbDataAdapter(strCom, myConn);
                //****
                OleDbCommandBuilder cb = new OleDbCommandBuilder(da);//这里的CommandBuilder对象一定不要忘了,一般就是写在DataAdapter定义的后面
                cb.QuotePrefix = "[";
                cb.QuoteSuffix = "]";
                DataSet midData = new DataSet();
                da.Fill(midData, name);
                DataRow dr = midData.Tables[name].NewRow();
                dr.ItemArray = sLine.ToArray();//行复制
                midData.Tables[name].Rows.Add(dr);
                da.Update(midData, name);
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public bool Add(string sql) //往表中添加一条记录
        {
            try
            {
                OleDbCommand oleDbCommand = new OleDbCommand(sql, myConn);
                int i = oleDbCommand.ExecuteNonQuery(); //返回被修改的数目
                return i > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public bool Del(string sql)
        {
            OleDbCommand oleDbCommand = new OleDbCommand(sql, myConn);
            int i = oleDbCommand.ExecuteNonQuery();
            return i > 0;
        }

        /// <summary>
        /// 是否有数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public bool FarettuFind(string sql)
        {
            try
            {
                OleDbDataAdapter dbDataAdapter = new OleDbDataAdapter(sql, myConn);
                DataTable dt = new DataTable();
                dbDataAdapter.Fill(dt);
                int count = dt.Rows.Count;
                if (count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            //foreach (DataRow item in dt.Rows)
            //{
            //    Console.WriteLine(item[0] + "|" + item[1] + "|" + InvalidOperationException:“未在本地计算机上注册“Microsoft.Jet.OLEDB.4.0”提供程item[2] + "|" + item[3]);
            //}
        }

        /// <summary>
        /// 修改记录
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public bool Change(string sql)
        {
            try
            {
                OleDbCommand oleDbCommand = new OleDbCommand(sql, myConn);
                int i = oleDbCommand.ExecuteNonQuery();
                return i > 0;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable Find(string sql)
        {
            try
            {
                OleDbDataAdapter dbDataAdapter = new OleDbDataAdapter(sql, myConn);
                DataTable dt = new DataTable();
                dbDataAdapter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 获取创建mdb表格的属性字段类型
        /// </summary>
        /// <param name="datatype"></param>
        /// <returns></returns>
        private string GetType(string datatype)
        {
            switch (datatype)//匹配类型选择
            {
                case "System.String":
                    return "TEXT(50)";
                case "System.DateTime":
                    return "DateTime";
                case "System.Double":
                    return "Double";
                case "System.Int32":
                case "System.Int16":
                case "System.Int64":
                    return "Int";
                default:
                    return "TEXT(50)";
            }
        }

        /// <summary>
        /// 创建Access数据库
        /// </summary>
        /// <param name="path">文件和文件路径</param>
        /// <returns>真为创建成功，假为创建失败或是文件已存在</returns>
        public static bool CreateAccessDatabase(string path)
        {
            //如果文件存在反回假
            if (File.Exists(path))
            {
                //MessageBox.Show("文件已存在！");
                return false;
            }
            try
            {
                //如果目录不存在，则创建目录
                string dirName = Path.GetDirectoryName(path);
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
                //创建Catalog目录类
                ADOX.CatalogClass catalog = new ADOX.CatalogClass();
                string _connectionStr = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + path
                       + ";Jet OLEDB:Database Password=" + "byd" + ";Jet OLEDB:Engine Type=5";
                //根据联结字符串使用Jet数据库引擎创建数据库
                catalog.Create(_connectionStr);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(catalog.ActiveConnection);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(catalog);
                catalog = null;
                return true;
            }
            catch (Exception)
            {
                throw new Exception("数据库创建失败!");
            }
        }

        //创建mdb 
        public static bool CreateMDBDataBase(string mdbPath)
        {
            try
            {
                ADOX.CatalogClass cat = new ADOX.CatalogClass();
                cat.Create("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdbPath + ";");
                cat = null;
                return true;
            }
            catch { return false; }
        }

        //新建mdb的表 
        //mdbHead是一个ArrayList，存储的是table表中的具体列名。 
        public static bool CreateMDBTable(string mdbPath, string tableName, ArrayList mdbHead)
        {
            try
            {
                ADOX.CatalogClass cat = new ADOX.CatalogClass();
                string sAccessConnection
                 = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdbPath + ";Persist Security Info=True;Jet OLEDB:Database Password=byd;User Id=admin";
                ADODB.Connection cn = new ADODB.Connection();
                cn.Open(sAccessConnection, null, null, -1);
                cat.ActiveConnection = cn;

                //新建一个表 
                ADOX.TableClass tbl = new ADOX.TableClass();
                tbl.ParentCatalog = cat;
                tbl.Name = tableName;

                int size = mdbHead.Count;
                for (int i = 0; i < size; i++)
                {
                    //增加一个文本字段 
                    ADOX.ColumnClass col2 = new ADOX.ColumnClass();
                    col2.ParentCatalog = cat;
                    col2.Name = mdbHead[i].ToString();//列的名称 
                    col2.Properties["Jet OLEDB:Allow Zero Length"].Value = true;
                    tbl.Columns.Append(col2, ADOX.DataTypeEnum.adVarWChar, 500);
                }
                cat.Tables.Append(tbl); //这句把表加入数据库(非常重要) 
                tbl = null;
                cat = null;
                cn.Close();
                return true;
            }
            catch { return false; }
        }

        // 读取mdb数据 
        public static DataTable ReadAllData(string tableName, string mdbPath, ref bool success)
        {
            DataTable dt = new DataTable();
            try
            {
                DataRow dr;
                //1、建立连接 
                string strConn
                 = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + mdbPath + ";Jet OLEDB:Database Password=haoren";
                OleDbConnection odcConnection = new OleDbConnection(strConn);
                //2、打开连接 
                odcConnection.Open();
                //建立SQL查询 
                OleDbCommand odCommand = odcConnection.CreateCommand();
                //3、输入查询语句 
                odCommand.CommandText = "select * from " + tableName;
                //建立读取 
                OleDbDataReader odrReader = odCommand.ExecuteReader();
                //查询并显示数据 
                int size = odrReader.FieldCount;
                for (int i = 0; i < size; i++)
                {
                    DataColumn dc;
                    dc = new DataColumn(odrReader.GetName(i));
                    dt.Columns.Add(dc);
                }
                while (odrReader.Read())
                {
                    dr = dt.NewRow();
                    for (int i = 0; i < size; i++)
                    {
                        dr[odrReader.GetName(i)] = odrReader[odrReader.GetName(i)].ToString();
                    }
                    dt.Rows.Add(dr);
                }
                //关闭连接 
                odrReader.Close();
                odcConnection.Close();
                success = true;
                return dt;
            }
            catch
            {
                success = false;
                return dt;
            }
        }

        public static DataTable ReadDataByColumns_beifen(string mdbPath, string tableName, string[] columns, ref bool success)
        {
            DataTable dt = new DataTable();
            try
            {
                DataRow dr;
                //1、建立连接 
                string strConn
                 = mdbPath;
                OleDbConnection odcConnection = new OleDbConnection(strConn);
                //2、打开连接 
                odcConnection.Open();
                //建立SQL查询 
                OleDbCommand odCommand = odcConnection.CreateCommand();
                //3、输入查询语句 
                string strColumn = "";
                for (int i = 0; i < columns.Length; i++)
                {
                    strColumn += columns[i].ToString() + ",";
                }
                strColumn = strColumn.TrimEnd(',');
                odCommand.CommandText = "select " + strColumn + " from " + tableName;
                //建立读取 
                OleDbDataReader odrReader = odCommand.ExecuteReader();
                //查询并显示数据 
                int size = odrReader.FieldCount;
                for (int i = 0; i < size; i++)
                {
                    DataColumn dc;
                    dc = new DataColumn(odrReader.GetName(i));
                    dt.Columns.Add(dc);
                }

                while (odrReader.Read())
                {
                    dr = dt.NewRow();
                    for (int i = 0; i < size; i++)
                    {
                        dr[odrReader.GetName(i)] = odrReader[odrReader.GetName(i)].ToString();
                    }
                    dt.Rows.Add(dr);
                }
                //关闭连接 
                odrReader.Close();
                odcConnection.Close();
                success = true;
                return dt;
            }
            catch
            {
                success = false;
                return dt;
            }
        }

        public static DataTable ReadDataByColumns(string mdbPath, string tableName, string columns, ref bool success)
        {
            DataTable dt = new DataTable();
            try
            {
                DataRow dr;
                //1、建立连接 
                string strConn
                 = mdbPath;
                OleDbConnection odcConnection = new OleDbConnection(strConn);
                //2、打开连接 
                odcConnection.Open();
                //建立SQL查询 
                OleDbCommand odCommand = odcConnection.CreateCommand();
                //3、输入查询语句 
                string strColumn = columns;
                strColumn = strColumn.TrimEnd(',');
                odCommand.CommandText = "select " + strColumn + " from " + tableName;
                //建立读取 
                OleDbDataReader odrReader = odCommand.ExecuteReader();
                //查询并显示数据 
                int size = odrReader.FieldCount;
                for (int i = 0; i < size; i++)
                {
                    DataColumn dc;
                    dc = new DataColumn(odrReader.GetName(i));
                    dt.Columns.Add(dc);
                }

                while (odrReader.Read())
                {
                    dr = dt.NewRow();
                    for (int i = 0; i < size; i++)
                    {
                        dr[odrReader.GetName(i)] = odrReader[odrReader.GetName(i)].ToString();
                    }
                    dt.Rows.Add(dr);
                }
                //关闭连接 
                odrReader.Close();
                odcConnection.Close();
                success = true;
                return dt;
            }
            catch
            {
                success = false;
                return dt;
            }
        }

    }
}

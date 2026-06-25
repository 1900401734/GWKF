using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MesDatas.Utility
{ 
    public class CsvHelper
    {
        /******Csv数据的路径*******/
        public string CsvFilePath = "";

        public CsvHelper(string str = "C:\\TmpData\\")
        {
            this.CsvFilePath = str;
        }

        /// <summary>
        /// 将Csv表格中的数据写入到Datatable中
        /// </summary>
        /// <returns>返回数据表</returns>
        public DataTable ReadCsvToDatatable()
        {
            DataTable myTable = new DataTable();
            try
            {
                if(!File.Exists(CsvFilePath))
                {
                    File.Create(CsvFilePath);
                }
                FileStream fs = new FileStream(CsvFilePath, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs, Encoding.Default);
                string readStr;
                bool firstRead = true;
                while ((readStr = sr.ReadLine()) != null)
                {
                    string[] str = readStr.Split(',');
                    if (firstRead)
                    {
                        for (int i = 0; i < str.Length; i++)
                        {
                            DataColumn myColumn = new DataColumn();
                            myColumn = myTable.Columns.Add(str[i]);
                        }
                        firstRead = false;
                    }
                    else
                    {
                        DataRow myRow = myTable.NewRow();
                        for (int j = 0; j < myTable.Columns.Count; j++)
                        {
                            myRow[j] = str[j];
                        }
                        myTable.Rows.Add(myRow);
                    }
                }
                sr.Close();
                fs.Close();
                sr.Dispose();
                fs.Dispose();
                return myTable;
            }
            catch (Exception e)
            {
                MessageBox.Show("ReadCsvToDatatable", e.Message);
                return myTable;
            }
        }

        /// <summary>
        ///将Datatable表中的数据写入到DataGridView控件中
        /// </summary>
        /// <param name="dt">Datatable数据表</param>
        /// <param name="myGridView">GridView控件</param>
        /// <returns>false，失败|true，成功</returns>
        public bool DatatableToDataGirdView(DataTable dt, DataGridView myGridView)
        {
            try
            {
             
               // dt = ReadCsvToDatatable();
                if (dt.Rows.Count == 0)
                {
                    return false;
                }
                Font myFont = new Font("宋体", 10, FontStyle.Bold);
                myGridView.GridColor = Color.Green;
                myGridView.ColumnCount = dt.Columns.Count;
                myGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
                myGridView.RowHeadersWidth = 30;
                myGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
                myGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                myGridView.ColumnHeadersDefaultCellStyle.Font = myFont;
                for (int i = 0; i < myGridView.Columns.Count; i++)
                {
                    myGridView.Columns[i].DefaultCellStyle.Font = new Font(myGridView.DefaultCellStyle.Font, FontStyle.Regular);
                    myGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    myGridView.Columns[i].Resizable = DataGridViewTriState.False;
                    myGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;

                }
                myGridView.CellBorderStyle = DataGridViewCellBorderStyle.Single;
                myGridView.MultiSelect = false;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    myGridView.Rows.Add(dt.Rows[i].ItemArray);
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("DatatableToDataGirdView", ex.ToString());
                return false;
            }
        }
       
        /// <summary>
        /// 将DataGridView控件内容写入到Datatable中
        /// </summary>
        /// <param name="myGridView">需要转换的DataGridView控件</param>
        /// <returns>Datatable表格</returns>
        public DataTable DataGridViewToDatatable(DataGridView myGridView)
        {
            DataTable dt = new DataTable();
            /******添加列到Datatable*********/
            for (int i = 0; i < myGridView.Columns.Count; i++)
            {
                dt.Columns.Add(myGridView.Columns[i].HeaderText);
            }
            /******添加行到Datatable控件中*********/
            for (int k = 0; k < myGridView.Rows.Count; k++)
            {
                DataRow myRow = dt.NewRow();
                for (int j = 0; j < myGridView.Columns.Count; j++)
                {
                    myRow[j] = myGridView[j, k].Value;
                }
                dt.Rows.Add(myRow);
            }
            return dt;
        }

        /// <summary>
        /// 将Datatable表格中的数据写入到Csv文件中
        /// </summary>
        /// <param name="dt">Datatable数据表</param>
        /// <param name="filePath">Csv文件的路径</param>
        /// <returns>False，失败|True，成功</returns>
        public bool DatatableToCsv(DataTable dt)
        {
            try
            {
                FileStream fs = null;
                StreamWriter sw = null;
                fs = new FileStream(CsvFilePath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                sw = new StreamWriter(fs, System.Text.Encoding.Default);
                //string data = "";
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    builder.Append(dt.Columns[i].ColumnName.ToString());
                    if (i < dt.Columns.Count - 1)
                    {
                        builder.Append(",");
                    }
                }
                sw.WriteLine(builder.ToString());

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    builder.Clear();
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        builder.Append(dt.Rows[i][j].ToString());
                        if (j < dt.Columns.Count - 1)
                        {
                            builder.Append(",");
                        }
                    }
                    sw.WriteLine(builder.ToString());
                }
                builder = null;
                sw.Close();
                fs.Close();
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show("DatatableToCsv", e.Message);
                return false;
            }
        }

        /// <summary>
        /// 读取特定某一行的值
        /// </summary>
        /// <param name="dt">Datatable表格</param>
        /// <param name="name">第一列的索引名称</param>
        /// <param name="list">存放读取数值的集合</param>
        /// <returns>true,读取成功|false,读取失败</returns>
        public bool ReadRowValue(DataTable dt, string name, out List<double> list)
        {
            try
            {
                List<double> list_Value = new List<double>();
                int count = dt.Rows.Count;
                for (int i = 0; i < count; i++)
                {
                    if (name == dt.Rows[i][0].ToString())
                    {
                        for (int k = 1; k < dt.Columns.Count; k++)
                        {
                            list_Value.Add(double.Parse(dt.Rows[i][k].ToString()));
                        }
                    }
                }
                list = list_Value;
                return true;
            }
            catch
            {
                list = null;
                return false;
            }
        }

        /// <summary>
        /// 获取表中列的名称
        /// </summary>
        /// <param name="dt">Datatable表格</param>
        /// <param name="list">存放名称的集合</param>
        /// <returns>true,读取成功|false,读取失败</returns>
        public bool GetTableColNames(DataTable dt, out List<string> list)
        {
            try
            {
                List<string> list_Nmae = new List<string>();
                int count = dt.Rows.Count;
                for (int i = 0; i < count; i++)
                {
                    if (dt.Rows[i][0].ToString() == "")
                    {
                        break;
                    }
                    else
                    {
                        list_Nmae.Add(dt.Rows[i][0].ToString());
                    }
                }
                list = list_Nmae;
                return true;
            }
            catch
            {
                list = null;
                return false;
            }
        }
    }
}

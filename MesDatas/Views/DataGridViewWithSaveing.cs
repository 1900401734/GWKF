using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MesDatas.Utility;

namespace MesDatas.Views
{
    public class DataGridViewWithSaveing : DataGridView
    {
        public DataTable mData = new DataTable();
        public CsvHelper myCSVDeal = new CsvHelper(); // tool Name + Control name  Application.StartupPath + "\\Config.csv"
        /// <summary>
        /// 保存缓冲数据&
        /// </summary>
        public void SaveCacheDateToCSV()
        {
            mData = myCSVDeal.DataGridViewToDatatable(this);
            myCSVDeal.DatatableToCsv(mData);
        }
        DataTable mySetData = new DataTable();
        public void LoadCacheDataFromCsv()
        {
            mySetData = myCSVDeal.ReadCsvToDatatable();

            myCSVDeal.DatatableToDataGirdView(mySetData, this);
        }

        protected string CsvPath = "";
        [Browsable(false)]
        public void SetDataSavePath(string sPath)
        {

            if (!Directory.Exists(sPath))
            {
                Directory.CreateDirectory(sPath);
            }
            CsvPath = sPath + "\\" + Name + ".csv";

            myCSVDeal.CsvFilePath = CsvPath;
        }

        public void SetDataTable(DataTable SetData)
        {
            mySetData = SetData;
            myCSVDeal.DatatableToDataGirdView(mySetData, this);
       
                      
        }
    }

    public class DataGridViewDynamic : DataGridViewWithSaveing
    {

        private List<string> Heards = new List<string>();

        public string ColumnKey { get; set; }

        public void AddHeader(string columnName)
        {
            if (!Heards.Contains(columnName))
            {
                Heards.Add(columnName);
                this.Columns.Add(columnName, columnName);
            }
        }

        public void ResetGrid(bool IsDelCells = false)
        {
            
            //if(IsDelCells && Rows.Count>1)
            //{
                
            //    foreach (DataGridViewRow r in Rows)
            //    {
            //        if (r != null )
            //        {
            //            Rows.Remove(r);
            //        }
                   
            //    }
               
               
            //}
            //else
            //{
            //    for (int i = 0; i < Rows.Count; i++)
            //    {
            //        for (int j = 0; j < Columns.Count; j++)
            //        {
            //            Rows[i].Cells[j].Value = "";
            //            Rows[i].Cells[j].Style.BackColor = Color.White;
            //        }
            //    }
            //}
            Rows.Clear();


        }

        string currentKey;

        public void UpdateTestData(string name, string value, bool ResultOkNg, bool IsKey = false)
        {
            this.Invoke(new Action(() =>
            {
                if (IsKey)
                {
                    currentKey = name;
                    PrvData(name, value, name, ResultOkNg);
                }
                else
                {
                    string curentKey = this.currentKey;
                    PrvData(name, value, curentKey, ResultOkNg);
                }

            }));

            SaveCacheDateToCSV();    //显示表格的缓存
            SaveData();
        }

        private void PrvData(string sKey, string sValue, string curentKey, bool OKng)
        {
            int index = 0;
            bool IsNeedAddNewRow = true;

            if (!Heards.Contains(sKey))
            {
                AddHeader(sKey);
            }




            bool IsHad = false;
            for (int i = 0; i < Rows.Count; i++)
            {
                if ((string)Rows[i].Cells[ColumnKey].Value == curentKey) //根据序号找到同一个产品数据行
                {
                    //产品序号                     
                    index = i;
                    IsHad = true;
                    IsNeedAddNewRow = false;
                    break;
                }
            }
            if (!IsHad)
            {
                //新增序号找到同一个产品数据行
                for (int i = 0; i < Rows.Count; i++)
                {
                    if ((string)Rows[i].Cells[ColumnKey].Value == "" || (string)Rows[i].Cells[ColumnKey].Value == null)
                    {
                        index = i;
                        Rows[index].Cells[ColumnKey].Value = curentKey;
                        IsNeedAddNewRow = false;
                        break;
                    }

                }
            }


            if (IsNeedAddNewRow)
            {
                DataGridViewRow Row = new DataGridViewRow();
                Rows.Add(Row);
                index = Rows.Count - 1;
            }

            Rows[index].Cells[sKey].Value = sValue;
            if (!OKng)
            {
                Rows[index].Cells[sKey].Style.BackColor = Color.Red;
            }

        }

        /// <summary>
        /// 保存数据到文件并且刷新表格
        /// </summary>
        private void SaveData()
        {
            /***********保存数据***********/

            if (CheckDGV())
            {

                this.Invoke(new Action(() =>
                {
                    SaveDgvData();
                    Rows.Remove(Rows[0]);
                    DataGridViewRow Row = new DataGridViewRow();
                    Rows.Add(Row);
                }));
            }
        }
        /// <summary>
        /// 确认数据是否已经完成
        /// </summary>
        /// <returns></returns>
        private bool CheckDGV()
        {
            for (int i = 0; i < ColumnCount; i++)
            {
                if (!(Rows[0].Cells[i].Value != null && Rows[0].Cells[i].Value.ToString() != ""))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 保存一行的数据
        /// </summary>
        private void SaveDgvData()
        {
            string datatime = DateTime.Now.ToString();
            string b = string.Format("{0:yyyyMMdd}", DateTime.Now);
            List<string> values = new List<string>();
            foreach (string skey in Heards)
            {
                values.Add((string)Rows[0].Cells[skey].Value);
            }
            //string dateLog = DateTime.Now.ToString();
            // Save_Data(values);
            updateData?.Invoke(values);
        }

        public delegateUpdateData updateData;

    }

    public delegate void delegateUpdateData(List<string> content);
}

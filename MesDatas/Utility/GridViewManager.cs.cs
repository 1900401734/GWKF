using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using static MesDatas.Utility.DataGridViewData;

namespace MesDatas.Utility
{
    public delegate void OperatorButton(object sender, DataGridViewCellEventArgs e, string columnName, Func<DataGridViewRow, bool> submitCondition = null);

    class ColumnButtonEntity
    {
        public string header_text { get; set; }
        public string button_name { get; set; }
        public string button_text { get; set; }
        public OperatorButton operatorButton { get; set; }
        public DataGridViewCellEventHandler operatorEvent { get; set; }  // 点击按钮触发事件函数指针
        public Func<DataGridViewRow, bool> submitCondition { get; set; }  // 点击按钮事件触发的限制条件
    }

    /// <summary>
    /// 自动绑定数据
    /// </summary>
    class DataGridViewData
    {
        private int rowCount;

        private string tableName;

        private readonly AccessHelper mdb;

        private DataGridView dataGridView;

        private readonly List<ColumnButtonEntity> columnButton = new List<ColumnButtonEntity>(); // 存储按键对应的方法，方便移除

        private Dictionary<string, string> dataMap; // 数据映射，数据库的字段名映射到dataGridView的列名 {"别名","字段名"}

        private readonly List<string> buttonNameLst = new List<string>();  // 保存所有的列名

        private readonly Dictionary<int, bool> recordRowIsChanged = new Dictionary<int, bool>();  //记录行号对应的值是否被改变

        private bool ShowMessage => true;   // 控制按键的操作结果是否需要提示用户

        public bool Changed { get; set; }   // 由用户决定dataGridView是否发生了改变，没有发生改变没必要再次访问数据库

        public DataGridViewData(DataGridView dataGridView, string tableName, AccessHelper mdb)
        {
            _InitDataGridView(dataGridView, tableName);
            this.mdb = mdb;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="dataGridView"></param>
        /// <param name="tableName"></param>
        private void _InitDataGridView(DataGridView dataGridView, string tableName)
        {
            this.tableName = tableName;
            this.dataGridView = dataGridView;

            this.dataGridView.CellValueChanged += DataGridView_CellValueChanged;
        }

        /// <summary>
        /// 获取用户新增行的id
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private int GetNewLineId(DataTable dt)
        {
            try
            {
                return dt.AsEnumerable().Max(row => row.Field<int>("id")) + 1;
            }
            catch
            {
                return 1;
            }
        }

        /// <summary>
        /// 设置DataGridView的样式
        /// </summary>
        private void SetDataGridViewStyle()
        {
            // 自动调整宽度
            int width = 0;

            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                width += (column.Width < 100 ? 100 : column.Width);
            }

            if (width > dataGridView.Width)
            {
                //dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            }
            else
                dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        /// <summary>
        /// 查询表的数据添加到dataGridView
        /// </summary>
        /// <param name="dataMap">{显示的列名:数据库表的字段名}</param>
        public void BindDataToDataGridView(Dictionary<string, string> dataMap)
        {
            string asField = "";
            foreach (var data in dataMap)
            {
                asField += $"{data.Value} as {data.Key},";
            }
            string sql = $"select {asField.Remove(asField.Length - 1)} from {tableName}";

            DataTable dt = mdb.Find(sql);

            // 记录每一行被改变的值
            foreach (DataRow row in dt.Rows)
            {
                recordRowIsChanged[int.Parse(row["id"].ToString())] = false;
            }

            dt.Columns["id"].AutoIncrement = true;
            dt.Columns["id"].ReadOnly = true;
            // 获取用户新增行的id
            dt.Columns["id"].AutoIncrementSeed = GetNewLineId(dt);

            rowCount = dt.Rows.Count;
            // 绑定数据集
            dataGridView.DataSource = dt;

            //if(this.hideIdColumn = hideId) dataGridView.Columns["id"].Visible = false;

            this.dataMap = dataMap;

            SetDataGridViewStyle();
        }

        public void AddOperatorColumnsButton(string headerText, string buttonName, string buttonText, OperatorButton operatorButton, Func<DataGridViewRow, bool> submitCondition = null)
        {
            ColumnButtonEntity columnButton = new ColumnButtonEntity
            {
                header_text = headerText,
                button_name = buttonName,
                button_text = buttonText,
                operatorButton = operatorButton,
                operatorEvent = (sender, e) => operatorButton(sender, e, buttonName, submitCondition),
                submitCondition = submitCondition,
            };

            DataGridViewButtonColumn button = _CreateButtonAtrribute(columnButton);

            // 将按钮添加进数据表
            dataGridView.Columns.Add(button);
            buttonNameLst.Add(buttonName);

            // 赋值该按钮点击触发的方法
            dataGridView.CellContentClick += columnButton.operatorEvent;
            this.columnButton.Add(columnButton);

            SetDataGridViewStyle();
        }

        /// <summary>
        /// 绑定其它按钮事件
        /// </summary>
        /// <param name="button"></param>
        /// <param name="eventMethod"></param>
        public void BindEventHandlerButton(Button button, EventHandler eventMethod)
        {
            button.Click += eventMethod;
        }

        /// <summary>
        /// 新建一个DataGridViewButtonColumn类型的button
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        private DataGridViewButtonColumn _CreateButtonAtrribute(ColumnButtonEntity button)
        {
            DataGridViewButtonColumn columnButton = new DataGridViewButtonColumn();
            columnButton.HeaderText = button.header_text;
            columnButton.Name = button.button_name;
            columnButton.Text = button.button_text;
            columnButton.UseColumnTextForButtonValue = true;
            columnButton.DefaultCellStyle.NullValue = button.button_text;
            columnButton.Width = 50;
            return columnButton;
        }

        /// <summary>
        /// dataGridView中的默认保存按钮触发事件
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        /// <param name="judgement">对某一列需要怎么进行判断，不通过则直接弹框 {列别名:判断条件}</param>
        /// <param name="columnName"></param>
        public void SaveButton_Click(Object Sender, DataGridViewCellEventArgs e, string columnName, Func<DataGridViewRow, bool> judgement = null)
        {
            // 判断是不是保存键
            if (e.ColumnIndex != dataGridView.Columns[columnName].Index) return;

            // 获取当前行对象
            DataGridViewRow curRow = dataGridView.Rows[e.RowIndex];

            if (recordRowIsChanged.Keys.Contains(curRow.Index) && !recordRowIsChanged[curRow.Index])
            {
                AlertMessageBox("数据没有更改过,无需重复提交");
                return;
            }

            if (judgement != null && !judgement(curRow))
            {
                AlertMessageBox("数据检查不通过，请正确输入后提交");
                return;
            }

            // 将列名和值添加进列表
            List<Dictionary<string, string>> dataLst = new List<Dictionary<string, string>>();
            foreach (DataGridViewColumn button in dataGridView.Columns)
            {
                if (buttonNameLst.Contains(button.Name)) continue;
                columnName = button.Name;

                dataLst.Add(new Dictionary<string, string> {
                    {"ColumnName",columnName },
                    {"Value",curRow.Cells[columnName].Value.ToString() }
                });
            }

            // 判断当前的行号是不是已经超过了统计的行号，如果超过了就是用户新增的一行
            // 否则就是修改数据
            if (rowCount - 1 < e.RowIndex)
            {
                if (_InsertData(tableName, dataLst))
                {
                    rowCount++;
                    recordRowIsChanged[curRow.Index] = false;
                    Changed = true;
                    AlertMessageBox("新建数据成功");
                }
                else
                {
                    AlertMessageBox("新建数据失败，请检查数据填写是否存在重复或格式问题");
                }
            }
            else
            {
                if (_UpdateData(tableName, dataLst, dataMap))
                {
                    recordRowIsChanged[curRow.Index] = false;
                    Changed = true;

                    AlertMessageBox("修改成功");
                    //Thread.Sleep(2000);
                }
                else
                {
                    AlertMessageBox("修改数据失败，请检查数据填写是否存在重复或格式问题");
                }
            }
        }

        /// <summary>
        /// 需要修改数据时
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dataList"></param>
        /// <param name="dataMap"></param>
        private bool _UpdateData(string tableName, List<Dictionary<string, string>> dataList, Dictionary<string, string> dataMap)
        {
            string set = "";
            string id = "-1";

            foreach (var data in dataList)
            {
                if (data["ColumnName"] == "id")
                {
                    id = data["Value"];
                    continue;
                }

                string field = dataMap[data["ColumnName"]];  //获取数据库的字段名
                string value = data["Value"];

                // 【修改开始】判断是否为布尔值或数字，决定是否加单引号
                // 假设你的 IsActive 传进来的是 "True", "False", "1", "0"
                if (value == "True" || value == "False")
                {
                    // 布尔值不加引号
                    set += $"{field}={value},";
                }
                else
                {
                    // 其它情况（字符串）加单引号
                    set += $"{field}='{value}',";
                }
                // 【修改结束】

                //如果需要更新id
                //set += (data["ColumnName"] == "id" ? $"{field}={id = data["Value"]}" : $"{field}='{data["Value"]}'") + ",";  //如果需要更新id:
            }
            string sql = $"update {tableName} set {set.Remove(set.Length - 1)} where id={id}";
            if (mdb.Change(sql))
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// 需要添加数据时
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dataList"></param>
        private bool _InsertData(string tableName, List<Dictionary<string, string>> dataList)
        {
            string values = "";
            string fields = "";

            foreach (var data in dataList)
            {
                //values += (data["ColumnName"] == "id" 
                //    ? $"{data["Value"]}" 
                //    : $"'{data["Value"]}'") + ",";

                if (data["ColumnName"] == "id")
                {
                    values += $"{data["Value"]},";
                }
                else if(data["ColumnName"] == "是否启用")
                {
                    if (data["Value"] == "True")
                    {
                        values += $"{data["Value"]},";
                    }
                    else
                    {
                        values += "False,";
                    }
                }
                else
                {
                    values += $"'{data["Value"]}',";
                }

                fields += dataMap[data["ColumnName"]] + ",";
            }

            values = values.Remove(values.Length - 1);
            fields = fields.Remove(fields.Length - 1);

            string sql = $"insert into {tableName}({fields}) values({values})";

            if (mdb.Add(sql))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// dataGridView中的删除按钮触发事件
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        /// <param name="columnName"></param>
        /// <param name="Judgement"></param>
        public void DeleteButton_Click(Object Sender, DataGridViewCellEventArgs e, string columnName, Func<DataGridViewRow, bool> Judgement = null)
        {
            //判断是不是删除按钮触发的事件
            if (e.ColumnIndex != this.dataGridView.Columns[columnName].Index) return;

            DataGridView dataGridView = Sender as DataGridView;
            //获取当前点击的行对象
            DataGridViewRow curRow = dataGridView.Rows[e.RowIndex];

            //获取id值
            string id = curRow.Cells["id"].Value.ToString();
            //获取数据库的表名
            string sql = $"delete from {tableName} where id={id}";
            if (rowCount - 1 < e.RowIndex)  //不是用户新增的数据
            {
                AlertMessageBox("未保存无需删除");
                return;
            }
            if (Judgement != null && !Judgement(curRow))
            {
                AlertMessageBox($"删除失败");
                return;
            }
            if (mdb.Del(sql))
            {
                Changed = true;
                recordRowIsChanged.Remove(curRow.Index);
                AlertMessageBox("删除数据成功");
                //移除当前行
                dataGridView.Rows.RemoveAt(e.RowIndex);
                //总行数减1
                rowCount--;
                if (rowCount == 0)
                {
                    mdb.Change($"ALTER TABLE {tableName} ALTER COLUMN id COUNTER (1, 1)");
                }

            }
            else AlertMessageBox("删除数据失败");
        }

        /// <summary>
        /// 页面上的数据改变时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            DataGridViewRow curRow = dataGridView.Rows[e.RowIndex];
            recordRowIsChanged[curRow.Index] = true;
        }

        /// <summary>
        /// 被绑定的刷新事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RefreshButton(object sender, EventArgs e)
        {
            foreach (var buttonEntity in columnButton)
            {
                dataGridView.CellContentClick -= buttonEntity.operatorEvent;
            }
            dataGridView.Columns.Clear();
            BindDataToDataGridView(dataMap);
            //拷贝进数组，防止在下面代码调用AddOperatorColumnsButton时columnButton被改变
            ColumnButtonEntity[] tempColumnButton = new ColumnButtonEntity[columnButton.Count];
            columnButton.CopyTo(tempColumnButton);

            columnButton.Clear();
            foreach (ColumnButtonEntity operColumn in tempColumnButton)
            {
                AddOperatorColumnsButton(operColumn.header_text, operColumn.button_name, operColumn.button_text, operColumn.operatorButton, operColumn.submitCondition);
            }
            //自动调整宽度
            SetDataGridViewStyle();
        }

        private void AlertMessageBox(string message)
        {
            if (ShowMessage)
                MessageBox.Show(message);
        }

        /// <summary>
        /// 辅助方法：提取单行数据
        /// </summary>
        private List<Dictionary<string, string>> GetRowDataDict(DataGridViewRow curRow)
        {
            List<Dictionary<string, string>> dataLst = new List<Dictionary<string, string>>();

            foreach (DataGridViewColumn col in dataGridView.Columns)
            {
                // 跳过操作按钮列
                if (buttonNameLst.Contains(col.Name)) continue;

                string columnName = col.Name;
                string strValue = "";
                var cellValue = curRow.Cells[columnName].Value;

                // 1. 处理空值
                if (cellValue == null || cellValue == DBNull.Value)
                {
                    // 如果是复选框，空值默认为 "0" (False)
                    if (col is DataGridViewCheckBoxColumn)
                        strValue = "0";
                    else
                        strValue = "";
                }
                else
                {
                    // 2. 处理布尔值 (复选框)
                    if (cellValue is bool bVal)
                    {
                        // 转为 1 或 0，兼容性最好，避免 'True' 字符串导致的 SQL 错误
                        strValue = bVal ? "1" : "0";
                    }
                    else
                    {
                        strValue = cellValue.ToString();
                    }
                }

                dataLst.Add(new Dictionary<string, string> {
            {"ColumnName", columnName },
            {"Value", strValue }
        });
            }
            return dataLst;
        }

        /// <summary>
        /// 一次性保存表格中的所有数据
        /// </summary>
        public void SaveAllData(object sender, EventArgs e)
        {
            // 1. 强制结束编辑状态，确保最后一个单元格的数据被提交
            dataGridView.EndEdit();

            int successCount = 0;
            int failCount = 0;
            bool hasError = false;

            // 2. 遍历每一行
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                // 跳过 DataGridView 最底部的那个自动生成的空白新行
                if (row.IsNewRow) continue;

                // 判断是否是新增行 (行索引 >= 原始行数)
                bool isInsert = (rowCount - 1 < row.Index);

                // 判断数据是否被修改过 (如果是旧数据且未修改，则跳过，提高效率)
                // 如果想强制保存所有，可以去掉 !isInsert 的判断
                bool isModified = recordRowIsChanged.ContainsKey(row.Index) && recordRowIsChanged[row.Index];

                if (!isInsert && !isModified)
                {
                    continue; // 没有变化，跳过
                }

                // 3. 获取该行数据
                var dataLst = GetRowDataDict(row);

                // 4. 执行数据库操作
                bool result = false;
                if (isInsert)
                {
                    // 新增
                    if (_InsertData(tableName, dataLst))
                    {
                        result = true;
                        // 注意：这里不要立即 rowCount++，因为循环中会依赖原始 rowCount 判断后续行
                        // 我们稍后统一更新状态，或者在这里只标记状态
                        recordRowIsChanged[row.Index] = false;
                    }
                }
                else
                {
                    // 更新
                    if (_UpdateData(tableName, dataLst, dataMap))
                    {
                        result = true;
                        recordRowIsChanged[row.Index] = false;
                    }
                }

                if (result) successCount++;
                else
                {
                    failCount++;
                    hasError = true;
                }
            }

            // 5. 循环结束后修正 rowCount
            // 重新统计行数，确保下一次保存时的判断逻辑正确
            if (mdb != null)
            {
                // 简单粗暴但准确的方法：重新查询一下总行数，或者根据 grid 行数更新
                // 考虑到可能有删除操作，最稳妥的是：
                rowCount = dataGridView.Rows.Count - 1; // 减去 NewRow
            }

            Changed = true; // 标记发生了改变

            // 6. 提示结果
            string msg = $"批量保存完成。\n成功: {successCount} 条\n失败: {failCount} 条";
            if (hasError)
            {
                MessageBox.Show(msg, "保存结果", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (successCount > 0)
            {
                AlertMessageBox("所有数据保存成功！");
            }
            else
            {
                AlertMessageBox("没有数据需要保存。");
            }
        }
    }
}

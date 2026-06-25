using System;
using System.Data;
using System.Windows.Forms;
using MesDatas.Models;
using MesDatas.Utility;

namespace MesDatas.Views
{
    public partial class Form3 : Form
    {

        public delegate void ValueSelectedEventHandler(Form3Entity selectedValue);
        public event ValueSelectedEventHandler ValueSelected;

        private AccessHelper mdb = new AccessHelper(Global.Instance.DataBase);

        public Form3()
        {
            InitializeComponent();
            Operator.Text = Global.Instance.LoginMessage.WorkId;
        }

        /// <summary>
        /// 只允许数据数字
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnlyAllowDigital_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (e.KeyChar == '0' && textBox.Text == "")
            {
                e.Handled = true;
            }
            // 允许输入数字0-9和小数点（如果需要）  
            else if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //if (OrderNo.Text.Length < 1)
            //{
            //    MessageBox.Show("工单号不能为空！");
            //    return;
            //}
            //else if (OrderNum.Text.Length < 1)
            //{
            //    MessageBox.Show("工单数量不能为空！");
            //    return;
            //}
            //else if (Operator.Text.Length < 1)
            //{
            //    MessageBox.Show("操作员不能为空！");
            //    return;
            //}
            string orderNum = string.IsNullOrEmpty(OrderNum.Text) ? "0" : OrderNum.Text;
            string orderNo = OrderNo.Text;
            string oper = Operator.Text;

            AccessHelper mdb = new AccessHelper(Global.Instance.DataBase);

            DataTable count = mdb.Find($"select * from ChangeOrder where OrderNo='{orderNo}' and Operator='{oper}' and OrderNum={orderNum}");

            bool result = true;
            //如果查出来没数据，需要将新方案保存
            if (count.Rows.Count == 0) 
            {
                string sql = $"insert into ChangeOrder (OrderNo,Operator,OrderNum) values ('{orderNo}','{oper}',{orderNum})";
                result = mdb.Add(sql);
            }
            //如果成功更新第账号权限条数据，用于更新记录上一次使用的数据
            if (result) 
            {
                Form3Entity form3Entity = new Form3Entity();
                form3Entity.GDH = orderNo;
                form3Entity.GDSL = int.Parse(orderNum);
                form3Entity.CZY = oper;

                ValueSelected?.Invoke(form3Entity);
                this.Close();
            }
            else
            {
                MessageBox.Show("切换失败");
            }
            
        }


        private void AddComboBoxItems(ComboBox comboBox, DataTable datas, int limit=8)
        {
            foreach (DataRow row in datas.Rows)
            {
                if (limit-- == 0) break;
                string text = $"{row["OrderNo"]};{row["Operator"]};{row["OrderNum"]}";
                comboBox.Items.Add(text);
            }
        }


        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string text = comboBox.Text;
            if (comboBox.Text == "")
            {
                OrderNum.Text = "";
                OrderNo.Text = "";
                Operator.Text = "";
                OrderNum.ReadOnly = false;
                OrderNo.ReadOnly = false;
                Operator.ReadOnly = false;
                DeleteOrder.Visible = false;
                return;
            }
            if (comboBox.Text.Split(';').Length == 3) return;

            int cursor = comboBox.SelectionStart;
            comboBox.Items.Clear();
            comboBox.SelectionStart = cursor;
            string sql = $"select * from ChangeOrder where OrderNo = '{comboBox.Text}' and Operator='{Operator.Text}'";
            DataTable orderNums = mdb.Find(sql);
            if(orderNums.Rows.Count == 0)
            {
                sql = $"select * from ChangeOrder where OrderNo like '{comboBox.Text}%' and Operator='{Operator.Text}'";
                orderNums = mdb.Find(sql);
                comboBox.SelectionStart = cursor;
            }
            AddComboBoxItems(comboBox, orderNums);
            if (orderNums.Rows.Count != 0)
            {
                comboBox.SelectedIndex = -1;
                comboBox.DroppedDown = true;
                comboBox.Text = text;
                this.Cursor = System.Windows.Forms.Cursors.Default;
            }
            comboBox.SelectionStart = cursor;
            
        }

        private void OrderNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] arr = History.Text.Split(';');
            if(arr.Length == 3)
            {
                OrderNo.Text = arr[0];
                Operator.Text = arr[1];
                OrderNum.Text = arr[2];
                //OrderNum.ReadOnly = true;
                //OrderNo.ReadOnly = true;
                //Operator.ReadOnly = true;
                DeleteOrder.Visible = true;
            }

        }

        private void DeleteOrder_Click(object sender, EventArgs e)
        {
            string sql = $"delete from ChangeOrder where OrderNo='{OrderNo.Text}' and Operator='{Operator.Text}' and OrderNum={OrderNum.Text}";
            if (mdb.Del(sql))
            {
                OrderNum.Text = "";
                OrderNo.Text = "";
                Operator.Text = "";
                History.Items.Clear();
                History.Text = "";
                DeleteOrder.Visible = false;
            }
            else
            {
                MessageBox.Show("删除失败");
            }
        }

        private void History_DropDown(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox.Text == "")
            {
                comboBox.Items.Clear();
                string sql = $"select * from ChangeOrder where Operator='{Global.Instance.LoginMessage.WorkId}'";
                DataTable datas = mdb.Find(sql);
                AddComboBoxItems(comboBox, datas);
            }
        }
    }
}

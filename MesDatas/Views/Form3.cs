using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MesDatas.Models;
using MesDatas.Services;
using MesDatas.Utility;

namespace MesDatas.Views
{
    public partial class Form3 : Form
    {

        public delegate void ValueSelectedEventHandler(Form3Entity selectedValue);
        public event ValueSelectedEventHandler ValueSelected;

        private readonly AccessHelper mdb = new AccessHelper(Global.Instance.DataBase);
        private readonly WorkOrderHistoryStore _workOrderHistoryStore;

        public Form3()
        {
            InitializeComponent();
            _workOrderHistoryStore = new WorkOrderHistoryStore(mdb);
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
            string orderNo = OrderNo.Text.Trim();
            string oper = Operator.Text.Trim();
            int orderQuantity;
            int.TryParse(OrderNum.Text, out orderQuantity);

            // 允许工单号为空进行生产，操作员仍必填。
            if (string.IsNullOrWhiteSpace(oper))
            {
                MessageBox.Show("操作员不能为空！");
                return;
            }

            if (!_workOrderHistoryStore.SaveRecentOrder(orderNo, oper, orderQuantity))
            {
                MessageBox.Show("切换失败");
                return;
            }

            ValueSelected?.Invoke(new Form3Entity
            {
                GDH = orderNo,
                GDSL = orderQuantity,
                CZY = oper
            });
            Close();
        }

        private static void AddComboBoxItems(ComboBox comboBox, IEnumerable<Form3Entity> orders)
        {
            foreach (Form3Entity order in orders)
            {
                comboBox.Items.Add($"{order.GDH};{order.CZY};{order.GDSL}");
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
                Operator.Text = Global.Instance.LoginMessage.WorkId;
                OrderNum.ReadOnly = false;
                OrderNo.ReadOnly = false;
                Operator.ReadOnly = true;
                DeleteOrder.Visible = false;
                return;
            }
            if (comboBox.Text.Split(';').Length == 3) return;

            int cursor = comboBox.SelectionStart;
            comboBox.Items.Clear();
            comboBox.SelectionStart = cursor;
            List<Form3Entity> orders = _workOrderHistoryStore.GetRecentOrders(Operator.Text, text);
            AddComboBoxItems(comboBox, orders);
            if (orders.Count != 0)
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
            int orderQuantity;
            int.TryParse(OrderNum.Text, out orderQuantity);
            if (_workOrderHistoryStore.DeleteOrder(OrderNo.Text, Operator.Text, orderQuantity))
            {
                OrderNum.Text = "";
                OrderNo.Text = "";
                Operator.Text = Global.Instance.LoginMessage.WorkId;
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
                AddComboBoxItems(
                    comboBox,
                    _workOrderHistoryStore.GetRecentOrders(Global.Instance.LoginMessage.WorkId));
            }
        }
    }
}

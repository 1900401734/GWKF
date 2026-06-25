using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using MesDatas.Utility;

namespace MesDatas.Views
{
    public partial class Login : Form
    {
        AccessHelper mdb = new AccessHelper(Global.Instance.SourceDataBase);

        Dictionary<string, int> privilegeRefer = new Dictionary<string, int>
        {
            {"管理员", 1 },
            {"技术员", 2 },
            {"作业员", 3 },
        };

        public Login()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataTable result = mdb.Find($"select * from userinfo where work_id='{user.Text}' and pwd='{pwd.Text}'");
            if (result.Rows.Count == 0)
            {
                MessageBox.Show("账号或密码错误");
                return;
            }
            Global.Instance.LoginMessage = new Models.LoginUserEntity()
            {
                WorkId = user.Text,
                Pwd = pwd.Text,
                Privilege = privilegeRefer[result.Rows[0]["privilege"].ToString()],
            };
            this.Hide();
            Form1 form1 = new Form1();
            form1.Show();
        }
    }
}

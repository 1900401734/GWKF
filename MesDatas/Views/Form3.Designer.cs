
namespace MesDatas.Views
{
    partial class Form3
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Operator = new System.Windows.Forms.TextBox();
            this.History = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.OrderNo = new System.Windows.Forms.TextBox();
            this.OrderNum = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.DeleteOrder = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(81, 96);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 24);
            this.label1.TabIndex = 5;
            this.label1.Text = "工单号";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.Control;
            this.label3.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(81, 139);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 24);
            this.label3.TabIndex = 7;
            this.label3.Text = "操作员";
            // 
            // Operator
            // 
            this.Operator.BackColor = System.Drawing.Color.Silver;
            this.Operator.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold);
            this.Operator.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Operator.Location = new System.Drawing.Point(167, 137);
            this.Operator.Name = "Operator";
            this.Operator.ReadOnly = true;
            this.Operator.Size = new System.Drawing.Size(237, 35);
            this.Operator.TabIndex = 2;
            // 
            // History
            // 
            this.History.Font = new System.Drawing.Font("微软雅黑", 13F);
            this.History.FormattingEnabled = true;
            this.History.IntegralHeight = false;
            this.History.Location = new System.Drawing.Point(121, 23);
            this.History.Name = "History";
            this.History.Size = new System.Drawing.Size(283, 31);
            this.History.TabIndex = 0;
            this.History.DropDown += new System.EventHandler(this.History_DropDown);
            this.History.SelectedIndexChanged += new System.EventHandler(this.OrderNo_SelectedIndexChanged);
            this.History.TextChanged += new System.EventHandler(this.comboBox1_TextChanged);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button1.Location = new System.Drawing.Point(192, 244);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(93, 42);
            this.button1.TabIndex = 10;
            this.button1.Text = "确定";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // OrderNo
            // 
            this.OrderNo.BackColor = System.Drawing.SystemColors.Window;
            this.OrderNo.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold);
            this.OrderNo.ForeColor = System.Drawing.SystemColors.ControlText;
            this.OrderNo.Location = new System.Drawing.Point(167, 93);
            this.OrderNo.Name = "OrderNo";
            this.OrderNo.Size = new System.Drawing.Size(237, 35);
            this.OrderNo.TabIndex = 1;
            // 
            // OrderNum
            // 
            this.OrderNum.BackColor = System.Drawing.SystemColors.Window;
            this.OrderNum.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold);
            this.OrderNum.ForeColor = System.Drawing.SystemColors.ControlText;
            this.OrderNum.Location = new System.Drawing.Point(167, 183);
            this.OrderNum.Name = "OrderNum";
            this.OrderNum.Size = new System.Drawing.Size(237, 35);
            this.OrderNum.TabIndex = 3;
            this.OrderNum.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnlyAllowDigital_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(54, 185);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(110, 24);
            this.label2.TabIndex = 15;
            this.label2.Text = "工单数量";
            // 
            // DeleteOrder
            // 
            this.DeleteOrder.Font = new System.Drawing.Font("微软雅黑", 12.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.DeleteOrder.Location = new System.Drawing.Point(410, 21);
            this.DeleteOrder.Name = "DeleteOrder";
            this.DeleteOrder.Size = new System.Drawing.Size(53, 32);
            this.DeleteOrder.TabIndex = 16;
            this.DeleteOrder.Text = "删除";
            this.DeleteOrder.UseVisualStyleBackColor = true;
            this.DeleteOrder.Visible = false;
            this.DeleteOrder.Click += new System.EventHandler(this.DeleteOrder_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.History);
            this.groupBox1.Controls.Add(this.DeleteOrder);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(482, 59);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "搜索";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.SystemColors.Control;
            this.label4.Font = new System.Drawing.Font("宋体", 16F, System.Drawing.FontStyle.Bold);
            this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(36, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 22);
            this.label4.TabIndex = 17;
            this.label4.Text = "工单号";
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 298);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.OrderNum);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.OrderNo);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Operator);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(498, 337);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(498, 337);
            this.Name = "Form3";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "设备数据采集系统";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Operator;
        private System.Windows.Forms.ComboBox History;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox OrderNo;
        private System.Windows.Forms.TextBox OrderNum;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button DeleteOrder;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
    }
}
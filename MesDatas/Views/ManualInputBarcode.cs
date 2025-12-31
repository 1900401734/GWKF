using System;
using System.Windows.Forms;
using HslCommunication;
using HslCommunication.Core;
using MesDatas.DataAcess;

namespace MesDatas.Views
{
    public partial class ManualInputBarcode : Form
    {
        IReadWriteNet readWriteNet;
        PlcAddressInfo sendAddr;

        public ManualInputBarcode(IReadWriteNet readWriteNet, PlcAddressInfo sendAddr)
        {
            this.readWriteNet = readWriteNet;
            this.sendAddr = sendAddr;

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string barcode = txtBarcode.Text;

            if (string.IsNullOrEmpty(barcode))
            {
                MessageBox.Show("请输入条码");
                return;
            }

            OperateResult result;

            result = readWriteNet.Write(sendAddr.ManualInputBarcode, barcode);
            if (result.IsSuccess)
            {
                MessageBox.Show("写入成功");
            }
            else
            {
                MessageBox.Show("写入失败，请检查PLC连接");
            }
            this.Close();
        }
    }
}

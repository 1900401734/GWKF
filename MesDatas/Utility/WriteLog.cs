using System;
using System.IO;
using System.Windows.Forms;

namespace MesDatas.Utility
{
    class WriteLog
    {

        /// <summary>
        /// 写入一行日志到组件
        /// </summary>
        /// <param name="richtextBox"></param>
        /// <param name="LogMessage"></param>
        /// <param name="ClearLength"></param>
        public void WriteLogToComponent(RichTextBox richtextBox, string LogMessage, int ClearLength = 2000)
        {
            // 检查是否需要跨线程调用  
            if (richtextBox.InvokeRequired)
            {
                // 使用BeginInvoke在UI线程上异步执行添加行的操作  
                richtextBox.BeginInvoke(new MethodInvoker(delegate
                {
                    _WriteLogToComponent(richtextBox, LogMessage, ClearLength);
                }));
            }
            else
            {
                _WriteLogToComponent(richtextBox, LogMessage, ClearLength);
            }
        }

        /// <summary>
        /// 执行实际的日志写入逻辑。
        /// <para>包含重复日志合并逻辑：如果新日志与上一条内容相同，只更新时间戳。</para>
        /// </summary>
        /// <param name="richTextBox">目标控件</param>
        /// <param name="logMessage">日志内容</param>
        /// <param name="maxLineCount">最大行数限制</param>
        private void _WriteLogToComponent(RichTextBox richTextBox, string logMessage, int maxLineCount)
        {
            try
            {
                // 行数限制检查
                int lineHeight = richTextBox.Lines.Length;

                if (lineHeight > maxLineCount)
                {
                    richTextBox.Clear();
                    return;
                }

                // 构造日志内容
                if (lineHeight > 0 && richTextBox.Lines[0].EndsWith(logMessage))
                {
                    // 设置SelectionStart为0以选中首行文本
                    richTextBox.SelectionStart = 0;

                    // 将SelectionLength设置为控件中第一个换行符之前的字符数
                    // 如果没有换行符，就设置为控件中所有文本的长度
                    int length = richTextBox.Text.IndexOf("\n") + 1;
                    if (length == -1)
                    {
                        length = richTextBox.Text.Length;
                    }
                    richTextBox.SelectionLength = length;

                    // 选中文本后，使用SelectedText属性修改文本内容
                    richTextBox.SelectedText = $"【{System.DateTime.Now}】" + logMessage + "\n";

                    // 重置选择区域
                    richTextBox.SelectionStart = 0;
                    richTextBox.SelectionLength = 0;
                }
                else
                {
                    richTextBox.SelectionStart = 0;
                    var log = $"【{System.DateTime.Now}】" + logMessage + "\n";
                    richTextBox.SelectedText = log;
                    Log4netHelper.Debug($"流程：{richTextBox.Name} 日志：{log}");
                }
            }
            catch (Exception ex)
            {
            }
        }

        private readonly object AccessLogLock = new object();
        public void writeTxtLogToLocal(string filePath, string fileName, string Content, string header)
        {
            lock (AccessLogLock)
            {
                Directory.CreateDirectory(filePath);
                using (StreamWriter txtWrite = new StreamWriter(Path.Combine(filePath, fileName), true, encoding: System.Text.Encoding.UTF8))
                {
                    txtWrite.WriteLine(header);
                    txtWrite.Write(Content + "\n");
                }
            }
        }
    }
}

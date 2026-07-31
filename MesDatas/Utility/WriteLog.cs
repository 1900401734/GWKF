using MesDatas.MyEnum;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;

namespace MesDatas.Utility
{
    public static class WriteLog
    {
        /*/// <summary>
        /// 写入一行日志到组件
        /// </summary>
        /// <param name="richtextBox"></param>
        /// <param name="LogMessage"></param>
        /// <param name="ClearLength"></param>
        public void AppendToComponent(RichTextBox richtextBox, string LogMessage, int ClearLength = 2000)
        {
            // 检查是否需要跨线程调用  
            if (richtextBox.InvokeRequired)
            {
                // 使用BeginInvoke在UI线程上异步执行添加行的操作  
                richtextBox.BeginInvoke(new MethodInvoker(delegate
                {
                    _WriteAppendToComponent(richtextBox, LogMessage, ClearLength);
                }));
            }
            else
            {
                _WriteAppendToComponent(richtextBox, LogMessage, ClearLength);
            }
        }

        /// <summary>
        /// 执行实际的日志写入逻辑。
        /// <para>包含重复日志合并逻辑：如果新日志与上一条内容相同，只更新时间戳。</para>
        /// </summary>
        /// <param name="richTextBox">目标控件</param>
        /// <param name="logMessage">日志内容</param>
        /// <param name="maxLineCount">最大行数限制</param>
        private void _WriteAppendToComponent(RichTextBox richTextBox, string logMessage, int maxLineCount)
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
                    var DefaultLogger = $"【{System.DateTime.Now}】" + logMessage + "\n";
                    richTextBox.SelectedText = DefaultLogger;
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
        }*/

        // 复用StringBuilder以减少内存分配
        [ThreadStatic]
        private static StringBuilder _logBuilder;

        private const int UiLogBatchSize = 25;
        private const int UiLogMaxQueueLength = 500;
        private static readonly object UiLogQueueLock = new object();
        private static readonly Dictionary<RichTextBox, UiLogQueueState> UiLogQueues = new Dictionary<RichTextBox, UiLogQueueState>();

        /// <summary>
        /// 单个 RichTextBox 对应的日志队列状态。
        /// </summary>
        private sealed class UiLogQueueState
        {
            public readonly Queue<UiLogItem> Items = new Queue<UiLogItem>();
            public bool IsFlushScheduled;
            public int? MaxLineCount;
        }

        /// <summary>
        /// 等待刷新到 UI 的一条日志。
        /// </summary>
        private sealed class UiLogItem
        {
            public string Message { get; set; }
            public int ClearLength { get; set; }
            public DateTime OccurredAt { get; set; }
            /// <summary>原样写入（不加时间戳、不去重），供统一流程日志使用。</summary>
            public bool IsRaw { get; set; }
        }

        /// <summary>
        /// 设置指定日志控件保留的最大行数。
        /// </summary>
        public static void SetUiLogLineLimit(this RichTextBox richTextBox, int maxLineCount)
        {
            if (richTextBox == null)
                return;

            if (maxLineCount < 1)
                throw new ArgumentOutOfRangeException(nameof(maxLineCount));

            lock (UiLogQueueLock)
            {
                if (!UiLogQueues.TryGetValue(richTextBox, out UiLogQueueState state))
                {
                    state = new UiLogQueueState();
                    UiLogQueues[richTextBox] = state;
                }

                state.MaxLineCount = maxLineCount;
            }
        }

        /// <summary>
        /// 写入一行日志到组件
        /// </summary>
        /// <param name="richtextBox"></param>
        /// <param name="LogMessage"></param>
        /// <param name="ClearLength"></param>
        public static void AppendToComponent(this RichTextBox richtextBox, string LogMessage, int ClearLength = 2000)
        {
            if (richtextBox == null || richtextBox.IsDisposed)
                return;

            int maxLineCount = GetUiLogLineLimit(richtextBox, ClearLength);
            string logMessage = LogMessage ?? string.Empty;
            // 所有线程统一入队，避免 UI 直写与后台延迟刷新交错后破坏时间顺序。
            EnqueueUiLog(richtextBox, logMessage, maxLineCount, DateTime.Now);
        }

        /// <summary>
        /// 原样追加一行到组件（不加时间戳、不去重），供统一流程日志使用。
        /// <para>调用方应自行拼好「时间 消息」整行；传入空串即写入一个空行作为流程分隔。</para>
        /// </summary>
        public static void AppendRaw(this RichTextBox richtextBox, string line, int clearLength = 2000)
        {
            if (richtextBox == null || richtextBox.IsDisposed)
                return;

            int maxLineCount = GetUiLogLineLimit(richtextBox, clearLength);
            EnqueueUiLog(richtextBox, line ?? string.Empty, maxLineCount, DateTime.Now, isRaw: true);
        }

        private static int GetUiLogLineLimit(RichTextBox richTextBox, int fallbackLineCount)
        {
            lock (UiLogQueueLock)
            {
                if (UiLogQueues.TryGetValue(richTextBox, out UiLogQueueState state) && state.MaxLineCount.HasValue)
                    return state.MaxLineCount.Value;
            }

            return Math.Max(1, fallbackLineCount);
        }

        /// <summary>
        /// 将跨线程日志加入待刷新队列。
        /// </summary>
        private static void EnqueueUiLog(RichTextBox richTextBox, string logMessage, int clearLength, DateTime occurredAt, bool isRaw = false)
        {
            bool shouldSchedule = false;

            lock (UiLogQueueLock)
            {
                if (!UiLogQueues.TryGetValue(richTextBox, out UiLogQueueState state))
                {
                    state = new UiLogQueueState();
                    UiLogQueues[richTextBox] = state;
                }

                while (state.Items.Count >= UiLogMaxQueueLength)
                    state.Items.Dequeue();

                state.Items.Enqueue(new UiLogItem
                {
                    Message = logMessage,
                    ClearLength = clearLength,
                    OccurredAt = occurredAt,
                    IsRaw = isRaw
                });

                if (!state.IsFlushScheduled)
                {
                    state.IsFlushScheduled = true;
                    shouldSchedule = true;
                }
            }

            if (shouldSchedule)
                ScheduleUiLogFlush(richTextBox);
        }

        /// <summary>
        /// 安排 UI 线程异步刷新日志队列。
        /// </summary>
        private static void ScheduleUiLogFlush(RichTextBox richTextBox)
        {
            try
            {
                if (!richTextBox.IsHandleCreated || richTextBox.IsDisposed)
                {
                    ResetUiLogSchedule(richTextBox);
                    return;
                }

                richTextBox.BeginInvoke((MethodInvoker)(() => FlushUiLogQueue(richTextBox)));
            }
            catch (ObjectDisposedException)
            {
                RemoveUiLogQueue(richTextBox);
            }
            catch (InvalidOperationException)
            {
                ResetUiLogSchedule(richTextBox);
            }
        }

        /// <summary>
        /// UI 线程批量刷新日志队列。
        /// </summary>
        private static void FlushUiLogQueue(RichTextBox richTextBox)
        {
            if (richTextBox == null || richTextBox.IsDisposed)
            {
                RemoveUiLogQueue(richTextBox);
                return;
            }

            bool queueDrained = false;
            int flushCount = 0;
            richTextBox.SuspendLayout();
            try
            {
                while (flushCount < UiLogBatchSize)
                {
                    UiLogItem item;

                    lock (UiLogQueueLock)
                    {
                        if (!UiLogQueues.TryGetValue(richTextBox, out UiLogQueueState state) || state.Items.Count == 0)
                        {
                            if (state != null)
                                state.IsFlushScheduled = false;

                            queueDrained = true;
                            break;
                        }

                        item = state.Items.Dequeue();
                    }

                    if (item.IsRaw)
                        _WriteRawToComponent(richTextBox, item.Message, item.ClearLength, manageLayout: false);
                    else
                        _WriteAppendToComponent(richTextBox, item.Message, item.ClearLength, item.OccurredAt, manageLayout: false);
                    flushCount++;
                }
            }
            finally
            {
                richTextBox.ResumeLayout();
            }

            if (queueDrained)
                return;

            bool hasMore;
            lock (UiLogQueueLock)
            {
                hasMore = UiLogQueues.TryGetValue(richTextBox, out UiLogQueueState state) && state.Items.Count > 0;
                if (!hasMore && state != null)
                    state.IsFlushScheduled = false;
            }

            if (hasMore)
                ScheduleUiLogFlush(richTextBox);
        }

        private static void ResetUiLogSchedule(RichTextBox richTextBox)
        {
            lock (UiLogQueueLock)
            {
                if (richTextBox != null && UiLogQueues.TryGetValue(richTextBox, out UiLogQueueState state))
                    state.IsFlushScheduled = false;
            }
        }

        private static void RemoveUiLogQueue(RichTextBox richTextBox)
        {
            lock (UiLogQueueLock)
            {
                if (richTextBox != null)
                    UiLogQueues.Remove(richTextBox);
            }
        }

        /// <summary>
        /// 执行实际的日志写入逻辑。
        /// <para>包含重复日志合并逻辑：如果新日志与上一条内容相同，只更新时间戳。</para>
        /// </summary>
        /// <param name="richTextBox">目标控件</param>
        /// <param name="logMessage">日志内容</param>
        /// <param name="maxLineCount">最大行数限制</param>
        private static void _WriteAppendToComponent(RichTextBox richTextBox, string logMessage, int maxLineCount, DateTime occurredAt, bool manageLayout = true)
        {
            try
            {
                if (manageLayout)
                    richTextBox.SuspendLayout();

                try
                {
                    int lineCount = GetUiLogLineCount(richTextBox);
                    if (lineCount >= maxLineCount)
                    {
                        richTextBox.Clear();
                        lineCount = 0;
                    }

                    if (_logBuilder == null)
                        _logBuilder = new StringBuilder(256);
                    else
                        _logBuilder.Clear();

                    _logBuilder.Append(occurredAt.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    _logBuilder.Append(' ');
                    _logBuilder.Append(logMessage);
                    _logBuilder.Append('\n');
                    string formattedLog = _logBuilder.ToString();

                    int firstLineLength = lineCount > 0 ? GetFirstLineLength(richTextBox) : 0;
                    bool isDuplicate = false;
                    if (firstLineLength > 0)
                    {
                        richTextBox.Select(0, firstLineLength);
                        string firstLine = richTextBox.SelectedText.TrimEnd('\r', '\n');
                        isDuplicate = firstLine.EndsWith(logMessage, StringComparison.Ordinal);
                    }

                    richTextBox.Select(0, isDuplicate ? firstLineLength : 0);
                    richTextBox.SelectedText = formattedLog;
                    richTextBox.Select(0, 0);
                }
                finally
                {
                    if (manageLayout)
                        richTextBox.ResumeLayout();
                }
            }
            catch (Exception)
            {
                // UI日志写入失败不再自动落盘，避免界面组件异常污染业务功能日志。
            }
        }

        /// <summary>
        /// 原样写入一行（不加时间戳、不去重），供统一流程日志使用。
        /// </summary>
        private static void _WriteRawToComponent(RichTextBox richTextBox, string line, int maxLineCount, bool manageLayout = true)
        {
            try
            {
                if (manageLayout)
                    richTextBox.SuspendLayout();

                try
                {
                    if (GetUiLogLineCount(richTextBox) >= maxLineCount)
                        richTextBox.Clear();

                    richTextBox.Select(0, 0);
                    richTextBox.SelectedText = line + "\n";
                    richTextBox.Select(0, 0);
                }
                finally
                {
                    if (manageLayout)
                        richTextBox.ResumeLayout();
                }
            }
            catch (Exception)
            {
            }
        }

        private static int GetUiLogLineCount(RichTextBox richTextBox)
        {
            return richTextBox.TextLength == 0
                ? 0
                : richTextBox.GetLineFromCharIndex(richTextBox.TextLength - 1) + 1;
        }

        private static int GetFirstLineLength(RichTextBox richTextBox)
        {
            int secondLineStart = richTextBox.GetFirstCharIndexFromLine(1);
            return secondLineStart >= 0 ? secondLineStart : richTextBox.TextLength;
        }

        //private readonly object AccessLogLock = new object();

        /// <summary>
        /// 写入文本日志到本地文件
        /// </summary>
        public static void writeTxtLogToLocal(string filePath, string fileName, string Content, string header)
        {
            //lock ()
            //{
            try
            {
                // 确保目录存在
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                string fullPath = Path.Combine(filePath, fileName);

                // 使用FileStream + StreamWriter优化性能
                using (FileStream fs = new FileStream(fullPath, FileMode.Append, FileAccess.Write, FileShare.Read,
                           4096))
                using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                {
                    writer.WriteLine(header);
                    writer.WriteLine(Content);
                }
            }
            catch (Exception)
            {
                // 这里是通用txt写入工具，业务侧需要落盘时应显式调用功能日志。
            }
            //}
        }
    }
}

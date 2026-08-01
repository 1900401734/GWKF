using MesDatas.MyEnum;
using System;
using System.Diagnostics;

namespace MesDatas.Utility
{
    /// <summary>
    /// 产品过站追踪上下文。
    /// <para>每次 PLC 触发上传时创建一个实例，用同一个 TraceId 串起过站、MES、PLC反馈和异常日志。</para>
    /// </summary>
    public sealed class ProductPassTraceContext
    {
        [ThreadStatic]
        private static string _currentTraceId;

        private readonly object _syncRoot = new object();
        private readonly Stopwatch _totalWatch;
        private bool _completed;
        private bool _pendingError;

        /// <summary>
        /// 当前线程正在处理的产品过站 TraceId，供 MES 工具类记录同一链路。
        /// </summary>
        public static string CurrentTraceId => _currentTraceId;

        /// <summary>
        /// 统一流程日志的 UI 输出槽。
        /// <para>单窗体应用，Form1 启动时注册一次（<c>UiSink = line => UploadMes.AppendRaw(line)</c>）；
        /// 主流程与后台任务共用，把同一行「时间 消息」原样写到界面。</para>
        /// </summary>
        public static Action<string> UiSink { get; set; }

        /// <summary>
        /// 整条过站链路从触发到当前的耗时（毫秒）。
        /// </summary>
        public long TotalElapsedMs => _totalWatch.ElapsedMilliseconds;

        /// <summary>
        /// 本次过站的唯一追踪编号。
        /// </summary>
        public string TraceId { get; }

        /// <summary>
        /// 当前工序名称。
        /// </summary>
        public ProcessName ProcessName { get; }

        /// <summary>
        /// PLC 触发上传的地址。
        /// </summary>
        public string TriggerPoint { get; }

        /// <summary>
        /// 上位机反馈给 PLC 的地址。
        /// </summary>
        public string FeedbackPoint { get; }

        private ProductPassTraceContext(ProcessName processName, string triggerPoint, string feedbackPoint)
        {
            TraceId = $"{DateTime.Now:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}".Substring(0, 26);
            ProcessName = processName;
            TriggerPoint = triggerPoint;
            FeedbackPoint = feedbackPoint;
            _totalWatch = Stopwatch.StartNew();
        }

        /// <summary>
        /// 创建一次新的产品过站追踪。
        /// </summary>
        public static ProductPassTraceContext Start(ProcessName processName, string triggerPoint, string feedbackPoint)
        {
            return new ProductPassTraceContext(processName, triggerPoint, feedbackPoint);
        }

        /// <summary>
        /// 将当前 TraceId 绑定到线程，方便下游同步调用记录同一链路。
        /// </summary>
        public IDisposable EnterScope()
        {
            string oldTraceId = _currentTraceId;
            _currentTraceId = TraceId;
            return new TraceScope(oldTraceId);
        }

        /// <summary>
        /// 写入一行统一流程日志：同一行「时间 消息」同时落 UI 与产品过站文件，逐字一致。
        /// </summary>
        /// <param name="message">不含时间戳的消息；为空时两侧都写一个空行（流程间分隔）。</param>
        private void WriteFlow(string message)
        {
            lock (_syncRoot)
            {
                if (_completed) return;
                WriteFlowCore(message);
            }
        }

        private void WriteFlowCore(string message)
        {
            string line = string.IsNullOrEmpty(message)
                ? string.Empty
                : $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {message}";
            UiSink?.Invoke(line);                       // UI 原样（含时间戳）
            Log4netHelper.LogProductPassLine(line);     // 文件原样（%m%n）；空串→空行
        }

        /// <summary>
        /// 记录一行流程日志（不含时间戳，内部统一拼接）。
        /// </summary>
        public void LogFlow(string message) => WriteFlow(message);

        /// <summary>
        /// 写入一个空行，用于分隔相邻的过站流程。
        /// </summary>
        public void LogFlowBlank() => WriteFlow(null);

        /// <summary>
        /// 记录一个带耗时的流程节点：<c>{label}{suffix}，耗时=Xms</c>。
        /// </summary>
        public void LogFlowElapsed(string label, Stopwatch watch, string suffix = null)
        {
            if (watch == null) return;
            if (watch.IsRunning) watch.Stop();
            WriteFlow($"{label}{suffix}，耗时={watch.ElapsedMilliseconds}ms");
        }

        /// <summary>
        /// 记录一个带耗时的流程节点（耗时由调用方给出，如整流程耗时）。
        /// </summary>
        public void LogFlowElapsedMs(string label, long elapsedMs, string suffix = null)
        {
            WriteFlow($"{label}{suffix}，耗时={elapsedMs}ms");
        }


        /// <summary>
        /// 记录 PLC 反馈成功，并结束本次产品过站日志块。
        /// </summary>
        public void CompleteFeedback(bool passed, short value)
        {
            lock (_syncRoot)
            {
                if (_completed) return;

                string result = passed ? "产品过站成功" : "产品过站失败";
                CompleteCore($"{result}，反馈{FeedbackPoint}={value}，总耗时={_totalWatch.ElapsedMilliseconds}ms");
            }
        }

        public void LogFeedbackWriteFailed(bool passed, short value, bool canRetry)
        {
            lock (_syncRoot)
            {
                if (_completed) return;

                string result = passed ? "产品过站成功" : "产品过站失败";
                WriteFlowCore($"{result}，但反馈{FeedbackPoint}={value}写入失败，总耗时={_totalWatch.ElapsedMilliseconds}ms");
                if (!canRetry)
                    CompleteCore(null);
            }
        }

        public void CompleteWithoutFeedback(bool passed)
        {
            lock (_syncRoot)
            {
                if (_completed) return;

                string result = passed ? "产品过站成功" : "产品过站失败";
                CompleteCore($"{result}，未反馈，总耗时={_totalWatch.ElapsedMilliseconds}ms");
            }
        }

        internal void HandOffToError()
        {
            lock (_syncRoot)
            {
                if (!_completed)
                    _pendingError = true;
            }
        }

        /// <summary>
        /// 记录非流程的诊断/排错信息到数据异常日志。
        /// </summary>
        public void Diag(string action, string message, Exception ex = null)
        {
            Log4netHelper.LogDataException(action, message, exception: ex, level: ex == null ? "INFO" : "ERROR");
        }

        /// <summary>
        /// 记录整条产品过站链路结束：停表并写入一个空行分隔下一流程。
        /// </summary>
        public void Finish(string barcode, string result)
        {
            lock (_syncRoot)
            {
                if (_completed || _pendingError) return;
                CompleteCore($"产品过站失败，未反馈，总耗时={_totalWatch.ElapsedMilliseconds}ms");
            }
        }

        private void CompleteCore(string finalMessage)
        {
            if (!string.IsNullOrEmpty(finalMessage))
                WriteFlowCore(finalMessage);

            _totalWatch.Stop();
            _completed = true;
            WriteFlowCore(string.Empty);
        }

        private sealed class TraceScope : IDisposable
        {
            private readonly string _oldTraceId;
            private bool _disposed;

            public TraceScope(string oldTraceId)
            {
                _oldTraceId = oldTraceId;
            }

            public void Dispose()
            {
                if (_disposed) return;
                _currentTraceId = _oldTraceId;
                _disposed = true;
            }
        }
    }
}

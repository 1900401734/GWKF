using MesDatas.MyEnum;
using System;
using System.Diagnostics;

namespace MesDatas.Utility
{
    /// <summary>
    /// 产品过站追踪上下文。
    /// <para>每次 PLC 触发上传时创建一个实例，用同一个 TraceId 串起整条过站链路。</para>
    /// </summary>
    public sealed class ProductPassTraceContext
    {
        [ThreadStatic]
        private static string _currentTraceId;

        private readonly Stopwatch _totalWatch;

        /// <summary>
        /// 当前线程正在处理的产品过站 TraceId，供 HTTP 工具类记录 MES 明细耗时。
        /// </summary>
        public static string CurrentTraceId => _currentTraceId;

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
            var trace = new ProductPassTraceContext(processName, triggerPoint, feedbackPoint);
            trace.Log($"开始产品过站追踪，触发地址={triggerPoint}，反馈地址={feedbackPoint}");
            return trace;
        }

        /// <summary>
        /// 将当前 TraceId 绑定到线程，便于下游同步调用记录同一链路。
        /// </summary>
        public IDisposable EnterScope()
        {
            string oldTraceId = _currentTraceId;
            _currentTraceId = TraceId;
            return new TraceScope(oldTraceId);
        }

        /// <summary>
        /// 记录普通链路节点。
        /// </summary>
        public void Log(string message)
        {
            Log4netHelper.LogProductPass(BuildMessage(message));
        }

        /// <summary>
        /// 记录一个阶段耗时。
        /// </summary>
        public void LogElapsed(string stageName, Stopwatch watch)
        {
            if (watch == null) return;
            if (watch.IsRunning) watch.Stop();
            Log($"{stageName}={watch.ElapsedMilliseconds}ms");
        }

        /// <summary>
        /// 记录一个阶段耗时。
        /// </summary>
        public void LogElapsed(string stageName, TimeSpan elapsed)
        {
            Log($"{stageName}={(long)elapsed.TotalMilliseconds}ms");
        }

        /// <summary>
        /// 记录整条链路结束。
        /// </summary>
        public void Finish(string barcode, string result)
        {
            if (_totalWatch.IsRunning) _totalWatch.Stop();
            Log($"产品过站追踪结束，条码={barcode ?? string.Empty}，结果={result ?? string.Empty}，总耗时={_totalWatch.ElapsedMilliseconds}ms");
        }

        private string BuildMessage(string message)
        {
            return $"TraceId={TraceId} 工序={ProcessName} 触发={TriggerPoint} 反馈={FeedbackPoint} {message}";
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

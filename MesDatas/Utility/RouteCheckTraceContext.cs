using System;
using System.Diagnostics;

namespace MesDatas.Utility
{
    internal sealed class RouteCheckTraceContext
    {
        private readonly object _syncRoot = new object();
        private readonly Stopwatch _totalWatch = Stopwatch.StartNew();
        private readonly Action<string> _uiSink;
        private bool _completed;

        private RouteCheckTraceContext(string triggerAddress, string feedbackAddress, Action<string> uiSink)
        {
            TriggerAddress = triggerAddress;
            FeedbackAddress = feedbackAddress;
            _uiSink = uiSink;
        }

        public string TriggerAddress { get; }

        public string FeedbackAddress { get; }

        public static RouteCheckTraceContext Start(string triggerAddress, string feedbackAddress, Action<string> uiSink)
        {
            return new RouteCheckTraceContext(triggerAddress, feedbackAddress, uiSink);
        }

        public void LogFlow(string message)
        {
            lock (_syncRoot)
            {
                if (_completed) return;
                WriteLine(message);
            }
        }

        public void LogElapsed(string label, Stopwatch watch)
        {
            if (watch == null) return;
            if (watch.IsRunning) watch.Stop();
            LogFlow($"{label}，耗时={watch.ElapsedMilliseconds}ms");
        }

        public void CompleteFeedback(bool passed, short value, bool skipped = false)
        {
            lock (_syncRoot)
            {
                if (_completed) return;

                string result = skipped ? "流程检查已跳过" : passed ? "流程检查通过" : "流程检查未通过";
                WriteLine($"{result}，反馈{FeedbackAddress}={value}，总耗时={_totalWatch.ElapsedMilliseconds}ms");
                Complete();
            }
        }

        public void LogFeedbackWriteFailed(bool passed, short value, bool canRetry)
        {
            lock (_syncRoot)
            {
                if (_completed) return;

                string result = passed ? "流程检查通过" : "流程检查未通过";
                WriteLine($"{result}，但反馈{FeedbackAddress}={value}写入失败，总耗时={_totalWatch.ElapsedMilliseconds}ms");
                if (!canRetry) Complete();
            }
        }

        public void CompleteWithoutFeedback(bool passed)
        {
            lock (_syncRoot)
            {
                if (_completed) return;

                string result = passed ? "流程检查通过" : "流程检查未通过";
                WriteLine($"{result}，未反馈，总耗时={_totalWatch.ElapsedMilliseconds}ms");
                Complete();
            }
        }

        private void Complete()
        {
            _totalWatch.Stop();
            _completed = true;
            WriteLine(string.Empty);
        }

        private void WriteLine(string message)
        {
            string fullLine = string.IsNullOrEmpty(message)
                ? string.Empty
                : $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {message}";
            _uiSink?.Invoke(fullLine);
            Log4netHelper.LogRouteCheckLine(fullLine);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MesDatas.Utility
{
    /// <summary>
    /// 日志功能区。
    /// <para>功能区决定日志写入哪个文件夹，不再使用 Debug/Info/Error 等等级拆文件。</para>
    /// </summary>
    public enum LogArea
    {
        MesInteraction,
        LabelPrint,
        ProductPass,
        RouteCheck,
        Torque,
        DataException
    }

    /// <summary>
    /// log4net 业务日志入口。
    /// <para>所有正文统一输出为“时间 + 中文日志流”，避免 level/area/action 这类机器格式混入现场日志。</para>
    /// </summary>
    public static class Log4netHelper
    {
        private const int MaxReasonLength = 500;

        private static readonly log4net.ILog MesInteractionLogger = log4net.LogManager.GetLogger("MesInteractionLog");
        private static readonly log4net.ILog LabelPrintLogger = log4net.LogManager.GetLogger("LabelPrintLog");
        private static readonly log4net.ILog ProductPassLogger = log4net.LogManager.GetLogger("ProductPassLog");
        private static readonly log4net.ILog RouteCheckLogger = log4net.LogManager.GetLogger("RouteCheckLog");
        private static readonly log4net.ILog TorqueLogger = log4net.LogManager.GetLogger("TorqueLog");
        private static readonly log4net.ILog DataExceptionLogger = log4net.LogManager.GetLogger("DataExceptionLog");

        /// <summary>
        /// 记录 MES 请求、响应、Token 和接口异常。
        /// </summary>
        public static void LogMesInteraction(string action, object message = null, IDictionary<string, object> fields = null, Exception exception = null, string level = "INFO")
        {
            Write(LogArea.MesInteraction, level, action, message, fields, exception);
        }

        /// <summary>
        /// 原样写入一段 MES 交互块日志。
        /// <para>请求/响应原始报文不能经过通用字段清理，否则会被单行化或截断。</para>
        /// </summary>
        public static void LogMesInteractionBlock(string fullBlock, string level = "INFO", Exception exception = null)
        {
            string normalizedLevel = string.IsNullOrWhiteSpace(level) ? "INFO" : level.Trim().ToUpperInvariant();

            if (normalizedLevel == "ERROR")
                MesInteractionLogger.Error(fullBlock, exception);
            else if (normalizedLevel == "WARN")
                MesInteractionLogger.Warn(fullBlock, exception);
            else
                MesInteractionLogger.Info(fullBlock, exception);
        }

        /// <summary>
        /// 记录标签打印流程。
        /// </summary>
        public static void LogLabelPrint(string action, object message = null, IDictionary<string, object> fields = null, Exception exception = null, string level = "INFO")
        {
            Write(LogArea.LabelPrint, level, action, message, fields, exception);
        }

        /// <summary>
        /// 记录产品过站流程。
        /// </summary>
        public static void LogProductPass(string action, object message = null, IDictionary<string, object> fields = null, Exception exception = null, string level = "INFO")
        {
            Write(LogArea.ProductPass, level, action, message, fields, exception);
        }

        /// <summary>
        /// 以纯文本方式写入一条产品过站流程日志。
        /// <para>调用方已经拼好整行，直接原样落盘。</para>
        /// </summary>
        public static void LogProductPassLine(string fullLine)
        {
            ProductPassLogger.Info(fullLine);
        }

        /// <summary>
        /// 记录扫码、拼版、CHECKROUTE 等流程检查。
        /// </summary>
        public static void LogRouteCheck(string action, object message = null, IDictionary<string, object> fields = null, Exception exception = null, string level = "INFO")
        {
            Write(LogArea.RouteCheck, level, action, message, fields, exception);
        }

        /// <summary>
        /// 记录扭力控制器、扭力串口和峰值采集。
        /// </summary>
        public static void LogTorque(string action, object message = null, IDictionary<string, object> fields = null, Exception exception = null, string level = "INFO")
        {
            Write(LogArea.Torque, level, action, message, fields, exception);
        }

        /// <summary>
        /// 记录数据异常、PLC读写失败和业务阻塞报警。
        /// </summary>
        public static void LogDataException(string action, object message = null, IDictionary<string, object> fields = null, Exception exception = null, string level = "ERROR")
        {
            Write(LogArea.DataException, level, action, message, fields, exception);
        }

        /// <summary>
        /// 旧异常入口兼容包装：所有旧 Error 调用都归入数据异常。
        /// </summary>
        public static void Error(object message, Exception exception = null)
        {
            LogDataException("LEGACY_ERROR", message, exception: exception);
        }

        /// <summary>
        /// 写入指定功能区日志。
        /// </summary>
        private static void Write(LogArea area, string level, string action, object message, IDictionary<string, object> fields, Exception exception)
        {
            string normalizedLevel = string.IsNullOrWhiteSpace(level) ? "INFO" : level.Trim().ToUpperInvariant();
            string logLine = FormatFlowLog(area, action, message, fields, exception);
            log4net.ILog logger = GetLogger(area);

            if (normalizedLevel == "ERROR")
                logger.Error(logLine, exception);
            else if (normalizedLevel == "WARN")
                logger.Warn(logLine, exception);
            else
                logger.Info(logLine, exception);
        }

        /// <summary>
        /// 生成“时间 + 中文日志流”正文。
        /// </summary>
        private static string FormatFlowLog(LogArea area, string action, object message, IDictionary<string, object> fields, Exception exception)
        {
            return $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {BuildFlowMessage(area, action, message, fields, exception)}";
        }

        /// <summary>
        /// 根据关键业务动作生成现场可读日志。
        /// </summary>
        private static string BuildFlowMessage(LogArea area, string action, object message, IDictionary<string, object> fields, Exception exception)
        {
            string process = Field(fields, "process");
            string barcode = Field(fields, "barcode");
            string source = Field(fields, "source");
            string status = Field(fields, "status");
            string retryCount = Field(fields, "retryCount");
            string recordId = Field(fields, "recordId");
            string errorMessage = FirstText(Field(fields, "reason"), Field(fields, "errorMessage"), CleanLogText(message), exception?.Message);

            switch (action)
            {
                case "MES_OUTBOX_CREATE":
                    return JoinSegments(
                        ProcessPrefix(process, area),
                        "先反馈再上传记录已创建，等待MES后台确认",
                        Segment("来源", "本地"),
                        Segment("SN", barcode),
                        Segment("状态", status),
                        Segment("记录", recordId));

                case "MES_OUTBOX_CONFIRMED_PASS":
                    return JoinSegments(
                        ProcessPrefix(process, area),
                        "过站成功",
                        Segment("来源", "MES"),
                        Segment("SN", barcode),
                        Segment("状态", status),
                        Segment("重试次数", retryCount));

                case "MES_OUTBOX_CONFIRMED_FAIL":
                    return JoinSegments(
                        ProcessPrefix(process, area),
                        "过站失败",
                        Segment("来源", "MES"),
                        Segment("SN", barcode),
                        Segment("类型", Field(fields, "failureType")),
                        Segment("重复键", Field(fields, "duplicateKey")),
                        Segment("状态", status),
                        Segment("原因", errorMessage));

                case "MES_OUTBOX_PENDING_RETRY":
                    return JoinSegments(
                        ProcessPrefix(process, area),
                        "MES后台上传待重试",
                        Segment("来源", "网络/接口"),
                        Segment("SN", barcode),
                        Segment("状态", status),
                        Segment("重试次数", retryCount),
                        Segment("原因", errorMessage));

                case "MES_OUTBOX_MANUAL_PROCESSING":
                    return JoinSegments(
                        ProcessPrefix(process, area),
                        "MES后台上传转人工处理",
                        Segment("来源", "本地"),
                        Segment("SN", barcode),
                        Segment("状态", status),
                        Segment("原因", errorMessage));

                case "MES_SYNC_CONFIRMED_PASS":
                    return JoinSegments(
                        ProcessPrefix(process, area),
                        "过站成功",
                        Segment("来源", FirstText(source, "MES")),
                        Segment("SN", barcode));

                case "MES_SYNC_CONFIRMED_FAIL":
                    return JoinSegments(
                        ProcessPrefix(process, area),
                        "过站失败",
                        Segment("来源", FirstText(source, "MES")),
                        Segment("SN", barcode),
                        Segment("类型", Field(fields, "failureType")),
                        Segment("重复键", Field(fields, "duplicateKey")),
                        Segment("原因", errorMessage));

                case "PRINT_BLOCKED_BY_WEIGHT":
                    return JoinSegments(
                        "[工序:打印]",
                        "禁止打印",
                        Segment("来源", "本地拦截"),
                        Segment("SN", barcode),
                        Segment("前置工序", FirstText(Field(fields, "previousProcess"), "Weight")),
                        Segment("前置状态", Field(fields, "previousStatus")),
                        Segment("失败来源", FirstText(Field(fields, "failureSource"), "MES")),
                        Segment("原因", errorMessage));

                case "WEIGHT_FORBID_PRINT":
                    return JoinSegments(
                        "[工序:Weight]",
                        "禁止当前条码打印",
                        Segment("来源", "本地"),
                        Segment("原因", errorMessage));

                case "OFFLINE_BYPASS":
                    return JoinSegments(
                        ProcessPrefix(process, area),
                        "离线模式未上传MES，已按本地结果反馈PLC",
                        Segment("来源", "本地"),
                        Segment("SN", barcode),
                        Segment("结果", Field(fields, "result")),
                        Segment("反馈点", Field(fields, "feedback")));

                default:
                    return BuildDefaultMessage(area, action, message, fields, exception);
            }
        }

        /// <summary>
        /// 生成通用日志语句。
        /// </summary>
        private static string BuildDefaultMessage(LogArea area, string action, object message, IDictionary<string, object> fields, Exception exception)
        {
            string process = Field(fields, "process");
            var segments = new List<string>
            {
                ProcessPrefix(process, area),
                FirstText(CleanLogText(message), CleanLogText(action), "未指定动作")
            };

            foreach (KeyValuePair<string, object> item in fields ?? new Dictionary<string, object>())
            {
                if (ShouldSkipDefaultField(item.Key)) continue;
                segments.Add(Segment(GetDisplayName(item.Key), item.Value));
            }

            if (exception != null)
            {
                segments.Add(Segment("异常类型", exception.GetType().Name));
                segments.Add(Segment("异常", exception.Message));
            }

            return JoinSegments(segments.ToArray());
        }

        /// <summary>
        /// 默认日志中跳过已经体现在前缀或专用模板里的字段。
        /// </summary>
        private static bool ShouldSkipDefaultField(string key)
        {
            return string.Equals(key, "process", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 获取字段显示名。
        /// </summary>
        private static string GetDisplayName(string key)
        {
            switch (key)
            {
                case "barcode":
                    return "SN";
                case "source":
                    return "来源";
                case "status":
                    return "状态";
                case "result":
                    return "结果";
                case "errorMessage":
                case "reason":
                    return "原因";
                case "retryCount":
                    return "重试次数";
                case "recordId":
                    return "记录";
                case "trigger":
                    return "触发点";
                case "feedback":
                    return "反馈点";
                case "requiredStatus":
                    return "期望状态";
                case "previousProcess":
                    return "前置工序";
                case "previousStatus":
                    return "前置状态";
                case "failureSource":
                    return "失败来源";
                case "failureType":
                    return "类型";
                case "duplicateKey":
                    return "重复键";
                default:
                    return key;
            }
        }

        /// <summary>
        /// 获取字段文本。
        /// </summary>
        private static string Field(IDictionary<string, object> fields, string key)
        {
            if (fields == null) return string.Empty;
            foreach (KeyValuePair<string, object> item in fields)
            {
                if (string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase))
                    return CleanLogText(item.Value);
            }

            return string.Empty;
        }

        /// <summary>
        /// 获取第一个非空文本。
        /// </summary>
        private static string FirstText(params string[] values)
        {
            foreach (string value in values)
            {
                if (!string.IsNullOrWhiteSpace(value)) return CleanLogText(value);
            }

            return string.Empty;
        }

        /// <summary>
        /// 工序日志优先展示工序，否则展示功能区。
        /// </summary>
        private static string ProcessPrefix(string process, LogArea area)
        {
            return string.IsNullOrWhiteSpace(process)
                ? $"[{GetAreaName(area)}]"
                : $"[工序:{process}]";
        }

        /// <summary>
        /// 拼接非空日志片段。
        /// </summary>
        private static string JoinSegments(params string[] segments)
        {
            return string.Join("，", segments.Where(item => !string.IsNullOrWhiteSpace(item)));
        }

        /// <summary>
        /// 生成“名称=值”片段，值为空时跳过。
        /// </summary>
        private static string Segment(string name, object value)
        {
            string text = CleanLogText(value);
            return string.IsNullOrWhiteSpace(text) ? string.Empty : $"{name}={text}";
        }

        /// <summary>
        /// 清理日志文本，将MES多行错误压缩成现场可读的一行。
        /// </summary>
        private static string CleanLogText(object value)
        {
            if (value == null) return string.Empty;

            string text = value.ToString();
            text = text
                .Replace("\\r\\n", "；")
                .Replace("\\n", "；")
                .Replace("\\r", "；")
                .Replace("\r\n", "；")
                .Replace("\r", "；")
                .Replace("\n", "；")
                .Replace("\t", " ");
            text = Regex.Replace(text, "\\s+", " ").Trim(' ', '；');

            if (text.Length > MaxReasonLength)
                text = text.Substring(0, MaxReasonLength) + "...";

            return text;
        }

        /// <summary>
        /// 获取功能区对应的 log4net logger。
        /// </summary>
        private static log4net.ILog GetLogger(LogArea area)
        {
            switch (area)
            {
                case LogArea.MesInteraction:
                    return MesInteractionLogger;
                case LogArea.LabelPrint:
                    return LabelPrintLogger;
                case LogArea.ProductPass:
                    return ProductPassLogger;
                case LogArea.RouteCheck:
                    return RouteCheckLogger;
                case LogArea.Torque:
                    return TorqueLogger;
                case LogArea.DataException:
                    return DataExceptionLogger;
                default:
                    return DataExceptionLogger;
            }
        }

        /// <summary>
        /// 获取写入日志的中文功能区名称。
        /// </summary>
        private static string GetAreaName(LogArea area)
        {
            switch (area)
            {
                case LogArea.MesInteraction:
                    return "MES交互";
                case LogArea.LabelPrint:
                    return "标签打印";
                case LogArea.ProductPass:
                    return "产品过站";
                case LogArea.RouteCheck:
                    return "流程检查";
                case LogArea.Torque:
                    return "扭力检测";
                case LogArea.DataException:
                    return "数据异常";
                default:
                    return "数据异常";
            }
        }
    }
}

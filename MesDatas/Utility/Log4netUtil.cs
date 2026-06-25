using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesDatas.Utility
{
    public static class Log4netHelper
    {
        private static readonly log4net.ILog ExceptionLogger = log4net.LogManager.GetLogger("ExceptionLog");
        private static readonly log4net.ILog RouteCheckLogger = log4net.LogManager.GetLogger("RouteCheckLog");
        private static readonly log4net.ILog TorqueLogger = log4net.LogManager.GetLogger("TorqueLog");
        private static readonly log4net.ILog UploadLogger = log4net.LogManager.GetLogger("UploadLog");
        private static readonly log4net.ILog ProductPassLogger = log4net.LogManager.GetLogger("ProductPassLog");

        private static readonly log4net.ILog DefaultLogger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region 按业务逻辑区分

        /// <summary>
        /// 1. 异常日志
        /// </summary>
        public static void Error(object message, Exception exception = null)
        {
            if (exception == null)
                ExceptionLogger.Error(message);
            else
                ExceptionLogger.Error(message, exception);
        }

        /// <summary>
        /// 2. 流程检查日志
        /// </summary>
        public static void LogRouteCheck(object message)
        {
            RouteCheckLogger.Info(message);
        }

        /// <summary>
        /// 3. 扭力监测日志
        /// </summary>
        public static void LogTorque(object message)
        {
            TorqueLogger.Info(message);
        }

        /// <summary>
        /// 4. 数据上传日志
        /// </summary>
        public static void LogUpload(object message)
        {
            UploadLogger.Info(message);
        }

        /// <summary>
        /// 5. 产品过站日志
        /// </summary>
        public static void LogProductPass(object message)
        {
            ProductPassLogger.Info(message);
        }

        #endregion

        #region 按等级区分

        public static void Info(object message)
        {
            DefaultLogger.Info(message);
        }

        public static void Debug(object message)
        {
            DefaultLogger.Debug(message);
        }

        public static void Warn(object message)
        {
            DefaultLogger.Warn(message);
        }

        public static void Error(object message)
        {
            DefaultLogger.Error(message);
        }

        public static void Fatal(object message)
        {
            DefaultLogger.Fatal(message);
        }

        #endregion
    }
}

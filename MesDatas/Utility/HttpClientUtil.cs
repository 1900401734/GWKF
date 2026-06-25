using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MesDatas.Utility
{
    /*#region 异常类

    /// <summary>
    /// token错误异常
    /// </summary>
    public class TokenMissingException : Exception
    {
        public TokenMissingException(string message) : base(message)
        {
        }
    }

    /// <summary>
    /// 传递参数异常
    /// </summary>
    public class RecevieArgsException : Exception
    {
        public RecevieArgsException(string message) : base(message)
        {

        }
    }

    /// <summary>
    /// ftp异常
    /// </summary>
    public class FtpException : Exception
    {
        public FtpException(string message) : base(message)
        {

        }
    }

    /// <summary>
    /// 文件异常
    /// </summary>
    public class FileException : Exception
    {
        public FileException(string message) : base(message)
        {

        }
    }

    #endregion*/

    #region 异常类

    public class TokenMissingException : Exception { public TokenMissingException(string message) : base(message) { } }
    public class RecevieArgsException : Exception { public RecevieArgsException(string message) : base(message) { } }
    public class FtpException : Exception { public FtpException(string message) : base(message) { } }
    public class FileException : Exception { public FileException(string message) : base(message) { } }

    #endregion

    /// <summary>
    /// 请求接口工具类
    /// </summary>
    public class HttpClientUtil
    {
        #region 静态 HttpClient 单例及全局网络优化
        // 【核心优化1】HttpClient 必须作为静态单例使用，自带连接池，避免端口耗尽
        private static readonly HttpClient _httpClient;

        static HttpClientUtil()
        {
            // 【核心优化2】全局网络底层配置，解除各种引发卡顿的机制
            ServicePointManager.DefaultConnectionLimit = 512; // 突破并发2个的限制
            ServicePointManager.UseNagleAlgorithm = false;    // 禁用Nagle，加速小包发送
            ServicePointManager.Expect100Continue = false;    // 禁用 100-Continue 等待

            // 忽略 HTTPS 证书校验带来的延迟（如果是内网的话）
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            var handler = new HttpClientHandler
            {
                UseProxy = false, // 【最重要】禁用默认代理搜索，解决 5 秒超时的罪魁祸首
                ClientCertificateOptions = ClientCertificateOption.Manual
            };

            _httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(15) // 设置合理超时时间
            };
            _httpClient.DefaultRequestHeaders.ConnectionClose = false; // 启用 Keep-Alive 复用连接
        }
        #endregion

        #region 成员变量

        // 用于 API 接口通讯
        private string _url;
        private JObject getTotkenJsonData;
        private string token;
        private DateTime expiredtimeToken;

        // 用于 FTP 操作
        private string user;
        private string passwd;
        private string fileType;
        private string process;
        private string line;
        public string FtpDirPath;

        #endregion

        #region public方法

        public HttpClientUtil(JObject getTotkenJsonData, string tokenUrl)
        {
            this.getTotkenJsonData = getTotkenJsonData;
            _url = tokenUrl;
        }

        public HttpClientUtil(string url, string process, string line, string user, string passwd, string fileType)
        {
            Dictionary<string, string> dates = _GetDateForCreateDirectory();
            this.FtpDirPath = Path.Combine(url, process, line, dates["Y"], dates["Y-M"], dates["Y-M-D"], fileType);
            this.FtpDirPath = HandlerFtpPathString(this.FtpDirPath);

            this._url = url;
            this.process = process;
            this.line = line;
            this.user = user;
            this.passwd = passwd;
            this.fileType = fileType;
        }

        public JObject GetResponse(string url, string function, JObject inputParameterJson, string logFile, string responseSelectJsonPath = "//MesServiceJsonResult", int getTokenCount = 0)
        {
            // 【优化3】如果 Token 过期了，主动先获取 Token，避免一次无谓的失败请求
            if (token == null || DateTime.Now >= expiredtimeToken)
            {
                Stopwatch tokenWatch = Stopwatch.StartNew();
                GetToken(logFile);
                tokenWatch.Stop();
                LogProductPassMesDetail($"MES Token检查/刷新耗时={tokenWatch.ElapsedMilliseconds}ms，logFile={logFile}");
            }

            JObject jsonObject = _GetResponse(url, function, inputParameterJson, logFile, responseSelectJsonPath: responseSelectJsonPath);
            if (jsonObject is null) return null;

            if (jsonObject["ErrorCode"]?.ToString() == "10001" && getTokenCount < 3)
            {
                getTokenCount++;
                Stopwatch tokenWatch = Stopwatch.StartNew();
                GetToken(logFile); // 重新获取并递归
                tokenWatch.Stop();
                LogProductPassMesDetail($"MES Token失效后重新获取耗时={tokenWatch.ElapsedMilliseconds}ms，重试次数={getTokenCount}");
                return GetResponse(url, function, inputParameterJson, logFile, responseSelectJsonPath, getTokenCount: getTokenCount);
            }
            else return jsonObject;
        }

        #endregion

        #region private方法 (核心 HTTP 请求重构)

        /// <summary>
        /// 发起请求并获得请求的结果 (使用 HttpClient 替代 HttpWebRequest)
        /// </summary>
        private JObject _GetResponse(string url, string function, JObject inputParameterJson, string logFile, string responseSelectJsonPath = "//MesServiceJsonResult", bool getToken = false)
        {
            string timeFormat = "yyyy-MM-dd HH:mm:ss.fff"; // 增加毫秒级记录
            string log = "时间:{0}  url地址:{1}  类型:{2}  数据:{3}";

            // 生成 XML 请求体
            string requestXml = GenerateXml(inputParameterJson, function, !getToken, this.token ?? "未获取到token");

            string sendTime = DateTime.Now.ToString(timeFormat);
            Stopwatch totalWatch = Stopwatch.StartNew();

            try
            {
                // 使用 StringContent 包装 XML 数据
                using (var content = new StringContent(requestXml, Encoding.UTF8, "text/xml"))
                {
                    // 使用 GetAwaiter().GetResult() 来同步等待异步结果，兼容现有老代码架构
                    Stopwatch postWatch = Stopwatch.StartNew();
                    HttpResponseMessage response = _httpClient.PostAsync(url, content).GetAwaiter().GetResult();
                    postWatch.Stop();

                    string receiveTime = DateTime.Now.ToString(timeFormat);

                    // 读取返回的字符串结果
                    Stopwatch readWatch = Stopwatch.StartNew();
                    string recvXml = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    readWatch.Stop();

                    // 解析为 JObject
                    Stopwatch parseWatch = Stopwatch.StartNew();
                    JObject jsonObject = _FromXmlGetJObject(recvXml, responseSelectJsonPath);
                    parseWatch.Stop();
                    totalWatch.Stop();

                    // 记录日志 (调用你原有的工具类)
                    Log4netHelper.Info(string.Format(log, sendTime, url, "发送", requestXml));
                    Log4netHelper.Info(string.Format(log, receiveTime, url, "接收", recvXml));
                    LogProductPassMesDetail(
                        $"MES请求明细 Function={function} Url={url} 发送时间={sendTime} 返回时间={receiveTime} " +
                        $"HTTP耗时={postWatch.ElapsedMilliseconds}ms 响应读取耗时={readWatch.ElapsedMilliseconds}ms " +
                        $"XML解析耗时={parseWatch.ElapsedMilliseconds}ms 总耗时={totalWatch.ElapsedMilliseconds}ms");

                    return jsonObject;
                }
            }
            catch (Exception ex)
            {
                totalWatch.Stop();
                Log4netHelper.Info(string.Format(log, sendTime, url, "无法访问", requestXml + "\r\n异常信息:" + ex.Message));
                LogProductPassMesDetail($"MES请求异常 Function={function} Url={url} 总耗时={totalWatch.ElapsedMilliseconds}ms 异常={ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 仅在产品过站链路内记录 MES 明细耗时，避免污染其它业务日志。
        /// </summary>
        private void LogProductPassMesDetail(string message)
        {
            string traceId = ProductPassTraceContext.CurrentTraceId;
            if (string.IsNullOrWhiteSpace(traceId)) return;

            Log4netHelper.LogProductPass($"TraceId={traceId} {message}");
        }

        private String GenerateXml(JObject inputParameterJson, string function, bool useHeader = false, string token = null)
        {
            if (useHeader == true && (token == null | function == null)) throw new RecevieArgsException("启用head必须传入token和function,缺一不可");
            XNamespace soap12 = "http://schemas.xmlsoap.org/soap/envelope/";
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace xsd = "http://www.w3.org/2001/XMLSchema";
            XNamespace tempuri = "http://tempuri.org/";

            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement(soap12 + "Envelope",
                    new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                    new XAttribute(XNamespace.Xmlns + "xsd", xsd),
                    new XAttribute(XNamespace.Xmlns + "soap", soap12),
                    new XElement(soap12 + "Body",
                        new XElement(tempuri + (useHeader ? "MesServiceJson" : "GetAccessTokenJson"),
                            useHeader ? new XElement(tempuri + "Function", function) : null,
                            new XElement(tempuri + "InputParameter", JsonConvert.SerializeObject(inputParameterJson, Newtonsoft.Json.Formatting.None))
                        )
                    )
                )
            );

            if (useHeader)
            {
                var header = new XElement(soap12 + "Header",
                    new XElement(tempuri + "SecurityToken",
                        new XElement(tempuri + "Access_Token", token)
                    )
                );
                doc.Root.AddFirst(header);
            }
            return doc.Declaration.ToString() + "\n" + doc.ToString();
        }

        private void GetToken(string logFile)
        {
            JObject response = _GetResponse(_url, "", getTotkenJsonData, logFile, getToken: true);
            if (response is null) return;

            if (response["Result"].ToString().Equals("PASS", StringComparison.OrdinalIgnoreCase))
            {
                token = response["Access_Token"].ToString();
                expiredtimeToken = _TimeFormat(response["Expiredtime_Token"].ToString());
            }
        }

        private DateTime _TimeFormat(string timeStr)
        {
            string format = "yyyy-MM-dd HH:mm:ss.fff";
            return DateTime.ParseExact(timeStr, format, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        private JObject _FromXmlGetJObject(string xmlString, string selectPath)
        {
            XDocument xDoc = XDocument.Parse(xmlString);
            XPathNavigator navigator = xDoc.CreateNavigator();
            string jsonString = navigator.Select(selectPath).Current.Value;
            return JObject.Parse(jsonString);
        }

        // =======================================================
        // 以下为原封不动保留的 FTP 上传及文件处理逻辑
        // =======================================================

        private string HandlerFtpPathString(string path)
        {
            if (path.StartsWith(@"ftp:\\") || path.StartsWith(@"ftp://")) path = path.Substring(6);
            else if (path.StartsWith(@"ftp:\") || path.StartsWith(@"ftp:/")) path = path.Substring(5);
            path = path.Replace(@"\", "/");
            return "ftp://" + path;
        }

        private void CreateFtpDirectory(string user, string pwd, string path)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.Credentials = new NetworkCredential(user, pwd);
            try
            {
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse()) { response.Close(); }
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.ProtocolError || !((FtpWebResponse)ex.Response).StatusDescription.StartsWith("550")) { throw; }
            }
        }

        private bool CheckFtpDirExist(string user, string pwd, string path)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.Credentials = new NetworkCredential(user, pwd);
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse()) { response.Close(); return true; }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ((FtpWebResponse)ex.Response).StatusDescription.StartsWith("550")) return false;
                else throw;
            }
        }

        private List<string> GetFtpParentDirExistPath(string user, string pwd, string path, List<string> paths)
        {
            if (!CheckFtpDirExist(user, pwd, path))
            {
                paths.Add(path);
                path = HandlerFtpPathString(Path.GetDirectoryName(path));
                GetFtpParentDirExistPath(user, pwd, path, paths);
            }
            return paths;
        }

        private void CreateFtpChildDirectory(string user, string pwd, List<string> paths, int index)
        {
            if (index == paths.Count) return;
            CreateFtpDirectory(user, pwd, paths[index]);
            CreateFtpChildDirectory(user, pwd, paths, index + 1);
        }

        public bool CheckFtpDirectory()
        {
            try
            {
                Dictionary<string, string> dates = _GetDateForCreateDirectory();
                List<string> paths = GetFtpParentDirExistPath(this.user, this.passwd, this.FtpDirPath, new List<string>());
                paths.Sort();
                CreateFtpChildDirectory(this.user, this.passwd, paths, 0);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public static Dictionary<string, string> _GetDateForCreateDirectory()
        {
            DateTime now = DateTime.Now;
            return new Dictionary<string, string> {
                { "Y", $"{now.Year}"},
                { "Y-M", $"{now.Year}-{now.Month}"},
                { "Y-M-D", $"{now.Year}-{now.Month}-{now.Day}"},
            };
        }

        public string UploadToFtpServer(string localPath, string localFileName, string uploadFileName)
        {
            string ftpfullPath = HandlerFtpPathString(Path.Combine(this.FtpDirPath, uploadFileName));
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpfullPath);
            request.Credentials = new NetworkCredential(this.user, this.passwd);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            request.UseBinary = true;
            request.UsePassive = true;

            FileInfo fileInfo = new FileInfo(Path.Combine(localPath, localFileName));
            FileStream fileStream = fileInfo.OpenRead();
            byte[] buffer = new byte[716800];
            int bytesRead;
            try
            {
                using (Stream requestStream = request.GetRequestStream())
                {
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0) { requestStream.Write(buffer, 0, bytesRead); }
                    fileStream.Close();
                }
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse()) { return this.FtpDirPath; }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
                {
                    FtpWebResponse response = (FtpWebResponse)ex.Response;
                    if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    {
                        if (!CheckFtpDirectory()) return null;
                        return UploadToFtpServer(localPath, localFileName, uploadFileName);
                    }
                }
                return null;
            }
        }

        private string _ExecuteFileName(JObject jsonData) { return jsonData["TemplateName"].ToString(); }
        private byte[] _ExcuteFileStream(JObject jsonData) { return ((byte[])jsonData["TemplateData"]); }

        private bool SaveFileFromFtpServer(string filename, byte[] fileStream)
        {
            try
            {
                if (File.Exists(filename)) return true;
                Directory.CreateDirectory(Path.GetDirectoryName(filename));
                using (FileStream ioStream = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    ioStream.Write(fileStream, 0, fileStream.Length);
                }
                return true;
            }
            catch (IOException ex) { throw new FileException("处理文件I/O相关的异常"); }
            catch (Exception ex) { throw new FileException($"保存文件发生其它错误：{ex.Message}"); }
        }

        #endregion
    }
}


using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MesDatas.Utility
{
    #region 异常类

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

    #endregion

    /// <summary>
    /// 请求接口工具类
    /// </summary>
    public class HttpClientUtil
    {
        #region 成员变量

        // 用于 API 接口通讯
        private string _url;
        private JObject getTotkenJsonData;
        private string token = null;
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

        /// <summary>
        /// 获取json数据实例化接口
        /// </summary>
        /// <param name="getTotkenJsonData">获取token的json数据</param>
        /// <param name="tokenUrl">获取token的url，不需要传递地址参数</param>
        /// <param name="accessUrl">访问的url，不需要传递地址参数</param>
        public HttpClientUtil(JObject getTotkenJsonData, string tokenUrl)
        {
            this.getTotkenJsonData = getTotkenJsonData;
            _url = tokenUrl;
            //GetToken();
        }

        /// <summary>
        /// 上传下载文件的实例化接口
        /// </summary>
        /// <param name="user">ftp用户名</param>
        /// <param name="passwd">ftp密码</param>
        /// <param name="ftpUrl">ftp访问服务器地址</param>
        public HttpClientUtil(string url, string process, string line, string user, string passwd, string fileType)
        {
            //ftp://10.24.236.210/CIBU8/TestDataBackup/工序名称/线体名称/2022/2022-10/2022-10-21/{Log|Picture}/ 文件路径
            Dictionary<string, string> dates = _GetDateForCreateDirectory();
            //对ftpFilePath赋值
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

            if (token is null) GetToken(logFile);

            //BringTokenAccessAsync(url,function,JsonConvert.SerializeObject(inputParameterJson));
            JObject jsonObject = _GetResponse(url, function, inputParameterJson, logFile, responseSelectJsonPath: responseSelectJsonPath);
            if (jsonObject is null) return null;
            if (jsonObject["ErrorCode"].ToString() == "10001" && getTokenCount < 3)
            {
                getTokenCount++;
                GetToken(logFile);
                return GetResponse(url, function, inputParameterJson, logFile, getTokenCount: getTokenCount);
            }
            else return jsonObject;
        }

        /// <summary>
        /// 发起请求并获得请求的结果
        /// </summary>
        /// <param name="function">访问接口的function标签中的内容（固定参数）</param>
        /// <param name="inputParameterJson">inputParameterJson标签中的Json数据</param>
        /// <param name="responseSelectJsonPath">响应的xml获取json的xml路径</param>
        /// <returns></returns>
        private JObject _GetResponse(string url, string function, JObject inputParameterJson, string logFile, string responseSelectJsonPath = "//MesServiceJsonResult", bool getToken = false)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "text/xml; charset=utf-8";
            string timeFormat = "yyyy-MM-dd HH:mm:ss";
            string log = "时间:{0}  url地址:{1}  类型:{2}  数据:{3}";
            string requestXml;

            using (Stream requestStream = request.GetRequestStream())
            {
                //获取xml
                if (getToken)
                    requestXml = AddXmlToRequst(requestStream, inputParameterJson, function, false);  //获取token不用头信息
                else
                    requestXml = AddXmlToRequst(requestStream, inputParameterJson, function, true);
            }
            //记录请求前时间，发起请求
            string sendTime = DateTime.Now.ToString(timeFormat);
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    //首先记录接收时间
                    string receiveTime = DateTime.Now.ToString(timeFormat);
                    //获取响应结果
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    string recvXml = reader.ReadToEnd();
                    //将xml字符串转为Json对象
                    JObject jsonObject = _FromXmlGetJObject(recvXml, responseSelectJsonPath);
                    //写发送和接收数据日志
                    //写日志到界面
                    //Form1.writeXmlLogToUi(sendTime, receiveTime, requestXml, recvXml, logFile);
                    //写日志到文件
                    Log4netHelper.Info(string.Format(log, sendTime, url, "发送", requestXml));
                    Log4netHelper.Info(string.Format(log, receiveTime, url, "接收", recvXml));
                    //如果得到的Json中ErrorCode的码是10001，则说明token过期，需要递归调用一次
                    return jsonObject;
                }
            }
            catch (System.Net.WebException ex)
            {
                //Form1.writeXmlLogToUi(sendTime, "null", requestXml, "访问异常", logFile);
                Log4netHelper.Info(string.Format(log, sendTime, url, "无法访问", requestXml));
                return null;
            }
        }

        /// <summary>
        /// 上传txt文件
        /// </summary>
        /// <param name="user">用户名</param>
        /// <param name="passwd">密码</param>
        /// <param name="Process">工序</param>
        /// <param name="Line">线体</param>
        /// <param name="fullPath">本地文件路径+文件名</param>
        /// <returns>返回ftp目录，不返回目录+文件名</returns>
        /*public string UploadFileToFtpServer(string url, string process, string line, string localPath, string localFilename, string uploadFilename)
        {
            try
            {
                return UploadToFtpServer(url, process, line, localPath, localFilename, uploadFilename, "Log");
            }
            catch (Exception e)
            {
                return null;
            }
        }*/

        /// <summary>
        /// 上传图片文件
        /// </summary>
        /// <param name="Process">工序</param>
        /// <param name="Line">线体</param>
        /// <param name="fullPath">本地文件路径+文件名</param>
        /// <returns>返回ftp目录，不返回目录+文件名</returns>
        /*public string UploadPictureToFtpServer(string url, string process, string line, string localPath, string localFilename, string uploadFilename)
        {
            try
            {
                return UploadToFtpServer(url, process, line, localPath, localFilename, uploadFilename, "Picture");
            }
            catch (Exception e)
            {
                return null;
            }
        }
*/

        /// <summary>
        /// 保存文件到本地
        /// </summary>
        /// <param name="filePath">本地文件路径</param>
        /// <param name="filename">本地文件名</param>
        /// <param name="fileSteam">文件字节流</param>
        /// <returns></returns>
        /*public bool SaveFileToLocalFromFtpServer(string localPath, JObject jsonData, out string filename)
        {
            filename = _ExecuteFileName(jsonData);
            byte[] fileStream = _ExcuteFileStream(jsonData);
            filename = Path.Combine(localPath, filename) + ".lab";
            return SaveFileFromFtpServer(filename, fileStream);
        }*/

        #endregion

        #region private方法

        /// <summary>
        /// 获取request Body(xml)字符串
        /// </summary>
        /// <param name="inputParameterJson">inputParamenterJson标签中的json数据,</param>
        /// <param name="function">固定方法名</param>
        /// <param name="useHeader">控制是否启用head标签,获取token时不启用</param>
        /// <param name="token">获取到的token，如果useHeader为true，必须传入token</param>
        /// <returns></returns>
        private String GenerateXml(JObject inputParameterJson, string function, bool useHeader = false, string token = null)
        {
            //如果启用了头信息，但是不传入token则是一个异常问题
            if (useHeader == true && (token == null | function == null)) throw new RecevieArgsException("启用head必须传入token和function,缺一不可");
            XNamespace soap12 = "http://schemas.xmlsoap.org/soap/envelope/";
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            XNamespace xsd = "http://www.w3.org/2001/XMLSchema";
            XNamespace tempuri = "http://tempuri.org/";
            //新建一个xml对象并添加固定不变的元素
            XDocument doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement(soap12 + "Envelope",
                    new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                    new XAttribute(XNamespace.Xmlns + "xsd", xsd),
                    new XAttribute(XNamespace.Xmlns + "soap", soap12),
                    //body无论是获取token还是通过token获取数据都是必须有的
                    new XElement(soap12 + "Body",
                        new XElement(tempuri + (useHeader ? "MesServiceJson" : "GetAccessTokenJson"),
                            useHeader ? new XElement(tempuri + "Function", function) : null,
                            new XElement(tempuri + "InputParameter", JsonConvert.SerializeObject(inputParameterJson, Newtonsoft.Json.Formatting.None))
                        )
                    )
                )
            );
            //头信息不一定有，获取token不需要在头信息中传入Access_Token，所以所以通过判断是否需要添加头信息
            //如果头信息是true，则添加头信息到xml里面
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
            //return doc;
        }

        /// <summary>
        /// 获取token
        /// </summary>
        private void GetToken(string logFile)
        {
            JObject response = _GetResponse(this._url, "", this.getTotkenJsonData, logFile, getToken: true);
            if (response is null) return;
            if (response["Result"].ToString().Equals("Pass"))
            {
                //设置token
                this.token = response["Access_Token"].ToString();
                //设置过期时间
                this.expiredtimeToken = _TimeFormat(response["Expiredtime_Token"].ToString());
            }
        }

        /// <summary>
        /// 将字符串渲染成"yyyy-MM-dd HH:mm:ss.fff"格式的DateTime对象
        /// </summary>
        /// <param name="timeStr"></param>
        /// <returns></returns>
        private DateTime _TimeFormat(string timeStr)
        {
            string format = "yyyy-MM-dd HH:mm:ss.fff";
            return DateTime.ParseExact(timeStr, format, CultureInfo.InvariantCulture, DateTimeStyles.None);
        }

        /// <summary>
        /// 添加xml参数进request
        /// </summary>
        /// <param name="requestStream">request的stream</param>
        /// <param name="inputParameterJson">json数据用于添加进xml的inputParamenter标签</param>
        /// <param name="function">接口方法名</param>
        private string AddXmlToRequst(Stream requestStream, JObject inputParameterJson, string function, bool useHeader)
        {
            using (Stream stream = requestStream)
            {
                ////获取xml
                string xmlData = GenerateXml(inputParameterJson, function, useHeader, this.token is null ? "未获取到token" : this.token);
                //string xmlData = @"<?xml version=""1.0"" encoding=""utf-8""?><soap:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/""><soap:Body><GetAccessTokenJson xmlns=""http://tempuri.org/""><InputParameter>{""Line"": ""D12"", ""Process"": ""Scan-DIP"", ""Station"": ""Scan-DIP-12"", ""Device"": ""Scan-DIP-12"", ""ComputerName"": ""ComputerName"", ""Key"": ""Scan-D12"", ""Security"": ""5d3c3f70d00be63dca714b1af5d81f7d"", ""Language"": ""CH""}</InputParameter></GetAccessTokenJson></soap:Body></soap:Envelope>";
                //进行字节编码
                Console.WriteLine(xmlData);
                byte[] buffer = Encoding.UTF8.GetBytes(xmlData);
                //写入xml到request
                stream.Write(buffer, 0, buffer.Length);
                //XDocument xml = GenerateXml(inputParameterJson, function, useHeader, this.token is null ? "未获取到token" : this.token);
                //xml.Save(stream);
                return xmlData;
            }
        }

        /// <summary>
        /// 从xml中提取出json数据
        /// </summary>
        /// <param name="xmlString">xml字符串</param>
        /// <param name="selectPath">获取json的xml路径</param>
        /// <returns></returns>
        private JObject _FromXmlGetJObject(string xmlString, string selectPath)
        {
            XDocument xDoc = XDocument.Parse(xmlString);
            //创建访问索引器
            XPathNavigator navigator = xDoc.CreateNavigator();
            string jsonString = navigator.Select(selectPath).Current.Value;
            return JObject.Parse(jsonString);
        }

        private string HandlerFtpPathString(string path)
        {
            if (path.StartsWith(@"ftp:\\") || path.StartsWith(@"ftp://"))
                path = path.Substring(6);
            else if (path.StartsWith(@"ftp:\") || path.StartsWith(@"ftp:/"))
                path = path.Substring(5);
            path = path.Replace(@"\", "/");
            return "ftp://" + path;
        }

        /// <summary>
        /// 向服务器创建目录
        /// </summary>
        /// <param name="path"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        private void CreateFtpDirectory(string user, string pwd, string path)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.Credentials = new NetworkCredential(user, pwd);

            try
            {
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    response.Close();
                }
            }
            catch (WebException ex)
            {
                // 可能目录已经存在，忽略异常  
                if (ex.Status != WebExceptionStatus.ProtocolError || !((FtpWebResponse)ex.Response).StatusDescription.StartsWith("550"))
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 检查ftp目录是否存在或创建
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool CheckFtpDirExist(string user, string pwd, string path)
        {
            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(path);
                request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                request.Credentials = new NetworkCredential(user, pwd);

                // 发送请求  
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    // 如果能获取到目录列表，说明目录存在，继续下一个  
                    response.Close();
                    return true;
                }
            }
            catch (WebException ex)
            {
                // 如果目录不存在
                if (ex.Status == WebExceptionStatus.ProtocolError && ((FtpWebResponse)ex.Response).StatusDescription.StartsWith("550"))
                {
                    return false;
                }
                else
                {
                    throw; // 其他异常重新抛出  
                }
            }
        }

        /// <summary>
        /// 向父级目录递归查找哪一级目录是已经存在了
        /// </summary>
        /// <param name="path"></param>
        /// <returns>存在的目录路径</returns>
        private List<string> GetFtpParentDirExistPath(string user, string pwd, string path, List<string> paths)
        {
            //如果向父级目录检查到该父级目录不存在，则继续递归上一级目录
            if (!CheckFtpDirExist(user, pwd, path))
            {
                paths.Add(path);
                path = HandlerFtpPathString(Path.GetDirectoryName(path));
                GetFtpParentDirExistPath(user, pwd, path, paths);
            }
            return paths;
        }

        /// <summary>
        /// 递归向ftp创建子级目录
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private void CreateFtpChildDirectory(string user, string pwd, List<string> paths, int index)
        {
            if (index == paths.Count) return;
            CreateFtpDirectory(user, pwd, paths[index]);
            CreateFtpChildDirectory(user, pwd, paths, index + 1);
        }

        /// <summary>
        /// 检查ftp路径信息的入口
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        /// <param name="fullPath"></param>
        /// <returns>true为正常，false为异常</returns>
        public bool CheckFtpDirectory()
        {
            try
            {
                //得到不存在的目录
                Dictionary<string, string> dates = _GetDateForCreateDirectory();
                List<string> paths = GetFtpParentDirExistPath(this.user, this.passwd, this.FtpDirPath, new List<string>());
                //从后往前依次创建
                paths.Sort();
                CreateFtpChildDirectory(this.user, this.passwd, paths, 0);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 创建路径所需要的路径时间信息
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> _GetDateForCreateDirectory()
        {
            DateTime now = DateTime.Now;
            return new Dictionary<string, string> {
                { "Y", $"{now.Year}"},
                { "Y-M", $"{now.Year}-{now.Month}"},
                { "Y-M-D", $"{now.Year}-{now.Month}-{now.Day}"},
            };
        }

        /// <summary>
        /// 上传文件到ftp服务器统一接口
        /// </summary>
        /// <param name="user">用户名</param>
        /// <param name="passwd">密码</param>
        /// <param name="Process">工序</param>
        /// <param name="Line">线体</param>
        /// <param name="localPath">本地路径</param>
        /// <param name="filename">本地文件名</param>
        /// <param name="fileTypeKeyWords">{Log|Picture}</param>
        /// <returns>返回ftp目录，不返回目录+文件名</returns>
        public string UploadToFtpServer(string localPath, string localFileName, string uploadFileName)
        {
            string ftpfullPath = HandlerFtpPathString(Path.Combine(this.FtpDirPath, uploadFileName));
            //上传文件,必须是路径+文件名
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpfullPath);
            //设置ftp验证
            request.Credentials = new NetworkCredential(this.user, this.passwd);
            request.Method = WebRequestMethods.Ftp.UploadFile;
            // 设置文件类型为二进制  
            request.UseBinary = true;
            // 被动模式  
            request.UsePassive = true;
            //request.KeepAlive = true;

            FileInfo fileInfo = new FileInfo(Path.Combine(localPath, localFileName));
            // 读取文件内容  
            FileStream fileStream = fileInfo.OpenRead();
            byte[] buffer = new byte[716800];  //开辟700K
            int bytesRead;
            try
            {
                // 获取请求流  
                using (Stream requestStream = request.GetRequestStream())
                {
                    // 读取文件内容并写入到FTP请求流中
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        requestStream.Write(buffer, 0, bytesRead);
                    }
                    // 关闭文件流  
                    fileStream.Close();
                }
                // 获取FTP响应  
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                { //文件上传成功
                    return this.FtpDirPath;
                }
            }
            catch (WebException ex)
            {
                // 检查异常是否由于目录不存在引起  
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
                {
                    FtpWebResponse response = (FtpWebResponse)ex.Response;
                    if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    {
                        //检查服务器目录，如果没有就创建
                        if (!CheckFtpDirectory()) return null;
                        return UploadToFtpServer(localPath, localFileName, uploadFileName);
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// 从json中解析出文件名
        /// </summary>
        /// <param name="jsonData">json对象</param>
        /// <returns></returns>
        private string _ExecuteFileName(JObject jsonData)
        {
            return jsonData["TemplateName"].ToString();
        }

        /// <summary>
        /// 从json中解析出字节流
        /// </summary>
        /// <param name="jsonData">json对象</param>
        /// <returns></returns>
        private byte[] _ExcuteFileStream(JObject jsonData)
        {
            return ((byte[])jsonData["TemplateData"]);
        }

        /// <summary>
        /// 从json数据获取到的字节流保存文件到本地统一接口
        /// </summary>
        /// <param name="localPath">本地目录路径</param>
        /// <param name="filename">本地文件名</param>
        /// <param name="fileStream">文件的字节流</param>
        /// <returns></returns>
        private bool SaveFileFromFtpServer(string filename, byte[] fileStream)
        {
            try
            {
                if (File.Exists(filename)) return true;
                //即使目录存在也不会保存，无需判断
                Directory.CreateDirectory(Path.GetDirectoryName(filename));

                using (FileStream ioStream = new FileStream(filename, FileMode.Create, FileAccess.Write))
                {
                    // 将字节流写入文件  
                    ioStream.Write(fileStream, 0, fileStream.Length);
                }
                return true;
            }
            catch (IOException ex)
            {
                // 处理文件I/O相关的异常，例如文件已存在且不允许覆盖，磁盘空间不足等  
                throw new FileException("处理文件I/O相关的异常");
            }
            catch (Exception ex)
            {
                // 处理其他类型的异常  
                throw new FileException($"保存文件发生其它错误：{ex.Message}");
            }
        }

        #endregion
    }
}


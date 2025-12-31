using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MesDatas.Utility
{
    /// <summary>
    /// C#对象 -> JSON -> HTTP POST -> JSON -> C#对象
    /// </summary>
    public class RequestMes
    {
        //private static Dictionary<string, string> dic = new Dictionary<string, string>();

        /// <summary>
        /// 访问接口，将获接口返回的结果反序列化,T1为返回类型，T2为输入类型
        /// <para>用于获取拼版、流程检查、结果上传、统一MES访问接口</para>
        /// </summary>
        /// <param name="function">Function固定参数</param>
        /// <param name="inputParam">反序列化对象,用于转请求接口的json</param>
        /// <param name="httpClient">接口连接对象</param>
        /// <returns></returns>
        public T1 GetResponseSerializeResult<T1, T2>(string url, HttpClientUtil httpClient, string function, T2 inputParam, string logFile)
        {
            string jsonString = JsonConvert.SerializeObject(inputParam);
            JObject json = JObject.Parse(jsonString);

            JObject jsonResult = httpClient.GetResponse(url, function, json, logFile);

            if (jsonResult == null)
                return default(T1);
            return JsonConvert.DeserializeObject<T1>(JsonConvert.SerializeObject(jsonResult, Formatting.None));
        }

        /*/// <summary>
        /// 获取拼版,将获取拼版的结果反序列化
        /// </summary>
        /// <param name="httpClient">接口连接对象</param>
        /// <param name="function">Function固定参数</param>
        /// <param name="inputParam">反序列化对象,用于转请求接口的json</param>
        /// <returns></returns>
        public GetBarCodeReturnParameter GetBarCode(string url, HttpClientUtil httpClient, string function, GetBarCodeInputParameter inputParam)
        {
            //string result = "{\"Result\":\"Pass\",\"ErrorCode\":\"\",\"ErrorMessage\":\"\",\"PrdSNInfo\":{\"PrdSNs\":[{\"PrdSN\":\"ELMBTC1214071850\",\"SubBoardId\":\"1\"},{\"PrdSN\":\"ELMBTC1214071851\",\"SubBoardId\":\"2\"}]}}";
            return GetResponseSerializeResult<GetBarCodeReturnParameter, GetBarCodeInputParameter>(url, httpClient, function, inputParam);
        }

        /// <summary>
        /// 流程检查,将获得接口返回的结果反序列化
        /// </summary>
        /// <param name="httpClient">接口连接对象</param>
        /// <param name="function">Function固定参数</param>
        /// <param name="inputParam">反序列化对象,用于转请求接口的json</param>
        /// <returns></returns>
        public RouteCheckReturnParam checkPath(string url, HttpClientUtil httpClient, string function, RouteCheckInputParam inputParam)
        {
            //string result = "{\"Result\":\"Pass\",\"ErrorCode\":\"\",\"ErrorMessage\":\"\",\"SkipBoards\":{\"SkipBoard\":[\"ELMBTC1214071852\"]},\"Program\":\"A\"}";
            return GetResponseSerializeResult<RouteCheckReturnParam, RouteCheckInputParam>(url, httpClient, function, inputParam);
        }

        public ReturnParamDeviceStatus deviceStatus(string url, HttpClientUtil httpClient, string function, InputParamDeviceStatus inputParam)
        {
            return GetResponseSerializeResult<ReturnParamDeviceStatus, InputParamDeviceStatus>(url, httpClient, function, inputParam);
        }

        /// <summary>
        /// 结果上传
        /// </summary>
        /// <param name="inputParam"></param>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public ReturnParamSendResult SendResult(string url, HttpClientUtil httpClient, string function, InputParamSendResult inputParam)
        {
            //string result = "{\"Result\":\"Pass\",\"ErrorCode\":\"\",\"ErrorMessage\":\"\"}";
            return GetResponseSerializeResult<ReturnParamSendResult, InputParamSendResult>(url, httpClient, function, inputParam);
        }

        public PrintBarCodeReturnParam PrintBarCode(string url, HttpClientUtil httpClient, string function, PrintBarCodeReturnParam inputParam)
        {
            //string result = "{\"Result\":\"Pass\",\"ErrorCode\":\"\",\"ErrorMessage\":\"\"}";
            return GetResponseSerializeResult<ReturnParamSendResult, InputParamSendResult>(url, httpClient, function, inputParam);
        }*/
    }
}

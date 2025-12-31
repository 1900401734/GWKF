using HslCommunication.Profinet.Keyence;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesDatas.Utility
{
    class CodeNum
    {
        /// <summary>
        /// //对string 转double ➗1000 在转string
        /// </summary>
        /// <param name="ss"></param>
        /// <returns></returns>
        public static string PThousdCode(string ss)
        {
            string aa = "0";
            double db = 0;
            if (double.TryParse(ss, out db))
            {
                if (db != 0)
                {
                    aa = Convert.ToString((Convert.ToDouble(ss)) / 1000);
                }
                else
                {
                    aa = Convert.ToString((Convert.ToDouble(ss)));
                }
            }
            return aa;

        }

        /// <summary>
        /// //对string 转double ➗100 在转string
        /// </summary>
        /// <param name="ss"></param>
        /// <returns></returns>
        public static string PNumCode(string ss)
        {
            string aa = "0";
            double db = 0;
            if (double.TryParse(ss, out db))
            {
                if (db != 0)
                {
                    aa = Convert.ToString((Convert.ToDouble(ss)) / 100);
                }
                else
                {
                    aa = Convert.ToString((Convert.ToDouble(ss)));
                }
            }
            return aa;
            
        }

        /// <summary>
        /// //对string 转double ➗100 在转string(取2为)
        /// </summary>
        /// <param name="ss"></param>
        /// <returns></returns>
        public static string PdounInCode(string ss)
        {
            string aa = "0";
            double db = 0;
            if (double.TryParse(ss, out db))
            {
                if (db != 0)
                {
                    double mydo = Math.Round((Convert.ToDouble(ss)) / 100, 2);
                    aa = mydo.ToString("0.00");
                }
                else
                {
                    aa = Convert.ToString((Convert.ToDouble(ss)));
                }
            }
            return aa;
        }

        /// <summary>
        /// //对string 转double ➗10 在转string
        /// </summary>
        /// <param name="ss"></param>
        /// <returns></returns>
        public static string PNtimeCode(string ss)
        {
            string aa = "0";
            double db = 0;
            if (double.TryParse(ss, out db))
            {
                if (db != 0)
                {
                    aa = Convert.ToString((Convert.ToDouble(ss)) / 10);
                }
                else
                {
                    aa = Convert.ToString((Convert.ToDouble(ss)));
                }
            }
            return aa;
        }

        /// <summary>
        /// 转换3:OK 2:NG 1:unknown
        /// </summary>
        /// <param name="ss"></param>
        /// <returns></returns>
        public static string PNumOKAG(string ss)
        {
            string aa = "NG";
            int db = 0;
            if (int.TryParse(ss, out db))
            {
                if (db == 3)
                {
                    aa = "OK";
                }
                else if(db==2)
                {
                    aa = "NG";
                }
                else if (db == 1)
                {
                    aa = "unknown";
                }
                else if (db == 66)
                {
                    aa = "正面";
                }
                else if (db == 99)
                {
                    aa = "反面";
                }
            }
            return aa;
        }
      
        /// <summary>
        /// 处理string
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string CleanString(string str)
        {
            if (!string.IsNullOrWhiteSpace(str))
            {
                int index = str.IndexOf("\0");
                if (index >= 0)
                {
                    str = str.Substring(0, index);
                    str = str.Replace("\0", "");
                }
                if (str.IndexOf("?") >= 0)
                {
                    str = str.Replace("?", "");
                }
            }
            return str;
        }

        /// <summary>
        /// 处理工位
        /// </summary>
        /// <param name="workstID"></param>
        /// <param name="stationName"></param>
        /// <returns></returns>
        public static List<string> WorkIDName(string[] workstID, string[] stationName)
        {
            List<string> workstNamelist = new List<string>();
            string worName = "  ";
            for (int i = 0; i < workstID.Length; i++)
            {
                string workID = workstID[i];
                if (workID.Contains("+"))
                {
                    int count = 0;
                    string newStr = workID.Replace("+", "");
                    int.TryParse(newStr, out count);
                    for (int j = 0; j < count; j++)
                    {
                        workstNamelist.Add(worName);
                    }
                }
                else
                {
                    int id = 0;
                    if (int.TryParse(workstID[i], out id))
                    {
                        id = id - 1;
                        if (stationName.Length > id)
                        {
                            worName = stationName[id];
                        }
                    }
                    workstNamelist.Add(worName);
                }
            }
            return workstNamelist;
        }

        /// <summary>
        /// 通过id获取名称
        /// </summary>
        /// <param name="workstID"></param>
        /// <param name="stationName"></param>
        /// <returns></returns>
        public static string WorkIDNm(string workstID,string[] stationName)
        {
            string worName = "";
            int id = 0;
            if (int.TryParse(workstID, out id))
            {
                id = id - 1;
                if (stationName.Length > id)
                {
                    worName = stationName[id];
                }
            }
            return worName;
        }

        /// <summary>
        /// 获取全部信息
        /// </summary>
        /// <param name="mstr"></param>
        /// <returns></returns>
        public static List<string> SMaxMindemo(string[] mstr)
        {
            List<string> listarr = new List<string>();
            if (mstr.Length > 0)
            {
                string boardBeat = "NO";//
                for (int i = 0; i < mstr.Length; i++)
                {
                    string beatcode = mstr[i];//
                    if (beatcode.Contains("+"))
                    {
                        int count = 0;
                        string newStr = beatcode.Replace("+", "");
                        int.TryParse(newStr, out count);
                        for (int j = 0; j < count; j++)
                        {
                            listarr.Add(boardBeat);
                        }
                    }
                    else
                    {
                        boardBeat = beatcode;
                        listarr.Add(beatcode);
                    }
                }
            }
            return listarr;
        }

        /// <summary>
        /// 获取不等于NO的个数
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int TMaxMinstrhomd(List<string> list)
        {
            int count = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].Equals("NO"))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 计算数组中不等于 "NO" 的元素数量。
        /// </summary>
        /// <param name="array">测试项信息中的测试项、上下限或其它数组</param>
        /// <returns>不等于 "NO" 的元素数量</returns>
        public static int GetValidItems(string[] array)
        {
            int count = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (!array[i].Equals("NO"))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 返回ON
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string NOCodes(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                code = "NO";
            }
            return code;
        }

        /// <summary>
        /// 返回+1
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string ONECodes(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                code = "+1";
            }
            return code;
        }

        /// <summary>
        /// 返回1
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string WorkIDONECodes(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                code = "1";
            }
            return code;
        }

        /// <summary>
        /// 如果为空或者null 就赋值空格
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string DaweiCodes(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                code = "   ";
            }
            return code;
        }

        /// <summary>
        /// 是否为int 如果不是默认100
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int Doubtowule(string strdou)
        {
            int couiou = 0;
            if (!int.TryParse(strdou,out couiou))
            {
                couiou = 100;
            }
            return couiou;
        }

        /// <summary>
        /// 是否为ushout 如果不是默认200
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static ushort Shoubtowule(string strdou)
        {
            ushort couiou = 0;
            if (!ushort.TryParse(strdou, out couiou))
            {
                couiou = 200;
            }
            return couiou;
        }

        /// <summary>
        /// 条码信息
        /// </summary>
        /// <param name="strdou"></param>
        /// <returns></returns>
        public static string Condend(string strCode)
        {
            string strCods = "";
            string[] strCodss = strCode.Split('|');
            if (strCodss.Length > 0)
            {
                strCods = strCodss[0];
            }
            return strCods;
        }

        /// <summary>
        /// 工装信息
        /// </summary>
        /// <param name="strdou"></param>
        /// <returns></returns>
        public static string[] Confrock(string strfrock)
        {
            string[] strfrocks = new string[] { };
            string[] strCodss = strfrock.Split('|');
            if (strCodss.Length > 1)
            {
                strfrocks =  strCodss[1].ToString().Split('+');
            }
            else
            {
                string strCod = "";
                strfrocks = strCod.Split('|'); 
            }
            return strfrocks;
        }

        /// <summary>
        /// string[] 值相等
        /// </summary>
        /// <param name="strArr1"></param>
        /// <param name="strArr2"></param>
        /// <returns></returns>
        public static bool CopareArr(string[] strArr1,string[] strArr2)
        {
            var q = from a in strArr1 join b in strArr2 on a equals b select a;
            bool flag = strArr1.Length == strArr2.Length && q.Count() == strArr1.Length;
            return flag;
        }

        /// <summary>
        /// 产品编码返回数组
        /// </summary>
        /// <param name="strCodedat"></param>
        /// <param name="codesDataM"></param>
        /// <returns></returns>
        public static string[] CodeMafror(string strCodedat,DataTable codesDataM)
        {
            string[] prodrow = new string[] { };
            DataRow[] rows = codesDataM.Select("条码验证型号与工装编号 = '" + strCodedat + "'");
            if (rows.Length > 0)
            {
                 prodrow = rows[0]["产品编码"].ToString().Split('+');
            }
            return prodrow;
        }

        /// <summary>
        /// 产品编码返回string
        /// </summary>
        /// <param name="strCodedat"></param>
        /// <param name="codesDataM"></param>
        /// <returns></returns>
        public static string CodeStrfror(string strCodedat, DataTable codesDataM)
        {
            string prodrow ="";
            DataRow[] rows = codesDataM.Select("条码验证型号与工装编号 = '" + strCodedat + "'");
            if (rows.Length > 0)
            {
                prodrow = rows[0]["产品编码"].ToString();
            }
            return prodrow;
        }

        /// <summary>
        /// 如果为空返回null
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string NullECoshu(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                str = "null";
            }
            return str;
        }

        /// <summary>
        /// 读取过来数据处理
        /// </summary>
        /// <param name="code"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string StatisCodehu(string code,string type)
        {
            if (type.Equals("I"))
            {
                code= PNumCode(code);
            }else if (type.Equals("T"))
            {
                code = PNtimeCode(code);
            }else if (type.Equals("J"))
            {
                code = PdounInCode(type);
            }else if (type.Equals("H"))
            {
                code = PNumOKAG(type);
            }
            return code;
        }
    }
}

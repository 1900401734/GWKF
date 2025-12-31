using MesDatas.Models;
using System;

namespace MesDatas
{
    class Global
    {
        // 线程安全的单例模式
        private static readonly Lazy<Global> _lazyInstance = new Lazy<Global>(() => new Global());
        public static Global Instance => _lazyInstance.Value;

        private string dataBaseName = "SystemDateBase";

        public string CurDataBaseName;

        public string SourceDataBase = AppDomain.CurrentDomain.BaseDirectory + $"SystemDateBase.mdb";

        public LoginUserEntity LoginMessage = null;

        public string DataBase
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + $"{this.dataBaseName}.mdb";
            }
            set
            {
                dataBaseName = value;
                CurDataBaseName = value;
            }
        }

        //public Form1 form1 = null;
    }
}

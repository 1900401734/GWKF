using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace MesDatas.Views
{
    class ToolLibsView : ListView
    {
        string mpath = null;
        public string RootPath
        {
            get
            {
                return mpath;
            }
            set
            {
                mpath = value;
                if (mpath!=null && !Directory.Exists(mpath))
                {
                    Directory.CreateDirectory(mpath);
                }
            }
        }
        public Dictionary<string, string> ToolsInfo = new Dictionary<string, string>();

        public void ResetToolLib()
        {
            DirectoryInfo root = new DirectoryInfo(RootPath);
            FileInfo[] files = root.GetFiles();
            foreach (FileInfo f in files)
            {
                if (File.Exists(f.FullName))
                {
                    File.Delete(f.FullName);
                }
            }
        }

        public void UpdateTools()
        {
            Items.Clear();
            ToolsInfo.Clear();

            //string filename = System.IO.Path.GetFileName(binFilePath);//文件名  “Default.aspx”
            //string extension = System.IO.Path.GetExtension(binFilePath);//扩展名 “.aspx”
            //string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(binFilePath);// 没有扩展名的文件名 “Default”
            //DirPath = System.IO.Path.GetDirectoryName(FilePath);//文件夹路径

            DirectoryInfo root = new DirectoryInfo(RootPath);
            FileInfo[] files = root.GetFiles();
            foreach (FileInfo f in files)
            {
                string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(f.FullName);
                ToolsInfo.Add(fileNameWithoutExtension, f.FullName);
                Items.Add(fileNameWithoutExtension);
            }

        }

    }
}

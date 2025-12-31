using System;
using System.Diagnostics;
using System.IO;

namespace MesDatas.Services
{
    class Resource
    {
        /// <summary>
        /// 获取占用当前文件的进程ID
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static int GetProcessId(string fileFullPath)
        {
            int processId = 0;
            string processName = Path.GetFileNameWithoutExtension(fileFullPath);
            // 获取所有相关进程
            Process[] processes = Process.GetProcessesByName(processName);
            foreach (Process p in processes)
            {
                try
                {
                    // 尝试获取进程的主模块路径
                    string modulePath = p.MainModule.FileName;
                    if (modulePath.Equals(fileFullPath, StringComparison.OrdinalIgnoreCase))
                    {
                        processId = p.Id;
                        break;
                    }
                }
                catch
                {
                    return -1;
                }
            }
            return processId;
        }
        
        /// <summary>
        /// 强制杀死进程
        /// </summary>
        /// <param name="processId"></param>
        public static void KillProcess(int processId)
        {
            try
            {
                Process process = Process.GetProcessById(processId);
                process.Kill();
                process.WaitForExit(); // 等待进程退出
            }
            catch
            {
                // 进程可能已经退出或者无法访问
            }
        }

        /// <summary>
        /// 强制删除文件夹内文件或文件夹
        /// </summary>
        /// <param name="folderPath"></param>
        public static void ForceDeleteFiles(string folderPath, bool includeFolder=false)
        {
            if (includeFolder)
            {
                foreach(var dir in Directory.GetDirectories(folderPath))
                {
                    Directory.Delete(dir,true);
                }
                return;
            }
            foreach(string file in Directory.GetFiles(folderPath))
            {
                Resource.ForceDeleteFile(file);
            }
        }

        /// <summary>
        /// 强制删除文件
        /// </summary>
        /// <param name="fullPath">文件路径+文件名</param>
        /// <param name="timeout">如果文件被占用，若等待timeout时长后还没有解除占用则强制</param>
        public static void ForceDeleteFile(string fullPath, int timeout = 5)
        {
            FileInfo fileInfo = new FileInfo(fullPath);

            System.DateTime startTime = System.DateTime.Now;
            while (true)
            {
                if ((System.DateTime.Now - startTime).Seconds <= timeout)
                {
                    try
                    {
                        fileInfo.Delete();
                        break;
                    }
                    catch (System.IO.IOException ioException) { }
                }
                else
                {
                    Resource.KillProcess(Resource.GetProcessId(fullPath));
                }

            }
        }
    }
}

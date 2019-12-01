/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
namespace IFramework
{
    public static partial class IO
    {
        public enum FileSizeLev
        {
            B = 0, KB, MB, GB, TB
        }
        public static string GetFileSize(long length)
        {
            int lev = 0;
            long len = length;
            if (length == 0) return "0 " + ((FileSizeLev)0).ToString();
            while (len / 1024 > 0)
            {
                len /= 1024;
                lev++;
            }
            return Math.Round(length / Math.Pow(1024, lev), 2).ToString() + " " + ((FileSizeLev)lev).ToString();
        }


        public static long GetFileLength(this string filePath)
        {
            if (!File.Exists(filePath) && !Directory.Exists(filePath)) return 0;
            FileInfo info = new FileInfo(filePath);
            return info.Length;
        }
        public static string GetFileSize(this string filePath)
        {
            if (!File.Exists(filePath) && !Directory.Exists(filePath)) return string.Empty;
            FileInfo info = new FileInfo(filePath);
            long length = info.Length;
            int lev = 0;
            if (length == 0) return info.Length + " " + ((FileSizeLev)0).ToString();
            while (length / 1024 > 0)
            {
                length /= 1024;
                lev++;
            }
            return Math.Round(info.Length / Math.Pow(1024, lev), 2).ToString() + " " + ((FileSizeLev)lev).ToString();
        }
        public static string ReadText(this string path, Encoding encoding)
        {
            var result = string.Empty;
            using (var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read))
            {
                using (var sr = new StreamReader(fs, encoding))
                {
                    result = sr.ReadToEnd();
                    sr.Close();
                    //fs.Flush();  
                    fs.Close();
                }
            }
            return result;
        }
        public static void WriteText(this string path, string content, Encoding encoding)
        {
            using (var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs, encoding))
                {
                    fs.Lock(0, fs.Length);
                    sw.Write(content);
                    fs.Unlock(0, fs.Length);

                    sw.Flush();
                    sw.Close();
                    sw.Dispose();

                    fs.Close();
                  //  fs.Flush();
                    fs.Dispose();
                }
            }
        }
        public static byte[] ReadBytes(this string path)
        {
            return File.ReadAllBytes(path);
        }
        public static void WriteBytes(this string path, byte[] buff)
        {
            File.WriteAllBytes(path, buff);
        }



        public static string[] GetSubDirs(this string path)
        {
            var di = new DirectoryInfo(path);
            var dirs = di.GetDirectories();
            return dirs.Select(d => d.Name).ToArray();
        }
        public static List<string> GetSubFiles(this string path, bool isAll = true, string suffix = "")
        {
            List<string> pathList = new List<string>();
            DirectoryInfo info = new DirectoryInfo(path);
            if (!info.Exists)
            {
                return pathList;
            }
            var files = info.GetFiles();
            foreach (var fi in files)
            {
                if (!string.IsNullOrEmpty(suffix))
                {
                    if (!fi.FullName.EndsWith(suffix, System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        continue;
                    }
                }
                pathList.Add(fi.FullName);
            }
            if (isAll)
            {
                var dirs = info.GetDirectories();
                foreach (var d in dirs)
                {
                    pathList.AddRange(GetSubFiles(d.FullName, isAll, suffix));
                }
            }
            return pathList;
        }

        public static bool IsDirectory(this string path)
        {
            FileInfo fi = new FileInfo(path);
            if ((fi.Attributes & FileAttributes.Directory) != 0)
                return true;
            return false;
        }
        public static void ClearDir(this string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);
        }
        public static bool RemoveEmptyDirectory(this string path)
        {
            if (string.IsNullOrEmpty(path)) throw new Exception("Directory name is invalid.");
            try
            {
                if (!Directory.Exists(path)) return false;
                string[] subDirectoryNames = Directory.GetDirectories(path, "*");
                int subDirectoryCount = subDirectoryNames.Length;
                foreach (string subDirectoryName in subDirectoryNames)
                {
                    if (RemoveEmptyDirectory(subDirectoryName))
                    {
                        subDirectoryCount--;
                    }
                }
                if (subDirectoryCount > 0) return false;
                if (Directory.GetFiles(path, "*").Length > 0) return false;
                Directory.Delete(path);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static string CombinePath(this string path, string toCombinePath)
        {
            return Path.Combine(path, toCombinePath).ToRegularPath();
        }
        public static string CombinePath(this string path, string[] paths)
        {
            for (int i = 1; i < paths.Length; i++)
            {
                path = path.CombinePath(paths[i]);
            }
            return path.ToRegularPath();
        }


        public static string GetFileName(this string path)
        {
            path = path.ToRegularPath();
            var lastIndex = path.LastIndexOf("/");
            return lastIndex >= 0 ? path.Substring(lastIndex + 1) : path;
        }
        public static string GetFileNameWithoutExtend(this string path)
        {
            var fileName = GetFileName(path);
            var lastIndex = fileName.LastIndexOf(".");
            return lastIndex >= 0 ? fileName.Substring(0, lastIndex) : fileName;
        }
        public static string GetFileExtendName(this string path)
        {
            var lastIndex = path.LastIndexOf(".");
            if (lastIndex >= 0)
            {
                return path.Substring(lastIndex);
            }
            return string.Empty;
        }

        public static string GetDirPath(this string path)
        {
            path = path.ToRegularPath();
            var lastIndex = path.LastIndexOf("/");
            return path.Substring(0, lastIndex + 1);
        }
        public static string GetLastDirName(this string path)
        {
            path = path.ToRegularPath();
            var dirs = path.Split('/');
            return dirs[dirs.Length - 2];
        }
        public static string ToRegularPath(this string path)
        {
            path = path.Replace('\\', '/');
            return path;
        }
        public static string ToRemotePath(this string path)
        {
            path = path.ToRegularPath();
            return path.Contains("://") ? path : ("file:///" + path).Replace("file:////", "file:///");
        }
    }
}

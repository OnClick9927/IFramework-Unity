/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.IO;
namespace IFramework.Net
{
    public abstract class DownLoader : IDownLoader
    {
        protected Action<float> CallProgress;
        protected float progress;
        protected long fileLength;
        protected long currentLength;
        public string Url { get; private set; }
        public bool IsDownLoading { get; protected set; }
        public string FileNameWithoutExt { get; protected set; }
        public string SaveDir { get; private set; }
        public string FileExt { get; protected set; }
        public string SaveFilePath { get; protected set; }
        public virtual float Progress { get { return progress; } }
        public virtual long FileLength { get { return fileLength; } }
        public virtual long CurrentLength { get { return currentLength; } }

        public DownLoader(string url, string SaveDir, Action<float> CallProgress)
        {
            this.CallProgress = CallProgress;
            this.Url = url;
            this.SaveDir = SaveDir;
            IsDownLoading = false;
            FileExt = Path.GetExtension(url);
            FileNameWithoutExt = Path.GetFileNameWithoutExtension(url)
                                                    .Replace(".", "")
                                                    .Replace("=", "")
                                                    .Replace("%", "")
                                                    .Replace("&", "")
                                                    .Replace("?", "");
            SaveFilePath = string.Format("{0}/{1}{2}", SaveDir, FileNameWithoutExt, FileExt);
        }
        public virtual void DownLoad()
        {
            if (string.IsNullOrEmpty(Url) || string.IsNullOrEmpty(SaveDir)) return;
            if (!string.IsNullOrEmpty(SaveFilePath))
            {
                string dirName = Path.GetDirectoryName(SaveFilePath);
                if (!Directory.Exists(dirName))
                {
                    Directory.CreateDirectory(dirName);
                }
            }
        }

        public abstract void Dispose();

    }
}
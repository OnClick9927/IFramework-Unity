/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.IO;
namespace IFramework.Net
{
    public abstract class FileDownLoader :IDisposable
    {
        public event Action onCompeleted;
        protected void Compelete()
        {
            if (onCompeleted!=null)
            {
                onCompeleted();
            }
        }

        public abstract float Progress { get; }
        public virtual long CurrentLength { get; }

        public string Url { get; private set; }
        public string SaveDir { get; private set; }
        public string FileExt { get; private set; }


        public bool IsDownLoading { get; protected set; }
        public string FileNameWithoutExt { get; protected set; }
        public string SaveFilePath { get; protected set; }

        public FileDownLoader(string url, string SaveDir)
        {
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
            if (string.IsNullOrEmpty(Url) || string.IsNullOrEmpty(SaveDir) || string.IsNullOrEmpty(SaveFilePath))
                throw new Exception(GetType().Name + " Ctor  Err");
        }

        
        public abstract void DownLoad();
        public virtual void Dispose()
        {
            onCompeleted = null;
            IsDownLoading = false;
        }

    }
}
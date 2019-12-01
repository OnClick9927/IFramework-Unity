/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.IO;
using System.Net;
using UnityEngine;
namespace IFramework.Net
{
    public class FileDownLoader_Http : FileDownLoader
    {
        private FileStream fileStream;
        private Stream stream;
        private HttpWebResponse response;
        private string tempFileExt = ".temp";
        /// 临时文件全路径
        private string tempSaveFilePath;

        private long currentLength;
        public override long CurrentLength { get { return currentLength; } }
        public virtual long FileLength { get; private set; }
        public override float Progress
        {
            get
            {
                if (FileLength > 0)
                    return Mathf.Clamp((float)currentLength / FileLength, 0, 1);
                return 0;
            }
        }


        public FileDownLoader_Http(string url, string SaveDir) : base(url, SaveDir)
        {
            tempSaveFilePath = string.Format("{0}/{1}{2}", SaveDir, FileNameWithoutExt, tempFileExt);
        }
        public override void DownLoad()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(Url);
            request.Method = "GET";
            if (File.Exists(tempSaveFilePath))
            {
                //若之前已下载了一部分，继续下载
                fileStream = File.OpenWrite(tempSaveFilePath);
                currentLength = fileStream.Length;
                fileStream.Seek(currentLength, SeekOrigin.Current);
                //设置下载的文件读取的起始位置
                request.AddRange((int)currentLength);
            }
            else
            {
                //第一次下载
                fileStream = new FileStream(tempSaveFilePath, FileMode.Create, FileAccess.Write);
                currentLength = 0;
            }

            response = (HttpWebResponse)request.GetResponse();
            stream = response.GetResponseStream();
           
            //总的文件大小=当前需要下载的+已下载的
            FileLength = response.ContentLength + currentLength;

            IsDownLoading = true;
            int lengthOnce;
            int bufferMaxLength = 1024 * 20;

            while (currentLength < FileLength)
            {
                byte[] buffer = new byte[bufferMaxLength];
                if (!stream.CanRead) break;
                //读写操作
                lengthOnce = stream.Read(buffer, 0, buffer.Length);
                currentLength += lengthOnce;
                fileStream.Write(buffer, 0, lengthOnce);
            }
            IsDownLoading = false;
            response.Close();
            stream.Close();
            fileStream.Close();
            //临时文件转为最终的下载文件
            if (File.Exists(SaveFilePath))
                File.Delete(SaveFilePath);
            File.Move(tempSaveFilePath, SaveFilePath);
            Compelete();
        }

        public override void Dispose()
        {
            base.Dispose();
            if (response!=null) response.Close();
            if (stream!=null) stream.Close();
            if (fileStream!=null) fileStream.Close();
        }
    }
}
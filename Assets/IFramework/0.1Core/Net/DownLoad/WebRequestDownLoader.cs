/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace IFramework.Net
{
    public class WebRequestDownLoader : DownLoader
    {
        private UnityWebRequest webRequest;
        public WebRequestDownLoader(string url, string SaveDir, Action<float> CallProgress) : base(url, SaveDir,CallProgress)
        {
            
        }
        CoroutineTask progressTask;
        CoroutineTask DownloadTask;

        public override void DownLoad()
        {
            base.DownLoad();
            DownloadTask = CoroutineTaskManager.CreateTask(Load(CallProgress), null, true);
        }
        private IEnumerator Call()
        {
            while (progress != 1)
            {
                CallProgress(progress);
                yield return 0;
            }
            progressTask.Stop();
            DownloadTask.Stop();

        }
        private IEnumerator Load(Action<float> CallProgress)
        {
            progress = 0;
            if (CallProgress != null) CallProgress(progress);
            webRequest = UnityWebRequest.Get(Url);

            IsDownLoading = true;
            // webRequest.timeout = 30;//设置超时，若webRequest.SendWebRequest()连接超时会返回，且isNetworkError为true
            if (CallProgress!=null) progressTask = CoroutineTaskManager.CreateTask(Call(), null, true);

            yield return webRequest.SendWebRequest();
            IsDownLoading = false;
            if (webRequest.isNetworkError)
            {
                Debug.Log("Download Error:" + webRequest.error);
            }
            else
            {
                File.WriteAllBytes(SaveFilePath, webRequest.downloadHandler.data);
            }
            if (CallProgress != null) CallProgress(1);
        }
        public override float Progress
        {
            get
            {
                if (webRequest != null)
                {
                    progress = webRequest.downloadProgress;
                    return progress;
                }
                return 0;
            }
        }
        public override long CurrentLength
        {
            get
            {
                if (webRequest != null)
                {
                    return (long)webRequest.downloadedBytes;
                }
                return 0;
            }
        }
        public override void Dispose()
        {
            if (webRequest != null)
            {
                webRequest.Dispose();
                webRequest = null;
            }
            if (progressTask!=null) progressTask.Stop();
            if (DownloadTask != null) DownloadTask.Stop();

        }
        public override long FileLength { get { return 0; } }
    }
}
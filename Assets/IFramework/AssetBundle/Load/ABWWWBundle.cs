/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace IFramework.AB
{
#if UNITY_2017_4_OR_NEWER
    public class ABwebRequestBundle : ABBundle
    {
        private UnityWebRequest request;
        public ABwebRequestBundle(string url, Hash128 hash) : base(url, hash) { }
        public override bool IsDone
        {
            get
            {
                if (Err != null) return true;
                if (request.error != null) return true;
                if (request.isDone && this.ab != null) return true;
                return false;
            }
        }
        public override AssetBundle AB
        {
            get
            {
                if (Err != null) return null;
                return DownloadHandlerAssetBundle.GetContent(request);
            }
        }
        protected override void OnLoad()
        {
            request = UnityWebRequestAssetBundle.GetAssetBundle(Path, version);
            request.timeout = 30;
            UnityWebRequestAsyncOperation op = request.SendWebRequest();
            op.completed += Completed;


        }

        private void Completed(AsyncOperation obj)
        {
            if (request.isNetworkError)
            {
                Err = Path + request.error;
            }
            this.ab = DownloadHandlerAssetBundle.GetContent(request);
        }

        protected override void OnUnLoad()
        {
            if (request != null)
            {
                request.Dispose();
            }

        }
    }


#else
     public class ABWWWBundle : ABBundle
    {

        private WWW www;
        public override AssetBundle AB
        {
            get
            {
                if (Err != null) return null;
                return www.assetBundle;
            }
        }

        public override bool IsDone
        {
            get
            {
                if (Err != null) return true;
                if (www.error != null) return true;
                if (www.isDone && www.assetBundle != null) return true;
                return false;
            }
        }


        protected override void OnLoad()
        {
            www = WWW.LoadFromCacheOrDownload(Path, version);
            if (www == null)
            {
                Err = Path + " LoadFromFileAsync falied.";
                if (OnErr != null)
                {
                    OnErr(Err);
                }
            } 
        }

        protected override void OnUnLoad()
        {
            if (www != null)
            {
                if (www.assetBundle != null) www.assetBundle.Unload(true);
                www = null;
            }
        }

        public ABWWWBundle(string url, Hash128 hash) : base(url, hash) { }

    }
#endif
}

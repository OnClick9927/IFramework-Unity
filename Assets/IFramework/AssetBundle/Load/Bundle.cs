/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-28
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace IFramework.AB
{
    public class Bundle : ABReference
    {
        protected void ThrowError(string err)
        {
            InvokeError(string.Format("URL: {0}  Err: {1}", url, err));
        }

        public string name { get; private set; }
        public string url { get; private set; }
        public Hash128 version { get; private set; }


        protected bool m_isdone;
        public override bool IsDone
        {
            get
            {
                return DependenceBundleIsDone() && m_isdone;
            }
        }
        protected bool DependenceBundleIsDone()
        {
            for (int i = 0; i < dpBundles.Count; i++)
                if (!dpBundles[i].IsDone)
                    return false;
            return true;
        }

        public virtual float progress
        {
            get
            {
                return m_ab == null ? 0 : 1;
            }
        }

        protected AssetBundle m_ab;
        public AssetBundle assetbundle { get { return m_ab; } }
        private readonly List<Bundle> dpBundles = new List<Bundle>();



        public Bundle(string url, string name, List<Bundle> dps, Hash128 version)
        {
            this.name = name;
            this.url = url;
            this.version = version;
            for (int i = 0; i < dps.Count; i++)
            {
                dps[i].onError += ThrowError;
                dpBundles.Add(dps[i]);
            }
        }
        public override void Dispose()
        {
            for (int i = 0; i < dpBundles.Count; i++)
            {
                dpBundles[i].onError -= ThrowError;
                dpBundles[i].Release();
            }
            dpBundles.Clear();
        }

        protected override void OnLoad()
        {
            m_ab = AssetBundle.LoadFromFile(url);
            if (m_ab == null)
                ThrowError(" LoadFromFile failed.");
            else
            {
                m_isdone = true;
            }
        }
        protected override void OnUnLoad()
        {
            if (m_ab == null) return;
            m_ab.Unload(true);
            m_ab = null;
        }


        public T LoadAsset<T>(string assetName) where T : Object
        {
            return LoadAsset(assetName, typeof(T)) as T;
        }
        public Object LoadAsset(string assetName, Type type)
        {
            if (!string.IsNullOrEmpty(error) || assetbundle == null) return null;
            return assetbundle.LoadAsset(assetName, type);
        }
        public AssetBundleRequest LoadAssetAsync(string assetName, Type type)
        {
            if (!string.IsNullOrEmpty(error) || assetbundle == null) return null;
            return assetbundle.LoadAssetAsync(assetName, type);
        }


    }
    public class AsyncBundle : Bundle
    {
        private AssetBundleCreateRequest request;
        public override float progress
        {
            get
            {
                if (request == null)
                    return 0;
                return request.progress;
            }
        }

        public AsyncBundle(string url, string name, List<Bundle> dps, Hash128 hash) : base(url, name, dps, hash) { }
        protected override void OnLoad()
        {
            request = AssetBundle.LoadFromFileAsync(url);
            request.completed += (op) =>
            {
                m_isdone = true;
                m_ab = request.assetBundle;
            };
            if (request == null)
            {
                ThrowError(" LoadFromFileAsync falied.");
                m_isdone = true;
            }
        }
        protected override void OnUnLoad()
        {
            if (request == null) return;
            if (request.assetBundle != null)
                request.assetBundle.Unload(true);
            request = null;
        }

    }
    public class WebRequestBundle : Bundle
    {
        private UnityWebRequest request;
        public WebRequestBundle(string url, string name, List<Bundle> dps, Hash128 hash) : base(url, name, dps, hash) { }
        public override float progress
        {
            get
            {
                if (request == null)
                    return 0;
                return request.downloadProgress;
            }
        }
        protected override void OnLoad()
        {
            request = UnityWebRequestAssetBundle.GetAssetBundle(url, version);
            request.timeout = 30;
            UnityWebRequestAsyncOperation op = request.SendWebRequest();
            op.completed += Completed;
        }

        private void Completed(AsyncOperation obj)
        {
            if (request.isNetworkError)
                ThrowError(request.error);
            else
                m_ab = DownloadHandlerAssetBundle.GetContent(request);
            m_isdone = true;
        }

        protected override void OnUnLoad()
        {
            if (request == null) return;
            if (m_ab != null)
                m_ab.Unload(true);
            request.Dispose();
            request = null;
        }
    }
}

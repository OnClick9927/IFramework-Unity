/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-28
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IFramework.AB
{
    public class Asset : ABReference
    {
        protected void ThrowError(string err)
        {
            InvokeError(string.Format("ABAsset:  {0}", err));
        }
        public Type assetType { get; private set; }
        protected Object m_asset;
        public Object asset { get { return m_asset; } }
        public string assetPath { get; private set; }
        protected bool m_isdone;
        public override bool IsDone { get { return m_isdone; } }
        public Asset(string path, Type type)
        {
            assetPath = path;
            assetType = type;

        }
        protected override void OnLoad()
        {
#if UNITY_EDITOR
            try
            {
                m_asset = UnityEditor.AssetDatabase.LoadAssetAtPath(assetPath, assetType);
                m_isdone = true;
            }
            catch (Exception e)
            {
                ThrowError(e.Message);
            }
#endif
        }

        protected override void OnUnLoad()
        {
            assetPath = null;
            m_asset = null;
        }
    }
    public class BundleAsset : Asset
    {
        protected Bundle bundle;
        internal BundleAsset(string path, Type type) : base(path, type) { }
        protected override void OnLoad()
        {
            bundle = ABAssets.Bundles.LoadSync(ABAssets.GetBundleName(assetPath));
            bundle.onError += ThrowError;
            m_asset = bundle.LoadAsset(ABAssets.GetAssetName(assetPath), assetType);
            m_isdone = true;
        }
        protected override void OnUnLoad()
        {
            base.OnUnLoad();
            if (bundle != null)
            {
                bundle.onError -= ThrowError;
                bundle.Release();
                bundle = null;
            }
        }

    }
    public class AsyncBundleAsset : BundleAsset
    {
        private AssetBundleRequest request;
        public AsyncBundleAsset(string path, Type type) : base(path, type) { }
        protected override void OnLoad()
        {
            bundle = ABAssets.Bundles.LoadAsync(ABAssets.GetBundleName(assetPath));
            bundle.onError += ThrowError;
        }
        protected override void OnUnLoad()
        {
            base.OnUnLoad();
            request = null;
        }
        public override bool IsDone
        {
            get
            {
                if (bundle == null) return false;
                if (!bundle.IsDone) return false;
                if (request == null)
                {
                    request = bundle.LoadAssetAsync(Path.GetFileName(assetPath), assetType);
                    request.completed += (op) => {
                        m_asset = request.asset;
                    };
                    return false;
                }
                return request.isDone;
            }
        }
    }
}

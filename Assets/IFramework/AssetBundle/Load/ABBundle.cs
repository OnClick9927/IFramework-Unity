/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;
namespace IFramework.AB
{
    public class ABBundle:Reference
	{
        public Action<string> OnErr;
        public int ReferenceCount;
        public virtual string Err { get; protected set; }
        public virtual float Progress { get; protected set; }
        public virtual bool IsDone { get; protected set; }
        public string Path { get; protected set; }
        public string ABName { get; internal set; }
        public virtual AssetBundle AB { get { return ab; } }
        public readonly List<ABBundle> dpBundles = new List<ABBundle>();
        protected AssetBundle ab;
        protected Hash128 version;
        public ABBundle(string url, Hash128 version)
        {
            Path = url;
            this.version = version;
        }
        public void Load() { OnLoad(); }
        protected virtual void OnLoad() {
            ab = AssetBundle.LoadFromFile(Path);
            if (ab == null)
            {
                Err = Path + " LoadFromFile failed.";
                if (OnErr!=null)
                {
                    OnErr(Err);
                }
            }
        }
        public void UnLoad() { OnUnLoad(); }
        protected virtual void OnUnLoad() {
            if (ab == null) return;
            ab.Unload(true);
            ab = null;
        }
        public T LoadAsset<T>(string assetName) where T : Object
        {
            //if (Err != null) return null;
            //if (ab == null) return null;
            //return ab.LoadAsset(assetName, typeof(T)) as T;
            return LoadAsset(assetName, typeof(T)) as T;
        }
        public Object LoadAsset(string assetName, System.Type assetType)
        {
            if (Err != null) return null;
            return AB.LoadAsset(assetName, assetType);
        }
        public AssetBundleRequest LoadAssetAsync(string assetName, System.Type assetType)
        {
            if (Err != null) return null;
            if (AB == null) return null;
            return AB.LoadAssetAsync(assetName, assetType);
        }
      
    }
    public class Reference:SimpleReference
    {
        public bool IsUnused {
            get { return RefCount <= 0; }
        }
        public override void Retain(object user = null)
        {
            base.Retain(user);
        }
        public override void Release(object user = null)
        {
            base.Release(user);
        }
        protected override void OnZero()
        {
            base.OnZero();
        }

    }
    public class ABAsyncBundle : ABBundle
    {
        private AssetBundleCreateRequest request;
        public override AssetBundle AB
        {
            get
            {
                if (Err != null) return null;
                return request.assetBundle;
            }
        }
        public override bool IsDone
        {
            get
            {
                if (Err != null) return true;
                return request.isDone;
            }
        }
        public ABAsyncBundle(string url, Hash128 hash) : base(url, hash) { }
        protected override void OnLoad()
        {
            request = AssetBundle.LoadFromFileAsync(Path);
            if (request == null)
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
            if (request == null) return;
            if (request.assetBundle != null)
            {
                request.assetBundle.Unload(true);
            }
            request = null;
        }
        public override float Progress
        {
            get { return request.progress; }
        }
    }
}

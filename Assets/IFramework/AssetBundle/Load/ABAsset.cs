/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace IFramework.AB
{
    public class ABAsset:Reference
    {
        public virtual UnityEngine.Object Asset { get; protected set; }
        public string AssetPath { get; private set; }
        public Type AssetType { get; private set; }
        public virtual bool IsDone { get { return true; } }
        public Action<ABAsset> OnComplete;
        public ABAsset(string path, Type type)
        {
            AssetPath = path;
            AssetType = type;

        }
        public void Load() { OnLoad(); }
        public virtual void OnLoad()
        {
#if UNITY_EDITOR
            Asset = UnityEditor.AssetDatabase.LoadAssetAtPath(AssetPath, AssetType);
#endif
        }
        public void UnLoad() {

            Asset = null;
            OnUnLoad();
            AssetPath = null;
        }
        public virtual void OnUnLoad() { }

        public override void Retain(object user=null)
        {
            base.Retain();
            if (IsDone)
            {
                if (OnComplete != null)
                {
                    OnComplete(this);
                    OnComplete = null;
                }
            }
        }

     
    }
    public class ABBundleAsset : ABAsset
    {
        protected ABBundle bundle;
        internal ABBundleAsset(string path, System.Type type) : base(path, type) { }
        public override void OnLoad()
        {
            bundle = ABBundles.LoadSync(ABAssets.GetBundleName(AssetPath));
            Asset = bundle.LoadAsset(ABAssets.GetAssetName(AssetPath), AssetType);
        }
        public override void OnUnLoad()
        {
            if (bundle != null) bundle.Release();
            bundle = null;
        }

    }

}

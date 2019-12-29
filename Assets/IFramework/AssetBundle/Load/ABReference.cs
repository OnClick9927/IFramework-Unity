/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-28
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Utility;
using System;

namespace IFramework.AB
{
    public abstract class ABReference : SimpleReference, IDisposable
    {
        public event Action<string> onError;
        public string error { get; private set; }
        protected void InvokeError(string err)
        {
            error = err;
            if (onError != null)
                onError(error);
        }
        public abstract bool IsDone { get; }
        public bool IsUnused
        {
            get { return RefCount <= 0; }
        }

        internal void Load() { OnLoad(); }
        internal void UnLoad() { OnUnLoad(); }


        protected abstract void OnLoad();
        protected abstract void OnUnLoad();

        public virtual void Dispose()
        {

        }
    }

}

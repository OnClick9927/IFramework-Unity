/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using System;

namespace IFramework.UI
{
    public class UIAsyncOperation
    {
        public Action completed;
        public bool _isDone = false;

        public bool isDone { get { return _isDone; } }

        protected void SetToDefault()
        {
            completed = null;
            _isDone = false;
        }
        protected void Compelete()
        {
            _isDone = true;
            completed?.Invoke();
            completed = null;
        }
    }
    public class UIAsyncOperation<T> : UIAsyncOperation
    {
        public T value;
        protected new void SetToDefault()
        {
            base.SetToDefault();
            _isDone = false;
        }
        public void SetValue(T value)
        {
            this.value = value;
            base.Compelete();
        }
    }
}

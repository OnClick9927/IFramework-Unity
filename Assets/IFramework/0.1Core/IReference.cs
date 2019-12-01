/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-07-10
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;

namespace IFramework
{
    public interface IReference
    {
        int RefCount { get; }
        void Retain(object user = null);
        void Release(object user = null);
    }
    public class SimpleReference : IReference
    {
        public SimpleReference()
        {
            RefCount = 0;
        }

        public int RefCount { get; private set; }

        public virtual void Retain(object user = null)
        {
            ++RefCount;
        }

        public virtual void Release(object user = null)
        {
            --RefCount;
            if (RefCount == 0)
            {
                OnZero();
            }
        }

        protected virtual void OnZero()
        {
        }
    }
    public class Reference : IReference
    {
        public int RefCount { get { return users.Count; } }
        public HashSet<object> Users { get { return users; } }
        private readonly HashSet<object> users = new HashSet<object>();
        public virtual void Retain(object user)
        {
            if (!Users.Add(user))
            {
                Log.E("ObjectIsAlreadyRetainedByOwnerException");
            }
        }

        public virtual void Release(object user)
        {
            if (!Users.Remove(user))
            {
                Log.E("ObjectIsNotRetainedByOwnerExceptionWithHint");
            }
        }
    }
}

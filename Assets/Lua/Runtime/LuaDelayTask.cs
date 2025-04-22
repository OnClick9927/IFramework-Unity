/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
using System;
using System.Collections;

namespace IFramework.Lua
{

    public class TaskForLua<T> : TaskForLua
    {
        public T value;
        public void SetComplete(T value)
        {
            this.value = value;
            base.SetComplete();
        }
    }
    public class TaskForLua
    {
        private bool isDone;
        public Action completed;
        public bool IsCompleted => isDone;

        public void SetComplete()
        {
            isDone = true;
            completed?.Invoke();
        }
    }
    public class LuaDelayTask : TaskForLua
    {

        public LuaDelayTask(float second)
        {
            LuaHotFix.Instance.StartCoroutine(IE(second));
        }


        private IEnumerator IE(float second)
        {
            yield return new WaitForSeconds(second);
            SetComplete();
        }
    }


}

/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2021-06-27
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;

namespace IFramework.UI
{
    public class MixedGroups : IGroups
    {
        private readonly IGroups[] _groups;
        private Dictionary<string, IGroups> _nameMap;
        public MixedGroups(IGroups[] groups)
        {
            this._groups = groups;
            _nameMap = new Dictionary<string, IGroups>();
        }

        void IDisposable.Dispose()
        {
            for (int i = 0; i < _groups.Length; i++)
            {
                _groups[i].Dispose();
            }
        }


        void IGroups.OnClose(string path)
        {
            if (_nameMap.ContainsKey(path))
            {
                _nameMap[path].OnClose(path);
            }
            else
            {
                UnityEngine.Debug.LogError("the panel have not subscribe  panel name :" + path);
            }
        }

        void IGroups.OnHide(string path)
        {
            if (_nameMap.ContainsKey(path))
            {
                _nameMap[path].OnHide(path);
            }
            else
            {
                UnityEngine.Debug.LogError("the panel have not subscribe  panel name :" + path);
            }
        }

        void IGroups.OnShow(string path)
        {
            if (_nameMap.ContainsKey(path))
            {
                _nameMap[path].OnShow(path);
            }
            else
            {
                UnityEngine.Debug.LogError("the panel have not subscribe  panel name :" + path);
            }
        }

        public void OnLoad(string path)
        {
            if (_nameMap.ContainsKey(path))
            {
                _nameMap[path].OnLoad(path);
            }
            else
            {
                UnityEngine.Debug.LogError("the panel have not subscribe  panel name :" + path);
            }
        }

        bool IGroups.Subscribe(string path, UIPanel panel)
        {
            bool sucess = false;
            for (int i = 0; i < _groups.Length; i++)
            {
                sucess |= _groups[i].Subscribe(path,panel);
                if (sucess)
                {
                    if (_nameMap.ContainsKey(path))
                    {
                        UnityEngine.Debug.LogError("Same name, can't Subscribe the panel with name " + path);
                        return false;
                    }
                    _nameMap[path] = _groups[i];
                    break;
                }
            }
            if (!sucess)
            {
                UnityEngine.Debug.LogError("can't Subscribe the panel with name " + path);
            }
            return sucess;
        }

        bool IGroups.UnSubscribe(string path)
        {
            if (_nameMap.ContainsKey(path))
            {
                return _nameMap[path].UnSubscribe(path);
            }
            else
            {
                UnityEngine.Debug.LogError("the panel have not subscribe  panel name :" + path);
                return false;
            }
        }

    }
}

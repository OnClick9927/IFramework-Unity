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
    public class MixedViewBridge : IViewBridge
    {
        private readonly IViewBridge[] _bridges;
        private Dictionary<string, IViewBridge> _nameMap;
        public MixedViewBridge(IViewBridge[] bridges)
        {
            this._bridges = bridges;
            _nameMap = new Dictionary<string, IViewBridge>();
        }

        void IDisposable.Dispose()
        {
            for (int i = 0; i < _bridges.Length; i++)
            {
                _bridges[i].Dispose();
            }
        }


        void IViewBridge.OnClose(string path)
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

        void IViewBridge.OnHide(string path)
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

        void IViewBridge.OnShow(string path)
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
        bool IViewBridge.Subscribe(string path, UIPanel panel)
        {
            bool sucess = false;
            for (int i = 0; i < _bridges.Length; i++)
            {
                sucess |= _bridges[i].Subscribe(path,panel);
                if (sucess)
                {
                    if (_nameMap.ContainsKey(path))
                    {
                        UnityEngine.Debug.LogError("Same name, can't Subscribe the panel with name " + path);
                        return false;
                    }
                    _nameMap[path] = _bridges[i];
                    break;
                }
            }
            if (!sucess)
            {
                UnityEngine.Debug.LogError("can't Subscribe the panel with name " + path);
            }
            return sucess;
        }

        bool IViewBridge.UnSubscribe(string path)
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

        void IViewBridge.OnLoad(string path)
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

        void IViewBridge.OnBecameVisible(string path)
        {
            if (_nameMap.ContainsKey(path))
            {
                _nameMap[path].OnBecameVisible(path);
            }
            else
            {
                UnityEngine.Debug.LogError("the panel have not subscribe  panel name :" + path);
            }
        }

        void IViewBridge.OnBecameInvisible(string path)
        {
            if (_nameMap.ContainsKey(path))
            {
                _nameMap[path].OnBecameInvisible(path);
            }
            else
            {
                UnityEngine.Debug.LogError("the panel have not subscribe  panel name :" + path);
            }
        }
    }
}

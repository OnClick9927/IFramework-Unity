/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using XLua;
using UnityEngine;
using System.Collections.Generic;

namespace IFramework.Hotfix.Lua
{
    public class XLuaModule : UpdateModule
    {
        private List<IXLuaDisposable> disposes = new List<IXLuaDisposable>();
        private LuaEnv _luaenv;
        private List<LuaTable> _tables;
        private static float _lastGCTime;
        public LuaTable gtable { get { return _luaenv.Global; } }
        public float gcInterval = 1f;
        public static bool available { get; private set; }
        public void Subscribe(IXLuaDisposable dispose)
        {
            if (!disposes.Contains(dispose))
            {
                disposes.Add(dispose);
            }
        }

        public void UnSubscribe(IXLuaDisposable dispose)
        {
            if (disposes.Contains(dispose))
            {
                disposes.Remove(dispose);
            }
        }

        public LuaTable NewTable()
        {
            LuaTable table = _luaenv.NewTable();
            _tables.Add(table);
            return table;
        }
        public void AddLoader(IXLuaLoader loader)
        {
            _luaenv.AddLoader(loader.load);
        }

        public LuaFunction LoadString(string chunk, string chunkName = "chunk", LuaTable env = null)
        {
            return _luaenv.LoadString(chunk, chunkName, env);
        }
        public T LoadString<T>(string chunk, string chunkName = "chunk", LuaTable env = null)
        {
            return _luaenv.LoadString<T>(chunk, chunkName, env);
        }
        public T LoadString<T>(byte[] chunk, string chunkName = "chunk", LuaTable env = null)
        {
            return _luaenv.LoadString<T>(chunk, chunkName, env);
        }



        public object[] DoString(byte[] chunk, string chunkName = "chunk", LuaTable env = null)
        {
            return _luaenv.DoString(chunk, chunkName, env);
        }
        public object[] DoString(string chunk, string chunkName = "chunk", LuaTable env = null)
        {
            return _luaenv.DoString(chunk, chunkName, env);
        }



        public LuaTable GetTable(TextAsset luaScript, string chunkName = "chunk")
        {
            //// 为每个脚本设置一个独立的环境，可一定程度上防止脚本间全局变量、函数冲突
            LuaTable meta = NewTable();
            meta.Set("__index", gtable);
            LuaTable scriptEnv = NewTable();
            scriptEnv.SetMetaTable(meta);
            meta.Dispose();
            DoString(luaScript.text, chunkName, scriptEnv);
            return scriptEnv;
        }


        public void FullGc()
        {
            _luaenv.FullGc();
        }

        protected override void Awake()
        {
            available = true;
            _luaenv = new LuaEnv();
            _tables = new List<LuaTable>();
        }

        protected override void OnDispose()
        {
            for (int i = disposes.Count - 1; i >= 0; i--)
            {
                if (disposes[i] != null)
                {
                    disposes[i].LuaDispose();
                }
            }
            available = false;
            _tables.ForEach((table) =>
            {
                table.Dispose();
            });
            //DoString(@"
            //         local util = require 'xlua.util'
            //         util.print_func_ref_by_csharp()
            // ");
            //_luaenv.Dispose();
            _luaenv = null;
        }

        protected override void OnUpdate()
        {
            if (_luaenv == null) return;
            if (Time.time - _lastGCTime > gcInterval)
            {
                _luaenv.Tick();
                _lastGCTime = Time.time;
            }
        }
    }
}

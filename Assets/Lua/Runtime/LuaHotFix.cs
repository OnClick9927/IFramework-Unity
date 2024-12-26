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
using System;

namespace IFramework.Lua
{


    [MonoSingletonPath("Lua")]
    public class LuaHotFix : MonoSingleton<LuaHotFix>
    {
        private LuaEnv _luaenv;
        private List<LuaTable> _tables;
        private static float _lastGCTime;
        public static LuaEnvConfig config { get; private set; }
        private Func<string, byte[]> scriptLoad;


        public LuaTable gtable { get { return _luaenv.Global; } }
        public float gcInterval = 1f;
        public static bool available { get; private set; }

        public LuaTable NewTable()
        {
            LuaTable table = _luaenv.NewTable();
            _tables.Add(table);
            return table;
        }


        public void Init(LuaEnvConfig config, Func<string, byte[]> scriptLoad)
        {
            available = true;
            _luaenv = new LuaEnv();
            _tables = new List<LuaTable>();
            _luaenv.AddLoader(Load);
            LuaHotFix.config = config;
            this.scriptLoad = scriptLoad;

            EnterLua();
        }
        private void EnterLua()
        {
#if UNITY_EDITOR
            DoString("require 'Framework.UpdateFunctions'");
#endif

            DoString("require 'Framework.Main'" +
                             " Awake()");
        }
        public void QuitLua()
        {
            DoString("require 'Framework.Main'" +
                            "OnDispose()");
        }

        private byte[] Load(ref string path)
        {
            if (path.EndsWith(".lua"))
                path = path.Replace(".lua", "");
            path = path.Replace(".", "/");
            var filepath = System.IO.Path.Combine(config.rootPath, $"{path}.lua.txt");
            return scriptLoad?.Invoke(filepath);
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


        private void OnDisable()
        {
            QuitLua();
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
        private void Update()
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

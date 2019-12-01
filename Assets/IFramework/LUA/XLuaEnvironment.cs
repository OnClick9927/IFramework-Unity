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


namespace IFramework
{
    public interface IXLuaLoader
    {
        byte[] load(ref string filepath);
    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class XLuaLoaderAttribute : Attribute { }
    public class XLuaEnvironment :MonoBehaviour,ISingleton
	{
        void ISingleton.OnSingletonInit()
        {
            Disposed = false;
            LuaEnv = new LuaEnv();
            tables = new List<LuaTable>();
            typeof(IXLuaLoader).GetSubTypesInAssemblys().ForEach((type) =>
            {
                if (type.IsAbstract || !type.IsDefined(typeof(XLuaLoaderAttribute),false)) return;
                IXLuaLoader loader= Activator.CreateInstance(type) as IXLuaLoader;
                AddLoader(loader.load);
            });
        }

        public void Dispose()
        {
            if (OnDispose != null) OnDispose();
            tables.ForEach((table) =>
            {
                table.Dispose();
            });
            DoString(@"
                    local util = require 'xlua.util'
                   -- util.print_func_ref_by_csharp()
            ");
            LuaEnv.Dispose();
            LuaEnv = null;
            Disposed = true;
        }
        public static Action OnDispose;
        private void OnDestroy()
        {
            Dispose();
        }
        private LuaEnv LuaEnv;
        private static XLuaEnvironment  Instance { get { return MonoSingletonProperty<XLuaEnvironment >.Instance; } }
        private List<LuaTable> tables;
        public static LuaTable Global { get { return Instance.LuaEnv.Global; } }
        private static float LastGCTime;
        public static float GCInterval=1f;
        public static bool Disposed { get; private set; }
        private void Update()
        {
            if (LuaEnv == null) return;
            if (Time.time - LastGCTime > GCInterval)
            {
                LuaEnv.Tick();
                LastGCTime = Time.time;
            }
        }
        public static object[] DoString(string chunk, string chunkName = "chunk", LuaTable env = null)
        {
            return Instance.LuaEnv.DoString(chunk, chunkName, env);
        }
        public static LuaTable NewTable()
        {
            LuaTable table = Instance.LuaEnv.NewTable();
            Instance.tables.Add(table);
            return table;
        }
        public static void AddLoader(LuaEnv.CustomLoader loader)
        {
           Instance. LuaEnv.AddLoader(loader);
        }
    }
}

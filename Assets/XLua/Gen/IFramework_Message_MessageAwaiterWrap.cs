﻿#if USE_UNI_LUA
using LuaAPI = UniLua.Lua;
using RealStatePtr = UniLua.ILuaState;
using LuaCSFunction = UniLua.CSharpFunctionDelegate;
#else
using LuaAPI = XLua.LuaDLL.Lua;
using RealStatePtr = System.IntPtr;
using LuaCSFunction = XLua.LuaDLL.lua_CSFunction;
#endif

using XLua;
using System.Collections.Generic;


namespace XLua.CSObjectWrap
{
    using Utils = XLua.Utils;
    public class IFrameworkMessageMessageAwaiterWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(IFramework.Message.MessageAwaiter);
			Utils.BeginObjectRegister(type, L, translator, 0, 3, 1, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetResult", _m_GetResult);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "OnCompleted", _m_OnCompleted);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "UnsafeOnCompleted", _m_UnsafeOnCompleted);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "IsCompleted", _g_get_IsCompleted);
            
			
			
			Utils.EndObjectRegister(type, L, translator, null, null,
			    null, null, null);

		    Utils.BeginClassRegister(type, L, __CreateInstance, 1, 0, 0);
			
			
            
			
			
			
			Utils.EndClassRegister(type, L, translator);
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int __CreateInstance(RealStatePtr L)
        {
            
			try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
				if(LuaAPI.lua_gettop(L) == 2 && translator.Assignable<IFramework.Message.IMessage>(L, 2))
				{
					IFramework.Message.IMessage _cor = (IFramework.Message.IMessage)translator.GetObject(L, 2, typeof(IFramework.Message.IMessage));
					
					var gen_ret = new IFramework.Message.MessageAwaiter(_cor);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
				if (LuaAPI.lua_gettop(L) == 1)
				{
				    translator.Push(L, default(IFramework.Message.MessageAwaiter));
			        return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to IFramework.Message.MessageAwaiter constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetResult(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                IFramework.Message.MessageAwaiter gen_to_be_invoked;translator.Get(L, 1, out gen_to_be_invoked);
            
            
                
                {
                    
                    gen_to_be_invoked.GetResult(  );
                    
                    
                        translator.Update(L, 1, gen_to_be_invoked);
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_OnCompleted(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                IFramework.Message.MessageAwaiter gen_to_be_invoked;translator.Get(L, 1, out gen_to_be_invoked);
            
            
                
                {
                    System.Action _continuation = translator.GetDelegate<System.Action>(L, 2);
                    
                    gen_to_be_invoked.OnCompleted( _continuation );
                    
                    
                        translator.Update(L, 1, gen_to_be_invoked);
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_UnsafeOnCompleted(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                IFramework.Message.MessageAwaiter gen_to_be_invoked;translator.Get(L, 1, out gen_to_be_invoked);
            
            
                
                {
                    System.Action _continuation = translator.GetDelegate<System.Action>(L, 2);
                    
                    gen_to_be_invoked.UnsafeOnCompleted( _continuation );
                    
                    
                        translator.Update(L, 1, gen_to_be_invoked);
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_IsCompleted(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                IFramework.Message.MessageAwaiter gen_to_be_invoked;translator.Get(L, 1, out gen_to_be_invoked);
                LuaAPI.lua_pushboolean(L, gen_to_be_invoked.IsCompleted);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
		
		
		
		
    }
}

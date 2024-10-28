#if USE_UNI_LUA
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
    public class UIGameAssetWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(UIGame.Asset);
			Utils.BeginObjectRegister(type, L, translator, 0, 4, 0, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LoadItemAsync", _m_LoadItemAsync);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LoadPanel", _m_LoadPanel);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ReleaseItemAsset", _m_ReleaseItemAsset);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "LoadPanelAsync", _m_LoadPanelAsync);
			
			
			
			
			
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
				if(LuaAPI.lua_gettop(L) == 3 && translator.Assignable<IFramework.UI.UILayerData>(L, 2) && translator.Assignable<IFramework.UI.PanelCollection>(L, 3))
				{
					IFramework.UI.UILayerData _layer = (IFramework.UI.UILayerData)translator.GetObject(L, 2, typeof(IFramework.UI.UILayerData));
					IFramework.UI.PanelCollection _collection = (IFramework.UI.PanelCollection)translator.GetObject(L, 3, typeof(IFramework.UI.PanelCollection));
					
					var gen_ret = new UIGame.Asset(_layer, _collection);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to UIGame.Asset constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadItemAsync(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                UIGame.Asset gen_to_be_invoked = (UIGame.Asset)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    IFramework.UI.LoadItemAsyncOperation _op = (IFramework.UI.LoadItemAsyncOperation)translator.GetObject(L, 2, typeof(IFramework.UI.LoadItemAsyncOperation));
                    
                        var gen_ret = gen_to_be_invoked.LoadItemAsync( _op );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadPanel(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                UIGame.Asset gen_to_be_invoked = (UIGame.Asset)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.RectTransform _parent = (UnityEngine.RectTransform)translator.GetObject(L, 2, typeof(UnityEngine.RectTransform));
                    string _path = LuaAPI.lua_tostring(L, 3);
                    
                        var gen_ret = gen_to_be_invoked.LoadPanel( _parent, _path );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ReleaseItemAsset(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                UIGame.Asset gen_to_be_invoked = (UIGame.Asset)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.GameObject _gameObject = (UnityEngine.GameObject)translator.GetObject(L, 2, typeof(UnityEngine.GameObject));
                    
                    gen_to_be_invoked.ReleaseItemAsset( _gameObject );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_LoadPanelAsync(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                UIGame.Asset gen_to_be_invoked = (UIGame.Asset)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    IFramework.UI.LoadPanelAsyncOperation _op = (IFramework.UI.LoadPanelAsyncOperation)translator.GetObject(L, 2, typeof(IFramework.UI.LoadPanelAsyncOperation));
                    
                        var gen_ret = gen_to_be_invoked.LoadPanelAsync( _op );
                        LuaAPI.lua_pushboolean(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        
        
        
        
        
		
		
		
		
    }
}

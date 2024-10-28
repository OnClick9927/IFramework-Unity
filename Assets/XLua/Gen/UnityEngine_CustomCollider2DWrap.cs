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
    public class UnityEngineCustomCollider2DWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(UnityEngine.CustomCollider2D);
			Utils.BeginObjectRegister(type, L, translator, 0, 4, 2, 0);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetCustomShapes", _m_GetCustomShapes);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetCustomShapes", _m_SetCustomShapes);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetCustomShape", _m_SetCustomShape);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "ClearCustomShapes", _m_ClearCustomShapes);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "customShapeCount", _g_get_customShapeCount);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "customVertexCount", _g_get_customVertexCount);
            
			
			
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
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					var gen_ret = new UnityEngine.CustomCollider2D();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to UnityEngine.CustomCollider2D constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetCustomShapes(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                UnityEngine.CustomCollider2D gen_to_be_invoked = (UnityEngine.CustomCollider2D)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& translator.Assignable<UnityEngine.PhysicsShapeGroup2D>(L, 2)) 
                {
                    UnityEngine.PhysicsShapeGroup2D _physicsShapeGroup = (UnityEngine.PhysicsShapeGroup2D)translator.GetObject(L, 2, typeof(UnityEngine.PhysicsShapeGroup2D));
                    
                        var gen_ret = gen_to_be_invoked.GetCustomShapes( _physicsShapeGroup );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 4&& translator.Assignable<UnityEngine.PhysicsShapeGroup2D>(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)) 
                {
                    UnityEngine.PhysicsShapeGroup2D _physicsShapeGroup = (UnityEngine.PhysicsShapeGroup2D)translator.GetObject(L, 2, typeof(UnityEngine.PhysicsShapeGroup2D));
                    int _shapeIndex = LuaAPI.xlua_tointeger(L, 3);
                    int _shapeCount = LuaAPI.xlua_tointeger(L, 4);
                    
                        var gen_ret = gen_to_be_invoked.GetCustomShapes( _physicsShapeGroup, _shapeIndex, _shapeCount );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 3&& translator.Assignable<UnityEngine.PhysicsShapeGroup2D>(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    UnityEngine.PhysicsShapeGroup2D _physicsShapeGroup = (UnityEngine.PhysicsShapeGroup2D)translator.GetObject(L, 2, typeof(UnityEngine.PhysicsShapeGroup2D));
                    int _shapeIndex = LuaAPI.xlua_tointeger(L, 3);
                    
                        var gen_ret = gen_to_be_invoked.GetCustomShapes( _physicsShapeGroup, _shapeIndex );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 3&& translator.Assignable<Unity.Collections.NativeArray<UnityEngine.PhysicsShape2D>>(L, 2)&& translator.Assignable<Unity.Collections.NativeArray<UnityEngine.Vector2>>(L, 3)) 
                {
                    Unity.Collections.NativeArray<UnityEngine.PhysicsShape2D> _shapes;translator.Get(L, 2, out _shapes);
                    Unity.Collections.NativeArray<UnityEngine.Vector2> _vertices;translator.Get(L, 3, out _vertices);
                    
                        var gen_ret = gen_to_be_invoked.GetCustomShapes( _shapes, _vertices );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to UnityEngine.CustomCollider2D.GetCustomShapes!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetCustomShapes(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                UnityEngine.CustomCollider2D gen_to_be_invoked = (UnityEngine.CustomCollider2D)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 2&& translator.Assignable<UnityEngine.PhysicsShapeGroup2D>(L, 2)) 
                {
                    UnityEngine.PhysicsShapeGroup2D _physicsShapeGroup = (UnityEngine.PhysicsShapeGroup2D)translator.GetObject(L, 2, typeof(UnityEngine.PhysicsShapeGroup2D));
                    
                    gen_to_be_invoked.SetCustomShapes( _physicsShapeGroup );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& translator.Assignable<Unity.Collections.NativeArray<UnityEngine.PhysicsShape2D>>(L, 2)&& translator.Assignable<Unity.Collections.NativeArray<UnityEngine.Vector2>>(L, 3)) 
                {
                    Unity.Collections.NativeArray<UnityEngine.PhysicsShape2D> _shapes;translator.Get(L, 2, out _shapes);
                    Unity.Collections.NativeArray<UnityEngine.Vector2> _vertices;translator.Get(L, 3, out _vertices);
                    
                    gen_to_be_invoked.SetCustomShapes( _shapes, _vertices );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to UnityEngine.CustomCollider2D.SetCustomShapes!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetCustomShape(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                UnityEngine.CustomCollider2D gen_to_be_invoked = (UnityEngine.CustomCollider2D)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 4&& translator.Assignable<UnityEngine.PhysicsShapeGroup2D>(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)) 
                {
                    UnityEngine.PhysicsShapeGroup2D _physicsShapeGroup = (UnityEngine.PhysicsShapeGroup2D)translator.GetObject(L, 2, typeof(UnityEngine.PhysicsShapeGroup2D));
                    int _srcShapeIndex = LuaAPI.xlua_tointeger(L, 3);
                    int _dstShapeIndex = LuaAPI.xlua_tointeger(L, 4);
                    
                    gen_to_be_invoked.SetCustomShape( _physicsShapeGroup, _srcShapeIndex, _dstShapeIndex );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 5&& translator.Assignable<Unity.Collections.NativeArray<UnityEngine.PhysicsShape2D>>(L, 2)&& translator.Assignable<Unity.Collections.NativeArray<UnityEngine.Vector2>>(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 5)) 
                {
                    Unity.Collections.NativeArray<UnityEngine.PhysicsShape2D> _shapes;translator.Get(L, 2, out _shapes);
                    Unity.Collections.NativeArray<UnityEngine.Vector2> _vertices;translator.Get(L, 3, out _vertices);
                    int _srcShapeIndex = LuaAPI.xlua_tointeger(L, 4);
                    int _dstShapeIndex = LuaAPI.xlua_tointeger(L, 5);
                    
                    gen_to_be_invoked.SetCustomShape( _shapes, _vertices, _srcShapeIndex, _dstShapeIndex );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to UnityEngine.CustomCollider2D.SetCustomShape!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_ClearCustomShapes(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                UnityEngine.CustomCollider2D gen_to_be_invoked = (UnityEngine.CustomCollider2D)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 1) 
                {
                    
                    gen_to_be_invoked.ClearCustomShapes(  );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    int _shapeIndex = LuaAPI.xlua_tointeger(L, 2);
                    int _shapeCount = LuaAPI.xlua_tointeger(L, 3);
                    
                    gen_to_be_invoked.ClearCustomShapes( _shapeIndex, _shapeCount );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to UnityEngine.CustomCollider2D.ClearCustomShapes!");
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_customShapeCount(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                UnityEngine.CustomCollider2D gen_to_be_invoked = (UnityEngine.CustomCollider2D)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.customShapeCount);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_customVertexCount(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                UnityEngine.CustomCollider2D gen_to_be_invoked = (UnityEngine.CustomCollider2D)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.customVertexCount);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
		
		
		
		
    }
}

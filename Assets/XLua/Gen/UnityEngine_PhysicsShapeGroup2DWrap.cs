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
    public class UnityEnginePhysicsShapeGroup2DWrap 
    {
        public static void __Register(RealStatePtr L)
        {
			ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			System.Type type = typeof(UnityEngine.PhysicsShapeGroup2D);
			Utils.BeginObjectRegister(type, L, translator, 0, 15, 3, 1);
			
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Clear", _m_Clear);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "Add", _m_Add);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetShapeData", _m_GetShapeData);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetShapeVertices", _m_GetShapeVertices);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetShapeVertex", _m_GetShapeVertex);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetShapeVertex", _m_SetShapeVertex);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetShapeRadius", _m_SetShapeRadius);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "SetShapeAdjacentVertices", _m_SetShapeAdjacentVertices);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "DeleteShape", _m_DeleteShape);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "GetShape", _m_GetShape);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddCircle", _m_AddCircle);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddCapsule", _m_AddCapsule);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddBox", _m_AddBox);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddPolygon", _m_AddPolygon);
			Utils.RegisterFunc(L, Utils.METHOD_IDX, "AddEdges", _m_AddEdges);
			
			
			Utils.RegisterFunc(L, Utils.GETTER_IDX, "shapeCount", _g_get_shapeCount);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "vertexCount", _g_get_vertexCount);
            Utils.RegisterFunc(L, Utils.GETTER_IDX, "localToWorldMatrix", _g_get_localToWorldMatrix);
            
			Utils.RegisterFunc(L, Utils.SETTER_IDX, "localToWorldMatrix", _s_set_localToWorldMatrix);
            
			
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
				if(LuaAPI.lua_gettop(L) == 3 && LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2) && LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3))
				{
					int _shapeCapacity = LuaAPI.xlua_tointeger(L, 2);
					int _vertexCapacity = LuaAPI.xlua_tointeger(L, 3);
					
					var gen_ret = new UnityEngine.PhysicsShapeGroup2D(_shapeCapacity, _vertexCapacity);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				if(LuaAPI.lua_gettop(L) == 2 && LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 2))
				{
					int _shapeCapacity = LuaAPI.xlua_tointeger(L, 2);
					
					var gen_ret = new UnityEngine.PhysicsShapeGroup2D(_shapeCapacity);
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				if(LuaAPI.lua_gettop(L) == 1)
				{
					
					var gen_ret = new UnityEngine.PhysicsShapeGroup2D();
					translator.Push(L, gen_ret);
                    
					return 1;
				}
				
			}
			catch(System.Exception gen_e) {
				return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
			}
            return LuaAPI.luaL_error(L, "invalid arguments to UnityEngine.PhysicsShapeGroup2D constructor!");
            
        }
        
		
        
		
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Clear(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                UnityEngine.PhysicsShapeGroup2D gen_to_be_invoked = (UnityEngine.PhysicsShapeGroup2D)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    
                    gen_to_be_invoked.Clear(  );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_Add(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                UnityEngine.PhysicsShapeGroup2D gen_to_be_invoked = (UnityEngine.PhysicsShapeGroup2D)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.PhysicsShapeGroup2D _physicsShapeGroup = (UnityEngine.PhysicsShapeGroup2D)translator.GetObject(L, 2, typeof(UnityEngine.PhysicsShapeGroup2D));
                    
                    gen_to_be_invoked.Add( _physicsShapeGroup );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetShapeData(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                UnityEngine.PhysicsShapeGroup2D gen_to_be_invoked = (UnityEngine.PhysicsShapeGroup2D)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& translator.Assignable<System.Collections.Generic.List<UnityEngine.PhysicsShape2D>>(L, 2)&& translator.Assignable<System.Collections.Generic.List<UnityEngine.Vector2>>(L, 3)) 
                {
                    System.Collections.Generic.List<UnityEngine.PhysicsShape2D> _shapes = (System.Collections.Generic.List<UnityEngine.PhysicsShape2D>)translator.GetObject(L, 2, typeof(System.Collections.Generic.List<UnityEngine.PhysicsShape2D>));
                    System.Collections.Generic.List<UnityEngine.Vector2> _vertices = (System.Collections.Generic.List<UnityEngine.Vector2>)translator.GetObject(L, 3, typeof(System.Collections.Generic.List<UnityEngine.Vector2>));
                    
                    gen_to_be_invoked.GetShapeData( _shapes, _vertices );
                    
                    
                    
                    return 0;
                }
                if(gen_param_count == 3&& translator.Assignable<Unity.Collections.NativeArray<UnityEngine.PhysicsShape2D>>(L, 2)&& translator.Assignable<Unity.Collections.NativeArray<UnityEngine.Vector2>>(L, 3)) 
                {
                    Unity.Collections.NativeArray<UnityEngine.PhysicsShape2D> _shapes;translator.Get(L, 2, out _shapes);
                    Unity.Collections.NativeArray<UnityEngine.Vector2> _vertices;translator.Get(L, 3, out _vertices);
                    
                    gen_to_be_invoked.GetShapeData( _shapes, _vertices );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to UnityEngine.PhysicsShapeGroup2D.GetShapeData!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetShapeVertices(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                UnityEngine.PhysicsShapeGroup2D gen_to_be_invoked = (UnityEngine.PhysicsShapeGroup2D)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _shapeIndex = LuaAPI.xlua_tointeger(L, 2);
                    System.Collections.Generic.List<UnityEngine.Vector2> _vertices = (System.Collections.Generic.List<UnityEngine.Vector2>)translator.GetObject(L, 3, typeof(System.Collections.Generic.List<UnityEngine.Vector2>));
                    
                    gen_to_be_invoked.GetShapeVertices( _shapeIndex, _vertices );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetShapeVertex(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                UnityEngine.PhysicsShapeGroup2D gen_to_be_invoked = (UnityEngine.PhysicsShapeGroup2D)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _shapeIndex = LuaAPI.xlua_tointeger(L, 2);
                    int _vertexIndex = LuaAPI.xlua_tointeger(L, 3);
                    
                        var gen_ret = gen_to_be_invoked.GetShapeVertex( _shapeIndex, _vertexIndex );
                        translator.PushUnityEngineVector2(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetShapeVertex(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                UnityEngine.PhysicsShapeGroup2D gen_to_be_invoked = (UnityEngine.PhysicsShapeGroup2D)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _shapeIndex = LuaAPI.xlua_tointeger(L, 2);
                    int _vertexIndex = LuaAPI.xlua_tointeger(L, 3);
                    UnityEngine.Vector2 _vertex;translator.Get(L, 4, out _vertex);
                    
                    gen_to_be_invoked.SetShapeVertex( _shapeIndex, _vertexIndex, _vertex );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetShapeRadius(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                UnityEngine.PhysicsShapeGroup2D gen_to_be_invoked = (UnityEngine.PhysicsShapeGroup2D)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _shapeIndex = LuaAPI.xlua_tointeger(L, 2);
                    float _radius = (float)LuaAPI.lua_tonumber(L, 3);
                    
                    gen_to_be_invoked.SetShapeRadius( _shapeIndex, _radius );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_SetShapeAdjacentVertices(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                UnityEngine.PhysicsShapeGroup2D gen_to_be_invoked = (UnityEngine.PhysicsShapeGroup2D)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _shapeIndex = LuaAPI.xlua_tointeger(L, 2);
                    bool _useAdjacentStart = LuaAPI.lua_toboolean(L, 3);
                    bool _useAdjacentEnd = LuaAPI.lua_toboolean(L, 4);
                    UnityEngine.Vector2 _adjacentStart;translator.Get(L, 5, out _adjacentStart);
                    UnityEngine.Vector2 _adjacentEnd;translator.Get(L, 6, out _adjacentEnd);
                    
                    gen_to_be_invoked.SetShapeAdjacentVertices( _shapeIndex, _useAdjacentStart, _useAdjacentEnd, _adjacentStart, _adjacentEnd );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_DeleteShape(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                UnityEngine.PhysicsShapeGroup2D gen_to_be_invoked = (UnityEngine.PhysicsShapeGroup2D)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _shapeIndex = LuaAPI.xlua_tointeger(L, 2);
                    
                    gen_to_be_invoked.DeleteShape( _shapeIndex );
                    
                    
                    
                    return 0;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_GetShape(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                UnityEngine.PhysicsShapeGroup2D gen_to_be_invoked = (UnityEngine.PhysicsShapeGroup2D)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    int _shapeIndex = LuaAPI.xlua_tointeger(L, 2);
                    
                        var gen_ret = gen_to_be_invoked.GetShape( _shapeIndex );
                        translator.Push(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddCircle(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                UnityEngine.PhysicsShapeGroup2D gen_to_be_invoked = (UnityEngine.PhysicsShapeGroup2D)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.Vector2 _center;translator.Get(L, 2, out _center);
                    float _radius = (float)LuaAPI.lua_tonumber(L, 3);
                    
                        var gen_ret = gen_to_be_invoked.AddCircle( _center, _radius );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddCapsule(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                UnityEngine.PhysicsShapeGroup2D gen_to_be_invoked = (UnityEngine.PhysicsShapeGroup2D)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    UnityEngine.Vector2 _vertex0;translator.Get(L, 2, out _vertex0);
                    UnityEngine.Vector2 _vertex1;translator.Get(L, 3, out _vertex1);
                    float _radius = (float)LuaAPI.lua_tonumber(L, 4);
                    
                        var gen_ret = gen_to_be_invoked.AddCapsule( _vertex0, _vertex1, _radius );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddBox(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                UnityEngine.PhysicsShapeGroup2D gen_to_be_invoked = (UnityEngine.PhysicsShapeGroup2D)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 5&& translator.Assignable<UnityEngine.Vector2>(L, 2)&& translator.Assignable<UnityEngine.Vector2>(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 5)) 
                {
                    UnityEngine.Vector2 _center;translator.Get(L, 2, out _center);
                    UnityEngine.Vector2 _size;translator.Get(L, 3, out _size);
                    float _angle = (float)LuaAPI.lua_tonumber(L, 4);
                    float _edgeRadius = (float)LuaAPI.lua_tonumber(L, 5);
                    
                        var gen_ret = gen_to_be_invoked.AddBox( _center, _size, _angle, _edgeRadius );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 4&& translator.Assignable<UnityEngine.Vector2>(L, 2)&& translator.Assignable<UnityEngine.Vector2>(L, 3)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 4)) 
                {
                    UnityEngine.Vector2 _center;translator.Get(L, 2, out _center);
                    UnityEngine.Vector2 _size;translator.Get(L, 3, out _size);
                    float _angle = (float)LuaAPI.lua_tonumber(L, 4);
                    
                        var gen_ret = gen_to_be_invoked.AddBox( _center, _size, _angle );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 3&& translator.Assignable<UnityEngine.Vector2>(L, 2)&& translator.Assignable<UnityEngine.Vector2>(L, 3)) 
                {
                    UnityEngine.Vector2 _center;translator.Get(L, 2, out _center);
                    UnityEngine.Vector2 _size;translator.Get(L, 3, out _size);
                    
                        var gen_ret = gen_to_be_invoked.AddBox( _center, _size );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to UnityEngine.PhysicsShapeGroup2D.AddBox!");
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddPolygon(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                UnityEngine.PhysicsShapeGroup2D gen_to_be_invoked = (UnityEngine.PhysicsShapeGroup2D)translator.FastGetCSObj(L, 1);
            
            
                
                {
                    System.Collections.Generic.List<UnityEngine.Vector2> _vertices = (System.Collections.Generic.List<UnityEngine.Vector2>)translator.GetObject(L, 2, typeof(System.Collections.Generic.List<UnityEngine.Vector2>));
                    
                        var gen_ret = gen_to_be_invoked.AddPolygon( _vertices );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _m_AddEdges(RealStatePtr L)
        {
		    try {
            
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
            
            
                UnityEngine.PhysicsShapeGroup2D gen_to_be_invoked = (UnityEngine.PhysicsShapeGroup2D)translator.FastGetCSObj(L, 1);
            
            
			    int gen_param_count = LuaAPI.lua_gettop(L);
            
                if(gen_param_count == 3&& translator.Assignable<System.Collections.Generic.List<UnityEngine.Vector2>>(L, 2)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 3)) 
                {
                    System.Collections.Generic.List<UnityEngine.Vector2> _vertices = (System.Collections.Generic.List<UnityEngine.Vector2>)translator.GetObject(L, 2, typeof(System.Collections.Generic.List<UnityEngine.Vector2>));
                    float _edgeRadius = (float)LuaAPI.lua_tonumber(L, 3);
                    
                        var gen_ret = gen_to_be_invoked.AddEdges( _vertices, _edgeRadius );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 2&& translator.Assignable<System.Collections.Generic.List<UnityEngine.Vector2>>(L, 2)) 
                {
                    System.Collections.Generic.List<UnityEngine.Vector2> _vertices = (System.Collections.Generic.List<UnityEngine.Vector2>)translator.GetObject(L, 2, typeof(System.Collections.Generic.List<UnityEngine.Vector2>));
                    
                        var gen_ret = gen_to_be_invoked.AddEdges( _vertices );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 7&& translator.Assignable<System.Collections.Generic.List<UnityEngine.Vector2>>(L, 2)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 3)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 4)&& translator.Assignable<UnityEngine.Vector2>(L, 5)&& translator.Assignable<UnityEngine.Vector2>(L, 6)&& LuaTypes.LUA_TNUMBER == LuaAPI.lua_type(L, 7)) 
                {
                    System.Collections.Generic.List<UnityEngine.Vector2> _vertices = (System.Collections.Generic.List<UnityEngine.Vector2>)translator.GetObject(L, 2, typeof(System.Collections.Generic.List<UnityEngine.Vector2>));
                    bool _useAdjacentStart = LuaAPI.lua_toboolean(L, 3);
                    bool _useAdjacentEnd = LuaAPI.lua_toboolean(L, 4);
                    UnityEngine.Vector2 _adjacentStart;translator.Get(L, 5, out _adjacentStart);
                    UnityEngine.Vector2 _adjacentEnd;translator.Get(L, 6, out _adjacentEnd);
                    float _edgeRadius = (float)LuaAPI.lua_tonumber(L, 7);
                    
                        var gen_ret = gen_to_be_invoked.AddEdges( _vertices, _useAdjacentStart, _useAdjacentEnd, _adjacentStart, _adjacentEnd, _edgeRadius );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                if(gen_param_count == 6&& translator.Assignable<System.Collections.Generic.List<UnityEngine.Vector2>>(L, 2)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 3)&& LuaTypes.LUA_TBOOLEAN == LuaAPI.lua_type(L, 4)&& translator.Assignable<UnityEngine.Vector2>(L, 5)&& translator.Assignable<UnityEngine.Vector2>(L, 6)) 
                {
                    System.Collections.Generic.List<UnityEngine.Vector2> _vertices = (System.Collections.Generic.List<UnityEngine.Vector2>)translator.GetObject(L, 2, typeof(System.Collections.Generic.List<UnityEngine.Vector2>));
                    bool _useAdjacentStart = LuaAPI.lua_toboolean(L, 3);
                    bool _useAdjacentEnd = LuaAPI.lua_toboolean(L, 4);
                    UnityEngine.Vector2 _adjacentStart;translator.Get(L, 5, out _adjacentStart);
                    UnityEngine.Vector2 _adjacentEnd;translator.Get(L, 6, out _adjacentEnd);
                    
                        var gen_ret = gen_to_be_invoked.AddEdges( _vertices, _useAdjacentStart, _useAdjacentEnd, _adjacentStart, _adjacentEnd );
                        LuaAPI.xlua_pushinteger(L, gen_ret);
                    
                    
                    
                    return 1;
                }
                
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            
            return LuaAPI.luaL_error(L, "invalid arguments to UnityEngine.PhysicsShapeGroup2D.AddEdges!");
            
        }
        
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_shapeCount(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                UnityEngine.PhysicsShapeGroup2D gen_to_be_invoked = (UnityEngine.PhysicsShapeGroup2D)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.shapeCount);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_vertexCount(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                UnityEngine.PhysicsShapeGroup2D gen_to_be_invoked = (UnityEngine.PhysicsShapeGroup2D)translator.FastGetCSObj(L, 1);
                LuaAPI.xlua_pushinteger(L, gen_to_be_invoked.vertexCount);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _g_get_localToWorldMatrix(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                UnityEngine.PhysicsShapeGroup2D gen_to_be_invoked = (UnityEngine.PhysicsShapeGroup2D)translator.FastGetCSObj(L, 1);
                translator.Push(L, gen_to_be_invoked.localToWorldMatrix);
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 1;
        }
        
        
        
        [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
        static int _s_set_localToWorldMatrix(RealStatePtr L)
        {
		    try {
                ObjectTranslator translator = ObjectTranslatorPool.Instance.Find(L);
			
                UnityEngine.PhysicsShapeGroup2D gen_to_be_invoked = (UnityEngine.PhysicsShapeGroup2D)translator.FastGetCSObj(L, 1);
                UnityEngine.Matrix4x4 gen_value;translator.Get(L, 2, out gen_value);
				gen_to_be_invoked.localToWorldMatrix = gen_value;
            
            } catch(System.Exception gen_e) {
                return LuaAPI.luaL_error(L, "c# exception:" + gen_e);
            }
            return 0;
        }
        
		
		
		
		
    }
}

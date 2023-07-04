using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IFramework.Inject
{
    /// <summary>
    /// 注入模块
    /// </summary>
    public partial class InjectModule : Module, IInjectModule
    {
        private InjectTypeMap _type;
        private InjectInstanceMap _instance;
#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        protected override ModulePriority OnGetDefautPriority()
        {
            return ModulePriority.Config;
        }
        protected override void Awake()
        {
            _type = new InjectTypeMap();
            _instance = new InjectInstanceMap();
        }

        protected override void OnDispose()
        {
            this.Clear();
        }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释

        /// <summary>
        /// 清除
        /// </summary>
        public void Clear()
        {
            this._instance.Clear();
            this._type.Clear();
        }
        /// <summary>
        /// 反射注入
        /// </summary>
        /// <param name="obj"></param>
        public void Inject(object obj)
        {
            if (obj == null)
            {
                return;
            }
            MemberInfo[] members = obj.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            for (int i = members.Length - 1; i >= 0; i--)
            {
                MemberInfo member = members[i];
                if (member.IsDefined(typeof(InjectAttribute), true))
                {
                    InjectAttribute attr = member.GetCustomAttributes(typeof(InjectAttribute), true)[0] as InjectAttribute;
                    if (member is PropertyInfo)
                    {
                        PropertyInfo propertyInfo = member as PropertyInfo;
                        propertyInfo.SetValue(obj, this.GetValue(propertyInfo.PropertyType, attr.name, new object[0]), null);
                    }
                    else if (member is FieldInfo)
                    {
                        FieldInfo fieldInfo = member as FieldInfo;
                        fieldInfo.SetValue(obj, this.GetValue(fieldInfo.FieldType, attr.name, new object[0]));
                    }
                }
            }
        }
        /// <summary>
        /// 注入所有
        /// </summary>
        public void InjectInstances()
        {
            foreach (object instance in this._instance.Values)
            {
                this.Inject(instance);
            }
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <param name="name"></param>
        public void Subscribe<Type>(string name = null)
        {
            this.Subscribe(typeof(Type), typeof(Type), name);
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <typeparam name="BaseType"></typeparam>
        /// <typeparam name="Type"></typeparam>
        /// <param name="name"></param>
        public void Subscribe<BaseType, Type>(string name = null) where Type : BaseType
        {
            this.Subscribe(typeof(BaseType), typeof(Type), name);
        }
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <param name="name"></param>
        public void Subscribe(Type source, Type target, string name = null)
        {
            this._type.Set(source, name, target);
        }
        /// <summary>
        /// 注册实例
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <param name="inject"></param>
        public void SubscribeInstance<Type>(Type instance, string name, bool inject = true) where Type : class
        {
            this.SubscribeInstance(typeof(Type), instance, name, inject);
        }
        /// <summary>
        /// 注册实例
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="instance"></param>
        /// <param name="name"></param>
        /// <param name="inject"></param>
        /// <exception cref="Exception"></exception>
        public void SubscribeInstance(Type baseType, object instance, string name, bool inject = true)
        {
            Type type = instance.GetType();
            
            if (type != baseType && !type.GetInterfaces().Contains(baseType) && !type.IsSubclassOf(baseType))
            {
                throw new Exception(string.Format("{0} is Not {1}", type, baseType));
            }
            this._instance.Set(baseType, name, instance);
            if (inject)
            {
                this.Inject(instance);
            }
        }
        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public T GetValue<T>(string name = null, params object[] args) where T : class
        {
            return (T)GetValue(typeof(T), name, args);
        }
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="name"></param>
        /// <param name="constructorArgs"></param>
        /// <returns></returns>
        public object GetValue(Type baseType, string name = null, params object[] constructorArgs)
        {
            object item = _instance.Get(baseType, name);
            if (item != null) return item;
            Type map = this._type.Get(baseType, name);
            if (map != null)
            {
                return this.CreateInstance(map, constructorArgs);
            }
            return null;
        }
        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<object> GetValues(Type type)
        {
            var ie = _instance.GetInstances(type); ;
            foreach (var item in ie)
            {
                yield return item;
            }

            var ies = _type.GetTypes(type);
            foreach (var item in ies)
            {
                object obj = Activator.CreateInstance(item);
                this.Inject(obj);
                yield return obj;
            }
        }
        /// <summary>
        /// 获取
        /// </summary>
        /// <typeparam name="Type"></typeparam>
        /// <returns></returns>
        public IEnumerable<Type> GetValues<Type>()
        {
            foreach (object obj in this.GetValues(typeof(Type)))
            {
                yield return (Type)((object)obj);
            }
        }

        private object CreateInstance(Type type, params object[] ctrArgs)
        {
            if (ctrArgs != null && ctrArgs.Length != 0)
            {
                object obj2 = Activator.CreateInstance(type, ctrArgs);
                this.Inject(obj2);
                return obj2;
            }
            ConstructorInfo[] ctrs = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            if (ctrs.Length < 1)
            {
                object obj3 = Activator.CreateInstance(type);
                this.Inject(obj3);
                return obj3;
            }
            ParameterInfo[] maxParameters = ctrs[0].GetParameters();
            for (int i = ctrs.Length - 1; i >= 0; i--)
            {
                ParameterInfo[] parameters = ctrs[i].GetParameters();
                if (parameters.Length > maxParameters.Length)
                {
                    maxParameters = parameters;
                }
            }
            object[] args = maxParameters.Select(delegate (ParameterInfo p)
            {
                if (p.ParameterType.IsArray)
                {
                    return this.GetValues(p.ParameterType);
                }
                object tmp = this.GetValue(p.ParameterType, null, new object[0]);
                if (tmp != null)
                {
                    return tmp;
                }
                return this.GetValue(p.ParameterType, p.Name, new object[0]);
            }).ToArray<object>();
            object obj4 = Activator.CreateInstance(type, args);
            this.Inject(obj4);
            return obj4;
        }
    }
}

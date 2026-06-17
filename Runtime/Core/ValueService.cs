/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace IFramework
{

    public interface IInjectAble { }
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class InjectAttribute : System.Attribute { }
    public static class ValueServiceEx
    {
        public static void UseValue(this Game game)
        {
            ValueService service = new ValueService();
            game.UseService(service);
        }
        public static void RegisterValue(this Game game, Type type, object instance) => game.GetService<ValueService>().RegisterInstance(type, instance);
        public static object GetValue(this Game game, Type type) => game.GetService<ValueService>().Get(type);
        public static void InjectValues(this Game game, object obj) => game.GetService<ValueService>().Inject(obj);
        public static void RegisterValueType<TBaseType, TType>(this Game game) where TType : class, TBaseType, new() => game.GetService<ValueService>().RegisterType<TBaseType, TType>();
        public static void RegisterValue<T>(this Game game, T instance) where T : class => RegisterValue(game, typeof(T), instance);
        public static T GetValue<T>(this Game game) where T : class => GetValue(game, typeof(T)) as T;
        public static void RegisterValueType<TType>(this Game game) where TType : class, new() => RegisterValueType<TType, TType>(game);
    }
    class ValueService : IGameService
    {
        private Dictionary<Type, object> values = new Dictionary<Type, object>();
        private Dictionary<Type, Type> typeMap = new Dictionary<Type, Type>();
        static Dictionary<Type, List<FieldInfo>> fieldsMap = new Dictionary<Type, List<FieldInfo>>();

        public void RegisterType<TBaseType, TType>() where TType : TBaseType, new()
        {
            var typeBase = typeof(TBaseType);
            var type = typeof(TType);
            typeMap[typeBase] = type;
        }

        public object RegisterInstance(Type type, object instance)
        {
            values[type] = instance;
            return instance;
        }
        public object Get(Type type)
        {
            object result = null;
            if (values.TryGetValue(type, out result))
                return result;
            Type subType = null;
            if (typeMap.TryGetValue(type, out subType))
            {
                var ins = Activator.CreateInstance(subType);
                RegisterInstance(type, ins);
                Inject(ins);
                return ins;
            }

            return null;
        }
        public void Inject(object inject)
        {
            if (!(inject is IInjectAble))
                return;
            var type = inject.GetType();
            List<FieldInfo> fields;
            if (!fieldsMap.TryGetValue(type, out fields))
            {
                fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                            .Where(x => x.IsDefined(typeof(InjectAttribute), false) &&
                            !x.IsInitOnly && !x.FieldType.IsValueType).ToList();

                fieldsMap[type] = fields;
            }
            for (int i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                field.SetValue(inject, Get(field.FieldType));
            }

        }

        public override void OnUse(Game game)
        {

        }

        public override void OnQuit(Game game)
        {
            values.Clear(); typeMap.Clear();
        }
    }
}

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
    class ValueService : ServiceBase
    {
        private readonly Dictionary<Type, Dictionary<string, object>> values = new();
        private Dictionary<Type, Type> typeMap = new();
        static Dictionary<Type, Dictionary<FieldInfo, string>> fieldsMap = new();

        public void RegisterType<TBaseType, TType>() where TType : TBaseType, new()
        {
            var typeBase = typeof(TBaseType);
            var type = typeof(TType);
            typeMap[typeBase] = type;
        }

        public object RegisterValue(Type type, object instance, string name)
        {
            name ??= string.Empty;
            if (!values.TryGetValue(type, out var dict))
            {
                dict = new Dictionary<string, object>();
                values[type] = dict;
            }
            dict[name] = instance;
            return instance;
        }
        public object Get(Type type, string name)
        {
            name ??= string.Empty;
            if (values.TryGetValue(type, out var dict) && dict.TryGetValue(name, out object result))
                return result;

            if (typeMap.TryGetValue(type, out Type subType))
            {
                try
                {
                    var instance = Activator.CreateInstance(subType);
                    RegisterValue(type, instance, string.Empty);
                    Inject(instance);
                    return instance;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to create instance of {subType} for name '{name}'", ex);
                }
            }

            return null;
        }
        public void Inject(object inject)
        {
            if (!(inject is IInjectAble))
                return;
            var type = inject.GetType();
            Dictionary<FieldInfo, string> fields;
            if (!fieldsMap.TryGetValue(type, out fields))
            {
                fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
                            .Where(x => x.IsDefined(typeof(InjectAttribute), false) &&
                            !x.IsInitOnly && !x.FieldType.IsValueType).ToDictionary(x => x, x => x.GetCustomAttribute<InjectAttribute>(false).name);

                fieldsMap[type] = fields;
            }
            foreach (var item in fields)
            {
                var field = item.Key;
                var value = Get(field.FieldType, item.Value);
                if (value == null)
                {
                    Log.FE($"Inject value Not found: {inject.GetType()}->{field.Name}\n" +
                        $"FieldType:{field.FieldType} \n" +
                        $"InjectName:{item.Value} ");

                }
                else
                    field.SetValue(inject, value);
            }

        }

        protected override void OnUse(IServiceCollection services)
        {

        }

        protected override void OnQuit(IServiceCollection services)
        {
            values.Clear(); typeMap.Clear();
        }
    }
}

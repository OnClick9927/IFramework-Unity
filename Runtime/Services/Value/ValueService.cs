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
using System.Reflection;
namespace IFramework
{
    class ValueService : ServiceBase, IValueService
    {
        private readonly Dictionary<Type, Dictionary<string, object>> values = new();
        private readonly Dictionary<Type, Func<IValueService, object>> factories = new();
        private static readonly Dictionary<Type, InjectField[]> fieldsMap = new();

        private readonly struct InjectField
        {
            public readonly FieldInfo field;
            public readonly string name;

            public InjectField(FieldInfo field, string name)
            {
                this.field = field;
                this.name = name;
            }
        }

        public void Register<TType>(Func<IValueService, TType> create)
        {
            if (create == null) throw new ArgumentNullException(nameof(create));
            factories[typeof(TType)] = service => create(service);
        }

        public void Register<TBaseType, TType>() where TType : TBaseType, new()
        {
            factories[typeof(TBaseType)] = _ => new TType();
        }

        public object Register(Type type, object instance, string name)
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

            if (factories.TryGetValue(type, out var create))
            {
                try
                {
                    var instance = create(this);
                    Register(type, instance, name);
                    Inject(instance);
                    return instance;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to create instance of {type} for name '{name}'", ex);
                }
            }

            return null;
        }
        public void Inject(object inject)
        {
            if (!(inject is IInjectAble))
                return;
            var type = inject.GetType();
            if (!fieldsMap.TryGetValue(type, out var fields))
            {
                var reflectedFields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                int injectionCount = 0;
                for (int i = 0; i < reflectedFields.Length; i++)
                {
                    var field = reflectedFields[i];
                    if (!field.IsInitOnly && !field.FieldType.IsValueType &&
                        field.IsDefined(typeof(InjectAttribute), false))
                        injectionCount++;
                }

                fields = new InjectField[injectionCount];
                int index = 0;
                for (int i = 0; i < reflectedFields.Length; i++)
                {
                    var field = reflectedFields[i];
                    if (field.IsInitOnly || field.FieldType.IsValueType)
                        continue;
                    var attribute = field.GetCustomAttribute<InjectAttribute>(false);
                    if (attribute != null)
                        fields[index++] = new InjectField(field, attribute.name);
                }

                fieldsMap[type] = fields;
            }
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                var value = Get(field.field.FieldType, field.name);
                if (value == null)
                {
                    Log.FE($"Inject value Not found: {type}->{field.field.Name}\n" +
                        $"FieldType:{field.field.FieldType} \n" +
                        $"InjectName:{field.name} ");

                }
                else
                    field.field.SetValue(inject, value);
            }

        }
        protected override void OnEnter(IServiceCollection services)
        {
            
        }
        protected override void OnUse(IServiceCollection services)
        {

        }

        protected override void OnQuit(IServiceCollection services)
        {
            values.Clear();
            factories.Clear();
        }
    }
}

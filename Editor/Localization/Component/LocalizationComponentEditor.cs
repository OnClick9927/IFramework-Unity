/*********************************************************************************
 *Author:         OnClick
 *Version:        0.1
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-04-25
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace IFramework
{
    public abstract class LocalizationComponentEditor<T> : UnityEditor.Editor where T : LocalizationComponent
    {
        private LocalizationComponent comp;
        private LocalizationSetting setting => LocalizationSetting.context;
        private Dictionary<Type, Type> typeMap;
        private List<FieldInfo> fields;
        private void OnEnable()
        {
            comp = target as T;
            if (comp.context == null && setting.defaultObject != null)
            {
                comp.context = setting.defaultObject;
                EditorUtility.SetDirty(comp);
            }
            typeMap = EditorTools.GetSubTypesInAssemblies(typeof(ILocalizationActorEditor))
                     .Where(x => !x.IsAbstract && x.GetCustomAttribute<LocalizationActorAttribute>() != null)
                     .Select(x => new { x, target = x.BaseType.GetGenericArguments()[0] })
                     .ToDictionary(x => x.target, x => x.x);


            fields = comp.GetType().GetFields().Where(x => typeof(ILocalizationActor).IsAssignableFrom(x.FieldType)).ToList()
               .ToList();

        }
        private Dictionary<Type, ILocalizationActorEditor> insMap = new Dictionary<Type, ILocalizationActorEditor>();
        public override void OnInspectorGUI()
        {
            comp.context = EditorGUILayout.ObjectField(nameof(LocalizationComponent.context),
                comp.context, typeof(LocalizationObject), false) as LocalizationObject;
            for (int i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                var type = typeMap[field.FieldType];
                if (!insMap.ContainsKey(type))
                    insMap[type] = Activator.CreateInstance(type) as ILocalizationActorEditor;
                var editor = insMap[type];

                var value = field.GetValue(comp);
                editor.OnGUI(field.Name, comp, value);
                AssetDatabase.SaveAssetIfDirty(comp);
            }


        }
    }
}

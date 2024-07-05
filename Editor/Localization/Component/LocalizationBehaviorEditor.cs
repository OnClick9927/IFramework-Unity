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
using UnityEngine;

namespace IFramework.Localization
{

    public abstract class LocalizationBehaviorEditor<T> : UnityEditor.Editor where T : LocalizationBehavior
    {

        protected T comp { get; private set; }
        protected void LoadFields()
        {
            fields.Clear();
            var typeMap = EditorTools.GetSubTypesInAssemblies(typeof(ILocalizationActorEditor))
                     .Where(x => !x.IsAbstract && x.GetCustomAttribute<LocalizationActorEditorAttribute>() != null)
                     .Select(x => new { x, target = x.BaseType.GetGenericArguments()[0] })
                     .ToDictionary(x => x.target, x => x.x);


            var fieldInfos = comp.GetType().GetFields().Where(x => typeof(ILocalizationActor).IsAssignableFrom(x.FieldType)).ToList()
                   .ToList();
            var insMap = new Dictionary<Type, ILocalizationActorEditor>();
            var insMap_obj = new Dictionary<Type, ILocalizationActorEditor>();
            for (int i = 0; i < fieldInfos.Count; i++)
            {
                var field = fieldInfos[i];
                if (typeMap.ContainsKey(field.FieldType))
                {
                    var type = typeMap[field.FieldType];
                    if (!insMap.ContainsKey(type))
                        insMap[type] = CreateEditor(type);
                    var editor = insMap[type];
                    var value = field.GetValue(comp);
                    AddField(field.Name, editor, value);
                }
                else
                {
                    if (field.FieldType.IsGenericType)
                    {
                        var types = field.FieldType.GetGenericArguments();
                        var type0 = types[0];
                        if (typeof(ObjectActor<>).MakeGenericType(type0) == field.FieldType)
                        {
                            if (!insMap_obj.ContainsKey(type0))
                                insMap_obj.Add(type0, CreateEditor(typeof(ObjectActorEditor<>).MakeGenericType(type0)));
                            var editor = insMap_obj[type0];
                            var value = field.GetValue(comp);
                            AddField(field.Name, editor, value);
                        }
                    }
                }
            }
            var actors = new List<ILocalizationActor>(comp.LoadActors());
            actors.RemoveAll(a => fields.Any(x => x.value == a));


            for (int i = 0; i < actors.Count; i++)
            {
                var actor = actors[i];
                var FieldType = actor.GetType();
                if (typeMap.ContainsKey(FieldType))
                {
                    var type = typeMap[FieldType];
                    if (!insMap.ContainsKey(type))
                        insMap[type] = CreateEditor(type);
                    var editor = insMap[type];
                    var value = actor;
                    AddField(actor.name, editor, value);
                }
                else
                {
                    if (FieldType.IsGenericType)
                    {
                        var types = FieldType.GetGenericArguments();
                        var type0 = types[0];
                        if (typeof(ObjectActor<>).MakeGenericType(type0) == FieldType)
                        {
                            if (!insMap_obj.ContainsKey(type0))
                                insMap_obj.Add(type0, CreateEditor(typeof(ObjectActorEditor<>).MakeGenericType(type0)));
                            var editor = insMap_obj[type0];
                            var value = actor;
                            AddField(actor.name, editor, value);
                        }
                    }
                }
            }
        }
        private void Awake()
        {
            comp = target as T;
            if (comp.context == null && LocalizationSetting.defaultData != null)
            {
                comp.context = LocalizationSetting.defaultData;
                EditorUtility.SetDirty(comp);
            }
            LoadFields();

            comp.enabled = false;
            EditorApplication.delayCall += () =>
            {
                comp.enabled = true;
            };

        }
        private void OnEnable()
        {
           
        }
        private ILocalizationActorEditor CreateEditor(Type type)
        {
            return Activator.CreateInstance(type) as ILocalizationActorEditor;
        }
        private void AddField(string name, ILocalizationActorEditor editor, object value)
        {
            fields.Add(new Field() { editor = editor, name = name, value = value });
        }
        private List<Field> fields = new List<Field>();

        private class Field
        {
            public string name;
            public object value;
            public ILocalizationActorEditor editor;
        }
        protected virtual void RemoveActor(ILocalizationActor actor)
        {

        }
        protected void DrawFields()
        {
            for (int i = 0; i < fields.Count; i++)
            {
                var field = fields[i];
                bool can = (field.value as ILocalizationActor).canRemove;
                if (can)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.BeginVertical();
                    field.editor.OnGUI(field.name, comp, field.value);
                    GUILayout.EndVertical();
                    if (GUILayout.Button("-", GUILayout.ExpandHeight(true)))
                    {
                        RemoveActor(field.value as ILocalizationActor);
                        LoadFields();
                    }
                    GUILayout.EndHorizontal();
                }
                else
                {
                    field.editor.OnGUI(field.name, comp, field.value);

                }


                AssetDatabase.SaveAssetIfDirty(comp);

            }
        }
        protected void DrawContext()
        {
            comp.context = EditorGUILayout.ObjectField(nameof(LocalizationBehavior.context),
      comp.context, typeof(LocalizationData), false) as LocalizationData;
        }
        public override void OnInspectorGUI()
        {

            DrawContext();
            DrawFields();


        }
    }
}

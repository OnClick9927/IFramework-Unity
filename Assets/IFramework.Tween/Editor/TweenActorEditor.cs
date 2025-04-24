/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static IFramework.EditorTools;
using static IFramework.TweenComponentActor;
namespace IFramework
{
    interface ITweenActorEditor
    {
        void OnInspectorGUI(TweenComponentActor actor);
        void OnSceneGUI(TweenComponentActor actor);

    }

    public class TweenActorEditor<T> : ITweenActorEditor where T : TweenComponentActor
    {
        protected virtual void OnSceneGUI(T actor) { }
        protected void DrawBase(T actor)
        {
            GUILayout.BeginVertical(EditorStyles.helpBox);
            FieldDefaultInspector(actor.GetType().GetField("target"), actor);

            actor.id = EditorGUILayout.TextField("ID", actor.id);
            actor.snap = EditorGUILayout.Toggle("Snap", actor.snap);
            actor.sourceDelta = EditorGUILayout.FloatField("Source Delta", actor.sourceDelta);
            actor.duration = EditorGUILayout.FloatField("Duration", actor.duration);
            actor.delay = EditorGUILayout.FloatField("Delay", actor.delay);
            GUILayout.Space(10);

            actor.loopType = (LoopType)EditorGUILayout.EnumPopup(nameof(LoopType), actor.loopType);
            actor.loops = EditorGUILayout.IntField("Loops", actor.loops);


            GUILayout.Space(10);

            actor.curveType = (CurveType)EditorGUILayout.EnumPopup(nameof(CurveType), actor.curveType);
            if (actor.curveType == CurveType.Ease)
            {
                actor.ease = (Ease)EditorGUILayout.EnumPopup(nameof(Ease), actor.ease);
            }
            else
            {
                AnimationCurve curve = actor.curve;
                if (curve == null)
                {
                    curve = new AnimationCurve();
                }
                actor.curve = EditorGUILayout.CurveField(nameof(AnimationCurve), curve);
            }
            GUILayout.EndVertical();
        }
        protected void DrawSelf(T actor)
        {
            List<Type> types = new List<Type>();


            var _baseType = actor.GetType();
            while (true)
            {
                if (_baseType.IsGenericType && _baseType.GetGenericTypeDefinition() == typeof(TweenComponentActor<,>))
                {
                    break;
                }
                types.Insert(0, _baseType);
                _baseType = _baseType.BaseType;
            }


            GUILayout.BeginVertical(EditorStyles.helpBox);

            foreach (var type in types)
            {
                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                for (int i = 0; i < fields.Length; i++)
                {
                    FieldDefaultInspector(fields[i], actor);
                }
            }
            GUILayout.EndVertical();
        }

        protected virtual void OnInspectorGUI(T actor)
        {
            DrawBase(actor);
            GUILayout.Space(5);
            DrawSelf(actor);
        }

        void ITweenActorEditor.OnInspectorGUI(TweenComponentActor actor)
        {
            OnInspectorGUI(actor as T);
        }

        void ITweenActorEditor.OnSceneGUI(TweenComponentActor actor)
        {
            OnSceneGUI(actor as T);
        }
    }
}

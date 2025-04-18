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
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static IFramework.TweenComponentActor;
namespace IFramework
{
    public partial class EditorTools
    {
        [CustomEditor(typeof(TweenComponent))]
        class TweenComponentEditor : Editor
        {
            TweenComponent comp;
            private void OnEnable()
            {
                comp = target as TweenComponent;
                var types = typeof(TweenComponentActor).GetSubTypesInAssemblies()
               .Where(x => !x.IsAbstract).ToList();
                Dictionary<string, int> map = new Dictionary<string, int>();
                foreach (var type in types)
                {
                    var _baseType = type;
                    bool find = false;
                    while (true)
                    {
                        if (_baseType.IsGenericType && _baseType.GetGenericTypeDefinition() == typeof(TweenComponentActor<,>))
                        {
                            find = true;
                            break;
                        }
                        else if (_baseType == typeof(object))
                        {
                            break;
                        }
                        _baseType = _baseType.BaseType;
                    }
                    if (find)
                    {
                        var args = _baseType.GetGenericArguments();
                        options.Add($"{args[1].Name}/{type.Name.Replace("Actor", "")}");
                        if (!map.ContainsKey(args[1].Name))
                            map[args[1].Name] = 0;
                        map[args[1].Name]++;
                        options_type.Add(type);
                    }

                }
                int count = map.Count;

                foreach (var item in map.Values)
                {
                    count = Mathf.Max(count, item);
                }
                max_count = count;
            }
            int max_count;
            List<string> options = new List<string>();
            List<Type> options_type = new List<Type>();
            private void Tools()
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(nameof(TweenComponent.Play)))
                {
                    comp.Play();
                }
                using (new EditorGUI.DisabledGroupScope(!comp.hasValue))
                {
                    GUILayout.Space(10);
                    if (comp.paused)
                    {
                        if (GUILayout.Button(nameof(TweenComponent.UnPause)))
                            comp.UnPause();
                    }
                    else
                    {
                        if (GUILayout.Button(nameof(TweenComponent.Pause)))
                            comp.Pause();
                    }
                    GUILayout.Space(10);

                    if (GUILayout.Button(nameof(TweenComponent.Stop)))
                        comp.Stop();

                    //if (GUILayout.Button(nameof(TweenComponent.ReStart)))
                    //    comp.ReStart();

                }

                GUILayout.EndHorizontal();
            }


            public override void OnInspectorGUI()
            {
                GUI.enabled = !EditorApplication.isPlaying;
                base.OnInspectorGUI();
                var _style = new GUIStyle(EditorStyles.miniPullDown)
                {
                    fixedHeight = 25
                };

                var rect = EditorGUILayout.GetControlRect(GUILayout.Height(25));
                var index = EditorTools.AdvancedPopup(rect, -1, options.ToArray(), Mathf.Min(max_count * 18 + 40, 400), _style);
                EditorGUI.LabelField(RectEx.Zoom(rect,
                    TextAnchor.MiddleCenter, new Vector2(0, -5)),
                    new GUIContent("Actors", EditorGUIUtility.TrIconContent("d_Toolbar Plus").image), EditorStyles.boldLabel);
                if (index != -1)
                {
                    var type = options_type[index];
                    comp.actors.Add(Activator.CreateInstance(type) as TweenComponentActor);
                }


                EditorGUI.BeginChangeCheck();
                for (int i = 0; i < comp.actors.Count; i++)
                {
                    var actor = comp.actors[i];
                    var mode = DrawActor(actor, i);
                    switch (mode)
                    {
                        case Mode.Remove:
                            comp.actors.RemoveAt(i);
                            break;
                        case Mode.MoveDown:
                            {
                                if (i != comp.actors.Count - 1)
                                {
                                    comp.actors[i] = comp.actors[i + 1];
                                    comp.actors[i + 1] = actor;
                                }
                            }
                            break;
                        case Mode.MoveUp:
                            {
                                if (i != 0)
                                {
                                    comp.actors[i] = comp.actors[i - 1];
                                    comp.actors[i - 1] = actor;
                                }
                            }
                            break;

                    }
                }
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(comp);
                    //if (comp.hasValue && !comp.paused)
                }
                Repaint();
                GUILayout.Space(10);

                var _fold = GetFoldout(this);
                if (EditorGUILayout.DropdownButton(new GUIContent("Events"), FocusType.Passive, _style))
                {
                    _fold = !_fold;
                    SetFoldout(this, _fold);
                }
                if (_fold)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(comp.onBegin)));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(comp.onCancel)));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(comp.onComplete)));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(comp.onTick)));

                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorUtility.SetDirty(comp);
                        serializedObject.ApplyModifiedProperties();
                    }
                }
                GUILayout.Space(10);

                Tools();
            }

            private enum Mode
            {
                Remove, MoveDown, MoveUp, None
            }
            private Mode DrawActor(TweenComponentActor actor, int index)
            {
                Mode mode = Mode.None;
                EditorGUILayout.LabelField("", GUI.skin.textField, GUILayout.Height(25));
                var rect = EditorTools.RectEx.Zoom(GUILayoutUtility.GetLastRect(),
                    TextAnchor.MiddleRight, new Vector2(-20, 0));
                var rs = EditorTools.RectEx.VerticalSplit(rect, rect.width - 80, 4);
                EditorGUI.ProgressBar(rs[0], actor.percent, "");
                var fold = EditorGUI.Foldout(rs[0], GetFoldout(actor), $"{actor.GetType().Name}", true);
                SetFoldout(actor, fold);


                var rss = RectEx.VerticalSplit(rs[1], rect.height, 0);
                if (GUI.Button(rss[0], EditorGUIUtility.TrIconContent("d_Toolbar Minus")))
                    mode = Mode.Remove;
                rss = RectEx.VerticalSplit(rss[1], rect.height, 0);
                using (new EditorGUI.DisabledGroupScope(index == 0))
                    if (GUI.Button(rss[0], EditorGUIUtility.TrIconContent("d_scrollup")))
                    mode = Mode.MoveUp;
                rss = RectEx.VerticalSplit(rss[1], rect.height, 0);
                using (new EditorGUI.DisabledGroupScope(index == comp.actors.Count - 1))
                    if (GUI.Button(rss[0], EditorGUIUtility.TrIconContent("d_scrolldown")))
                    mode = Mode.MoveDown;





                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                GUILayout.BeginVertical();
                if (mode == Mode.None && fold)
                {

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    FieldDefaultInspector(actor.GetType().GetField("target"), actor);

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
                    GUILayout.Space(5);

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



                    //actor.startType = (StartValueType)EditorGUILayout.EnumPopup(nameof(StartValueType), actor.startType);

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
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                return mode;
            }








        }
    }
}

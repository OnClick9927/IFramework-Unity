/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-11-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace IFramework.GUITool.RectDesign
{
    public abstract class GUIElementEditor : ILayoutGUIDrawer
    {
        protected class GUIStyleEditor
        {
            public class GUIStyleStateEditor
            {
                public string name;
                public GUIStyleState state;

                public GUIStyleStateEditor(string name, GUIStyleState state)
                {
                    this.name = name;
                    this.state = state;
                }

                private bool foldon;
                private bool foldonList;
                public void OnGUI()
                {
                    foldon = EditorGUILayout.Foldout(foldon, name, true);
                    if (!foldon) return;
                    HorizontalView(() => {
                        GUILayout.Space(20);
                        VerticalView(() => {
                            state.textColor = EditorGUILayout.ColorField("Text Color", state.textColor);
                            state.background = (Texture2D)EditorGUILayout.ObjectField("Back Ground", state.background, typeof(Texture2D), false);
                            foldonList = EditorGUILayout.Foldout(foldonList, "Scaled Backgrounds", true);

                            if (foldonList)
                            {
                                HorizontalView(() =>
                                {
                                    GUILayout.Space(20);
                                    VerticalView(() => {
                                        int size = EditorGUILayout.IntField("Size", state.scaledBackgrounds.Length);

                                        if (size != state.scaledBackgrounds.Length)
                                        {
                                            Texture2D[] txs = new Texture2D[state.scaledBackgrounds.Length];
                                            for (int i = 0; i < state.scaledBackgrounds.Length; i++)
                                            {
                                                txs[i] = txs[i];
                                            }
                                            state.scaledBackgrounds = new Texture2D[size];
                                            for (int i = 0; i < (txs.Length < size ? txs.Length : size); i++)
                                            {
                                                state.scaledBackgrounds[i] = txs[i];
                                            }
                                        }
                                        List<Texture2D> tts = new List<Texture2D>();
                                        tts = state.scaledBackgrounds.ToList();
                                        for (int i = 0; i < tts.Count; i++)
                                        {
                                            tts[i] = (Texture2D)EditorGUILayout.ObjectField("Back Ground", tts[i], typeof(Texture2D), false);
                                        }
                                        state.scaledBackgrounds = tts.ToArray();
                                    }, GUIStyle.none);
                                }, GUIStyle.none);
                            }


                        }, GUIStyle.none);
                    }, GUIStyle.none);
                }
            }
            public class RectOffsetDesign
            {
                private RectOffset offset;
                private string name;

                public RectOffsetDesign(string name, RectOffset offset)
                {
                    this.name = name;
                    this.offset = offset;
                }

                private bool foldon;
                public void OnGUI()
                {
                    foldon = EditorGUILayout.Foldout(foldon, name, true);
                    if (foldon)
                    {
                        GUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(20);
                            GUILayout.BeginVertical();
                            {
                                offset.left = EditorGUILayout.IntField("Left", offset.left);
                                offset.right = EditorGUILayout.IntField("Right", offset.right);
                                offset.top = EditorGUILayout.IntField("Top", offset.top);
                                offset.bottom = EditorGUILayout.IntField("Bottom", offset.bottom);
                            }
                            GUILayout.EndVertical();
                        }
                        GUILayout.EndHorizontal();
                    }

                }
            }

            private GUIStyle style;
            private string name;
            private List<GUIStyleStateEditor> states;
            private List<RectOffsetDesign> offsets;

            public GUIStyleEditor(GUIStyle style, string name)
            {
                this.name = name;
                this.style = style;
                states = new List<GUIStyleStateEditor>();
                offsets = new List<RectOffsetDesign>();
                states.Add(new GUIStyleStateEditor("normal", style.normal));
                states.Add(new GUIStyleStateEditor("onNormal", style.onNormal));
                states.Add(new GUIStyleStateEditor("hover", style.hover));
                states.Add(new GUIStyleStateEditor("onHover", style.onHover));
                states.Add(new GUIStyleStateEditor("active", style.active));
                states.Add(new GUIStyleStateEditor("onActive", style.onActive));
                states.Add(new GUIStyleStateEditor("focused", style.focused));
                states.Add(new GUIStyleStateEditor("onFocused", style.onFocused));
                offsets.Add(new RectOffsetDesign("border", style.border));
                offsets.Add(new RectOffsetDesign("margin", style.margin));
                offsets.Add(new RectOffsetDesign("padding", style.padding));
                offsets.Add(new RectOffsetDesign("overflow", style.overflow));
            }

            private bool foldon;
            public void OnGUI()
            {
                foldon = EditorGUILayout.Foldout(foldon, name, true);
                if (!foldon) return;
                HorizontalView(() => {
                    GUILayout.Space(20);
                    VerticalView(() => {
                        style.name = EditorGUILayout.TextField("Name", style.name);
                        states.ForEach((s) => { s.OnGUI(); });
                        offsets.ForEach((o) => { o.OnGUI(); });
                        style.font = EditorGUILayout.ObjectField("Font", style.font, typeof(Font), false) as Font;
                        style.fontSize = EditorGUILayout.IntField("Font Stize", style.fontSize);
                        style.fontStyle = (FontStyle)EditorGUILayout.EnumPopup("Font Style", style.fontStyle);
                        style.alignment = (TextAnchor)EditorGUILayout.EnumPopup("Alignment", style.alignment);
                        style.wordWrap = EditorGUILayout.Toggle("Word Wrap", style.wordWrap);
                        style.richText = EditorGUILayout.Toggle("Rich Text", style.richText);

                        style.clipping = (TextClipping)EditorGUILayout.EnumPopup("Over flow", style.clipping);
                        style.imagePosition = (ImagePosition)EditorGUILayout.EnumPopup("Image Position", style.imagePosition);

                        style.contentOffset = EditorGUILayout.Vector2Field("Content Offset", style.contentOffset);
                        style.fixedWidth = EditorGUILayout.FloatField("Fixed Width", style.fixedWidth);
                        style.fixedHeight = EditorGUILayout.FloatField("Fixed Height", style.fixedHeight);
                        style.stretchWidth = EditorGUILayout.Toggle("Stretch Width", style.stretchWidth);
                        style.stretchHeight = EditorGUILayout.Toggle("Stretch Height", style.stretchHeight);

                    }, GUIStyle.none);
                }, GUIStyle.none);
            }
        }
        protected class RectCalculatorEditor : ILayoutGUIDrawer
        {
            private RectCalculator calculator;

            public RectCalculatorEditor(RectCalculator calculator)
            {
                this.calculator = calculator;
            }
            private bool insFold = true;
            private bool anchorFold;
            public void OnInspectorGUI()
            {
                insFold = FormatInspectorHeadGUI(insFold, "RectCalculator", null, ContentGUI);
            }
            private void ContentGUI()
            {
                this.DrawHorizontal(() => {
                    this.DrawVertical(() =>
                    {
                        this.BeginHorizontal()
                                .Button(() => { calculator.pivot = Vector2.one; calculator.anchorMax = calculator.anchorMin = Vector2.zero; }, "◤", GUILayout.Width(20), GUILayout.Height(20))
                                .Button(() => { calculator.pivot = new Vector2(0.5f, 1); calculator.anchorMax = calculator.anchorMin = new Vector2(0.5f, 0); }, "△", GUILayout.Width(20), GUILayout.Height(20))
                                .Button(() => { calculator.pivot = new Vector2(0, 1); calculator.anchorMax = calculator.anchorMin = new Vector2(1, 0); }, "◥", GUILayout.Width(20), GUILayout.Height(20))
                            .EndHorizontal()
                            .BeginHorizontal()
                                .Button(() => { calculator.pivot = new Vector2(1, 0.5f); calculator.anchorMax = calculator.anchorMin = new Vector2(0, 0.5f); }, "◁", GUILayout.Width(20), GUILayout.Height(20))
                                .Button(() => { calculator.pivot = new Vector2(0.5f, 0.5f); calculator.anchorMax = calculator.anchorMin = Vector2.one * 0.5f; }, "▣", GUILayout.Width(20), GUILayout.Height(20))
                                .Button(() => { calculator.pivot = new Vector2(0, 0.5f); calculator.anchorMax = calculator.anchorMin = new Vector2(1, 0.5f); }, "▷", GUILayout.Width(20), GUILayout.Height(20))
                            .EndHorizontal()
                            .BeginHorizontal()
                                .Button(() => { calculator.pivot = new Vector2(1, 0); calculator.anchorMax = calculator.anchorMin = new Vector2(0, 1); }, "◣", GUILayout.Width(20), GUILayout.Height(20))
                                .Button(() => { calculator.pivot = new Vector2(0.5f, 0); calculator.anchorMax = calculator.anchorMin = new Vector2(0.5f, 1); }, "▽", GUILayout.Width(20), GUILayout.Height(20))
                                .Button(() => { calculator.pivot = new Vector2(0, 0); calculator.anchorMax = calculator.anchorMin = Vector2.one; }, "◢", GUILayout.Width(20), GUILayout.Height(20))
                            .EndHorizontal();

                    });
                    this.DrawVertical(() =>
                    {
                        this.BeginHorizontal()
                                .Space(25)
                                .Button(() => {
                                    calculator.pivot = new Vector2(0.5f, 1);
                                    calculator.anchorMax = new Vector2(1, 0);
                                    calculator.anchorMin = Vector2.zero;

                                }, "↔", GUILayout.Width(40), GUILayout.Height(17))
                            .EndHorizontal()
                            .BeginHorizontal()
                                .Button(() => {
                                    calculator.pivot = new Vector2(1, 0.5f);
                                    calculator.anchorMax = new Vector2(0, 1);
                                    calculator.anchorMin = Vector2.zero;
                                },
                                                "↕", GUILayout.Width(17), GUILayout.Height(40))
                                .Button(() => {
                                    calculator.pivot = new Vector2(0.5f, 0.5f);
                                    calculator.anchorMin = Vector2.zero;
                                    calculator.anchorMax = Vector2.one;
                                }
                                                , "▣", GUILayout.Width(40), GUILayout.Height(40))
                                .Button(() => {
                                    calculator.pivot = new Vector2(0, 0.5f);
                                    calculator.anchorMax = Vector2.one;
                                    calculator.anchorMin = new Vector2(1, 0);
                                }, "↕", GUILayout.Width(17), GUILayout.Height(40))
                            .EndHorizontal()
                            .BeginHorizontal()
                                 .Space(25)
                                 .Button(() => {
                                     calculator.pivot = new Vector2(0.5f, 0);
                                     calculator.anchorMax = new Vector2(1, 1);
                                     calculator.anchorMin = new Vector2(0, 1);
                                 }, "↔", GUILayout.Width(40), GUILayout.Height(17))
                            .EndHorizontal();

                    });
                    this.DrawVertical(() =>
                    {
                        this.Button(() =>
                                {
                                    calculator.pivot = new Vector2(0.5f, 0.5f);
                                    calculator.anchorMax = new Vector2(1, calculator.anchorMax.y);
                                    calculator.anchorMin = new Vector2(0, calculator.anchorMin.y);

                                }, "↔", GUILayout.Width(40), GUILayout.Height(17))
                            .Button(() =>
                            {
                                calculator.pivot = new Vector2(0.5f, 0.5f);
                                calculator.anchorMax = new Vector2(calculator.anchorMax.x, 1);
                                calculator.anchorMin = new Vector2(calculator.anchorMin.x, 0);
                            },
                            "↕", GUILayout.Width(17), GUILayout.Height(40));
                    });
                });


                anchorFold = EditorGUILayout.Foldout(anchorFold, "Anchor", true);
                if (anchorFold)
                {
                    HorizontalView(() => {
                        GUILayout.Space(20);
                        VerticalView(() =>
                        {
                            calculator.anchorMin = EditorGUILayout.Vector2Field("Min", calculator.anchorMin);
                            calculator.anchorMax = EditorGUILayout.Vector2Field("Max", calculator.anchorMax);
                        }, GUIStyle.none);
                    }, GUIStyle.none);
                }
                calculator.pivot = EditorGUILayout.Vector2Field("Pivot", calculator.pivot);
                if (calculator.anchorMin == calculator.anchorMax)
                {
                    calculator.pos = EditorGUILayout.Vector2Field("Position", calculator.pos);
                    calculator.size = EditorGUILayout.Vector2Field("Size", calculator.size);
                }
                else if (calculator.anchorMin.x == calculator.anchorMax.x && calculator.anchorMin.y != calculator.anchorMax.y)
                {
                    calculator.pos = EditorGUILayout.Vector2Field("Position", calculator.pos);
                    calculator.top = EditorGUILayout.FloatField("Top", calculator.top);
                    calculator.bottom = EditorGUILayout.FloatField("Bottom", calculator.bottom);
                    calculator.width = EditorGUILayout.FloatField("Width", calculator.width);
                }
                else if (calculator.anchorMin.x != calculator.anchorMax.x && calculator.anchorMin.y == calculator.anchorMax.y)
                {
                    calculator.pos = EditorGUILayout.Vector2Field("Position", calculator.pos);
                    calculator.left = EditorGUILayout.FloatField("Left", calculator.left);
                    calculator.right = EditorGUILayout.FloatField("Right", calculator.right);
                    calculator.height = EditorGUILayout.FloatField("Height", calculator.height);
                }
                else
                {
                    calculator.left = EditorGUILayout.FloatField("Left", calculator.left);
                    calculator.right = EditorGUILayout.FloatField("Right", calculator.right);
                    calculator.top = EditorGUILayout.FloatField("Top", calculator.top);
                    calculator.bottom = EditorGUILayout.FloatField("Bottom", calculator.bottom);
                }

            }
        }

        private GUIElement m_element;
        public GUIElement element
        {
            get { return m_element; }
            set
            {
                this.m_element = value;
                if (value.GetType() != typeof(GUICanvas))
                    calculatorDesign = new RectCalculatorEditor(value.calculator);
            }
        }
        private bool insFold = true;
        private RectCalculatorEditor calculatorDesign;

        public virtual void OnInspectorGUI()
        {
            if (calculatorDesign != null)
            {
                calculatorDesign.OnInspectorGUI();
            }
            insFold = FormatInspectorHeadGUI(insFold, "Element:" + element.GetType().Name, HeadGUI, ContentGUI);
        }
        private void ContentGUI()
        {

            this.Pan(() =>
            {
                element.name = EditorGUILayout.TextField("Name", element.name);
                using (new EditorGUI.DisabledGroupScope(true)) EditorGUILayout.RectField("Position", element.position);
                element.enable = EditorGUILayout.Toggle("Enable", element.enable);
            })
                  .FloatField("Rotate Angle", ref element.rotateAngle)
                  .Vector2Field("Rotate Offset", ref element.rotateOffset)
                  .ColorField("Color", ref element.color)
                  .ColorField("Content Color", ref element.contentColor)
                  .ColorField("Background Color", ref element.backgroundColor);
        }
        private bool HeadGUI(bool bo, string title)
        {
            this.BeginHorizontal()
                    .Foldout(ref bo, title, true)
                    .Pan(() => { element.active = EditorGUILayout.Toggle(element.active, GUILayout.Width(18)); })
                    .Button(() => { element.Reset(); }, EditorGUIUtility.IconContent("d_TreeEditor.Refresh"), GUILayout.Width(25))
                .EndHorizontal();
            return bo;
        }


        private Color preContentColor;
        private Color preBgColor;
        private Color preColor;
        private GUISkin preSkin;
        private Matrix4x4 preMat4x4;
        private bool preEnable;
        protected void BeginGUI()
        {
            preSkin = GUI.skin;
            preContentColor = GUI.contentColor;
            preBgColor = GUI.backgroundColor;
            preColor = GUI.color;
            preMat4x4 = GUI.matrix;
            preEnable = GUI.enabled;

            GUI.color = element.color;
            GUI.backgroundColor = element.backgroundColor;
            GUI.contentColor = element.contentColor;
            Vector2 tmp = new Vector2(element.rotateOffset.x * element.position.width, element.rotateOffset.y * element.position.height);
            GUIUtility.RotateAroundPivot(element.rotateAngle, element.position.center + tmp);

            //bool bo = this.element.enable;
            //if (bo)
            //{
            //    IGUIElement d = this.element;
            //    while (d.parent != null)
            //    {
            //        d = d.parent;
            //        bo = d.enable && bo;
            //        if (!bo) break;
            //    }
            //}
            //GUI.enabled = bo;
            GUI.enabled = false;

        }
        protected void EndGUI()
        {


            if (GUIElementSelection.element == this.element)
            {
                element.position.DrawOutLine(6, Color.magenta);
            }
            GUI.backgroundColor = preBgColor;
            GUI.contentColor = preContentColor;
            GUI.skin = preSkin;
            GUI.matrix = preMat4x4;
            GUI.color = preColor;
            GUI.enabled = preEnable;
            if (GUIElementSelection.element == this.element)
            {
                element.calculator.OnGUI();
            }
            if (GUIElementSelection.element != null && GUIElementSelection.element.parent == this.element)
            {
                element.position.DrawOutLine(2, Color.blue);
            }
        }
        public virtual void OnSceneGUI(Action children)
        {
        }



        private static void HorizontalView(Action draw, GUIStyle style)
        {
            if (style == GUIStyle.none)
                GUILayout.BeginHorizontal();
            else
                GUILayout.BeginHorizontal(style);
            if (draw != null) draw();
            GUILayout.EndHorizontal();
        }
        private static void VerticalView(Action draw, GUIStyle style)
        {
            if (style == GUIStyle.none)
                GUILayout.BeginVertical();
            else
                GUILayout.BeginVertical(style);
            if (draw != null) draw();
            GUILayout.EndVertical();
        }
        protected static bool FormatInspectorHeadGUI(bool fold, string title, Func<bool, string, bool> titledraw, Action draw)
        {
            VerticalView(() => {
                if (titledraw == null)
                    fold = EditorGUILayout.Foldout(fold, title, true);
                else
                    fold = titledraw(fold, title);
                HorizontalView(() => {
                    GUILayout.Space(20);
                    VerticalView(() => {
                        if (fold && draw != null)
                            draw();
                    }, GUIStyle.none);
                }, GUIStyle.none);
            }, "box");
            return fold;
        }
    }
}
 
/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
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

namespace IFramework.GUIDesign.RectDesign
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ElementDesignAttribute : Attribute
    {
        public Type designType { get; set; }
        public ElementDesignAttribute(Type designTYpe)
        {
            this.designType = designTYpe;
        }
    }
    public class ElementSceneView : ILayoutGUIDrawer
    {
        private Dictionary<Type, ElementDesign> dic = new Dictionary<Type, ElementDesign>();
        public GUICanvas canvas;
        public ElementSceneView()
        {
            var designs = typeof(ElementDesign).GetSubTypesInAssemblys().ToList().FindAll((t) => { return t.IsDefined(typeof(ElementDesignAttribute), false); });
            var eles = typeof(Element).GetSubTypesInAssemblys();
            foreach (var type in eles)
            {
                var typeTree = type.GetTypeTree();
                for (int i = 0; i < typeTree.Count; i++)
                {
                    Type des = designs.Find((t) => {
                        return (t.GetCustomAttributes(typeof(ElementDesignAttribute), false).First() as ElementDesignAttribute).designType == typeTree[i];
                    });
                    if (des != null)
                    {
                        dic.Add(type, Activator.CreateInstance(des) as ElementDesign);
                        break;

                    }
                }
            }
        }
        public void OnGUI(Rect rect)
        {
            canvas.canvasRect = rect;
            EleGUI(canvas);
        }
        public void EleGUI(Element ele)
        {
            ElementDesign des = dic[ele.GetType()];
            des.element = ele;
         des.OnSceneGUI(()=> {
              for (int i = 0; i < ele.Children.Count; i++)
              {
                  EleGUI(ele.Children[i] as Element);
              }
          });

        }

    }
    public class ElementDesignView : ILayoutGUIDrawer
    {
        private Dictionary<Type, ElementDesign> dic = new Dictionary<Type, ElementDesign>();
        protected Element element;
        ElementDesign Pan;
        private Vector2 scroll2;

        public ElementDesignView()
        {
            var designs = typeof(ElementDesign).GetSubTypesInAssemblys().ToList().FindAll((t) => { return t.IsDefined(typeof(ElementDesignAttribute), false); });
            var eles = typeof(Element).GetSubTypesInAssemblys();
            foreach (var type in eles)
            {
                var typeTree = type.GetTypeTree();
                for (int i = 0; i < typeTree.Count; i++)
                {
                   Type des= designs.Find((t) => {
                       return (t.GetCustomAttributes(typeof(ElementDesignAttribute), false).First() as ElementDesignAttribute ).designType== typeTree[i]; });
                    if (des!=null)
                    {
                        dic.Add(type, Activator.CreateInstance(des) as ElementDesign);
                        break;

                    }
                }
            }

            ElementSelection.onElementChange += (ele) =>
            {
                this.element = ele;
                if (ele!=null)
                {
                    Pan = dic[ele.GetType()];
                    Pan.element = ele;
                }
            };
        }
        public void OnGUI(Rect rect)
        {
            if (Pan == null) return;
            this.BeginArea(rect)
                    .BeginScrollView(ref scroll2)
                        .Pan(Pan.OnGUI)
                    .LayoutEndScrollView()
                .EndArea();
        }
    }
    public abstract class ElementDesign : ILayoutGUIDrawer
    {
        protected class GUIStyleDesign
        {
            public class GUIStyleStateDesign
            {
                public string name;
                public GUIStyleState state;

                public GUIStyleStateDesign(string name, GUIStyleState state)
                {
                    this.name = name;
                    this.state = state;
                    //state.scaledBackgrounds = new Texture2D[2];

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
            private List<GUIStyleStateDesign> states;
            private List<RectOffsetDesign> offsets;

            public GUIStyleDesign(GUIStyle style, string name)
            {
                this.name = name;
                this.style = style;
                states = new List<GUIStyleStateDesign>();
                offsets = new List<RectOffsetDesign>();
                states.Add(new GUIStyleStateDesign("normal", style.normal));
                states.Add(new GUIStyleStateDesign("onNormal", style.onNormal));
                states.Add(new GUIStyleStateDesign("hover", style.hover));
                states.Add(new GUIStyleStateDesign("onHover", style.onHover));
                states.Add(new GUIStyleStateDesign("active", style.active));
                states.Add(new GUIStyleStateDesign("onActive", style.onActive));
                states.Add(new GUIStyleStateDesign("focused", style.focused));
                states.Add(new GUIStyleStateDesign("onFocused", style.onFocused));
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
        protected class RectCalculatorDesign : ILayoutGUIDrawer
        {
            private RectCalculator calculator;

            public RectCalculatorDesign(RectCalculator calculator)
            {
                this.calculator = calculator;
            }
            private bool insFold = true;
            private bool anchorFold;
            public void OnGUI()
            {
                insFold = FormatFoldGUI(insFold, "RectCalculator", null, ContentGUI);
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
                                .Button(() => { calculator.pivot = new Vector2(1, 0.5f);
                                                calculator.anchorMax = new Vector2(0, 1);
                                                calculator.anchorMin = Vector2.zero; }, 
                                                "↕", GUILayout.Width(17), GUILayout.Height(40))
                                .Button(() => { calculator.pivot = new Vector2(0.5f, 0.5f);
                                                    calculator.anchorMin = Vector2.zero;
                                                    calculator.anchorMax = Vector2.one; }
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
                                     calculator.anchorMin = new Vector2(0,1);
                                 }, "↔", GUILayout.Width(40), GUILayout.Height(17)) 
                            .EndHorizontal();

                    });
                    this.DrawVertical(() =>
                    {
                        this/*.BeginHorizontal()*/
                                .Button(() => {
                                    calculator.pivot = new Vector2(0.5f,0.5f);
                                    calculator.anchorMax = new Vector2(1, calculator.anchorMax.y);
                                    calculator.anchorMin = new Vector2(0, calculator.anchorMin.y);

                                }, "↔", GUILayout.Width(40), GUILayout.Height(17))
                                .Button(() => {
                                    calculator.pivot = new Vector2(0.5f, 0.5f);
                                    calculator.anchorMax = new Vector2( calculator.anchorMax.x,1);
                                    calculator.anchorMin = new Vector2( calculator.anchorMin.x,0);
                                },
                                "↕", GUILayout.Width(17), GUILayout.Height(40))
                            /*.EndHorizontal()*/;

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
        private Element m_element;
        public Element element { get { return m_element; }set {
                this.m_element = value;
                if (value.GetType() != typeof(GUICanvas))
                    calculatorDesign = new RectCalculatorDesign(value.calculator);
            }
        }
        private bool insFold = true;
        RectCalculatorDesign calculatorDesign;


        private Color preContentColor;
        private Color preBgColor;
        private Color preColor;
        private GUISkin preSkin;
        private Matrix4x4 preMat4x4;
        private bool preEnable;
        public void BeginGUI()
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

            if (element.parent == null || element.enable == false)
                GUI.enabled = element.enable;
            else
            {
                bool bo = true;
                IElement d = this.element;
                while (d.parent != null)
                {
                    d = d.parent;
                    if (d.enable == false)
                    {
                        bo = false;
                        break;
                    }
                }
                GUI.enabled = bo;
            }
        }

        public void EndGUI()
        {
            if (element.CustomGUI != null) element.CustomGUI(element.position);

            if (ElementSelection.element == this.element)
            {
                element.position.DrawOutLine(6, Color.magenta);
            }
            GUI.backgroundColor = preBgColor;
            GUI.contentColor = preContentColor;
            GUI.skin = preSkin;
            GUI.matrix = preMat4x4;
            GUI.color = preColor;
            GUI.enabled = preEnable;
            if (ElementSelection.element == this.element)
            {
                element.calculator.OnGUI();
            }
            if (ElementSelection.element!=null && ElementSelection.element.parent == this.element)
            {
                element.position.DrawOutLine(2, Color.blue);
            }
        }
        public virtual void OnSceneGUI(Action children)
        {
        }
        public virtual void OnGUI()
        {
            if (calculatorDesign!=null)
            {
                calculatorDesign.OnGUI();
            }
            insFold = FormatFoldGUI(insFold, "Element:" + element.GetType().Name, HeadGUI, ContentGUI);
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
        protected static bool FormatFoldGUI(bool fold, string title, Func<bool, string, bool> titledraw, Action draw)
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
    [ElementDesign(typeof(TextElement))]
    public class TextElementDesign : ElementDesign
    {
        private TextElement textElement { get { return element as TextElement; } }
        private bool insFold = true;
        private GUIStyleDesign textStyleDrawer;


        public override void OnGUI()
        {
            base.OnGUI();
            if (textStyleDrawer == null)
                textStyleDrawer = new GUIStyleDesign(textElement.textStyle, "Text Style");
            insFold = FormatFoldGUI(insFold, "Text Element", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.LabelField("Text")
                .TextArea(ref textElement.text, GUILayout.Height(50))
                .LabelField("Tooltip")
                .TextArea(ref textElement.tooltip, GUILayout.Height(50))
                .ObjectField("Font", ref textElement.font, false)
                .IntField("Font Stize", ref textElement.fontSize)
                .Toggle("Rich Text", ref textElement.richText)
                 .Pan(() => {
                     textElement.overflow = (TextClipping)EditorGUILayout.EnumPopup("Over flow", textElement.overflow);
                     textElement.alignment = (TextAnchor)EditorGUILayout.EnumPopup("Alignment", textElement.alignment);
                     textElement.fontStyle = (FontStyle)EditorGUILayout.EnumPopup("Font Style", textElement.fontStyle);
                 });
            textStyleDrawer.OnGUI();
        }
    }
    [ElementDesign(typeof(TextLabel))]
    public class TextLabelDesign : ElementDesign
    {
        private TextLabel textElement { get { return element as TextLabel; } }

        private GUIStyleDesign textStyleDrawer;
        public override void OnSceneGUI(Action children)
        {
            if (!element.active) return;

            BeginGUI();
            GUI.Label(element.position, new GUIContent(textElement.text, textElement.tooltip), textElement.textStyle);
            if (children != null) children();
           
            EndGUI();
        }
    }
    [ElementDesign(typeof(TextField))]
    public class TextFieldDesign : ElementDesign
    {
        private TextField textElement { get { return element as TextField; } }

        private GUIStyleDesign textStyleDrawer;
        public override void OnSceneGUI(Action children)
        {
            if (!element.active) return;

            BeginGUI();
            textElement.text= GUI.TextField(textElement.position, textElement.text, textElement.textStyle);
            if (children != null) children();

            EndGUI();
        }
    }
    [ElementDesign(typeof(TextArea))]
    public class TextAreaDesign : ElementDesign
    {
        private TextArea textElement { get { return element as TextArea; } }

        private GUIStyleDesign textStyleDrawer;
        public override void OnSceneGUI(Action children)
        {
            if (!element.active) return;

            BeginGUI();
            textElement.text = GUI.TextArea(textElement.position, textElement.text, textElement.textStyle);
            if (children != null) children();

            EndGUI();
        }
    }
    [ElementDesign(typeof(Image))]
    public class ImageDesign : ElementDesign
    {
        private Image image { get { return element as Image; } }
        private bool insFold = true;
        public override void OnSceneGUI(Action children)
        {
            if (!image.active) return;
            BeginGUI();
            if (image != null)
                GUI.DrawTexture(image.position, image.image, image.mode, image.alphaBlend, image.imageAspect, image.color, image.borderWidths, image.borderRadius);
            if (children != null) children();

            EndGUI();
           
        }


        public override void OnGUI()
        {
            base.OnGUI();
            insFold = FormatFoldGUI(insFold, "Image", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.ObjectField("Image", ref image.image, false)
                .Pan(() => { image.mode = (ScaleMode)EditorGUILayout.EnumPopup("Mode", image.mode); })
                
                .Toggle("Alpha Blend", ref image.alphaBlend)
                .FloatField("Image Aspect", ref image.imageAspect)
                .FloatField("Border Radius", ref image.borderRadius)
                .Vector4Field("Border Widths", ref image.borderWidths);
        }
    }
    [ElementDesign(typeof(ShaderImage))]
    public class ShaderImageDesign : ElementDesign
    {
        private ShaderImage image { get { return element as ShaderImage; } }
        private bool insFold = true;

        public override void OnSceneGUI(Action children)
        {
            if (!image.active) return;
            BeginGUI();
            if (image != null)
            {
                if (image.material != null)
                {
                    if (image.pass > image.material.passCount)
                        image.pass = image.material.passCount - 1;
                }
                else image.pass = -1;
                Graphics.DrawTexture(image.position, image.image, image.sourceRect, image.leftBorder, image.rightBorder, image.topBorder, image.bottomBorder, image.material, image.pass);
            }
            if (children != null) children();

            EndGUI();
        }

        public override void OnGUI()
        {
            base.OnGUI();
            insFold = FormatFoldGUI(insFold, "ShaderImage", null, ContentGUI);
        }

        private void ContentGUI()
        {
            image.pass = EditorGUILayout.IntField("Pass", image.pass);
            if (image.material != null)
            {
                if (image.pass > image.material.passCount)
                    image.pass = image.material.passCount - 1;
            }
            this.IntField("Left Border", ref image.leftBorder)
                .IntField("Right Border", ref image.rightBorder)
                .IntField("Top Border", ref image.topBorder)
                .IntField("Bottom Border", ref image.bottomBorder)
                .RectField("Source Rect", ref image.sourceRect)
                .ObjectField("Image", ref image.image, false)
                .ObjectField("Image", ref image.material, false);
        }
    }

    [ElementDesign(typeof(ImageElement))]
    public class ImageElementDesign : ElementDesign
    {
        private ImageElement image { get { return element as ImageElement; } }
        private bool insFold = true;
        private GUIStyleDesign imageStyleDrawer;



        public override void OnGUI()
        {
            base.OnGUI();
            if (imageStyleDrawer == null)
                imageStyleDrawer = new GUIStyleDesign(image.imageStyle, "Image Style");
            insFold = FormatFoldGUI(insFold, "Image Element", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.ObjectField("Image", ref image.image, false);
            imageStyleDrawer.OnGUI();
        }
    }

    [ElementDesign(typeof(ImageLabel))]
    public class ImageLabelDesign : ImageElementDesign
    {
        private ImageLabel image { get { return element as ImageLabel; } }

        public override void OnSceneGUI(Action children)
        {
            if (!image.active) return;
            BeginGUI();
            GUI.Label(image.position, image.image, image.imageStyle);
            if (children != null) children();

            EndGUI();
        }
    }
    [ElementDesign(typeof(ImageBox))]
    public class ImageBoxDesign : ImageElementDesign
    {
        private ImageBox image { get { return element as ImageBox; } }
        public override void OnSceneGUI(Action children)
        {
            if (!image.active) return;
            BeginGUI();
            GUI.Box(image.position, image.image, image.imageStyle);
            if (children != null) children();

            EndGUI();
        }
    }

    [ElementDesign(typeof(ImageToggle))]
    public class ImageToggleDesign : ImageElementDesign
    {
        private ImageToggle toggle { get { return element as ImageToggle; } }
        private bool insFold = true;



        public override void OnGUI()
        {
            base.OnGUI();
            insFold = FormatFoldGUI(insFold, "Toggle", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.Toggle("Value", ref toggle.value);
        }
        public override void OnSceneGUI(Action children)
        {
            if (!toggle.active) return;
            BeginGUI();
            toggle.value = GUI.Toggle(toggle.position, toggle. value, toggle.image, toggle.imageStyle);
            if (children != null) children();

            EndGUI();


        }
    }
    [ElementDesign(typeof(Toggle))]
    public class ToggleDesign : ImageToggleDesign
    {
        private Toggle toggle { get { return element as Toggle; } }
        private bool insFold = true;

        public override void OnSceneGUI(Action children)
        {
            if (!toggle.active) return;
            BeginGUI();
            toggle.value = GUI.Toggle(toggle.position, toggle.value, new GUIContent(toggle.text, toggle.image, toggle.tooltip), toggle.imageStyle);
            if (children != null) children();
            EndGUI();
        }

        public override void OnGUI()
        {
            base.OnGUI();
            insFold = FormatFoldGUI(insFold, "Content", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.ETextField("Text", ref toggle.text)
                .ETextField("Tooltip", ref toggle.tooltip);
        }
    }

    [ElementDesign(typeof(ToolBarElement))]
    public class ToolBarElementDesign : ElementDesign
    {
        private ToolBarElement toolbar { get { return element as ToolBarElement; } }
        private bool insFold = true;
        private GUIStyleDesign styleDrawer;


        public override void OnGUI()
        {
            base.OnGUI();
            if (styleDrawer == null)
                styleDrawer = new GUIStyleDesign(toolbar.style, "ToolBar Style");
            insFold = FormatFoldGUI(insFold, "ToolBar Element", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.IntField("Value", ref toolbar.value);
            styleDrawer.OnGUI();
        }
    }
    [ElementDesign(typeof(TextToolBar))]
    public class TextToolBarDesign : ToolBarElementDesign
    {
        private TextToolBar toolbar { get { return element as TextToolBar; } }
        private bool insFold = true;

        public override void OnSceneGUI(Action children)
        {
            if (!toolbar.active) return;
            BeginGUI();
            toolbar.value = GUI.Toolbar(toolbar.position, toolbar.value, toolbar.texs, toolbar.style);
            if (children != null) children();
            EndGUI();
        }

        public override void OnGUI()
        {
            base.OnGUI();
            insFold = FormatFoldGUI(insFold, "ToolBar Values", null, ContentGUI);
        }
        private void ContentGUI()
        {
            int size = EditorGUILayout.IntField("Size", toolbar.texs.Length);
            if (size > toolbar.texs.Length)
            {
                var list = toolbar.texs.ToList();
                while (list.Count < size)
                    list.Add("");
                toolbar.texs = list.ToArray();
            }
            else if (size < toolbar.texs.Length)
            {
                var list = toolbar.texs.ToList();
                while (list.Count > size)
                    list.RemoveAt(list.Count - 1);
                toolbar.texs = list.ToArray();
            }

            for (int i = 0; i < toolbar.texs.Length; i++)
                toolbar.texs[i] = EditorGUILayout.TextField("Text", toolbar.texs[i]);
        }
    }
    [ElementDesign(typeof(ImageToolBar))]
    public class ImageToolBarDesign : ToolBarElementDesign
    {
        private ImageToolBar toolbar { get { return element as ImageToolBar; } }
        private bool insFold = true;

        public override void OnSceneGUI(Action children)
        {
            if (!toolbar.active) return;
            BeginGUI();
            toolbar.value = GUI.Toolbar(toolbar.position, toolbar.value, toolbar.texs, toolbar.style);
            if (children != null) children();
            EndGUI();
        }

        public override void OnGUI()
        {
            base.OnGUI();
            insFold = FormatFoldGUI(insFold, "ToolBar Values", null, ContentGUI);
        }
        private void ContentGUI()
        {
            int size = EditorGUILayout.IntField("Size", toolbar.texs.Length);
            if (size > toolbar.texs.Length)
            {
                var list = toolbar.texs.ToList();
                while (list.Count < size)
                    list.Add(new Texture2D(100, 100));
                toolbar.texs = list.ToArray();
            }
            else if (size < toolbar.texs.Length)
            {
                var list = toolbar.texs.ToList();
                while (list.Count > size)
                    list.RemoveAt(list.Count - 1);
                toolbar.texs = list.ToArray();
            }
            for (int i = 0; i < toolbar.texs.Length; i++)
                toolbar.texs[i] = (Texture2D)EditorGUILayout.ObjectField("Image", toolbar.texs[i], typeof(Texture2D), false);
        }
    }

    [ElementDesign(typeof(SliderElement))]
    public class SliderElementDesign : ElementDesign
    {
        private SliderElement slider { get { return element as SliderElement; } }
        private bool insFold = true;
        private GUIStyleDesign thumbDrawer;
        private GUIStyleDesign sliderDrawer;



        public override void OnGUI()
        {
            base.OnGUI();
            if (sliderDrawer == null)
                sliderDrawer = new GUIStyleDesign(slider.slider, "Slider Style");
            if (thumbDrawer == null)
                thumbDrawer = new GUIStyleDesign(slider.thumb, "Thumb Style");
            insFold = FormatFoldGUI(insFold, "Slider", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.FloatField("Value", ref slider.value)
                .FloatField("Start Value", ref slider.startValue)
                .FloatField("End Value", ref slider.endValue);
            sliderDrawer.OnGUI();
            thumbDrawer.OnGUI();
        }
    }
    [ElementDesign(typeof(VerticalSlider))]
    public class VerticalSliderDesign : SliderElementDesign
    {
        private VerticalSlider toolbar { get { return element as VerticalSlider; } }

        public override void OnSceneGUI(Action children)
        {
            if (!toolbar.active) return;
            BeginGUI();
            toolbar.value = GUI.VerticalSlider(toolbar.position, toolbar.value, toolbar.startValue, toolbar.endValue, toolbar.slider, toolbar.thumb);
            if (children != null) children();

            EndGUI();
        }

    }
    [ElementDesign(typeof(HorizontalSlider))]
    public class HorizontalSliderDesign : SliderElementDesign
    {
        private VerticalSlider toolbar { get { return element as VerticalSlider; } }

        public override void OnSceneGUI(Action children)
        {
            if (!toolbar.active) return;
            BeginGUI();
            toolbar.value = GUI.HorizontalSlider(toolbar.position, toolbar.value, toolbar.startValue, toolbar.endValue, toolbar.slider, toolbar.thumb);
            if (children != null) children();

            EndGUI();
        }

    }

    [ElementDesign(typeof(Box))]
    public class BoxDesign : ImageElementDesign
    {
        private Box ele { get { return element as Box; } }
        private bool insFold = true;

        public override void OnSceneGUI(Action children)
        {
            if (!ele.active) return;
            BeginGUI();
            GUI.Box(ele.position, new GUIContent(ele.text, ele.image, ele.tooltip), ele.imageStyle);
            if (children != null) children();

            EndGUI();
        }

        public override void OnGUI()
        {
            base.OnGUI();
            insFold = FormatFoldGUI(insFold, "Content", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.ETextField("Text", ref ele.text)
                 .ETextField("Tooltip", ref ele.tooltip);
        }
    }
    [ElementDesign(typeof(Button))]
    public class ButtonDesign : TextElementDesign
    {
        private Button ele { get { return element as Button; } }
        private bool insFold = true;
        private GUIStyleDesign btnStyleDrawer;

        public override void OnSceneGUI(Action children)
        {
            if (!ele.active) return;
            BeginGUI();
            GUI.Button(ele.position, "", ele.btnStyle);
            if (ele.image != null)
                GUI.DrawTexture(ele.position, ele.image, ele.mode, ele.alphaBlend, ele.imageAspect, ele.color, ele.borderWidths, ele.borderRadius);
            GUI.Label(ele.position, new GUIContent(ele.text, ele.tooltip), ele.textStyle);
            if (children != null) children();
            EndGUI();
        }
        public override void OnGUI()
        {
            base.OnGUI();
            if (btnStyleDrawer == null)
                btnStyleDrawer = new GUIStyleDesign(ele.btnStyle, "ButtonStyle");
            insFold = FormatFoldGUI(insFold, "Button", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.ObjectField("Image", ref ele.image, false)
                .Pan(() => { ele.mode = (ScaleMode)EditorGUILayout.EnumPopup("Mode", ele.mode); })
             
                .Toggle("Alpha Blend", ref ele.alphaBlend)
                .FloatField("Image Aspect", ref ele.imageAspect)
                .FloatField("Border Radius", ref ele.borderRadius)
                .Vector4Field("Border Widths", ref ele.borderWidths);
            btnStyleDrawer.OnGUI();
        }

    }

    [ElementDesign(typeof(ScrollView))]
    public class ScrollViewDesign : ElementDesign
    {
        private ScrollView ele { get { return element as ScrollView; } }
        private bool insFold = true;
        private GUIStyleDesign HstyleDrawer;
        private GUIStyleDesign VstyleDrawer;

        public override void OnSceneGUI(Action children)
        {
            if (!ele.active) return;

            BeginGUI();
            ele.value = GUI.BeginScrollView(ele.position, ele.value, ele.contentRect, ele.alwaysShowHorizontal, ele.alwaysShowVertical, ele.Hstyle, ele.Vstyle);
            if (children != null) children();
            GUI.EndScrollView();

            EndGUI();
        }

        public override void OnGUI()
        {
            base.OnGUI();
            if (HstyleDrawer == null)
                HstyleDrawer = new GUIStyleDesign(ele.Hstyle, "Hrizontal Style");
            if (VstyleDrawer == null)
                VstyleDrawer = new GUIStyleDesign(ele.Vstyle, "Vertical Style");
            insFold = FormatFoldGUI(insFold, "ScrollView", null, ContentGUI);
        }
        private void ContentGUI()
        {
            using(new EditorGUI.DisabledScope(true)) EditorGUILayout.RectField("Perfect Content Rect", ele.perfectContentRect);
            ele.contentRect = EditorGUILayout.RectField("Content Rect", ele.contentRect);
            ele.alwaysShowHorizontal = EditorGUILayout.Toggle("Always Show Horizontal", ele.alwaysShowHorizontal);
            ele.alwaysShowVertical = EditorGUILayout.Toggle("Always Show Vertical", ele.alwaysShowVertical);
            ele.value = EditorGUILayout.Vector2Field("Value", ele.value);
            HstyleDrawer.OnGUI();
            VstyleDrawer.OnGUI();
        }

    }
    [ElementDesign(typeof(GUICanvas))]
    public class GUICanvasDesign : ElementDesign
    {
        private GUICanvas ele { get { return element as GUICanvas; } }
        private bool insFold = true;
        public override void OnSceneGUI(Action children)
        {
            if (!ele.active) return;
            GUI.BeginClip(ele.canvasRect);
            BeginGUI();
            if (children!=null) children();
            EndGUI();
            GUI.EndClip();
        }

        public override void OnGUI()
        {
            base.OnGUI();
            insFold = FormatFoldGUI(insFold, "Canvas", null, ContentGUI);
        }
        private void ContentGUI()
        {
            ele.canvasRect = EditorGUILayout.RectField("Canvas Rect", ele.canvasRect);
        }
    }
}
 
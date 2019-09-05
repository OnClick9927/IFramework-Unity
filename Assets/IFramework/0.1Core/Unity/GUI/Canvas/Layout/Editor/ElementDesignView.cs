/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.14f1
 *Date:           2019-10-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace IFramework.GUIDesign.LayoutDesign
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

    public class ElementDesignView:ILayoutGUIDrawer
    {
        protected Element element;
        ElementDesign Pan;
        private Vector2 scroll2;

        public ElementDesignView()
        {
            ElementSelection.onElementChange += (ele) =>
            {
                this.element = ele;
                Pan = ElementDesign.Create(ele);
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
    public class ElementDesign:ILayoutGUIDrawer
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

                                        if (size!= state.scaledBackgrounds.Length)
                                        {
                                            Texture2D[] txs = new Texture2D[state.scaledBackgrounds.Length];
                                            for (int i = 0; i < state.scaledBackgrounds.Length; i++)
                                            {
                                                txs[i] = txs[i];
                                            }
                                            state.scaledBackgrounds = new Texture2D[size];
                                            for (int i = 0; i < (txs.Length<size?txs.Length:size); i++)
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

        protected Element element;
        private bool insFold=true;
        public void OnSceneGUI()
        {
            element.OnGUI();
        }
        public virtual void OnGUI()
        {
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
                .ColorField("Background Color", ref element.backgroundColor)
                .BeginVertical("box")
                    .Label("GUILayoutOptions", EditorStyles.boldLabel)
                    .Foldout(ref element.enableSize, "Enable Size", true)
                    .Pan(() => { if (element.enableSize) this.Vector2Field("Size", ref element.size); })
                    .Foldout(ref element.enableMinSize, "Enable Min Size", true)
                    .Pan(() => { if (element.enableMinSize) this.Vector2Field("Min Size", ref element.minSize); })
                    .Foldout(ref element.enableMaxSize, "Enable Max Size", true)
                    .Pan(() => { if (element.enableMaxSize) this.Vector2Field("Max Size", ref element.maxSize); })
                    .Foldout(ref element.enableExpandHeight, "Enable Expand Height", true)
                    .Pan(() => { if (element.enableExpandHeight) this.Toggle("Expand Height", ref element.expandHeight); })
                    .Foldout(ref element.enableExpandWidth, "Enable Expand Width", true)
                    .Pan(() => { if (element.enableExpandWidth) this.Toggle("Expand Width", ref element.expandWidth); })
                .EndVertical();
            
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
        public static ElementDesign Create(Element element)
        {
            if (element == null) return null;
            Type type = element.GetType();
            Dictionary<Type, Type> attrDic = new Dictionary<Type, Type>();
            typeof(ElementDesign).GetSubTypesInAssemblys().ToList().FindAll((t) =>
            {
                return t.IsDefined(typeof(ElementDesignAttribute), false);
            })
            .ForEach((t) => {

                ElementDesignAttribute cus = t.GetCustomAttributes(typeof(ElementDesignAttribute), false).First() as ElementDesignAttribute;
                attrDic.Add(cus.designType, t);
            });
            var typeTree = type.GetTypeTree();
            for (int i = 0; i < typeTree.Count; i++)
            {
                if (attrDic.ContainsKey(typeTree[i]))
                {
                    type = attrDic[typeTree[i]];
                    break;
                }
            }

            if (!type.IsSubclassOf(typeof(ElementDesign)))
                type = typeof(ElementDesign);

            ElementDesign res = Activator.CreateInstance(type) as ElementDesign;
            res.element = element;
            return res;
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
                    textElement.fontStyle= (FontStyle)EditorGUILayout.EnumPopup("Font Style", textElement.fontStyle);
                });

            textStyleDrawer.OnGUI();
        }
    }
    [ElementDesign(typeof(HaveChildElement))]
    public class HaveChildElementDesign : ElementDesign
    {
        private HaveChildElement haveChildElement { get { return element as HaveChildElement; } }
        private bool insFold = true;
        public override void OnGUI()
        {
            base.OnGUI();
            insFold = FormatFoldGUI(insFold, "Children Elements", null, ContentGUI);
        }
        private void ContentGUI()
        {
            Event e = Event.current;
            using (new EditorGUI.DisabledScope(true))
            {
                for (int i = 0; i < haveChildElement.Children.Count; i++)
                {
                    EditorGUILayout.TextField(haveChildElement.Children[i].GetType().Name, haveChildElement.Children[i].name, "ObjectField");
                    Rect r = GUILayoutUtility.GetLastRect();
                    if (r.Contains(e.mousePosition) && e.clickCount == 2)
                        ElementSelection.element = haveChildElement.Children[i] as Element;
                }
            }
        }

    }

    [ElementDesign(typeof(Image))]
    public class ImageDesign : ElementDesign
    {
        private Image image { get { return element as Image; } }
        private bool insFold = true;
        public override void OnGUI()
        {
            base.OnGUI();
            insFold = FormatFoldGUI(insFold, "Image", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.ObjectField("Image",ref image.image, false)
                .Pan(() => { image.mode = (ScaleMode)EditorGUILayout.EnumPopup("Mode", image.mode);   })
                
                .Toggle("Alpha Blend",ref image.alphaBlend)
                .FloatField("Image Aspect",ref image.imageAspect)
                .FloatField("Border Radius",ref image.borderRadius)
                .Vector4Field("Border Widths",ref image.borderWidths);
        }
    }

    [ElementDesign(typeof(ShaderImage))]
    public class ShaderImageDesign : ElementDesign
    {
        private ShaderImage image { get { return element as ShaderImage; } }
        private bool insFold = true;
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
            this.IntField("Left Border",ref image.leftBorder)
                .IntField("Right Border",ref image.rightBorder)
                .IntField("Top Border",ref image.topBorder)
                .IntField("Bottom Border",ref image.bottomBorder)
                .RectField("Source Rect",ref image.sourceRect)
                .ObjectField("Image",ref image.image, false)
                .ObjectField("Image",ref image.material, false);
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
            this.ObjectField("Image",ref image.image, false);
            imageStyleDrawer.OnGUI();
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
           this.Toggle("Value",ref toggle.value);
        }
    }
    [ElementDesign(typeof(Toggle))]
    public class ToggleDesign : ImageToggleDesign
    {
        private Toggle toggle { get { return element as Toggle; } }
        private bool insFold = true;
        public override void OnGUI()
        {
            base.OnGUI();
            insFold = FormatFoldGUI(insFold, "Content", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.ETextField("Text",ref toggle.text)
                .ETextField("Tooltip",ref toggle.tooltip);
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
            this.IntField("Value",ref toolbar.value);
            styleDrawer.OnGUI();
        }
    }
    [ElementDesign(typeof(TextToolBar))]
    public class TextToolBarDesign : ToolBarElementDesign
    {
        private TextToolBar toolbar { get { return element as TextToolBar; } }
        private bool insFold = true;
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
                this.ETextField("Text",ref toolbar.texs[i]);
        }
    }

    [ElementDesign(typeof(ImageToolBar))]
    public class ImageToolBarDesign : ToolBarElementDesign
    {
        private ImageToolBar toolbar { get { return element as ImageToolBar; } }
        private bool insFold = true;
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
                this.ObjectField("Image",ref toolbar.texs[i], false);
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
            this.FloatField("Value",ref slider.value)
                .FloatField("Start Value",ref slider.startValue)
                .FloatField("End Value",ref slider.endValue);
            sliderDrawer.OnGUI();
            thumbDrawer.OnGUI();
        }
    }
    [ElementDesign(typeof(HaveChildImageElement))]
    public class HaveChildImageElementDesign : HaveChildElementDesign
    {
        private HaveChildImageElement ele { get { return element as HaveChildImageElement; } }
        private bool insFold = true;
        private GUIStyleDesign imageStyleDrawer;

        public override void OnGUI()
        {
            base.OnGUI();
            if (imageStyleDrawer == null)
                imageStyleDrawer = new GUIStyleDesign(ele.imageStyle, "Image Style");
            insFold = FormatFoldGUI(insFold, "Image", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.ObjectField("Image",ref ele.image, false);
            imageStyleDrawer.OnGUI();
        }
    }
    [ElementDesign(typeof(Horizontal))]
    public class HorizontalDesign : HaveChildImageElementDesign
    {
        private Horizontal ele { get { return element as Horizontal; } }
        private bool insFold = true;
        public override void OnGUI()
        {
            base.OnGUI();
            insFold = FormatFoldGUI(insFold, "Content", null, ContentGUI);
        }
        private void ContentGUI()
        {
           this.ETextField("Text",ref ele.text)
                .ETextField("Tooltip",ref ele.tooltip);
        }
    }
    [ElementDesign(typeof(Vertical))]
    public class VerticalDesign : HaveChildImageElementDesign
    {
        private Vertical ele { get { return element as Vertical; } }
        private bool insFold = true;
        public override void OnGUI()
        {
            base.OnGUI();
            insFold = FormatFoldGUI(insFold, "Content", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.ETextField("Text",ref ele.text)
                .ETextField("Tooltip",ref ele.tooltip);
        }
    }
    [ElementDesign(typeof(ImageArea))]
    public class ImageAreaDesign : HaveChildImageElementDesign
    {
        private ImageArea ele { get { return element as ImageArea; } }
        private bool insFold = true;
        public override void OnGUI()
        {
            base.OnGUI();
            insFold = FormatFoldGUI(insFold, "Area Position", null, ContentGUI);
        }
        private void ContentGUI()
        {
            ele.areaRect = EditorGUILayout.RectField("Area Rect", ele.areaRect);
        }
    }
    [ElementDesign(typeof(Area))]
    public class AreaDesign : ImageAreaDesign
    {
        private Area ele { get { return element as Area; } }
        private bool insFold = true;
        public override void OnGUI()
        {
            base.OnGUI();
            insFold = FormatFoldGUI(insFold, "Area Content", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.ETextField("Text",ref ele.text)
                  .ETextField("tooltip",ref ele.tooltip);
        }
    }
    [ElementDesign(typeof(Box))]
    public class BoxDesign : ImageElementDesign
    {
        private Box ele { get { return element as Box; } }
        private bool insFold = true;
        public override void OnGUI()
        {
            base.OnGUI();
            insFold = FormatFoldGUI(insFold, "Content", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.ETextField("Text",ref ele.text)
                .ETextField("Tooltip",ref ele.tooltip);
        }
    }
    [ElementDesign(typeof(Button))]
    public class ButtonDesign : TextElementDesign
    {
        private Button ele { get { return element as Button; } }
        private bool insFold = true;
        private GUIStyleDesign btnStyleDrawer;

        public override void OnGUI()
        {
            base.OnGUI();
            if (btnStyleDrawer == null)
                btnStyleDrawer = new GUIStyleDesign(ele.btnStyle, "ButtonStyle");
            insFold = FormatFoldGUI(insFold, "Button", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.ObjectField("Image",ref ele.image, false)
                .Pan(() => { ele.mode = (ScaleMode)EditorGUILayout.EnumPopup("Mode", ele.mode); })
                
                .Toggle("Alpha Blend",ref ele.alphaBlend)
                .FloatField("Image Aspect",ref ele.imageAspect)
                .FloatField("Border Radius", ref ele.borderRadius)
                .Vector4Field("Border Widths",ref ele.borderWidths);
            btnStyleDrawer.OnGUI();
        }

    }
    [ElementDesign(typeof(Space))]
    public class SpaceDesign : ElementDesign
    {
        private Space ele { get { return element as Space; } }
        private bool insFold = true;
        public override void OnGUI()
        {
            base.OnGUI();
            insFold = FormatFoldGUI(insFold, "Space", null, ContentGUI);
        }
        private void ContentGUI()
        {
            this.FloatField("Pixels",ref ele.pixels);
        }
    }
    [ElementDesign(typeof(ScrollView))]
    public class ScrollViewDesign : HaveChildElementDesign
    {
        private ScrollView ele { get { return element as ScrollView; } }
        private bool insFold = true;
        private GUIStyleDesign HstyleDrawer;
        private GUIStyleDesign VstyleDrawer;

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
            this.Toggle("Always Show Horizontal",ref ele.alwaysShowHorizontal)
                .Toggle("Always Show Vertical",ref ele.alwaysShowVertical)
                .Vector2Field("Value",ref ele.value);
            HstyleDrawer.OnGUI();
            VstyleDrawer.OnGUI();
        }

    }
    [ElementDesign(typeof(GUICanvas))]
    public class GUICanvasDesign : AreaDesign
    {
        private GUICanvas ele { get { return element as GUICanvas; } }
        private bool insFold = true;
        public override void OnGUI()
        {
            base.OnGUI();
            insFold = FormatFoldGUI(insFold, "Canvas", null, ContentGUI);
        }
        private void ContentGUI()
        {
            ele.CanvasRect = EditorGUILayout.RectField("Canvas Rect", ele.CanvasRect);
        }
    }
}

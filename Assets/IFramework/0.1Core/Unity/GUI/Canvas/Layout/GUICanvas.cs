/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-10-09
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Xml;
using System.Reflection;

namespace IFramework.GUIDesign.LayoutDesign
{
    public class ElementSelection
    {
        public delegate void OnElementChange(Element element);
        public static event OnElementChange onElementChange;
        private static Element m_element;
        public static Element element
        {
            get { return m_element; }
            set
            {
                m_element = value;
#if UNITY_EDITOR

                if (UnityEditor.EditorWindow.focusedWindow != null)
                    UnityEditor.EditorWindow.focusedWindow.Repaint();
#endif
                if (Event.current != null && Event.current.type != EventType.Layout)
                    Event.current.Use();
                if (onElementChange != null)
                    onElementChange(m_element);
            }
        }

        public static Element copyElement { get; set; }
        public static Element dragElement { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class GUICreateMenuPathAttribute : Attribute
    {
        public Type InspectorGUIDrawerType { get; set; }
        public string path { get; set; }
        public GUICreateMenuPathAttribute() { }
        public GUICreateMenuPathAttribute(string path)
        {
            this.path = path;
        }
    }

    public interface IElement
    {
        string name { get; set; }
        bool active { get; set; }
        bool enable { get; set; }

        int depth { get; }
        Rect position { get; }
        int siblingIndex { get; }
        string treePath { get; }
        IElement parent { get; }
        IElement root { get; }
        List<IElement> Children { get; }

        void OnGUI();
        void Reset();
        IElement Find(string path);
        T Find<T>(string path) where T : IElement;
        XmlElement Serialize(XmlDocument doc);
        void DeSerialize(XmlElement element);
    }
    public abstract class Element : IElement
    {
        protected class GUIStyleSerializer
        {
            public class GUIStyleStateSerializer
            {
                public string name;
                public GUIStyleState state;
                public GUIStyleStateSerializer(string name, GUIStyleState state)
                {
                    this.name = name;
                    this.state = state;
                }

                public XmlElement Serializate(XmlDocument doc)
                {
                    XmlElement root = doc.CreateElement(name);
                    SerializeField(root, "textColor", state.textColor);
                    if (state.background != null)
                    {
                        SerializeField(root, "background", state.background.CreateReadableTexture().EncodeToPNG());
                    }
#if UNITY_EDITOR
                    XmlElement ele = doc.CreateElement("scaledBackgrounds");
                    for (int i = 0; i < state.scaledBackgrounds.Length; i++)
                        if (state.scaledBackgrounds[i] != null)
                        {
                            SerializeField(ele, "background", state.scaledBackgrounds[i].CreateReadableTexture().EncodeToPNG());
                        }
                    root.AppendChild(ele);
#endif
                    return root;
                }
                public void DeSerializate(XmlElement root)
                {
                    if (root.SelectSingleNode("background") != null)
                    {
                        byte[] bytes = new byte[0];
                        DeSerializeField(root, "background", ref bytes);
                        state.background = new Texture2D(1, 1);
                        state.background.LoadImage(bytes);
                        state.background.hideFlags = HideFlags.DontSave;
                    }
                    Color color = Color.white;
                    DeSerializeField(root, "textColor", ref color);
                    state.textColor = color;
#if UNITY_EDITOR

                    XmlElement ele = root.SelectSingleNode("scaledBackgrounds") as XmlElement;
                    state.scaledBackgrounds = new Texture2D[ele.ChildNodes.Count];
                    List<Texture2D> txs = new List<Texture2D>();
                    for (int i = 0; i < ele.ChildNodes.Count; i++)
                    {
                        byte[] bs = new byte[0];
                        StringConvert.TryConvert(ele.ChildNodes[i].InnerText, out bs);

                        Texture2D txx = new Texture2D(1, 1);
                        txx.hideFlags = HideFlags.DontSaveInEditor;
                        txx.LoadImage(bs);
                        txs.Add(txx);
                    }
                    state.scaledBackgrounds = txs.ToArray();
#endif

                }
            }
            public class RectOffsetSerializer
            {
                private RectOffset offset;
                private string name;
                public RectOffsetSerializer(string name, RectOffset offset)
                {
                    this.name = name;
                    this.offset = offset;
                }

                public XmlElement Serializate(XmlDocument doc)
                {
                    XmlElement root = doc.CreateElement(name);
                    XmlNode node = root.OwnerDocument.CreateTextNode(StringConvert.ConvertToString(offset, offset.GetType()));
                    root.AppendChild(node);
                    return root;
                }
                public void DeSerializate(XmlElement ele)
                {
                    StringConvert.TryConvert(ele.InnerText, out offset);
                }
            }

            private GUIStyle style;
            private string name;
            private List<GUIStyleStateSerializer> states;
            private List<RectOffsetSerializer> offsets;
            public GUIStyleSerializer(GUIStyle style, string name)
            {
                this.name = name;
                this.style = style;
                states = new List<GUIStyleStateSerializer>();
                offsets = new List<RectOffsetSerializer>();
                states.Add(new GUIStyleStateSerializer("normal", style.normal));
                states.Add(new GUIStyleStateSerializer("onNormal", style.onNormal));
                states.Add(new GUIStyleStateSerializer("hover", style.hover));
                states.Add(new GUIStyleStateSerializer("onHover", style.onHover));
                states.Add(new GUIStyleStateSerializer("active", style.active));
                states.Add(new GUIStyleStateSerializer("onActive", style.onActive));
                states.Add(new GUIStyleStateSerializer("focused", style.focused));
                states.Add(new GUIStyleStateSerializer("onFocused", style.onFocused));
                offsets.Add(new RectOffsetSerializer("border", style.border));
                offsets.Add(new RectOffsetSerializer("margin", style.margin));
                offsets.Add(new RectOffsetSerializer("padding", style.padding));
                offsets.Add(new RectOffsetSerializer("overflow", style.overflow));
            }

            public XmlElement Serializate(XmlDocument doc)
            {
                XmlElement root = doc.CreateElement("GUIStyle");
                root.SetAttribute("Name", name);
                SerializeField(root, "name", style.name);
                SerializeField(root, "fontSize", style.fontSize);
                SerializeField(root, "fontStyle", style.fontStyle);
                SerializeField(root, "alignment", style.alignment);
                SerializeField(root, "wordWrap", style.wordWrap);
                SerializeField(root, "richText", style.richText);
                SerializeField(root, "clipping", style.clipping);
                SerializeField(root, "imagePosition", style.imagePosition);
                SerializeField(root, "contentOffset", style.contentOffset);
                SerializeField(root, "fixedWidth", style.fixedWidth);
                SerializeField(root, "fixedHeight", style.fixedHeight);
                SerializeField(root, "stretchWidth", style.stretchWidth);
                SerializeField(root, "stretchHeight", style.stretchHeight);

                XmlElement statesE = doc.CreateElement("GUIStyleStates");
                for (int i = 0; i < states.Count; i++)
                    statesE.AppendChild(states[i].Serializate(doc));
                XmlElement offsetsE = doc.CreateElement("RectOffsets");
                for (int i = 0; i < offsets.Count; i++)
                    offsetsE.AppendChild(offsets[i].Serializate(doc));
                root.AppendChild(offsetsE);
                root.AppendChild(statesE);
                return root;
            }
            public void DeSerializate(XmlElement root)
            {
                string _name = "";
                DeSerializeField(root, "name", ref _name);
                style.name = _name;
                int _fontSize = 0;
                DeSerializeField(root, "fontSize", ref _fontSize);
                style.fontSize = _fontSize;
                FontStyle _fontStyle = FontStyle.Bold;
                DeSerializeField(root, "fontStyle", ref _fontStyle);
                style.fontStyle = _fontStyle;
                TextAnchor _alignment = TextAnchor.LowerCenter;
                DeSerializeField(root, "alignment", ref _alignment);
                style.alignment = _alignment;
                bool _wordWrap = false;
                DeSerializeField(root, "wordWrap", ref _wordWrap);
                style.wordWrap = _wordWrap;
                bool _richText = false;
                DeSerializeField(root, "richText", ref _richText);
                style.richText = _richText;
                TextClipping _clipping = TextClipping.Clip;
                DeSerializeField(root, "clipping", ref _clipping);
                style.clipping = _clipping;
                ImagePosition _imagePosition = ImagePosition.ImageAbove;
                DeSerializeField(root, "imagePosition", ref _imagePosition);
                style.imagePosition = _imagePosition;
                Vector2 _contentOffset = Vector2.zero;
                DeSerializeField(root, "contentOffset", ref _contentOffset);
                style.contentOffset = _contentOffset;
                float _fixedWidth = 0;
                DeSerializeField(root, "fixedWidth", ref _fixedWidth);
                style.fixedWidth = _fixedWidth;
                float _fixedHeight = 0;
                DeSerializeField(root, "fixedHeight", ref _fixedHeight);
                style.fixedHeight = _fixedHeight;
                bool _stretchWidth = false;
                DeSerializeField(root, "stretchWidth", ref _stretchWidth);
                style.stretchWidth = _stretchWidth;
                bool _stretchHeight = false;
                DeSerializeField(root, "stretchHeight", ref _stretchHeight);
                style.stretchHeight = _stretchHeight;


                XmlElement statesE = root.SelectSingleNode("GUIStyleStates") as XmlElement;
                states.Clear();
                for (int i = 0; i < statesE.ChildNodes.Count; i++)
                {
                    XmlElement stateE = statesE.ChildNodes[i] as XmlElement;
                    GUIStyleStateSerializer d = default(GUIStyleStateSerializer);

                    switch (stateE.Name)
                    {
                        case "normal": d = new GUIStyleStateSerializer(stateE.Name, style.normal); break;
                        case "onNormal": d = new GUIStyleStateSerializer(stateE.Name, style.onNormal); break;
                        case "hover": d = new GUIStyleStateSerializer(stateE.Name, style.hover); break;
                        case "onHover": d = new GUIStyleStateSerializer(stateE.Name, style.onHover); break;
                        case "onFocused": d = new GUIStyleStateSerializer(stateE.Name, style.onFocused); break;
                        case "focused": d = new GUIStyleStateSerializer(stateE.Name, style.focused); break;
                        case "active": d = new GUIStyleStateSerializer(stateE.Name, style.active); break;
                        case "onActive": d = new GUIStyleStateSerializer(stateE.Name, style.onActive); break;
                    }
                    d.DeSerializate(stateE);
                    states.Add(d);
                }
                XmlElement offsetsE = root.SelectSingleNode("RectOffsets") as XmlElement;
                offsets.Clear();
                for (int i = 0; i < offsetsE.ChildNodes.Count; i++)
                {
                    XmlElement stateE = offsetsE.ChildNodes[i] as XmlElement;
                    RectOffsetSerializer d = default(RectOffsetSerializer);
                    switch (stateE.Name)
                    {
                        case "margin": d = new RectOffsetSerializer(stateE.Name, style.margin); break;
                        case "padding": d = new RectOffsetSerializer(stateE.Name, style.padding); break;
                        case "overflow": d = new RectOffsetSerializer(stateE.Name, style.overflow); break;
                        case "border": d = new RectOffsetSerializer(stateE.Name, style.border); break;
                    }
                    d.DeSerializate(stateE);
                    offsets.Add(d);
                }

            }
        }
        public Action<bool> onActiveChange { get; set; }
        public Action<bool> onEnableChange { get; set; }

        private string m_name;
        private bool m_enable;
        private bool m_active;
        public string name { get { return m_name; } set { m_name = value; } }
        public bool active
        {
            get { return m_active; }
            set
            {
                m_active = value;
                if (onActiveChange != null)
                    onActiveChange(value);
            }
        }
        public bool enable
        {
            get { return m_enable; }
            set
            {
                m_enable = value;
                if (onEnableChange != null)
                    onEnableChange(value);
            }
        }

        public float rotateAngle;
        public Vector2 rotateOffset;
        public Color color;
        public Color contentColor;
        public Color backgroundColor;
        public Vector2 size;
        public Vector2 minSize;
        public Vector2 maxSize;
        public bool expandHeight;
        public bool expandWidth;
        public bool enableMinSize;
        public bool enableSize;
        public bool enableMaxSize;
        public bool enableExpandHeight;
        public bool enableExpandWidth;


        public Rect position { get; protected set; }
        public int depth
        {
            get
            {
                if (parent == null) return 0;
                return parent.depth + 1;
            }
        }
        private IElement m_parent;
        public IElement root
        {
            get
            {
                IElement tmp = this;
                while (tmp.parent != null)
                    tmp = tmp.parent;
                return tmp;
            }
        }
        public string treePath
        {
            get
            {
                string tmp = name;
                IElement ele = this;
                while (ele.parent != null)
                {
                    ele = ele.parent;
                    tmp = ele.name + "/" + tmp;
                }
                return tmp;
            }
        }
        public IElement parent
        {
            get { return m_parent; }
            set
            {
                if (m_parent == value) return;
                if (m_parent != null)
                    m_parent.Children.Remove(this);
                if (value == null)
                    m_parent = value;
                else
                {
                    if (!value.GetType().IsSubclassOf(typeof(HaveChildElement)))
                    {
                        Log.E(value.name + " can't HaveChild");
                        return;
                    }
                    m_parent = value;
                    m_parent.Children.Add(this);
                }

            }
        }
        public int siblingIndex
        {
            get
            {
                if (parent == null) return -1;
                for (int i = 0; i < parent.Children.Count; i++)
                    if (parent.Children[i] == this)
                        return i;
                return -1;
            }
        }
        public List<IElement> Children { get { return children; } }
        protected readonly List<IElement> children = new List<IElement>();
        public Action<Rect> CustomGUI { get; set; }


        protected Element() { Reset(); }
        protected Element(Element other)
        {
            m_name = other.m_name;
            m_enable = other.m_enable;
            m_active = other.m_active;
            rotateAngle = other.rotateAngle;
            rotateOffset = other.rotateOffset;
            color = other.color;
            contentColor = other.contentColor;
            backgroundColor = other.backgroundColor;
            enableMinSize = other.enableMinSize;
            enableSize = other.enableSize;
            enableMaxSize = other.enableMaxSize;
            enableExpandHeight = other.enableExpandHeight;
            enableExpandWidth = other.enableExpandWidth;
            size = other.size;
            minSize = other.minSize;
            maxSize = other.maxSize;
            expandHeight = other.expandHeight;
            expandWidth = other.expandWidth;
        }
        public virtual void Reset()
        {
            m_name = GetType().Name;
            m_enable = m_active = true;
            color = contentColor = backgroundColor = Color.white;
            rotateAngle = 0;
            rotateOffset = Vector2.zero;
            enableMinSize = enableSize = enableMaxSize = enableExpandHeight = enableExpandWidth = false;
            size = minSize = maxSize = new Vector2(100, 100);
            expandWidth = expandHeight = false;
        }

        public IElement Find(string path)
        {
            if (children.Count == 0) return null;
            if (!path.Contains("/"))
            {
                for (int i = 0; i < children.Count; i++)
                {
                    IElement tmp = children[i] as IElement;
                    if (tmp.name == path)
                        return tmp;
                }
                return null;
            }
            else
            {
                int index = path.IndexOf("/");
                string tmp = path.Substring(0, index);
                path = path.Substring(index + 1);
                for (int i = 0; i < children.Count; i++)
                {
                    IElement tmpE = children[i] as IElement;
                    if (tmpE.name == tmp)
                        return tmpE.Find(path);
                }
                return null;
            }

        }
        public T Find<T>(string path) where T : IElement
        {
            return (T)Find(path);
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

            GUI.color = color;
            GUI.backgroundColor = backgroundColor;
            GUI.contentColor = contentColor;
            Vector2 tmp = new Vector2(rotateOffset.x * position.width, rotateOffset.y * position.height);
            GUIUtility.RotateAroundPivot(rotateAngle, position.center + tmp);

            if (parent == null || enable == false)
                GUI.enabled = enable;
            else
            {
                bool bo = true;
                IElement d = this;
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
        protected void EndGUI()
        {
            if (CustomGUI != null) CustomGUI(position);
#if UNITY_EDITOR
            if (ElementSelection.element == this)
            {
                UnityEditor.Handles.color = Color.magenta;
                UnityEditor.Handles.DrawAAPolyLine(2, new Vector3(position.x,
                                             position.y,
                                             0),
                              new Vector3(position.x,
                                             position.yMax,
                                             0),
                              new Vector3(position.xMax,
                                             position.yMax,
                                             0),
                              new Vector3(position.xMax,
                                             position.y,
                                             0),
                              new Vector3(position.x,
                                             position.y,
                                             0)
                                );

                UnityEditor.Handles.color = Color.white;
            }
#endif

            GUI.backgroundColor = preBgColor;
            GUI.contentColor = preContentColor;
            GUI.skin = preSkin;
            GUI.matrix = preMat4x4;
            GUI.color = preColor;
            GUI.enabled = preEnable;
        }

        protected GUILayoutOption[] CalcGUILayOutOptions()
        {
            List<GUILayoutOption> options = new List<GUILayoutOption>();
            if (enableSize)
            {
                options.Add(GUILayout.Width(size.x));
                options.Add(GUILayout.Height(size.y));
            }
            if (enableMinSize)
            {
                options.Add(GUILayout.MinWidth(minSize.x));
                options.Add(GUILayout.MinHeight(minSize.y));
            }
            if (enableMaxSize)
            {
                options.Add(GUILayout.MaxWidth(maxSize.x));
                options.Add(GUILayout.MaxHeight(maxSize.y));
            }
            if (enableExpandHeight)
                options.Add(GUILayout.ExpandHeight(expandHeight));
            if (enableExpandWidth)
                options.Add(GUILayout.ExpandWidth(expandWidth));
            return options.ToArray();
        }
        public abstract void OnGUI();


        public virtual XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = doc.CreateElement("Element");
            SerializeField(root, "name", name);
            SerializeField(root, "rotateAngle", rotateAngle);
            SerializeField(root, "rotateOffset", rotateOffset);
            SerializeField(root, "color", color);
            SerializeField(root, "contentColor", contentColor);
            SerializeField(root, "backgroundColor", backgroundColor);
            SerializeField(root, "active", active);
            SerializeField(root, "enable", enable);
            SerializeField(root, "enableMinSize", enableMinSize);
            SerializeField(root, "enableSize", enableSize);
            SerializeField(root, "enableMaxSize", enableMaxSize);
            SerializeField(root, "enableExpandHeight", enableExpandHeight);
            SerializeField(root, "enableExpandWidth", enableExpandWidth);
            SerializeField(root, "size", size);
            SerializeField(root, "minSize", minSize);
            SerializeField(root, "maxSize", maxSize);
            SerializeField(root, "expandHeight", expandHeight);
            SerializeField(root, "expandWidth", expandWidth);
            root.SetAttribute("ElementType", GetType().Name);
            return root;
        }
        public virtual void DeSerialize(XmlElement root)
        {
            DeSerializeField(root, "name", ref m_name);
            DeSerializeField(root, "rotateAngle", ref rotateAngle);
            DeSerializeField(root, "rotateOffset", ref rotateOffset);
            DeSerializeField(root, "color", ref color);
            DeSerializeField(root, "contentColor", ref contentColor);
            DeSerializeField(root, "backgroundColor", ref backgroundColor);
            DeSerializeField(root, "active", ref m_active);
            DeSerializeField(root, "enable", ref m_enable);
            DeSerializeField(root, "enableMinSize", ref enableMinSize);
            DeSerializeField(root, "enableSize", ref enableSize);
            DeSerializeField(root, "enableMaxSize", ref enableMaxSize);

            DeSerializeField(root, "enableExpandHeight", ref enableExpandHeight);
            DeSerializeField(root, "enableExpandWidth", ref enableExpandWidth);
            DeSerializeField(root, "size", ref size);
            DeSerializeField(root, "minSize", ref minSize);
            DeSerializeField(root, "maxSize", ref maxSize);
            DeSerializeField(root, "expandHeight", ref expandHeight);
            DeSerializeField(root, "expandWidth", ref expandWidth);

        }
        protected static XmlElement SerializeField<T>(XmlElement root, string name, T value)
        {
            try
            {
                XmlElement ele = root.OwnerDocument.CreateElement(name);
                if (value != null)
                {
                    Type type = value.GetType();
                    XmlNode node = root.OwnerDocument.CreateTextNode(StringConvert.ConvertToString(value, type));
                    ele.AppendChild(node);
                    root.AppendChild(ele);
                }
                else
                {
                    Log.W("GUICanvas:  " + name + "  value is mull,will not Serializate");
                }
                return root;
            }
            catch (Exception)
            {
                throw new Exception(name);
            }

        }
        protected static void DeSerializeField<T>(XmlElement root, string name, ref T obj)
        {
            try
            {
                T obj1;
                XmlNode node = root.SelectSingleNode(name);
                if (node != null)
                {
                    StringConvert.TryConvert(node.InnerText, out obj1);
                    obj = obj1;
                }
                else
                    Log.W("GUICanvas:   Not Find Field   " + name);
            }
            catch
            {
                throw new Exception(name);
            }
        }
    }
    public abstract class HaveChildElement : Element
    {
        protected HaveChildElement() : base() { }
        protected HaveChildElement(HaveChildElement other) : base(other)
        {
            for (int i = 0; i < other.children.Count; i++)
            {
                Element element = other.children[i] as Element;
                Element copy = Activator.CreateInstance(element.GetType(), other.children[i]) as Element;
                copy.parent = this;
            }
        }

        protected void ChildrenOnGUI()
        {
            if (!active) return;
            children.ForEach((drawer) => {
                drawer.OnGUI();
            });
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);
            XmlElement ele = root.OwnerDocument.CreateElement("children");
            for (int i = 0; i < children.Count; i++)
                ele.AppendChild(children[i].Serialize(doc));
            root.PrependChild(ele);
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            children.Clear();
            XmlElement ele = root.SelectSingleNode("children") as XmlElement;
            for (int i = 0; i < ele.ChildNodes.Count; i++)
            {
                XmlElement child = ele.ChildNodes[i] as XmlElement;
                Type type = typeof(Element).GetSubTypesInAssemblys().ToList().Find((tmp) =>
                {
                    return tmp.Name == child.GetAttribute("ElementType");
                });
                //Type type = (Type)Assembly.GetExecutingAssembly().CreateInstance(child.GetAttribute("ElementType"));

                Element element = Activator.CreateInstance(type, null) as Element;
                element.DeSerialize(child);
                element.parent = this;
            }
        }
    }
    public static class HaveChildElementExtension
    {
        public static T Element<T>(this T self, Element element) where T : HaveChildElement
        {
            element.parent = self;
            return self;
        }
    }

    public abstract class TextElement : Element
    {
        public string text;
        public string tooltip;
        public Font font;
        public FontStyle fontStyle;
        public int fontSize;
        public TextAnchor alignment;
        public TextClipping overflow;
        public bool richText;

        protected GUIStyle m_style;
        public virtual GUIStyle textStyle
        {
            get
            {
                if (m_style == null)
                    m_style = new GUIStyle(GUI.skin.label);
                return m_style;
            }
            set { m_style = new GUIStyle(value); }
        }

        protected TextElement() : base() { }
        protected TextElement(TextElement other) : base(other)
        {
            text = other.text;
            tooltip = other.tooltip;
            font = other.font;
            fontStyle = other.fontStyle;
            fontSize = other.fontSize;
            alignment = other.alignment;
            overflow = other.overflow;
            richText = other.richText;
            m_style = new GUIStyle(other.m_style);
        }
        public override void Reset()
        {
            base.Reset();
            text = string.Empty;
            tooltip = string.Empty;
            alignment = TextAnchor.MiddleLeft;
            font = null;
            fontSize = 10;
            richText = true;
            overflow = TextClipping.Clip;
            fontStyle = FontStyle.Normal;
        }

        public override void OnGUI()
        {
            if (!active) return;
            textStyle.font = font;
            textStyle.fontStyle = fontStyle;
            textStyle.fontSize = fontSize;
            textStyle.alignment = alignment;
            textStyle.clipping = overflow;
            textStyle.richText = richText;
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);

            SerializeField(root, "fontStyle", fontStyle);
            SerializeField(root, "fontSize", fontSize);
            SerializeField(root, "alignment", alignment);
            SerializeField(root, "overflow", overflow);
            SerializeField(root, "richText", richText);
            SerializeField(root, "text", text);
            SerializeField(root, "tooltip", tooltip);
            root.AppendChild(new GUIStyleSerializer(textStyle, "Text Style").Serializate(doc));
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            DeSerializeField(root, "fontStyle", ref fontStyle);
            DeSerializeField(root, "fontSize", ref fontSize);
            DeSerializeField(root, "alignment", ref alignment);
            DeSerializeField(root, "overflow", ref overflow);
            DeSerializeField(root, "richText", ref richText);
            DeSerializeField(root, "text", ref text);
            DeSerializeField(root, "tooltip", ref tooltip);

            XmlElement styleE = root.SelectSingleNode("GUIStyle") as XmlElement;
            m_style = new GUIStyle();

            new GUIStyleSerializer(textStyle, "Text Style").DeSerializate(styleE);
        }
    }
    [GUICreateMenuPath(path = "Text/Label")]
    public class TextLabel : TextElement
    {
        public TextLabel() : base() { }
        public TextLabel(TextElement other) : base(other) { }

        public override void OnGUI()
        {
            base.OnGUI();
            if (!active) return;
            BeginGUI();
            GUILayout.Label(new GUIContent(text, tooltip), textStyle, CalcGUILayOutOptions());
            position = GUILayoutUtility.GetLastRect();
            EndGUI();
        }
    }
    [GUICreateMenuPath(path = "Text/TextField")]
    public class TextField : TextElement
    {
        public Action<string> onValueChange { get; set; }

        public override GUIStyle textStyle
        {
            get
            {
                if (m_style == null)
                    m_style = new GUIStyle(GUI.skin.textField);
                return m_style;
            }
            set { m_style = new GUIStyle(value); }
        }
        public TextField() : base() { }
        public TextField(TextElement other) : base(other) { }

        public override void OnGUI()
        {
            base.OnGUI();
            if (!active) return;
            BeginGUI();
            string tmp = GUILayout.TextField(text, textStyle, CalcGUILayOutOptions());
            position = GUILayoutUtility.GetLastRect();
            if (tmp != text)
            {
                text = tmp;
                if (onValueChange != null) onValueChange(tmp);
            }
            EndGUI();
        }
    }
    [GUICreateMenuPath(path = "Text/TextArea")]
    public class TextArea : TextElement
    {
        public Action<string> onValueChange { get; set; }
        public override GUIStyle textStyle
        {
            get
            {
                if (m_style == null)
                    m_style = new GUIStyle(GUI.skin.textArea);
                return m_style;
            }
            set { m_style = new GUIStyle(value); }
        }

        public TextArea() : base() { }
        public TextArea(TextElement other) : base(other) { }

        public override void OnGUI()
        {
            base.OnGUI();
            if (!active) return;
            BeginGUI();
            string tmp = GUILayout.TextArea(text, textStyle, CalcGUILayOutOptions());
            position = GUILayoutUtility.GetLastRect();
            if (tmp != text)
            {
                text = tmp;
                if (onValueChange != null) onValueChange(tmp);
            }
            EndGUI();
        }
    }

    [GUICreateMenuPath(path = "Image/Image")]
    public class Image : Element
    {
        public Texture2D image;
        public ScaleMode mode;
        public bool alphaBlend;
        public float imageAspect;
        public Vector4 borderWidths;
        public float borderRadius;

        public Image() : base() { }
        public Image(Image other) : base(other)
        {
            image = other.image;
            mode = other.mode;
            alphaBlend = other.alphaBlend;
            imageAspect = other.imageAspect;
            borderWidths = other.borderWidths;
            borderRadius = other.borderRadius;
        }
        public override void Reset()
        {
            base.Reset();
            mode = ScaleMode.StretchToFill;
            imageAspect = 2;
            alphaBlend = true;
            borderRadius = 2;
            borderWidths = Vector4.zero;
            image = null;
        }

        public override void OnGUI()
        {
            if (!active) return;
            GUILayout.Label("", CalcGUILayOutOptions());
            position = GUILayoutUtility.GetLastRect();
            BeginGUI();
            if (image != null)
                GUI.DrawTexture(position, image, mode, alphaBlend, imageAspect, color, borderWidths, borderRadius);
            EndGUI();
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);
            SerializeField(root, "mode", mode);
            SerializeField(root, "alphaBlend", alphaBlend);
            SerializeField(root, "imageAspect", imageAspect);
            SerializeField(root, "borderWidths", borderWidths);
            SerializeField(root, "borderRadius", borderRadius);
            if (image != null)
                SerializeField(root, "image", image.CreateReadableTexture().EncodeToPNG());
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            DeSerializeField(root, "mode", ref mode);
            DeSerializeField(root, "alphaBlend", ref alphaBlend);
            DeSerializeField(root, "imageAspect", ref imageAspect);
            DeSerializeField(root, "borderWidths", ref borderWidths);
            DeSerializeField(root, "borderRadius", ref borderRadius);
            if (root.SelectSingleNode("image") != null)
            {
                byte[] bs = new byte[0];
                DeSerializeField(root, "image", ref bs);
                image = new Texture2D(200, 200);
                image.LoadImage(bs);
                image.hideFlags = HideFlags.DontSaveInEditor;
            }
        }
    }
    [GUICreateMenuPath(path = "Image/ShaderImage")]
    public class ShaderImage : Element
    {
        public int leftBorder, rightBorder, topBorder, bottomBorder, pass;
        public Rect sourceRect;
        public Material material;
        public Texture2D image;
        public ShaderImage() : base() { }
        public ShaderImage(ShaderImage other) : base(other)
        {
            leftBorder = other.leftBorder;
            rightBorder = other.rightBorder;
            topBorder = other.topBorder;
            bottomBorder = other.bottomBorder;
            pass = other.pass;
            image = other.image;
            material = other.material;
            sourceRect = other.sourceRect;
        }
        public override void Reset()
        {
            base.Reset();
            image = null;
            leftBorder = rightBorder = topBorder = bottomBorder = 100;
            pass = -1;
            material = null;
            sourceRect = new Rect(1, 1, -1, -1);
        }

        public override void OnGUI()
        {
            BeginGUI();
            GUILayout.Label("", CalcGUILayOutOptions());
            position = GUILayoutUtility.GetLastRect();
            if (image != null)
            {
                if (material != null)
                {
                    if (pass > material.passCount)
                        pass = material.passCount - 1;
                }
                else pass = -1;
                Graphics.DrawTexture(position, image, sourceRect, leftBorder, rightBorder, topBorder, bottomBorder, material, pass);
            }
            EndGUI();
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);
            SerializeField(root, "leftBorder", leftBorder);
            SerializeField(root, "rightBorder", rightBorder);
            SerializeField(root, "topBorder", topBorder);
            SerializeField(root, "bottomBorder", bottomBorder);
            SerializeField(root, "pass", pass);
            SerializeField(root, "sourceRect", sourceRect);
            if (image != null)
                SerializeField(root, "image", image.CreateReadableTexture().EncodeToPNG());
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            DeSerializeField(root, "leftBorder", ref leftBorder);
            DeSerializeField(root, "rightBorder", ref rightBorder);
            DeSerializeField(root, "topBorder", ref topBorder);
            DeSerializeField(root, "bottomBorder", ref bottomBorder);
            DeSerializeField(root, "pass", ref pass);
            DeSerializeField(root, "sourceRect", ref sourceRect);
            if (root.SelectSingleNode("image") != null)
            {
                byte[] bs = new byte[0];
                DeSerializeField(root, "image", ref bs);
                image = new Texture2D(200, 200);
                image.LoadImage(bs);
                image.hideFlags = HideFlags.DontSaveInEditor;
            }
        }
    }
    public abstract class ImageElement : Element
    {
        public Texture2D image;
        protected GUIStyle m_style;
        public virtual GUIStyle imageStyle
        {
            get
            {
                if (m_style == null)
                    m_style = new GUIStyle(GUI.skin.box);
                return m_style;
            }
            set { m_style = new GUIStyle(value); }
        }

        protected ImageElement() : base() { }
        protected ImageElement(ImageElement other) : base(other)
        {
            image = other.image;
            m_style = new GUIStyle(other.m_style);
        }
        public override void Reset()
        {
            base.Reset();
            image = null;
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);

            root.AppendChild(new GUIStyleSerializer(imageStyle, "Image Style").Serializate(doc));
            if (image != null)
                SerializeField(root, "image", image.CreateReadableTexture().EncodeToPNG());

            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            if (root.SelectSingleNode("image") != null)
            {
                byte[] bs = new byte[0];
                DeSerializeField(root, "image", ref bs);
                image = new Texture2D(200, 200);
                image.LoadImage(bs);
                image.hideFlags = HideFlags.DontSaveInEditor;
            }
            XmlElement styleE = root.SelectSingleNode("GUIStyle") as XmlElement;
            m_style = new GUIStyle();

            new GUIStyleSerializer(imageStyle, "Image Style").DeSerializate(styleE);
        }
    }
    [GUICreateMenuPath(path = "Image/ImageLabel")]
    public class ImageLabel : ImageElement
    {
        public override GUIStyle imageStyle
        {
            get
            {
                if (m_style == null)
                    m_style = new GUIStyle(GUI.skin.label);
                return m_style;
            }
            set { m_style = new GUIStyle(value); }
        }
        public ImageLabel() : base() { }
        public ImageLabel(ImageLabel other) : base(other) { }

        public override void OnGUI()
        {
            if (!active) return;
            BeginGUI();
            GUILayout.Label(image, imageStyle, CalcGUILayOutOptions());
            position = GUILayoutUtility.GetLastRect();
            EndGUI();
        }
    }
    [GUICreateMenuPath(path = "Image/ImageBox")]
    public class ImageBox : ImageElement
    {
        public ImageBox() : base() { }
        public ImageBox(ImageBox other) : base(other) { }
        public override void OnGUI()
        {
            if (!active) return;
            BeginGUI();
            GUILayout.Box(image, imageStyle, CalcGUILayOutOptions());
            position = GUILayoutUtility.GetLastRect();
            EndGUI();
        }
    }
    [GUICreateMenuPath(path = "Toggle/ImageToggle")]
    public class ImageToggle : ImageElement
    {
        public bool value;
        public Action<bool> onValueChange { get; set; }
        public override GUIStyle imageStyle
        {
            get
            {
                if (m_style == null)
                    m_style = new GUIStyle(GUI.skin.toggle);
                return m_style;
            }
            set { m_style = new GUIStyle(value); }
        }

        public ImageToggle() : base() { }
        public ImageToggle(ImageToggle other) : base(other)
        {
            value = other.value;
        }
        public override void Reset()
        {
            base.Reset();
            value = false;
        }

        public override void OnGUI()
        {
            if (!active) return;
            BeginGUI();
            bool tmp = DrawGUI();
            position = GUILayoutUtility.GetLastRect();
            if (tmp != value)
            {
                value = tmp;
                if (onValueChange != null) onValueChange(tmp);
            }
            EndGUI();
        }
        protected virtual bool DrawGUI()
        {
            return GUILayout.Toggle(value, image, imageStyle, CalcGUILayOutOptions());
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);
            SerializeField(root, "value", value);
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            DeSerializeField(root, "value", ref value);

        }
    }

    [GUICreateMenuPath(path = "Toggle/Toggle")]
    public class Toggle : ImageToggle
    {
        public string text;
        public string tooltip;

        public Toggle() : base() { }
        public Toggle(Toggle other) : base(other)
        {
            text = other.text;
            tooltip = other.tooltip;
        }
        public override void Reset()
        {
            base.Reset();
            text = string.Empty;
            tooltip = string.Empty;
        }

        protected override bool DrawGUI()
        {
            return GUILayout.Toggle(value, new GUIContent(text, image, tooltip), imageStyle, CalcGUILayOutOptions());
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);
            SerializeField(root, "text", text);
            SerializeField(root, "tooltip", tooltip);
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            DeSerializeField(root, "text", ref text);
            DeSerializeField(root, "tooltip", ref tooltip);
        }
    }


    public abstract class ToolBarElement : Element
    {
        public Action<int> onValueChange { get; set; }
        public int value;
        private GUIStyle m_style;
        public GUIStyle style
        {
            get
            {
                if (m_style == null)
                    m_style = new GUIStyle(GUI.skin.button);
                return m_style;
            }
            set { m_style = new GUIStyle(value); }
        }

        protected ToolBarElement() : base() { }
        protected ToolBarElement(ToolBarElement other) : base(other)
        {
            value = other.value;
            m_style = new GUIStyle(other.m_style);
        }
        public override void Reset()
        {
            base.Reset();
            value = 0;
        }

        public override void OnGUI()
        {
            if (!active) return;
            BeginGUI();
            GUILayout.Label("", CalcGUILayOutOptions());
            position = GUILayoutUtility.GetLastRect();
            int tmp = DrawGUI(position);
            if (tmp != value)
            {
                value = tmp;
                if (onValueChange != null) onValueChange(tmp);
            }
            EndGUI();
        }
        protected abstract int DrawGUI(Rect position);

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);

            SerializeField(root, "value", value);
            root.AppendChild(new GUIStyleSerializer(style, "ToolBar Style").Serializate(doc));
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            DeSerializeField(root, "value", ref value);
            XmlElement styleE = root.SelectSingleNode("GUIStyle") as XmlElement;
            m_style = new GUIStyle();

            new GUIStyleSerializer(style, "ToolBar Style").DeSerializate(styleE);
        }
    }
    [GUICreateMenuPath(path = "TextToolBar/ToolBar")]
    public class TextToolBar : ToolBarElement
    {
        public string[] texs;

        public TextToolBar() : base() { }
        public TextToolBar(TextToolBar other) : base(other)
        {
            texs = new string[other.texs.Length];
            for (int i = 0; i < texs.Length; i++)
            {
                texs[i] = other.texs[i];
            }
        }
        public override void Reset()
        {
            base.Reset();
            texs = new string[0];
        }

        protected override int DrawGUI(Rect position)
        {
            return GUI.Toolbar(position, value, texs, style);
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);
            XmlElement texsE = doc.CreateElement("texs");
            for (int i = 0; i < texs.Length; i++)
                SerializeField(texsE, "value", texs[i]);
            root.AppendChild(texsE);
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            XmlElement texsE = root.SelectSingleNode("texs") as XmlElement;
            texs = new string[texsE.ChildNodes.Count];
            for (int i = 0; i < texs.Length; i++)
                texs[i] = texsE.ChildNodes[i].InnerText;
        }
    }
    [GUICreateMenuPath(path = "TextToolBar/ImageToolBar")]
    public class ImageToolBar : ToolBarElement
    {
        public Texture2D[] texs;

        public ImageToolBar() : base() { }
        public ImageToolBar(ImageToolBar other) : base(other)
        {
            texs = new Texture2D[other.texs.Length];
            for (int i = 0; i < texs.Length; i++)
            {
                texs[i] = other.texs[i];
            }
        }
        public override void Reset()
        {
            base.Reset();
            texs = new Texture2D[0];
        }

        protected override int DrawGUI(Rect position)
        {
            return GUI.Toolbar(position, value, texs, style);
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);
            XmlElement texsE = doc.CreateElement("texs");
            for (int i = 0; i < texs.Length; i++)
                SerializeField(texsE, "image", texs[i].CreateReadableTexture().EncodeToPNG());
            root.AppendChild(texsE);
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            XmlElement texsE = root.SelectSingleNode("texs") as XmlElement;
            texs = new Texture2D[texsE.ChildNodes.Count];
            for (int i = 0; i < texs.Length; i++)
            {
                byte[] bs = new byte[0];
                texs[i] = new Texture2D(100, 100);
                StringConvert.TryConvert(texsE.ChildNodes[i].InnerText, out bs);
                texs[i].LoadImage(bs);
                texs[i].hideFlags = HideFlags.DontSaveInEditor;

            }
        }
    }

    public abstract class SliderElement : Element
    {
        public float startValue;
        public float endValue;
        public float value;
        protected GUIStyle m_slider;
        protected GUIStyle m_thumb;
        public abstract GUIStyle slider { get; set; }
        public abstract GUIStyle thumb { get; set; }

        public Action<float> onValueChange { get; set; }
        protected SliderElement() : base() { }
        protected SliderElement(SliderElement other) : base(other)
        {
            startValue = other.startValue;
            endValue = other.endValue;
            value = other.value;
            m_slider = new GUIStyle(other.m_slider);
            m_thumb = new GUIStyle(other.m_thumb);
        }
        public override void Reset()
        {
            base.Reset();
            startValue = endValue = value = 0;
        }

        public override void OnGUI()
        {
            if (!active) return;
            BeginGUI();
            float tmp = 0;
            tmp = DrawGUI();
            position = GUILayoutUtility.GetLastRect();
            if (tmp != value)
            {
                value = tmp;
                if (onValueChange != null)
                    onValueChange(value);
            }
            EndGUI();
        }
        protected abstract float DrawGUI();

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);

            SerializeField(root, "startValue", startValue);
            SerializeField(root, "endValue", endValue);
            SerializeField(root, "value", value);
            XmlElement stylesE = doc.CreateElement("Styles");
            stylesE.AppendChild(new GUIStyleSerializer(slider, "Slider Style").Serializate(doc));
            stylesE.AppendChild(new GUIStyleSerializer(thumb, "Thumb Style").Serializate(doc));
            root.AppendChild(stylesE);
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            DeSerializeField(root, "startValue", ref startValue);
            DeSerializeField(root, "endValue", ref endValue);
            DeSerializeField(root, "value", ref value);
            m_slider = new GUIStyle();
            m_thumb = new GUIStyle();


            XmlElement styleE = root.SelectSingleNode("Styles") as XmlElement;
            new GUIStyleSerializer(slider, "Slider Style").DeSerializate(styleE.FirstChild as XmlElement);
            new GUIStyleSerializer(thumb, "Thumb Style").DeSerializate(styleE.LastChild as XmlElement);
        }
    }
    [GUICreateMenuPath(path = "Slider/Horizontal")]
    public class HorizontalSlider : SliderElement
    {
        public override GUIStyle slider
        {
            get
            {
                if (m_slider == null)
                {
                    m_slider = new GUIStyle(GUI.skin.horizontalSlider);
                }
                return m_slider;
            }
            set { m_slider = new GUIStyle(value); }
        }
        public override GUIStyle thumb
        {
            get
            {
                if (m_thumb == null)
                {
                    m_thumb = new GUIStyle(GUI.skin.horizontalSliderThumb);
                }
                return m_thumb;
            }
            set { m_thumb = new GUIStyle(value); }
        }

        public HorizontalSlider() : base() { }
        public HorizontalSlider(SliderElement other) : base(other) { }

        protected override float DrawGUI()
        {
            return GUILayout.HorizontalSlider(value, startValue, endValue, slider, thumb, CalcGUILayOutOptions());
        }
    }
    [GUICreateMenuPath(path = "Slider/Vertical")]
    public class VerticalSlider : SliderElement
    {
        public override GUIStyle slider
        {
            get
            {
                if (m_slider == null)
                {
                    m_slider = new GUIStyle(GUI.skin.verticalSlider);
                }
                return m_slider;
            }
            set { m_slider = new GUIStyle(value); }
        }
        public override GUIStyle thumb
        {
            get
            {
                if (m_thumb == null)
                {
                    m_thumb = new GUIStyle(GUI.skin.verticalScrollbarThumb);
                }
                return m_thumb;
            }
            set { m_thumb = new GUIStyle(value); }
        }

        public VerticalSlider() : base() { }
        public VerticalSlider(SliderElement other) : base(other) { }

        protected override float DrawGUI()
        {
            return GUILayout.VerticalSlider(value, startValue, endValue, slider, thumb, CalcGUILayOutOptions());
        }
    }


    public abstract class HaveChildImageElement : HaveChildElement
    {
        public Texture2D image;
        protected GUIStyle m_style;
        public virtual GUIStyle imageStyle
        {
            get
            {
                if (m_style == null)
                    m_style = new GUIStyle(GUI.skin.label);
                return m_style;
            }
            set { m_style = new GUIStyle(value); }
        }

        protected HaveChildImageElement() : base() { }
        protected HaveChildImageElement(HaveChildImageElement other) : base(other)
        {
            image = other.image;
            m_style = new GUIStyle(other.m_style);
        }
        public override void Reset()
        {
            base.Reset();
            image = null;
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);
            if (image != null)
                SerializeField(root, "image", image.CreateReadableTexture().EncodeToPNG());
            root.AppendChild(new GUIStyleSerializer(imageStyle, "Image Style").Serializate(doc));
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            if (root.SelectSingleNode("image") != null)
            {
                byte[] bs = new byte[0];
                DeSerializeField(root, "image", ref bs);
                image = new Texture2D(200, 200);
                image.LoadImage(bs);
                image.hideFlags = HideFlags.DontSaveInEditor;
            }
            m_style = new GUIStyle();

            XmlElement styleE = root.SelectSingleNode("GUIStyle") as XmlElement;
            new GUIStyleSerializer(imageStyle, "Image Style").DeSerializate(styleE);
        }
    }
    [GUICreateMenuPath(path = "Horizontal/ImageHorizontal")]
    public class ImageHorizontal : HaveChildImageElement
    {
        public override GUIStyle imageStyle
        {
            get
            {
                if (m_style == null)
                    m_style = new GUIStyle(GUI.skin.label);
                return m_style;
            }
            set { m_style = new GUIStyle(value); }
        }

        public ImageHorizontal() : base() { }
        public ImageHorizontal(ImageHorizontal other) : base(other) { }

        public override void OnGUI()
        {
            if (!active) return;
            BeginGUI();
            GUILayout.BeginHorizontal(image, imageStyle, CalcGUILayOutOptions());
            ChildrenOnGUI();
            GUILayout.EndHorizontal();
            EndGUI();
        }
    }
    [GUICreateMenuPath(path = "Horizontal/Horizontal")]
    public class Horizontal : HaveChildImageElement
    {
        public string text;
        public string tooltip;

        public Horizontal() : base() { }
        public Horizontal(Horizontal other) : base(other)
        {
            text = other.text;
            tooltip = other.tooltip;
        }
        public override void Reset()
        {
            base.Reset();
            text = tooltip = string.Empty;
        }

        public override void OnGUI()
        {
            if (!active) return;
            BeginGUI();
            GUILayout.BeginHorizontal(new GUIContent(text, image, tooltip), imageStyle, CalcGUILayOutOptions());
            ChildrenOnGUI();
            GUILayout.EndHorizontal();
            EndGUI();
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);
            SerializeField(root, "text", text);
            SerializeField(root, "tooltip", tooltip);
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            DeSerializeField(root, "text", ref text);
            DeSerializeField(root, "tooltip", ref tooltip);
        }
    }
    [GUICreateMenuPath(path = "Vertical/ImageVertical")]
    public class ImageVertical : HaveChildImageElement
    {
        public override GUIStyle imageStyle
        {
            get
            {
                if (m_style == null)
                    m_style = new GUIStyle(GUI.skin.label);
                return m_style;
            }
            set { m_style = new GUIStyle(value); }
        }
        public ImageVertical() : base() { }
        public ImageVertical(ImageVertical other) : base(other) { }

        public override void OnGUI()
        {
            if (!active) return;

            BeginGUI();
            GUILayout.BeginVertical(image, imageStyle, CalcGUILayOutOptions());
            ChildrenOnGUI();
            GUILayout.EndVertical();
            EndGUI();
        }
    }
    [GUICreateMenuPath(path = "Vertical/Vertical")]
    public class Vertical : HaveChildImageElement
    {
        public string text;
        public string tooltip;

        public Vertical() : base() { }
        public Vertical(Vertical other) : base(other)
        {
            text = other.text;
            tooltip = other.tooltip;
        }
        public override void Reset()
        {
            base.Reset();
            text = tooltip = string.Empty;
        }

        public override void OnGUI()
        {
            if (!active) return;

            BeginGUI();
            GUILayout.BeginVertical(new GUIContent(text, image, tooltip), imageStyle, CalcGUILayOutOptions());
            ChildrenOnGUI();
            GUILayout.EndVertical();
            EndGUI();
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);
            SerializeField(root, "text", text);
            SerializeField(root, "tooltip", tooltip);
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            DeSerializeField(root, "text", ref text);
            DeSerializeField(root, "tooltip", ref tooltip);
        }
    }
    [GUICreateMenuPath(path = "Area/ImageArea")]
    public class ImageArea : HaveChildImageElement
    {
        public override GUIStyle imageStyle
        {
            get
            {
                if (m_style == null)
                    m_style = new GUIStyle(GUI.skin.label);
                return m_style;
            }
            set { m_style = new GUIStyle(value); }
        }
        public Rect areaRect { get { return position; } set { position = value; } }

        public ImageArea() : base() { }
        public ImageArea(ImageArea other) : base(other) { areaRect = other.areaRect; }
        public override void Reset()
        {
            base.Reset();
            areaRect = new Rect(0, 0, 100, 100);
        }

        public override void OnGUI()
        {
            if (!active) return;

            BeginGUI();
            GUILayout.BeginArea(areaRect, image, imageStyle);
            ChildrenOnGUI();
            GUILayout.EndArea();
            EndGUI();
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);
            SerializeField(root, "areaRect", areaRect);
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            Rect _areaRect = Rect.zero;
            DeSerializeField(root, "areaRect", ref _areaRect);
            areaRect = _areaRect;
        }
    }
    [GUICreateMenuPath(path = "Area/Area")]
    public class Area : ImageArea
    {
        public string text;
        public string tooltip;

        public Area() : base() { }
        public Area(Area other) : base(other)
        {
            text = other.text;
            tooltip = other.tooltip;
        }
        public override void Reset()
        {
            base.Reset();
            text = tooltip = string.Empty;
        }

        public override void OnGUI()
        {
            if (!active) return;

            BeginGUI();
            GUILayout.BeginArea(areaRect, new GUIContent(text, image, tooltip), imageStyle);
            ChildrenOnGUI();
            GUILayout.EndArea();
            EndGUI();
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);
            SerializeField(root, "text", text);
            SerializeField(root, "tooltip", tooltip);
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            DeSerializeField(root, "text", ref text);
            DeSerializeField(root, "tooltip", ref tooltip);
        }
    }
    [GUICreateMenuPath(path = "Box")]
    public class Box : ImageBox
    {
        public string text;
        public string tooltip;

        public Box() : base() { }
        public Box(Box other) : base(other)
        {
            text = other.text;
            tooltip = other.tooltip;
        }
        public override void Reset()
        {
            base.Reset();
            text = string.Empty;
            tooltip = string.Empty;
        }

        public override void OnGUI()
        {
            BeginGUI();
            GUILayout.Box(new GUIContent(text, image, tooltip), imageStyle, CalcGUILayOutOptions());
            position = GUILayoutUtility.GetLastRect();
            EndGUI();
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);
            SerializeField(root, "text", text);
            SerializeField(root, "tooltip", tooltip);
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            DeSerializeField(root, "text", ref text);
            DeSerializeField(root, "tooltip", ref tooltip);
        }
    }
    [GUICreateMenuPath(path = "Button")]
    public class Button : TextElement
    {
        public Texture2D image;
        public ScaleMode mode;
        public bool alphaBlend;
        public float imageAspect;
        public Vector4 borderWidths;
        public float borderRadius;
        private GUIStyle m_btnStyle;
        public GUIStyle btnStyle
        {
            get
            {
                if (m_btnStyle == null)
                    m_btnStyle = new GUIStyle(GUI.skin.button);
                return m_btnStyle;
            }
            set { m_btnStyle = new GUIStyle(value); }
        }
        public Action OnClick;

        public Button() : base() { }
        public Button(Button other) : base(other)
        {
            image = other.image;
            mode = other.mode;
            alphaBlend = other.alphaBlend;
            imageAspect = other.imageAspect;
            borderWidths = other.borderWidths;
            borderRadius = other.borderRadius;
            m_btnStyle = new GUIStyle(other.m_btnStyle);
        }
        public override void Reset()
        {
            base.Reset();
            alignment = TextAnchor.MiddleCenter;
            mode = ScaleMode.StretchToFill;
            imageAspect = 2;
            alphaBlend = true;
            borderRadius = 2;
            borderWidths = Vector4.zero;
            image = null;
        }

        public override void OnGUI()
        {
            base.OnGUI();
            if (!active) return;
            GUILayout.Label("", CalcGUILayOutOptions());
            position = GUILayoutUtility.GetLastRect();
            BeginGUI();
            if (GUI.Button(position, "", btnStyle))
            {
                if (OnClick != null)
                {
                    OnClick();
                }
            }
            if (image != null)
                GUI.DrawTexture(position, image, mode, alphaBlend, imageAspect, color, borderWidths, borderRadius);
            GUI.Label(position, new GUIContent(text, tooltip), textStyle);
            EndGUI();
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);

            SerializeField(root, "mode", mode);
            SerializeField(root, "alphaBlend", alphaBlend);
            SerializeField(root, "imageAspect", imageAspect);
            SerializeField(root, "borderWidths", borderWidths);
            SerializeField(root, "borderRadius", borderRadius);
            if (image != null)
                SerializeField(root, "image", image.CreateReadableTexture().EncodeToPNG());
            root.AppendChild(new GUIStyleSerializer(btnStyle, "ButtonStyle").Serializate(doc));
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            DeSerializeField(root, "mode", ref mode);
            DeSerializeField(root, "alphaBlend", ref alphaBlend);
            DeSerializeField(root, "imageAspect", ref imageAspect);
            DeSerializeField(root, "borderWidths", ref borderWidths);
            DeSerializeField(root, "borderRadius", ref borderRadius);
            if (root.SelectSingleNode("image") != null)
            {
                byte[] bs = new byte[0];
                DeSerializeField(root, "image", ref bs);
                image = new Texture2D(200, 200);
                image.LoadImage(bs);
                image.hideFlags = HideFlags.DontSaveInEditor;
            }
            var list = root.SelectNodes("GUIStyle");
            btnStyle = new GUIStyle();

            new GUIStyleSerializer(btnStyle, "ButtonStyle").DeSerializate(list[1] as XmlElement);
        }
    }
    [GUICreateMenuPath(path = "Space")]
    public class Space : Element
    {
        public float pixels;

        public Space() : base() { }
        public Space(Space other) : base(other)
        {
            pixels = other.pixels;
        }
        public override void Reset()
        {
            base.Reset();
            pixels = 0;
        }
        public override void OnGUI()
        {
            if (!active) return;
            GUILayout.Space(pixels);
            position = GUILayoutUtility.GetLastRect();
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);
            SerializeField(root, "pixels", pixels);
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            DeSerializeField(root, "pixels", ref pixels);
        }
    }
    [GUICreateMenuPath(path = "FlexibleSpace")]
    public class FlexibleSpace : Element
    {
        public FlexibleSpace() : base() { }
        public FlexibleSpace(FlexibleSpace other) : base(other) { }

        public override void OnGUI()
        {
            if (!active) return;

            GUILayout.FlexibleSpace();
            position = GUILayoutUtility.GetLastRect();
        }
    }
    [GUICreateMenuPath(path = "ScrollView")]
    public class ScrollView : HaveChildElement
    {
        public bool alwaysShowHorizontal;
        public bool alwaysShowVertical;
        public Vector2 value;
        public Action<Vector2> onValueChange { get; set; }
        private GUIStyle m_Hstyle;
        private GUIStyle m_Vstyle;

        public GUIStyle Hstyle
        {
            get
            {
                if (m_Hstyle == null)
                    m_Hstyle = new GUIStyle(GUI.skin.horizontalScrollbar);
                return m_Hstyle;
            }
            set { m_Hstyle = new GUIStyle(value); }
        }
        public GUIStyle Vstyle
        {
            get
            {
                if (m_Vstyle == null)
                    m_Vstyle = new GUIStyle(GUI.skin.verticalScrollbar);
                return m_Vstyle;
            }
            set { m_Vstyle = new GUIStyle(value); }
        }
        private GUIStyleSerializer HstyleDrawer;
        private GUIStyleSerializer VstyleDrawer;

        public ScrollView() : base() { }
        public ScrollView(ScrollView other) : base(other)
        {
            alwaysShowHorizontal = other.alwaysShowHorizontal;
            alwaysShowVertical = other.alwaysShowVertical;
            value = other.value;
            m_Hstyle = new GUIStyle(other.m_Hstyle);
            m_Vstyle = new GUIStyle(other.m_Vstyle);
        }
        public override void Reset()
        {
            base.Reset();
            value = Vector2.zero;
            alwaysShowHorizontal = alwaysShowVertical = false;
        }

        public override void OnGUI()
        {
            if (!active) return;

            BeginGUI();
            Vector2 tmp = GUILayout.BeginScrollView(value, alwaysShowHorizontal, alwaysShowVertical, Hstyle, Vstyle, CalcGUILayOutOptions());
            ChildrenOnGUI();
            GUILayout.EndScrollView();
            if (tmp != value)
            {
                value = tmp;
                if (onValueChange != null)
                    onValueChange(value);
            }
            EndGUI();
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);

            SerializeField(root, "alwaysShowHorizontal", alwaysShowHorizontal);
            SerializeField(root, "alwaysShowVertical", alwaysShowVertical);
            SerializeField(root, "value", value);
            XmlElement stylesE = doc.CreateElement("Styles");
            stylesE.AppendChild(new GUIStyleSerializer(Hstyle, "Hrizontal Style").Serializate(doc));
            stylesE.AppendChild(new GUIStyleSerializer(Vstyle, "Vertical Style").Serializate(doc));
            root.AppendChild(stylesE);
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            DeSerializeField(root, "alwaysShowHorizontal", ref alwaysShowHorizontal);
            DeSerializeField(root, "alwaysShowVertical", ref alwaysShowVertical);
            DeSerializeField(root, "value", ref value);
            m_Hstyle = new GUIStyle();
            m_Vstyle = new GUIStyle();


            XmlElement styleE = root.SelectSingleNode("Styles") as XmlElement;
            new GUIStyleSerializer(Hstyle, "Hrizontal Style").DeSerializate(styleE.FirstChild as XmlElement);
            new GUIStyleSerializer(Vstyle, "Vertical Style").DeSerializate(styleE.LastChild as XmlElement);
        }

    }

    [GUICreateMenuPath(path = "Delegate")]
    public class DelegateElement : Element
    {
        public Action<GUILayoutOption[]> DrawGUI;

        public DelegateElement() : base() { }
        public DelegateElement(DelegateElement other) : base(other) { }

        public override void OnGUI()
        {
            if (!active) return;
            BeginGUI();
            if (DrawGUI != null) DrawGUI(CalcGUILayOutOptions());
            position = GUILayoutUtility.GetLastRect();
            EndGUI();
        }
    }

    public class GUICanvas : Area
    {
        public Rect CanvasRect { get { return areaRect; } set { areaRect = value; } }
        public GUICanvas(GUICanvas other) : base(other) { }
        public GUICanvas() : base() { }

        public override void OnGUI()
        {
            if (!active) return;
            BeginGUI();
            GUI.Box(position, "");
            GUILayout.BeginArea(areaRect, image, imageStyle);
            ChildrenOnGUI();
            GUILayout.EndArea();
            EndGUI();
        }

        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            ElementSelection.element = null;
        }
    }
}

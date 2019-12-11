/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace IFramework.GUITool.RectDesign
{
    public interface IGUIElement
    {
        string name { get; set; }
        bool active { get; set; }
        bool enable { get; set; }

        int depth { get; }
        Rect position { get; }
        int siblingIndex { get; }
        string treePath { get; }
        IGUIElement parent { get; }
        IGUIElement root { get; }
        List<IGUIElement> Children { get; }

        void Reset();
        void Destoty();
        IGUIElement Find(string path);
        T Find<T>(string path) where T : IGUIElement;
        XmlElement Serialize(XmlDocument doc);
        void DeSerialize(XmlElement element);
    }
    public abstract class GUIElement : IGUIElement
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

        public virtual Rect position
        {
            get
            {
                if (parent == null)
                    return Rect.zero;
                if (parent is ScrollView)
                    return calculator.Calc(new Rect(Vector2.zero, parent.position.size));
                return calculator.Calc(parent.position);
            }
        }
        public int depth
        {
            get
            {
                if (parent == null) return 0;
                return parent.depth + 1;
            }
        }
        private IGUIElement m_parent;
        public IGUIElement root
        {
            get
            {
                IGUIElement tmp = this;
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
                IGUIElement ele = this;
                while (ele.parent != null)
                {
                    ele = ele.parent;
                    tmp = ele.name + "/" + tmp;
                }
                return tmp;
            }
        }
        public IGUIElement parent
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
            set
            {
                GUIElement ele = this.parent as GUIElement;
                ele.Children.Remove(this);
                ele.Children.Insert(value, this);
                GUICanvas canvas = ele.root as GUICanvas;
                if (canvas != null)
                    canvas.TreeChange();
            }
        }

        public List<IGUIElement> Children { get { return children; } }
        protected readonly List<IGUIElement> children = new List<IGUIElement>();
        public RectCalculator calculator;

        protected GUIElement() { calculator = new RectCalculator(); Reset(); }
        protected GUIElement(GUIElement other)
        {
            m_name = other.m_name;
            m_enable = other.m_enable;
            m_active = other.m_active;
            rotateAngle = other.rotateAngle;
            rotateOffset = other.rotateOffset;
            color = other.color;
            contentColor = other.contentColor;
            backgroundColor = other.backgroundColor;
            this.calculator = new RectCalculator(other.calculator);
            for (int i = 0; i < other.children.Count; i++)
            {
                GUIElement element = other.children[i] as GUIElement;
                GUIElement copy = Activator.CreateInstance(element.GetType(), other.children[i]) as GUIElement;
                copy.parent = this;
            }
        }
        public virtual void Reset()
        {
            calculator.Reset();
            m_name = GetType().Name;
            m_enable = m_active = true;
            color = contentColor = backgroundColor = Color.white;
            rotateAngle = 0;
            rotateOffset = Vector2.zero;


        }
        public void Destoty()
        {
            GUICanvas canvas = root as GUICanvas;
            parent = null;
            if (canvas != null)
                canvas.TreeChange();
            OnDestory();
        }
        protected virtual void OnDestory()
        {
        }
        public IGUIElement Find(string path)
        {
            if (children.Count == 0) return null;
            if (!path.Contains("/"))
            {
                for (int i = 0; i < children.Count; i++)
                {
                    IGUIElement tmp = children[i] as IGUIElement;
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
                    IGUIElement tmpE = children[i] as IGUIElement;
                    if (tmpE.name == tmp)
                        return tmpE.Find(path);
                }
                return null;
            }

        }
        public T Find<T>(string path) where T : IGUIElement
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

            bool bo = this.enable;
            if (bo)
            {
                IGUIElement d = this;
                while (d.parent != null)
                {
                    d = d.parent;
                    bo = d.enable && bo;
                    if (!bo) break;
                }
            }
            GUI.enabled = bo;
        }

        protected void EndGUI()
        {
            GUI.backgroundColor = preBgColor;
            GUI.contentColor = preContentColor;
            GUI.skin = preSkin;
            GUI.matrix = preMat4x4;
            GUI.color = preColor;
            GUI.enabled = preEnable;
        }

        public abstract void OnGUI(Action child);


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
            root.SetAttribute("ElementType", GetType().Name);
            XmlElement ele = root.OwnerDocument.CreateElement("children");
            for (int i = 0; i < children.Count; i++)
                ele.AppendChild(children[i].Serialize(doc));
            root.PrependChild(ele);
            root.AppendChild(calculator.Serialize(doc));
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
            calculator.DeSerialize(root.SelectSingleNode("RectCalculator") as XmlElement);
            children.Clear();
            XmlElement ele = root.SelectSingleNode("children") as XmlElement;
            for (int i = 0; i < ele.ChildNodes.Count; i++)
            {
                XmlElement child = ele.ChildNodes[i] as XmlElement;
                Type type = typeof(GUIElement).GetSubTypesInAssemblys().ToList().Find((tmp) =>
                {
                    return tmp.Name == child.GetAttribute("ElementType");
                });
                GUIElement element = Activator.CreateInstance(type, null) as GUIElement;
                element.DeSerialize(child);
                element.parent = this;
            }
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
}

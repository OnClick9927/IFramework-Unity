/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Xml;
using UnityEngine;

namespace IFramework.GUITool.RectDesign
{
    public abstract class TextElement : GUIElement
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

        public override void OnGUI(Action child)
        {
            if (active)
            {
                textStyle.font = font;
                textStyle.fontStyle = fontStyle;
                textStyle.fontSize = fontSize;
                textStyle.alignment = alignment;
                textStyle.clipping = overflow;
                textStyle.richText = richText;
                BeginGUI();
                DrawGUI();
                if (child != null) child();
                EndGUI();
            }
        }
        protected abstract void DrawGUI();
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

}

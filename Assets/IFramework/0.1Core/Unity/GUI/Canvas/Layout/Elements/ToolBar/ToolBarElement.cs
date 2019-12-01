/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Xml;
using UnityEngine;

namespace IFramework.GUITool.LayoutDesign
{
    public abstract class ToolBarElement : GUIElement
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

        public override void OnGUI(Action child)
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
}

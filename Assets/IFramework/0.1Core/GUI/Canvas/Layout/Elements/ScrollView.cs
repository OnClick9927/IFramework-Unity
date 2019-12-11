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
    [GUIElement("ScrollView")]
    public class ScrollView : ParentGUIElement
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

        public override void OnGUI(Action child)
        {
            if (!active) return;

            BeginGUI();
            Vector2 tmp = GUILayout.BeginScrollView(value, alwaysShowHorizontal, alwaysShowVertical, Hstyle, Vstyle, CalcGUILayOutOptions());
            if (child != null) child();
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
}

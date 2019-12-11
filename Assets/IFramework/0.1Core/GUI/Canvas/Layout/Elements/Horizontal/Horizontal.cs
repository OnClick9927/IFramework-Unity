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
    [GUIElement("Horizontal/Horizontal")]
    public class Horizontal : ParentImageElement
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

        public override void OnGUI(Action child)
        {
            if (!active) return;
            BeginGUI();
            GUILayout.BeginHorizontal(new GUIContent(text, image, tooltip), imageStyle, CalcGUILayOutOptions());
            if (child != null) child();

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
}

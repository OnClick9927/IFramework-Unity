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
    [GUIElement("Box")]
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
        public override void OnGUI(Action child)
        {
            base.OnGUI(child);
            if (active)
            {
                BeginGUI();
                GUI.Box(position, new GUIContent(text, image, tooltip), imageStyle);
                if (child != null) child();

                EndGUI();
            }
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

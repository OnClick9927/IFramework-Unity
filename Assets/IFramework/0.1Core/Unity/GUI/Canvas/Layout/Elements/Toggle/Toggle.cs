/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Xml;
using UnityEngine;

namespace IFramework.GUITool.LayoutDesign
{
    [GUIElement("Toggle/Toggle")]
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
}

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
    [GUIElement("Toggle/ImageToggle")]
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

        public override void OnGUI(Action child)
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
}

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
    public abstract class SliderElement : GUIElement
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

        public override void OnGUI(Action child)
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

}

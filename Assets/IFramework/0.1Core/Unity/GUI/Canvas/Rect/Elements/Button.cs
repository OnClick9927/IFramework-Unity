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
    [GUIElement("Button")]
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
        public override void OnGUI(Action child)
        {
            base.OnGUI(child);
            if (active)
            {
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
                if (child != null) child();

                EndGUI();
            }
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

        protected override void DrawGUI()
        {

        }
    }
}

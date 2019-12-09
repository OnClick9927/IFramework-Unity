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
    public abstract class ParentImageElement : ParentGUIElement
    {
        public Texture2D image;
        protected GUIStyle m_style;
        public virtual GUIStyle imageStyle
        {
            get
            {
                if (m_style == null)
                    m_style = new GUIStyle(GUI.skin.label);
                return m_style;
            }
            set { m_style = new GUIStyle(value); }
        }

        protected ParentImageElement() : base() { }
        protected ParentImageElement(ParentImageElement other) : base(other)
        {
            image = other.image;
            m_style = new GUIStyle(other.m_style);
        }
        public override void Reset()
        {
            base.Reset();
            image = null;
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);
            if (image != null)
                SerializeField(root, "image", image.CreateReadableTexture().EncodeToPNG());
            root.AppendChild(new GUIStyleSerializer(imageStyle, "Image Style").Serializate(doc));
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            if (root.SelectSingleNode("image") != null)
            {
                byte[] bs = new byte[0];
                DeSerializeField(root, "image", ref bs);
                image = new Texture2D(200, 200);
                image.LoadImage(bs);
                image.hideFlags = HideFlags.DontSaveInEditor;
            }
            m_style = new GUIStyle();

            XmlElement styleE = root.SelectSingleNode("GUIStyle") as XmlElement;
            new GUIStyleSerializer(imageStyle, "Image Style").DeSerializate(styleE);
        }
    }

}

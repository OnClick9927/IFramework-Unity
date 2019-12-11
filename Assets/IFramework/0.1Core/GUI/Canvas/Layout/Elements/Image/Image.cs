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
    [GUIElement("Image/Image")]
    public class Image : GUIElement
    {
        public Texture2D image;
        public ScaleMode mode;
        public bool alphaBlend;
        public float imageAspect;
        public Vector4 borderWidths;
        public float borderRadius;

        public Image() : base() { }
        public Image(Image other) : base(other)
        {
            image = other.image;
            mode = other.mode;
            alphaBlend = other.alphaBlend;
            imageAspect = other.imageAspect;
            borderWidths = other.borderWidths;
            borderRadius = other.borderRadius;
        }
        public override void Reset()
        {
            base.Reset();
            mode = ScaleMode.StretchToFill;
            imageAspect = 2;
            alphaBlend = true;
            borderRadius = 2;
            borderWidths = Vector4.zero;
            image = null;
        }

        public override void OnGUI(Action child)
        {
            if (!active) return;
            GUILayout.Label("", CalcGUILayOutOptions());
            position = GUILayoutUtility.GetLastRect();
            BeginGUI();
            if (image != null)
                GUI.DrawTexture(position, image, mode, alphaBlend, imageAspect, color, borderWidths, borderRadius);
            EndGUI();
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
        }
    }
}

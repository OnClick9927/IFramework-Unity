/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Serialization;
using System.Xml;
using UnityEngine;

namespace IFramework.GUITool.LayoutDesign
{
    [GUIElement("TextToolBar/ImageToolBar")]
    public class ImageToolBar : ToolBarElement
    {
        public Texture2D[] texs;

        public ImageToolBar() : base() { }
        public ImageToolBar(ImageToolBar other) : base(other)
        {
            texs = new Texture2D[other.texs.Length];
            for (int i = 0; i < texs.Length; i++)
            {
                texs[i] = other.texs[i];
            }
        }
        public override void Reset()
        {
            base.Reset();
            texs = new Texture2D[0];
        }

        protected override int DrawGUI(Rect position)
        {
            return GUI.Toolbar(position, value, texs, style);
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);
            XmlElement texsE = doc.CreateElement("texs");
            for (int i = 0; i < texs.Length; i++)
                SerializeField(texsE, "image", texs[i].CreateReadableTexture().EncodeToPNG());
            root.AppendChild(texsE);
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            XmlElement texsE = root.SelectSingleNode("texs") as XmlElement;
            texs = new Texture2D[texsE.ChildNodes.Count];
            for (int i = 0; i < texs.Length; i++)
            {
                byte[] bs = new byte[0];
                texs[i] = new Texture2D(100, 100);
                StringConvert.TryConvert(texsE.ChildNodes[i].InnerText, out bs);
                texs[i].LoadImage(bs);
                texs[i].hideFlags = HideFlags.DontSaveInEditor;

            }
        }
    }
}

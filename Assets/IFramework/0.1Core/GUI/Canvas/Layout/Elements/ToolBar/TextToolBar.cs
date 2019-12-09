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
    [GUIElement("TextToolBar/ToolBar")]
    public class TextToolBar : ToolBarElement
    {
        public string[] texs;

        public TextToolBar() : base() { }
        public TextToolBar(TextToolBar other) : base(other)
        {
            texs = new string[other.texs.Length];
            for (int i = 0; i < texs.Length; i++)
            {
                texs[i] = other.texs[i];
            }
        }
        public override void Reset()
        {
            base.Reset();
            texs = new string[0];
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
                SerializeField(texsE, "value", texs[i]);
            root.AppendChild(texsE);
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            XmlElement texsE = root.SelectSingleNode("texs") as XmlElement;
            texs = new string[texsE.ChildNodes.Count];
            for (int i = 0; i < texs.Length; i++)
                texs[i] = texsE.ChildNodes[i].InnerText;
        }
    }
}

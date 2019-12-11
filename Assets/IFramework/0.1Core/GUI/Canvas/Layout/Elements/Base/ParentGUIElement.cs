/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Linq;
using System.Xml;

namespace IFramework.GUITool.LayoutDesign
{
    public abstract class ParentGUIElement : GUIElement
    {
        protected ParentGUIElement() : base() { }
        protected ParentGUIElement(ParentGUIElement other) : base(other)
        {
            for (int i = 0; i < other.children.Count; i++)
            {
                GUIElement element = other.children[i] as GUIElement;
                GUIElement copy = Activator.CreateInstance(element.GetType(), other.children[i]) as GUIElement;
                copy.parent = this;
            }
        }

        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = base.Serialize(doc);
            XmlElement ele = root.OwnerDocument.CreateElement("children");
            for (int i = 0; i < children.Count; i++)
                ele.AppendChild(children[i].Serialize(doc));
            root.PrependChild(ele);
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            children.Clear();
            XmlElement ele = root.SelectSingleNode("children") as XmlElement;
            for (int i = 0; i < ele.ChildNodes.Count; i++)
            {
                XmlElement child = ele.ChildNodes[i] as XmlElement;
                Type type = typeof(GUIElement).GetSubTypesInAssemblys().ToList().Find((tmp) =>
                {
                    return tmp.Name == child.GetAttribute("ElementType");
                });
                if (type==null)
                    throw new Exception(" Type Not Found " + child.GetAttribute("ElementType"));

                GUIElement element = Activator.CreateInstance(type, null) as GUIElement;
                element.DeSerialize(child);
                element.parent = this;
            }
        }
    }

}

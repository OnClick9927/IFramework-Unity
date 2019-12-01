/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Linq;
using System.Xml;

namespace IFramework.GUITool.RectDesign
{
    public static class GUIElementExtension
    {
        public static T Element<T>(this T t, GUIElement element) where T : GUIElement
        {
            element.parent = t;
            GUICanvas canvas = element.root as GUICanvas;
            if (canvas != null)
                canvas.TreeChange();
            return t;
        }
        public static void SaveXmlPrefab(this GUIElement e, string path)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement ele = doc.CreateElement("Element_Prefab");
            ele.SetAttribute("Type", e.GetType().Name);
            ele.AppendChild(e.Serialize(doc));
            doc.AppendChild(ele);
            doc.Save(path);
        }
        public static void LoadXmlPrefab(this GUIElement e, string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            if (doc.DocumentElement.Name != "Element_Prefab") return;
            string attr = doc.DocumentElement.GetAttribute("Type");
            Type type = typeof(GUIElement).GetSubTypesInAssemblys().ToList().Find((t) => { return t.Name == attr; });
            GUIElement element = Activator.CreateInstance(type) as GUIElement;
            element.DeSerialize(doc.FirstChild.FirstChild as XmlElement);
            e.Element(element);
        }
    }

}

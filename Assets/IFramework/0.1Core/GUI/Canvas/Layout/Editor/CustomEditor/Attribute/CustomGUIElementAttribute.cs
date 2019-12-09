/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace IFramework.GUITool.LayoutDesign
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CustomGUIElementAttribute : Attribute
    {
        public Type EditType { get; set; }
        public CustomGUIElementAttribute(Type designTYpe)
        {
            this.EditType = designTYpe;
        }
    }
}

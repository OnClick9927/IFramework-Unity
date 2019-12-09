/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-07
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace IFramework.GUITool.RectDesign
{
    class GUIElementSceneView : ILayoutGUIDrawer
    {
        private Dictionary<Type, GUIElementEditor> dic = new Dictionary<Type, GUIElementEditor>();
        public GUICanvas canvas;
        public GUIElementSceneView()
        {
            var designs = typeof(GUIElementEditor).GetSubTypesInAssemblys().ToList().FindAll((t) => { return t.IsDefined(typeof(CustomGUIElementAttribute), false); });
            var eles = typeof(GUIElement).GetSubTypesInAssemblys();
            foreach (var type in eles)
            {
                var typeTree = type.GetTypeTree();
                for (int i = 0; i < typeTree.Count; i++)
                {
                    Type des = designs.Find((t) => {
                        return (t.GetCustomAttributes(typeof(CustomGUIElementAttribute), false).First() as CustomGUIElementAttribute).EditType == typeTree[i];
                    });
                    if (des != null)
                    {
                        dic.Add(type, Activator.CreateInstance(des) as GUIElementEditor);
                        break;

                    }
                }
            }
        }
        public void OnGUI(Rect rect)
        {

            EleGUI(canvas);
        }
        private void EleGUI(GUIElement ele)
        {
            GUIElementEditor des = dic[ele.GetType()];
            des.element = ele;
            des.OnSceneGUI(() => {
                for (int i = 0; i < ele.Children.Count; i++)
                {
                    EleGUI(ele.Children[i] as GUIElement);
                }
            });

        }

    }

}

/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.14f1
 *Date:           2019-11-04
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Xml;
using UnityEngine;

namespace IFramework.GUITool.RectDesign
{
    public class GUICanvas : GUIElement
    {
        public event Action OnCanvasTreeChange;
        public void TreeChange()
        {
            if (OnCanvasTreeChange != null)
                OnCanvasTreeChange();
        }
        public Rect canvasRect;
        public override Rect position { get { return new Rect(Vector2.zero, canvasRect.size); } }
        public override void OnGUI(Action child)
        {
            if (active)
            {
                BeginGUI();
                GUI.BeginClip(canvasRect);
                if (child != null) child();

                GUI.EndClip();
                EndGUI();
            }
        }

        public void OnGUI()
        {
            Draw(this);

        }
        private void Draw(GUIElement ele)
        {
            ele.OnGUI(() =>
            {
                for (int i = 0; i < ele.Children.Count; i++)
                {
                    Draw(ele.Children[i] as GUIElement);
                }
            });

        }
        public override XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root= base.Serialize(doc);
            SerializeField(root, "canvasRect", canvasRect);
            return root;
        }
        public override void DeSerialize(XmlElement root)
        {
            base.DeSerialize(root);
            DeSerializeField(root, "canvasRect", ref canvasRect);
        }
    }
}

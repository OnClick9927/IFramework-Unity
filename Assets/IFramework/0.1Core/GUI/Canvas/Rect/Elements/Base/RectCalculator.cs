/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.Serialization;
using System;
using System.Xml;
using UnityEngine;

namespace IFramework.GUITool.RectDesign
{
    public class RectCalculator
    {
        public RectCalculator(RectCalculator other)
        {
            this.m_anchorMax = other.m_anchorMax;
            this.anchorMin = other.anchorMin;
            this.pivot = other.pivot;
            this.size = other.size;
            this.pos = other.pos;
            this.left = other.left;
            this.right = other.right;
            this.top = other.top;
            this.bottom = other.bottom;
        }
        public RectCalculator(){Reset();}
        public void Reset()
        {
            pivot = m_anchorMin = m_anchorMax = new Vector2(0.5f, 0.5f);
            size = new Vector2(100, 100);
        }
        private Vector2 m_anchorMin;
        private Vector2 m_anchorMax;
        public Vector2 anchorMin { get { return m_anchorMin; } set { m_anchorMin.Set(value.x < 0 ? 0 : value.x > 1 ? 1 : value.x, value.y < 0 ? 0 : value.y > 1 ? 1 : value.y); } }
        public Vector2 anchorMax { get { return m_anchorMax; } set { m_anchorMax.Set(value.x < 0 ? 0 : value.x > 1 ? 1 : value.x, value.y < 0 ? 0 : value.y > 1 ? 1 : value.y); } }
        public Vector2 pivot;
        public Vector2 size;
        public Vector2 pos;
        public float height { get { return size.y; } set { size.y = value; } }
        public float width { get { return size.x; } set { size.x = value; } }

        public float left, right;
        public float top, bottom;
        private Rect m_rect;
        private Rect m_anchorRect;
        private Vector2 m_pivotPos;
        public Vector2 pivotPos { get { return m_pivotPos; } }

        public Rect Rect { get { return m_rect; } }
        private Rect anchorRect { get { return m_anchorRect; } }


        public Rect Calc(Rect pRect)
        {
            if (m_anchorMin == m_anchorMax)
            {
                Vector2 anchorPos = new Vector2(
                    pRect.xMin + pRect.width * m_anchorMin.x,
                    pRect.yMin + pRect.height * m_anchorMin.y
                    );
                Vector2 pivotPos = anchorPos + pos;
                float x = pivotPos.x - (1 - pivot.x) * size.x;
                float y = pivotPos.y - (1 - pivot.y) * size.y;
                m_rect = new Rect(new Vector2(x, y), size);
                m_anchorRect = new Rect(anchorPos, Vector2.zero);
            }
            else if (m_anchorMin.x == m_anchorMax.x && m_anchorMin.y != m_anchorMax.y)
            {
                float yMin = pRect.yMin + pRect.height * anchorMin.y;
                float yMax = pRect.yMax - pRect.height * (1 - anchorMax.y);
                m_rect.yMin = yMin + top;
                m_rect.yMax = yMax - bottom;
                m_rect.width = width;
                float anchprX = pRect.xMin + pRect.width * m_anchorMin.x;
                float x = anchprX - (1 - pivot.x) * width + pos.x;
                m_rect.x = x;
                m_anchorRect = new Rect(anchprX, yMin, 0, yMax - yMin);
            }
            else if (m_anchorMin.x != m_anchorMax.x && m_anchorMin.y == m_anchorMax.y)
            {
                float xMin = pRect.xMin + pRect.width * anchorMin.x;
                float xMax = pRect.xMax - pRect.width * (1 - anchorMax.x);

                m_rect.xMin = xMin + left;
                m_rect.xMax = xMax - right;
                m_rect.height = height;
                float anchprY = pRect.yMin + pRect.height * m_anchorMin.y;
                float y = anchprY - (1 - pivot.y) * height + pos.y;

                m_rect.y = y;
                m_anchorRect = new Rect(xMin, anchprY, xMax - xMin, 0);
            }
            else
            {
                m_anchorRect.xMin = pRect.xMin + anchorMin.x * pRect.width;
                m_anchorRect.xMax = pRect.xMax - (1 - anchorMax.x) * pRect.width;
                m_anchorRect.yMin = pRect.yMin + anchorMin.y * pRect.height;
                m_anchorRect.yMax = pRect.yMax - (1 - anchorMax.y) * pRect.height;

                m_rect.xMin = m_anchorRect.xMin + left;
                m_rect.xMax = m_anchorRect.xMax - right;
                m_rect.yMin = m_anchorRect.yMin + top;
                m_rect.yMax = m_anchorRect.yMax - bottom;
            }
            m_pivotPos = new Vector2(m_rect.xMin + m_rect.width * pivot.x, m_rect.yMin + m_rect.height * pivot.y);
            return m_rect;
        }
        public void OnGUI()
        {
            DrawPoint(m_pivotPos, "U2D.pivotDot");
            DrawPoint(m_anchorRect.TopLeft(), "U2D.dragDotActive");
            DrawPoint(m_anchorRect.TopRight(), "U2D.dragDotActive");
            DrawPoint(m_anchorRect.BottomLeft(), "U2D.dragDotActive");
            DrawPoint(m_anchorRect.BottomRight(), "U2D.dragDotActive");
        }
        private void DrawPoint(Vector2 pos, GUIStyle style)
        {
            Rect rect = new Rect();
            rect.center = pos;
            rect.yMin -= style.fixedHeight / 2;
            rect.xMin -= style.fixedWidth / 2;
            rect.size = new Vector2(style.fixedWidth, style.fixedHeight);
            GUI.Box(rect, "", style);
        }

        public virtual XmlElement Serialize(XmlDocument doc)
        {
            XmlElement root = doc.CreateElement("RectCalculator");
            SerializeField(root, "anchorMin", anchorMin);
            SerializeField(root, "anchorMax", anchorMax);
            SerializeField(root, "pivot", pivot);
            SerializeField(root, "size", size);
            SerializeField(root, "pos", pos);
            SerializeField(root, "left", left);
            SerializeField(root, "right", right);
            SerializeField(root, "top", top);
            SerializeField(root, "bottom", bottom);
            return root;
        }
        public virtual void DeSerialize(XmlElement root)
        {
            DeSerializeField(root, "anchorMin", ref m_anchorMin);
            DeSerializeField(root, "anchorMax", ref m_anchorMax);
            DeSerializeField(root, "pivot", ref pivot);
            DeSerializeField(root, "size", ref size);
            DeSerializeField(root, "pos", ref pos);
            DeSerializeField(root, "left", ref left);
            DeSerializeField(root, "right", ref right);
            DeSerializeField(root, "top", ref top);
            DeSerializeField(root, "bottom", ref bottom);
        }
        protected static XmlElement SerializeField<T>(XmlElement root, string name, T value)
        {
            try
            {
                XmlElement ele = root.OwnerDocument.CreateElement(name);
                if (value != null)
                {
                    Type type = value.GetType();
                    XmlNode node = root.OwnerDocument.CreateTextNode(StringConvert.ConvertToString(value, type));
                    ele.AppendChild(node);
                    root.AppendChild(ele);
                }
                else
                {
                    Log.W("GUICanvas:  " + name + "  value is mull,will not Serializate");
                }
                return root;
            }
            catch (Exception)
            {
                throw new Exception(name);
            }

        }
        protected static void DeSerializeField<T>(XmlElement root, string name, ref T obj)
        {
            try
            {
                T obj1;
                XmlNode node = root.SelectSingleNode(name);
                if (node != null)
                {
                    StringConvert.TryConvert(node.InnerText, out obj1);
                    obj = obj1;
                }
                else
                    Log.W("GUICanvas:   Not Find Field   " + name);
            }
            catch
            {
                throw new Exception(name);
            }
        }
    }

}

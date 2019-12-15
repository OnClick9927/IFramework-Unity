/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-15
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Xml;
using UnityEngine;
namespace IFramework.GUITool.LayoutDesign
{
    [ExecuteInEditMode]
    public class GUICanvasComponet : MonoBehaviour
    {
        public TextAsset textAsset;
        public GUICanvas guiCanvas = new GUICanvas();
        public void LoadCanvas(TextAsset textAsset)
        {
            this.textAsset = textAsset;
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(textAsset.text);
            //guiCanvas = new GUICanvas();
            guiCanvas.DeSerialize(doc.DocumentElement);
        }
        private void OnGUI()
        {
            if (guiCanvas != null)
            {
                guiCanvas.OnGUI();
            }
        }
    }
}

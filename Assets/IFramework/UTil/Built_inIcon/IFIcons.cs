/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-05
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using UnityEngine;
namespace IFramework
{
    public class IFIcons
    {
        public const string StoName = "IFEditorIconSto";
        private static IFEditorIconSto Sto
        {
            get
            {
                if (sto==null)
                sto= Resources.Load<IFEditorIconSto>(StoName);
                return sto;
            }
        }
        private static IFEditorIconSto sto;

        public static GUIContent GetGUIContent(string name)
        {
            if (Sto == null) throw new System.ArgumentNullException("Asset is Null");
            for (int i = 0; i < sto.Buildt_inIcon.Count; i++)
            {
                if (sto.Buildt_inIcon[i].iconName==name)
                {
                    return sto.Buildt_inIcon[i].content;
                }
            }
            return default(GUIContent);
        }
        public static Texture GetTexture(string name)
        {
            return GetGUIContent(name).image;
        }
        public static bool Exsit(string name)
        {
            if (Sto == null) throw new System.ArgumentNullException("Asset is Null");
            for (int i = 0; i < sto.Buildt_inIcon.Count; i++)
            {
                if (sto.Buildt_inIcon[i].iconName == name)
                {
                    return true;
                }
            }
            return false;
        }
        public static string GetName(Texture texture)
        {
            if (Sto == null) throw new System.ArgumentNullException("Asset is Null");
            for (int i = 0; i < sto.Buildt_inIcon.Count; i++)
            {
                if (sto.Buildt_inIcon[i].content.image == texture)
                {
                    return sto.Buildt_inIcon[i].iconName;
                }
            }
            return string .Empty;
        }
    }

}

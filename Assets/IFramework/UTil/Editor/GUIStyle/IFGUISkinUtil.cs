/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-05-16
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace IFramework
{
	public class IFGUISkinUtil
	{
        public const string AssetName= "IFGUISKin";
        private static List<string> styleNames = new List<string>();
        private static IFGUISKin skin;
        private static IFGUISKin Skin
        {
            get {
                CheckNull();
                return skin;
            }
        }
        private static void CheckNull()
        {
            if (skin == null)
            {
                styleNames.Clear();
                skin = Resources.Load<IFGUISKin>(AssetName);
                for (int i = 0; i < skin.Styles.Count; i++)
                {
                    styleNames.Add(skin.Styles[i].name);
                }
            }
        }
        public static bool Contains(string styleName) {

            CheckNull();
            return styleNames.Contains(styleName);
        }
        public static GUIStyle GetStyle(string styleName)
        {
            CheckNull();

            if (!Contains(styleName)) return default(GUIStyle);
            for (int i = 0; i < skin.Styles.Count; i++)
            {
                if (skin.Styles[i].name == styleName) return skin.Styles[i];
            }
            return default(GUIStyle);
        }
        public static List<string> StyleNames
        {
            get {
                CheckNull();

                return styleNames;
            }
        }
    }
}

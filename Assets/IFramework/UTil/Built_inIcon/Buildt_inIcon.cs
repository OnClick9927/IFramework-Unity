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
    public class Buildt_inIcon : ScriptableObject
    {
        public string iconName;
        public GUIContent content;
        public void OnEnable()
        {
            if (!string.IsNullOrEmpty(iconName))
            {
                this.name = iconName;
            }
        }
      

    }

}

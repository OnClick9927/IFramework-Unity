/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-12
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace IFramework
{
	public class ProgressBarAttribute : PropertyAttribute
    {
        public string Text;
        public float MinValue;
        public float MaxValue;
        public Color color=Color.white;
        public float MinbarHeight=40f;
        public bool ShowSlider = false;
	}
}

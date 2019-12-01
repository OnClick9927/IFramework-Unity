/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-10-15
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace IFramework
{
	public class IsShowAttribute : PropertyAttribute
    {
        public string checkProperty { get; private set; }
        public IsShowAttribute(string checkProperty)
        {
            this.checkProperty = checkProperty;
        }
    }
}

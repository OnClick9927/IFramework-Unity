/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-12
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace IFramework
{
	public class SearchableStringAttribute : PropertyAttribute {
        public SearchableStringAttribute(string searchArray) { this.searchArray = searchArray; }
        public string searchArray;
    }
}

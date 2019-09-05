/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-09-02
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;
namespace IFramework
{
	public class LanGroup:ScriptableObject
	{
       [ReadOnly] public List<LanPair> lanPairs = new List<LanPair>();
        [ReadOnly]public List<string> Keys = new List<string>();
	}
}

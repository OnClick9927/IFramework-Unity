/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-19
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace IFramework.UI
{
    [CreateAssetMenu]
    public class UILayerData : UnityEngine.ScriptableObject
    {
        public const string rayCast_layer = "RayCast";
        public List<string> GetLayerNames() { return layers; }
        public int LayerNameToIndex(string layer) => layers.IndexOf(layer);
        public string GetLayerName(int layer) => layers[layer];
        //public bool ignoreOrder = true;
        [UnityEngine.SerializeField]
        private List<string> layers = new List<string> {
            "Background" ,
            "Mid"        ,
            "Pop"        ,
            "Guide"      ,
            "Toast"        ,
            "Top"        ,
        };
    }
}

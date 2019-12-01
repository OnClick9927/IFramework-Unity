/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-09-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
using System;
using System.Xml.Serialization;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
namespace IFramework
{
    [Serializable]
	public class LanPair
	{
        [JsonProperty("Language")]
        [XmlAttribute("Language")]
        public SystemLanguage Lan;
        [JsonProperty("Key")]
        [XmlAttribute("Key")]
        public string key;
        [JsonProperty("Val")]
        [XmlAttribute("Val")]
        public string Value;
	}
}

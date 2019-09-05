/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-06
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;

namespace IFramework.AB
{
    [Serializable]
    public class ABManifestContent
    {
        public string assetBundleName;
        public string[] assetNames;
        public ABManifestContent() { }
        public ABManifestContent(string assetBundleName, string[] assetNames)
        {
            this.assetNames = assetNames;
            this.assetBundleName = assetBundleName;
        }
    }

}

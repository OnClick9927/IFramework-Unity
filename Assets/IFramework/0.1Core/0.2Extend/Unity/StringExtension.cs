/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-22
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/

using System.IO;
using UnityEngine;

namespace IFramework
{
    public static partial class StringExtension
	{
        public static string ToAbsPathWithoutExtension(this string self)
        {
            return Path.Combine(Path.GetDirectoryName(self), Path.GetFileNameWithoutExtension(self)).ToRegularPath();
        }
        public static string ToAbsPath(this string self)
        {
            string assetRootPath = Path.GetFullPath(Application.dataPath);
            assetRootPath= assetRootPath.Substring(0, assetRootPath.Length - 6) + self;
            return assetRootPath.ToRegularPath();
        }
        public static string ToAssetsPath(this string self)
        {
            string assetRootPath = Path.GetFullPath(Application.dataPath);
            return "Assets" + Path.GetFullPath(self).Substring(assetRootPath.Length).Replace("\\", "/");
        }
        public static string ToReltivePath(this string self)
        {
            return self.Replace("Assets/", "");
        }
    }
}

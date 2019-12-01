/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.14f1
 *Date:           2019-09-11
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;

namespace IFramework
{
	public class ExampleAssetProcessor : UnityEditor.AssetModificationProcessor
    {
        public static bool logEnable;
        private static void OnWillCreateAsset(string metaPath)
        {
            if (logEnable)
            {
                Log.L(string.Format("Wiil Create File \n{0}", metaPath));

            }

        }
        private static AssetDeleteResult OnWillDeleteAsset(string AssetPath, RemoveAssetOptions rao)
        {
            if (logEnable)
            {
                Log.L(string.Format("Will Delete File \n{0} Options \n{1}", AssetPath, rao));

            }
            return AssetDeleteResult.DidNotDelete;
        }
        static string[] OnWillSaveAssets(string[] paths)
        {
            if (logEnable)
            {
                string res = string.Empty;
                foreach (string path in paths)
                    res = res.Append(path).Append("\n");
                Log.L("Will Save Assets  " + res);

            }

            return paths;
        }
        private static AssetMoveResult OnWillMoveAsset(string sourcePath, string destinationPath)
        {
            if (logEnable)
            {
                Log.L("Will Move Asset   \n" + sourcePath + " \n  To \n  " + destinationPath);
            }
            return AssetMoveResult.DidNotMove;
        }
    }
}

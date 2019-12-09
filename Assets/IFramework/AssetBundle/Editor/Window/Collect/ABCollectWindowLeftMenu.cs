/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-04-15
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.GUITool;
using IFramework.Utility;
using UnityEditor;
using UnityEngine;

namespace IFramework.AB
{
    internal class ABCollectWindowLeftMenu : IRectGUIDrawer,ILayoutGUIDrawer
    {
        private ABTempInfo tempInfo { get { return ABWindow.Instance.Info.TempInfo; } }
        public void OnGUI( Rect position)
        {
            position = position.Zoom(AnchorType.MiddleCenter, -2);
            this. Box(position);
            this.DrawArea(() => {
               this.Label("BuildSetting");
               this.Label("", new GUIStyle("IN Title"), GUILayout. Height(5));
               this.Label("Build  Target:");
               this.Label(EditorUserBuildSettings.activeBuildTarget.ToString());
               this.Label("", new GUIStyle("IN Title"), GUILayout.Height(5));
               this.Label("AssetBundle OutPath:");
               this.Label("Assets/../AssetBundles");
               this.Label("", new GUIStyle("IN Title"), GUILayout.Height(5));
               this.Label("Manifest FilePath:");
               this.Label(ABTool.ManifestPath);
               this.Label("", new GUIStyle("IN Title"), GUILayout.Height(5));
               this.Space(10);
                this.Label("LoadSetting In Editor");
                ABTool.ActiveBundleMode = !EditorGUILayout.Toggle(new GUIContent("AssetDataBase Load"),!ABTool.ActiveBundleMode);
            this. Space(10);
            this. Button(() => {
                ABBuild.DeleteBundleFile();

            }, "Clear Bundle Files");
                this.Button(() => {
                    ABBuild.BuildManifest(ABTool.ManifestPath, tempInfo.ToAssetBundleBuild());
                }, "Build Manifest");
                this.Button(() => {
                    ABBuild.BuildManifest(ABTool.ManifestPath, tempInfo.ToAssetBundleBuild());
                    ABBuild.BuildAssetBundles(tempInfo.ToAssetBundleBuild(), EditorUserBuildSettings.activeBuildTarget);
                    ProcessUtil.OpenFloder(ABTool.AssetBundlesOutputDirName);
                }, "Build AssetBundle");
                this.Button(() => {
                    ABBuild.CopyAssetBundlesTo(Application.streamingAssetsPath);
                }, "copy to Stream");

            }, position.Zoom( AnchorType.MiddleCenter,-10));
        }

       
    }
}

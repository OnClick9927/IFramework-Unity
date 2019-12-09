/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-05-24
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEditor;
using UnityEngine;
using IFramework.AB;
using UnityEditorInternal;
using System.Collections;
using IFramework;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;
using Object = UnityEngine.Object;
using System.Linq;
using TreeEditor;
using UnityEditor.IMGUI.Controls;

namespace IFramework
{

    public class tttttt : EditorWindow
    {

        



        public class GameObjectEditorWindow : EditorWindow
        {

            GameObject gameObject;
            Editor gameObjectEditor;

            [MenuItem("Tool/GameObject Editor")]
            static void ShowWindow()
            {
                GetWindow<GameObjectEditorWindow>("GameObject Editor");
            }

            void OnGUI()
            {
                gameObject = (GameObject)EditorGUILayout.ObjectField(gameObject, typeof(GameObject), true);
                if (gameObject != null)
                {
                    if (gameObjectEditor == null)
                        gameObjectEditor = Editor.CreateEditor(gameObject);

                    gameObjectEditor.OnPreviewGUI(new Rect(0, 0, 100, 100), EditorStyles.whiteLabel);
                }
            }
        }


    }
}
public class GUIDRefReplace : EditorWindow
{
    private static GUIDRefReplace _window;
    private Object _sourceOld;
    private Object _sourceNew;

    private string _oldGuid;
    private string _newGuid;

    private bool isContainScene = true;
    private bool isContainPrefab = true;
    private bool isContainMat = true;
    private bool isContainAsset = false;

    private List<string> withoutExtensions = new List<string>();


    [MenuItem("Tool/GUIDRefReplaceWin")]   // 菜单开启并点击的   处理
    public static void GUIDRefReplaceWin()
    {
        _window = (GUIDRefReplace)EditorWindow.GetWindow(typeof(GUIDRefReplace), true, "引用替换 (●'◡'●)");
        _window.Show();

    }

    void OnGUI()
    {
        // 要被替换的（需要移除的）
        GUILayout.Space(20);


        _sourceOld = EditorGUILayout.ObjectField("旧的资源", _sourceOld, typeof(Object), true);
        _sourceNew = EditorGUILayout.ObjectField("新的资源", _sourceNew, typeof(Object), true);


        // 在那些类型中查找（.unity\.prefab\.mat）
        GUILayout.Space(20);
        GUILayout.Label("要在哪些类型中查找替换：");
        EditorGUILayout.BeginHorizontal();

        isContainScene = GUILayout.Toggle(isContainScene, ".unity");
        isContainPrefab = GUILayout.Toggle(isContainPrefab, ".prefab");
        isContainMat = GUILayout.Toggle(isContainMat, ".mat");
        isContainAsset = GUILayout.Toggle(isContainAsset, ".asset");

        EditorGUILayout.EndHorizontal();


        GUILayout.Space(20);
        if (GUILayout.Button("开始替换!"))
        {
            if (EditorSettings.serializationMode != SerializationMode.ForceText)
            {
                Debug.LogError("需要设置序列化模式为 SerializationMode.ForceText");
                ShowNotification(new GUIContent("需要设置序列化模式为 SerializationMode.ForceText"));
            }
            else if (_sourceNew == null || _sourceOld == null)
            {
                Debug.LogError("不能为空！");
                ShowNotification(new GUIContent("不能为空！"));
            }
            else if (_sourceNew.GetType() != _sourceOld.GetType())
            {
                Debug.LogError("两种资源类型不一致！");
                ShowNotification(new GUIContent("两种资源类型不一致！"));
            }
            else if (!isContainScene && !isContainPrefab && !isContainMat && !isContainAsset)
            {
                Debug.LogError("要选择一种 查找替换的类型");
                ShowNotification(new GUIContent("要选择一种 查找替换的类型"));
            }
            else   // 执行替换逻辑
            {
                StartReplace();
            }
        }
    }

    private void StartReplace()
    {
        var path = AssetDatabase.GetAssetPath(_sourceOld);

        _oldGuid = AssetDatabase.AssetPathToGUID(path);
        _newGuid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(_sourceNew));

        Debug.Log("oldGUID = " + _oldGuid + "  " + "_newGuid = " + _newGuid);

        withoutExtensions = new List<string>();
        if (isContainScene)
        {
            withoutExtensions.Add(".unity");
        }
        if (isContainPrefab)
        {
            withoutExtensions.Add(".prefab");
        }
        if (isContainMat)
        {
            withoutExtensions.Add(".mat");
        }
        if (isContainAsset)
        {
            withoutExtensions.Add(".asset");
        }

        Find();
    }


    private void Find()
    {
        if (withoutExtensions == null || withoutExtensions.Count == 0)
        {
            withoutExtensions = new List<string>() { ".prefab", ".unity", ".mat", ".asset" };
        }

        string[] files = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories)
        .Where(s => withoutExtensions.Contains(Path.GetExtension(s).ToLower())).ToArray();
        int startIndex = 0;

        if (files == null || files.Length == 0)
        {
            Debug.Log("没有找到 筛选的引用");
            return;
        }

        EditorApplication.update = delegate ()
        {
            string file = files[startIndex];

            bool isCancel = EditorUtility.DisplayCancelableProgressBar("匹配资源中", file, (float)startIndex / (float)files.Length);

            var content = File.ReadAllText(file);
            if (Regex.IsMatch(content, _oldGuid))
            {
                Debug.Log(file, AssetDatabase.LoadAssetAtPath<Object>(GetRelativeAssetsPath(file)));

                content = content.Replace(_oldGuid, _newGuid);

                File.WriteAllText(file, content);
            }
            else
            {
                Debug.Log(file);
            }

            startIndex++;
            if (isCancel || startIndex >= files.Length)
            {
                EditorUtility.ClearProgressBar();
                EditorApplication.update = null;
                startIndex = 0;

                AssetDatabase.Refresh();
                Debug.Log("替换结束");
            }

        };
    }

    private string GetRelativeAssetsPath(string path)
    {
        return "Assets" + Path.GetFullPath(path).Replace(Path.GetFullPath(Application.dataPath), "").Replace('\\', '/');
    }


}


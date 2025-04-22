/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEditor;
using UnityEngine;

namespace IFramework
{
    public partial class EditorTools
    {
        public class FolderField
        {
            public event Action<string> onValueChange;
            [SerializeField] private string _path;
            public string path { get { return _path; } }
            public string title;
            public string folder;
            public string defaultName;
            public FolderField(string path = "Assets", string folder = "Assets", string title = "Select Folder", string defaultName = "")
            {
                this._path = path;
                this.title = title;
                this.folder = folder;
                this.defaultName = defaultName;
            }

            public void SetPath(string path)
            {
                this._path = path;
            }
            public bool legal { get { return Fitter(path); } }
            protected virtual bool Fitter(string path) { return true; }

            public void OnGUI(Rect position)
            {
                var rects = EditorTools.RectEx.VerticalSplit(position, position.width - 30);
                bool last = GUI.enabled;
                GUI.enabled = false;
                EditorGUI.TextField(rects[0], path);
                GUI.enabled = last;
                Event e = Event.current;
                if (rects[0].Contains(e.mousePosition))
                {
                    var info = EditorTools.DragAndDropTool.Drag(e, rects[0]);
                    if (info.enterArea && info.complete && info.paths.Length == 1 && System.IO.Directory.Exists(info.paths[0]))
                    {
                        _path = info.paths[0];
                        onValueChange?.Invoke(_path);
                    }
                    if (e.clickCount == 2 && e.button == 0 && !string.IsNullOrEmpty(path))
                    {
                        var tmp = path.ToRegularPath();
                        if (tmp.EndsWith("/"))
                            tmp = tmp.Remove(tmp.Length - 1);
                        var o = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(tmp);
                        EditorGUIUtility.PingObject(o);
                    }
                }
                if (GUI.Button(rects[1], EditorGUIUtility.IconContent("Folder Icon")))
                {
                    string tmp = EditorUtility.OpenFolderPanel(title, folder, defaultName);
                    if (!string.IsNullOrEmpty(tmp) && System.IO.Directory.Exists(tmp))
                    {
                        _path = tmp.ToAssetsPath();
                        onValueChange?.Invoke(_path);
                    }
                }
                if (legal)
                {

                    EditorTools.RectEx.DrawOutLine(rects[0], Color.grey);
                }
                else
                {
                    EditorTools.RectEx.DrawOutLine(rects[0], Color.red);
                }
            }

        }

    }
}

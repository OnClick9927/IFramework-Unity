/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-01
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework.GUITool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace IFramework
{
    [CustomEditor(typeof(ScriptCreater))]
    public class ScriptCreaterEditor : Editor,ILayoutGUIDrawer
    {
        private static string originScriptPath;
        public static ScriptCreater SC;
        private void OnEnable()
        {
            originScriptPath = FrameworkConfig.CorePath.CombinePath(@"ScriptCreater/Editor/FormatScript.txt");
            SC = this.target as ScriptCreater;
        }
        private void OnSceneGUI()
        {
            
        }


        public override void OnInspectorGUI()
        {
            if (SC == null) SC = this.target as ScriptCreater;
            SC = this.target as ScriptCreater;

            this.Space(10);
            this.ETextField("Script Name", ref SC.ScriptName);
            if (!SC.ScriptName.IsLegalFieldName()) SC.ScriptName = SC.name.Replace(" ", "").Replace("(", "").Replace(")", "");

           this.Label("Description");
           this.TextArea(ref SC.description, GUILayout.Height(40));
           this.Space(10);
           this.DrawHorizontal(() => {
                GUILayout.Label(new GUIContent("Create Path:", "Drag Floder To Box"));
                Rect rect = EditorGUILayout.GetControlRect();
                rect.DrawOutLine(2, Color.black);
                EditorGUI.LabelField(rect, SC.CreatePath);
                if (!rect.Contains(Event.current.mousePosition)) return;
                var info = DragAndDropUtil.Drag(Event.current, rect);
                if (info.paths.Length > 0 && info.Finsh && info.EnterArera && info.paths[0].IsDirectory())
                    SC.CreatePath = info.paths[0];
            });

            GUILayout.Space(10);
            this.Toggle("Create Prefab", ref SC.isCreatePrefab);
            if (SC.isCreatePrefab)
            {
                this.ETextField("Prefab Name", ref SC.prefabName);
                this.DrawHorizontal(() =>
                {
                    GUILayout.Label(new GUIContent("Prefab Path:", "Drag Floder To Box"));
                    Rect rect = EditorGUILayout.GetControlRect();
                    rect.DrawOutLine(2, Color.black);
                    EditorGUI.LabelField(rect, SC.prefabDir);
                    if (!rect.Contains(Event.current.mousePosition)) return;
                    var info = DragAndDropUtil.Drag(Event.current, rect);
                    if (info.paths.Length > 0 && info.Finsh && info.EnterArera && info.paths[0].IsDirectory())
                        SC.prefabDir = info.paths[0];
                });
            }
            GUILayout.Space(10);

            this.DrawHorizontal(() => {

                if (GUILayout.Button("Build", GUILayout.Height(25)))
                {
                    if (BuildCheck())
                    {
                        Selection.objects = new Object[] { AssetDatabase.LoadAssetAtPath<Object>(SC.CreatePath) };
                        CopyAsset.CopyNewAsset(SC.ScriptName.Append(".cs"), originScriptPath);
                    }
                }
                if (GUILayout.Button("Remove", GUILayout.Height(25)))
                {
                    SC.GetComponentsInChildren<ScriptMark>(true).ToList().ForEach((sm) => {
                        DestroyImmediate(sm);
                    });
                    DestroyImmediate(SC);
                }
            });
            serializedObject.Update();

        }
        private bool BuildCheck()
        {
            if (EditorApplication.isCompiling) return false;
            SC.SMs = SC.GetComponentsInChildren<ScriptMark>(true);
            for (int i = 0; i < SC.SMs.Length; i++)
            {
                if (SC.SMs[i].fieldName == SC.ScriptName)
                {
                    EditorUtility.DisplayDialog("Err", "Field Name Should be diferent With ScriptName", "ok");
                    return false;
                }
                for (int j = i + 1; j < SC.SMs.Length; j++)
                {
                    if (SC.SMs[i].fieldName == SC.SMs[j].fieldName)
                    {
                        EditorUtility.DisplayDialog("Err", "Can't Exist Same Name Field", "ok");
                        return false;
                    }
                }
            }

            CreateOriginIfNull();
            if (!Directory.Exists(SC.CreatePath))
            {
                EditorUtility.DisplayDialog("Err", "Directory Not Exist ", "ok");
                return false;
            }
            if (SC.isCreatePrefab)
            {
                if (!Directory.Exists(SC.prefabDir))
                {
                    EditorUtility.DisplayDialog("Err", "Prefab Directory Not Exist ", "ok");
                    return false;
                }
                if (File.Exists(SC.prefabPath))
                {
                    AssetDatabase.DeleteAsset(SC.prefabPath);
                    AssetDatabase.Refresh();
                }
            }
            return true;
        }
        private void CreateOriginIfNull()
        {
            if (File.Exists(originScriptPath)) return;
            using (FileStream fs = new FileStream(originScriptPath, FileMode.Create, FileAccess.Write))
            {

                using (StreamWriter sw = new StreamWriter(fs))
                {
                    fs.Lock(0, fs.Length);
                    sw.WriteLine("/*********************************************************************************");
                    sw.WriteLine(" *Author:         #SCAuthor#");
                    sw.WriteLine(" *Version:        #SCVERSION#");
                    sw.WriteLine(" *UnityVersion:   #SCUNITYVERSION#");
                    sw.WriteLine(" *Date:           #SCDATE#");
                    sw.WriteLine(" *Description:    #SCDescription#");
                    sw.WriteLine(" *History:        #SCDATE#--");
                    sw.WriteLine("*********************************************************************************/");
                    sw.WriteLine("using System.Collections;");
                    sw.WriteLine("using System.Collections.Generic;");
                    //sw.WriteLine("using UnityEngine;");
                    sw.WriteLine("using IFramework;");
                    sw.WriteLine("#SCUsing#");
                    sw.WriteLine("namespace #SCNameSpace#");
                    sw.WriteLine("{");
                    sw.WriteLine("\tpublic class #SCSCRIPTNAME# : MonoBehaviour");
                    sw.WriteLine("\t{");
                    sw.WriteLine("#SCField#");
                    sw.WriteLine("\t}");
                    sw.WriteLine("}");
                    fs.Unlock(0, fs.Length);
                    sw.Flush();
                    fs.Flush();
                }
            }
            AssetDatabase.Refresh();
        }
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void SetFields()
        {
            if (!EditorPrefs.GetBool(IsCreateKey, false)) return;
            if (!EditorPrefs.HasKey(ScriptNameKey) || !EditorPrefs.HasKey(GameobjKey))
            {
                if (EditorPrefs.HasKey(ScriptNameKey))
                    EditorPrefs.DeleteKey(ScriptNameKey);
                if (EditorPrefs.HasKey(GameobjKey))
                    EditorPrefs.DeleteKey(GameobjKey);
                EditorPrefs.SetBool(IsCreateKey, false);
                EditorUtility.ClearProgressBar();
                return;
            }

            Assembly defaultAssembly = AppDomain.CurrentDomain.GetAssemblies()
                            .First(assembly => assembly.GetName().Name == "Assembly-CSharp");
            Type type = defaultAssembly.GetType(EditorProjectConfig.NameSpace + "." + EditorPrefs.GetString(ScriptNameKey));
            GameObject gameObj = GameObject.Find(EditorPrefs.GetString(GameobjKey));
            if (gameObj == null || type == null || gameObj.GetComponent<ScriptCreater>() == null)
            {
                if (EditorPrefs.HasKey(ScriptNameKey))
                    EditorPrefs.DeleteKey(ScriptNameKey);
                if (EditorPrefs.HasKey(GameobjKey))
                    EditorPrefs.DeleteKey(GameobjKey);
                EditorPrefs.SetBool(IsCreateKey, false);
                EditorUtility.ClearProgressBar();
                return;
            }
            EditorUtility.DisplayProgressBar("Build Script  " + type.Name, "Don't do anything", 0.7f);
            ScriptCreater SC = gameObj.GetComponent<ScriptCreater>();

            ScriptMark[] SMs = SC.SMs;
            Component component = gameObj.GetComponent(type);
            if (component == null) component = gameObj.AddComponent(type);
            SerializedObject serialiedScript = new SerializedObject(component);

            foreach (var sm in SMs)
            {
                serialiedScript.FindProperty(sm.fieldName).objectReferenceValue = sm.GetComponent(sm.fieldType);
            }
            serialiedScript.ApplyModifiedPropertiesWithoutUndo();
            //serialiedScript.Update();
            if (SC.isCreatePrefab)
            {
                EditorUtil.CreatePrefab(gameObj, SC.prefabPath);
            }



            EditorPrefs.SetBool(IsCreateKey, false);
            EditorPrefs.DeleteKey(ScriptNameKey);
            EditorPrefs.DeleteKey(GameobjKey);
            EditorUtility.ClearProgressBar();
        }
        const string ScriptNameKey = "ScriptCreaterEditorSpName";
        const string GameobjKey = "ScriptCreaterEditorGo";
        const string IsCreateKey = "ScriptCreaterEditor";
        private class CreateScriptProcessor : UnityEditor.AssetModificationProcessor
        {
            private static void OnWillCreateAsset(string metaPath)
            {
                if (SC == null) return;
                string filePath = metaPath.Replace(".meta", "");
                if (!filePath.EndsWith(".cs")) return;
                string realPath = filePath.ToAbsPath();
                string txt = File.ReadAllText(realPath);
                if (!txt.Contains("#SCAuthor#")) return;
                string spName = Path.GetFileNameWithoutExtension(filePath);
                EditorUtility.DisplayProgressBar("Build Script  " + spName, "Don't do anything", 0.1f);
                EditorPrefs.SetString(ScriptNameKey, spName);
                EditorPrefs.SetString(GameobjKey, SC.name);
                EditorPrefs.SetBool(IsCreateKey, true);

                txt = txt.Replace("#SCAuthor#", EditorProjectConfig.UserName)
                         .Replace("#SCVERSION#", EditorProjectConfig.Version)
                         .Replace("#SCUNITYVERSION#", Application.unityVersion)
                         .Replace("#SCDATE#", DateTime.Now.ToString("yyyy-MM-dd"))
                         .Replace("#SCNameSpace#", EditorProjectConfig.NameSpace)
                         .Replace("#SCSCRIPTNAME#", Path.GetFileNameWithoutExtension(filePath))
                         .Replace("#SCDescription#", DescriptionStr())
                         .Replace("#SCUsing#", NameSpaceStr(txt));

                EditorUtility.DisplayProgressBar("Build Script  " + spName, "Don't do anything", 0.2f);

                File.WriteAllText(realPath, txt.Replace("#SCField#", FieldsStr(txt)), Encoding.UTF8);
                EditorUtility.DisplayProgressBar("Build Script  " + spName, "Don't do anything", 0.6f);

                AssetDatabase.Refresh();

                TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(filePath);
                if (asset != null)
                {
                    EditorUtility.SetDirty(asset);
                }
                AssetDatabase.ImportAsset(filePath);
                AssetDatabase.SaveAssets();
            }
            private static string DescriptionStr()
            {
                string res = string.IsNullOrEmpty(SC.description) ? EditorProjectConfig.Description : SC.description;
                if (!res.Contains("\n"))
                    return res;
                else
                {
                    string s = string.Empty;
                    var strs = res.Split('\n');
                    for (int i = 0; i < strs.Length; i++)
                    {
                        string str = strs[i];
                        if (i == 0)
                        {
                            s = s.Append(str);
                            if (strs.Length > 1)
                                s = s.Append("\n");
                        }
                        else
                        {
                            s = s.Append("                  " + str);
                            if (i < strs.Length - 1)
                                s = s.Append("\n");
                        }
                    }

                    return s;
                }

            }
            private static string NameSpaceStr(string txt)
            {
                string res = string.Empty;
                List<string> NameSpaces = new List<string>();
                for (int i = 0; i < SC.SMs.Length; i++)
                {
                    NameSpaces.Add(SC.SMs[i].GetComponent(SC.SMs[i].fieldType).GetType().Namespace);
                }
                NameSpaces = NameSpaces.Distinct().ToList();
                NameSpaces.ForEach((ns) => {
                    string tmp= "using ".Append(ns);
                    if (!txt.Contains(tmp))
                    {
                        res = res.Append(tmp).Append(";\n");
                    }
                });
                if (txt.Contains("MonoBehaviour") && !res.Contains("using UnityEngine"))
                {
                    res = res.Append("using UnityEngine;\n");
                }
                return res;
            }
            private static string FieldsStr(string txt)
            {
                string result = string.Empty;
                SC.SMs.ForEach((sm) => {
                    if (!string.IsNullOrEmpty(sm.description))
                    {
                        if (sm.description.Contains("\n"))
                        {
                            sm.description.Split('\n').ToList().ForEach((str) =>
                            {
                                result = result.Append("\t\t//" + str + "\n");
                            });
                        }
                        else
                            result = result.Append("\t\t//" + sm.description + "\n");

                    }

                    if (sm.isPublic)
                        result = result.Append("\t\tpublic " + sm.fieldType + " " + sm.fieldName + ";\n");
                    else
                    {
                        if (txt.Contains("using UnityEngine"))
                            result = result.Append("\t\t[SerializeField] private " + sm.fieldType + " " + sm.fieldName + ";\n");
                        else
                            result = result.Append("\t\t[UnityEngine.SerializeField] private " + sm.fieldType + " " + sm.fieldName + ";\n");

                    }
                });
                return result;
            }
        }
    }
}


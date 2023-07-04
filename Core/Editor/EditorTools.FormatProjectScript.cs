/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Text;

namespace IFramework
{

    public static partial class EditorTools
    {

        class FormatProjectScript
        {
            const string key = "FormatUserScript";
            private static string cs = EditorTools.projectMemoryPath.CombinePath("UserCSharpScript.txt");
            private static string newScriptName = "newScript.cs";
            private static void Head(StreamWriter sw)
            {
                sw.WriteLine("/*********************************************************************************");
                sw.WriteLine(" *Author:         #User#");
                sw.WriteLine(" *Version:        #UserVERSION#");
                sw.WriteLine(" *UnityVersion:   #UserUNITYVERSION#");
                sw.WriteLine(" *Date:           #UserDATE#");
                sw.WriteLine("*********************************************************************************/");
                sw.WriteLine("using System;");
                sw.WriteLine("using System.Collections;");
                sw.WriteLine("using System.Collections.Generic;");
                sw.WriteLine("using IFramework;");
                sw.WriteLine("");

            }

            private static void CS()
            {
                if (File.Exists(cs)) return;
                using (FileStream fs = new FileStream(cs, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        fs.Lock(0, fs.Length);
                        Head(sw);
                        sw.WriteLine("namespace #UserNameSpace#");
                        sw.WriteLine("{");
                        sw.WriteLine("\tpublic class #UserSCRIPTNAME#");
                        sw.WriteLine("\t{");
                        sw.WriteLine("\t");
                        sw.WriteLine("\t}");
                        sw.WriteLine("}");
                        fs.Unlock(0, fs.Length);
                        sw.Flush();
                        fs.Flush();
                    }
                }
                AssetDatabase.Refresh();
            }

            [MenuItem("Assets/Create/FormatCSharpScript", priority = -1000)]
            public static void Create2()
            {
                CS();
                ProjectWindowUtil.CreateScriptAssetFromTemplateFile(cs, newScriptName);
                EditorPrefs.SetBool(key, true);

            }
            private class FormatUserScriptProcessor : UnityEditor.AssetModificationProcessor
            {
                public static void OnWillCreateAsset(string metaPath)
                {
                    if (!EditorPrefs.GetBool(key, false)) return;

                    string filePath = metaPath.Replace(".meta", "");
                    if (!filePath.EndsWith(".cs")) return;
                    string realPath = filePath.ToAbsPath();
                    string txt = File.ReadAllText(realPath);

                    if (!txt.Contains("#User#")) return;
                    //这里实现自定义的一些规则
                    txt = txt.Replace("#User#", ProjectConfig.UserName)
                             .Replace("#UserSCRIPTNAME#", Path.GetFileNameWithoutExtension(filePath))
                             .Replace("#UserNameSpace#", ProjectConfig.NameSpace)
                             .Replace("#UserVERSION#", ProjectConfig.Version)
                             .Replace("#UserUNITYVERSION#", Application.unityVersion)
                             .Replace("#UserDATE#", DateTime.Now.ToString("yyyy-MM-dd")).ToUnixLineEndings();

                    File.WriteAllText(realPath, txt, Encoding.UTF8);
                    EditorPrefs.SetBool(key, false);
                    AssetDatabase.Refresh();
                }
            }

        }
    }
}

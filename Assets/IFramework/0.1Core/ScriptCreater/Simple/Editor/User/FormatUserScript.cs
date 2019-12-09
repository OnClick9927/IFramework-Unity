/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-23
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace IFramework
{
    class FormatUserScript
    {
        const string key = "FormatUserScript";

        private class FormatUserScriptProcessor : UnityEditor.AssetModificationProcessor
        {
            public static void OnWillCreateAsset(string metaPath)
            {
                if (!EditorPrefs.GetBool(key, false) ) return;

                string filePath = metaPath.Replace(".meta", "");
                if (!filePath.EndsWith(".cs")) return;
                string realPath = filePath.ToAbsPath();
                string txt = File.ReadAllText(realPath);

                if (!txt.Contains("#User#")) return;
                //这里实现自定义的一些规则
                txt = txt.Replace("#User#", EditorProjectConfig.UserName)
                         .Replace("#UserSCRIPTNAME#", Path.GetFileNameWithoutExtension(filePath))
                         .Replace("#UserNameSpace#", EditorProjectConfig.NameSpace)
                         .Replace("#UserVERSION#", EditorProjectConfig.Version)
                        .Replace("#UserDescription#", EditorProjectConfig.Description)

                         .Replace("#UserUNITYVERSION#", Application.unityVersion)
                         .Replace("#UserDATE#", DateTime.Now.ToString("yyyy-MM-dd"));

                File.WriteAllText(realPath, txt, Encoding.UTF8);
                EditorPrefs.SetBool(key, false);
            }
        }
        private class FormatUserCSScript
        {

            private static string newScriptName = "newScript.cs";
            private static string originScriptPathWithNameSpace = FrameworkConfig.CorePath.CombinePath(@"ScriptCreater/Simple/Editor/User/CSharpScript.txt");

            [MenuItem("IFramework /Create/FormatCSharpScript")]
            [MenuItem("Assets/IFramework/FormatCSharpScript")]
            public static void CreateUIBase()
            {
                CreateOriginIfNull();
                CopyAsset.CopyNewAsset(newScriptName, originScriptPathWithNameSpace);
                EditorPrefs.SetBool(key, true);
            }
            private static void CreateOriginIfNull()
            {
                if (File.Exists(originScriptPathWithNameSpace)) return;
                using (FileStream fs = new FileStream(originScriptPathWithNameSpace, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        fs.Lock(0, fs.Length);
                        sw.WriteLine("/*********************************************************************************");
                        sw.WriteLine(" *Author:         #User#");
                        sw.WriteLine(" *Version:        #UserVERSION#");
                        sw.WriteLine(" *UnityVersion:   #UserUNITYVERSION#");
                        sw.WriteLine(" *Date:           #UserDATE#");
                        sw.WriteLine(" *Description:    #UserDescription#");
                        sw.WriteLine(" *History:        #UserDATE#--");
                        sw.WriteLine("*********************************************************************************/");
                        sw.WriteLine("using System;");
                        sw.WriteLine("using System.Collections;");
                        sw.WriteLine("using System.Collections.Generic;");
                        sw.WriteLine("using IFramework;");

                        sw.WriteLine("");
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
        }
        private class FormatUserMonoScript
        {
            private static string newScriptName = "newScript.cs";
            private static string originScriptPathWithNameSpace = FrameworkConfig.CorePath.CombinePath(@"ScriptCreater/Simple/Editor/User/MonoScript.txt");

            [MenuItem("IFramework/Create/FormatMonoScript")]
            [MenuItem("Assets/IFramework/FormatMonoScript")]
            public static void CreateUIBase()
            {
                CreateOriginIfNull();
                CopyAsset.CopyNewAsset(newScriptName, originScriptPathWithNameSpace);


                EditorPrefs.SetBool(key, true);
            }

            private static void CreateOriginIfNull()
            {
                if (File.Exists(originScriptPathWithNameSpace)) return;
                using (FileStream fs = new FileStream(originScriptPathWithNameSpace, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        fs.Lock(0, fs.Length);
                        sw.WriteLine("/*********************************************************************************");
                        sw.WriteLine(" *Author:         #User#");
                        sw.WriteLine(" *Version:        #UserVERSION#");
                        sw.WriteLine(" *UnityVersion:   #UserUNITYVERSION#");
                        sw.WriteLine(" *Date:           #UserDATE#");
                        sw.WriteLine(" *Description:    #UserDescription#");
                        sw.WriteLine(" *History:        #UserDATE#--");
                        sw.WriteLine("*********************************************************************************/");
                        sw.WriteLine("using System;");
                        sw.WriteLine("using System.Collections;");
                        sw.WriteLine("using System.Collections.Generic;");
                        sw.WriteLine("using UnityEngine;");
                        sw.WriteLine("using IFramework;");

                        sw.WriteLine("");
                        sw.WriteLine("namespace #UserNameSpace#");
                        sw.WriteLine("{");
                        sw.WriteLine("\tpublic class #UserSCRIPTNAME# : MonoBehaviour");
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
        }
    }
	
}

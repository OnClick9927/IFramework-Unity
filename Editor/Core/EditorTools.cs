/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.1f1
 *Date:           2019-03-18
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace IFramework
{
    partial class EditorTools
    {
        class DrawerObject : UnityEngine.ScriptableObject
        {
            public static Editor CreateEditor(object target)
            {
                sto = sto ?? DrawerObject.CreateInstance<DrawerObject>();
                sto.hideFlags = HideFlags.DontSave;
                sto.obj = target;
                if (editor == null) editor = Editor.CreateEditor(sto);
                return editor;
            }
            [SerializeReference]
            public object obj;
            private static DrawerObject sto;
            private static Editor editor;
        }
        [CustomEditor(typeof(DrawerObject))]
        class DrawerObjectEditor : Editor
        {

            public static List<SerializedProperty> GetDirectChildProperties(SerializedProperty parentProp)
            {
                List<SerializedProperty> childProps = new List<SerializedProperty>();
                if (parentProp == null || !parentProp.hasChildren) return childProps;

                // 重置到第一个子属性
                SerializedProperty childProp = parentProp.Copy();
                bool hasNext = childProp.Next(true);

                while (hasNext)

                {
                    // 终止条件：遍历到当前父属性的同级属性时，停止遍历
                    if (childProp.propertyPath == parentProp.propertyPath)
                    {
                        break;
                    }

                    childProps.Add(childProp.Copy()); // 必须Copy！否则后续Next会改变当前引用
                    hasNext = childProp.Next(false);
                }
                return childProps;
            }
            //private Vector2 scroll;
            public override void OnInspectorGUI()
            {
                this.serializedObject.Update();
                var p = this.serializedObject.FindProperty(nameof(DrawerObject.obj));
                var children = GetDirectChildProperties(p);
                //scroll = GUILayout.BeginScrollView(scroll);
                GUILayout.BeginVertical();
                foreach (var item in children)
                {
                    EditorGUILayout.PropertyField(item);
                    //GUILayout.Space(2);
                }
                GUILayout.EndVertical();
                //GUILayout.EndScrollView();
                this.serializedObject.ApplyModifiedProperties();
            }
        }
    }

    [InitializeOnLoad]

    public partial class EditorTools
    {
        static Dictionary<Type, List<Delegate>> on_addComp = new Dictionary<Type, List<Delegate>>();
        public static void CallAddComponent(Component obj)
        {
            Type type = obj.GetType();
            List<Delegate> list;
            if (!on_addComp.TryGetValue(type, out list)) return;

            foreach (var del in list)
            {
                del.DynamicInvoke(obj);
            }

        }

        static EditorTools()
        {
            ObjectFactory.componentWasAdded += CallAddComponent;

            var result = GetTypes()
                    .SelectMany(x => x.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
                   .Where(x => x.IsDefined(typeof(OnAddComponentAttribute)))
                   .Where(x =>
                   {
                       var param = x.GetParameters();
                       return param.Length == 1 && param[0].ParameterType.IsSubclassOf(typeof(Component));
                   })
                   .Select(x =>
                   {
                       var attr = x.GetCustomAttribute<OnAddComponentAttribute>();
                       return new { method = x, type = attr.type, oder = attr.oder };
                   });
            var types = result.Select(x => x.type).Distinct().ToList();
            foreach (var type in types)
            {
                var list = result.Where(x => x.type == type).ToList();
                list.Sort((x, y) => x.oder - y.oder);
                on_addComp[type] = new List<Delegate>(list.Select(x => x.method.ToDelegate(null)));
            }

            var directorys = new List<string>()
            {
                "Assets/Editor",
                EditorTools.projectMemoryPath,
            };
            CreateDirectories(directorys);

            AssetDatabase.Refresh();




            Log.logger = new UnityLogger();
            SetLogStatus();
        }



        public static void SetLogStatus()
        {
            Log.enable_F = ProjectConfig.enable_F;

            Log.enable_L = ProjectConfig.enable_L;
            Log.enable_W = ProjectConfig.enable_W;
            Log.enable_E = ProjectConfig.enable_E;

            Log.enable = ProjectConfig.enable;
        }

        public const string projectMemoryPath = "Assets/Editor/IFramework";

        private static string GetFilePath() => AssetDatabase.GetAllAssetPaths().FirstOrDefault(x => x.Contains(nameof(IFramework))
                                                        && x.EndsWith($"{nameof(EditorTools)}.cs"));
        public static string pkgPath
        {
            get
            {
                string packagePath = Path.GetFullPath("Packages/com.woo.iframework");
                if (Directory.Exists(packagePath))
                {
                    return packagePath;
                }

                string path = GetFilePath();
                var index = path.LastIndexOf("IFramework");
                path = path.Substring(0, index + "IFramework".Length);
                return path;
            }
        }


        public static void SaveToPrefs<T>(T value, string key, bool unique = true) => Prefs.SetObject(value.GetType(), key, value, unique);
        public static T GetFromPrefs<T>(string key, bool unique = true) => Prefs.GetObject<T>(key, unique);
        public static object GetFromPrefs(Type type, string key, bool unique = true) => Prefs.GetObject(type, key, unique);



        public static void OpenFolder(string folder) => EditorUtility.OpenWithDefaultApp(folder);

        public static string ToAbsPath(this string self)
        {
            string assetRootPath = Path.GetFullPath(Application.dataPath);
            assetRootPath = assetRootPath.Substring(0, assetRootPath.Length - 6) + self;
            return assetRootPath.ToRegularPath();
        }
        public static string ToAssetsPath(this string self) => "Assets" + Path.GetFullPath(self).Substring(Path.GetFullPath(Application.dataPath).Length).Replace("\\", "/");
        public static string GetPath(this Transform transform)
        {
            var sb = new System.Text.StringBuilder();
            var t = transform;
            while (true)
            {
                sb.Insert(0, t.name);
                t = t.parent;
                if (t)
                {
                    sb.Insert(0, "/");
                }
                else
                {
                    return sb.ToString();
                }
            }
        }
        public static string ToUnixLineEndings(this string self) => self.Replace("\r\n", "\n").Replace("\r", "\n");

        public static Delegate ToDelegate(this MethodInfo method, object target)
        {
            var _params = method.GetParameters();
            Type delegateType = default;
            var void_func = method.ReturnType == typeof(void);

            Type base_func_type = void_func ? typeof(Action) : typeof(Func<>);
            if (void_func)
            {
                if (_params == null || _params.Length == 0)
                    delegateType = typeof(Action);
                else
                {
                    if (_params.Length == 1) base_func_type = typeof(Action<>);
                    else if (_params.Length == 2) base_func_type = typeof(Action<,>);
                    else if (_params.Length == 3) base_func_type = typeof(Action<,,>);
                    else if (_params.Length == 4) base_func_type = typeof(Action<,,,>);
                    else if (_params.Length == 5) base_func_type = typeof(Action<,,,,>);
                    else if (_params.Length == 6) base_func_type = typeof(Action<,,,,,>);
                    else if (_params.Length == 7) base_func_type = typeof(Action<,,,,,,>);
                    else if (_params.Length == 8) base_func_type = typeof(Action<,,,,,,,>);
                    else if (_params.Length == 9) base_func_type = typeof(Action<,,,,,,,,>);
                    else if (_params.Length == 10) base_func_type = typeof(Action<,,,,,,,,,>);
                    else if (_params.Length == 11) base_func_type = typeof(Action<,,,,,,,,,,>);
                    else if (_params.Length == 12) base_func_type = typeof(Action<,,,,,,,,,,,>);
                    else if (_params.Length == 13) base_func_type = typeof(Action<,,,,,,,,,,,,>);
                    else if (_params.Length == 14) base_func_type = typeof(Action<,,,,,,,,,,,,,>);
                    else if (_params.Length == 15) base_func_type = typeof(Action<,,,,,,,,,,,,,,>);
                    else if (_params.Length == 16) base_func_type = typeof(Action<,,,,,,,,,,,,,,,>);
                    delegateType = base_func_type
                                    .MakeGenericType(_params
                                            .Select(x => x.ParameterType)
                                            .ToArray());

                }
            }
            else
            {

                if (_params == null || _params.Length == 0)
                {
                    delegateType = base_func_type.MakeGenericType(new Type[] { method.ReturnType });
                }
                else
                {
                    if (_params.Length == 1) base_func_type = typeof(Func<,>);
                    else if (_params.Length == 2) base_func_type = typeof(Func<,,>);
                    else if (_params.Length == 3) base_func_type = typeof(Func<,,,>);
                    else if (_params.Length == 4) base_func_type = typeof(Func<,,,,>);
                    else if (_params.Length == 5) base_func_type = typeof(Func<,,,,,>);
                    else if (_params.Length == 6) base_func_type = typeof(Func<,,,,,,>);
                    else if (_params.Length == 7) base_func_type = typeof(Func<,,,,,,,>);
                    else if (_params.Length == 8) base_func_type = typeof(Func<,,,,,,,,>);
                    else if (_params.Length == 9) base_func_type = typeof(Func<,,,,,,,,,>);
                    else if (_params.Length == 10) base_func_type = typeof(Func<,,,,,,,,,,>);
                    else if (_params.Length == 11) base_func_type = typeof(Func<,,,,,,,,,,,>);
                    else if (_params.Length == 12) base_func_type = typeof(Func<,,,,,,,,,,,,>);
                    else if (_params.Length == 13) base_func_type = typeof(Func<,,,,,,,,,,,,,>);
                    else if (_params.Length == 14) base_func_type = typeof(Func<,,,,,,,,,,,,,,>);
                    else if (_params.Length == 15) base_func_type = typeof(Func<,,,,,,,,,,,,,,,>);
                    else if (_params.Length == 16) base_func_type = typeof(Func<,,,,,,,,,,,,,,,,>);
                    delegateType = base_func_type
                                    .MakeGenericType(_params
                                            .Select(x => x.ParameterType)
                                            .Concat(new Type[] { method.ReturnType })
                                            .ToArray());

                }

            }
            return method.CreateDelegate(delegateType, target);
        }

        public static IEnumerable<Type> GetTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                                 .SelectMany(item => item.GetTypes());
        }
        public static IEnumerable<Type> GetSubTypesInAssemblies(this Type self)
        {
            if (self.IsInterface)
                return GetTypes().Where(item => item.GetInterfaces().Contains(self));
            if (self.IsGenericType)
            {
                return GetTypes().Where(x =>
                {
                    var _type = x;
                    while (_type != typeof(System.Object))
                    {
                        if (_type.IsGenericType && _type.GetGenericTypeDefinition() == self)
                        {
                            return true;
                        }
                        _type = _type.BaseType;
                        if (_type == null)
                        {
                            break;
                        }
                    }

                    return false;
                });
            }
            return GetTypes().Where(item => item.IsSubclassOf(self));
        }

        public static string ToRegularPath(this string path) => path.Replace('\\', '/');

        public static string CombinePath(this string path, string toCombinePath) => Path.Combine(path, toCombinePath).ToRegularPath();
        public static void CreateDirectories(List<string> directories)
        {
            foreach (var path in directories)
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
        }


        private static Type winType;
        static MethodInfo _AdvancedPopup, _AdvancedPopup_layout;
        public static int AdvancedPopup(Rect rect, int selectedIndex, string[] displayedOptions, float minHeight, GUIStyle style)
        {
            if (_AdvancedPopup == null)
            {
                _AdvancedPopup = typeof(EditorGUI).GetMethod(nameof(AdvancedPopup), BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] {
                 typeof(Rect),   typeof(int),typeof(string[]),typeof(GUIStyle)
                }, null);
            }
            if (winType == null)
                winType = typeof(TreeView).Assembly.GetTypes().First(x => x.Name == "AdvancedDropdownWindow");
            var find = Resources.FindObjectsOfTypeAll(winType);
            if (find != null && find.Length != 0)
            {
                var win = (find[0] as EditorWindow);
                var pos = win.position;
                win.minSize = new Vector2(win.minSize.x, minHeight);
            }
            var value = _AdvancedPopup.Invoke(null, new object[]
                {
                       rect, selectedIndex,displayedOptions,style
                });
            return (int)value;

        }
        public static int AdvancedPopup(int selectedIndex, string[] displayedOptions, float minHeight, GUIStyle style, params GUILayoutOption[] options)
        {
            if (_AdvancedPopup_layout == null)
            {
                _AdvancedPopup_layout = typeof(EditorGUILayout).GetMethod(nameof(AdvancedPopup), BindingFlags.Static | BindingFlags.NonPublic, null, new Type[] {
                    typeof(int),typeof(string[]),typeof(GUIStyle),typeof(GUILayoutOption[])
                }, null);


            }

            var value = _AdvancedPopup_layout.Invoke(null, new object[]
                  {
                        selectedIndex,displayedOptions,style,options
                  });
            if (winType == null)
                winType = typeof(TreeView).Assembly.GetTypes().First(x => x.Name == "AdvancedDropdownWindow");

            var find = Resources.FindObjectsOfTypeAll(winType);
            if (find != null && find.Length != 0)
            {
                var win = (find[0] as EditorWindow);
                var pos = win.position;
                win.minSize = new Vector2(win.minSize.x, minHeight);
            }
            return (int)value;
        }


        public static object DrawDefaultInspector(object obj)
        {
            var editor = DrawerObject.CreateEditor(obj);
            editor.OnInspectorGUI();
            return obj;
        }





        [MenuItem("GameObject/Copy Path", true, 1001)]
        public static bool ValidateLog2()
        {
            return Selection.activeTransform != null && Selection.transforms.Length == 1;
        }
        [MenuItem("GameObject/Copy Path", priority = -10000, validate = false)]
        static void CopyPath()
        {
            GUIUtility.systemCopyBuffer = Selection.activeTransform.GetPath();
        }



        [MenuItem("Tools/IFramework/Open Path/Persistent")]
        public static void OpenPath_Persistent() => EditorTools.OpenFolder(Application.persistentDataPath);

        [MenuItem("Tools/IFramework/Open Path/Streaming")]
        public static void OpenPath_Streaming() => EditorTools.OpenFolder(Application.streamingAssetsPath);

        [MenuItem("Tools/IFramework/Open Path/Assets")]
        public static void OpenPath_Assets() => EditorTools.OpenFolder(Application.dataPath);

        [MenuItem("Tools/IFramework/Open Path/Temporary")]
        public static void OpenPath_Temporary() => EditorTools.OpenFolder(Application.temporaryCachePath);
#if UNITY_2018_1_OR_NEWER

        [MenuItem("Tools/IFramework/Open Path/Console")]
        public static void OpenPath_Console() => EditorTools.OpenFolder(Path.GetDirectoryName(Application.consoleLogPath));

#endif
        [MenuItem("Tools/IFramework/Github")]
        static void Github() => Application.OpenURL("https://github.com/OnClick9927/IFramework-Unity");
        [MenuItem("Tools/IFramework/Join us")]
        static void Join() => Application.OpenURL("https://jq.qq.com/?_wv=1027&k=TTSfAM1P");




        public static void DrawStackTrace(string stackTrack)
        {
            GUIStyle style = "CN Message";
            var result = Regex.Matches(stackTrack, @"\(at <a href.+>(.+)</a>\)").ToArray();
            foreach (var item in result)
            {
                EditorGUILayout.LabelField(item.Value, style);

                var _rect = GUILayoutUtility.GetLastRect();
                EditorGUIUtility.AddCursorRect(_rect, MouseCursor.Link);
                if (GUI.Button(_rect, "", style))
                {
                    var match = Regex.Match(item.Value, @">.+:([0-9]+)</a>");
                    var r = match.Value.Replace("</a>", "").Replace(">", "").Replace("//", "/");
                    var rs = r.Split(':');
                    string path = rs[0];
                    int line = int.Parse(rs[1]);
                    UnityEditorInternal.InternalEditorUtility.TryOpenErrorFileFromConsole(path, line);
                }

            }
        }
        public static string AddHyperLink(this System.Diagnostics.StackTrace stackTrace)
        {
            if (stackTrace == null) return "";
            StringBuilder sb = new StringBuilder();
            sb.Clear();
            for (int i = 0; i < stackTrace.FrameCount; i++)
            {
                var sf = stackTrace.GetFrame(i);

                if (sf.GetILOffset() != -1)
                {
                    string fileName = null;
                    try
                    {
                        fileName = sf.GetFileName();
                    }
                    catch (NotSupportedException) { }
                    catch (SecurityException) { }

                    if (fileName != null)
                    {
                        sb.Append(' ');
                        sb.AppendFormat(CultureInfo.InvariantCulture, "(at {0})", AppendHyperLink(fileName, sf.GetFileLineNumber().ToString()));
                        sb.AppendLine();
                    }
                }
            }
            return sb.ToString();
        }
        static string AppendHyperLink(string path, string line)
        {
            var fi = new FileInfo(path);
            if (fi.Directory == null)
            {
                return fi.Name;
            }
            else
            {
                var fname = fi.FullName.Replace(Path.DirectorySeparatorChar, '/').Replace(Application.dataPath, "");
                var withAssetsPath = "Assets/" + fname;
                return "<a href=\"" + withAssetsPath + "\" line=\"" + line + "\">" + withAssetsPath + ":" + line + "</a>";
            }
        }

        private static Dictionary<Type, UnityEngine.Object> scriptObjs = new Dictionary<Type, UnityEngine.Object>();

        public static void DrawPingScript(string label,Type type)
        {
            if (!scriptObjs.TryGetValue(type, out var obj))
            {
                var path = LocateScript(type);
                obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                scriptObjs[type] = obj;
            }
            if (obj != null)
            {
                GUILayout.Space(10);
                GUI.enabled = false;
                EditorGUILayout.ObjectField(label, obj, obj.GetType(), false);
                GUI.enabled = true;
                GUILayout.Space(10);
            }
        }
        public static string LocateScript(Type targetType)
        {

            if (targetType == null)
                return string.Empty;

            string fullTypeName = targetType.FullName;
            string className = targetType.Name;

            string[] csGuids = AssetDatabase.FindAssets("t:Script");

            foreach (string guid in csGuids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);

                if (!assetPath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                    continue;
                string fileName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
                if (fileName.Equals(className, StringComparison.OrdinalIgnoreCase))
                    return assetPath;

                string fileContent = System.IO.File.ReadAllText(assetPath);
                string pattern = $@"\b(class|struct|enum)\s+{Regex.Escape(className)}\b";
                if (Regex.IsMatch(fileContent, pattern, RegexOptions.IgnoreCase))
                    return assetPath;

            }

            return string.Empty;
        }
    }
}

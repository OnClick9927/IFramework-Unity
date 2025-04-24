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
using System.Runtime.Remoting.Contexts;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace IFramework
{

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
            var type = obj.GetType();
            //得到字段的值,只能得到public类型的字典的值
            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            //排序一下，子类的字段在后，父类的在前
            //Array.Sort(fieldInfos, FieldsSprtBy);

            //判断需要过滤不显示的字段
            List<FieldInfo> needShowField = new List<FieldInfo>();
            foreach (var field in fieldInfos)
            {
                var need = true;
                var attributes = field.GetCustomAttributes();
                foreach (var attribute in attributes)
                {
                    if (attribute is HideInInspector hide)
                    {
                        need = false;
                        break;
                    }


                }

                if (need)
                {
                    needShowField.Add(field);
                }
            }
            GUILayout.BeginVertical();
            foreach (var field in needShowField)
            {
                FieldDefaultInspector(field, obj);
            }
            GUILayout.EndVertical();
            return obj;
        }

        static List<Type> _base = new List<Type>()
            {
                typeof(int),typeof(float),typeof(double),typeof(bool),typeof(long),typeof(string),
                typeof(Color),typeof(Vector2),typeof(Vector3),typeof(Vector4),typeof(Vector2Int),typeof(Vector3Int),
                typeof(Rect),typeof(RectInt),typeof(Bounds),typeof(UnityEngine.Object),typeof(AnimationCurve),
            };
        private static bool IsBaseType(Type type)
        {
            if (type.IsSubclassOf(typeof(UnityEngine.Object)))
                return true;
            if (type.IsEnum || _base.Contains(type)) return true;
            return false;
        }
        private static object DrawBase(object value, string name, Type fieldType)
        {

            if (fieldType == typeof(int)) return EditorGUILayout.IntField(name, (int)value);
            else if (fieldType == typeof(float)) return EditorGUILayout.FloatField(name, (float)value);
            else if (fieldType == typeof(bool)) return EditorGUILayout.Toggle(name, (bool)value);
            else if (fieldType == typeof(string)) return EditorGUILayout.TextField(name, (string)value);
            else if (fieldType == typeof(long)) return EditorGUILayout.LongField(name, (long)value);
            else if (fieldType == typeof(double)) return EditorGUILayout.DoubleField(name, (double)value);
            else if (fieldType.IsEnum) return EditorGUILayout.EnumPopup(name, (Enum)value);
            else if (fieldType == typeof(Color)) return EditorGUILayout.ColorField(name, (Color)value);
            else if (fieldType == typeof(Vector2)) return EditorGUILayout.Vector2Field(name, (Vector2)value);
            else if (fieldType == typeof(Vector3)) return EditorGUILayout.Vector3Field(name, (Vector3)value);
            else if (fieldType == typeof(Vector4)) return EditorGUILayout.Vector4Field(name, (Vector4)value);
            else if (fieldType == typeof(Vector2Int)) return EditorGUILayout.Vector2IntField(name, (Vector2Int)value);
            else if (fieldType == typeof(Vector3Int)) return EditorGUILayout.Vector3IntField(name, (Vector3Int)value);
            else if (fieldType == typeof(Rect)) return EditorGUILayout.RectField(name, (Rect)value);
            else if (fieldType == typeof(RectInt)) return EditorGUILayout.RectIntField(name, (RectInt)value);
            else if (fieldType == typeof(Bounds)) return EditorGUILayout.BoundsField(name, (Bounds)value);
            else if (fieldType.IsSubclassOf(typeof(UnityEngine.Object))) return EditorGUILayout.ObjectField(name, (UnityEngine.Object)value, fieldType, true);
            else if (fieldType == typeof(AnimationCurve))
            {
                AnimationCurve curve = value as AnimationCurve;
                if (curve == null)
                {
                    curve = new AnimationCurve();
                }

                return EditorGUILayout.CurveField(name, curve);
            }
            return value;
        }
        private static object DrawObj(object value, string name, Type fieldType)
        {

            bool fold = GetFoldout(value);
            fold = EditorGUILayout.Foldout(fold, $"{name}", true);
            SetFoldout(value, fold);
            if (fold)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                var Newvalue= DrawDefaultInspector(value);
                GUILayout.EndHorizontal();
                return Newvalue;
            }
            return value;
        }

        private static float DrawRange(string name, float value, float min, float max)
        {
            return EditorGUILayout.Slider(name, (float)value, min, max);
        }
        private static string DrawMutiLine(string name, string value, int lines)
        {
            GUILayout.Label(name);
            return EditorGUILayout.TextArea(value, GUILayout.MinHeight(lines * 18));
        }


        private static Dictionary<int, bool> _unfoldDictionary = new Dictionary<int, bool>();

        private static IList DrawArr(ref bool fold, string name, IList arr, Type ele)
        {
            GUILayout.BeginVertical();
            IList array = Activator.CreateInstance(typeof(List<>).MakeGenericType(ele)) as IList;

            for (int i = 0; i < arr.Count; i++)
                array.Add(arr[i]);
            var cout = array.Count;
            //GUILayout.Label("", EditorStyles.toolbar);
            var rect = EditorGUILayout.GetControlRect(GUILayout.Height(20));
            GUI.Label(rect, "", EditorStyles.toolbarPopup);

            var rs_second = RectEx.VerticalSplit(rect, rect.width - 20);

            fold = EditorGUI.Foldout(rs_second[0], fold, $"{name}({ele.Name}): {cout}", true);
            if (GUI.Button(rs_second[1], EditorGUIUtility.TrIconContent("d_Toolbar Plus"), EditorStyles.toolbarButton))
            {
                Array newArray = Array.CreateInstance(ele, array != null ? array.Count + 1 : 1);
                if (array != null)
                {
                    array.CopyTo(newArray, 0);
                }

                newArray.SetValue(Activator.CreateInstance(ele), newArray.Length - 1);
                array = newArray;
                SetFoldout(newArray, true);
            }

            if (fold)
            {
                //GUILayout.Space(6);
                GUILayout.BeginVertical();
                for (int i = 0; i < array.Count; i++)
                {
                    object listItem = array[i];
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (IsBaseType(ele))
                            array[i] = DrawBase(listItem, $"Element {i}", ele);
                        else
                            array[i] = DrawDefaultInspector(listItem);

                        if (GUILayout.Button(EditorGUIUtility.TrIconContent("d_Toolbar Minus"), GUILayout.Width(20)))
                        {
                            array.Remove(listItem);
                            break;
                        }

                        using (new EditorGUI.DisabledGroupScope(i == 0))
                            if (GUILayout.Button(EditorGUIUtility.TrIconContent("d_scrollup"), GUILayout.Width(20)))
                            {
                                var temp = array[i];
                                array[i] = array[i - 1];
                                array[i - 1] = temp;

                            }
                        using (new EditorGUI.DisabledGroupScope(i == array.Count - 1))

                            if (GUILayout.Button(EditorGUIUtility.TrIconContent("d_scrolldown"), GUILayout.Width(20)))
                            {
                                var temp = array[i];
                                array[i] = array[i + 1];
                                array[i + 1] = temp;
                            }
                    }

                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space(2);
                }
                GUILayout.EndVertical();
            }
            GUILayout.EndVertical();
            return array;
        }


        public static void FieldDefaultInspector(MemberInfo field, object obj)
        {

            if (obj is Delegate) return;
            if (!(field is FieldInfo) && !(field is PropertyInfo)) return;

            Type fieldType = null;
            Type showType = null;
            object value = null;
            if (field is FieldInfo)
            {
                fieldType = (field as FieldInfo).FieldType;
                showType = (field as FieldInfo).FieldType;
                value = (field as FieldInfo).GetValue(obj);
            }
            else if (field is PropertyInfo)
            {

                fieldType = (field as PropertyInfo).PropertyType;
                showType = (field as PropertyInfo).PropertyType;
                value = (field as PropertyInfo).GetValue(obj);
            }
            if (typeof(Delegate).IsAssignableFrom(fieldType)) return;


            var newValue = value;
            var name = field.Name;
            var attributes = field.GetCustomAttributes();
            SpaceAttribute space = attributes.FirstOrDefault(x => x is SpaceAttribute) as SpaceAttribute;
            if (space != null)
            {
                GUILayout.Space(space.height);
            }
            HeaderAttribute header = attributes.FirstOrDefault(x => x is HeaderAttribute) as HeaderAttribute;
            if (header != null)
                GUILayout.Label(header.header, EditorStyles.boldLabel);
            RangeAttribute range = attributes.FirstOrDefault(x => x is RangeAttribute) as RangeAttribute;
            MultilineAttribute mutiline = attributes.FirstOrDefault(x => x is MultilineAttribute) as MultilineAttribute;

            if (range != null && fieldType == typeof(float))
                newValue = DrawRange(name, (float)value, range.min, range.max);
            else if (mutiline != null && fieldType == typeof(string))
                newValue = DrawMutiLine(name, (string)value, mutiline.lines);
            else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type elementType = fieldType.GetGenericArguments()[0];
                IList array = (IList)value;
                if (array == null)
                    array = Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType)) as IList;
                var fold = GetFoldout(value);
                var result = DrawArr(ref fold, name, array, elementType);
                array.Clear();
                SetFoldout(array, fold);
                for (int i = 0; i < result.Count; i++)
                    array.Add(result[i]);
                newValue = array;
            }
            // 处理数组类型
            else if (fieldType.IsArray)
            {

                Type elementType = fieldType.GetElementType();
                Array array = (Array)value;

                if (array == null)
                    array = Array.CreateInstance(elementType, 0);
                var fold = GetFoldout(value);
                var result = DrawArr(ref fold, name, array, elementType);
                Array.Clear(array, 0, array.Length);
                if (array.Length != result.Count)
                    array = Array.CreateInstance(elementType, result.Count);
                SetFoldout(array, fold);

                for (int i = 0; i < result.Count; i++)
                    array.SetValue(result[i], i);
                newValue = array;
            }
            else if (IsBaseType(fieldType))
            {
                newValue = DrawBase(value, name, fieldType);
            }
            else
                newValue = DrawObj(value, name, fieldType);

            if (value != newValue)
            {
                if (field is FieldInfo)
                    (field as FieldInfo).SetValue(obj, newValue);
                else if ((field as PropertyInfo).CanWrite)
                    (field as PropertyInfo).SetValue(obj, newValue);
            }
        }

        public static bool GetFoldout(object obj)
        {
            if (obj == null) return false;
            if (!_unfoldDictionary.TryGetValue(obj.GetHashCode(), out var value))
            {
                _unfoldDictionary[obj.GetHashCode()] = false;
            }

            return value;
        }

        public static void SetFoldout(object obj, bool unfold)
        {
            if (obj == null) return;
            _unfoldDictionary[obj.GetHashCode()] = unfold;
        }





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
                    var match = Regex.Match(stackTrack, @">.+:([0-9]+)</a>");
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

    }
}

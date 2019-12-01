/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-05-31
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace IFramework
{
    [AttributeUsage(AttributeTargets.Method,AllowMultiple =false,Inherited =false)]
    public class SceneViewMenuItemAttribute : Attribute
    {
        public SceneViewMenuItemAttribute(string path)
        {
            this.menuItem = path;
        }
        public string menuItem { get; private set; }
        public bool check;
        public int priority;
    }
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class SceneViewMenuItemCheckAttribute : Attribute
    {
        public string menuItem { get; private set; }
        public SceneViewMenuItemCheckAttribute(string path)
        {
            this.menuItem = path;
        }
    }
    public class SceneViewEvent
	{
        public static int displayIndex { get; set; }
        public static Vector2 mousePosition { get; set; }
        public static Vector2 delta { get; set; }
        public static bool shift { get; set; }
        public static bool control { get; set; }
        public static bool command { get; set; }
        public static KeyCode keyCode { get; set; }
        public static bool capsLock { get; set; }
        public static bool numeric { get; set; }
        public static bool functionKey { get; private set; }
        public static bool isKey { get; private set; }
        public static bool alt { get; set; }
        public static string commandName { get; set; }
        public static int clickCount { get; set; }
        public static bool isMouse { get; private set; }
        public static float pressure { get; set; }
        public static EventModifiers modifiers { get; set; }
        public static int button { get; set; }
        public static EventType type { get; set; }
        public static char character { get; set; }
        public static bool isScrollWheel { get; private set; }

        [InitializeOnLoadMethod]
        private static void SceneEve()
        {
#if UNITY_2019_1_OR_NEWER
			SceneView.duringSceneGui -= OnSceneFunc;
			SceneView.duringSceneGui += OnSceneFunc;
#else
            SceneView.onSceneGUIDelegate -= OnSceneFunc;
            SceneView.onSceneGUIDelegate += OnSceneFunc;
#endif

            ViewEventInit();
            _sceneViewPickingClass = Type.GetType("UnityEditor.SceneViewPicking,UnityEditor");
            _getAllOverlapping = _sceneViewPickingClass.GetMethod("GetAllOverlapping", BindingFlags.Static | BindingFlags.NonPublic);
        }
        private static void ViewEventInit()
        {
            List<MethodInfo> methodInfos = new List<MethodInfo>();
            AppDomain.CurrentDomain.GetAssemblies().SelectMany((assembly) => { return assembly.GetTypes(); }).ForEach((type) => {

                var methods = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                    .ToList()
                    .FindAll((m) => { return m.IsDefined(typeof(SceneViewMenuItemAttribute), false); });
                methodInfos.AddRange(methods);
            });
            methodInfos.Sort((m1, m2) => {
                int priority1 = (m1.GetCustomAttributes(typeof(SceneViewMenuItemAttribute), false).First() as SceneViewMenuItemAttribute).priority;
                int priority2 = (m2.GetCustomAttributes(typeof(SceneViewMenuItemAttribute), false).First() as SceneViewMenuItemAttribute).priority;
                return priority1.CompareTo(priority2);
            });
            dic = methodInfos.ToDictionary((m) =>
            {
                return (m.GetCustomAttributes(typeof(SceneViewMenuItemAttribute), false).First() as SceneViewMenuItemAttribute).menuItem;
            });



            List<MethodInfo> checkIethodInfos = new List<MethodInfo>();
            AppDomain.CurrentDomain.GetAssemblies().SelectMany((assembly) => { return assembly.GetTypes(); }).ForEach((type) => {

                var methods = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
                    .ToList()
                    .FindAll((m) => { return m.IsDefined(typeof(SceneViewMenuItemCheckAttribute), false); })
                    .FindAll((m) => { return m.ReturnType == typeof(bool); });
                checkIethodInfos.AddRange(methods);
            });
            //checkDic = checkIethodInfos.ToDictionary((m) =>
            //{
            //    return (m.GetCustomAttributes(typeof(SceneViewMenuItemCheckAttribute), false).First() as SceneViewMenuItemCheckAttribute).menuItem;
            //});

        }
        private static Dictionary<string, MethodInfo> dic;
        //private static Dictionary<string, MethodInfo> checkDic;
        private static void OnSceneFunc(SceneView sceneView)
        {
             Event e = Event.current;
             type = e.type;
             displayIndex = e.displayIndex;
             mousePosition = e.mousePosition;
             delta = e.delta;
             shift = e.shift;
             control = e.control;
             command = e.command;
             keyCode = e.keyCode;
             capsLock = e.capsLock;
             numeric = e.numeric;
             functionKey = e.functionKey;
             isKey = e.isKey;
             alt = e.alt;
             commandName = e.commandName;
             clickCount = e.clickCount;
             isMouse = e.isMouse;
             pressure = e.pressure;
             modifiers = e.modifiers;
             button = e.button;
             character = e.character;
             isScrollWheel = e.isScrollWheel;

            ViewEvent(e);
        }
        private static void ViewEvent(Event e)
        {
            if (e.modifiers== EventModifiers.Control && e.isMouse && e.button == 1 && e.clickCount==1)
            {
                OverlappingGameObjects = GetAllOverlapping(e.mousePosition).ToList();

                List<GUIContent> contents = new List<GUIContent>();
                foreach (var item in dic.Keys)
                {
                    contents.Add(new GUIContent(item));
                }
                GenericMenu generic = new GenericMenu();

                generic.DropDown(new Rect(e.mousePosition, Vector2.one));
                EditorUtility.DisplayCustomMenu(new Rect(e.mousePosition, Vector2.one), contents.ToArray(),
                //    (index)=> {
                //    string path = contents[index].text;
                //    if (!(dic[path].GetCustomAttributes(typeof(SceneViewMenuItemAttribute), false).First() as SceneViewMenuItemAttribute).check)
                //        return true;
                //    if (!checkDic.ContainsKey(path)) return true;
                //    return  (bool)checkDic[path].Invoke(null, null);
                //}, 
                    -1, ViewEventCallBack, e);
            }
        }
        private static void ViewEventCallBack(object userData, string[] options, int selected)
        {
            string path = options[selected];
            if (dic.ContainsKey(path))
            {
                dic[path].Invoke(null,null);
            }

        }
        public static List<GameObject> OverlappingGameObjects;
        public static IEnumerable<GameObject> GetAllOverlapping(Vector2 position)
        {
            return (IEnumerable<GameObject>)_getAllOverlapping.Invoke(null, new object[] { position });
        }
        private static MethodInfo _getAllOverlapping;
        private static Type _sceneViewPickingClass;



        public static Vector3 MouseWoldPositionInScene(SceneView sceneView,int depth=20)
        {
            float mult = 1;
#if UNITY_5_4_OR_NEWER
            mult = EditorGUIUtility.pixelsPerPoint;
#endif
            Vector2 smousPos = mousePosition;
            // 转换成摄像机可接受的屏幕坐标，左下角是（0，0，0）右上角是（camera.pixelWidth，camera.pixelHeight，0）
            smousPos.y = sceneView.camera.pixelHeight - smousPos.y * mult;
            smousPos.x *= mult;

            Vector3 result = smousPos;
            // 近平面往里一些，才能看得到摄像机里的位置
            result.z = depth;
            return sceneView.camera.ScreenToWorldPoint(result); ;
        }

       
    }
}

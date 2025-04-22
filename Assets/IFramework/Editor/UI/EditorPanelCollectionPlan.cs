/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2020-01-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using static IFramework.UI.UIModuleWindow;

namespace IFramework.UI
{
    [System.Serializable]
    class EditorPanelCollectionPlan
    {
        public string ConfigGenPath;
        public string PanelCollectPath;
        public string ScriptGenPath;
        public string ScriptName;
        public string name;
        public string collectionJsonPath => ConfigGenPath.CombinePath($"{ConfigName}.json");


        private static string[] _typeNames, _shortTypes;
        public static string[] typeNames
        {
            get
            {
                if (_typeNames == null)
                    Enable();
                return _typeNames;
            }
        }
        public static string[] shortTypes
        {
            get
            {
                if (_shortTypes == null)
                    Enable();
                return _shortTypes;
            }
        }
        public static Type[] __types;
        public static Type[] types
        {
            get
            {
                if (__types == null)
                {
                    Enable();
                }
                return __types;
            }
        }
        public int typeIndex;
        public static Type baseType = typeof(UIGenCode);
        public string ConfigName;

        private static void Enable()
        {
            var list = EditorTools.GetSubTypesInAssemblies(baseType)
           .Where(type => !type.IsAbstract);
            __types = list.ToArray();
            _typeNames = list.Select(type => type.FullName).ToArray();
            _shortTypes = list.Select(type => type.Name).ToArray();
        }
        public Type GetSelectType()
        {
            typeIndex = Mathf.Clamp(typeIndex, 0, typeNames.Length);
            var type_str = typeNames[typeIndex];
            Type type = types.FirstOrDefault(x => x.FullName == type_str);

            return type;
        }


    }
}

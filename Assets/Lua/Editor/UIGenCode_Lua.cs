﻿/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2020-01-13
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using IFramework.UI;
using static IFramework.UI.UIModuleWindow;
using static IFramework.UI.UIModuleWindow.UIGenCodeCS;
using static IFramework.UI.UIModuleWindow.UICollectData;

namespace IFramework.Lua
{
    [Serializable]

    class UIGenCode_Lua : IFramework.UI.UIModuleWindow.UIGenCode<GameObject>
    {

        public override string name => "Lua";
        protected override GameObject gameObject => panel;
        [SerializeField] private string config_path;
        [SerializeField] private ItemType _type;
        protected override string GetFieldCode(string source, string fieldType, string fieldName)
        {
            return string.Empty;
        }
        protected override string GetFindFieldCode(string source, string fieldType, string fieldName, string path)
        {
            string type = fieldType.Split('.').Last();

            return $"\t\t---@type {fieldType}\n" +
             $"\t\t{fieldName} = self:GetComponent(" + path + $", typeof({type})),\n";
        }
        public override void GenPanelNames(PanelCollection collect, string scriptGenPath, string scriptName)
        {
            var datas = collect.datas;

            string s = $"{scriptName} = \n" +
                "{\n";
            foreach (var data in datas)
            {
                s += $"\t{data.name} = \"{data.path}\",\n";
            }


            s += "}\n";
            if (config != null)
            {
                s += $"{scriptName}.map =\n{{\n";
                foreach (var data in datas)
                {
                    var seg = ScriptPathCollection.GetSeg(data);
                    if (!System.IO.File.Exists(seg.ScriptPath)) continue;
                    var require = seg.ScriptPath.Remove(0, config.rootPath.Length + 1).Replace("/", ".").Replace(".lua.txt", "");
                    s += $"\t[{scriptName}.{data.name}] = require \"{require}\",\n";

                }
                s += "}\n";
            }
            File.WriteAllText(System.IO.Path.Combine(scriptGenPath, $"{scriptName}.lua.txt"), s);
            AssetDatabase.Refresh();
        }
        protected override string GetScriptTemplate()
        {

            string GetBase()
            {
                if (_type == ItemType.UIItem)
                    return "UIItemView";
                else if (_type == ItemType.UI)
                    return "UIView";
                else if (_type == ItemType.GameObject)
                    return "GameObjectView";

                return string.Empty;
            }
            string _base = GetBase();
            string add = _type != ItemType.UIItem ? "\tself:SetGameObject(gameObject)\n" : "";
            string ctor()
            {
                if (_type == ItemType.UIItem)
                    return string.Empty;
                else if (_type == ItemType.UI)
                    return $"function {ScriptName}:ctor(gameObject)\n" +
  "\tself:SetGameObject(gameObject)\n" +
 "end\n\n";


                else if (_type == ItemType.GameObject)
                    return $"function {ScriptName}:ctor(gameObject)\n" +
            "\tself:SetGameObject(gameObject)\n" +
           "end\n\n";

                return string.Empty;
            }
            string MoreFunction()
            {
                if (_type == ItemType.UIItem)
                {
                    return $"function {ScriptName}:OnGet()\n" +
                        "end\n\n" +
                        $"function {ScriptName}:OnSet()\n" +
                          $" \tself:DisposeEvents()\n " +
                            $" \tself:DisposeUIEvents()\n" +
                        "end\n\n";
                }
                else if (_type == ItemType.UI)
                {
                    return $"function {ScriptName}:OnShow()\n\n" +
                            "end\n\n" +
                            $"function {ScriptName}:OnHide()\n\n" +
                            "end\n\n" +
                            $"function {ScriptName}:OnClose()\n" +
                            $" \tself:DisposeEvents()\n " +
                            $" \tself:DisposeUIEvents()\n" +
                            $"\t{ctrls} = nil\n" +
                            "end\n\n" +
                             $"function {ScriptName}:OnLoad()\n" +
                            "end\n\n";
                }
                return string.Empty;
            }

            string target = "--*********************************************************************************\n" +
          $"--Author:         {Author}\n" +
          $"--Version:        {Version}\n" +
          $"--UnityVersion:   {UnityVersion}\n" +
          $"--Date:           {Date}\n" +
          "--*********************************************************************************\n" +

            $"{StaticUsing}\n" +
           $"{StaticUsing}\n" +
           $"---@class {ScriptName} : {_base}\n" +
           $"local {ScriptName} = class(\"{ScriptName}\",{_base})\n\n" +
           $"{ctor()}" +
           $"function {ScriptName}:InitComponents()\n" +

           $"\t{ctrls} = {{\n" +
            $"--{InitComponentsStart}\n" +
            $"{FindField}\n" +
            $"--{InitComponentsEnd}\n" +
           "\t}\n" +
           " end\n" +
           $"{MoreFunction()}\n return {ScriptName}";


            return target;
        }
        protected override string GetViewScriptName(string viewName) => $"{viewName}.lua.txt";





        protected override void OnFindDirSuccess()
        {
            string txt = File.ReadAllText(scriptPath);
            var names = Enum.GetNames(typeof(UIGenCodeCS.ItemType));
            foreach (var item in names)
            {
                if (txt.Contains($": {item}"))
                {
                    _type = (ItemType)Enum.Parse(typeof(ItemType), item);
                    break;
                }
            }
        }
        protected override void OnFindDirFail()
        {
            UIPanel find = gameObject.GetComponent<UIPanel>();
            if (find != null)
            {
                _type = ItemType.UI;
            }
            else
            {
                if (_type == ItemType.UI)
                    _type = ItemType.GameObject;
            }
        }
        protected override void LoadLastData(UIGenCode<GameObject> _last)
        {
            var last = _last as UIGenCode_Lua;
            this._type = last._type;
            this.config_path = last.config_path;
        }

        LuaEnvConfig cache;

        LuaEnvConfig config
        {
            get
            {
                if (cache != null) return cache;
                if (config_path == string.Empty)
                    return null;
                return AssetDatabase.LoadAssetAtPath<LuaEnvConfig>(config_path);
            }
            set
            {
                if (value != null)
                {
                    config_path = AssetDatabase.GetAssetPath(value);
                    cache = value;
                }
                else
                {
                    config_path = string.Empty;
                    cache = null;
                }

            }

        }

        protected override void Draw()
        {
            _type = (ItemType)EditorGUILayout.EnumPopup("Type", _type);
            config = EditorGUILayout.ObjectField("Config", config, typeof(LuaEnvConfig), false) as LuaEnvConfig;
        }

        public const string StaticUsing = "---StaticUsing";
        public const string ctrls = "self.view";
        protected override string OverwriteWriteFile(ScriptCreator creator, string source)
        {
            source = source.Replace(".lua", "");
            var static_use = StaticUse(creator);
            var start = source.IndexOf(StaticUsing) + StaticUsing.Length + 1;
            var end = source.LastIndexOf(StaticUsing);
            var first = source.Substring(0, start) + static_use;
            return first.Replace("\r\n\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n").Replace("\r\n", "\n") + source.Substring(end);
        }
        static string StaticUse(ScriptCreator creator)
        {
            List<string> usings = new List<string>();
            var marks = creator.GetMarks();
            if (marks != null)
            {
                for (int i = 0; i < marks.Count; i++)
                {
                    string ns = marks[i].fieldType;
                    if (!usings.Contains(ns))
                    {
                        usings.Add(ns);
                    }
                }
            }
            string use = "";
            for (int i = 0; i < usings.Count; i++)
            {
                use += $"local {usings[i].Split('.').Last()} = StaticUsing(\"{usings[i]}\")\n";
            }
            return use;
        }

        public override string GetScriptFitter() => "t:TextAsset";
    }

}

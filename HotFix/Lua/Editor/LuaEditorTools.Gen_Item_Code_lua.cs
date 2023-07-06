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
using System;
using System.IO;
using IFramework.UI;

namespace IFramework.Hotfix.Lua
{
    static partial class LuaEditorTools
    {
        [Serializable]

        class Gen_Item_Code_lua : UIMoudleWindow.UIGenCode<GameObject>
        {
            public enum ItemType
            {
                UIItem,
                LuaObject,
                UIObject,
            }
            private const string key = "Gen_Item_Code_lua";
            public override string name => "Lua/Gen_Item_Code_lua";
            protected override GameObject gameobject => panel.gameObject;

            protected override string viewScriptName => $"{panelName}.lua.txt";
            [SerializeField] private ItemType _type;


            protected override void OnFindDirSuccess()
            {
                string txt = File.ReadAllText(UIdir.CombinePath(viewScriptName));
                var names = Enum.GetNames(typeof(ItemType));
                foreach (var item in names)
                {
                    if (txt.Contains($": {item}"))
                    {
                        _type = (ItemType)Enum.Parse(typeof(ItemType), item);
                        break;
                    }
                }
            }

            protected override void LoadLastData()
            {
                var last = EditorTools.GetFromPrefs<Gen_Item_Code_lua>(key);
                if (last != null)
                {
                    this.UIdir = last.UIdir;
                    this.panel = last.panel;
                    this._type = last._type;
                    this.state = last.state;
                }
            }

            protected override void WriteView()
            {
                string path = UIdir.CombinePath(viewScriptName);

                if (File.Exists(path))
                {
                    MVC_GenCodeView_Lua.UpdateLua(creater, $"function {viewName}:ctor(gameObject)", path);
                }
                else
                {
                    string source = vSource;
                    if (_type == ItemType.LuaObject)
                        source = ObjSource;
                    if (_type == ItemType.UIObject)
                        source = UISource;
                    MVC_GenCodeView_Lua.WriteLua(creater, path, viewName, source);
                }
            }


            protected override void Draw()
            {
                _type = (ItemType)EditorGUILayout.EnumPopup("Type", _type);

            }



            private static string Get_base(ItemType type)
            {
                string _base = type.ToString();
                string add = type != ItemType.UIItem ? "\tself:SetGameObject(gameObject)\n" : "";
                return
MVC_GenCodeView_Lua.head + "\n" +
$"{MVC_GenCodeView_Lua.ViewUseFlagFT}\n" +
MVC_GenCodeView_Lua.ViewUseFlag + "\n\n" +
$"{MVC_GenCodeView_Lua.ViewUseFlagFT}\n" +
$"---@class {MVC_GenCodeView_Lua.ViewNameFlag} : {_base}\n" +
$"local {MVC_GenCodeView_Lua.ViewNameFlag} = class(\"{MVC_GenCodeView_Lua.ViewNameFlag}\",{_base})\n\n" +
$"function {MVC_GenCodeView_Lua.ViewNameFlag}:ctor(gameObject)\n" +
 add +
"\tself.Controls = {\n" +
MVC_GenCodeView_Lua.ViewFeildFlag + "\n" +
"\t}\n" +
"end\n\n";
            }


            static string vSource
            {
                get
                {
                    return Get_base(ItemType.UIItem) +
    $"function {MVC_GenCodeView_Lua.ViewNameFlag}:OnGet()\n" +
    "end\n\n" +
    $"function {MVC_GenCodeView_Lua.ViewNameFlag}:OnSet()\n\n" +
    "end\n\n" +
    $"return {MVC_GenCodeView_Lua.ViewNameFlag}";
                }
            }



            static string ObjSource
            {
                get
                {
                    return Get_base(ItemType.LuaObject) + "return " + MVC_GenCodeView_Lua.ViewNameFlag;
                }
            }
            static string UISource
            {
                get
                {
                    return Get_base(ItemType.UIObject) + "return " + MVC_GenCodeView_Lua.ViewNameFlag;

                }
            }


        }

    }


}


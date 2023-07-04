/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.319
 *UnityVersion:   2018.4.17f1
 *Date:           2020-06-04
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
namespace IFramework.Hotfix.Lua
{
	public class XluaMain: IXLuaDisposable
    {
        private readonly XLuaModule module;

        public XluaMain(XLuaModule module)
		{
            module.Subscribe(this);
#if UNITY_EDITOR
            module.DoString("require 'UpdateFunctions'");
#endif

            module.DoString("require 'Main'" +
			                 " Awake()");
            this.module = module;
        }

		public void LuaDispose()
		{
            module.DoString("require 'Main'" +
			                 "OnDispose()");
            module.UnSubscribe(this);

        }
    }
}

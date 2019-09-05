/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-15
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using XLua;
using UnityEngine;
namespace IFramework
{
    [Hotfix]
	public class HotFixExample:MonoBehaviour
	{
        private int index;
        private void Update()
        {
            Log.E(index++ % 2 + "   CS");
            if (Input.GetKeyDown(KeyCode.Space))
            {
                XLuaEnvironment.DoString(@"xlua.hotfix(CS.IFramework.HotFixExample,'Update',
                                    function(self)
                                        self.index=self.index+1
                                        print(self.index%2)
                                    end)");
            }
        }

    }
}

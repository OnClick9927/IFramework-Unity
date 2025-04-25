/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2021.3.33f1c1
 *Date:           2024-10-24
*********************************************************************************/
using IFramework.UI;
namespace IFramework
{
    public abstract class TestWidgetBase : GameObjectView
    {

    }
    public abstract class TestViewBase : UIView
    {
        protected override void OnClearFields()
        {
            base.OnClearFields();
            this.KillTweens();
            this.KillTimers();
            
        }
        protected override void AfterOnClose()
        {
      
        }
    }
}
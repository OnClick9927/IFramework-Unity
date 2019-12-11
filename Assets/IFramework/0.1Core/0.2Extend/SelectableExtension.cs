/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-08
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine.UI;

namespace IFramework
{
    public static partial class SelectableExtension
    {
        public static T EnableInteract<T>(this T selfSelectable) where T :Selectable
        {
            selfSelectable.interactable = true;
            return selfSelectable;
        }

        public static T DisableInteract<T>(this T selfSelectable) where T : Selectable
        {
            selfSelectable.interactable = false;
            return selfSelectable;
        }
        
        public static T CancalAllTransitions<T>(this T selfSelectable) where T :Selectable
        {
            selfSelectable.transition = Selectable.Transition.None;
            return selfSelectable;
        } 
    }
}
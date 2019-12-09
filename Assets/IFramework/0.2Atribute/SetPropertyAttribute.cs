/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-05-08
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace IFramework
{
    public class SetPropertyAttribute : PropertyAttribute
    {
        public string Name { get; private set; }
        public bool IsDirty { get; set; }

        public SetPropertyAttribute(string name)
        {
            this.Name = name;
        }
    }
}

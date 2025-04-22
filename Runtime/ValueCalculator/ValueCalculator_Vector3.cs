/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;

namespace IFramework
{
    class ValueCalculator_Vector3 : ValueCalculator<Vector3>
    {




        public override Vector3 Minus(Vector3 value, Vector3 value2) => value - value2;

        public override Vector3 Add(Vector3 value, Vector3 value2) => value + value2;
        public override Vector3 Dev(Vector3 value, float dev) => value / dev;

        public override Vector3 Multi(Vector3 value, float dev) => value * dev;



        public override Vector3 Snap(Vector3 value)
        {
            value.x = Mathf.RoundToInt(value.x);
            value.y = Mathf.RoundToInt(value.y);
            value.z = Mathf.RoundToInt(value.z);
            return value;
        }

        public override Vector3 Lerp(Vector3 start, Vector3 end, float percent) => Vector3.Lerp(start, end, percent);

        public override Vector3 MultiStrength(Vector3 strength, Vector3 value) => new Vector3(strength.x * value.x, strength.y * value.y, strength.z * value.z);

        public override Vector3 RangeValue(Vector3 value)
        {
            value.x *= Range();
            value.y *= Range();
            value.z *= Range();
            return value;
        }
    }


}
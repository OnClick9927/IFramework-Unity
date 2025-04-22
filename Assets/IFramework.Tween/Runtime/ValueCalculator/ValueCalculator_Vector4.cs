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
    class ValueCalculator_Vector4 : ValueCalculator<Vector4>
    {
        public override Vector4 Dev(Vector4 value, float dev) => value / dev;

        public override Vector4 Multi(Vector4 value, float dev) => value * dev;

        public override Vector4 Minus(Vector4 value, Vector4 value2) => value - value2;

        public override Vector4 Add(Vector4 value, Vector4 value2) => value + value2;

        public override Vector4 Snap(Vector4 value)
        {
            value.x = Mathf.RoundToInt(value.x);
            value.y = Mathf.RoundToInt(value.y);
            value.z = Mathf.RoundToInt(value.z);
            value.w = Mathf.RoundToInt(value.w);
            return value;
        }

        public override Vector4 Lerp(Vector4 start, Vector4 end, float percent) => Vector4.Lerp(start, end, percent);

        public override Vector4 MultiStrength(Vector4 strength, Vector4 value) => new Vector4(strength.x * value.x, strength.y * value.y, strength.z * value.z, strength.w * value.w);

        public override Vector4 RangeValue(Vector4 value)
        {
            value.x *= Range();
            value.y *= Range();
            value.z *= Range();
            value.w *= Range();
            return value;
        }
    }


}
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
    class ValueCalculator_Vector2 : ValueCalculator<Vector2>
    {
        public override Vector2 Minus(Vector2 value, Vector2 value2) => value - value2;

        public override Vector2 Add(Vector2 value, Vector2 value2) => value + value2;
        public override Vector2 Dev(Vector2 value, float dev) => value / dev;

        public override Vector2 Multi(Vector2 value, float dev) => value * dev;

        public override Vector2 Snap(Vector2 value)
        {
            value.x = Mathf.RoundToInt(value.x);
            value.y = Mathf.RoundToInt(value.y);
            return value;
        }

        public override Vector2 Lerp(Vector2 start, Vector2 end, float percent) => Vector2.Lerp(start, end, percent);

        public override Vector2 MultiStrength(Vector2 strength, Vector2 value) => new Vector2(strength.x * value.x, strength.y * value.y);

        public override Vector2 RangeValue(Vector2 value)
        {
            value.x *= Range();
            value.y *= Range();
            return value;
        }
    }


}
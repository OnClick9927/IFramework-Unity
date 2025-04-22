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
    class ValueCalculator_Color : ValueCalculator<Color>
    {

        public override Color Minus(Color value, Color value2) => value - value2;

        public override Color Add(Color value, Color value2) => value + value2;

        public override Color Dev(Color value, float dev) => value / dev;

        public override Color Multi(Color value, float dev) => value * dev;

        public override Color Snap(Color value)
        {
            value.a = Mathf.RoundToInt(value.a);
            value.r = Mathf.RoundToInt(value.r);
            value.g = Mathf.RoundToInt(value.g);
            value.b = Mathf.RoundToInt(value.b);
            return value;
        }

        public override Color Lerp(Color start, Color end, float percent) => Color.Lerp(start, end, percent);

        public override Color MultiStrength(Color strength, Color value) => new Color(strength.r * value.r, strength.g * value.g, strength.b * value.b, strength.a * value.a);

        public override Color RangeValue(Color value)
        {
            value.r *= Range();
            value.g *= Range();
            value.b *= Range();
            value.a *= Range();
            return value;
        }
    }


}
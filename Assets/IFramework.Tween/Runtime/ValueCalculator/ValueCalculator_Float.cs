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
    class ValueCalculator_Float : ValueCalculator<float>
    {

        public override float Minus(float value, float value2) => value - value2;

        public override float Add(float value, float value2) => value + value2;

        public override float Dev(float value, float dev) => value / dev;

        public override float Multi(float value, float dev) => value * dev;

        public override float Snap(float value) => Mathf.RoundToInt(value);

        public override float Lerp(float start, float end, float percent) => Mathf.Lerp(start, end, percent);

        public override float MultiStrength(float strength, float value) => strength * value;

        public override float RangeValue(float value) => value * Range();
    }


}
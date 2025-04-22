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
    class ValueCalculator_Rect : ValueCalculator<Rect>
    {
        public override Rect Minus(Rect value, Rect value2)
        {
            return new Rect(
       value.x - value2.x,
       value.y - value2.y,
       value.width - value2.width,
       value.height - value2.height);
        }

        public override Rect Add(Rect value, Rect value2)
        {
            return new Rect(
                value.x + value2.x,
                value.y + value2.y,
                value.width + value2.width,
                value.height + value2.height);
        }


        public override Rect Dev(Rect value, float dev) => new Rect(value.x / dev, value.y / dev, value.width / dev, value.height / dev);

        public override Rect Multi(Rect value, float dev) => new Rect(value.x * dev, value.y * dev, value.width * dev, value.height * dev);

        public override Rect Snap(Rect value)
        {
            value.x = Mathf.RoundToInt(value.x);
            value.y = Mathf.RoundToInt(value.y);
            value.width = Mathf.RoundToInt(value.width);
            value.height = Mathf.RoundToInt(value.height);
            return value;
        }

        public override Rect Lerp(Rect a, Rect b, float t)
        {
            Rect r = Rect.zero;
            r.x = Mathf.Lerp(a.x, b.x, t);
            r.y = Mathf.Lerp(a.y, b.y, t);
            r.width = Mathf.Lerp(a.width, b.width, t);
            r.height = Mathf.Lerp(a.height, b.height, t);
            return r;
        }

        public override Rect MultiStrength(Rect strength, Rect end)
        {
            return new Rect(strength.x * end.x, strength.y * end.y, strength.width * end.width, strength.height * end.height);

        }

        public override Rect RangeValue(Rect value)
        {
            value.x *= Range();
            value.y *= Range();
            value.width *= Range();
            value.height *= Range();
            return value;
        }
    }


}
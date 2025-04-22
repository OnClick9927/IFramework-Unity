/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System.Collections.Generic;
using UnityEngine;

namespace IFramework
{
    abstract class ValueCalculator<T>
    {

        public T Calculate(TweenType mode, T start, T end, float percent, T srcValue,
            float srcPercent, bool snap, T strength, int frequency, float dampingRatio, int jumpCount, float jumpDamping,
            T[] points, int pointsLength)
        {
            T dest = default(T);
            if (mode == TweenType.Bezier || mode == TweenType.Array)
            {
                if (percent == 1)
                    dest = points[pointsLength - 1];
                else
                {
                    if (mode == TweenType.Bezier)
                    {
                        float[] tempCoefficients = EvaluateBezier(percent, pointsLength);
                        for (int i = 0; i < pointsLength; i++)
                            dest = Add(dest, Multi(points[i], tempCoefficients[i]));
                        CycleArray(tempCoefficients);
                    }
                    else
                    {
                        T _start, _end; float _percent;
                        EvaluateArray(percent, points, pointsLength, out _start, out _end, out _percent);
                        dest = Lerp(_start, _end, _percent);
                    }
                }
            }
            else
                dest = Lerp(start, end, percent);


            dest = Lerp(srcValue, dest, srcPercent);


            if (mode == TweenType.Shake || mode == TweenType.Punch)
            {
                strength = MultiStrength(strength, end);
                var s = Multi(strength, EvaluateStrength(frequency, dampingRatio, percent));
                if (mode == TweenType.Punch)
                    dest = Add(dest, s);
                else
                {
                    s = Multi(s, percent);
                    dest = Add(dest, RangeValue(s));
                }
            }
            else if (mode == TweenType.Jump)
            {
                int devCount;
                float jumpAdd;
                EvaluateJump(jumpCount, percent, out devCount, out jumpAdd);
                for (int i = 0; i < devCount; i++)
                    strength = Dev(strength, jumpDamping);
                dest = Add(dest, Multi(strength, jumpAdd));
            }
            if (snap)
                dest = Snap(dest);
            return dest;


        }

        public abstract T Snap(T value);
        public abstract T Lerp(T start, T end, float percent);
        public abstract T Minus(T value, T value2);
        public abstract T Add(T value, T value2);
        public abstract T Dev(T value, float dev);
        public abstract T Multi(T value, float dev);
        public abstract T MultiStrength(T strength, T value);
        public abstract T RangeValue(T value);









        protected static float Range() => UnityEngine.Random.Range(-1, 1);

        const float E = 2.71828175F;
        const float PI = 3.14159274F;

        private static Dictionary<int, float[]> arrays = new Dictionary<int, float[]>();
        private static float[] AllocateArray(int length)
        {
            var min = 16;
            while (min < length)
                min *= 2;
            float[] result = null;
            if (arrays.TryGetValue(min, out result))
            {
                return result;
            }
            return new float[min];
        }
        private static void CycleArray(float[] arr)
        {
            arrays[arr.Length] = arr;
        }
        private static float[] EvaluateBezier(float percent, int length)
        {
            float u = 1f - percent;

            // 使用Bernstein多项式递推关系优化计算
            float[] tempCoefficients = AllocateArray(length);
            tempCoefficients[0] = Mathf.Pow(u, length - 1);

            for (int i = 1; i < length; i++)
                tempCoefficients[i] = tempCoefficients[i - 1] * (percent / u) * (length - i) / i;

            return tempCoefficients;
        }
        private static void EvaluateArray(float percent, T[] array, int length, out T start, out T end, out float _percent)
        {
            var temp = percent * (length - 1);
            var floor = Mathf.FloorToInt(temp);
            _percent = temp - floor;
            start = array[floor];

            end = array[floor + 1];
        }
        private static void EvaluateJump(int jumpCount, float percent, out int devCount, out float jumpAdd)
        {
            if (percent == 0 || percent == 1)
            {
                jumpAdd = 0;
                devCount = 0;
                return;
            }

            var gap = 1f / jumpCount;
            var _percent = (percent % gap) * jumpCount;
            devCount = Mathf.FloorToInt(percent / gap);
            jumpAdd = (1 - (4 * Mathf.Pow(_percent - 0.5f, 2)));
        }
        private static float EvaluateStrength(int frequency, float dampingRatio, float t)
        {
            if (t == 1f || t == 0f)
            {
                return 0;
            }
            float angularFrequency = (frequency - 0.5f) * PI;
            float dampingFactor = dampingRatio * frequency / (2f * PI);
            return Mathf.Cos(angularFrequency * t) * Mathf.Pow(E, -dampingFactor * t);
        }
    }


}
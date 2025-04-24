/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.2.116
 *UnityVersion:   2018.4.24f1
 *Date:           2020-11-29
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using UnityEngine;

namespace IFramework
{
    struct EaseEvaluator : IValueEvaluator
    {
        public static EaseEvaluator Default = new EaseEvaluator(Ease.Linear);
        public Ease ease;
        public EaseEvaluator(Ease ease) => this.ease = ease;
        private static float Evaluate(Ease easeType, float time, float duration)
        {
            float percent = Mathf.Clamp01((time / duration));
            switch (easeType)
            {
                case Ease.Linear:
                    return percent;
                case Ease.InSine:
                    return -(float)Math.Cos((double)(percent * 1.5707964f)) + 1f;
                case Ease.OutSine:
                    return (float)Math.Sin((double)(percent * 1.5707964f));
                case Ease.InOutSine:
                    return -0.5f * ((float)Math.Cos((double)(3.1415927f * percent)) - 1f);
                case Ease.InQuad:
                    return percent * percent;
                case Ease.OutQuad:
                    return -(time /= duration) * (time - 2f);
                case Ease.InOutQuad:
                    if ((time /= duration * 0.5f) < 1f)
                    {
                        return 0.5f * time * time;
                    }
                    return -0.5f * ((time -= 1f) * (time - 2f) - 1f);
                case Ease.InCubic:
                    return (time /= duration) * time * time;
                case Ease.OutCubic:
                    return (time = percent - 1f) * time * time + 1f;
                case Ease.InOutCubic:
                    if ((time /= duration * 0.5f) < 1f)
                    {
                        return 0.5f * time * time * time;
                    }
                    return 0.5f * ((time -= 2f) * time * time + 2f);
                case Ease.InQuart:
                    return (time /= duration) * time * time * time;
                case Ease.OutQuart:
                    return -((time = percent - 1f) * time * time * time - 1f);
                case Ease.InOutQuart:
                    if ((time /= duration * 0.5f) < 1f)
                    {
                        return 0.5f * time * time * time * time;
                    }
                    return -0.5f * ((time -= 2f) * time * time * time - 2f);
                case Ease.InQuint:
                    return (time /= duration) * time * time * time * time;
                case Ease.OutQuint:
                    return (time = percent - 1f) * time * time * time * time + 1f;
                case Ease.InOutQuint:
                    if ((time /= duration * 0.5f) < 1f)
                    {
                        return 0.5f * time * time * time * time * time;
                    }
                    return 0.5f * ((time -= 2f) * time * time * time * time + 2f);
                case Ease.InExpo:
                    if (time != 0f)
                    {
                        return (float)Math.Pow(2.0, (double)(10f * (percent - 1f)));
                    }
                    return 0f;
                case Ease.OutExpo:
                    if (time == duration)
                    {
                        return 1f;
                    }
                    return -(float)Math.Pow(2.0, (double)(-10f * percent)) + 1f;
                case Ease.InOutExpo:
                    if (time == 0f)
                    {
                        return 0f;
                    }
                    if (time == duration)
                    {
                        return 1f;
                    }
                    if ((time /= duration * 0.5f) < 1f)
                    {
                        return 0.5f * (float)Math.Pow(2.0, (double)(10f * (time - 1f)));
                    }
                    return 0.5f * (-(float)Math.Pow(2.0, (double)(-10f * (time -= 1f))) + 2f);
                case Ease.InCirc:
                    return -((float)Math.Sqrt((double)(1f - (time /= duration) * time)) - 1f);
                case Ease.OutCirc:
                    return (float)Math.Sqrt((double)(1f - (time = percent - 1f) * time));
                case Ease.InOutCirc:
                    if ((time /= duration * 0.5f) < 1f)
                    {
                        return -0.5f * ((float)Math.Sqrt((double)(1f - time * time)) - 1f);
                    }
                    return 0.5f * ((float)Math.Sqrt((double)(1f - (time -= 2f) * time)) + 1f);

                default:
                    return -(time /= duration) * (time - 2f);
            }
        }
        float IValueEvaluator.Evaluate(float percent, float time, float duration) => Evaluate(ease, time, duration);
    }





}
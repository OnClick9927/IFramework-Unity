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
    struct AnimationCurveEvaluator : IValueEvaluator
    {
        public AnimationCurve curve;

        public AnimationCurveEvaluator(AnimationCurve curve) => this.curve = curve;
        float IValueEvaluator.Evaluate(float percent, float time, float duration) => curve.Evaluate(percent);
    }





}
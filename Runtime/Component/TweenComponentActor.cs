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
    [System.Serializable]
    public abstract class TweenComponentActor
    {
        internal abstract float percent { get; }
        internal Transform transform { get; set; }
        public enum CurveType
        {
            Ease,
            AnimationCurve,
        }
        public CurveType curveType = CurveType.Ease;
        public LoopType loopType = LoopType.Restart;
        public int loops = 1;
        public float delay = 0;
        public float duration = 1;
        public float sourceDelta = 0;
        public bool snap = false;
        public AnimationCurve curve = new AnimationCurve();
        public Ease ease;

        internal abstract ITweenContext Create();

        internal abstract void ResetPercent();
    }
    public abstract class TweenComponentActor<T, TTarget> : TweenComponentActor
    {
        [NonSerialized] private float _percent;
        internal sealed override float percent => _percent;
        internal sealed override void ResetPercent() => _percent = 0;
        internal override ITweenContext Create()
        {
            if (target == null)
                target = transform.GetComponent<TTarget>();

            if (target == null)
            {
                Log.FE($"Can not GetComponent<{typeof(TTarget)}>() from {transform.name}");
            }

            _percent = 0;
            var context = OnCreate();
            context.SetLoop(loopType, loops).SetDelay(delay).SetSnap(snap).SetDuration(duration).SetSourceDelta(sourceDelta);
            if (curveType == CurveType.Ease)
                context.SetEase(ease);
            else
                context.SetAnimationCurve(curve);
#if UNITY_EDITOR
            context.OnTick(OnTick);
            context.OnComplete(OnEnd);

#endif
            return context;
        }

        private void OnEnd(ITweenContext context) => _percent = 1;

        private void OnTick(ITweenContext context, float time, float delta) => _percent = time / duration;

        protected abstract ITweenContext<T, TTarget> OnCreate();

        public TTarget target;


    }

}
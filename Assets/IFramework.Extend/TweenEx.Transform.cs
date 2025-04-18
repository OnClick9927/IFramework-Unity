﻿/*********************************************************************************
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
    public static partial class TweenEx
    {
        enum TransformActSpace
        {
            World,
            Local
        }
        class DoScaleActor : TweenComponentActor<Vector3, Transform>
        {
            public Vector3 start = Vector3.one;
            public Vector3 end = Vector3.one;
            protected override ITweenContext<Vector3, Transform> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoLocalScale(end, duration, snap);
                return target.DoLocalScale(start, end, duration, snap);
            }
        }
        class DoPositionActor : TweenComponentActor<Vector3, Transform>
        {

            public TransformActSpace space;
            public Vector3 start = Vector3.zero;
            public Vector3 end = Vector3.one;
            protected override ITweenContext<Vector3, Transform> OnCreate()
            {
                if (space == TransformActSpace.Local)
                {
                    if (startType == StartValueType.Relative)
                        return target.DoLocalPosition(end, duration, snap);
                    return target.DoLocalPosition(start, end, duration, snap);
                }
                if (startType == StartValueType.Relative)
                    return target.DoPosition(end, duration, snap);
                return target.DoPosition(start, end, duration, snap);

            }
        }
        class DoRotateActor : TweenComponentActor<Vector3, Transform>
        {
            public TransformActSpace space;
            public Vector3 start = Vector3.zero;
            public Vector3 end = Vector3.one;

            protected override ITweenContext<Vector3, Transform> OnCreate()
            {
                if (space == TransformActSpace.Local)
                {
                    if (startType == StartValueType.Relative)
                        return target.DoLocalRotate(end, duration, snap);
                    return target.DoLocalRotate(start, end, duration, snap);
                }
                if (startType == StartValueType.Relative)
                    return target.DoRotate(end, duration, snap);
                return target.DoRotate(start, end, duration, snap);

            }
        }


        class DoShakePositionActor : DoPositionActor
        {
            public Vector3 strength = Vector3.one;
            public int frequency = 10;
            public float dampingRatio = 1;
            protected override ITweenContext<Vector3, Transform> OnCreate()
            {
                if (space == TransformActSpace.Local)
                {
                    if (startType == StartValueType.Relative)
                        return target.DoShakeLocalPosition(end, duration, strength, frequency, dampingRatio, snap);
                    return target.DoShakeLocalPosition(start, end, duration, strength, frequency, dampingRatio, snap);
                }
                if (startType == StartValueType.Relative)
                    return target.DoShakePosition(end, duration, strength, frequency, dampingRatio, snap);
                return target.DoShakePosition(start, end, duration, strength, frequency, dampingRatio, snap);

            }
        }
        class DoShakeRotationActor : DoRotateActor
        {
            public Vector3 strength = Vector3.one;
            public int frequency = 10;
            public float dampingRatio = 1;
            protected override ITweenContext<Vector3, Transform> OnCreate()
            {
                if (space == TransformActSpace.Local)
                {
                    if (startType == StartValueType.Relative)
                        return target.DoShakeLocalRotate(end, duration, strength, frequency, dampingRatio, snap);
                    return target.DoShakeLocalRotate(start, end, duration, strength, frequency, dampingRatio, snap);
                }
                if (startType == StartValueType.Relative)
                    return target.DoShakeRotate(end, duration, strength, frequency, dampingRatio, snap);
                return target.DoShakeRotate(start, end, duration, strength, frequency, dampingRatio, snap);

            }
        }
        class DoShakeScaleActor : DoScaleActor
        {
            public Vector3 strength = Vector3.one;
            public int frequency = 10;
            public float dampingRatio = 1;
            protected override ITweenContext<Vector3, Transform> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoShakeLocalScale(end, duration, strength, frequency, dampingRatio, snap);
                return target.DoShakeLocalScale(start, end, duration, strength, frequency, dampingRatio, snap);

            }
        }

        class DoPunchPositionActor : DoPositionActor
        {
            public Vector3 strength = Vector3.one;
            public int frequency = 10;
            public float dampingRatio = 1;
            protected override ITweenContext<Vector3, Transform> OnCreate()
            {
                if (space == TransformActSpace.Local)
                {
                    if (startType == StartValueType.Relative)
                        return target.DoPunchLocalPosition(end, duration, strength, frequency, dampingRatio, snap);
                    return target.DoPunchLocalPosition(start, end, duration, strength, frequency, dampingRatio, snap);
                }
                if (startType == StartValueType.Relative)
                    return target.DoPunchPosition(end, duration, strength, frequency, dampingRatio, snap);
                return target.DoPunchPosition(start, end, duration, strength, frequency, dampingRatio, snap);

            }
        }
        class DoPunchRotateActor : DoRotateActor
        {
            public Vector3 strength = Vector3.one;
            public int frequency = 10;
            public float dampingRatio = 1;
            protected override ITweenContext<Vector3, Transform> OnCreate()
            {
                if (space == TransformActSpace.Local)
                {
                    if (startType == StartValueType.Relative)
                        return target.DoPunchLocalRotate(end, duration, strength, frequency, dampingRatio, snap);
                    return target.DoPunchLocalRotate(start, end, duration, strength, frequency, dampingRatio, snap);
                }
                if (startType == StartValueType.Relative)
                    return target.DoPunchRotate(end, duration, strength, frequency, dampingRatio, snap);
                return target.DoPunchRotate(start, end, duration, strength, frequency, dampingRatio, snap);

            }
        }
        class DoPunchScaleActor : DoScaleActor
        {
            public Vector3 strength = Vector3.one;
            public int frequency = 10;
            public float dampingRatio = 1;
            protected override ITweenContext<Vector3, Transform> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoPunchLocalScale(end, duration, strength, frequency, dampingRatio, snap);
                return target.DoPunchLocalScale(start, end, duration, strength, frequency, dampingRatio, snap);

            }
        }

        class DoJumpPositionActor : DoPositionActor
        {
            public Vector3 strength = Vector3.one;
            public int jumpCount = 5;
            public float jumpDamping = 2;
            protected override ITweenContext<Vector3, Transform> OnCreate()
            {
                if (space == TransformActSpace.Local)
                {
                    if (startType == StartValueType.Relative)
                        return target.DoJumpLocalPosition(end, duration, strength, jumpCount, jumpDamping, snap);
                    return target.DoJumpLocalPosition(start, end, duration, strength, jumpCount, jumpDamping, snap);
                }
                if (startType == StartValueType.Relative)
                    return target.DoJumpPosition(end, duration, strength, jumpCount, jumpDamping, snap);
                return target.DoJumpPosition(start, end, duration, strength, jumpCount, jumpDamping, snap);

            }
        }
        class DoJumpRotateActor : DoRotateActor
        {
            public Vector3 strength = Vector3.one;
            public int jumpCount = 5;
            public float jumpDamping = 2;
            protected override ITweenContext<Vector3, Transform> OnCreate()
            {
                if (space == TransformActSpace.Local)
                {
                    if (startType == StartValueType.Relative)
                        return target.DoJumpLocalRotate(end, duration, strength, jumpCount, jumpDamping, snap);
                    return target.DoJumpLocalRotate(start, end, duration, strength, jumpCount, jumpDamping, snap);
                }
                if (startType == StartValueType.Relative)
                    return target.DoJumpRotate(end, duration, strength, jumpCount, jumpDamping, snap);
                return target.DoJumpRotate(start, end, duration, strength, jumpCount, jumpDamping, snap);

            }
        }
        class DoJumpScaleActor : DoScaleActor
        {
            public Vector3 strength = Vector3.one;
            public int jumpCount = 5;
            public float jumpDamping = 2;
            protected override ITweenContext<Vector3, Transform> OnCreate()
            {
                if (startType == StartValueType.Relative)
                    return target.DoJumpLocalScale(end, duration, strength, jumpCount, jumpDamping, snap);
                return target.DoJumpLocalScale(start, end, duration, strength, jumpCount, jumpDamping, snap);

            }
        }











        public static ITweenContext<Vector3, Transform> DoLocalScale(this Transform target, Vector3 start, Vector3 end, float duration, bool snap = false)
            => Tween.DoGoto(target, start, end, duration, static (target) => target.localScale, static (target, value) => target.localScale = value, snap);
        public static ITweenContext<Vector3, Transform> DoLocalScale(this Transform target, Vector3 end, float duration, bool snap = false)
            => DoLocalScale(target, target.localScale, end, duration, snap);
        public static ITweenContext<Vector3, Transform> DoPosition(this Transform target, Vector3 start, Vector3 end, float duration, bool snap = false)
           => Tween.DoGoto(target, start, end, duration, static (target) => target.position, static (target, value) => target.position = value, snap);
        public static ITweenContext<Vector3, Transform> DoPosition(this Transform target, Vector3 end, float duration, bool snap = false)
            => DoPosition(target, target.position, end, duration, snap);
        public static ITweenContext<Vector3, Transform> DoLocalPosition(this Transform target, Vector3 start, Vector3 end, float duration, bool snap = false)
          => Tween.DoGoto(target, start, end, duration, static (target) => target.localPosition, static (target, value) => target.localPosition = value, snap);
        public static ITweenContext<Vector3, Transform> DoLocalPosition(this Transform target, Vector3 end, float duration, bool snap = false)
            => DoLocalPosition(target, target.localPosition, end, duration, snap);


        public static ITweenContext<Vector3, Transform> DoRotate(this Transform target, Vector3 start, Vector3 end, float duration, bool snap = false)
            => Tween.DoGoto(target, start, end, duration, static (target) => target.eulerAngles, static (target, value) => target.eulerAngles = value, snap);
        public static ITweenContext<Vector3, Transform> DoRotate(this Transform target, Vector3 end, float duration, bool snap = false)
            => DoRotate(target, target.eulerAngles, end, duration, snap);


        public static ITweenContext<Vector3, Transform> DoLocalRotate(this Transform target, Vector3 start, Vector3 end, float duration, bool snap = false)
            => Tween.DoGoto(target, start, end, duration, static (target) => target.localEulerAngles, static (target, value) => target.localEulerAngles = value, snap);
        public static ITweenContext<Vector3, Transform> DoLocalRotate(this Transform target, Vector3 end, float duration, bool snap = false)
            => DoLocalRotate(target, target.localEulerAngles, end, duration, snap);




        public static ITweenContext<Vector3, Transform> DoShakeRotate(this Transform target, Vector3 start, Vector3 end, float duration, Vector3 strength, int frequency = 10, float dampingRatio = 1, bool snap = false)
     => Tween.DoShake(target, start, end, duration, static (target) => target.eulerAngles, static (target, value) => target.eulerAngles = value, strength, frequency, dampingRatio, snap);
        public static ITweenContext<Vector3, Transform> DoShakeRotate(this Transform target, Vector3 end, float duration, Vector3 strength, int frequency = 10, float dampingRatio = 1, bool snap = false)
            => DoShakeRotate(target, target.eulerAngles, end, duration, strength, frequency, dampingRatio, snap);
        public static ITweenContext<Vector3, Transform> DoShakeLocalRotate(this Transform target, Vector3 start, Vector3 end, float duration, Vector3 strength, int frequency = 10, float dampingRatio = 1, bool snap = false)
            => Tween.DoShake(target, start, end, duration, static (target) => target.localEulerAngles, static (target, value) => target.localEulerAngles = value, strength, frequency, dampingRatio, snap);
        public static ITweenContext<Vector3, Transform> DoShakeLocalRotate(this Transform target, Vector3 end, float duration, Vector3 strength, int frequency = 10, float dampingRatio = 1, bool snap = false)
            => DoShakeLocalRotate(target, target.localEulerAngles, end, duration, strength, frequency, dampingRatio, snap);
        public static ITweenContext<Vector3, Transform> DoShakeLocalPosition(this Transform target, Vector3 start, Vector3 end, float duration, Vector3 strength, int frequency = 10, float dampingRatio = 1, bool snap = false)
         => Tween.DoShake(target, start, end, duration, static (target) => target.localPosition, static (target, value) => target.localPosition = value, strength, frequency, dampingRatio, snap);
        public static ITweenContext<Vector3, Transform> DoShakeLocalPosition(this Transform target, Vector3 end, float duration, Vector3 strength, int frequency = 10, float dampingRatio = 1, bool snap = false)
            => DoShakeLocalPosition(target, target.localPosition, end, duration, strength, frequency, dampingRatio, snap);
        public static ITweenContext<Vector3, Transform> DoShakePosition(this Transform target, Vector3 start, Vector3 end, float duration, Vector3 strength, int frequency = 10, float dampingRatio = 1, bool snap = false)
     => Tween.DoShake(target, start, end, duration, static (target) => target.position, static (target, value) => target.position = value, strength, frequency, dampingRatio, snap);
        public static ITweenContext<Vector3, Transform> DoShakePosition(this Transform target, Vector3 end, float duration, Vector3 strength, int frequency = 10, float dampingRatio = 1, bool snap = false)
            => DoShakePosition(target, target.position, end, duration, strength, frequency, dampingRatio, snap);
        public static ITweenContext<Vector3, Transform> DoShakeLocalScale(this Transform target, Vector3 start, Vector3 end, float duration, Vector3 strength, int frequency = 10, float dampingRatio = 1, bool snap = false)
            => Tween.DoShake(target, start, end, duration, static (target) => target.localScale, static (target, value) => target.localScale = value, strength, frequency, dampingRatio, snap);
        public static ITweenContext<Vector3, Transform> DoShakeLocalScale(this Transform target, Vector3 end, float duration, Vector3 strength, int frequency = 10, float dampingRatio = 1, bool snap = false)
            => DoShakeLocalScale(target, target.localScale, end, duration, strength, frequency, dampingRatio, snap);



        public static ITweenContext<Vector3, Transform> DoPunchRotate(this Transform target, Vector3 start, Vector3 end, float duration, Vector3 strength, int frequency = 10, float dampingRatio = 1, bool snap = false)
=> Tween.DoPunch(target, start, end, duration, static (target) => target.eulerAngles, static (target, value) => target.eulerAngles = value, strength, frequency, dampingRatio, snap);
        public static ITweenContext<Vector3, Transform> DoPunchRotate(this Transform target, Vector3 end, float duration, Vector3 strength, int frequency = 10, float dampingRatio = 1, bool snap = false)
            => DoPunchRotate(target, target.eulerAngles, end, duration, strength, frequency, dampingRatio, snap);
        public static ITweenContext<Vector3, Transform> DoPunchLocalRotate(this Transform target, Vector3 start, Vector3 end, float duration, Vector3 strength, int frequency = 10, float dampingRatio = 1, bool snap = false)
            => Tween.DoPunch(target, start, end, duration, static (target) => target.localEulerAngles, static (target, value) => target.localEulerAngles = value, strength, frequency, dampingRatio, snap);
        public static ITweenContext<Vector3, Transform> DoPunchLocalRotate(this Transform target, Vector3 end, float duration, Vector3 strength, int frequency = 10, float dampingRatio = 1, bool snap = false)
            => DoPunchLocalRotate(target, target.localEulerAngles, end, duration, strength, frequency, dampingRatio, snap);
        public static ITweenContext<Vector3, Transform> DoPunchLocalPosition(this Transform target, Vector3 start, Vector3 end, float duration, Vector3 strength, int frequency = 10, float dampingRatio = 1, bool snap = false)
         => Tween.DoPunch(target, start, end, duration, static (target) => target.localPosition, static (target, value) => target.localPosition = value, strength, frequency, dampingRatio, snap);
        public static ITweenContext<Vector3, Transform> DoPunchLocalPosition(this Transform target, Vector3 end, float duration, Vector3 strength, int frequency = 10, float dampingRatio = 1, bool snap = false)
            => DoPunchLocalPosition(target, target.localPosition, end, duration, strength, frequency, dampingRatio, snap);
        public static ITweenContext<Vector3, Transform> DoPunchPosition(this Transform target, Vector3 start, Vector3 end, float duration, Vector3 strength, int frequency = 10, float dampingRatio = 1, bool snap = false)
     => Tween.DoPunch(target, start, end, duration, static (target) => target.position, static (target, value) => target.position = value, strength, frequency, dampingRatio, snap);
        public static ITweenContext<Vector3, Transform> DoPunchPosition(this Transform target, Vector3 end, float duration, Vector3 strength, int frequency = 10, float dampingRatio = 1, bool snap = false)
            => DoPunchPosition(target, target.position, end, duration, strength, frequency, dampingRatio, snap);
        public static ITweenContext<Vector3, Transform> DoPunchLocalScale(this Transform target, Vector3 start, Vector3 end, float duration, Vector3 strength, int frequency = 10, float dampingRatio = 1, bool snap = false)
            => Tween.DoPunch(target, start, end, duration, static (target) => target.localScale, static (target, value) => target.localScale = value, strength, frequency, dampingRatio, snap);
        public static ITweenContext<Vector3, Transform> DoPunchLocalScale(this Transform target, Vector3 end, float duration, Vector3 strength, int frequency = 10, float dampingRatio = 1, bool snap = false)
            => DoPunchLocalScale(target, target.localScale, end, duration, strength, frequency, dampingRatio, snap);



        public static ITweenContext<Vector3, Transform> DoJumpRotate(this Transform target, Vector3 start, Vector3 end, float duration, Vector3 strength, int jumpCount = 5, float jumpDamping = 2, bool snap = false)
=> Tween.DoJump(target, start, end, duration, static (target) => target.eulerAngles, static (target, value) => target.eulerAngles = value, strength, jumpCount, jumpDamping, snap);
        public static ITweenContext<Vector3, Transform> DoJumpRotate(this Transform target, Vector3 end, float duration, Vector3 strength, int jumpCount = 5, float jumpDamping = 2, bool snap = false)
            => DoJumpRotate(target, target.eulerAngles, end, duration, strength, jumpCount, jumpDamping, snap);
        public static ITweenContext<Vector3, Transform> DoJumpLocalRotate(this Transform target, Vector3 start, Vector3 end, float duration, Vector3 strength, int jumpCount = 5, float jumpDamping = 2, bool snap = false)
            => Tween.DoJump(target, start, end, duration, static (target) => target.localEulerAngles, static (target, value) => target.localEulerAngles = value, strength, jumpCount, jumpDamping, snap);
        public static ITweenContext<Vector3, Transform> DoJumpLocalRotate(this Transform target, Vector3 end, float duration, Vector3 strength, int jumpCount = 5, float jumpDamping = 2, bool snap = false)
            => DoJumpLocalRotate(target, target.localEulerAngles, end, duration, strength, jumpCount, jumpDamping, snap);
        public static ITweenContext<Vector3, Transform> DoJumpLocalPosition(this Transform target, Vector3 start, Vector3 end, float duration, Vector3 strength, int jumpCount = 5, float jumpDamping = 2, bool snap = false)
         => Tween.DoJump(target, start, end, duration, static (target) => target.localPosition, static (target, value) => target.localPosition = value, strength, jumpCount, jumpDamping, snap);
        public static ITweenContext<Vector3, Transform> DoJumpLocalPosition(this Transform target, Vector3 end, float duration, Vector3 strength, int jumpCount = 5, float jumpDamping = 2, bool snap = false)
            => DoJumpLocalPosition(target, target.localPosition, end, duration, strength, jumpCount, jumpDamping, snap);
        public static ITweenContext<Vector3, Transform> DoJumpPosition(this Transform target, Vector3 start, Vector3 end, float duration, Vector3 strength, int jumpCount = 5, float jumpDamping = 2, bool snap = false)
     => Tween.DoJump(target, start, end, duration, static (target) => target.position, static (target, value) => target.position = value, strength, jumpCount, jumpDamping, snap);
        public static ITweenContext<Vector3, Transform> DoJumpPosition(this Transform target, Vector3 end, float duration, Vector3 strength, int jumpCount = 5, float jumpDamping = 2, bool snap = false)
            => DoJumpPosition(target, target.position, end, duration, strength, jumpCount, jumpDamping, snap);
        public static ITweenContext<Vector3, Transform> DoJumpLocalScale(this Transform target, Vector3 start, Vector3 end, float duration, Vector3 strength, int jumpCount = 5, float jumpDamping = 2, bool snap = false)
            => Tween.DoJump(target, start, end, duration, static (target) => target.localScale, static (target, value) => target.localScale = value, strength, jumpCount, jumpDamping, snap);
        public static ITweenContext<Vector3, Transform> DoJumpLocalScale(this Transform target, Vector3 end, float duration, Vector3 strength, int jumpCount = 5, float jumpDamping = 2, bool snap = false)
            => DoJumpLocalScale(target, target.localScale, end, duration, strength, jumpCount, jumpDamping, snap);
    }

}
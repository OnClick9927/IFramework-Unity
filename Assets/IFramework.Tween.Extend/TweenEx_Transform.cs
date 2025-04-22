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
    public static partial class TweenEx
    {

#if UNITY_EDITOR
        class DoPositionArrayActorEditor : TweenActorEditor<DoPositionArrayActor>
        {
            Vector3 CalculateBezierPoint(Vector3[] points, float t)
            {
                int n = points.Length - 1;
                Vector3 point = Vector3.zero;

                for (int i = 0; i <= n; i++)
                {
                    Vector3 controlPoint = points[i];
                    float binomialCoefficient = BinomialCoefficient(n, i);
                    point += binomialCoefficient * Mathf.Pow(1 - t, n - i) * Mathf.Pow(t, i) * controlPoint;
                }

                return point;
            }
            // 二项式系数计算 (n choose k)
            int BinomialCoefficient(int n, int k)
            {
                if (k < 0 || k > n) return 0;
                if (k == 0 || k == n) return 1;

                k = Mathf.Min(k, n - k); // 利用对称性优化计算
                int result = 1;

                for (int i = 1; i <= k; i++)
                {
                    result = result * (n - k + i) / i;
                }

                return result;
            }
            Vector3[] BezierPoints = new Vector3[200];

            protected override void OnSceneGUI(DoPositionArrayActor actor)
            {
                base.OnSceneGUI(actor);
                UnityEditor.Handles.BeginGUI();
                UnityEditor.Handles.color = Color.yellow;
                for (int i = 0; i < actor.points.Length; i++)
                {
                    var point = actor.points[i];
                    UnityEditor.Handles.Label(point, $"point_{i}", new GUIStyle() { fontSize = 20 });

                    var pos = UnityEditor.SceneView.lastActiveSceneView.camera.transform.position;
                    var size = Mathf.Max(0.5f, Vector3.Distance(pos, point) * 0.02f);
                    UnityEditor.Handles.SphereHandleCap(0, point, Quaternion.identity, size, EventType.Repaint);
                    UnityEditor.EditorGUI.BeginChangeCheck();
                    var p = UnityEditor.Handles.DoPositionHandle(point, Quaternion.identity);
                    if (UnityEditor.EditorGUI.EndChangeCheck())
                    {
                        actor.points[i] = p;
                    }
                }
                if (actor.type == ArrayTweenType.Direct)
                {
                    UnityEditor.Handles.DrawPolyLine(actor.points);
                }
                else
                {
                    float count = BezierPoints.Length;
                    for (int i = 0; i < count; i++)
                    {
                        var t = CalculateBezierPoint(actor.points, i / count);
                        BezierPoints[i] = t;
                    }
                    for (int i = 0; i < count - 1; i++)
                    {
                        UnityEditor.Handles.color = Color.Lerp(Color.blue, Color.cyan, i / count);
                        UnityEditor.Handles.DrawLine(BezierPoints[i], BezierPoints[i + 1], 2);
                    }
                }
                UnityEditor.Handles.EndGUI();
            }
        }

#endif
         enum ArrayTweenType
        {
            Direct = 0,
            Bezier = 1,
        }
         enum TransformActSpace
        {
            World,
            Local
        }
         enum StartValueType
        {
            Relative,
            Direct
        }
         class DoScaleActor : TweenComponentActor<Vector3, Transform>
        {
            public StartValueType startType;

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


            public StartValueType startType;
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
            public StartValueType startType;

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



         class DoScaleArrayActor : TweenComponentActor<Vector3, Transform>
        {
            public ArrayTweenType type;
            public Vector3[] points;

            protected override ITweenContext<Vector3, Transform> OnCreate()
            {
                if (type == ArrayTweenType.Direct)

                    return target.DoLocalScaleArray(duration, points, snap);
                return target.DoLocalScaleArrayBezier(duration, points, snap);

            }
        }
         class DoPositionArrayActor : TweenComponentActor<Vector3, Transform>
        {

            public ArrayTweenType type;
            public TransformActSpace space;
            public Vector3[] points;

            protected override ITweenContext<Vector3, Transform> OnCreate()
            {
                if (space == TransformActSpace.Local)
                {
                    if (type == ArrayTweenType.Direct)
                        return target.DoLocalPositionArray(duration, points, snap);
                    return target.DoLocalPositionArrayBezier(duration, points, snap);
                }
                if (type == ArrayTweenType.Direct)
                    return target.DoPositionArray(duration, points, snap);
                return target.DoPositionArrayBezier(duration, points, snap);

            }
        }
         class DoRotateArrayActor : TweenComponentActor<Vector3, Transform>
        {
            public ArrayTweenType type;

            public TransformActSpace space;
            public Vector3[] points;

            protected override ITweenContext<Vector3, Transform> OnCreate()
            {
                if (space == TransformActSpace.Local)
                {
                    if (type == ArrayTweenType.Direct)
                        return target.DoLocalRotateArray(duration, points, snap);
                    return target.DoLocalRotateArrayBezier(duration, points, snap);

                }
                if (type == ArrayTweenType.Direct)
                    return target.DoRotateArray(duration, points, snap);
                return target.DoRotateArrayBezier(duration, points, snap);
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





        public static ITweenContext<Vector3, Transform> DoRotateArray(this Transform target, float duration, Vector3[] points, bool snap = false)
=> Tween.DoArray(target, duration, static (target) => target.eulerAngles, static (target, value) => target.eulerAngles = value, points, snap);

        public static ITweenContext<Vector3, Transform> DoLocalRotateArray(this Transform target, float duration, Vector3[] points, bool snap = false)
=> Tween.DoArray(target, duration, static (target) => target.localEulerAngles, static (target, value) => target.localEulerAngles = value, points, snap);

        public static ITweenContext<Vector3, Transform> DoLocalPositionArray(this Transform target, float duration, Vector3[] points, bool snap = false)
=> Tween.DoArray(target, duration, static (target) => target.localPosition, static (target, value) => target.localPosition = value, points, snap);

        public static ITweenContext<Vector3, Transform> DoPositionArray(this Transform target, float duration, Vector3[] points, bool snap = false)
=> Tween.DoArray(target, duration, static (target) => target.position, static (target, value) => target.position = value, points, snap);

        public static ITweenContext<Vector3, Transform> DoLocalScaleArray(this Transform target, float duration, Vector3[] points, bool snap = false)
=> Tween.DoArray(target, duration, static (target) => target.localScale, static (target, value) => target.localScale = value, points, snap);





        public static ITweenContext<Vector3, Transform> DoRotateArrayBezier(this Transform target, float duration, Vector3[] points, bool snap = false)
=> Tween.DoBezier(target, duration, static (target) => target.eulerAngles, static (target, value) => target.eulerAngles = value, points, snap);

        public static ITweenContext<Vector3, Transform> DoLocalRotateArrayBezier(this Transform target, float duration, Vector3[] points, bool snap = false)
=> Tween.DoBezier(target, duration, static (target) => target.localEulerAngles, static (target, value) => target.localEulerAngles = value, points, snap);

        public static ITweenContext<Vector3, Transform> DoLocalPositionArrayBezier(this Transform target, float duration, Vector3[] points, bool snap = false)
=> Tween.DoBezier(target, duration, static (target) => target.localPosition, static (target, value) => target.localPosition = value, points, snap);

        public static ITweenContext<Vector3, Transform> DoPositionArrayBezier(this Transform target, float duration, Vector3[] points, bool snap = false)
=> Tween.DoBezier(target, duration, static (target) => target.position, static (target, value) => target.position = value, points, snap);

        public static ITweenContext<Vector3, Transform> DoLocalScaleArrayBezier(this Transform target, float duration, Vector3[] points, bool snap = false)
=> Tween.DoBezier(target, duration, static (target) => target.localScale, static (target, value) => target.localScale = value, points, snap);
    }

}
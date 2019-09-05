/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.11f1
 *Date:           2019-07-25
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
namespace IFramework
{
    public enum ConditionCompareType
    {
        SmallerThanCompare, BiggerThanCompare, EqualsWithCompare, NotEqualsWithCompare
    }
    public interface ITransitionCondition
    {
        Type Conditiontype { get; }
        string Name { get; }
        ConditionCompareType CompareType { get; set; }
        bool IsMetCondition();
    }
    public abstract class TransitionCondition<T> : ITransitionCondition
    {
        protected IFsmConditionValue ConditionValue { get; set; }
        public Type Conditiontype { get { return typeof(T); } }
        protected T Value { get { return (T)ConditionValue.Value; } }
        public string Name { get { return ConditionValue.Name; } }

        public T CompareValue { get; set; }
        public ConditionCompareType CompareType { get;  set; }
        public TransitionCondition(FsmConditionValue<T> ConditionValue, object CompareValue, ConditionCompareType CompareType)
        {
            this.ConditionValue = ConditionValue;
            this.CompareValue = (T)CompareValue;
            SetConditionType(CompareType);
        }
        protected abstract void SetConditionType(ConditionCompareType CompareType);
        public abstract bool IsMetCondition();
    }
    public class StringTransitionCondition : TransitionCondition<string>
    {
        public StringTransitionCondition(FsmConditionValue<string> ConditionValue, object CompareValue, ConditionCompareType CompareType) : base(ConditionValue, CompareValue, CompareType) { }
        public override bool IsMetCondition()
        {
            switch (CompareType)
            {
                case ConditionCompareType.EqualsWithCompare:
                    return CompareValue == Value;
                case ConditionCompareType.NotEqualsWithCompare:
                    return CompareValue != Value;
                case ConditionCompareType.SmallerThanCompare:
                case ConditionCompareType.BiggerThanCompare:
                default:
                    return false;
            }
        }
        protected override void SetConditionType(ConditionCompareType CompareType)
        {
            switch (CompareType)
            {
                case ConditionCompareType.SmallerThanCompare:
                case ConditionCompareType.BiggerThanCompare:
                    throw new Exception("The Type is Illeagal whith " + Conditiontype);
                case ConditionCompareType.EqualsWithCompare:
                case ConditionCompareType.NotEqualsWithCompare:
                    this.CompareType = CompareType;
                    break;
            }
        }
    }
    public class IntTransitionCondition : TransitionCondition<int>
    {
        public IntTransitionCondition(FsmConditionValue<int> ConditionValue, object CompareValue,  ConditionCompareType CompareType) : base( ConditionValue ,CompareValue, CompareType) { }
        protected override void SetConditionType(ConditionCompareType CompareType)
        {
            this.CompareType = CompareType;
        }
        public override bool IsMetCondition()
        {
            switch (CompareType)
            {
                case ConditionCompareType.SmallerThanCompare:
                    return Value < CompareValue;
                case ConditionCompareType.BiggerThanCompare:
                    return Value > CompareValue;
                case ConditionCompareType.EqualsWithCompare:
                    return CompareValue == Value;
                case ConditionCompareType.NotEqualsWithCompare:
                    return CompareValue != Value;
                default:
                    return false;
            }
        }
    }
    public class FloatTransitionCondition : TransitionCondition<float>
    {
        public FloatTransitionCondition(FsmConditionValue<float> ConditionValue, object CompareValue ,ConditionCompareType CompareType) : base(ConditionValue, CompareValue, CompareType) { }
        protected override void SetConditionType(ConditionCompareType CompareType)
        {
            this.CompareType = CompareType;
        }
        public override bool IsMetCondition()
        {
            switch (CompareType)
            {
                case ConditionCompareType.SmallerThanCompare:
                    return Value < CompareValue;
                case ConditionCompareType.BiggerThanCompare:
                    return Value > CompareValue;
                case ConditionCompareType.EqualsWithCompare:
                    return CompareValue == Value;
                case ConditionCompareType.NotEqualsWithCompare:
                    return CompareValue != Value;
                default:
                    return false;
            }
        }
    }
    public class BoolTransitionCondition : TransitionCondition<bool>
    {
        public BoolTransitionCondition(FsmConditionValue<bool> ConditionValue, object CompareValue, ConditionCompareType CompareType) : base(ConditionValue, CompareValue, CompareType) { }
        public override bool IsMetCondition()
        {
            switch (CompareType)
            {
                case ConditionCompareType.EqualsWithCompare:
                    return CompareValue == Value;
                case ConditionCompareType.NotEqualsWithCompare:
                    return CompareValue != Value;
                case ConditionCompareType.SmallerThanCompare:
                case ConditionCompareType.BiggerThanCompare:
                default:
                    return false;
            }
        }
        protected override void SetConditionType(ConditionCompareType CompareType)
        {
            switch (CompareType)
            {
                case ConditionCompareType.SmallerThanCompare:
                case ConditionCompareType.BiggerThanCompare:
                    throw new Exception("The Type is Illeagal whith " + Conditiontype);
                case ConditionCompareType.EqualsWithCompare:
                case ConditionCompareType.NotEqualsWithCompare:
                    this.CompareType = CompareType;
                    break;
            }
        }
    }
}

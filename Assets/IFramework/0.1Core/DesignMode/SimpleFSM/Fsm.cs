/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.11f1
 *Date:           2019-07-25
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;

namespace IFramework
{
	public class Fsm
	{
        private class StateInfo
        {
            public IFsmState state { get; private set; }
            public StateInfo(IFsmState state)
            {
                this.state = state;
                Transitions = new List<FsmTransition>();
            }
            public List<FsmTransition> Transitions { get; private set; }
            public void SetTransition(FsmTransition stateTransition)
            {
                var sameOne = Transitions.Find((t) => { return t.Trail == stateTransition.Trail; });
                if (sameOne == null)
                    Transitions.Add(stateTransition);
                else
                    throw new Exception("The Trastion Same");
            }
        }
        public string Name { get; private set; }
        public IFsmState ExitState { get; set; }
        public IFsmState EnterState { get; set; }
        public IFsmState CurrentState { get; private set; }
        public bool IsRuning { get; private set; }
        //private Dictionary<Type, List<ITransitionCondition>> conditions;
        private Dictionary<IFsmState, StateInfo> stateInfo;
        private Dictionary<Type, Dictionary<string, IFsmConditionValue>> ConditionValues;
        public Fsm(string name)
        {
            this.Name = name;
            IsRuning = false;
            //conditions = new Dictionary<Type, List<ITransitionCondition>>();
            stateInfo = new Dictionary<IFsmState, StateInfo>();
            ConditionValues = new Dictionary<Type, Dictionary<string, IFsmConditionValue>>();
        }

        public void CreateState(IFsmState state)
        {
            if (!stateInfo.ContainsKey(state))
                stateInfo.Add(state, new StateInfo(state));
            else
                Log.E(" Have  Exist  State ");
        }
        public FsmTransition CreateTransition(IFsmState head, IFsmState trail)
        {
            if (!stateInfo.ContainsKey(head))
                Log.E("Create StateInfo Fist");
            if (!stateInfo.ContainsKey(trail))
                Log.E("Create StateInfo Fist");
            FsmTransition transition = new FsmTransition();
            transition.Head = head;
            transition.Trail = trail;
            stateInfo[head].SetTransition(transition);
            return transition;
        }
        public FsmConditionValue<T> CreateConditionValue<T>(string name, T value)
        {
            if (!ConditionValues.ContainsKey(typeof(T)))
                ConditionValues.Add(typeof(T), new Dictionary<string, IFsmConditionValue>());
            if (!ConditionValues[typeof(T)].ContainsKey(name))
                ConditionValues[typeof(T)].Add(name, new FsmConditionValue<T>() { Name = name, Value = value });
            else
                Log.E("ConditionValue Exsit " + name);
            return ConditionValues[typeof(T)][name] as FsmConditionValue<T>;
        }
        public TransitionCondition<T> CreateCondition<T>(string conditionValName, T CompareValue, ConditionCompareType CompareType)
        {
            if (!ConditionValues.ContainsKey(typeof(T)) || !ConditionValues[typeof(T)].ContainsKey(conditionValName))
            {
                Log.E("Please Create ConditionVal First Type " + typeof(T) + "  Name  " + conditionValName);
                return default(TransitionCondition<T>);
            }
            return CreateCondition(ConditionValues[typeof(T)][conditionValName] as FsmConditionValue<T>, CompareValue, CompareType);
        }
        public TransitionCondition<T> CreateCondition<T>(FsmConditionValue<T> value, T CompareValue, ConditionCompareType CompareType)
        {
            ITransitionCondition condtion;
            if (typeof(T) == typeof(bool))
                condtion = new BoolTransitionCondition(value as FsmConditionValue<bool>, CompareValue, CompareType);
            else if (typeof(T) == typeof(string))
                condtion = new StringTransitionCondition(value as FsmConditionValue<string>, CompareValue, CompareType);
            else if (typeof(T) == typeof(float))
                condtion = new FloatTransitionCondition(value as FsmConditionValue<float>, CompareValue, CompareType);
            else if (typeof(T) == typeof(int))
                condtion = new IntTransitionCondition(value as FsmConditionValue<int>, CompareValue, CompareType);
            else
                throw new Exception("Fault Type Of T" + typeof(T));
            //if (!conditions.ContainsKey(typeof(T))) conditions.Add(typeof(T), new List<ITransitionCondition>());
            //conditions[typeof(T)].Add(condtion);
            return condtion as TransitionCondition<T>;
        }

        public void Run()
        {
            if (EnterState == null) throw new Exception("StartState Is Null");
            IsRuning = true;
            EnterState.OnEnter();
            CurrentState = EnterState;
        }

        public void Update()
        {
            if (CurrentState == ExitState) return;
            CurrentState.Update();
            CheckNext();
            if (CurrentState == ExitState) IsRuning = false;
        }
        private void CheckNext()
        {
            if (CurrentState == null) return;
            List<FsmTransition> transitions = stateInfo[CurrentState].Transitions;
            for (int i = 0; i < transitions.Count; i++)
            {
                if (transitions[i].IsMetCondition)
                {
                    FsmTransition transition = transitions[i];
                    CurrentState = transition.GoToNextState();
                    break;
                }
            }
        }

        private bool CheckLeagal(Type valType, string name)
        {
            if (!ConditionValues.ContainsKey(valType))
            {
                Log.E("Not Have Condition Type  " + valType);
                return false;
            }
            if (!ConditionValues[valType].ContainsKey(name))
            {
                Log.E("Not Have ConditionVal Name  " + name);
                return false;
            }
            return true;
        }
        public void SetBool(string valName, bool value)
        {
            if (!CheckLeagal(typeof(bool), valName)) return;
            ConditionValues[typeof(bool)][valName].Value = value;
        }
        public void SetInt(string valName, int value)
        {
            if (!CheckLeagal(typeof(int), valName)) return;
            ConditionValues[typeof(int)][valName].Value = value;
        }
        public void SetFloat(string valName, float value)
        {
            if (!CheckLeagal(typeof(float), valName)) return;
            ConditionValues[typeof(float)][valName].Value = value;
        }
        public void SetString(string valName, string value)
        {
            if (!CheckLeagal(typeof(string), valName)) return;
            ConditionValues[typeof(string)][valName].Value = value;
        }
    }
}

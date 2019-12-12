/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-07-24
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using IFramework;
using IFramework.FSM;
using UnityEngine;
namespace IFramework_Demo
{
    public class State:IFsmState
    {
        public void OnEnter()
        {
            Log.L(GetType() + "  Enter");
        }
        public void OnExit()
        {
            Log.L(GetType() + "  OnExit");

        }
        public void Update()
        {
            Log.L(GetType() + "  Update");
        }
    }
    public class State1 : State
    {
    }
    public class State2 : State
    {
    }
    public class FsmExample : MonoBehaviour
	{
        FsmMoudle f = new FsmMoudle();
        State1 s1 = new State1();
        State2 s2 = new State2();

        private void Awake()
        {
            f.SubscribeState(s1);
            f.EnterState = s1;
            f.SubscribeState(s2); 
            //f.ExitState = s2;
         var val=   f.CreateConditionValue<bool>("bool", true);

            var t1=  f.CreateTransition(s1, s2);
            var t2 = f.CreateTransition(s2, s1);

            t1.BindCondition(f.CreateCondition<bool>("bool", false, ConditionCompareType.EqualsWithCompare));
            t2.BindCondition(f.CreateCondition<bool>(val, true, ConditionCompareType.EqualsWithCompare));

            f.Start();
        }
        private void Update()
        {
            f.Update();
            if (Input.GetKeyDown(KeyCode.Space))
            {
               f.SetBool("bool", false);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                f.SetBool("bool", true);
            }
        }
    }
}

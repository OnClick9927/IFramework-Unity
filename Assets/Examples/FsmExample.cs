/*********************************************************************************
 *Author:         OnClick
 *Version:        1.0
 *UnityVersion:   2018.3.11f1
 *Date:           2019-07-24
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using UnityEngine;
namespace IFramework.Example
{
    public class State1 : IFsmState
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
    public class State2 : IFsmState
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
    public class FsmExample : MonoBehaviour
	{
        Fsm f = new Fsm("");
        State1 s1 = new State1();
        State2 s2 = new State2();

        private void Awake()
        {
            f.CreateState(s1);
            f.EnterState = s1;
            f.CreateState(s2); 
            //f.ExitState = s2;

            var t=  f.CreateTransition(s1, s2);
            var t2 = f.CreateTransition(s2, s1);

            f.CreateConditionValue<bool>("bool", true);
            t.BindCondition(f.CreateCondition<bool>("bool", false, ConditionCompareType.EqualsWithCompare));
            t2.BindCondition(f.CreateCondition<bool>("bool", true, ConditionCompareType.EqualsWithCompare));

            f.Run();
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

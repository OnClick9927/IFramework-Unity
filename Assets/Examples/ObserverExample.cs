/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2018.3.11f1
 *Date:           2019-12-16
 *Description:    Description
 *History:        2019-12-16--
*********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using IFramework;
using UnityEngine;
namespace IFramework_Demo
{

    public class ObserverExample:MonoBehaviour,IPublisher
	{
        public class Listenner : IObserver
        {
            public Listenner()
            {
                ObserveManager.Subscribe<ObserverExample>(this);
                this.Subscribe<ObserverExample>();
            }
            public void Listen(IPublisher publisher, Type eventType, int code, IEventArgs args, params object[] param)
            {
                Log.L(string.Format("Recieve code {0} from type {1}", code,eventType));
            }
        }
        private void Awake()
        {
            Listenner listenner = new Listenner();
            this.Publish(100, null);
            ObserveManager.Publish(this, 100, null);
        }
    }
}

/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-03-14
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Net.Sockets;
using System.Threading;

namespace IFramework.Net
{
    public class SocketEventArgPool : ObjectPool<SocketAsyncEventArgs>
    {
        public SocketEventArgPool(int capcity) : base(capcity) { }
        protected override void OnClear(SocketAsyncEventArgs t, IEventArgs arg, params object[] param)
        {
            base.OnClear(t,arg,param);
            t.Dispose();
        }
        public SocketAsyncEventArgs GetFreeArg(Func<object, bool> func, bool wait)
        {
            int retry = 1;
            while (true)
            {
                var tArgs = Get();
                if (tArgs != null) return tArgs;
                if (wait == false)
                {
                    if (retry > 16) break;
                    retry++;
                }
                var isContinue = func(retry);
                if (isContinue == false) break;
                Thread.Sleep(1000 * retry);
            }
            return default(SocketAsyncEventArgs);
        }

        protected override SocketAsyncEventArgs CreatNew(IEventArgs arg, params object[] param)
        {
            return default(SocketAsyncEventArgs);
        }
    }

}

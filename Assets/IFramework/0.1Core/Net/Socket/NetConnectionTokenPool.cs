/*********************************************************************************
 *Author:         OnClick
 *Version:        0.0.1
 *UnityVersion:   2017.2.3p3
 *Date:           2019-08-21
 *Description:    IFramework
 *History:        2018.11--
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace IFramework.Net
{
    public class NetConnectionTokenPool
    {
        private LinkedList<NetConnectionToken> list = null;
        private int timerSpace = 60;//s
        private Timer timer;
        private LockParam lockParam;

        public int ConnectionTimeout = 60;//s

        public int Count { get { return list.Count; } }

        public NetConnectionTokenPool(int timerSpace)
        {
            this.timerSpace = timerSpace < 2 ? 2 : timerSpace;
            lockParam = new LockParam();
            int _period = TimerSpaceToSeconds();
            list = new LinkedList<NetConnectionToken>();
            timer = new Timer(new TimerCallback(TimerLoop), null, _period, _period);
        }
        private int TimerSpaceToSeconds()
        {
            return (timerSpace * 1000) >> 1;
        }
        private void TimerLoop(object obj)
        {
            using (LockWait lwait = new LockWait(ref lockParam))
            {
                var items = list.Where(x => x.Verification == false ||
                                        DateTime.Now.Subtract(x.ConnectionTime).TotalSeconds >= ConnectionTimeout)
                                        .ToArray();
                for (int i = 0; i < items.Length; ++i)
                {
                    items[i].Token.Close();
                    list.Remove(items[i]);
                }
            }
        }

        public void TimerEnable(bool enable)
        {
            if (enable)
            {
                int space = TimerSpaceToSeconds();
                timer.Change(space, space);
            }
            else timer.Change(-1, -1);
        }
        public void ChangeTimerSpace(int space)
        {
            this.timerSpace = space < 2 ? 2 : space;
            int _p = TimerSpaceToSeconds();
            timer.Change(_p, _p);
        }

        public NetConnectionToken GetTop()
        {
            using (LockWait lwait = new LockWait(ref lockParam))
            {
                if (list.Count > 0)
                    return list.First();
                return null;
            }
        }

        public IEnumerable<NetConnectionToken> ReadNext()
        {
            using (LockWait lwait = new LockWait(ref lockParam))
            {
                foreach (var l in list)
                {
                    yield return l;
                }
            }
        }

        public void AddToken(NetConnectionToken ncToken)
        {
            using (LockWait lwait = new LockWait(ref lockParam))
            {
                list.AddLast(ncToken);
            }
        }

        public bool RemoveToken(NetConnectionToken ncToken, bool isClose)
        {
            using (LockWait lwait = new LockWait(ref lockParam))
            {
                if (isClose) ncToken.Token.Close();
                return list.Remove(ncToken);
            }
        }

        public bool RemoveToken(SocketToken sToken)
        {
            using (LockWait lwait = new LockWait(ref lockParam))
            {
                var item = list.Where(x => x.Token.CompareTo(sToken) == 0).FirstOrDefault();
                if (item != null)
                {
                    return list.Remove(item);
                }
            }
            return false;
        }

        public NetConnectionToken GetTokenById(int Id)
        {
            using (LockWait lwait = new LockWait(ref lockParam))
            {
                return list.Where(x => x.Token.TokenId == Id).FirstOrDefault();
            }
        }

        public NetConnectionToken GetTokenBySocketToken(SocketToken sToken)
        {
            using (LockWait lwait = new LockWait(ref lockParam))
            {
                return list.Where(x => x.Token.CompareTo(sToken) == 0).FirstOrDefault();
            }
        }

        public void Clear(bool isClose)
        {
            using (LockWait lwait = new LockWait(ref lockParam))
            {
                while (list.Count > 0)
                {
                    var item = list.First();
                    list.RemoveFirst();

                    if (isClose)
                    {
                        if (item.Token != null)
                            item.Token.Close();
                    }
                }
            }
        }

        public bool RefreshConnectionToken(SocketToken sToken)
        {
            using (LockWait lwait = new LockWait(ref lockParam))
            {
                var rt = list.Find(new NetConnectionToken(sToken));
                if (rt == null) return false;
                rt.Value.ConnectionTime = DateTime.Now;
                return true;
            }
        }

    }
    public class NetConnectionToken : IComparable<NetConnectionToken>
    {
        public NetConnectionToken() { }
        public NetConnectionToken(SocketToken sToken)
        {
            this.Token = sToken;
            Verification = true;
            ConnectionTime = DateTime.Now;
        }
        public SocketToken Token { get; set; }
        public DateTime ConnectionTime { get; set; }
        public bool Verification { get; set; }
        public int CompareTo(NetConnectionToken item)
        {
            return Token.CompareTo(item.Token);
        }
        public override bool Equals(object obj)
        {
            var nc = obj as NetConnectionToken;
            if (nc == null) return false;
            return this.CompareTo(nc) == 0;
        }
        public override int GetHashCode()
        {
            return Token.TokenId.GetHashCode() | Token.Sock.GetHashCode();
        }
    }
}

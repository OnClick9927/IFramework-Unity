using System;
using System.Collections.Generic;
namespace IFramework.Message
{
    class Subject<T> : IDisposable
    {
        private readonly T _listenType;
        public T listenType { get { return _listenType; } }
        private List<MessageListener> _listeners;
        public Subject(T listenType)
        {
            this._listenType = listenType;
            _listeners = new List<MessageListener>();
        }
        public bool Subscribe(MessageListener listener)
        {
            if (_listeners.Contains(listener)) return false;
            else _listeners.Add(listener); return true;
        }
        public bool UnSubscribe(MessageListener listener)
        {
            if (!_listeners.Contains(listener)) return false;
            else _listeners.Remove(listener); return true;
        }
        public bool Publish(IMessage message)
        {
            if (_listeners.Count == 0) return false;
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                var listener = _listeners[i];
                if (listener != null) listener.Invoke(message);
                else _listeners.Remove(listener);
            }
            return true;
        }
        public void Dispose()
        {
            _listeners.Clear();
        }
    }
}

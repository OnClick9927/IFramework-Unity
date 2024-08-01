using System.Runtime.CompilerServices;

namespace IFramework
{
    public interface IAwaitable<out TAwaiter> where TAwaiter : IAwaiter
    {

        IAwaiter GetAwaiter();
    }


    public interface IAwaitable<out TAwaiter, out TResult> where TAwaiter : IAwaiter<TResult>
    {
        IAwaiter<TResult> GetAwaiter();
    }
    public interface IAwaiter : INotifyCompletion, ICriticalNotifyCompletion
    {

        bool IsCompleted { get; }


        void GetResult();
    }

    public interface IAwaiter<out TResult> : INotifyCompletion, ICriticalNotifyCompletion
    {
     
        bool IsCompleted { get; }
        TResult GetResult();
    }

  
}

using System.Runtime.CompilerServices;

namespace IFramework
{
    /// <summary>
    /// 表示一个可等待对象，如果一个方法返回此类型的实例，则此方法可以使用 `await` 异步等待。
    /// </summary>
    /// <typeparam name="TAwaiter">用于给 await 确定返回时机的 IAwaiter 的实例。</typeparam>
    public interface IAwaitable<out TAwaiter> where TAwaiter : IAwaiter
    {
        /// <summary>
        /// 获取一个可用于 await 关键字异步等待的异步等待对象。
        /// 此方法会被编译器自动调用。
        /// </summary>
        TAwaiter GetAwaiter();
    }

    /// <summary>
    /// 表示一个包含返回值的可等待对象，如果一个方法返回此类型的实例，则此方法可以使用 `await` 异步等待返回值。
    /// </summary>
    /// <typeparam name="TAwaiter">用于给 await 确定返回时机的 IAwaiter{<typeparamref name="TResult"/>} 的实例。</typeparam>
    /// <typeparam name="TResult">异步返回的返回值类型。</typeparam>
    public interface IAwaitable<out TAwaiter, out TResult> where TAwaiter : IAwaiter<TResult>
    {
        /// <summary>
        /// 获取一个可用于 await 关键字异步等待的异步等待对象。
        /// 此方法会被编译器自动调用。
        /// </summary>
        TAwaiter GetAwaiter();
    }

   
}

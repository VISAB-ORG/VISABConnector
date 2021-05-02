using System;
using System.Threading.Tasks;

namespace VISABConnector
{
    /// <summary>
    /// Helper class for running asynchronous methods synchronously without deadlocking the current
    /// thread. Should be used when VISABApi has to be used in non async methods.
    /// </summary>
    internal static class AsyncHelper
    {
        /// <summary>
        /// Invokes the given Func on a different thread and waits on execution finish
        /// </summary>
        /// <typeparam name="TReturn">The return type of the Func</typeparam>
        /// <param name="asyncMethod">The Func (function reference) to invoke</param>
        /// <returns>The result of the Func invocation</returns>
        public static TReturn RunSynchronously<TReturn>(Func<Task<TReturn>> asyncMethod)
        {
            return Task.Run(async () => await asyncMethod.Invoke()).Result;
        }

        /// <summary>
        /// Invokes the given Func on a different thread and waits on execution finish
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter</typeparam>
        /// <typeparam name="TReturn">The return type of the Func</typeparam>
        /// <param name="asyncMethod">The Func (function reference) to invoke</param>
        /// <param name="value1">The first parameter</param>
        /// <returns>The result of the Func invocation</returns>
        public static TReturn RunSynchronously<T1, TReturn>(Func<T1, Task<TReturn>> asyncMethod, T1 value)
        {
            return Task.Run(async () => await asyncMethod.Invoke(value)).Result;
        }

        /// <summary>
        /// Invokes the given Func on a different thread and waits on execution finish
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter</typeparam>
        /// <typeparam name="T2">The type of the second parameter</typeparam>
        /// <typeparam name="TReturn">The return type of the Func</typeparam>
        /// <param name="asyncMethod">The Func (function reference) to invoke</param>
        /// <param name="value1">The first parameter</param>
        /// <param name="value2">The second parameter</param>
        /// <returns>The result of the Func invocation</returns>
        public static TReturn RunSynchronously<T1, T2, TReturn>(Func<T1, T2, Task<TReturn>> asyncMethod, T1 value1, T2 value2)
        {
            return Task.Run(async () => await asyncMethod.Invoke(value1, value2)).Result;
        }

        /// <summary>
        /// Invokes the given Func on a different thread and waits on execution finish
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter</typeparam>
        /// <typeparam name="T2">The type of the second parameter</typeparam>
        /// <typeparam name="T3">The type of the third parameter</typeparam>
        /// <typeparam name="TReturn">The return type of the Func</typeparam>
        /// <param name="asyncMethod">The Func (function reference) to invoke</param>
        /// <param name="value1">The first parameter</param>
        /// <param name="value2">The second parameter</param>
        /// <param name="value3">The third parameter</param>
        /// <returns>The result of the Func invocation</returns>
        public static TReturn RunSynchronously<T1, T2, T3, TReturn>(Func<T1, T2, T3, Task<TReturn>> asyncMethod, T1 value1, T2 value2, T3 value3)
        {
            return Task.Run(async () => await asyncMethod.Invoke(value1, value2, value3)).Result;
        }
    }
}
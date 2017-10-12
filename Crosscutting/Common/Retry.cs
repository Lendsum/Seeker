using System;

namespace Lendsum.Crosscutting.Common
{
    /// <summary>
    /// Helper to retry a method until it doesnt have
    /// </summary>
    public static class Retrier
    {
        /// <summary>
        /// Retries the specified function.
        /// </summary>
        /// <typeparam name="T1">The type of the 1.</typeparam>
        /// <param name="retries">The retries.</param>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">retries cannot be less than 1 - retries</exception>
        public static T1 Retry<T1>(int retries, Func<T1> func)
        {
            Check.NotNull(() => func);
            try
            {
                if (retries < 1)
                {
                    throw new LendsumException("The retry has been retrying the max of posible retries");
                }

                return func();
            }
            catch (Exception)
            {
                if (retries < 1)
                {
                    throw;
                }
            }

            return Retry<T1>(retries - 1, func);
        }
    }
}
using System;
using System.Threading;
using Polly;
using Polly.Retry;


namespace NUnitXMLReader.Wait
{
    public class WaitCondition
    {
        private WaitCondition() { }
        /// <summary>
        /// Waiting for a action condition
        /// </summary>
        /// <param name="func">Delegate for the action condition</param>
        /// <param name="action">Executed action after failed condition</param>
        /// <param name="retryCount">Namber of retry</param>
        /// <param name="timeout">Timeout of retry</param>
        /// <returns></returns>
        public static bool WaitAndRetry(Func<bool> func, Action action, int retryCount, TimeSpan timeout)
        {
            for(int i = 0; i < retryCount; i++)
            {
                if (func())
                {
                    return true;
                }
                else
                {
                    Thread.Sleep(timeout);
                    action();
                }
            }
            return false;
        }
    }
}

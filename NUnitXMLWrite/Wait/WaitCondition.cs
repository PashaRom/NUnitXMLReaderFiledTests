using System;
using Polly;
using Polly.Retry;

namespace NUnitXMLReader.Wait
{
    public class WaitCondition
    {
        private WaitCondition() { }

        public static bool Retry(Func<bool> func, int numberOfRetry, TimeSpan timeOuts)
        {
            RetryPolicy<bool> retryPolicy = Policy.HandleResult<bool>(b => b != true)
                .WaitAndRetry(numberOfRetry, i => timeOuts);
            return retryPolicy.Execute(func);
        }
    }
}

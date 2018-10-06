using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Just.Anarchy.Test.Unit.utils
{
    public static class Wait
    {
        public static async Task<bool> Until(Func<bool> shouldFinish, double timeoutSecs)
        {
            try
            {
                await WaitInternal(shouldFinish, TimeSpan.FromSeconds(timeoutSecs));
                return true;
            }
            catch (WaitFailedException)
            {
                return false;
            }
        }

        public static async Task<TimeSpan> AndTimeActionAsync(Func<bool> shouldFinish, double timeoutSecs)
        {
            return await WaitInternal(shouldFinish, TimeSpan.FromSeconds(timeoutSecs));
        }

        private static async Task<TimeSpan> WaitInternal(Func<bool> shouldFinish, TimeSpan timeout)
        {
            var sw = Stopwatch.StartNew();
            while (!shouldFinish() && sw.Elapsed < timeout)
            {
                await Task.Delay(100);
            }

            if (!shouldFinish())
            {
                throw new WaitFailedException();
            }

            return sw.Elapsed;
        }
    }
}

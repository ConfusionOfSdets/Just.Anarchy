using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Just.Anarchy.Test.Common.Utilities
{
    public static class Block
    {
        public static async Task UntilCancelled(CancellationToken ct, Action<CancellationToken> cancelledCallback = null)
        {
            try
            {
                await Task.Delay(-1, ct);
            }
            catch (TaskCanceledException e)
            {
                cancelledCallback?.Invoke(e.CancellationToken);
                //then swallow the exception and just return...
            }
        }
    }
}

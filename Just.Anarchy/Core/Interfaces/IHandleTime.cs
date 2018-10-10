using System;
using System.Threading;
using System.Threading.Tasks;

namespace Just.Anarchy
{
    public interface IHandleTime
    {
        Task DelayInitial(Schedule schedule, CancellationToken ct);
        Task DelayInterval(Schedule schedule, CancellationToken ct);
    }
}
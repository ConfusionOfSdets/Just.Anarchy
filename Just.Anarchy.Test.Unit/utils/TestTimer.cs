using System;
using System.Threading;
using System.Threading.Tasks;
using NSubstitute;

namespace Just.Anarchy.Test.Unit.utils
{
    public static class TestTimer
    {
        public static IHandleTime WithoutDelays()
        {
            var timer = Substitute.For<IHandleTime>();
            timer.DelayInitial(Arg.Any<Schedule>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
            timer.DelayInterval(Arg.Any<Schedule>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            return timer;
        }

        public static IHandleTime WithDelays()
        {
            var timer = Substitute.For<IHandleTime>();
            timer.DelayInitial(Arg.Any<Schedule>(), Arg.Any<CancellationToken>())
                .Returns(r => Task.Delay(r.ArgAt<Schedule>(0).Delay, r.ArgAt<CancellationToken>(1)));
            timer.DelayInterval(Arg.Any<Schedule>(), Arg.Any<CancellationToken>())
                .Returns(r => Task.Delay(r.ArgAt<Schedule>(0).Interval, r.ArgAt<CancellationToken>(1)));

            return timer;
        }
    }
}

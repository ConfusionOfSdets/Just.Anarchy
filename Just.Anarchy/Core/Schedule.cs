using System;

namespace Just.Anarchy
{
    public class Schedule
    {
        public TimeSpan Delay;
        public TimeSpan Interval;
        public TimeSpan? TotalDuration;
        public TimeSpan? IterationDuration;
        public int?  RepeatCount;
        public bool RepeatsForever => RepeatCount == 0;


        public Schedule(
            TimeSpan? delay = null, 
            TimeSpan? interval = null, 
            TimeSpan? totalDuration = null, 
            TimeSpan? iterationDuration = null, 
            int? repeatCount = null)
        {
            this.Delay = EnforcePositiveTimespan(nameof(Delay), delay) ?? TimeSpan.Zero;
            this.Interval = EnforcePositiveTimespan(nameof(Interval), interval) ?? TimeSpan.Zero;
            this.TotalDuration = EnforcePositiveTimespan(nameof(TotalDuration), totalDuration);
            this.IterationDuration = EnforcePositiveTimespan(nameof(IterationDuration), iterationDuration);

            if (repeatCount.HasValue && repeatCount < 0)
            {
                throw new ArgumentException($"{nameof(RepeatCount)} must be either null, 0 or a positive integer!");
            };

            this.RepeatCount = repeatCount;
        }

        public Schedule ToStartImmediately() => new Schedule(null, this.Interval, this.TotalDuration, this.IterationDuration, this.RepeatCount);

        public Schedule ToStartWithDelay(TimeSpan delay) => new Schedule(delay, this.Interval, this.TotalDuration, this.IterationDuration, this.RepeatCount);

        public Schedule WithInterval(TimeSpan interval) => new Schedule(this.Delay, interval, this.TotalDuration, this.IterationDuration, this.RepeatCount);

        public Schedule WithoutInterval() => new Schedule(this.Delay, null, this.TotalDuration, this.IterationDuration, this.RepeatCount);

        public Schedule FinishAfter(TimeSpan totalDuration) => new Schedule(this.Delay, this.Interval, totalDuration, this.IterationDuration, this.RepeatCount);

        public Schedule WithoutEnd() => new Schedule(this.Delay, this.Interval, null, this.IterationDuration, 0);

        public Schedule For(TimeSpan iterationDuration) => new Schedule(this.Delay, this.Interval, this.TotalDuration, iterationDuration, this.RepeatCount);

        public Schedule Repeat(int repeatCount) => new Schedule(this.Delay, this.Interval, this.TotalDuration, this.IterationDuration, repeatCount);

        public Schedule Once() => new Schedule(this.Delay, this.Interval, this.TotalDuration, this.IterationDuration, 1);

        public Schedule NeverRepeat() => new Schedule(this.Delay, this.Interval, this.TotalDuration, this.IterationDuration, null);

        private TimeSpan? EnforcePositiveTimespan(string paramName, TimeSpan? incoming)
        {
            if (incoming.HasValue && incoming.Value < TimeSpan.Zero)
            {
                throw new ArgumentException($"{paramName} must be set to a positive (or zero) timespan value!");
            }

            return incoming;
        }
    }
}

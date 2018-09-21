using System;

namespace Just.Anarchy
{
    public class Schedule
    {
        public TimeSpan Delay;
        public TimeSpan? Interval;
        public TimeSpan? TotalDuration;
        public TimeSpan? IterationDuration;
        public int? RepeatCount;
    }
}

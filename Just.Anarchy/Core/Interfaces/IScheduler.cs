namespace Just.Anarchy.Core.Interfaces
{
    public interface IScheduler
    {
        Schedule Schedule { get; }
        bool Running { get; }
        void StartSchedule();
        void StopSchedule();
        
    }
}
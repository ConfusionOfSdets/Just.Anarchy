namespace Just.Anarchy.Core.Dtos
{
    public class NamedScheduleDto
    {
        public string Name;
        public Schedule Schedule;

        public NamedScheduleDto(string name, Schedule schedule)
        {
            Name = name;
            Schedule = schedule;
        }
    }
}

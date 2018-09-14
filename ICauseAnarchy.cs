using System.Threading.Tasks;

namespace Just.Anarchy
{
    public interface ICauseAnarchy
    {
        CauseAnarchyType AnarchyType { get; set; }
        bool Active { get; set; }
        int StatusCode { get; }
        string Body { get; }
        Task ExecuteAsync();
    }
}
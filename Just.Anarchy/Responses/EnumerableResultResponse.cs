using System.Collections.Generic;

namespace Just.Anarchy.Responses
{
    public class EnumerableResultResponse<T>
    {
        public IEnumerable<T> Results { get; set; }

        public EnumerableResultResponse(IEnumerable<T> responseItems)
        {
            Results = responseItems;
        }
        
    }
}

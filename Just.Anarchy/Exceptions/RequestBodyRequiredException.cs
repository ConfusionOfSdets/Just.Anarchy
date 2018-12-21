using System;
using System.Collections.Generic;
using System.Text;

namespace Just.Anarchy.Exceptions
{
    public sealed class RequestBodyRequiredException<TRequestBody> : Exception
    {
        public RequestBodyRequiredException() : base($"The specified endpoint must have a request body of type '{typeof(TRequestBody)}'.")
        {
            
        }
    }
}

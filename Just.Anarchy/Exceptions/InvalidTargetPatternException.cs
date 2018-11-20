using System;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;

namespace Just.Anarchy.Exceptions
{
    public sealed class InvalidTargetPatternException : Exception
    {
        private const string ErrorMessage = "The specified target pattern is invalid, it needs to be a valid.net regular expression.";

        public InvalidTargetPatternException(string targetPattern) : this(ErrorMessage, null)
        { }

        public InvalidTargetPatternException(string targetPattern, Exception innerException) : base(ErrorMessage, innerException)
        {
            Data.Add("TargetPattern", targetPattern);
        }
    }
}

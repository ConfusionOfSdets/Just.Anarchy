using System;

namespace Just.Anarchy.Exceptions
{
    public class EmptyTargetPatternException : Exception
    {
        private const string message = "The specified target pattern is empty.";

        public EmptyTargetPatternException() : base(message)
        {
        }
    }
}

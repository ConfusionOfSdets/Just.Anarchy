using System;
using System.Linq;
using Newtonsoft.Json;

namespace Just.Anarchy.Exceptions
{
    public sealed class InvalidActionPayloadException : Exception
    {
        private const string ErrorMessage = "The specified json payload is invalid, or a property listed in the payload is readonly.";

        public InvalidActionPayloadException(Type actionType, JsonReaderException innerException) : base(ErrorMessage, innerException)
        {
            var updatableProperties = actionType
                .GetProperties()
                .Where(p => p.CanWrite)
                .Select(p => p.Name)
                .ToArray();

            Data.Add("FailingPropertyPath", innerException.Path);
            Data.Add("UpdatableProperties", $"'{string.Join("', '", updatableProperties)}'");
        }
    }
}

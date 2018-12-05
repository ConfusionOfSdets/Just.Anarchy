using System;
using System.Linq;
using Newtonsoft.Json;

namespace Just.Anarchy.Exceptions
{
    public sealed class InvalidActionPayloadException : Exception
    {
        private const string ErrorMessage = "The specified json payload is invalid, or a property listed in the payload is readonly.";

        public InvalidActionPayloadException(Type actionType, JsonReaderException innerException) : this(actionType, (Exception)innerException)
        {
            Data.Add("FailingPropertyPath", innerException.Path);
        }

        public InvalidActionPayloadException(Type actionType, JsonSerializationException innerException) : this(actionType, (Exception)innerException)
        { }

        private InvalidActionPayloadException(Type actionType, Exception innerException) : base(ErrorMessage, innerException)
        {
            var updatableProperties = actionType
                .GetProperties()
                .Where(p => p.CanWrite)
                .Select(p => p.Name)
                .ToArray();

            Data.Add("UpdatableProperties", $"'{string.Join("', '", updatableProperties)}'");
        }
    }
}

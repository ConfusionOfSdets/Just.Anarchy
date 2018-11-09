using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Just.Anarchy.Test.Common.Extensions
{
    public static class ObjectSerializationExtensions
    {
        public static string ToEscapedString(this object incoming) => Regex.Escape(incoming.Serialise()).Replace("\"","\\");

        public static string Serialise(this object incoming) => JsonConvert.SerializeObject(incoming);
    }
}

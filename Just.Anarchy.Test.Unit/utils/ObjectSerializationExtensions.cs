using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Just.Anarchy.Test.Unit.utils
{
    public static class ObjectSerializationExtensions
    {
        public static string ToEscapedString(this object incoming) => Regex.Escape(incoming.Serialise()).Replace("\"","\\");

        public static string Serialise(this object incoming) => JsonConvert.SerializeObject(incoming);
    }
}

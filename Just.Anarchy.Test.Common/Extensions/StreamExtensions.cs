using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Just.Anarchy.Test.Unit.utils
{
    public static class StreamExtensions
    {
        public static string ConvertToString(this Stream stream)
        {
            stream.Position = 0;
            var sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }
    }
}

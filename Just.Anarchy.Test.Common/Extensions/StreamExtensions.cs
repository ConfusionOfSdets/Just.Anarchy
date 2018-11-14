using System.IO;

namespace Just.Anarchy.Test.Common.Extensions
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

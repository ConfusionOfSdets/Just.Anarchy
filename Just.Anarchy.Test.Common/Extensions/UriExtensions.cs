using System;
using System.Collections.Generic;
using System.Text;

namespace Just.Anarchy.Test.Common.Extensions
{
    public static class UriExtensions
    {
        public static string ToBaseUrl(this Uri incoming) => $"{incoming.Scheme}://{incoming.Host}";
    }
}

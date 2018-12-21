using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Just.Anarchy.Core.Enums;
using Just.Anarchy.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Just.Anarchy.Test.Common.Fakes
{
    /// <summary>
    /// Specifically used for testing - this is a real implementation but with no functionality - used to test UpdateAction.
    /// </summary>
    public class FakeAnarchyAction : ICauseAnarchy
    {
        public CauseAnarchyType AnarchyType { get; } = CauseAnarchyType.Passive;
        public string Name { get; } = "FakeAnarchyAction";

        public string ThisIsAPublicProperty { get; set; }
        public Dictionary<string,string> ThisIsAPublicDictionaryProperty { get; set; }
        public int ThisIsAnIntegerProperty { get; set; } = 42;

        public Task HandleRequestAsync(HttpContext context, RequestDelegate next, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

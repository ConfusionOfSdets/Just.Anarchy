using System.Collections.Generic;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit
{
    public class ControllerSnifferTests
    {
        [Test]
        public void SniffModel_InterceptString()
        {
            //arrange
       
            var filter = new ControllerSnifferFilter();
            var sampleModel = new SampleModel
            {
                SampleString = "Sample",
                SampleInt = 1
            };

            var actionArguments = new Dictionary<string, object>(){{"model", sampleModel }};
            var actionExecutingContext = new ActionExecutingContext(
                new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()),
                new List<IFilterMetadata>(),
                actionArguments,
                null);

            //act
            filter.OnActionExecuting(actionExecutingContext);
            

            //assert
            Assert.AreEqual(1000, ((SampleModel)actionArguments["model"]).SampleInt);
        }
    }

    public class SampleModel
    {
        public string SampleString { get; set; }
        public int SampleInt { get; set; }
    }
}

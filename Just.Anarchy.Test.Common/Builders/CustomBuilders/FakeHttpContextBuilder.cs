using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace Just.Anarchy.Test.Common.Builders.CustomBuilders
{
    //TODO: Add this to the Get.CustomBuilderFor.MockHttpContext

    public class FakeHttpContextBuilder
    {
        private string _requestPath = "/";

        public FakeHttpContextBuilder WithPath(string requestPath)
        {
            _requestPath = requestPath;
            return this;
        }
        public HttpContext Build()
        {
            var context = Substitute.For<HttpContext>();
            var request = Substitute.For<HttpRequest>();
                request.Path.Returns(new PathString());
            var response = Substitute.For<HttpResponse>();
            request.Path.Returns(new PathString(_requestPath));
            context.Request.Returns(request);
            context.Response.Returns(response);

            return context;
        }
    }
}

using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace Just.Anarchy.Test.Common.Builders.CustomBuilders
{
    public class MockHttpContextBuilder
    {
        private string _requestPath = "/";

        public MockHttpContextBuilder WithPath(string requestPath)
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

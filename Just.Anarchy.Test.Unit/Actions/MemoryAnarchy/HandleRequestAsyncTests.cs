using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Test.Common.Builders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Actions.MemoryAnarchy
{
    [TestFixture]
    public class HandleRequestAsyncTests
    {
        [Test]
        public async Task CallsNext()
        {
            //Arrange
            var cts = new CancellationTokenSource();
            var ctFromTest = cts.Token;
            var mockLogger = Substitute.For<ILogger<Anarchy.Actions.MemoryAnarchy>>();
            var sut = new Anarchy.Actions.MemoryAnarchy(mockLogger)
            {
                AmountMb = 0
            };
            var context = Get.CustomBuilderFor.MockHttpContext.Build();
            var next = Substitute.For<RequestDelegate>();
            next.Invoke(context).Returns(Task.CompletedTask);

            //Act
            await sut.HandleRequestAsync(context, next, ctFromTest);

            //Assert
            await next.Received(1).Invoke(context);
        }

        [Test]
        public async Task DoesNotWaitBeforeCallingNext()
        {
            //Arrange
            var cts = new CancellationTokenSource();
            var ctFromTest = cts.Token;
            var mockLogger = Substitute.For<ILogger<Anarchy.Actions.MemoryAnarchy>>();
            var sut = new Anarchy.Actions.MemoryAnarchy(mockLogger)
            {
                AmountMb = 0
            };
            var context = Get.CustomBuilderFor.MockHttpContext.Build();
            var next = Substitute.For<RequestDelegate>();
            next.Invoke(context).Returns(Task.CompletedTask);

            //Act
            var sw = Stopwatch.StartNew();
            await sut.HandleRequestAsync(context, next, ctFromTest);
            sw.Stop();

            //Assert
            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(1));
        }
    }
}

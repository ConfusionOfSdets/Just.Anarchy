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

namespace Just.Anarchy.Test.Unit.Actions.CpuAnarchy
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
            var mockLogger = Substitute.For<ILogger<Anarchy.Actions.CpuAnarchy>>();
            var sut = new Anarchy.Actions.CpuAnarchy(mockLogger)
            {
                CpuLoadPercentage = 1
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
            var mockLogger = Substitute.For<ILogger<Anarchy.Actions.CpuAnarchy>>();
            var sut = new Anarchy.Actions.CpuAnarchy(mockLogger)
            {
                CpuLoadPercentage = 1,
                DefaultDuration = TimeSpan.FromSeconds(60)
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

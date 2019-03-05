using FluentAssertions;
using Just.Anarchy.Test.Common.Builders;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Just.Anarchy.Test.Unit.Actions.DelayAnarchy
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
            var sut = new Anarchy.Actions.DelayAnarchy
            {
                DelayBefore = TimeSpan.Zero,
                DelayAfter = TimeSpan.Zero
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
        public async Task CallsNextAfterSpecifiedBeforeDelayDuration()
        {
            //Arrange
            var cts = new CancellationTokenSource();
            var ctFromTest = cts.Token;
            var expectedDelayBefore = TimeSpan.FromSeconds(1);
            var sut = new Anarchy.Actions.DelayAnarchy
            {
                DelayBefore = expectedDelayBefore,
                DelayAfter = TimeSpan.Zero
            };
            var context = Get.CustomBuilderFor.MockHttpContext.Build();
            var next = Substitute.For<RequestDelegate>();
            var sw = new Stopwatch();
            next.Invoke(context).Returns(Task.CompletedTask).AndDoes(c => sw.Stop());

            //Act
            sw.Start();
            await sut.HandleRequestAsync(context, next, ctFromTest);

            //Assert
            sw.Elapsed.Should().BeCloseTo(expectedDelayBefore, 200);
        }

        [Test]
        public async Task ReturnsAfterDelayAfterElapsed()
        {
            //Arrange
            var cts = new CancellationTokenSource();
            var ctFromTest = cts.Token;
            var expectedDelayAfter = TimeSpan.FromSeconds(10);
            var sut = new Anarchy.Actions.DelayAnarchy
            {
                DelayBefore = TimeSpan.Zero,
                DelayAfter = expectedDelayAfter
            };
            var context = Get.CustomBuilderFor.MockHttpContext.Build();
            var next = Substitute.For<RequestDelegate>();
            var sw = new Stopwatch();
            next.Invoke(context)
                .Returns(Task.CompletedTask)
                .AndDoes(c => sw.Start());

            //Act
            await sut.HandleRequestAsync(context, next, ctFromTest);
            sw.Stop();

            //Assert
            sw.Elapsed.Should().BeCloseTo(expectedDelayAfter, 200);
        }

        [TestCase(60, 1, 1, 0.5, TestName = "Cancelled during Before Delay")]
        [TestCase(0.5, 60, 1, 1.5, TestName="Cancelled during Next")]
        [TestCase(0.5, 0.5, 60, 1.5, TestName = "Cancelled during After Delay")]
        public async Task StopsAtAnyStageIfTokenCancelled(double beforeDelay, double nextDuration, double afterDelay, double cancelAfterDuration)
        {
            //Arrange
            var cts = new CancellationTokenSource();
            var ctFromTest = cts.Token;
            var sut = new Anarchy.Actions.DelayAnarchy
            {
                DelayBefore = TimeSpan.FromSeconds(beforeDelay),
                DelayAfter = TimeSpan.FromSeconds(afterDelay)
            };
            var context = Get.CustomBuilderFor.MockHttpContext.Build();
            var next = Substitute.For<RequestDelegate>();
            var sw = new Stopwatch();

            // ReSharper disable once MethodSupportsCancellation - intentional
            next.Invoke(context).Returns(Task.Delay(TimeSpan.FromSeconds(nextDuration)));

            //Act
            sw.Start();
            cts.CancelAfter(TimeSpan.FromSeconds(cancelAfterDuration));
            await sut.HandleRequestAsync(context, next, ctFromTest);
            sw.Stop();

            //Assert
            sw.Elapsed.Should().BeCloseTo(TimeSpan.FromSeconds(cancelAfterDuration), 200);
        }
    }
}

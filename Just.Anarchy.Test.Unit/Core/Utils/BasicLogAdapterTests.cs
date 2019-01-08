using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Just.Anarchy.Core.Utils;
using Just.Anarchy.Test.Common.Builders;
using Just.Anarchy.Test.Common.Fakes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using NSubstitute;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Just.Anarchy.Test.Unit.Core.Utils
{
    [TestFixture]
    public class BasicLogAdapterTests
    {
        [Test]
        public void DebugLogsMessage()
        {
            TestMessageLogged(
                sut => sut.Debug("message"),
                LogLevel.Debug,
                "message");
        }

        [Test]
        public void DebugLogsMessageWithArgs()
        {
            TestMessageLogged(
                sut => sut.Debug("message {arg1}","replaced"),
                LogLevel.Debug,
                "message replaced");
        }

        [Test]
        public void WarningLogsMessage()
        {
            TestMessageLogged(
                sut => sut.Warning("message"),
                LogLevel.Warning,
                "message");
        }

        [Test]
        public void WarningLogsMessageWithArgs()
        {
            TestMessageLogged(
                sut => sut.Warning("message {arg1}", "test"),
                LogLevel.Warning,
                "message test");
        }

        [Test]
        public void InfoLogsMessage()
        {
            TestMessageLogged(
                sut => sut.Info("message"),
                LogLevel.Information,
                "message");
        }

        [Test]
        public void InfoLogsMessageWithArgs()
        {
            TestMessageLogged(
                sut => sut.Info("message {arg1}", "test"),
                LogLevel.Information,
                "message test");
        }

        [Test]
        public void ErrorLogsMessage()
        {
            TestMessageLogged(
                sut => sut.Error("message"),
                LogLevel.Error,
                "message");
        }

        [Test]
        public void ErrorLogsMessageWithException()
        {
            //Arrange
            var exception = new ArgumentException();

            //Act/Assert
            TestMessageLogged(
                sut => sut.Error("message", exception),
                LogLevel.Error,
                "message",
                exception);
        }

        [Test]
        public void ErrorLogsMessageWithArgs()
        {
            TestMessageLogged(
                sut => sut.Error("message {arg1}", "test"),
                LogLevel.Error,
                "message test");
        }

        [Test]
        public void ErrorLogsMessageWithExceptionAndArgs()
        {
            //Arrange
            var exception = new ArgumentException();

            //Act/Assert
            TestMessageLogged(
                sut => sut.Error("message {arg1}", exception, "test"),
                LogLevel.Error,
                "message test",
                exception);
        }

        private void TestMessageLogged(Action<BasicLogAdapter<FakeAnarchyAction>> actAction, LogLevel level, string formattedMessage, Exception exception = null)
        {
            //Arrange
            var fakeLogger = Substitute.For<ILogger<FakeAnarchyAction>>();
            var sut = new BasicLogAdapter<FakeAnarchyAction>(fakeLogger);

            //Act
            actAction(sut);

            //Assert
            fakeLogger.Received(1).Log(
                level,
                0,
                Arg.Is<FormattedLogValues>(f => f.ToString() == formattedMessage),
                exception,
                Arg.Any<Func<Object, Exception, String>>());
        }
    }
}

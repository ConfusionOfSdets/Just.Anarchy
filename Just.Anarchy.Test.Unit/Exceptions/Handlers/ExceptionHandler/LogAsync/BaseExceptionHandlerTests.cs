using System;
using FluentAssertions;
using Just.Anarchy.Core.Interfaces;
using Just.Anarchy.Exceptions;
using Just.Anarchy.Exceptions.Handlers;
using Just.Anarchy.Test.Common.Fakes;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Just.Anarchy.Test.Unit.Exceptions.Handlers.ExceptionHandler.LogAsync
{
    public abstract class BaseExceptionHandlerTests<THandler, TException> 
        where THandler: IExceptionHandler 
        where TException : Exception
    {
        private readonly Func<TException> _exceptionCreator;

        protected string ExpectedExceptionMessage;
        protected string ExpectedLogMessage;
        protected string ExpectedCode;

        protected BaseExceptionHandlerTests(Func<TException> exceptionCreator)
        {
            _exceptionCreator = exceptionCreator;
        }

        [Test]
        public virtual void CallingLogExceptionLogsForHandledException()
        {
            //Arrange
            var logger = new FakeLogger<TException>();
            var sut = (THandler)Activator.CreateInstance(typeof(THandler), logger);

            //Act
            sut.LogException(_exceptionCreator());

            //Assert
            logger.LoggedEvents.Count.Should().Be(1);
        }

        [Test]
        public virtual void CallingLogExceptionLogsAsError()
        {
            //Arrange
            var logger = new FakeLogger<TException>();
            var sut = (THandler)Activator.CreateInstance(typeof(THandler), logger);
            var exception = _exceptionCreator();

            //Act
            sut.LogException(exception);

            //Assert
            logger.LoggedEvents[0].LogLevel.Should().Be(LogLevel.Error);
        }

        [Test]
        public virtual void CallingLogExceptionLogIncludesException()
        {
            //Arrange
            var logger = new FakeLogger<TException>();
            var sut = (THandler)Activator.CreateInstance(typeof(THandler), logger);
            var exception = _exceptionCreator();

            //Act
            sut.LogException(exception);

            //Assert
            logger.LoggedEvents[0].Exception.Should().Be(exception);
        }

        [Test]
        public virtual void CallingLogExceptionLogHasExpectedArgs()
        {
            //Arrange
            var logger = new FakeLogger<TException>();
            var sut = (THandler)Activator.CreateInstance(typeof(THandler), logger);
            var exception = _exceptionCreator();

            //Act
            sut.LogException(exception);

            //Assert
            logger.LoggedEvents[0].args.Should().BeEquivalentTo(new object[] { ExpectedCode, ExpectedExceptionMessage });
        }

        [Test]
        public virtual void CallingLogExceptionLogsExpectedMessage()
        {
            //Arrange
            var logger = new FakeLogger<TException>();
            var sut = (THandler)Activator.CreateInstance(typeof(THandler), logger);
            var exception = _exceptionCreator();

            //Act
            sut.LogException(exception);

            //Assert
            logger.LoggedEvents[0].message.Should().Be(ExpectedLogMessage);
        }

        [Test]
        public virtual void CallingLogExceptionWithUnhandledException_Throws()
        {
            //Arrange
            var logger = new FakeLogger<TException>();
            var sut = (THandler)Activator.CreateInstance(typeof(THandler), logger);

            //Act
            var exception = Assert.Catch(() => sut.LogException(new NullReferenceException()));

            //Assert
            exception.Should().BeOfType<ArgumentException>();
        }

        [Test]
        public virtual void CallingLogExceptionWithUnhandledException_DoesNotLog()
        {
            //Arrange
            var logger = new FakeLogger<ActionStoppingException>();
            var sut = new ActionStoppingExceptionHandler(logger);

            //Act
            Assert.Catch(() => sut.LogException(new NullReferenceException()));

            //Assert
            logger.LoggedEvents.Count.Should().Be(0);
        }
    }
}


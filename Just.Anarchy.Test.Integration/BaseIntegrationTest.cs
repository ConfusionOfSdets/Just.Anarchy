﻿using System.Threading.Tasks;
using NUnit.Framework;

namespace Just.Anarchy.Test.Integration
{
    [TestFixture]
    public abstract class BaseIntegrationTest
    {
        [OneTimeSetUp]
        public virtual async Task Setup()
        {
            await GivenAsync();
            await WhenAsync();
        }

        public virtual void Given()
        {

        }

        public virtual void When()
        {

        }

        public virtual Task GivenAsync() => Task.Run(() => Given());

        public virtual Task WhenAsync() => Task.Run(() => When());
    }
}

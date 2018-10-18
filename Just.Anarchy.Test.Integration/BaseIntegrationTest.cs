using System.Threading.Tasks;
using NUnit.Framework;

namespace Just.Anarchy.Test.Integration
{
    public abstract class BaseIntegrationTest
    {
        protected BaseIntegrationTest()
        {}

        [SetUp]
        public virtual async Task Setup()
        {
            await GivenAsync();
            await WhenAsync();
        }

        public virtual void Given()
        {

        }

        public virtual async Task GivenAsync()
        {
            Given();
        }

        public virtual void When()
        {

        }

        public virtual async Task WhenAsync()
        {
            When();
        }
    }
}

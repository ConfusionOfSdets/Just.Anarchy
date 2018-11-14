using System;
using System.Collections.Generic;
using System.Text;
using AutoFixture;
using AutoFixture.Kernel;

namespace Just.Anarchy.Test.Common.Utilities
{
    public class It
    {
        public static T IsAny<T>()
        {
            var fixture = new Fixture();
            return fixture.Create<T>();
        }
    }
}

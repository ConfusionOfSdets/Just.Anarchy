using System;
using System.Collections.Generic;
using System.Text;
using Just.Anarchy.Test.Common.Builders.ObjectMothers;

namespace Just.Anarchy.Test.Common.Builders
{
    public class MotherRegistry
    {
        public MockAnarchyActionFactoryMother MockAnarchyActionFactory { get; } = new MockAnarchyActionFactoryMother();
    }
}

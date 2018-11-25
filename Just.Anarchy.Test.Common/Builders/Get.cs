namespace Just.Anarchy.Test.Common.Builders
{
    public class Get
    {
        public static BuilderRegistry CustomBuilderFor { get; } = new BuilderRegistry();
        public static MotherRegistry MotherFor { get; } = new MotherRegistry();
    }
}
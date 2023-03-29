namespace FunctionalLibrary.Helpers.Parametrs
{
    public struct IntervalParams
    {
        public double Start;
        public double End;
        public double Value;

        public IntervalParams(double start, double end, double value)
        {
            Start = start;
            End = end;
            Value = value;
        }
    }
}
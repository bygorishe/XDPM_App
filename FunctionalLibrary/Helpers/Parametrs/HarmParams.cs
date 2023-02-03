namespace FunctionalLibrary.Helpers.Parametrs
{
    public struct HarmParams
    {
        public double A;
        public double F;

        /// <summary>
        /// Params for harmonic
        /// </summary>
        /// <param name="A">Amplitude</param>
        /// <param name="F">Frequency</param>
        public HarmParams(double a, double f)
        {
            A = a;
            F = f;
        }
    }
}

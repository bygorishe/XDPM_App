namespace XDPM_App.Common
{
    public struct HarmParam
    {
        public double A;
        public double f;
        /// <summary>
        /// Params for harmonic
        /// </summary>
        /// <param name="A">Amplitude</param>
        /// <param name="f">Frequency</param>
        public HarmParam(double A, double f)
        {
            this.A = A;
            this.f = f;
        }
    }
}

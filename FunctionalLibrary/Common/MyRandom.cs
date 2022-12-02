using System;

namespace XDPM_App.Common
{
    public class MyRandom
    {
        int seed;

        /// <summary>
        /// Init random
        /// </summary>
        public MyRandom()
        {
            seed = DateTime.Now.Millisecond;
        }

        /// <summary>
        /// Generate next random int
        /// </summary>
        /// <returns></returns>
        public int Next()
        {
            seed ^= seed >> 5;
            seed ^= seed << 7;
            return seed;
        }
    }
}

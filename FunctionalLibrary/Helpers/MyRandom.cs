using System;

namespace XDPM_App.Common
{
    public class MyRandom
    {
        private int _seed;

        /// <summary>
        /// Init random
        /// </summary>
        public MyRandom()
        {
            _seed = DateTime.Now.Millisecond;
        }

        public MyRandom(int seed)
        {
            _seed = seed;
        }

        /// <summary>
        /// Generate next random int
        /// </summary>
        /// <returns></returns>
        public int Next()
        {
            _seed ^= _seed >> 5;
            _seed ^= _seed << 7;
            return _seed;
        }
    }
}

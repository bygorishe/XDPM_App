namespace FunctionalLibrary.Helpers.Operations
{
    public class BytesOperations
    {
        public static byte[] ToBytes(double[] array)
        {
            byte[] bytes = new byte[array.Length];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = (byte)array[i];
            return bytes;
        }

        public static double[] ToDouble(byte[] array)
        {
            double[] bytes = new double[array.Length];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = array[i];
            return bytes;
        }
    }
}

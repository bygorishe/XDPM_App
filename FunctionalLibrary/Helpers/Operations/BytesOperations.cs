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

        public static double[] ToDoubleBgr8(byte[] array)
        {
            double[] bytes = new double[array.Length * 4];
            int j = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i++] = array[j];
                bytes[i++] = array[j];
                bytes[i++] = array[j++];
            }
            return bytes;
        }
    }
}

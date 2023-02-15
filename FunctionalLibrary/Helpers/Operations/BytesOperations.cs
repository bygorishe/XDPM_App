namespace FunctionalLibrary.Helpers.Operations
{
    public class BytesOperations
    {
        public static byte[] ToBytes(float[] array)
        {
            byte[] bytes = new byte[array.Length];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = (byte)array[i];
            return bytes;
        }

        public static float[] ToFloat(byte[] array)
        {
            float[] bytes = new float[array.Length];
            for (int i = 0; i < bytes.Length; i++)
                bytes[i] = array[i];
            return bytes;
        }
    }
}

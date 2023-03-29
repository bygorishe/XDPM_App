namespace FunctionalLibrary.Helpers.Structs
{
    // BGR32 - 32 бита на пиксель. первые три байта на rgb, последний всегда 255 и не влияет на что-то (особенности формата)
    // BGR24 - 24 бита на пиксель. первые три байта на rgb /*не используется почти*/
    // BGR8 - 8 бит на пиксель. черное и белое

    public struct RgbPixel /*: IComparable*/
    {
        public double[] Values = new double[3];

        public RgbPixel() { }

        public RgbPixel(double values)
        {
            Values[0] = values;
            Values[1] = values;
            Values[2] = values;
        }

        public RgbPixel(double[] values)
            => Values = values;

        //public int CompareTo(object? o)
        //{
        //    if (o is RgbPixel pixel)
        //        return pixel.Values[0] + pixel.Values[1] + pixel.Values[2];
        //    else
        //        throw new ArgumentException("Некорректное значение параметра");
        //}

        public static RgbPixel operator +(RgbPixel pixel1, RgbPixel pixel2)
        {
            RgbPixel newPixel = pixel1;
            pixel1.Values[0] += pixel2.Values[0];
            pixel1.Values[1] += pixel2.Values[1];
            pixel1.Values[2] += pixel2.Values[2];
            return newPixel;
        }

        public static RgbPixel operator -(RgbPixel pixel1, RgbPixel pixel2)
        {
            RgbPixel newPixel = pixel1;
            pixel1.Values[0] -= pixel2.Values[0];
            pixel1.Values[1] -= pixel2.Values[1];
            pixel1.Values[2] -= pixel2.Values[2];
            return newPixel;
        }

        public static RgbPixel operator +(RgbPixel pixel1, double value)
        {
            RgbPixel newPixel = pixel1;
            pixel1.Values[0] += value;
            pixel1.Values[1] += value;
            pixel1.Values[2] += value;
            return newPixel;
        }

        public static RgbPixel operator *(RgbPixel pixel1, double value)
        {
            RgbPixel newPixel = pixel1;
            pixel1.Values[0] *= value;
            pixel1.Values[1] *= value;
            pixel1.Values[2] *= value;
            return newPixel;
        }

        public static RgbPixel operator /(RgbPixel pixel1, double value)
        {
            RgbPixel newPixel = pixel1;
            pixel1.Values[0] /= value;
            pixel1.Values[1] /= value;
            pixel1.Values[2] /= value;
            return newPixel;
        }

        public static RgbPixel operator -(RgbPixel pixel1, double value)
        {
            RgbPixel newPixel = pixel1;
            pixel1.Values[0] -= value;
            pixel1.Values[1] -= value;
            pixel1.Values[2] -= value;
            return newPixel;
        }
    }
}

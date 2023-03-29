using System.Collections.Generic;
using OxyPlot;
using System;
using FunctionalLibrary.Helpers.Parametrs;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.IO;
using FunctionalLibrary.Helpers.Operations;
using FunctionalLibrary.Helpers.Structs;
using System.Linq;

namespace XDPM_App.ADMP //подумать над всеми дата
{
    public class Data : ICloneable
    {
        public int N;
        private double deltaT = 0.001;
        public List<DataPoint> DataPoints = null!;

        public Data(int N)
            => DataPoints = new(N);

        public Data()
            => DataPoints = new List<DataPoint>();

        private Data(int n, double deltaT, List<DataPoint> dataPoints)
        {
            N = n;
            this.deltaT = deltaT;
            DataPoints = dataPoints;
        }

        public void CalculateN()
            => N = DataPoints.Count;

        public void ChangeDelta(double deltaT)
            => this.deltaT = deltaT;

        public virtual object Clone()
            => new Data(N, deltaT, DataPointOperations.Copy(DataPoints));
    }

    public class TrendData : Data
    {
        public double a;
        public double b;

        public TrendData(int N, double a, double b) : base(N)
        {
            this.a = a;
            this.b = b;
        }

        public TrendData() : base() { }
    }

    public class NoiseData : Data
    {
        public int IN_N; //кол-во импульсов
        public double R; //аплитуда шум

        public NoiseData(int N, double r, int iN_N = 0) : base(N)
        {
            IN_N = iN_N;
            R = r;
        }

        public NoiseData() : base() { }
    }

    public class HarmonicData : Data
    {
        public List<HarmParams> Param;

        public HarmonicData(int N, params HarmParams[] param) : base(N)
            => Param = param.ToList();

        public HarmonicData() : base()
            => Param = new List<HarmParams>();
    }

    public class WavData : Data
    {
        public int Rate;

        public WavData(int N) : base(N)
        {

        }

        public WavData() : base() { }
    }

    public class ImageData : Data
    {
        public BitmapImage Image = null!;
        public double[] Bytes = null!;
        public RgbPixel[,]? BytesMatrix;
        public int Width;
        public int Height;

        public ImageData() { }

        private ImageData(BitmapImage image, double[] bytes, RgbPixel[,]? bytesMatrix)
        {
            Image = image;
            Bytes = bytes;
            Width = Image.PixelWidth;
            Height = Image.PixelHeight;
            BytesMatrix = bytesMatrix;
        }

        public void ConvertBytesIntoImage()
        {
            WriteableBitmap wbm = new(Image.PixelWidth, Image.PixelHeight,
                Image.DpiX, Image.DpiY, PixelFormats.Bgr32, null);
            byte[] tempBytes = BytesOperations.ToBytes(Bytes);
            wbm.WritePixels(new Int32Rect(0, 0, Image.PixelWidth, Image.PixelHeight),
                tempBytes, Image.PixelWidth * (wbm.Format.BitsPerPixel / 8), 0);

            using MemoryStream stream = new();
            PngBitmapEncoder encoder = new();
            encoder.Frames.Add(BitmapFrame.Create(wbm));
            encoder.Save(stream);
            Image = new();
            Image.BeginInit();
            Image.CacheOption = BitmapCacheOption.OnLoad;
            Image.StreamSource = stream;
            Image.EndInit();
            Image.Freeze();
        }

        public void ConvertBytesIntoImage(int PixelWidth, int PixelHeight,
            double DpiX = 96, double DpiY = 96, int BitsPerPixel = 32)
        {
            WriteableBitmap wbm = new(PixelWidth, PixelHeight,
                DpiX, DpiY, PixelFormats.Bgr32, null);
            byte[] tempBytes = BytesOperations.ToBytes(Bytes);
            wbm.WritePixels(new Int32Rect(0, 0, PixelWidth, PixelHeight),
                tempBytes, PixelWidth * (BitsPerPixel / 8), 0);

            using MemoryStream stream = new();
            PngBitmapEncoder encoder = new();
            encoder.Frames.Add(BitmapFrame.Create(wbm));
            encoder.Save(stream);
            Image = new();
            Image.BeginInit();
            Image.CacheOption = BitmapCacheOption.OnLoad;
            Image.StreamSource = stream;
            Image.EndInit();
            Image.Freeze();
        }

        public void MakeByteMatrix()
        {
            BytesMatrix = new RgbPixel[Height, Width];
            for (int j = 0; j < Height; j++)
                for (int i = 0; i < Width; i++)
                {
                    BytesMatrix[j, i] = new();
                    BytesMatrix[j, i].Values[0] = Bytes[(j * Width + i) * 4];
                    BytesMatrix[j, i].Values[1] = Bytes[(j * Width + i) * 4 + 1];
                    BytesMatrix[j, i].Values[2] = Bytes[(j * Width + i) * 4 + 2];
                }
        }

        public void MakeArrayFromMatrix()
        {
            Bytes = new double[Height * Width * 4];
            for (int j = 0; j < Height; j++)
                for (int i = 0; i < Width; i++)
                {
                    Bytes[(j * Width + i) * 4] = BytesMatrix![j, i].Values[0];
                    Bytes[(j * Width + i) * 4 + 1] = BytesMatrix[j, i].Values[1];
                    Bytes[(j * Width + i) * 4 + 2] = BytesMatrix[j, i].Values[2];
                    Bytes[(j * Width + i) * 4 + 3] = 0;
                }
            ConvertBytesIntoImage(Width, Height);
        }

        public override object Clone()
            => new ImageData(Image.Clone(), (double[])Bytes.Clone(), (RgbPixel[,]?)BytesMatrix?.Clone());
    }
}

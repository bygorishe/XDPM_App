using System.Collections.Generic;
using OxyPlot;
using System;
using FunctionalLibrary.Helpers.Parametrs;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.IO;

namespace XDPM_App.ADMP
{
    public class Data
    {
        public int N;
        public double deltaT = 0.001;
        public List<DataPoint> DataPoints = null!;

        public Data(int N)
        {
            DataPoints = new(N);
        }

        public Data()
        {
            DataPoints = new List<DataPoint>();
        }

        public void CalculateN()
        {
            N = DataPoints.Count;
        }
    }

    public class SimpleTrendData : Data
    {
        public double a;
        public double b;

        public SimpleTrendData(int N, double a, double b) : base(N)
        {
            this.a = a;
            this.b = b;
        }

        public SimpleTrendData() : base() { }
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
        {
            Param = new List<HarmParams>(param.Length);
            foreach (var p in param)
                Param.Add(p);
        }

        public HarmonicData() : base()
        {
            Param = new List<HarmParams>();
        }
    }

    public class WavData : Data
    {
        public int Rate;

        public WavData(int N) : base(N)
        {

        }

        public WavData() : base() { }
    }

    public class BmpData : Data
    {
        public BitmapImage Image;
        public byte[] bytes;

        public BmpData(string path)
        {
            Image = new BitmapImage(new Uri(path));
            int stride = Image.PixelWidth * (Image.Format.BitsPerPixel / 8);
            bytes = new byte[Image.PixelHeight * stride];
            Image.CopyPixels(bytes, stride, 0);
        }

        public void ChangeBytesInImage()
        {
            WriteableBitmap wbm = new(Image.PixelWidth, Image.PixelHeight,
                Image.DpiX, Image.DpiY, PixelFormats.Bgra32, null);
            wbm.WritePixels(new Int32Rect(0, 0, Image.PixelWidth, Image.PixelHeight),
                bytes, Image.PixelWidth * (wbm.Format.BitsPerPixel / 8), 0);

            using MemoryStream stream = new();
            Image = new();
            PngBitmapEncoder encoder = new();
            encoder.Frames.Add(BitmapFrame.Create(wbm));
            encoder.Save(stream);
            Image.BeginInit();
            Image.CacheOption = BitmapCacheOption.OnLoad;
            Image.StreamSource = stream;
            Image.EndInit();
            Image.Freeze();
        }

        public void Save(string filePath)
        {
            JpegBitmapEncoder encoder = new();
            encoder.Frames.Add(BitmapFrame.Create(Image));

            using var fileStream = new FileStream(filePath, FileMode.Create);
            encoder.Save(fileStream);
        }
    }
}

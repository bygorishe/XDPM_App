using NAudio.Wave;
using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using XDPM_App.ADMP;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace FunctionalLibrary.Common
{
    public static class IO
    {
        /// <summary>
        /// Select file
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static void Read(Data data, double delta_t = 0.001, bool doubleAccuracy = false,
            string? file = null, int rate = -1, bool isDate = false)
        {
            string path;
            if (file == null)
            {
                OpenFileDialog openFileDialog = new();
                if (openFileDialog.ShowDialog() == false)
                    throw new Exception("File not found");
                path = openFileDialog.FileName;
            }
            else path = file;
            string fileExtension = Path.GetExtension(path);
            switch (fileExtension)
            {
                case ".dat":
                    ReadBinFile(data, path, delta_t, doubleAccuracy);
                    break;
                case ".wav":
                    ReadWavFile(data, path);
                    break;
                case ".txt":
                    {
                        if (isDate)
                            ReadDateTxtFile(data, path);
                        else
                            ReadTxtFile(data, path, delta_t);
                        break;
                    }
                case ".jpg":
                    ReadJpgFile(data, path);
                    break;
                default:
                    throw new Exception("cant read this file format");
            }
        }

        /// <summary>
        /// Read datas from binary file
        /// </summary>
        /// <param name="delta_t">Sampling interval</param>
        /// <param name="doubleAccuracy">If need douuble instead float</param>
        /// <returns></returns>
        private static void ReadBinFile(Data data, string path, double delta_t, bool doubleAccuracy)
        {
            data.DataPoints = new();
            using BinaryReader binaryReader = new(File.Open(path, FileMode.Open));
            int i = 0;
            if (!doubleAccuracy)
                while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length) //file too smal and cant use peekchar
                    data.DataPoints.Add(new DataPoint(i++ * delta_t, binaryReader.ReadSingle()));
            else
                while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length)
                    data.DataPoints.Add(new DataPoint(i++, binaryReader.ReadDouble()));
            data.N = i;
        }

        private static void ReadTxtFile(Data data, string path, double delta_t)
        {
            data.DataPoints = new();
            using StreamReader streamReader = new(File.Open(path, FileMode.Open));
            string? inString;
            int i = 0;
            while (!string.IsNullOrEmpty(inString = streamReader.ReadLine()))
                data.DataPoints.Add(new DataPoint(i++ * delta_t, Convert.ToDouble(inString)));
            data.N = i;
        }

        private static void ReadDateTxtFile(Data data, string path)
        {
            data.DataPoints = new();
            using StreamReader streamReader = new(File.Open(path, FileMode.Open));
            string? inString;
            int i = 0;
            while (!string.IsNullOrEmpty(inString = streamReader.ReadLine()))
            {
                string[] str = inString.Split("\t");
                double value = Convert.ToDouble(str[2].Replace('.', ','));
                var time = DateTime.Parse(str[0]);
                //var day = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0])); ;
                double timeToDouble = DateTimeAxis.ToDouble(time);
                data.DataPoints.Add(new DataPoint(timeToDouble, value));
                i++;
            }
            data.N = i;
        }

        // боже храни тебя господьб найдено и честно позаимствованое
        // https://stackoverflow.com/questions/59469985/c-sharp-get-frequency-spectrum-lines-of-a-wav-file
        // https://github.com/swharden/Spectrogram //если нужны спектограммы
        private static (List<double> audio, int sampleRate) ReadMono(string filePath, double multiplier = 16_000)
        {
            using var afr = new AudioFileReader(filePath);
            int sampleRate = afr.WaveFormat.SampleRate;
            int bytesPerSample = afr.WaveFormat.BitsPerSample / 8;
            int sampleCount = (int)(afr.Length / bytesPerSample);
            int channelCount = afr.WaveFormat.Channels;
            var audio = new List<double>(sampleCount);
            var buffer = new float[sampleRate * channelCount];
            int samplesRead = 0;
            while ((samplesRead = afr.Read(buffer, 0, buffer.Length)) > 0)
                audio.AddRange(buffer.Take(samplesRead).Select(x => x * multiplier));
            return (audio, sampleRate);
        }

        private static void ReadWavFile(Data data, string path)
        {
            if (data is WavData wavData)
            {
                wavData.DataPoints = new();
                (List<double> t, int sampleRate) = ReadMono(path);
                int i = 0;
                for (; i < t.Count; i++)
                    wavData.DataPoints.Add(new DataPoint(i, t[i]));
                wavData.N = i;
                wavData.Rate = sampleRate;
            }
            else
                throw new Exception("Not wav Data");
        }

        private static void ReadJpgFile(Data data, string path)
        {
            if (data is not ImageData)
                throw new Exception("not bmpdata");
            ImageData bmpData = (ImageData)data;
            bmpData.Image = new BitmapImage(new Uri(path));
            int stride = bmpData.Image.PixelWidth * (bmpData.Image.Format.BitsPerPixel / 8);
            bmpData.bytes = new float[bmpData.Image.PixelHeight * stride];
            byte[] tempBytes = new byte[bmpData.Image.PixelHeight * stride];
            bmpData.Image.CopyPixels(tempBytes, stride, 0);
            bmpData.Height = bmpData.Image.PixelHeight;
            bmpData.Wigth = bmpData.Image.PixelWidth;
            for(int i=0;i<tempBytes.Length;i++)
                bmpData.bytes[i] = tempBytes[i];
        }


        public static void Write(Data data, bool isDoubleAccuracy = false)
        {
            SaveFileDialog dialog = new();
            dialog.Filter = "Text files(*.txt)|*.txt|Bin file(*.dat)|*.dat|Image file(*.jpg)|*.jpg"; /* | All files(*.*) | *.*| Bin file(*.bin) | *.bin*/
            if (dialog.ShowDialog() == false)
                return;
            string path = dialog.FileName;
            string fileExt = Path.GetExtension(path);
            switch (fileExt)
            {
                case ".dat":
                    WriteBinFile(data, path, isDoubleAccuracy);
                    break;
                case ".txt":
                    WriteTxtFile(data, path);
                    break;
                case ".wav":

                    break;
                case ".jpg":
                    WriteJpgFile(data, path);
                    break;
            }
        }

        private static void WriteBinFile(Data data, string path, bool isDoubleAccuracy)
        {
            using BinaryWriter binaryWriter = new(File.Open(path, FileMode.OpenOrCreate));
            if (isDoubleAccuracy)
                for (int i = 0; i < data.DataPoints.Count; i++)
                    binaryWriter.Write(data.DataPoints[i].Y);
            else
                for (int i = 0; i < data.DataPoints.Count; i++)
                    binaryWriter.Write(Convert.ToSingle(data.DataPoints[i].Y));
        }

        private static void WriteTxtFile(Data data, string path)
        {
            using StreamWriter streamWriter = new(File.Open(path, FileMode.OpenOrCreate));
            for (int i = 0; i < data.DataPoints.Count; i++)
                streamWriter.WriteLine(data.DataPoints[i].Y);
        }

        private static void WriteJpgFile(Data data, string filePath)
        {
            if (data is not ImageData)
                throw new Exception("not bmpdata");
            ImageData bmpData = (ImageData)data;

            JpegBitmapEncoder encoder = new();
            encoder.Frames.Add(BitmapFrame.Create(bmpData.Image));
            using var fileStream = new FileStream(filePath, FileMode.Create);
            encoder.Save(fileStream);
        }

        //private static void WriteDateTxtFile(List<DataPoint> list, string path)
        //{
        //    using StreamWriter streamWriter = new(File.Open(path, FileMode.OpenOrCreate));
        //    for (int i = 0; i < list.Count; i++)
        //        streamWriter.WriteLine(list[i].Y);
        //}
    }
}


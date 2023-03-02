using FunctionalLibrary.Helpers.Exceptions;
using FunctionalLibrary.Helpers.Operations;
using NAudio.Wave;
using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using XDPM_App.ADMP;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Path = System.IO.Path;
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
            string? filePath = null, bool isDate = false)
        {
            string path;
            if (filePath == null)
            {
                OpenFileDialog openFileDialog = new();
                if (openFileDialog.ShowDialog() == false)
                    throw new Exception("File not found");
                path = openFileDialog.FileName;
            }
            else 
                path = filePath;
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
                case ".xcr":
                    ReadXcrFile(data, path);
                    break;
                default:
                    throw new Exception("Сant read this file format");
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
                double value = Convert.ToDouble(str[2].Replace('.', ',')); //usually, second column is Name, that why str[2]
                var time = DateTime.Parse(str[0]);
                double timeToDouble = DateTimeAxis.ToDouble(time);
                data.DataPoints.Add(new DataPoint(timeToDouble, value));
                i++;
            }
            data.N = i;
        }

        // боже храни тебя господьб найдено и честно позаимствованое
        // https://stackoverflow.com/questions/59469985/c-sharp-get-frequency-spectrum-lines-of-a-wav-file
        // https://github.com/swharden/Spectrogram //если нужны спектограммы
        private static (List<double> audio, int sampleRate) ReadMono(string path, double multiplier = 16_000)
        {
            using var afr = new AudioFileReader(path);
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
            if (data is not WavData wavData)
                throw new WavWrongDataException();
            wavData.DataPoints = new();
            (List<double> t, int sampleRate) = ReadMono(path);
            int i = 0;
            for (; i < t.Count; i++)
                wavData.DataPoints.Add(new DataPoint(i, t[i]));
            wavData.N = i;
            wavData.Rate = sampleRate;
        }

        private static void ReadJpgFile(Data data, string path)
        {
            if (data is not ImageData bmpData)
                throw new ImageWrongDataException();
            bmpData.Image = new BitmapImage(new Uri(path));
            int stride = bmpData.Image.PixelWidth * (bmpData.Image.Format.BitsPerPixel / 8);
            bmpData.Bytes = new double[bmpData.Image.PixelHeight * stride];
            byte[] tempBytes = new byte[bmpData.Image.PixelHeight * stride];
            bmpData.Image.CopyPixels(tempBytes, stride, 0);
            bmpData.Bytes = bmpData.Image.Format.BitsPerPixel == 32 ?
                BytesOperations.ToDouble(tempBytes) : BytesOperations.ToDoubleBgr8(tempBytes);
            bmpData.Width = bmpData.Image.PixelWidth;
            bmpData.Height = bmpData.Image.PixelHeight;
            data.N = bmpData.Image.PixelWidth * bmpData.Image.PixelHeight;
        }

        private static void ReadXcrFile(Data data, string path) /////////////
        {
            if (data is not ImageData bmpData)
                throw new ImageWrongDataException();

            using BinaryReader binaryReader = new(File.Open(path, FileMode.Open));
            binaryReader.BaseStream.Position = 608;
            string str = "";
            for(int d = 0;d<4;d++)
            str += (char)binaryReader.ReadByte();
            bmpData.Height = Convert.ToInt32(str); //в файле сначала высота потом ширина
            binaryReader.BaseStream.Position = 624;
            str = "";
            for (int d = 0; d < 4; d++)
                str += (char)binaryReader.ReadByte();
            bmpData.Width = Convert.ToInt32(str);

            bmpData.Bytes = new double[bmpData.Height * bmpData.Width * 4];
            byte[] tempBytes = new byte[bmpData.Height * bmpData.Width * 2];
            int i = 0;
            binaryReader.BaseStream.Position = 2048;
            while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length - 8192) //608  624 ///////////////////////
                tempBytes[i++] = binaryReader.ReadByte();

            for (int k = 0, j = 0; k < bmpData.Bytes.Length; k++, j += 2)
            {
                bmpData.Bytes[k++] = tempBytes[j] * 256 + tempBytes[j + 1];
                bmpData.Bytes[k++] = tempBytes[j] * 256 + tempBytes[j + 1];
                bmpData.Bytes[k++] = tempBytes[j] * 256 + tempBytes[j + 1];
            }

            bmpData.ConvertBytesIntoImage(bmpData.Width, bmpData.Height);
            data.N = bmpData.Image.PixelWidth * bmpData.Image.PixelHeight;
        }

        public static void Write(Data data, bool isDoubleAccuracy = false)
        {
            SaveFileDialog dialog = new();
            dialog.Filter = "Text files(*.txt)|*.txt|Bin file(*.dat)|*.dat|Bin file(*.bin)|*.bin|" +
                "Image file(*.jpg)|*.jpg|Image file(*.xcr)|*.xcr";
            if (dialog.ShowDialog() == false)
                return;
            string path = dialog.FileName;
            string fileExt = Path.GetExtension(path);
            switch (fileExt)
            {
                case ".dat":
                    WriteBinFile(data, path, isDoubleAccuracy);
                    break;
                //case ".bin":
                //    WriteBinFile(data, path, isDoubleAccuracy);
                //    break;
                case ".txt":
                    WriteTxtFile(data, path);
                    break;
                case ".wav":
                    //
                    break;
                case ".jpg":
                    WriteJpgFile(data, path);
                    break;
                case ".xcr":
                    WriteXcrFile(data, path);
                    break;
                default:
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
            if (data is not ImageData bmpData)
                throw new ImageWrongDataException();

            JpegBitmapEncoder encoder = new();
            encoder.Frames.Add(BitmapFrame.Create(bmpData.Image));
            using var fileStream = new FileStream(filePath, FileMode.Create);
            encoder.Save(fileStream);
        }

        private static void WriteXcrFile(Data data, string filePath)////////////////////////////////
        {
            if (data is not ImageData bmpData)
                throw new ImageWrongDataException();

            using BinaryWriter binaryWriter = new(File.Open(filePath, FileMode.OpenOrCreate));
            binaryWriter.BaseStream.Position = 608;
            string str = bmpData.Height.ToString();
            for (int i = 0; i < str.Length; i++)
                binaryWriter.Write(str[i]);

            binaryWriter.BaseStream.Position = 624;
            str = bmpData.Width.ToString();
            for (int i = 0; i < str.Length; i++)
                binaryWriter.Write(str[i]);

            binaryWriter.BaseStream.Position = 2048;
            for (int i = 0; i < bmpData.Bytes.Length; i += 4)
            {
                binaryWriter.Write((byte)0);// т.к. уже нормированные значения
                binaryWriter.Write((byte)bmpData.Bytes[i]);
            }
            binaryWriter.BaseStream.Position = 2048 + 8192 + bmpData.Bytes.Length / 4;
            binaryWriter.Write((byte)0);
            binaryWriter.Close();
        }

        //private static void WriteDateTxtFile(List<DataPoint> list, string path)
        //{
        //    using StreamWriter streamWriter = new(File.Open(path, FileMode.OpenOrCreate));
        //    for (int i = 0; i < list.Count; i++)
        //        streamWriter.WriteLine(list[i].Y);
        //}
    }
}


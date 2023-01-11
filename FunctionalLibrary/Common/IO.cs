using NAudio.Wave;
using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
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
        public static List<DataPoint> Read(double delta_t = 0.001, bool doubleAccuracy = false, string? file = null, int rate = -1, bool isDate = false)
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
                    return ReadBinFile(path, delta_t, doubleAccuracy);
                case ".wav":
                    return ReadWavFile(path, out rate);
                case ".txt":
                    {
                        //if(isDate)
                            return ReadDateTxtFile(path);
                        //else
                        //    return ReadTxtFile(path, delta_t);
                    }
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
        private static List<DataPoint> ReadBinFile(string path, double delta_t, bool doubleAccuracy)
        {
            List<DataPoint> points = new();
            using (BinaryReader binaryReader = new(File.Open(path, FileMode.Open)))
            {
                try
                {
                    int i = 0;
                    if (!doubleAccuracy)
                    {
                        while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length) //file too smal and cant use peekchar
                            points.Add(new DataPoint(i++ * delta_t, binaryReader.ReadSingle()));
                    }
                    else
                        while (binaryReader.BaseStream.Position != binaryReader.BaseStream.Length)
                            points.Add(new DataPoint(i++, binaryReader.ReadDouble()));
                }
                catch (EndOfStreamException)
                {

                }
            }
            return points;
        }

        private static List<DataPoint> ReadDateTxtFile(string path)
        {
            List<DataPoint> points = new();
            using (StreamReader streamReader = new(File.Open(path, FileMode.Open)))
            {
                try
                {
                    string? inString;
                    while (!string.IsNullOrEmpty(inString = streamReader.ReadLine()))
                    {
                        string[] str = inString.Split("\t");
                        double value = Convert.ToDouble(str[2].Replace('.', ','));
                        var time = DateTime.Parse(str[0]);
                        //var day = new DateTime(int.Parse(date[2]), int.Parse(date[1]), int.Parse(date[0])); ;
                        double timeToDouble = DateTimeAxis.ToDouble(time);
                        points.Add(new DataPoint(timeToDouble, value));
                    }
                }
                catch (EndOfStreamException)
                {

                }
            }
            return points;
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

        private static List<DataPoint> ReadWavFile(string path, out int sampleRate)
        {
            List<DataPoint> points = new();
            (List<double> t, sampleRate) = ReadMono(path);
            for (int i = 0; i < t.Count; i++)
                points.Add(new DataPoint(i, t[i]));
            return points;
        }

        public static void Write(List<DataPoint> list)
        {
            SaveFileDialog dialog = new();
            dialog.Filter = "Text files(*.txt)|*.txt|Bin file(*.dat)|*.dat"; /* | All files(*.*) | *.*| Bin file(*.bin) | *.bin*/
            if (dialog.ShowDialog() == false)
                return;
            string path = dialog.FileName;
            string fileExt = Path.GetExtension(path);
            switch (fileExt)
            {
                case ".dat":
                    WriteBinFile(list, path);
                    break;
                case ".txt":
                    WriteTxtFile(list, path);
                    break;
                case ".wav":

                    break;
            }
        }

        private static void WriteBinFile(List<DataPoint> list, string path)
        {
            using (BinaryWriter binaryWriter = new(File.Open(path, FileMode.OpenOrCreate)))
            {
                try
                {
                    for (int i = 0; i < list.Count; i++)
                        binaryWriter.Write(list[i].Y);
                }
                finally { }
            }
        }

        private static void WriteTxtFile(List<DataPoint> list, string path)
        {
            using (StreamWriter streamWriter = new(File.Open(path, FileMode.OpenOrCreate)))
            {
                try
                {
                    for (int i = 0; i < list.Count; i++)
                        streamWriter.Write(list[i].Y);
                }
                finally { }
            }
        }
    }
}


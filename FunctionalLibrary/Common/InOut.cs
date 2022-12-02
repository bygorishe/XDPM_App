using NAudio.Wave;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace XDPM_App.Common
{
    public static class InOut
    {
        /// <summary>
        /// Select file
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static List<DataPoint> Read(out int rate, double delta_t = 0.001, bool doubleAccuracy = false)
        {
            OpenFileDialog openFileDialog = new();
            if (openFileDialog.ShowDialog() == false)
                throw new Exception("File not found");
            string path = openFileDialog.FileName;
            string fileExtension = Path.GetExtension(path);
            switch (fileExtension)
            {
                case ".dat":
                        return ReadBinFile(path, out rate, delta_t, doubleAccuracy);
                case ".wav":
                    return ReadWavFile(path, out rate);
                case ".txt":

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
        private static List<DataPoint> ReadBinFile(string path, out int rate, double delta_t, bool doubleAccuracy)
        {
            List<DataPoint> points = new();
            using (BinaryReader binaryReader = new(File.Open(path, FileMode.Open)))
            {
                try
                {
                    int i = 0;
                    if (!doubleAccuracy)
                    {
                        while (true) //file too smal and cant use peekchar
                        {
                            float value = binaryReader.ReadSingle();
                            points.Add(new DataPoint(i++ * delta_t, value));
                        }
                    }
                    else
                        while (true)
                            points.Add(new DataPoint(i++, binaryReader.ReadDouble()));
                }
                catch (EndOfStreamException)
                {

                }
            }
            //fix
            rate = 1;

            return points;
        }

        private static List<DataPoint> ReadTxtFile(string path, out int rate, double delta_t)
        {
            List<DataPoint> points = new();
            using (StreamReader streamReader = new(File.Open(path, FileMode.Open)))
            {
                try
                {
                    int i = 0;
                    while (true)
                    {
                        double value = streamReader.Read();
                        points.Add(new DataPoint(i++ * delta_t, value));
                    }
                }
                catch (EndOfStreamException)
                {

                }
            }
            //fix
            rate = 1;

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
                    using (BinaryWriter binaryWriter = new(File.Open(path, FileMode.OpenOrCreate)))
                    {
                        try
                        {
                            for (int i = 0; i < list.Count; i++)
                                binaryWriter.Write(list[i].Y);
                        }
                        finally { }
                    }
                    break;
                case ".txt":
                    using (StreamWriter streamWriter = new(File.Open(path, FileMode.OpenOrCreate)))
                    {
                        try
                        {
                            for (int i = 0; i < list.Count; i++)
                                streamWriter.Write(list[i].Y);
                        }
                        finally { }
                    }
                    break;
                case ".wav":
                    
                    break;
            }
        }
    }
}


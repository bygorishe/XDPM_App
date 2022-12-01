using Microsoft.Win32;
using NAudio.Wave;
using OxyPlot;
using Spectrogram;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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
        private static string Read()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == false)
                throw new Exception("File not found");
            return openFileDialog.FileName;
        }

        /// <summary>
        /// Read datas from binary file
        /// </summary>
        /// <param name="delta_t">Sampling interval</param>
        /// <param name="doubleAccuracy">If need douuble instead float</param>
        /// <returns></returns>
        public static List<DataPoint> ReadBinFile(double delta_t = 0.001, bool doubleAccuracy = false)
        {
            string path = Read();
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
            return points;
        }

        // боже храни тебя господь https://github.com/swharden/Spectrogram
        static (List<double> audio, int sampleRate) ReadMono(string filePath, double multiplier = 16_000)
        {
            using var afr = new NAudio.Wave.AudioFileReader(filePath);
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

        public static List<DataPoint> ReadWavFile(string path, out int sampleRate)
        {
            List<DataPoint> points = new();

            (List<double> t, sampleRate) = ReadMono(path);

            for (int i = 0; i < t.Count; i++)
                points.Add(new DataPoint(i, t[i]));

            return points;
        }


        //public static void Write(List<DataPoint> list, string name)
        //{
        //    int BUFFERSIZE = 4096;
        //    var buffer = new byte[BUFFERSIZE];
        //    for (int i = 0; i < list.Count(); i++)
        //        buffer[i] = Convert.ToByte(list[i].Y);
        //    WaveFileWriter writer = new WaveFileWriter(name, new WaveFormat(8000, 1)); ;
        //    writer.Write(buffer, 0, buffer.Length);

        //}

        /// <summary>
        /// Write your datapoints to file
        /// </summary>
        /// <param name="list">Datapoints</param>
        /// <param name="fileName">Name of your file</param>
        /// <param name="fileFormat">File format that you need</param>
        public static void Write(List<DataPoint> list)
        {
            SaveFileDialog dialog = new();
            dialog.Filter = "Text files(*.txt)|*.txt|All files(*.*)|*.*|Bin file(*.bin)|*.bin|Bin file(*.dat)|*.dat";
            if (dialog.ShowDialog() == false)
                return;
            //StringBuilder builder = new(fileName);
            //builder.Append('.').Append(fileFormat);
            using (BinaryWriter binaryWriter = new(File.Open(dialog.FileName, FileMode.OpenOrCreate)))
            {
                try
                {
                    for (int i = 0; i < list.Count; i++)
                        binaryWriter.Write(list[i].Y);
                }
                finally { }
            }
        }
    }
}


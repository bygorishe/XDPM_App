using Microsoft.Win32;
using NAudio.Wave;
using OxyPlot;
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

        public static List<DataPoint> ReadWavFile(string path)
        {
            List<DataPoint> points = new();
            //List<short> unanalyzed_values = new List<short>();
            //AudioFileReader audioReader = new(path);

            ////8192, 4096, 2048, 1024
            //int BUFFERSIZE = 8192;
            //var buffer = new byte[BUFFERSIZE];
            //int bytes_read = audioReader.Read(buffer, 0, buffer.Length);
            //int BYTES_PER_POINT = audioReader.WaveFormat.BitsPerSample / 8; //8Bit = 1Byte
            //for (int n = 0; n < BYTES_PER_POINT; n++)
            //{
            //    short[] values = new short[buffer.Length / BYTES_PER_POINT];
            //    for (int i = 0; i < bytes_read; i += BYTES_PER_POINT)
            //        values[i / BYTES_PER_POINT] = (short)((buffer[i + 1] << 8) | buffer[i + 0]);
            //    unanalyzed_values.AddRange(values);
            //}

            //for (int i = 0; i < unanalyzed_values.Count; i++)
            //    points.Add(new DataPoint(i, unanalyzed_values[i]));

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


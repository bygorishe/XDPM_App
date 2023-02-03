using XDPM_App.Common;
using OxyPlot;
using System.Collections.Generic;
using static System.Math;
using System.Windows.Media.Imaging;

namespace XDPM_App.ADMP
{
    public static class Processing
    {
        /// <summary>
        /// Correct datapoints on number
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <param name="M">Correcting number</param>
        public static void AntiShift(ref List<DataPoint> dataPoints, double M)
        {
            DataPointOperations.SumPointsWithNumber(ref dataPoints, -M);
        }

        /// <summary>
        /// Delete values from datapoint, which more than R number
        /// </summary>
        /// <param name="R"></param>
        /// <param name="dataPoints"></param>
        //public static void AntiSpike(double R, ref List<DataPoint> dataPoints)
        //{
        //    for (int i = 0; i < dataPoints.Count; i++)
        //    {
        //        if (Abs(dataPoints[i].Y) > R && i != 0 && i != dataPoints.Count - 1)
        //        {
        //            double value = (dataPoints[i - 1].Y + dataPoints[i + 1].Y) / 2;
        //            dataPoints[i] = new DataPoint(dataPoints[i].X, value);
        //        }
        //        else if (i == 0)
        //            dataPoints[i] = new DataPoint(dataPoints[i].X, dataPoints[i + 1].Y / 2);
        //        else if (i == dataPoints.Count - 1)
        //            dataPoints[i] = new DataPoint(dataPoints[i].X, dataPoints[i - 1].Y / 2);
        //        else
        //            dataPoints[i] = new DataPoint(dataPoints[i].X, dataPoints[i].Y);
        //    }
        //}

        /// <summary>
        /// Delete linear trend from general trend
        /// </summary>
        /// <param name="dataPoints"></param>
        public static void AntiTrendLinear(ref List<DataPoint> dataPoints)
        {
            for (int i = 0; i < dataPoints.Count - 1; i++)
                dataPoints[i] = new DataPoint(dataPoints[i].X, dataPoints[i + 1].Y - dataPoints[i].Y);
        }

        public static void MovingAverage(int W, ref List<DataPoint> dataPoints)
        {
            int step =dataPoints.Count / W;
            for (int i = 0; i < W; i++)
            {
                double average = 0;
                for (int j = i * step; j < (i + 1) * step; j++)
                    average += dataPoints[j].Y;
                average /= step;
                for (int j = i * step; j < (i + 1) * step; j++)
                    dataPoints[j] = new DataPoint(dataPoints[j].X, average);
            }
        }

        /// <summary>
        /// Delete exp trend from general trend
        /// </summary>
        /// <param name="W">Number of intervals to detecting trend</param>
        /// <param name="dataPoints"></param>
        public static void AntiTrendNonLinear(int W, ref List<DataPoint> dataPoints)
        {
            List<DataPoint> tempDataPoints = DataPointOperations.Copy(dataPoints);
            MovingAverage(W, ref tempDataPoints);
            for (int i = 0; i < dataPoints.Count; i++)
                dataPoints[i] = new DataPoint(dataPoints[i].X, dataPoints[i].Y - tempDataPoints[i].Y);
        }

        public static List<DataPoint> ImplementationMNoise(int M, int N, List<DataPoint>? dataPoints = null, double R = 1)
        {
            List<DataPoint> newDataPoints = new(N);
            for (int j = 0; j < N; j++)
                newDataPoints.Add(new DataPoint(0, 0));
            if (dataPoints == null)
                for (int i = 0; i < M; i++)
                {
                    List<DataPoint> temp = Model.RandomNoiseTrend(N, R);
                    newDataPoints = DataPointOperations.SumPoints(temp, newDataPoints);
                }
            else
                for (int i = 0; i < M; i++)
                {
                    List<DataPoint> temp = Model.RandomNoiseTrend(N, R);
                    newDataPoints = DataPointOperations.SumPoints(temp, newDataPoints, dataPoints);
                }
            return newDataPoints;
        }

        public static List<DataPoint> AntiNoise(int M, int N, List<DataPoint> dataPoints)
        {
            for (int j = 0; j < N; j++)
                dataPoints[j] = new DataPoint(dataPoints[j].X, dataPoints[j].Y / M);
            return dataPoints;
        }

        public static void LowPassFilter(double fc, double delta_t, int m, ref List<double> lpw)
        {
            double[] d = { 0.35577019, 0.2436983, 0.07211497, 0.00630165 };
            double fact = fc * 2 * delta_t, arg = fact * PI;
            lpw.Add(fact);
            for (int i = 1; i <= m; i++)
                lpw.Add(Sin(arg * i) / (PI * i));
            lpw[m] /= 2;
            double sumG = lpw[0];
            for (int i = 1; i <= m; i++)
            {
                double sum = d[0];
                arg = PI * i / m;
                for (int k = 1; k <= 3; k++)
                    sum += 2 * d[k] * Cos(arg * k);
                lpw[i] *= sum;
                sumG += 2 * lpw[i];
            }
            for (int i = 0; i <= m; i++)
                lpw[i] /= sumG;

            lpw.Reverse();
            for (int i = m - 1; i >= 0; i--)
                lpw.Add(lpw[i]);
        }

        public static void HighPassFilter(double fc, double delta_t, int m, ref List<double> hpw)
        {
            List<double> lpw = new(2 * m + 1);
            LowPassFilter(fc, delta_t, m, ref lpw);
            for (int k = 0; k <= 2 * m; k++)
                hpw.Add(k == m ? 1 - lpw[k] : -lpw[k]);
        }

        public static void StrokeFilter(double fc1, double fc2, double delta_t, int m, ref List<double> bpw)
        {
            List<double> lpw1 = new(2 * m + 1);
            List<double> lpw2 = new(2 * m + 1);
            LowPassFilter(fc1, delta_t, m, ref lpw1);
            LowPassFilter(fc2, delta_t, m, ref lpw2);
            for (int k = 0; k <= 2 * m; k++)
                bpw.Add(lpw2[k] - lpw1[k]);
        }

        public static void RejectFilter(double fc1, double fc2, double delta_t, int m, ref List<double> bsw)
        {
            List<double> lpw1 = new(2 * m + 1);
            List<double> lpw2 = new(2 * m + 1);
            LowPassFilter(fc1, delta_t, m, ref lpw1);
            LowPassFilter(fc2, delta_t, m, ref lpw2);
            for (int k = 0; k <= 2 * m; k++)
                bsw.Add(k == m ? 1 + lpw1[k] - lpw2[k] : lpw1[k] - lpw2[k]);
        }
    }

    public static class ImageProccesing
    {
        public static void Shift(BmpData data, byte shiftValue)
        {
            for (int i = 0; i < data.bytes.Length; i++)
            {
                data.bytes[i++] += shiftValue;
                data.bytes[i++] += shiftValue;
                data.bytes[i++] += shiftValue;
            }
            data.ChangeBytesInImage();
        }

        public static void Mult(BmpData data, double multValue)
        {
            for (int i = 0; i < data.bytes.Length; i++)
            {
                data.bytes[i] = (byte)(data.bytes[i++] * multValue);
                data.bytes[i] = (byte)(data.bytes[i++] * multValue);
                data.bytes[i] = (byte)(data.bytes[i++] * multValue);
            }
            data.ChangeBytesInImage();
        }
    }
}
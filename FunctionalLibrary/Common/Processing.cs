using OxyPlot;
using System.Collections.Generic;
using static System.Math;
using System.Linq;
using FunctionalLibrary.Helpers.Operations;
using System.Windows.Media.Imaging;
using System;
using System.Windows.Media.Media3D;
using static XDPM_App.ADMP.ImageData;

namespace XDPM_App.ADMP
{
    public static class Processing
    {
        public static void Shift(ref List<DataPoint> dataPoints, double S)
        {
            DataPointOperations.SumPointsWithNumber(ref dataPoints, S);
        }
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
        public static void AntiSpike(double R, ref List<DataPoint> dataPoints)
        {
            for (int i = 0; i < dataPoints.Count; i++)
            {
                if (Abs(dataPoints[i].Y) > R && i != 0 && i != dataPoints.Count - 1)
                {
                    double value = (dataPoints[i - 1].Y + dataPoints[i + 1].Y) / 2;
                    dataPoints[i] = new DataPoint(dataPoints[i].X, value);
                }
                else if (i == 0)
                    dataPoints[i] = new DataPoint(dataPoints[i].X, dataPoints[i + 1].Y / 2);
                else if (i == dataPoints.Count - 1)
                    dataPoints[i] = new DataPoint(dataPoints[i].X, dataPoints[i - 1].Y / 2);
                else
                    dataPoints[i] = new DataPoint(dataPoints[i].X, dataPoints[i].Y);
            }
        }

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
            int step = dataPoints.Count / W;
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
        public static void Shift(ImageData data, double shiftValue)
        {
            for (int i = 0; i < data.Bytes.Length; i++)
            {
                data.Bytes[i++] += shiftValue;
                data.Bytes[i++] += shiftValue;
                data.Bytes[i++] += shiftValue;
            }
            data.ConvertBytesIntoImage();
        }

        public static void Mult(ImageData data, double multValue)
        {
            for (int i = 0; i < data.Bytes.Length; i++)
            {
                data.Bytes[i] = data.Bytes[i++] * multValue;
                data.Bytes[i] = data.Bytes[i++] * multValue;
                data.Bytes[i] = data.Bytes[i++] * multValue;
            }
            data.ConvertBytesIntoImage();
        }

        public static void ByteScale(ImageData data)
        {
            List<double> bytes = new(data.Bytes.Length * 3 / 4);
            for (int i = 0; i < data.Bytes.Length; i++)
            {
                bytes.Add(data.Bytes[i++]);
                bytes.Add(data.Bytes[i++]);
                bytes.Add(data.Bytes[i++]);
            }
            double min = bytes.Min(), div = bytes.Max() - min;
            for (int i = 0; i < data.Bytes.Length; i++)
                data.Bytes[i] = (data.Bytes[i] - min) * byte.MaxValue / div;
            data.ConvertBytesIntoImage();
        }

        public static void Negative(ImageData data)
        {
            BytesOperations.ToBytes(data.Bytes);
            for (int i = 0; i < data.Bytes.Length; i++)
            {
                data.Bytes[i] = byte.MaxValue - data.Bytes[i++];
                data.Bytes[i] = byte.MaxValue - data.Bytes[i++];
                data.Bytes[i] = byte.MaxValue - data.Bytes[i++];
            }
            data.ConvertBytesIntoImage();
        }

        public static void Rotate(ImageData data)
        {
            int height = data.Height, width = data.Width;

            if (data.BytesMatrix == null)
                data.MakeByteMatrix();
            data.Height = width;
            data.Width = height;

            Bgr32Byte[,] newMatrix = new Bgr32Byte[width, height];
            for (int j = 0; j < width; j++)
                for (int i = 0; i < height; i++)
                {
                    newMatrix[j, i] = new();
                    newMatrix[j, i] = data.BytesMatrix![i, width - 1 - j];
                }

            data.BytesMatrix = newMatrix;
            data.MakeArrayFromMatrix();
            data.ConvertBytesIntoImage(height, width);
        }

        public static void Symmetry(ImageData data)
        {
            int height = data.Height, width = data.Width;

            if (data.BytesMatrix == null)
                data.MakeByteMatrix();

            Bgr32Byte[,] newMatrix = data.BytesMatrix!;
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width / 2; i++)
                    (newMatrix[j, width - 1 - i], newMatrix[j, i]) = (newMatrix[j, i], newMatrix[j, width - 1 - i]);

            data.MakeArrayFromMatrix();
            data.ConvertBytesIntoImage();
        }

        public static void GammaTransform(ImageData data, float C, double y)
        {
            BytesOperations.ToBytes(data.Bytes);
            for (int i = 0; i < data.Bytes.Length; i++)
            {
                data.Bytes[i] = C * Pow(data.Bytes[i++], y);
                data.Bytes[i] = C * Pow(data.Bytes[i++], y);
                data.Bytes[i] = C * Pow(data.Bytes[i++], y);
            }
            BytesOperations.ToBytes(data.Bytes);
            data.ConvertBytesIntoImage();
        }

        public static void LogTransform(ImageData data, float C)
        {
            BytesOperations.ToBytes(data.Bytes);
            for (int i = 0; i < data.Bytes.Length; i++)
            {
                data.Bytes[i] = C * Log(data.Bytes[i++] + 1);
                data.Bytes[i] = C * Log(data.Bytes[i++] + 1);
                data.Bytes[i] = C * Log(data.Bytes[i++] + 1);
            }
            BytesOperations.ToBytes(data.Bytes);
            data.ConvertBytesIntoImage();
        }

        public static void NearestNeighborInterpolation(ImageData data, double resizeCoefficient) //без матрицы попробовать
        {
            int newHeight = (int)(data.Height * resizeCoefficient);
            int newWidth = (int)(data.Width * resizeCoefficient);

            if (data.BytesMatrix == null)
                data.MakeByteMatrix();
            Bgr32Byte[,] newByteMatrix = new Bgr32Byte[newHeight, newWidth];

            for (int j = 0; j < newHeight; j++)
                for (int i = 0; i < newWidth; i++)
                    //newByteMatrix[j, i] = data.BytesMatrix![(int)(j * (1 / resizeCoefficient)), (int)(i * (1 / resizeCoefficient))];
                    newByteMatrix[j, i] = data.BytesMatrix![(int)((j + 0.5) * (1 / resizeCoefficient) - 0.5),
                        (int)((i + 0.5) * (1 / resizeCoefficient) - 0.5)];

            data.BytesMatrix = newByteMatrix;
            data.Height = newHeight;
            data.Width = newWidth;
            data.MakeArrayFromMatrix();
        }

        public static double bilinear(double p00, double p10, double p01, double p11, double x, double y)
        {
            return p00 * (1 - x) * (1 - y) + p10 * x * (1 - y) + p01 * (1 - x) * y + p11 * x * y;
        }

//int x = (i + 0.5) * m / a - 0.5
//int y = (j + 0.5) * n / b - 0.5

//instead of

//int x = i * m / a
//int y = j * n / b

//        for (int i = 0; i<newW; i++)
//                    {
//                        int x0 = Convert.ToInt32(Math.Floor(i / scaleX));
//                        if (x0 == oldW - 1)
//                            x0--;
//                        int x1 = Convert.ToInt32(x0 + 1);

//        double x = (i / scaleX) % 1;
//                        for (int j = 0; j<newH; j++)
//                        {
//                            int y0 = Convert.ToInt32(Math.Floor(j / scaleY));
//                            if (y0 == oldH - 1)
//                                y0--;
//                            int y1 = Convert.ToInt32(y0 + 1);
//        double y = (j / scaleY) % 1;

//        newR[j + i * Convert.ToInt32(newH)] = bilinear(
//            oldR[y0 + x0 * Convert.ToInt32(oldH)],
//            oldR[y0 + x1 * Convert.ToInt32(oldH)],
//            oldR[y1 + x0 * Convert.ToInt32(oldH)],
//            oldR[y1 + x1 * Convert.ToInt32(oldH)],
//            x, y);
//        newG[j + i * Convert.ToInt32(newH)] = bilinear(
//            oldG[y0 + x0 * Convert.ToInt32(oldH)],
//            oldG[y0 + x1 * Convert.ToInt32(oldH)],
//            oldG[y1 + x0 * Convert.ToInt32(oldH)],
//            oldG[y1 + x1 * Convert.ToInt32(oldH)],
//            x, y);
//        newB[j + i * Convert.ToInt32(newH)] = bilinear(
//            oldB[y0 + x0 * Convert.ToInt32(oldH)],
//            oldB[y0 + x1 * Convert.ToInt32(oldH)],
//            oldB[y1 + x0 * Convert.ToInt32(oldH)],
//            oldB[y1 + x1 * Convert.ToInt32(oldH)],
//            x, y);
//    }
//}
    }
}
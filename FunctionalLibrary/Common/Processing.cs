using OxyPlot;
using System.Collections.Generic;
using static System.Math;
using System.Linq;
using FunctionalLibrary.Helpers.Operations;
using static XDPM_App.ADMP.ImageData;
using OxyPlot.Series;
using System;
using FunctionalLibrary.Helpers.Structs;
using NAudio.Utils;

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

        public static void Cw(ImageData data)
        {
            int height = data.Height, width = data.Width;

            if (data.BytesMatrix == null)
                data.MakeByteMatrix();
            data.Height = width;
            data.Width = height;

            RgbPixel[,] newMatrix = new RgbPixel[width, height];
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

        public static void Ccw(ImageData data)
        {
            int height = data.Height, width = data.Width;

            if (data.BytesMatrix == null)
                data.MakeByteMatrix();
            data.Height = width;
            data.Width = height;

            RgbPixel[,] newMatrix = new RgbPixel[width, height];
            for (int j = 0; j < width; j++)
                for (int i = 0; i < height;)
                {
                    newMatrix[j, i] = new();
                    newMatrix[j, i] = data.BytesMatrix![height - ++i, j];
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

            RgbPixel[,] newMatrix = data.BytesMatrix!;
            for (int j = 0; j < height; j++)
                for (int i = 0; i < width / 2; i++)
                    (newMatrix[j, width - 1 - i], newMatrix[j, i])
                    = (newMatrix[j, i], newMatrix[j, width - 1 - i]);

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

        public static void NearestNeighborInterpolation(ImageData data, double resizeCoefficient) //без матрицы mb попробовать
        {
            int newHeight = (int)(data.Height * resizeCoefficient);
            int newWidth = (int)(data.Width * resizeCoefficient);

            if (data.BytesMatrix == null)
                data.MakeByteMatrix();
            RgbPixel[,] newByteMatrix = new RgbPixel[newHeight, newWidth];

            for (int j = 0; j < newHeight; j++)
                for (int i = 0; i < newWidth; i++)
                    newByteMatrix[j, i] = data.BytesMatrix![(int)(j / resizeCoefficient),
                                                            (int)(i / resizeCoefficient)];

            data.BytesMatrix = newByteMatrix;
            data.Height = newHeight;
            data.Width = newWidth;
            data.MakeArrayFromMatrix();
        }
        //int x = (i + 0.5) * m / a - 0.5
        //int y = (j + 0.5) * n / b - 0.5
        //instead of
        //int x = i * m / a
        //int y = j * n / b

        private static double Bilinear(double p00, double p10, double p01, double p11, double x, double y)
            => p00 * (1 - x) * (1 - y) + p10 * x * (1 - y) + p01 * (1 - x) * y + p11 * x * y;

        public static void BilinearInterpolation(ImageData data, double resizeCoefficient) //без матрицы попробовать
        {
            int newHeight = (int)(data.Height * resizeCoefficient);
            int newWidth = (int)(data.Width * resizeCoefficient);

            if (data.BytesMatrix == null)
                data.MakeByteMatrix();
            RgbPixel[,] newByteMatrix = new RgbPixel[newHeight, newWidth];

            for (int j = 0; j < newHeight; j++)
            {
                int x0 = (int)(j / resizeCoefficient);
                if (x0 == data.Height - 1) x0--;
                int x1 = x0 + 1;

                double x = (j / resizeCoefficient) % 1;
                for (int i = 0; i < newWidth; i++)
                {
                    int y0 = (int)(i / resizeCoefficient);
                    if (y0 == data.Width - 1)
                        y0--;
                    int y1 = y0 + 1;
                    double y = (i / resizeCoefficient) % 1;
                    RgbPixel refByte = newByteMatrix[j, i] = new();

                    refByte.Values[0] = Bilinear(data.BytesMatrix![x0, y0].Values[0], data.BytesMatrix[x1, y0].Values[0],
                        data.BytesMatrix[x0, y1].Values[0], data.BytesMatrix[x1, y1].Values[0], x, y);
                    refByte.Values[1] = Bilinear(data.BytesMatrix[x0, y0].Values[1], data.BytesMatrix[x1, y0].Values[1],
                        data.BytesMatrix[x0, y1].Values[1], data.BytesMatrix[x1, y1].Values[1], x, y);
                    refByte.Values[2] = Bilinear(data.BytesMatrix[x0, y0].Values[2], data.BytesMatrix[x1, y0].Values[2],
                        data.BytesMatrix[x0, y1].Values[2], data.BytesMatrix[x1, y1].Values[2], x, y);
                }

            }

            data.BytesMatrix = newByteMatrix;
            data.Height = newHeight;
            data.Width = newWidth;
            data.MakeArrayFromMatrix();
        }

        public static double[] NormHist(double[] bytes)
        {
            int size = bytes.Length;
            double[] count = new double[byte.MaxValue + 1];
            for (int i = 0; i < size; i += 4)
                count[Convert.ToInt32(bytes[i])]++;
            size /= 4;
            for (int i = 0; i <= byte.MaxValue; i++)
                count[i] /= size;
            return count;
        }

        public static void MakeDistribution(ref double[] bytes)
        {
            for (int i = 1; i < bytes.Length; i++)
                bytes[i] += bytes[i - 1];
        }

        public static void CDF(ImageData data, double[] cdf)
        {
            for (int i = 0; i < data.Bytes.Length; i++)
            {
                data.Bytes[i] = byte.MaxValue * cdf[Convert.ToInt32(data.Bytes[i++])];
                data.Bytes[i] = byte.MaxValue * cdf[Convert.ToInt32(data.Bytes[i++])];
                data.Bytes[i] = byte.MaxValue * cdf[Convert.ToInt32(data.Bytes[i++])];
            }
            data.ConvertBytesIntoImage();
        }

        public static double[] Difference(double[] b1, double[] b2)
        {
            if (b1.Length != b2.Length)
                throw new Exception("Lenght");
            double[] newArray = new double[b1.Length];
            for (int i = 0; i < b1.Length; i++)
                newArray[i] = b1[i] - b2[i];
            return newArray;
        }

        public static void AverageFilter(ImageData data, int windowSize)
        {
            if (data.BytesMatrix == null)
                data.MakeByteMatrix();

            RgbPixel[,] pixels = data.BytesMatrix!;

            int halfWinSize = windowSize / 2;
            for (int i = halfWinSize; i < data.Height - halfWinSize; i++)
            {
                for (int j = halfWinSize; j < data.Width - halfWinSize; j++)
                {
                    RgbPixel sumPixel = new();
                    for (int m = i - halfWinSize; m <= i + halfWinSize; m++)
                        for (int n = j - halfWinSize; n <= j + halfWinSize; n++)
                            sumPixel += pixels[m, n];
                    sumPixel /= windowSize * windowSize;

                    data.BytesMatrix![i, j] = sumPixel;
                }
            }
            data.MakeArrayFromMatrix();
            data.ConvertBytesIntoImage();
        }

        public static void MedianFilter(ImageData data, int windowSize)
        {
            if (data.BytesMatrix == null)
                data.MakeByteMatrix();

            RgbPixel[,] pixels = data.BytesMatrix!;

            int halfWinSize = windowSize / 2;
            for (int i = halfWinSize; i < data.Height - halfWinSize; i++)
            {
                for (int j = halfWinSize; j < data.Width - halfWinSize; j++)
                {
                    /*
                    List<RgbPixel> matrix = new List<RgbPixel>(windowSize * windowSize);
                    for (int m = i - halfWinSize; m <= i + halfWinSize; m++)
                        for (int n = j - halfWinSize; n <= j + halfWinSize; n++)
                            matrix.Add(pixels[m, n]);
                    matrix.Sort();
                     */
                    List<double> matrix = new List<double>(windowSize * windowSize);
                    for (int m = i - halfWinSize; m <= i + halfWinSize; m++)
                        for (int n = j - halfWinSize; n <= j + halfWinSize; n++)
                            matrix.Add(pixels[m, n].Values[0]);
                    matrix.Sort();

                    data.BytesMatrix![i, j].Values[0] = matrix[windowSize * windowSize / 2];
                    data.BytesMatrix![i, j].Values[1] = matrix[windowSize * windowSize / 2];
                    data.BytesMatrix![i, j].Values[2] = matrix[windowSize * windowSize / 2];
                }
            }
            data.MakeArrayFromMatrix();
            data.ConvertBytesIntoImage();
        }

        public static List<List<DataPoint>> XrayDetector(ImageData data, double dx = 1)
        {
            if (data.BytesMatrix == null)
                data.MakeByteMatrix();

            double[,] deriviateStrokes = new double[data.Height / 10 + 1, data.Width];
            int k = 0;
            for (int i = 0; i < data.Height; i += 10)
            {
                deriviateStrokes[k, 0] = data.BytesMatrix![i, 0].Values[0];
                for (int j = 1; j < data.Width; j++)
                    deriviateStrokes[k, j ] = (data.BytesMatrix![i, j - 1].Values[0] - data.BytesMatrix[i, j].Values[0]) / dx;
                k++;
            }

            //////
           List< List<DataPoint>> datas = new List<List<DataPoint>>();
           List< List<DataPoint>> datas2 = new List<List<DataPoint>>();
           List< List<DataPoint>> datas3 = new List<List<DataPoint>>();
            for(int i = 0; i < data.Height / 10 + 1; i++)
            {
                datas.Add(new List<DataPoint>());
                datas2.Add(new List<DataPoint>());
                datas3.Add(new List<DataPoint>());

                for (int j = 0; j < data.Width; j++)
                    datas[i].Add(new DataPoint(j, deriviateStrokes[i, j ]));
                Analysis analysis = new Analysis(datas[i], datas[i].Count, 10);
                if (i == 0)
                    datas2[i] = analysis.CCF(datas[0].Select(x => x.Y).ToList());
                else
                    datas2[i] = analysis.CCF(datas[i-1].Select(x => x.Y).ToList());

                Analysis analysis1 = new(datas[i], datas[i].Count, 10);
                datas3[i] = analysis1.SpectrFourier(datas[i].Count, 0, 1);

            }
            //////
            double dada = 0;
            foreach(var c in datas3)
            {
                dada += c.Aggregate((x,y) => x.Y > y.Y ? x : y).X; ///////////////////
            }
            dada /= datas3.Count;

            List<List<DataPoint>> datassss = new List<List<DataPoint>>();
            List<List<DataPoint>> datassss123 = new List<List<DataPoint>>();
            int m = 16;

            for (int i= 0; i< data.Height; i++)
            {
                datassss.Add(new List<DataPoint>());
                datassss123.Add(new List<DataPoint>());
                //for (int j = 0; j < 2 * m + 1; j++)
                //    datassss[i].Add(new DataPoint(j, data.BytesMatrix[i, data.Width - (2*m + 1)  + j].Values[0]));
                for (int j = 0; j < data.Width; j++)
                        datassss[i].Add(new DataPoint(j, data.BytesMatrix[i, j].Values[0])); 
                for (int j = 0; j < data.Width; j++)
                        datassss[i].Add(new DataPoint(j, data.BytesMatrix[i, j].Values[0]));





                List<DataPoint> t3;
                List<double> bsw = new(2 * m + 1);
                Processing.RejectFilter(dada - dada/10, dada+ dada/5, 1, m, ref bsw);
                t3 = new List<DataPoint>(2 * m + 1);
                for (int kk = 0; kk <= 2 * m; kk++)
                    t3.Add(new DataPoint(kk, bsw[kk]));

                Analysis a44 = new(t3, 2 * m + 1);

                datassss123[i] = (DataPointOperations.Convol(datassss[i].Count , 2 * m + 1, t3, datassss[i]));
            }

            //data.Width = datassss[0].Count;
            //data.BytesMatrix = new RgbPixel[data.Height, datassss[0].Count];
            for(int i = 0; i< data.Height; i++)
            {
                for(int j  =0; j < data.Width; j++)
                {
                    int f = m;
                    //if (j < 2 * m + 1)
                    //    f = 3 * m + 1;
                    //else
                    //    f = -2 * m - 1;
                    //data.BytesMatrix[i, j].Values[0] = datassss123[i][f + j].Y;
                    //data.BytesMatrix[i, j].Values[1] = datassss123[i][f + j].Y;
                    //data.BytesMatrix[i, j].Values[2] = datassss123[i][f + j].Y;
                    data.BytesMatrix[i, j] = new (datassss123[i][f + j].Y);
                }
            }
            data.MakeArrayFromMatrix();
            data.ConvertBytesIntoImage();
            return datas3;

        }

        public static void XrayFix()
        {

        }
    }
}
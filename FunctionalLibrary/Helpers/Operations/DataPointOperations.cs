using OxyPlot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FunctionalLibrary.Helpers.Operations
{
    public class DataPointOperations
    {
        public static List<DataPoint> MakeDataPointsList(double[] values, double deltaT = 0.001)
        {
            int size = values.Length;
            List<DataPoint> dataPoints = new List<DataPoint>(size);
            for (int i = 0; i < size; i++)
                dataPoints.Add(new DataPoint(i * deltaT, values[i]));
            return dataPoints;
        }

        public static DataPoint[] MakeDataPoints(double[] values, double deltaT = 0.001)
        {
            int size = values.Length;
            DataPoint[] dataPoints = new DataPoint[size];
            for (int i = 0; i < size; i++)
                dataPoints[i] = new DataPoint(i * deltaT, values[i]);
            return dataPoints;
        }

        /// <summary>
        /// Summarize number to all datapoints
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <param name="number"></param>
        public static void SumDataPointsWithNumber(ref DataPoint[] dataPoints, double number)
        {
            int N = dataPoints.Length;
            for (int i = 0; i < N; i++)
                dataPoints[i] = new DataPoint(dataPoints[i].X, dataPoints[i].Y + number);
        }

        public static void SumPointsWithNumber(ref double[] dataPoints, double number)
        {
            int N = dataPoints.Length;
            for (int i = 0; i < N; i++)
                dataPoints[i] += number;
        }

        /// <summary>
        /// Multiply datapoints with amber
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <param name="number"></param>
        public static void MultDataPointsWithNumber(ref DataPoint[] dataPoints, double number)
        {
            int N = dataPoints.Length;
            for (int i = 0; i < N; i++)
                dataPoints[i] = new DataPoint(dataPoints[i].X, dataPoints[i].Y * number);
        }

        public static void MultPointsWithNumber(ref double[] dataPoints, double number)
        {
            int N = dataPoints.Length;
            for (int i = 0; i < N; i++)
                dataPoints[i] *= number;
        }

        /// <summary>
        /// Summarize datapoints
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <returns></returns>
        public static DataPoint[] SumPoints(params DataPoint[][] dataPoints)
        {
            int N = dataPoints[0].Length;
            double[] y = new double[N];
            DataPoint[] newDataPoints = new DataPoint[N];
            foreach (var list in dataPoints)
                for (int i = 0; i < N; i++)
                    y[i] += list[i].Y;
            for (int i = 0; i < N; i++)
                newDataPoints[i] = new DataPoint(dataPoints[0][i].X, y[i]);
            return newDataPoints;
        }

        /// <summary>
        /// Multiply all datapoints
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <returns></returns>
        public static DataPoint[] MultPoints(params DataPoint[][] dataPoints)
        {
            int N = dataPoints[0].Length;
            double[] y = new double[N];
            DataPoint[] newDataPoints = new DataPoint[N];
            for (int i = 0; i < N; i++)
                y[i] = 1;
            foreach (var list in dataPoints)
                for (int i = 0; i < N; i++)
                    y[i] *= list[i].Y;
            for (int i = 0; i < N; i++)
                newDataPoints[i] = new DataPoint(dataPoints[0][i].X, y[i]);
            return newDataPoints;
        }

        /// <summary>
        /// Get list with datapoints value(Y)
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <returns></returns>;  
        public static double[] GetValue(DataPoint[] dataPoints)
            => dataPoints.Select(x => x.Y).ToArray();

        public static double[] GetValue(List<DataPoint> dataPoints)
            => dataPoints.Select(x => x.Y).ToArray();

        /// <summary>
        /// Normalized datapoints
        /// </summary>
        /// <param name="dataPoints"></param>
        public static void Normalized(ref DataPoint[] dataPoints)
        {
            int N = dataPoints.Length;
            double max = dataPoints.Select(x => x.Y).Max();
            MultDataPointsWithNumber(ref dataPoints, 1 / max);
        }

        /// <summary>
        /// Make convolution between two datapoints
        /// </summary>
        /// <param name="N">Datapoints lenght</param>
        /// <param name="M"></param>
        /// <param name="dataPoints1">First datapoint's list</param>
        /// <param name="dataPoints2">Second datapoint's list</param>
        /// <param name="delta_t">Sampling interval</param>
        /// <returns></returns>
        public static DataPoint[] DataPointsConvol(int N, int M, DataPoint[] dataPoints1,
                                                                 DataPoint[] dataPoints2) //свертка
        {
            DataPoint[] newDataPoints = new DataPoint[N];
            for (int i = 0; i < N; i++)
            {
                double sum = 0;
                for (int m = 0, t = i - m; m < M && t >= 0; m++, t--)
                    sum += dataPoints1[m].Y * dataPoints2[i - m].Y;
                newDataPoints[i] = new DataPoint(dataPoints2[i].X, sum);
            }
            return newDataPoints;
        }

        public static double[] Convol(int N, int M, double[] dataPoints1,
                                                    double[] dataPoints2) //свертка
        {
            double[] newDataPoints = new double[N];
            for (int i = 0; i < N; i++)
            {
                double sum = 0;
                for (int m = 0, t = i - m; m < M && t >= 0; m++, t--)
                    sum += dataPoints1[m] * dataPoints2[i - m];
                newDataPoints[i] = sum;
            }
            return newDataPoints;
        }

        public static DataPoint[] TakeRangeOf(DataPoint[] dataPoints, int index,
                                                                      int count)
        {
            DataPoint[] newDataPoints = new DataPoint[count];
            Array.Copy(dataPoints, index, newDataPoints, 0, count);
            return newDataPoints;
        }

        public static List<DataPoint> TakeRangeOfAndNullOther(List<DataPoint> dataPoints,
            int index, int count)
        {
            int size = dataPoints.Count;
            DataPoint[] newDataPoints = new DataPoint[size];
            dataPoints.CopyTo(index, newDataPoints, index, count);
            return newDataPoints.ToList();
        }

        public static List<DataPoint> Copy(List<DataPoint> dataPoints)
        {
            int size = dataPoints.Count;
            DataPoint[] newDataPoints = new DataPoint[size];
            dataPoints.CopyTo(0, newDataPoints, 0, size);
            return newDataPoints.ToList();
        }

        public static List<DataPoint> Join(params List<DataPoint>[] dataPoints)
        {
            List<DataPoint> newDataPoints = new();
            foreach (var points in dataPoints)
                newDataPoints.AddRange(points);
            return newDataPoints;
        }
    }
}
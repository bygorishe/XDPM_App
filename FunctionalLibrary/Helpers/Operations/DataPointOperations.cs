using OxyPlot;
using System.Collections.Generic;
using System.Linq;

namespace FunctionalLibrary.Helpers.Operations
{
    public class DataPointOperations
    {
        /// <summary>
        /// Summarize number to all datapoints
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <param name="number"></param>
        public static void SumPointsWithNumber(ref List<DataPoint> dataPoints, double number)
        {
            int N = dataPoints.Count;
            for (int i = 0; i < N; i++)
                dataPoints[i] = new DataPoint(dataPoints[i].X, dataPoints[i].Y + number);
        }

        /// <summary>
        /// Multiply datapoints with amber
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <param name="number"></param>
        public static void MultPointsWithNumber(ref List<DataPoint> dataPoints, double number)
        {
            int N = dataPoints.Count;
            for (int i = 0; i < N; i++)
                dataPoints[i] = new DataPoint(dataPoints[i].X, dataPoints[i].Y * number);
        }

        /// <summary>
        /// Summarize datapoints
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <returns></returns>
        public static List<DataPoint> SumPoints(params List<DataPoint>[] dataPoints)
        {
            int N = dataPoints[0].Count;
            List<double> y = new(N);
            List<DataPoint> newDataPoints = new(N);
            for (int i = 0; i < N; i++)
                y.Add(0);
            foreach (var list in dataPoints)
                for (int i = 0; i < N; i++)
                    y[i] += list[i].Y;
            for (int i = 0; i < N; i++)
                newDataPoints.Add(new DataPoint(dataPoints[0][i].X, y[i]));
            return newDataPoints;
        }

        public static List<DataPoint> SumPoints(int N, params List<DataPoint>[] dataPoints)
        {
            List<double> y = new(N);
            List<DataPoint> newDataPoints = new(N);
            for (int i = 0; i < N; i++)
                y.Add(0);
            foreach (var list in dataPoints)
                for (int i = 0; i < N; i++)
                    y[i] += list[i].Y;
            for (int i = 0; i < N; i++)
                newDataPoints.Add(new DataPoint(dataPoints[0][i].X, y[i]));
            return newDataPoints;
        }

        /// <summary>
        /// Multiply all datapoints
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <returns></returns>
        public static List<DataPoint> MultPoints(params List<DataPoint>[] dataPoints)
        {
            int N = dataPoints[0].Count;
            List<double> y = new(N);
            List<DataPoint> newDataPoints = new(N);
            for (int i = 0; i < N; i++)
                y.Add(1);
            foreach (var list in dataPoints)
                for (int i = 0; i < N; i++)
                    y[i] *= list[i].Y;
            for (int i = 0; i < N; i++)
                newDataPoints.Add(new DataPoint(dataPoints[0][i].X, y[i]));
            return newDataPoints;
        }


        /// <summary>
        /// Get list with datapoints value(Y)
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <returns></returns>
        public static List<double> GetValue(List<DataPoint> dataPoints)
            => dataPoints.Select(x => x.Y).ToList();

        /// <summary>
        /// Normalized datapoints
        /// </summary>
        /// <param name="dataPoints"></param>
        public static void Normalized(ref List<DataPoint> dataPoints)
        {
            int N = dataPoints.Count;
            double max = dataPoints.Select(x => x.Y).Max();
            MultPointsWithNumber(ref dataPoints, 1 / max);
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
        public static List<DataPoint> Convol(int N, int M, List<DataPoint> dataPoints1,
            List<DataPoint> dataPoints2) //свертка
        {
            List<DataPoint> newDataPoints = new(N);
            for (int i = 0; i < N; i++)
            {
                double sum = 0;
                for (int m = 0, t = i - m; m < M && t >= 0; m++, t--)
                    sum += dataPoints1[m].Y * dataPoints2[i - m].Y;
                newDataPoints.Add(new DataPoint(dataPoints2[i].X, sum));
            }
            return newDataPoints;
        }

        public static List<DataPoint> TakeRangeOf(List<DataPoint> dataPoints, int index,
            int count)
        {
            DataPoint[] newDataPoints = new DataPoint[count];
            dataPoints.CopyTo(index, newDataPoints, 0, count);
            return newDataPoints.ToList();
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
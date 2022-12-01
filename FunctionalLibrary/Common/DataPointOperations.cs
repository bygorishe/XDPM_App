using OxyPlot;
using System.Collections.Generic;
using System.Linq;

namespace XDPM_App.Common
{
    public class DataPointOperations
    {
        /// <summary>
        /// Summarize number to all datapoints
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <param name="t"></param>
        public static void SumPointsWithVar(ref List<DataPoint> dataPoints, double t)
        {
            int N = dataPoints.Count;
            for (int i = 0; i < N; i++)
                dataPoints[i] = new DataPoint(dataPoints[i].X, dataPoints[i].Y + t);
        }

        /// <summary>
        /// Multiply datapoints with amber
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <param name="t"></param>
        public static void MultPointsWithVar(ref List<DataPoint> dataPoints, double t)
        {
            int N = dataPoints.Count;
            for (int i = 0; i < N; i++)
                dataPoints[i] = new DataPoint(dataPoints[i].X, dataPoints[i].Y * t);
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
            List<DataPoint> dataPointsT = new(N);
            for (int i = 0; i < N; i++)
                y.Add(0);
            foreach (var c in dataPoints)
                for (int i = 0; i < N; i++)
                    y[i] += c[i].Y;
            for (int i = 0; i < N; i++)
                dataPointsT.Add(new DataPoint(dataPoints[0][i].X, y[i]));
            return dataPointsT;
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
            List<DataPoint> dataPointsT = new(N);
            for (int i = 0; i < N; i++)
                y.Add(1);
            foreach (var c in dataPoints)
                for (int i = 0; i < N; i++)
                    y[i] *= c[i].Y;
            for (int i = 0; i < N; i++)
                dataPointsT.Add(new DataPoint(dataPoints[0][i].X, y[i]));
            return dataPointsT;
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
            MultPointsWithVar(ref dataPoints, 1 / max);
        }

        /// <summary>
        /// Make convolution between two datapoints
        /// </summary>
        /// <param name="N">Datapoints lenght</param>
        /// <param name="M"></param>
        /// <param name="l1">First datapoint's list</param>
        /// <param name="l2">Second datapoint's list</param>
        /// <param name="delta_t">Sampling interval</param>
        /// <returns></returns>
        public static List<DataPoint> Convol(int N, int M, List<DataPoint> l1, List<DataPoint> l2, double delta_t = 0.001) //свертка
        {
            List<DataPoint> tl = new(N);
            for (int i = 0; i < N; i++)
            {
                double temp = 0;
                for (int m = 0, t = i - m; m < M && t >= 0; m++, t--)
                    temp += l1[m].Y * l2[i - m].Y;
                tl.Add(new DataPoint(i * delta_t, temp));
            }
            return tl;
        }
    }
}
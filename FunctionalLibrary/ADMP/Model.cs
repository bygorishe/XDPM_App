using XDPM_App.Common;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using static System.Math;

namespace XDPM_App.ADMP
{
    public static class Model
    {
        private const double delta_t = 0.001;

        /// <summary>
        /// Build a plot from datapoints
        /// </summary>
        /// <param name="plotName">Name of plot</param>
        /// <param name="seriesName">Name of series</param>
        /// <param name="dataPoints"></param>
        /// <param name="marker">Set true if you need mark points on series</param>
        /// <overloads>Build a plot from series</overloads>
        /// <returns></returns>
        public static PlotModel BuildModel(string plotName, string seriesName, List<DataPoint> dataPoints, bool marker = false)
        {
            PlotModel plotModel = new() { Title = plotName };
            var series = new LineSeries
            {
                Title = seriesName,
                MarkerType = marker ? MarkerType.Circle : MarkerType.None
            };
            series.Points.AddRange(dataPoints);
            plotModel.Series.Add(series);
            return plotModel;
        }

        public static PlotModel BuildModel(string plotName, params Series[] series)
        {
            PlotModel plotModel = new() { Title = plotName };
            foreach (var s in series)
                plotModel.Series.Add(s);
            return plotModel;
        }

        /// <summary>
        /// Convert list of datapoints to linear series
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <param name="seriesName"></param>
        /// <param name="marker">Set true if you need mark points on series</param>
        /// <returns></returns>
        public static LineSeries ConvertToLineSeries(List<DataPoint> dataPoints, string seriesName, bool marker = false)
        {
            var series = new LineSeries
            {
                Title = seriesName,
                MarkerType = marker ? MarkerType.Circle : MarkerType.None
            };
            series.Points.AddRange(dataPoints);
            return series;
        }

        public static List<DataPoint> LinearTrend(int N, double a, double b, double delta_t = delta_t)
        {
            List<DataPoint> list = new(N);
            for (int i = 0; i < N; i++)
                list.Add(LinearTrendPoint(i * delta_t, a, b));
            return list;
        }

        private static DataPoint LinearTrendPoint(double t, double a, double b)
            => new(t, a * t + b);

        public static List<DataPoint> ExpTrend(int N, double a, double b, double delta_t = delta_t)
        {
            List<DataPoint> list = new(N);
            for (int i = 0; i < N; i++)
                list.Add(ExpTrendPoint(i * delta_t, a, b));
            return list;
        }

        private static DataPoint ExpTrendPoint(double t, double a, double b)
            => new(t, b * Exp(-a * t));

        public static List<DataPoint> RandomNoiseTrend(int N, double R, double delta_t = delta_t)
        {
            List<DataPoint> list = new(N);
            Random random = new();
            for (int i = 0; i < N; i++)
            {
                double temp = random.Next();
                temp = (temp / int.MaxValue - 0.5) * 2 * R;
                list.Add(new DataPoint(i * delta_t, temp));
            }
            return list;
        }

        public static List<DataPoint> MyRandomNoiseTrend(int N, double R, double delta_t = delta_t)
        {
            List<DataPoint> list = new(N);
            MyRandom myrandom = new MyRandom();
            for (int i = 0; i < N; i++)
            {
                double temp = myrandom.Next();
                temp = temp / int.MaxValue * R;
                list.Add(new DataPoint(i * delta_t, temp));
            }
            return list;
        }

        public static List<DataPoint> ImpulseNoiseTrend(int N, int M, double R, double delta_t = delta_t)
        {
            List<DataPoint> list = new(N);
            Random random = new();
            for (int i = 0; i < N; i++)
                list.Add(new DataPoint(i * delta_t, (random.Next(0, 2) == 0 ? 1 : -1) * random.NextDouble() * (R / 100)));
            for (int i = 0; i < M; i++)
            {
                int x = 0;
                x += random.Next(0, N);
                int sign = random.Next(0, 2) == 0 ? 1 : -1;
                int rs_sign = random.Next(0, 2) == 0 ? 1 : -1;
                double RS = random.NextDouble() * (R / 10);
                double y = sign * R + rs_sign * RS;
                list[x] = new DataPoint(x * delta_t, y);
            }
            return list;
        }

        public static List<DataPoint> ImpulseT_NoiseTrend(int N, int T, double R, bool s = false, double delta_t = delta_t)
        {
            List<DataPoint> list = new(N);
            Random random = new();
            for (int i = 0; i < N; i++)
                list.Add(new DataPoint(i * delta_t, 0));
            for (int i = 1; i < N / T; i++)
            {
                int sign;
                if (!s)
                    sign = random.Next(0, 2) == 0 ? 1 : -1;
                else
                    sign = 1;
                int rs_sign = random.Next(0, 2) == 0 ? 1 : -1;
                double RS = random.NextDouble() * (R / 10);
                double y = sign * R + rs_sign * RS;
                list[i*T] = new DataPoint(i * T * delta_t, y);
            }
            return list;
        }

        public static void Shift(double S, ref List<DataPoint> list)
        {
            DataPointOperations.SumPointsWithVar(ref list, S);
        }

        private static double HarmFuncValue(int i, double A, double f, double delta_t = delta_t)
            => A * Sin(2 * PI * f * i * delta_t);

        public static List<DataPoint> HarmTrend(int N, double A, double f, double delta_t = delta_t)
        {
            List<DataPoint> list = new(N);
            for (int i = 0; i < N; i++)
                list.Add(new DataPoint(i * delta_t, HarmFuncValue(i, A, f)));
            return list;
        }

        public static List<DataPoint> PolyHarmTrend(int N, List<HarmParam> param, double delta_t = delta_t)
        {
            List<DataPoint> list = new(N);
            double[] temp = new double[N];
            foreach (var p in param)
                for (int i = 0; i < N; i++)
                    temp[i] += HarmFuncValue(i, p.A, p.f, delta_t);

            for (int i = 0; i < N; i++)
                list.Add(new DataPoint(i * delta_t, temp[i]));
            return list;
        }
    }
}
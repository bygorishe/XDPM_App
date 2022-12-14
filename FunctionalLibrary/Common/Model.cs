using XDPM_App.Common;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using static System.Math;
using OxyPlot.Axes;
using System.Linq;

namespace XDPM_App.ADMP
{
    public static class Model
    {
        private const double _deltaT = 0.001;

        /// <summary>
        /// Build a plot from datapoints
        /// </summary>
        /// <param name="plotName">Name of plot</param>
        /// <param name="seriesName">Name of series</param>
        /// <param name="dataPoints"></param>
        /// <param name="showMarker">Set true if you need mark points on series</param>
        /// <overloads>Build a plot from series</overloads>
        /// <returns></returns>
        public static PlotModel BuildModel(string plotName, string seriesName, List<DataPoint> dataPoints, MarkerType markerType = MarkerType.None)
        {
            PlotModel plotModel = new() { Title = plotName };
            var series = new LineSeries
            {
                Title = seriesName,
                MarkerType = markerType
            };
            series.Points.AddRange(dataPoints);
            plotModel.Series.Add(series);
            return plotModel;
        }

        public static PlotModel BuildDateModel(string plotName, string seriesName, List<DataPoint> dataPoints, MarkerType markerType = MarkerType.None)
        {
            PlotModel plotModel = new() { Title = plotName };
            plotModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom/*, Minimum = minValue, Maximum = maxValue*/, StringFormat = "dd-MM-yyyy" });
            var series = new FunctionSeries
            {
                Title = seriesName,
                MarkerType = markerType
            };
            series.Points.AddRange(dataPoints);
            plotModel.Series.Add(series);
            return plotModel;
        }

        public static PlotModel BuildHistModel(string plotName, string seriesName, List<DataPoint> dataPoints, int barCount = 10, MarkerType markerType = MarkerType.None)
        {
            var valueList = DataPointOperations.GetValue(dataPoints);
            double min = valueList.Min();
            double max = valueList.Max();
            double step = (max - min) / barCount;
            List<double> ranges = new(barCount + 1);
            List<HistogramItem> bars = new(barCount + 1);
            List<int> count = new(barCount);
            PlotModel plotModel = new() { Title = plotName };
            for (int i = 0; i < barCount; i++)
            {
                ranges.Add(min + step * i);
                count.Add(0);
            }
            ranges.Add(min + step * barCount);
            foreach (var value in valueList)
                for (int i = 0; i < barCount; i++)
                    if (ranges[i] <= value && value <= ranges[i + 1])
                    {
                        count[i]++;
                        break;
                    }
            for (int i = 0; i < barCount; i++)
                bars.Add(new HistogramItem(ranges[i], ranges[i + 1], count[i] * (ranges[i + 1] - ranges[i]), count[i]));
            HistogramSeries series = new() { StrokeThickness = 1 };
            series.Items.AddRange(bars);
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
        /// <param name="showMarker">Set true if you need mark points on series</param>
        /// <returns></returns>
        public static LineSeries ConvertToLineSeries(List<DataPoint> dataPoints, string seriesName, MarkerType markerType = MarkerType.None)
        {
            var series = new LineSeries
            {
                Title = seriesName,
                MarkerType = markerType
            };
            series.Points.AddRange(dataPoints);
            return series;
        }

        public static List<DataPoint> LinearTrend(int N, double a, double b, double delta_t = _deltaT)
        {
            List<DataPoint> dataPoints = new(N);
            for (int i = 0; i < N; i++)
                dataPoints.Add(LinearTrendPoint(i * delta_t, a, b));
            return dataPoints;
        }

        private static DataPoint LinearTrendPoint(double t, double a, double b)
            => new(t, a * t + b);

        public static List<DataPoint> ExpTrend(int N, double a, double b, double delta_t = _deltaT)
        {
            List<DataPoint> dataPoints = new(N);
            for (int i = 0; i < N; i++)
                dataPoints.Add(ExpTrendPoint(i * delta_t, a, b));
            return dataPoints;
        }

        private static DataPoint ExpTrendPoint(double t, double a, double b)
            => new(t, b * Exp(-a * t));

        public static List<DataPoint> RandomNoiseTrend(int N, double R, double delta_t = _deltaT)
        {
            List<DataPoint> dataPoints = new(N);
            Random random = new();
            for (int i = 0; i < N; i++)
            {
                double value = random.Next();
                value = (value / int.MaxValue - 0.5) * 2 * R;
                dataPoints.Add(new DataPoint(i * delta_t, value));
            }
            return dataPoints;
        }

        public static List<DataPoint> MyRandomNoiseTrend(int N, double R, double delta_t = _deltaT)
        {
            List<DataPoint> dataPoints = new(N);
            MyRandom myRandom = new();
            for (int i = 0; i < N; i++)
            {
                double value = myRandom.Next();
                value = value / int.MaxValue * R;
                dataPoints.Add(new DataPoint(i * delta_t, value));
            }
            return dataPoints;
        }

        public static List<DataPoint> ImpulseNoiseTrend(int N, int M, double R, double delta_t = _deltaT)
        {
            List<DataPoint> dataPoints = new(N);
            Random random = new();
            for (int i = 0; i < N; i++)
                dataPoints.Add(new DataPoint(i * delta_t, (random.Next(0, 2) == 0 ? 1 : -1) * random.NextDouble() * (R / 100)));
            for (int i = 0; i < M; i++)
            {
                int x = 0;
                x += random.Next(0, N);
                int sign = random.Next(0, 2) == 0 ? 1 : -1;
                int rs_sign = random.Next(0, 2) == 0 ? 1 : -1;
                double RS = random.NextDouble() * (R / 10);
                double y = sign * R + rs_sign * RS;
                dataPoints[x] = new DataPoint(x * delta_t, y);
            }
            return dataPoints;
        }

        public static List<DataPoint> ImpulseT_NoiseTrend(int N, int T, double R, bool negativeSign = false, double delta_t = _deltaT)
        {
            List<DataPoint> dataPoints = new(N);
            Random random = new();
            for (int i = 0; i < N; i++)
                dataPoints.Add(new DataPoint(i * delta_t, 0));
            for (int i = 1; i < N / T; i++)
            {
                int sign;
                if (!negativeSign)
                    sign = random.Next(0, 2) == 0 ? 1 : -1;
                else
                    sign = 1;
                int rs_sign = random.Next(0, 2) == 0 ? 1 : -1;
                double RS = random.NextDouble() * (R / 10);
                double y = sign * R + rs_sign * RS;
                dataPoints[i * T] = new DataPoint(i * T * delta_t, y);
            }
            return dataPoints;
        }

        public static void Shift(double S, ref List<DataPoint> dataPoints)
        {
            DataPointOperations.SumPointsWithNumber(ref dataPoints, S);
        }

        private static double HarmFuncValue(int i, double A, double f, double delta_t = _deltaT)
            => A * Sin(2 * PI * f * i * delta_t);

        public static List<DataPoint> HarmTrend(int N, double A, double f, double delta_t = _deltaT)
        {
            List<DataPoint> dataPoints = new(N);
            for (int i = 0; i < N; i++)
                dataPoints.Add(new DataPoint(i * delta_t, HarmFuncValue(i, A, f)));
            return dataPoints;
        }

        public static List<DataPoint> PolyHarmTrend(int N, List<HarmParam> harmParams, double delta_t = _deltaT)
        {
            List<DataPoint> dataPoints = new(N);
            double[] value = new double[N];
            foreach (var parametrs in harmParams)
                for (int i = 0; i < N; i++)
                    value[i] += HarmFuncValue(i, parametrs.A, parametrs.f, delta_t);

            for (int i = 0; i < N; i++)
                dataPoints.Add(new DataPoint(i * delta_t, value[i]));
            return dataPoints;
        }
    }
}
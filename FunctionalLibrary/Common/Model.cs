using XDPM_App.Common;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using static System.Math;
using OxyPlot.Axes;
using System.Linq;
using MathNet.Numerics.Distributions;
using FunctionalLibrary.Helpers.Parametrs;
using FunctionalLibrary.Helpers.Operations;

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
        public static PlotModel BuildModel(string plotName, string seriesName, List<DataPoint> dataPoints,
            MarkerType markerType = MarkerType.None, OxyColor? seriesColor = null)
        {
            PlotModel plotModel = new() { Title = plotName };
            var series = new LineSeries
            {
                Title = seriesName,
                MarkerType = markerType,
                Color = seriesColor != null ? (OxyColor)seriesColor : OxyColors.Automatic,
            };
            series.Points.AddRange(dataPoints);
            plotModel.Series.Add(series);
            return plotModel;
        }

        public static PlotModel BuildDateModel(string plotName, string seriesName, List<DataPoint> dataPoints,
            MarkerType markerType = MarkerType.None, OxyColor? seriesColor = null)
        {
            PlotModel plotModel = new() { Title = plotName };
            plotModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, StringFormat = "dd-MM-yyyy" });
            var series = new FunctionSeries
            {
                Title = seriesName,
                MarkerType = markerType,
                Color = seriesColor != null ? (OxyColor)seriesColor : OxyColors.Automatic,
            };
            series.Points.AddRange(dataPoints);
            plotModel.Series.Add(series);
            return plotModel;
        }

        public static PlotModel BuildHistModel(string plotName, List<DataPoint> dataPoints, int barCount = 10,
            OxyColor? seriesColor = null)
        {
            var valueList = DataPointOperations.GetValue(dataPoints);
            double min = valueList.Min();
            double max = valueList.Max();
            double step = (max - min) / barCount;
            List<double> ranges = new(barCount + 1);
            List<HistogramItem> bars = new(barCount + 1);
            int[] count = new int[barCount];
            PlotModel plotModel = new() { Title = plotName };
            for (int i = 0; i <= barCount; i++) //полная хуйня, переделать   
                ranges.Add(min + step * i);
            foreach (var value in valueList)
                for (int i = 0; i < barCount; i++)
                    if (ranges[i] <= value && value <= ranges[i + 1])
                    {
                        count[i]++;
                        break;
                    }
            for (int i = 0; i < barCount; i++)
                bars.Add(new HistogramItem(ranges[i], ranges[i + 1], count[i] * (ranges[i + 1] - ranges[i]), count[i]));
            HistogramSeries series = new()
            {
                StrokeThickness = 1,
                FillColor = seriesColor != null ? (OxyColor)seriesColor : OxyColors.Automatic,
            };
            series.Items.AddRange(bars);
            plotModel.Series.Add(series);
            return plotModel;
        }

        public static PlotModel BuildHistModel(string plotName, double[] dataPoints, OxyColor? seriesColor = null)
        {
            //var valueList = DataPointOperations.GetValue(dataPoints);
            //double min = valueList.Min();
            //double max = valueList.Max();
            //double step = (max - min) / barCount;
            PlotModel plotModel = new() { Title = plotName };

            List<HistogramItem> bars = new(dataPoints.Length);


            for (int i = 0; i < dataPoints.Length; i++)
                bars.Add(new HistogramItem(i, i + 1, dataPoints[i], (int)dataPoints[i]));
            HistogramSeries series = new()
            {
                StrokeThickness = 1,
                FillColor = seriesColor != null ? (OxyColor)seriesColor : OxyColors.Automatic,
            };
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
        
        public static PlotModel BuildModel(string plotName, List<List<DataPoint>> series)
        {
            PlotModel plotModel = new() { Title = plotName };
            foreach (var s in series)
                plotModel.Series.Add(ConvertToLineSeries(s, ""));
            return plotModel;
        }

        /// <summary>
        /// Convert list of datapoints to linear series
        /// </summary>
        /// <param name="dataPoints"></param>
        /// <param name="seriesName"></param>
        /// <param name="showMarker">Set true if you need mark points on series</param>
        /// <returns></returns>
        public static LineSeries ConvertToLineSeries(List<DataPoint> dataPoints, string seriesName,
            MarkerType markerType = MarkerType.None, OxyColor? seriesColor = null)
        {
            var series = new LineSeries
            {
                Title = seriesName,
                MarkerType = markerType,
                Color = seriesColor != null ? (OxyColor)seriesColor : OxyColors.Automatic,
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

        public static List<DataPoint> GBM(List<DataPoint> dataPoints, int N, int rangeNumber, double delta_t = 1)
        {
            List<DataPoint> newDataPoints = new(N);
            List<double> value = DataPointOperations.GetValue(dataPoints);
            int size = N / rangeNumber;
            for (int j = 0; j < rangeNumber; j++)
            {
                double[] rt = new double[size];
                for (int i = 1; i < size; i++)
                    rt[i] = (value[i + j * size] - value[i + j * size - 1]) / value[i + j * size - 1];
                double mu = rt.Average();
                double sum = 0;
                double[] xk = new double[size];
                for (int i = 0; i < size; i++)
                {
                    xk[i] = rt[i] - mu;
                    sum += xk[i] * xk[i];
                }
                double sigma = Sqrt(sum / size);
                double S0 = value[j * size];
                double[] W = new double[size];
                Random random = new();
                double[] S = new double[size];
                for (int i = 0; i < size; i++)
                {
                    W[i] = Normal.Sample(random, 0.0, 1.0);
                    S[i] = S0 * Exp(mu - 0.5 * (sigma * sigma) * delta_t + sigma * W[i] * Sqrt(delta_t));
                    S0 = S[i];
                    newDataPoints.Add(new DataPoint(i + j * size, S[i]));
                }
            }
            return newDataPoints;
        }

        private static DataPoint ExpTrendPoint(double t, double a, double b)
            => new(t, a * Exp(b));

        public static List<DataPoint> RandomNoiseTrend(int N, double R, double delta_t = _deltaT,
            bool canBeNegative = true)
        {
            List<DataPoint> dataPoints = new(N);
            Random random = new();
            if (canBeNegative)
                for (int i = 0; i < N; i++)
                {
                    double value = random.Next();
                    value = (value / int.MaxValue - 0.5) * 2 * R;
                    dataPoints.Add(new DataPoint(i * delta_t, value));
                }
            else
                for (int i = 0; i < N; i++)
                {
                    double value = random.Next();
                    value = value / int.MaxValue * R;
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
                dataPoints.Add(new DataPoint(i * delta_t, (random.Next(0, 2) == 0 ? 1 : -1)
                    * random.NextDouble() * (R / 100)));
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

        public static List<DataPoint> ImpulseT_NoiseTrend(int N, int T, double R, bool negativeSign = false,
            double delta_t = _deltaT)
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

        private static double HarmFuncValue(int i, double A, double f, double delta_t = _deltaT)
            => A * Sin(2 * PI * f * i * delta_t);

        public static List<DataPoint> HarmTrend(int N, double A, double f, double delta_t = _deltaT)
        {
            List<DataPoint> dataPoints = new(N);
            for (int i = 0; i < N; i++)
                dataPoints.Add(new DataPoint(i * delta_t, HarmFuncValue(i, A, f)));
            return dataPoints;
        }

        public static List<DataPoint> PolyHarmTrend(int N, List<HarmParams> harmParams, double delta_t = _deltaT)
        {
            List<DataPoint> dataPoints = new(N);
            double[] value = new double[N];
            foreach (var parametrs in harmParams)
                for (int i = 0; i < N; i++)
                    value[i] += HarmFuncValue(i, parametrs.A, parametrs.F, delta_t);

            for (int i = 0; i < N; i++)
                dataPoints.Add(new DataPoint(i * delta_t, value[i]));
            return dataPoints;
        }

        public static double[] ImageNoise(double[] bytes, int R = 10)
        {
            double[] newBytes = bytes;
            Random random = new Random();
            for (int i = 0; i < newBytes.Length; i++)
            {
                double value = random.Next();
                value = (value / int.MaxValue - 0.5) * 2 * R;
                newBytes[i++] += value;
                newBytes[i++] += value;
                newBytes[i++] += value;
            }
            return newBytes;
        }

        public static double[] ImageSaltAndPepper(double[] bytes, double chance = 0.1)
        {
            double[] newBytes = bytes;
            Random random = new Random();
            for (int i = 0; i < newBytes.Length; i += 4)
            {
                double value = random.NextSingle();
                if (chance / 2 <= value && value < chance)
                {
                    newBytes[i] = 0;
                    newBytes[i + 1] = 0;
                    newBytes[i + 2] = 0;
                }
                else if (0 <= value && value < chance / 2)
                {
                    newBytes[i] = 255;
                    newBytes[i + 1] = 255;
                    newBytes[i + 2] = 255;
                }
            }
            return newBytes;
        }
    }
}
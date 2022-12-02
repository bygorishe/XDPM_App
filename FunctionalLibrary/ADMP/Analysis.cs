using XDPM_App.Common;
using OxyPlot;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace XDPM_App.ADMP
{
    public class Analysis
    {
        private const double delta_t = 0.001;
        private readonly int N, m;
        public double min, max, M, D, beta, sigma, eps, gamma1, gamma2;
        public bool stationarity;
        private readonly List<double> f;
        private List<double> xk_X = null!;
        private List<double> AvevageValue_i = null!;
        private List<double> StandartOtklonenie_i = null!;

        public Analysis(List<DataPoint> dataPoints, int N)
        {
            this.N = N;
            f = DataPointOperations.GetValue(dataPoints);
        }
        public Analysis(List<DataPoint> dataPoints, int N, int M)
        {
            this.N = N;
            m = M;
            f = DataPointOperations.GetValue(dataPoints);
            min = f.Min();
            max = f.Max();
            this.M = ExpectedValue();
            D = Variation();
            beta = Betta();
            sigma = Sigma();
            eps = Epsilon();
            gamma1 = Assimetry() / Pow(beta, 3);
            gamma2 = Kurtosis() / Pow(beta, 4) - 3;
            stationarity = Stationarity();
        }

        private double ExpectedValue() => f.Average();

        private double Variation()
        {
            double sum = 0;
            xk_X = new List<double>(N);
            for (int i = 0; i < N; i++) //xk - x
                xk_X.Add(f[i] - M);
            for (int i = 0; i < N; i++)
                sum += xk_X[i] * xk_X[i];
            return sum / N;
        }

        private double Betta() => Sqrt(D);

        private double Sigma()
        {
            double sum = 0;
            for (int i = 0; i < N; i++)
                sum += f[i] * f[i];
            return sum / N;
        }

        private double Epsilon() => Sqrt(sigma);

        private double Assimetry()
        {
            double sum = 0;
            for (int i = 0; i < N; i++)
                sum += Pow(xk_X[i], 3);
            return sum / N;
        }

        private double Kurtosis()
        {
            double sum = 0;
            for (int i = 0; i < N; i++)
                sum += Pow(xk_X[i], 4);
            return sum / N;
        }

        private bool Stationarity()
        {
            int step = N / m;
            AvevageValue_i = new List<double>(m);
            StandartOtklonenie_i = new List<double>(m);
            for (int k = 0; k < m; k++)
            {
                double sum1 = 0, sum2 = 0;
                for (int i = k * step; i < (k + 1) * step; i++)
                {
                    sum1 += f[i];
                    sum2 += xk_X[i] * xk_X[i];
                }
                AvevageValue_i.Add(sum1 / step);
                StandartOtklonenie_i.Add(Abs(sum2 / step));
            }
            double t;
            if (max.Equals(0)) t = min;
            else if (min.Equals(0)) t = max;
            else t = max - min;
            for (int i = 0; i < m; i++)
            {
                for (int j = i + 1; j < m; j++)
                    if (Abs((AvevageValue_i[i] - AvevageValue_i[j]) / t) > 0.1 ||
                    Abs((StandartOtklonenie_i[i] - StandartOtklonenie_i[j]) / t) > 0.1) // для линейных проверить
                        return false;
                //if (Abs((AvevageValue_i[i] - AvevageValue_i[j]) / AvevageValue_i[j]) > 0.1 ||
                //Abs((StandartOtklonenie_i[i] - StandartOtklonenie_i[j]) / StandartOtklonenie_i[j]) > 0.1) //  ПЕРЕДЕЛАТЬ
                //    return false;
            }
            //for (int i = 0; i < m; i++)
            //{
            //    for (int j = i + 1; j < m; j++)
            //        if (Abs(AvevageValue_i[i] - AvevageValue_i[j]) > 0.1 &&
            //        Abs(StandartOtklonenie_i[i] - StandartOtklonenie_i[j]) > 0.1) // для линейных проверить
            //            return false;
            //    //if (Abs((AvevageValue_i[i] - AvevageValue_i[j]) / AvevageValue_i[j]) > 0.1 &&
            //    //Abs((StandartOtklonenie_i[i] - StandartOtklonenie_i[j]) / StandartOtklonenie_i[j]) > 0.1) //  ПЕРЕДЕЛАТЬ
            //    //    return false;
            //}
            return true;
        }

        public PlotModel Hist(string name)
        {
            double step = (max - min) / m;
            List<double> lsit = new(m + 1);
            List<HistogramItem> bars = new(m + 1);
            List<int> count = new(m);
            PlotModel plotModel = new() { Title = name };
            for (int i = 0; i < m; i++)
            {
                lsit.Add(min + step * i);
                count.Add(0);
            }
            lsit.Add(min + step * m);
            foreach (var c in f)
                for (int i = 0; i < m; i++)
                    if (lsit[i] <= c && c <= lsit[i + 1])
                    {
                        count[i]++;
                        break;
                    }
            for (int i = 0; i < m; i++)
                bars.Add(new HistogramItem(lsit[i], lsit[i + 1], count[i] * (lsit[i + 1] - lsit[i]), count[i]));
            HistogramSeries series = new() { StrokeThickness = 1 };
            series.Items.AddRange(bars);
            plotModel.Series.Add(series);
            return plotModel;
        }

        private double Rxx(int L)
        {
            double r = 0;
            for (int i = 0; i < N - L; i++)
                r += xk_X[i] * (f[i + L] - M);
            return r / N;
        }

        public List<double> GetXk() => xk_X;

        public List<DataPoint> ACF(double delta_t = delta_t)
        {
            List<DataPoint> series = new(N);
            double max = double.MinValue;
            List<double> points = new(N);
            for (int i = 0; i < N; i++)
            {
                double temp = Rxx(i);
                if (max < temp) max = temp;
                points.Add(temp);
            }
            for (int i = 0; i < N; i++)
                series.Add(new DataPoint(i * delta_t, points[i] / max));
            return series;
        }

        private double Rxy(int L, List<double> t)
        {
            double r = 0;
            for (int i = 0; i < N - L - 1; i++)
                r += xk_X[i] * (t[i + L] - M);
            return r / N;
        }

        public List<DataPoint> CCF(List<double> t, double delta_t = delta_t)
        {
            List<DataPoint> series = new(N);
            List<double> points = new(N);
            for (int i = 0; i < N; i++)
                points.Add(Rxy(i, t));
            for (int i = 0; i < N; i++)
                series.Add(new DataPoint(i * delta_t, points[i]));
            return series;
        }

        public List<double> Fourier(int L) //имееться лист f, которые содержит значения функции, следовательно, если анализировали тренд, то f существует
        {
            double Re, Im;
            List<double> X = new(N);
            for (int i = 0; i < N; i++)
            {
                Re = 0;
                Im = 0;
                for (int k = 0; k < N - L; k++)
                {
                    Re += f[k] * Cos(2 * PI * i * k / N);
                    Im += f[k] * Sin(2 * PI * i * k / N);
                }
                Re /= N;
                Im /= N;
                X.Add(Sqrt(Re * Re + Im * Im));
            }
            return X;
        }

        public List<DataPoint> SpectrFourier(int n, int L, double delta_t = delta_t)
        {
            double f_top = 1 / (2 * delta_t),
                f_delta = 2 * f_top / (n * 2);
            List<double> temp = Fourier(L);
            List<DataPoint> dataPoints = new(n);
            for (int i = 0; i < n; i++)
                dataPoints.Add(new DataPoint(i * f_delta, temp[i]));
            return dataPoints;
        }
    }
}

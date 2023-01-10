using XDPM_App.Common;
using OxyPlot;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Linq;
using static System.Math;
using System;

namespace XDPM_App.ADMP
{
    public class Analysis
    {
        private const double _deltaT = 0.001;
        private readonly int _N, _analysisSpan;
        public double Min, Max, M, D, Betta, Sigma, Eps, Gamma1, Gamma2;
        public bool Sationarity;
        private readonly List<double> _valueList;
        private List<double> _xk = null!;
        private List<double> _averageValue = null!;
        private List<double> _standartDeviation = null!;

        public Analysis(List<DataPoint> dataPoints, int N)
        {
            if (dataPoints == null) throw new Exception("NULL Data Points");
            _N = N;
            _valueList = DataPointOperations.GetValue(dataPoints);
        }
        public Analysis(List<DataPoint> dataPoints, int N, int M) : this(dataPoints, N)
        {
            _analysisSpan = M;
            Min = _valueList.Min();
            Max = _valueList.Max();
            this.M = ExpectedValue();
            D = Variation();
            Betta = Sqrt(D);
            Sigma = SigmaValue();
            Eps = Sqrt(Sigma);
            Gamma1 = Assimetry() / Pow(Betta, 3);
            Gamma2 = Kurtosis() / Pow(Betta, 4) - 3;
            Sationarity = Stationarity();
        }

        private double ExpectedValue() => _valueList.Average();

        private double Variation()
        {
            double sum = 0;
            _xk = new List<double>(_N);
            for (int i = 0; i < _N; i++) //xk - x
                _xk.Add(_valueList[i] - M);
            for (int i = 0; i < _N; i++)
                sum += _xk[i] * _xk[i];
            return sum / _N;
        }

        private double SigmaValue()
        {
            double sum = 0;
            for (int i = 0; i < _N; i++)
                sum += _valueList[i] * _valueList[i];
            return sum / _N;
        }

        private double Assimetry()
        {
            double sum = 0;
            for (int i = 0; i < _N; i++)
                sum += Pow(_xk[i], 3);
            return sum / _N;
        }

        private double Kurtosis()
        {
            double sum = 0;
            for (int i = 0; i < _N; i++)
                sum += Pow(_xk[i], 4);
            return sum / _N;
        }

        private bool Stationarity()
        {
            int step = _N / _analysisSpan;
            _averageValue = new List<double>(_analysisSpan);
            _standartDeviation = new List<double>(_analysisSpan);
            for (int k = 0; k < _analysisSpan; k++)
            {
                double sum1 = 0, sum2 = 0;
                for (int i = k * step; i < (k + 1) * step; i++)
                {
                    sum1 += _valueList[i];
                    sum2 += _xk[i] * _xk[i];
                }
                _averageValue.Add(sum1 / step);
                _standartDeviation.Add(Abs(sum2 / step));
            }
            double t;
            if (Max.Equals(0)) t = Min;
            else if (Min.Equals(0)) t = Max;
            else t = Max - Min;
            for (int i = 0; i < _analysisSpan; i++)
            {
                for (int j = i + 1; j < _analysisSpan; j++)
                    if (Abs((_averageValue[i] - _averageValue[j]) / t) > 0.1 ||
                    Abs((_standartDeviation[i] - _standartDeviation[j]) / t) > 0.1) // для линейных проверить
                        return false;
                //if (Abs((AvevageValue_i[i] - AvevageValue_i[j]) / AvevageValue_i[j]) > 0.1 ||
                //Abs((StandartOtklonenie_i[i] - StandartOtklonenie_i[j]) / StandartOtklonenie_i[j]) > 0.1) //  ПЕРЕДЕЛАТЬ
                //    return false;
            }
            return true;
        }

        public PlotModel Hist(string name)
        {
            double step = (Max - Min) / _analysisSpan;
            List<double> ranges = new(_analysisSpan + 1);
            List<HistogramItem> bars = new(_analysisSpan + 1);
            List<int> count = new(_analysisSpan);
            PlotModel plotModel = new() { Title = name };
            for (int i = 0; i < _analysisSpan; i++)
            {
                ranges.Add(Min + step * i);
                count.Add(0);
            }
            ranges.Add(Min + step * _analysisSpan);
            foreach (var c in _valueList)
                for (int i = 0; i < _analysisSpan; i++)
                    if (ranges[i] <= c && c <= ranges[i + 1])
                    {
                        count[i]++;
                        break;
                    }
            for (int i = 0; i < _analysisSpan; i++)
                bars.Add(new HistogramItem(ranges[i], ranges[i + 1], count[i] * (ranges[i + 1] - ranges[i]), count[i]));
            HistogramSeries series = new() { StrokeThickness = 1 };
            series.Items.AddRange(bars);
            plotModel.Series.Add(series);
            return plotModel;
        }

        private double Rxx(int L)
        {
            double r = 0;
            for (int i = 0; i < _N - L; i++)
                r += _xk[i] * (_valueList[i + L] - M);
            return r / _N;
        }

        public List<double> GetXk() => _xk;

        public List<DataPoint> ACF(double delta_t = _deltaT)
        {
            List<DataPoint> dataPoints = new(_N);
            double max = double.MinValue;
            List<double> pointsValue = new(_N);
            for (int i = 0; i < _N; i++)
            {
                double temp = Rxx(i);
                if (max < temp) max = temp;
                pointsValue.Add(temp);
            }
            for (int i = 0; i < _N; i++)
                dataPoints.Add(new DataPoint(i * delta_t, pointsValue[i] / max));
            return dataPoints;
        }

        private double Rxy(int L, List<double> t)
        {
            double r = 0;
            for (int i = 0; i < _N - L - 1; i++)
                r += _xk[i] * (t[i + L] - M);
            return r / _N;
        }

        public List<DataPoint> CCF(List<double> t, double delta_t = _deltaT)
        {
            List<DataPoint> dataPoints = new(_N);
            List<double> pointsValue = new(_N);
            for (int i = 0; i < _N; i++)
                pointsValue.Add(Rxy(i, t));
            for (int i = 0; i < _N; i++)
                dataPoints.Add(new DataPoint(i * delta_t, pointsValue[i]));
            return dataPoints;
        }

        public List<double> Fourier(int L) //имееться лист f, которые содержит значения функции, следовательно, если анализировали тренд, то f существует
        {
            double Re, Im;
            List<double> X = new(_N);
            for (int i = 0; i < _N; i++)
            {
                Re = 0;
                Im = 0;
                for (int k = 0; k < _N - L; k++)
                {
                    Re += _valueList[k] * Cos(2 * PI * i * k / _N);
                    Im += _valueList[k] * Sin(2 * PI * i * k / _N);
                }
                Re /= _N;
                Im /= _N;
                X.Add(Sqrt(Re * Re + Im * Im));
            }
            return X;
        }

        public List<DataPoint> SpectrFourier(int N, int L, double delta_t = _deltaT)
        {
            double f_top = 1 / (2 * delta_t),
                f_delta = 2 * f_top / N;
            List<double> temp = Fourier(L);
            List<DataPoint> dataPoints = new(N / 2);
            for (int i = 0; i < N / 2; i++)
                dataPoints.Add(new DataPoint(i * f_delta, temp[i]));
            return dataPoints;
        }
    }
}

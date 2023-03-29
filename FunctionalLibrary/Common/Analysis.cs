using OxyPlot;
using System.Collections.Generic;
using System.Linq;
using static System.Math;
using System;

namespace XDPM_App.ADMP
{
    public class StatisticsAnalysis
    {
        private const double _deltaT = 0.001;
        private readonly int _N, _analysisSpans;
        public double Min, Max, M, D, Sigma, MSE, Eps, Gamma1, Gamma2;
        public bool Stationarity;
        private readonly double[] _valueList;
        private double[] _xk = null!;
        private double[] _averageValue = null!;
        private double[] _standartDeviation = null!;


        public StatisticsAnalysis(double[] values, int N)
        {
            _N = N;
            _valueList = values ?? throw new Exception("NULL Data Points");
        }

        public StatisticsAnalysis(List<double> values, int N)
        {
            _N = N;
            _valueList = values.ToArray() ?? throw new Exception("NULL Data Points");
        }

        public StatisticsAnalysis(double[] values, int N, int spans) : this(values, N)
        {
            _analysisSpans = spans;
            Min = _valueList.Min();
            Max = _valueList.Max();
            M = ExpectedValue();
            D = Variation();
            Sigma = Sqrt(D);
            MSE = MSE_Value();
            Eps = Sqrt(MSE);
            Gamma1 = Assimetry() / Pow(Sigma, 3);
            Gamma2 = Kurtosis() / Pow(Sigma, 4) - 3;
            Stationarity = IsStationarity();
        }

        public StatisticsAnalysis(List<double> values, int N, int spans) : this(values, N)
        {
            _analysisSpans = spans;
            Min = _valueList.Min();
            Max = _valueList.Max();
            M = ExpectedValue();
            D = Variation();
            Sigma = Sqrt(D);
            MSE = MSE_Value();
            Eps = Sqrt(MSE);
            Gamma1 = Assimetry() / Pow(Sigma, 3);
            Gamma2 = Kurtosis() / Pow(Sigma, 4) - 3;
            Stationarity = IsStationarity();
        }

        private double ExpectedValue() => _valueList.Average();

        private double Variation()
        {
            double sum = 0;
            _xk = new double[_N];
            for (int i = 0; i < _N; i++) //xk - x
                _xk[i] = _valueList[i] - M;
            for (int i = 0; i < _N; i++)
                sum += _xk[i] * _xk[i];
            return sum / _N;
        }

        private double MSE_Value()
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

        private bool IsStationarity()
        {
            int step = _N / _analysisSpans;
            _averageValue = new double[_analysisSpans];
            _standartDeviation = new double[_analysisSpans];
            for (int k = 0; k < _analysisSpans; k++)
            {
                double sum1 = 0, sum2 = 0;
                for (int i = k * step; i < (k + 1) * step; i++)
                {
                    sum1 += _valueList[i];
                    sum2 += _xk[i] * _xk[i];
                }
                _averageValue[k] = sum1 / step;
                _standartDeviation[k] = Abs(sum2 / step);
            }
            double t;
            if (Max.Equals(0))
                t = Min;
            else if (Min.Equals(0))
                t = Max;
            else
                t = Max - Min;
            for (int i = 0; i < _analysisSpans; i++)
            {
                for (int j = i + 1; j < _analysisSpans; j++)
                    if (Abs((_averageValue[i] - _averageValue[j]) / t) > 0.1 ||
                    Abs((_standartDeviation[i] - _standartDeviation[j]) / t) > 0.1) // для линейных проверить
                        return false;
                //if (Abs((AvevageValue_i[i] - AvevageValue_i[j]) / AvevageValue_i[j]) > 0.1 ||
                //Abs((StandartOtklonenie_i[i] - StandartOtklonenie_i[j]) / StandartOtklonenie_i[j]) > 0.1) //  ПЕРЕДЕЛАТЬ
                //    return false;
            }
            return true;
        }

        private double Rxx(int L)
        {
            double r = 0;
            for (int i = 0; i < _N - L; i++)
                r += _xk[i] * (_valueList[i + L] - M);
            return r / _N;
        }

        public DataPoint[] ACF(double delta_t = _deltaT)
        {
            DataPoint[] dataPoints = new DataPoint[_N];
            double max = double.MinValue;
            List<double> pointsValue = new(_N);
            for (int i = 0; i < _N; i++)
            {
                double temp = Rxx(i);
                if (max < temp) max = temp;
                pointsValue.Add(temp);
            }
            for (int i = 0; i < _N; i++)
                dataPoints[i] = new DataPoint(i * delta_t, pointsValue[i] / max);
            return dataPoints;
        }

        private double Rxy(int L, double[] t)
        {
            double r = 0;
            for (int i = 0; i < _N - L - 1; i++)
                r += _xk[i] * (t[i + L] - M);
            return r / _N;
        }

        public DataPoint[] CCF(double[] t, double delta_t = _deltaT)
        {
            DataPoint[] dataPoints = new DataPoint[_N]; ;
            for (int i = 0; i < _N; i++)
                dataPoints[i] = new DataPoint(i * delta_t, Rxy(i, t));
            return dataPoints;
        }

        public double[] Fourier(int L = 0) //имееться лист f, которые содержит значения функции, следовательно, если анализировали тренд, то f существует
        {
            double Re, Im;
            double[] X = new double[_N];
            for (int i = 0; i < _N; i++)
            {
                Re = 0;
                Im = 0;
                for (int k = 0; k < _N - L; k++)
                {
                    double arg = 2 * PI * i * k / _N;
                    Re += _valueList[k] * Cos(arg);
                    Im += _valueList[k] * Sin(arg);
                }
                Re /= _N;
                Im /= _N;
                //X[i] = Re + Im;
                X[i] = Sqrt(Re * Re + Im * Im);
            }
            return X;
        }

        public DataPoint[] SpectrFourier(int N, int L = 0, double delta_t = _deltaT)
        {
            double f_top = 1 / (2 * delta_t),
                f_delta = 2 * f_top / N;
            double[] temp = Fourier(L);
            DataPoint[] dataPoints = new DataPoint[N / 2];
            for (int i = 0; i < N / 2; i++)
                dataPoints[i] = new DataPoint(i * f_delta, temp[i]);
            return dataPoints;
        }
    }

    public static class Analysis
    {
        const double _deltaT = 0.001;

        private static double Rxx(double[] values, int L)
        {
            double r = 0;
            int N = values.Length;
            double M = values.Average();
            double[] xk = new double[N];
            for (int i = 0; i < N; i++)
                xk[i] = values[i] - M;
            for (int i = 0; i < N - L; i++)
                r += xk[i] * (values[i + L] - M);
            return r / N;
        }

        public static double[] ACF(double[] values)
        {
            int N = values.Length;
            double max = double.MinValue;
            double[] pointsValue = new double[N];
            for (int i = 0; i < N; i++)
            {
                double temp = Rxx(values, i);
                if (max < temp) max = temp;
                pointsValue[i] = temp;
            }
            for (int i = 0; i < N; i++)
                pointsValue[i] /= max;
            return pointsValue;
        }

        private static double Rxy(int L, double[] values1, double[] values2)
        {
            int N = values1.Length;
            double r = 0;
            double M1 = values1.Average();
            double M2 = values2.Average();
            for (int i = 0; i < N - L - 1; i++)
                r += (values1[i] - M1) * (values2[i + L] - M2);
            return r / N;
        }

        public static double[] CCF(double[] values1, double[] values2)
        {
            int N = values1.Length;
            double[] pointsValue = new double[N];
            for (int i = 0; i < N; i++)
                pointsValue[i] = Rxy(i, values1, values2);
            return pointsValue;
        }

        public static double[] Fourier(double[] values, int L = 0) //имееться лист f, которые содержит значения функции, следовательно, если анализировали тренд, то f существует
        {
            int N = values.Length;
            double Re, Im;
            double[] X = new double[N];
            for (int i = 0; i < N; i++)
            {
                Re = 0;
                Im = 0;
                for (int k = 0; k < N - L; k++)
                {
                    double arg = 2 * PI * i * k / N;
                    Re += values[k] * Cos(arg);
                    Im += values[k] * Sin(arg);
                }
                Re /= N;
                Im /= N;
                X[i] = Re + Im;
                //X[i] = Sqrt(Re * Re + Im * Im);
            }
            return X;
        }

        public static double[] InverseFourier(double[] values)
        {
            int N = values.Length;
            double Re, Im;
            double[] X = new double[N];
            for (int i = 0; i < N; i++)
            {
                Re = 0;
                Im = 0;
                for (int k = 0; k < N; k++)
                {
                    double arg = 2 * PI * i * k / N;
                    Re += values[k] * Cos(arg);
                    Im += values[k] * Sin(arg);
                }
                //Re /= _N;
                //Im /= _N;
                //X[i] = Sqrt(Re * Re + Im * Im);
                X[i] = Re + Im;
            }
            return X;
        }

        public static DataPoint[] SpectrFourier(double[] values, int L = 0, double delta_t = _deltaT)
        {
            int N = values.Length;
            double f_top = 1 / (2 * delta_t),
                f_delta = 2 * f_top / N;
            double[] temp = Fourier(values, L);
            DataPoint[] dataPoints = new DataPoint[N / 2];
            for (int i = 0; i < N / 2; i++)
                dataPoints[i] = new DataPoint(i * f_delta, temp[i]);
            return dataPoints;
        }

        public static double[][] Fourier2D(ImageData data) //////////////mrak
        {
            double[][] newArray = new double[data.Height][];
            if (data.BytesMatrix == null)
                data.MakeByteMatrix();

            for (int i = 0; i < data.Height; i++)
            {
                newArray[i] = new double[data.Width];
                for (int j = 0; j < data.Width; j++)
                {
                    newArray[i][j] = data.BytesMatrix![i, j].Values[0];
                }
                newArray[i] = Fourier(newArray[i]);
            }

            double[][] tempArray = new double[data.Width][];
            for (int i = 0; i < data.Width; i++)
            {
                tempArray[i] = new double[data.Height];
                for (int j = 0; j < data.Height; j++)
                    tempArray[i][j] = newArray[data.Height - j - 1][i];
                tempArray[i] = Fourier(tempArray[i]);
            }

            for (int i = 0; i < data.Height; i++)
                for (int j = 0; j < data.Width; j++)
                    data.BytesMatrix![i, j] = new FunctionalLibrary.Helpers.Structs.RgbPixel(tempArray[data.Width - 1 - j][i]);

            data.MakeArrayFromMatrix();
            ImageProccesing.ByteScale(data);
            data.ConvertBytesIntoImage();
            return tempArray;
        }
    }
}

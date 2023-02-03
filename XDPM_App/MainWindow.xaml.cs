using XDPM_App.Common;
using XDPM_App.ADMP;
using OxyPlot;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using static XDPM_App.ADMP.Model;
using System.Windows.Input;
using System;
using System.Windows.Media;
using FunctionalLibrary.Common;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.IO;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows.Ink;
using System.Windows.Media.Media3D;

namespace XDPM_App
{
    public partial class MainWindow : Window
    {
        Dictionary<string, Data> datas = new();
        HarmParamWindow harmParamWindow = null!;

        public MainWindow()
        {
            InitializeComponent();
            BmpData data = new("C:/Users/angry/Desktop/grace.jpg");
            Image1.Source = data.Image;
            ImageProccesing.Shift(data, 30);
            data.Save("C:/Users/angry/Desktop/grace1.jpg");
            Image2.Source = data.Image;
            ImageProccesing.Mult(data, 1.3);
            data.Save("C:/Users/angry/Desktop/grace2.jpg");
            Image3.Source = data.Image;
        }

        //ComboBox comboBox = new ComboBox
        //{
        //    ItemsSource = typeof(Colors).GetProperties(),
        //};

        //private void CreateTabItem(object sender, RoutedEventArgs e)
        //{
        //    MainTabControl.Items.Add(new TabItem
        //    {
        //        Header = new TextBlock { Text = "s" },
        //        Name = "Name",
        //        Focusable = true,
        //    });
        //}

        //private void ComboBox_DropDownOpened(object sender, EventArgs e)
        //{
        //    if (comboBox.IsDropDownOpen)
        //    {
        //        _shouldOpenDropDown = false;
        //    }
        //    else
        //    {
        //        comboBox.IsDropDownOpen = false;
        //    }
        //}

        //private void ComboBox_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    _shouldOpenDropDown = true;
        //    comboBox.IsDropDownOpen = true;
        //}

        private void TextBox_paramN_TextChanged(object sender, TextChangedEventArgs e)
        {
            //BuildGraphButton.IsEnabled = false;
            //BuildNoiseButton.IsEnabled = false;
            //BuildHarmButton.IsEnabled = false;
        }

        private void NClick(object sender, RoutedEventArgs e)
        {
            //if (int.TryParse(TextBox_paramN.Text, out data.N) && data.N > 0)  //с доступом кнопок подумать еще
            //{
            //    BuildGraphButton.IsEnabled = true;
            //    BuildNoiseButton.IsEnabled = true;
            //    BuildHarmButton.IsEnabled = true;

            //    ////////////////////////////////////////////////////////////////////////
            //    data.implementNoiseMDependenceDP = new List<DataPoint>(data.N);
            //}
            //else
            //    MessageBox.Show("Введите положительное целое число N");
        }

        //private void BuildGraph(object sender, RoutedEventArgs e)
        //{
        //    if (double.TryParse(TextBox_paramA.Text.Replace(".", ","), out data.a)
        //    && double.TryParse(TextBox_paramB.Text.Replace(".", ","), out data.b))
        //    {
        //        data.positiveLinearDP = LinearTrend(data.N, data.a, data.b);
        //        PosLinTrendPlot.Model = BuildModel("Linear ax+b", "ax+b", data.positiveLinearDP);
        //        data.negativeLinearDP = LinearTrend(data.N, -data.a, data.b);
        //        NegLinTrendPlot.Model = BuildModel("Linear -ax+b", "-ax+b", data.negativeLinearDP);
        //        data.positiveExpDP = ExpTrend(data.N, data.a, data.b);
        //        PosExpTrendPlot.Model = BuildModel("Exp b*exp(-a)", "b*exp(-a)", data.positiveExpDP);
        //        data.negativeExpDP = ExpTrend(data.N, data.a, -data.b);
        //        NegExpTrendPlot.Model = BuildModel("Exp -b*exp(-a)", "-b*exp(-a)", data.negativeExpDP);
        //    }
        //    else
        //        MessageBox.Show("Ошибка в линейных параметрах");
        //}

        //private void BuildNoise(object sender, RoutedEventArgs e)
        //{
        //    if (int.TryParse(TextBox_NoiseparamN.Text, out data.IN_N)
        //    && double.TryParse(TextBox_NoiseparamR.Text.Replace(".", ","), out data.R))
        //    {
        //        data.randomNoiseDP = RandomNoiseTrend(data.N, data.R);
        //        data.myrandomNoiseDP = MyRandomNoiseTrend(data.N, data.R);
        //        data.impulseNoiseDP = ImpulseNoiseTrend(data.N, data.IN_N, data.R);
        //        RandNoisePlot.Model = BuildModel("Random Noise", "", data.randomNoiseDP);
        //        MyRandNoisePlot.Model = BuildModel("MyRandom Noise", "", data.myrandomNoiseDP);
        //        ImpulseNoisePlot.Model = BuildModel("Impulse Noise", "", data.impulseNoiseDP);
        //    }
        //    else
        //        MessageBox.Show("Ошибка в параметрах для шумов");
        //}

        //private void BuildHarm(object sender, RoutedEventArgs e)
        //{
        //    if (double.TryParse(TextBox_paramDt.Text.Replace(".", ","), out data.delta_t))
        //    {
        //        data.harmDP = HarmTrend(data.N, data.HarmParams[0].A, data.HarmParams[0].f);
        //        HarmPlot.Model = BuildModel("Harmonic", "", data.harmDP);
        //        data.polyHarnDP = PolyHarmTrend(data.N, data.HarmParams);
        //        PolyHarmPlot.Model = BuildModel("PolyHarmonic", "", data.polyHarnDP);
        //    }
        //    else
        //        MessageBox.Show("Ошибка во временном шаге");
        //}

        //private void OpenHarmWin(object sender, RoutedEventArgs e)
        //{
        //    if (int.TryParse(TextBox_paramHarmN.Text, out int n) && n > 0)
        //    {
        //        harmParamWindow = new HarmParamWindow(n, data);
        //        harmParamWindow.ShowDialog();
        //    }
        //    else
        //        MessageBox.Show("Количество параметров для гармоник не верно");
        //}

        //private static void Stat(Analysis analysis, params TextBlock[] p)
        //{
        //    p[0].Text = analysis.min.ToString("0.000000");
        //    p[1].Text = analysis.max.ToString("0.000000");
        //    p[2].Text = analysis.M.ToString("0.000000");
        //    p[3].Text = analysis.D.ToString("0.000000");
        //    p[4].Text = analysis.eps.ToString("0.000000");
        //    p[5].Text = analysis.gamma1.ToString("0.000000");
        //    p[6].Text = analysis.gamma2.ToString("0.000000");
        //    p[7].Text = (analysis.stationarity) ? "Да" : "Нет";
        //}

        //private void ShowAnalysis(object sender, RoutedEventArgs e)
        //{
        //    if (int.TryParse(TextBox_ParamM.Text, out int M))
        //    {
        //        if (data.positiveLinearDP.Count != 0)
        //        {
        //            analysisT1 = new Analysis(data.positiveLinearDP, data.N, M);
        //            Stat(analysisT1, min_T1, max_T1, M_T1, D_T1, SKO_T1, As_T1, K_T1, S_T1);
        //        }
        //        if (data.negativeLinearDP.Count != 0)
        //        {
        //            analysisT2 = new Analysis(data.negativeLinearDP, data.N, M);
        //            Stat(analysisT2, min_T2, max_T2, M_T2, D_T2, SKO_T2, As_T2, K_T2, S_T2);
        //        }
        //        if (data.positiveExpDP.Count != 0)
        //        {
        //            analysisT3 = new Analysis(data.positiveExpDP, data.N, M);
        //            Stat(analysisT3, min_T3, max_T3, M_T3, D_T3, SKO_T3, As_T3, K_T3, S_T3);
        //        }
        //        if (data.negativeExpDP.Count != 0)
        //        {
        //            analysisT4 = new Analysis(data.negativeExpDP, data.N, M);
        //            Stat(analysisT4, min_T4, max_T4, M_T4, D_T4, SKO_T4, As_T4, K_T4, S_T4);
        //        }
        //        if (data.randomNoiseDP.Count != 0)
        //        {
        //            analysisN1 = new Analysis(data.randomNoiseDP, data.N, M);
        //            Stat(analysisN1, min_N1, max_N1, M_N1, D_N1, SKO_N1, As_N1, K_N1, S_N1);
        //        }
        //        if (data.myrandomNoiseDP.Count != 0)
        //        {
        //            analysisN2 = new Analysis(data.myrandomNoiseDP, data.N, M);
        //            Stat(analysisN2, min_N2, max_N2, M_N2, D_N2, SKO_N2, As_N2, K_N2, S_N2);
        //        }
        //        if (data.impulseNoiseDP.Count != 0)
        //        {
        //            analysisIN = new Analysis(data.impulseNoiseDP, data.N, M);
        //            Stat(analysisIN, min_IN, max_IN, M_IN, D_IN, SKO_IN, As_IN, K_IN, S_IN);
        //        }
        //        if (data.harmDP.Count != 0)
        //        {
        //            analysisH = new Analysis(data.harmDP, data.N, M);
        //            Stat(analysisH, min_H, max_H, M_H, D_H, SKO_H, As_H, K_H, S_H);
        //        }
        //        if (data.polyHarnDP.Count != 0)
        //        {
        //            analysisPH = new Analysis(data.polyHarnDP, data.N, M);
        //            Stat(analysisPH, min_PH, max_PH, M_PH, D_PH, SKO_PH, As_PH, K_PH, S_PH);
        //        }
        //        if (data.implementNoiseDP.Count != 0 && data.implementNoiseWithHarmDP.Count != 0)
        //        {
        //            analysisIP = new Analysis(data.implementNoiseDP, data.N, M);
        //            Stat(analysisIP, min_1, max_1, M_1, D_1, SKO_1, As_1, K_1, S_1);
        //            analysisIPH = new Analysis(data.implementNoiseWithHarmDP, data.N, M);
        //            Stat(analysisIPH, min_2, max_2, M_2, D_2, SKO_2, As_2, K_2, S_2);
        //            data.implementNoiseMDependenceDP.Add(new DataPoint(int.Parse(TextBox_M.Text), analysisIP.beta));
        //        }
        //        HistButton.IsEnabled = true;
        //    }
        //    else
        //        MessageBox.Show("Ошибка в числе разбиений графика для анализа (М)");
        //}

        //private void HideAnalysis(object sender, RoutedEventArgs e)
        //{
        //    if (!hideAnalysis)
        //    {
        //        Anal1.Width = new GridLength(width);
        //        Anal1.MaxWidth = width;
        //        Anal2.Width = new GridLength(width);
        //        Anal2.MaxWidth = width;
        //        Anal3.Width = new GridLength(width);
        //        Anal3.MaxWidth = width;
        //        Anal4.Width = new GridLength(width);
        //        Anal4.MaxWidth = width;
        //        Anal5.Width = new GridLength(width);
        //        Anal5.MaxWidth = width;
        //        Anal6.Width = new GridLength(width);
        //        Anal6.MaxWidth = width;
        //        Anal7.Width = new GridLength(width);
        //        Anal7.MaxWidth = width;
        //        Anal8.Width = new GridLength(width);
        //        Anal8.MaxWidth = width;
        //        Anal9.Width = new GridLength(width);
        //        Anal9.MaxWidth = width;

        //        hideAnalysis = !hideAnalysis;
        //    }
        //    else
        //    {
        //        Anal1.Width = new GridLength(0);
        //        Anal2.Width = new GridLength(0);
        //        Anal3.Width = new GridLength(0);
        //        Anal4.Width = new GridLength(0);
        //        Anal5.Width = new GridLength(0);
        //        Anal6.Width = new GridLength(0);
        //        Anal7.Width = new GridLength(0);
        //        Anal8.Width = new GridLength(0);
        //        Anal9.Width = new GridLength(0);

        //        hideAnalysis = !hideAnalysis;
        //    }
        //}

        //private void ShiftClick(object sender, RoutedEventArgs e)
        //{
        //    if (double.TryParse(TextBox_ParamC.Text.Replace(".", ","), out data.S))
        //    {
        //        Shift(data.S, ref data.positiveLinearDP);
        //        Shift(data.S, ref data.negativeLinearDP);
        //        Shift(data.S, ref data.positiveExpDP);
        //        Shift(data.S, ref data.negativeExpDP);
        //        Shift(data.S, ref data.randomNoiseDP);
        //        Shift(data.S, ref data.myrandomNoiseDP);
        //        Shift(data.S, ref data.implementNoiseDP);
        //        Shift(data.S, ref data.harmDP);
        //        Shift(data.S, ref data.polyHarnDP);

        //        PosLinTrendPlot.Model = BuildModel("Linear ax+b", "ax+b", data.positiveLinearDP);
        //        NegLinTrendPlot.Model = BuildModel("Linear -ax+b", "-ax+b", data.negativeLinearDP);
        //        PosExpTrendPlot.Model = BuildModel("Exp b*exp(-a)", "b*exp(-a)", data.positiveExpDP);
        //        NegExpTrendPlot.Model = BuildModel("Exp -b*exp(-a)", "-b*exp(-a)", data.negativeExpDP);
        //        RandNoisePlot.Model = BuildModel("Random Noise", "", data.randomNoiseDP);
        //        MyRandNoisePlot.Model = BuildModel("MyRandom Noise", "", data.myrandomNoiseDP);
        //        ImpulseNoisePlot.Model = BuildModel("Impulse Noise", "", data.impulseNoiseDP);
        //        HarmPlot.Model = BuildModel("Harmonic", "", data.harmDP);
        //        PolyHarmPlot.Model = BuildModel("PolyHarmonic", "", data.polyHarnDP);
        //    }
        //    else
        //        MessageBox.Show("Ошибка в линейных параметрах");
        //}

        //private void AntiShiftClick(object sender, RoutedEventArgs e)
        //{
        //    Processing.AntiShift(ref data.positiveLinearDP, analysisT1.M);
        //    PosLinTrendPlot.Model = BuildModel("Linear ax+b", "ax+b", data.positiveLinearDP);
        //    Processing.AntiShift(ref data.negativeLinearDP, analysisT2.M);
        //    NegLinTrendPlot.Model = BuildModel("Linear -ax+b", "-ax+b", data.negativeLinearDP);
        //    Processing.AntiShift(ref data.positiveExpDP, analysisT3.M);
        //    PosExpTrendPlot.Model = BuildModel("Exp b*exp(-a)", "b*exp(-a)", data.positiveExpDP);
        //    Processing.AntiShift(ref data.negativeExpDP, analysisT4.M);
        //    NegExpTrendPlot.Model = BuildModel("Exp -b*exp(-a)", "-b*exp(-a)", data.negativeExpDP);
        //}

        //private void HistClick(object sender, RoutedEventArgs e)
        //{
        //    TrendHist1.Model = analysisT1.Hist("ax+b Hist");
        //    TrendHist2.Model = analysisT2.Hist("-ax+b Hist");
        //    TrendHist3.Model = analysisT3.Hist("b*exp(-a) 3 Hist");
        //    TrendHist4.Model = analysisT4.Hist("-b*exp(-a) Hist");
        //    NoiseHist1.Model = analysisN1.Hist("Random Noise Hist");
        //    NoiseHist2.Model = analysisN2.Hist("MyRandom Noise Hist");
        //    ImpulseNoiseHist.Model = analysisIN.Hist("Impulse Noise Hist");
        //    HarmHist.Model = analysisH.Hist("Harm Hist");
        //    PolyHarmHist.Model = analysisPH.Hist("Poly Harm Hist");
        //}

        //private void ACFClick(object sender, RoutedEventArgs e)
        //{
        //    ACFHarmPlot.Model = BuildModel("Harmonic ACF", "", analysisH.ACF());
        //    ACFNoisePlot.Model = BuildModel("Noise ACF", "", analysisN1.ACF());
        //}

        //private void SaveDataClick(object sender, RoutedEventArgs e)
        //{
        //    dataX1 = analysisN1.GetXk();
        //    dataX2 = analysisN2.GetXk();
        //    dataX3 = analysisH.GetXk();
        //}

        //private void CCFClick(object sender, RoutedEventArgs e)
        //{
        //    CCFNoisePlot.Model = BuildModel("Noise CCF", "", analysisN1.CCF(dataX1));
        //    CCFMyNoisePlot.Model = BuildModel("MyNoise CCF", "", analysisN2.CCF(dataX2));
        //    CCFHarmPlot.Model = BuildModel("Harmonic CCF", "", analysisH.CCF(dataX3));
        //}

        //private void BuildSpikes(object sender, RoutedEventArgs e)
        //{
        //    data.noiseAndImpulseDP = DataPointOperations.SumPoints(data.randomNoiseDP, data.impulseNoiseDP);
        //    data.harmAndImpulseDP = DataPointOperations.SumPoints(data.harmDP, data.impulseNoiseDP);
        //    SpikePlot1.Model = BuildModel("Noise + Impulse", "", data.noiseAndImpulseDP);
        //    SpikePlot2.Model = BuildModel("Harm + Impulse", "", data.harmAndImpulseDP);
        //}

        //private void Filtration(object sender, RoutedEventArgs e)
        //{
        //    Processing.AntiSpike(data.R, ref data.noiseAndImpulseDP);
        //    Processing.AntiSpike(data.R, ref data.harmAndImpulseDP);
        //    SpikePlot1.Model = BuildModel("Noise + Impulse", "", data.noiseAndImpulseDP);
        //    SpikePlot2.Model = BuildModel("Harm + Impulse", "", data.harmAndImpulseDP);
        //}

        //private void BuildCombinedTrend(object sender, RoutedEventArgs e)
        //{
        //    data.linearTrendAndHarmDP = DataPointOperations.SumPoints(data.positiveLinearDP, data.harmDP);
        //    data.negativeExpAndNoiseDP = DataPointOperations.SumPoints(data.positiveExpDP, data.randomNoiseDP);
        //    AntiTrendPlot1.Model = BuildModel("Lin + Harm", ConvertToLineSeries(data.linearTrendAndHarmDP, "Lin&Harm"),
        //        ConvertToLineSeries(data.positiveLinearDP, "Linear"), ConvertToLineSeries(data.harmDP, "Harmonic"));
        //    AntiTrendPlot2.Model = BuildModel("Exp + Noise", ConvertToLineSeries(data.negativeExpAndNoiseDP, "Exp&Noise"),
        //        ConvertToLineSeries(data.positiveExpDP, "Exp"), ConvertToLineSeries(data.randomNoiseDP, "noise"));
        //}

        //private void DeleteLineareTrend(object sender, RoutedEventArgs e)
        //{
        //    Processing.AntiTrendLinear(ref data.linearTrendAndHarmDP);
        //    AntiTrendPlot1.Model = BuildModel("Lin + Harm", ConvertToLineSeries(data.linearTrendAndHarmDP, "Lin&Harm"),
        //        ConvertToLineSeries(data.positiveLinearDP, "Linear"), ConvertToLineSeries(data.harmDP, "Harmonic"));
        //}

        //private void DeleteNonLineareTrend(object sender, RoutedEventArgs e)
        //{
        //    if (int.TryParse(TextBox_ParamMSpikes.Text.Replace(".", ","), out int M))
        //    {
        //        Processing.AntiTrendNonLinear(M, ref data.negativeExpAndNoiseDP);
        //        AntiTrendPlot2.Model = BuildModel("Exp + Noise", ConvertToLineSeries(data.negativeExpAndNoiseDP, "Exp&Noise"),
        //            ConvertToLineSeries(data.positiveExpDP, "Exp"), ConvertToLineSeries(data.randomNoiseDP, "noise"));
        //    }
        //    else
        //        MessageBox.Show("Ошибка в параметре удаления тренда");
        //}

        //private void FourierClick(object sender, RoutedEventArgs e)
        //{
        //    if (int.TryParse(TextBox_ParamLFourier.Text.Replace(".", ","), out int L))
        //    {
        //        FourierPlot1.Model = BuildModel("Test", "", analysisH.SpectrFourier(data.N / 2, L));
        //        FourierPlot2.Model = BuildModel("Test", "", analysisPH.SpectrFourier(data.N / 2, L));
        //    }
        //    else
        //        MessageBox.Show("Ошибка в параметре Фурье");
        //}

        //private void TextBox_M_TextChanged(object sender, TextChangedEventArgs e)
        //    => ImpButton.IsEnabled = false;

        //private void BuildImplementsClick(object sender, RoutedEventArgs e)
        //{
        //    if (int.TryParse(TextBox_M.Text, out int M))
        //    {
        //        data.implementNoiseDP = Processing.ImplementationMNoise(M, data.N);
        //        ImplementPlot1.Model = BuildModel("Test", "", data.implementNoiseDP);
        //        data.implementNoiseWithHarmDP = Processing.ImplementationMNoise(M, data.N, data.harmDP, 30);
        //        ImplementPlot2.Model = BuildModel("Test", "", data.implementNoiseWithHarmDP);
        //        ImpButton.IsEnabled = true;
        //    }
        //    else
        //        MessageBox.Show("Ошибка в параметре Фурье");
        //}

        //private void ImplementsAverageClick(object sender, RoutedEventArgs e)
        //{
        //    int M = int.Parse(TextBox_M.Text);
        //    data.implementNoiseDP = Processing.AntiNoise(M, data.N, data.implementNoiseDP);
        //    ImplementPlot1.Model = BuildModel("Test", "", data.implementNoiseDP);
        //    data.implementNoiseWithHarmDP = Processing.AntiNoise(M, data.N, data.implementNoiseWithHarmDP);
        //    ImplementPlot2.Model = BuildModel("Test", "", data.implementNoiseWithHarmDP);
        //}

        //private void ImplementMDependenceClick(object sender, RoutedEventArgs e)
        //    => ImplementPlot3.Model = BuildModel("Test", "", data.implementNoiseMDependenceDP);

        //private void Click11_3_a(object sender, RoutedEventArgs e)
        //{
        //    List<DataPoint> h1 = HarmTrend(data.N, 1, 42, 0.005);
        //    List<DataPoint> h2 = ExpTrend(data.N, 30, 1, 0.005);
        //    data.cardio_htDP = DataPointOperations.MultPoints(h1, h2);
        //    DataPointOperations.Normalized(ref data.cardio_htDP);
        //    DataPointOperations.MultPointsWithVar(ref data.cardio_htDP, 120);
        //    LinearHeartPlot.Model = BuildModel("Linear Heart", ConvertToLineSeries(h1, "Harm"),
        //        ConvertToLineSeries(h2, "Exp"), ConvertToLineSeries(data.cardio_htDP, "Heart"));
        //}

        //private void Click11_3_b(object sender, RoutedEventArgs e)
        //{
        //    data.cardio_xtDP = ImpulseT_NoiseTrend(data.N, 200, 1, true, 0.005);
        //    HeartImpulsePlot.Model = BuildModel("4 Impulse", "", data.cardio_xtDP);
        //}

        //private void Click11_3_c(object sender, RoutedEventArgs e)
        //{
        //    double delta_t = 0.005;
        //    int M = 200;
        //    int N = data.N;
        //    data.cardio_ytDP = new(N + M);
        //    data.cardio_ytDP = DataPointOperations.Convol(N, M, data.cardio_htDP, data.cardio_xtDP, delta_t);
        //    CardiogramaPlot.Model = BuildModel("Cardiograma", "", data.cardio_ytDP);
        //}

        //List<DataPoint> t, t1, t2, t3;
        //readonly int m = 64;

        //private void Click12_2(object sender, RoutedEventArgs e)
        //{
        //    double fc = 50, delta_t = 0.002, fc1 = 35, fc2 = 75;
        //    int m = 64;
        //    List<double> lpw = new(2 * m + 1), hpw = new(2 * m + 1), bpw = new(2 * m + 1), bsw = new(2 * m + 1);
        //    Processing.LowPassFilter(fc, delta_t, m, ref lpw);
        //    Processing.HighPassFilter(fc, delta_t, m, ref hpw);
        //    Processing.StrokeFilter(fc1, fc2, delta_t, m, ref bpw);
        //    Processing.RejectFilter(fc1, fc2, delta_t, m, ref bsw);
        //    t = new List<DataPoint>(m + 1); t1 = new List<DataPoint>(m + 1); t2 = new List<DataPoint>(m + 1); t3 = new List<DataPoint>(m + 1);
        //    for (int i = 0; i <= 2 * m; i++)
        //    {
        //        t.Add(new DataPoint(i, lpw[i]));
        //        t1.Add(new DataPoint(i, hpw[i]));
        //        t2.Add(new DataPoint(i, bpw[i]));
        //        t3.Add(new DataPoint(i, bsw[i]));
        //    }
        //    f4.Model = BuildModel("Low Pass", "", t);
        //    f3.Model = BuildModel("High Pass", "", t1);
        //    f2.Model = BuildModel("Stroke", "", t2);
        //    f1.Model = BuildModel("Reject", "", t3);
        //}
        //Analysis a1, a2, a3, a4;
        //private void Click12_3(object sender, RoutedEventArgs e)
        //{
        //    a1 = new(t, 2 * m + 1, 10);
        //     a2 = new(t1, 2 * m + 1, 10);
        //     a3 = new(t2, 2 * m + 1, 10);
        //     a4 = new(t3, 2 * m + 1, 10);

        //    ff4.Model = BuildModel("Low Pass spect", "", a1.SpectrFourier(m, 0, 0.002));
        //    ff3.Model = BuildModel("high Pass spect", "", a2.SpectrFourier(m, 0, 0.002));
        //    ff2.Model = BuildModel("stroke spect", "", a3.SpectrFourier(m, 0, 0.002));
        //    ff1.Model = BuildModel("rej spect", "", a4.SpectrFourier(m, 0, 0.002));
        //}

        //private void FileClick(object sender, RoutedEventArgs e)
        //{
        //    data.fileDP = InOut.Read(out int rate, 0.002);
        //    Analysis a = new(data.fileDP, 1000, 10);


        //    FileSpectrPlot.Model = BuildModel("Plot FROM file", "", a.SpectrFourier(500, 0, 0.002));
        //    FilePlot.Model = BuildModel("file", rate.ToString(), data.fileDP);
        //    //InOut.Write(data.fileDP);
        //}

        //private void Click13(object sender, RoutedEventArgs e)
        //{
        //    //List<DataPoint> points = data.fileDP;
        //    //DataPointOperations.Convol(1000, 64, t, data.fileDP);
        //    List<DataPoint> t, t1, t2, t3;
        //    int m = 64;
        //    double delta_t = 0.002;
        //    List<double> lpw = new(2 * m + 1), hpw = new(2 * m + 1), bpw = new(2 * m + 1), bsw = new(2 * m + 1);
        //    Processing.LowPassFilter(15, delta_t, m, ref lpw);
        //    Processing.HighPassFilter(100, delta_t, m, ref hpw);
        //    Processing.StrokeFilter(10, 100, delta_t, m, ref bpw);
        //    Processing.RejectFilter(10, 100, delta_t, m, ref bsw);
        //    t = new List<DataPoint>(2*m + 1); t1 = new List<DataPoint>(2*m + 1); t2 = new List<DataPoint>(2*m + 1); t3 = new List<DataPoint>(2*m + 1);
        //    for (int i = 0; i <= 2 * m; i++)
        //    {
        //        t.Add(new DataPoint(i*delta_t, lpw[i]));
        //        t1.Add(new DataPoint(i*delta_t, hpw[i]));
        //        t2.Add(new DataPoint(i*delta_t, bpw[i]));
        //        t3.Add(new DataPoint(i*delta_t, bsw[i]));
        //    }

        //    f4.Model = BuildModel("Low Pass", "", t);
        //    f3.Model = BuildModel("High Pass", "", t1);
        //    f2.Model = BuildModel("Stroke", "", t2);
        //    f1.Model = BuildModel("Reject", "", t3);

        //    Analysis a11 = new(t, 2*m+1);
        //    Analysis a22 = new(t1, 2 * m + 1);
        //    Analysis a33 = new(t2, 2 * m + 1);
        //    Analysis a44 = new(t3, 2 * m + 1);

        //    ff4.Model = BuildModel("Low Pass spect", "", a11.SpectrFourier(m, 0, 0.002));
        //    ff3.Model = BuildModel("high Pass spect", "", a22.SpectrFourier(m, 0, 0.002));
        //    ff2.Model = BuildModel("stroke spect", "", a33.SpectrFourier(m, 0, 0.002));
        //    ff1.Model = BuildModel("rej spect", "", a44.SpectrFourier(m, 0, 0.002));

        //    List<DataPoint> points1 = DataPointOperations.Convol(1000, 2 * m + 1, t, data.fileDP, 0.002);
        //    List<DataPoint> points2 = DataPointOperations.Convol(1000, 2 * m + 1, t1, data.fileDP, 0.002);
        //    List<DataPoint> points3 = DataPointOperations.Convol(1000, 2 * m + 1, t2, data.fileDP, 0.002);
        //    List<DataPoint> points4 = DataPointOperations.Convol(1000, 2 * m + 1, t3, data.fileDP, 0.002);

        //    //points1.RemoveRange(0, 128);
        //    //points2.RemoveRange(0, 128);
        //    //points3.RemoveRange(0, 128);
        //    //points4.RemoveRange(0, 128);

        //    fff4.Model = BuildModel("Low Pass ", "", points1);
        //    fff3.Model = BuildModel("high Pass ", "", points2);
        //    fff2.Model = BuildModel("stroke ", "", points3);
        //    fff1.Model = BuildModel("reject ", "", points4);

        //    a11 = new(points1, 1000/* - 128*/);
        //    a22 = new(points2, 1000/* - 128*/);
        //    a33 = new(points3, 1000/* - 128*/);
        //    a44 = new(points4, 1000/* - 128*/);


        //    ffff4.Model = BuildModel("Low Pass (only low frequency)", "", a11.SpectrFourier( /*(1000 - 128 ) / 2*/500, 0, 0.002));
        //    ffff3.Model = BuildModel("high Pass (only high frequency)", "", a22.SpectrFourier(/*(1000 - 128) / 2*/500, 0, 0.002));
        //    ffff2.Model = BuildModel("stroke (only middle frequency)", "", a33.SpectrFourier( /*(1000 - 128) / 2*/500, 0, 0.002));
        //    ffff1.Model = BuildModel("reject (low and high frequency)", "", a44.SpectrFourier(/*(1000 - 128) / 2*/500, 0, 0.002));
        //}
    }
}

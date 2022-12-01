using System.Collections.Generic;
using XDPM_App.Common;
using OxyPlot;

namespace XDPM_App.ADMP
{
    public class Data
    {
        public int N;
        public int IN_N; //кол-во импульсов
        public double R; //шум
        public double a;
        public double b;
        public double delta_t;
        public double S; // смещение
        //public double S; // смещение
        //public double S; // смещение
        //public double S; // смещение
        //public double S; // смещение
        //public double S; // смещение
        //public double S; // смещение
        //public double S; // смещение
        //public double S; // смещение
        public List<DataPoint> positiveLinearDP;
        public List<DataPoint> negativeLinearDP;
        public List<DataPoint> positiveExpDP;
        public List<DataPoint> negativeExpDP;
        public List<DataPoint> randomNoiseDP;
        public List<DataPoint> myrandomNoiseDP;
        public List<DataPoint> impulseNoiseDP;
        public List<DataPoint> harmDP;
        public List<DataPoint> polyHarnDP;
        public List<DataPoint> noiseAndImpulseDP;
        public List<DataPoint> harmAndImpulseDP;
        public List<DataPoint> linearTrendAndHarmDP;
        public List<DataPoint> negativeExpAndNoiseDP;
        //public List<DataPoint> fourier;   //////
        //public List<DataPoint> fourierd; //////
        public List<DataPoint> implementNoiseDP;
        public List<DataPoint> implementNoiseWithHarmDP;
        public List<DataPoint> implementNoiseMDependenceDP;
        public List<DataPoint> cardio_htDP;
        public List<DataPoint> cardio_xtDP;
        public List<DataPoint> cardio_ytDP;
        public List<DataPoint> fileDP;
        public List<HarmParam> HarmParams { get; set; } = null!;

        public Data()
        {
            positiveLinearDP = new List<DataPoint>();
            negativeLinearDP = new List<DataPoint>();
            positiveExpDP = new List<DataPoint>();
            negativeExpDP = new List<DataPoint>();
            randomNoiseDP = new List<DataPoint>();
            myrandomNoiseDP = new List<DataPoint>();
            impulseNoiseDP = new List<DataPoint>();
            harmDP = new List<DataPoint>();
            polyHarnDP = new List<DataPoint>();
            noiseAndImpulseDP = new List<DataPoint>();
            harmAndImpulseDP = new List<DataPoint>();
            linearTrendAndHarmDP = new List<DataPoint>();
            negativeExpAndNoiseDP = new List<DataPoint>();
            implementNoiseDP = new List<DataPoint>();
            implementNoiseWithHarmDP = new List<DataPoint>();
            implementNoiseMDependenceDP = new List<DataPoint>();
            cardio_htDP = new List<DataPoint>();
            cardio_xtDP = new List<DataPoint>();
            cardio_ytDP = new List<DataPoint>();
            fileDP = new List<DataPoint>();
        }
    }
}

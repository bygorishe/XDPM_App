using System.Collections.Generic;
using XDPM_App.Common;
using OxyPlot;
using System;

namespace XDPM_App.ADMP
{
    public class Data
    {
        private static readonly List<string> s_names = new();
        private static int s_count = 0;
        public string Name { get; private set; } = null!;
        public int N;
        public double deltaT = 0.001;
        public List<DataPoint> DataPoints = null!;

        public Data(int N, string Name)
        {
            if (Name != null && !s_names.Contains(Name))
            {
                this.N = N;
                this.Name = Name;
                s_names.Add(Name);
                s_count++;
                DataPoints = new(N);
            }
            else
                throw new Exception("Incorrect data");
        }

        public Data(int N) : this(N, (++s_count).ToString())
        { }
    }

    public class SimpleTrendData : Data
    {
        public double a;
        public double b;
        public SimpleTrendData(int N, string Name, double a, double b) : base(N, Name)
        {
            this.a = a;
            this.b = b;
        }
        public SimpleTrendData(int N, double a, double b) : base(N)
        {
            this.a = a;
            this.b = b;
        }
    }

    public class NoiseData : Data
    {
        public int IN_N; //кол-во импульсов
        public double R; //аплитуда шум
        public NoiseData(int N, string Name, double r, int iN_N = 0) : base(N, Name)
        {
            IN_N = iN_N;
            R = r;
        }
        public NoiseData(int N, double r, int iN_N = 0) : base(N)
        {
            IN_N = iN_N;
            R = r;
        }
    }

    public class HarmonicData : Data
    {
        public List<HarmParam> Param;
        public HarmonicData(int N, string Name, params HarmParam[] param) : base(N, Name)
        {
            Param = new List<HarmParam>(param.Length);
            foreach (var p in param)
                Param.Add(p);
        }
        public HarmonicData(int N, params HarmParam[] param) : base(N)
        {
            Param = new List<HarmParam>(param.Length);
            foreach (var p in param)
                Param.Add(p);
        }
    }

    public class WavData : Data
    {
        public int Rate;
        public WavData(int N, string Name) : base(N, Name)
        {

        }
        public WavData(int N) : base(N)
        {

        }
    }
}

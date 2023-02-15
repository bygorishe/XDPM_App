using System;

namespace FunctionalLibrary.Helpers.Exceptions
{
    public class WrongDataException : Exception
    {
        public string? Model { get; set; }
        public override string Message => $"Wrond data type. You need {Model} data";
    }

    public class ImageWrongDataException : WrongDataException
    {
        public ImageWrongDataException()
        {
            Model = "Image";
        }
    }

    public class NoiseWrongDataException : WrongDataException
    {
        public NoiseWrongDataException()
        {
            Model = "Noise";
        }
    }
    public class HarmWrongDataException : WrongDataException
    {
        public HarmWrongDataException()
        {
            Model = "Harmonic";
        }
    }
    public class WavWrongDataException : WrongDataException
    {
        public WavWrongDataException()
        {
            Model = "Wav";
        }
    }
    public class TrendWrongDataException : WrongDataException
    {
        public TrendWrongDataException()
        {
            Model = "Simple Trend";
        }
    }
    public class JustWrongDataException : WrongDataException
    {
        public JustWrongDataException()
        {
            Model = "just";
        }
    }
}

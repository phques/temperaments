using System;
using ScaleLib;

namespace Partch
{
    public class Partch : Scale
    {
        protected double[] intervalRatios = {
            1 / 1,
            81.0 / 80 ,
            33.0 / 32 ,
            21.0 / 20 ,
            16.0 / 15,
            12.0 / 11 ,
            11.0 / 10 ,
            10.0 / 9 ,
            9.0 / 8 ,
            8.0 / 7 ,
            7.0 / 6 ,
            32.0 / 27,
            6.0 / 5 ,
            11.0 / 9 ,
            5.0 / 4 ,
            14.0 / 11 ,
            9.0 / 7 ,
            21.0 / 16 ,
            4.0 / 3 ,
            27.0 / 20 ,
            11.0 / 8 ,
            7.0 / 5,
            10.0 / 7 ,
            16.0 / 11 ,
            40.0 / 27 ,
            3.0 / 2 ,
            32.0 / 21 ,
            14.0 / 9 ,
            11.0 / 7 ,
            8.0 / 5 ,
            18.0 / 11 ,
            5.0 / 3 ,
            27.0 / 16,
            12.0 / 7 ,
            7.0 / 4 ,
            16.0 / 9 ,
            9.0 / 5 ,
            20.0 / 11 ,
            11.0 / 6 ,
            15.0 / 8 ,
            40.0 / 21 ,
            64.0 / 33 ,
            160.0 / 81 ,
            2.0 / 1 ,
        };

        public Partch() : base(43)
        {
        }

        public override void Generate()
        {
            for (int i = 0; i < intervalRatios.Length; i++)
            {
                double cents = IntervalRatio.CentsFromRatio(intervalRatios[i]);
                scaleSteps[i] = new ScaleStep(i, cents, 0);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Partch partch = new Partch();
            partch.Generate();
            partch.PlaceRatios();
            partch.Show();
        }
    }
}

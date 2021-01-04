using System;
using ScaleLib;

namespace EDOgen
{
    // Generates values fro 'EDO' (equaly divided octave) scales
    // ie 19-EDO is a 19 notes scale
    class EDO : Scale
    {
        private readonly double refFrequency = 0;

        public EDO(int nbSteps, double refFrequency) : base(nbSteps)
        {
            this.refFrequency = refFrequency;
        }

        public override void Generate()
        {

            double a = Math.Pow(2, 1.0 / nbSteps);

            for (int i = 0; i <= nbSteps; i++)
            {
                double freq = refFrequency * Math.Pow(a, i);
                double cents = 1200.0 / nbSteps * i;

                scaleSteps[i] = new ScaleStep(i, cents, freq);
            }
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1 || !int.TryParse(args[0], out int nbSteps))
            {
                Console.WriteLine("paramter: nbr of steps of the EDO scale to generate");
                return;
            }


            Console.WriteLine("generating EDO {0} @ C4", nbSteps);

            const double C4 = 248.30;
            EDO edo = new EDO(nbSteps, C4);

            edo.Generate();
            edo.PlaceRatios();
            edo.Show();
        }
    }
}

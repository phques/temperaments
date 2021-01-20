using System;
using ScaleLib;

namespace Orwell
{
    class Orwell : Scale
    {
        public Orwell(int nbSteps) : base(nbSteps)
        {
        }

        protected double[] GenerateEDOSteps(int nbEDOsteps)
        {

            var edoSteps = new double[nbEDOsteps + 1];
            for (int i = 0; i <= nbEDOsteps; i++)
            {
                double cents = 1200.0 / nbEDOsteps * i;
                edoSteps[i] = cents;
            }

            return edoSteps;
        }

        public void GenerateFromEDO(int edoNbSteps, int generatorNbSteps)
        {
            var edoSteps = GenerateEDOSteps(edoNbSteps);
            double[] orwellCents = new double[nbSteps];

            int genIdx = generatorNbSteps; // start here

            scaleSteps[0] = new ScaleStep(0, 0, 0);
            for (int i = 1; i < nbSteps; i++)
            {
                orwellCents[i]= edoSteps[genIdx];

                // go to next generator in EDO scale
                genIdx = (genIdx + generatorNbSteps) % edoNbSteps;
            }

            GenerateSteps(orwellCents);

        }

        // generate from "perfect 12th" / 7
        public override void Generate()
        {
            // create the generator (P12 = P5 @ octave)
            // get cents for P5 and add an octave (1200)
            var p5 = new IntervalRatio(3, 2, "P5", "G");
            double genCents = (p5.Cents + 1200) / 7;
            double cents = 0;
            double[] orwellCents = new double[nbSteps];

            for (int i = 0; i < nbSteps; i++)
            {
                orwellCents[i] = cents;

                cents += genCents;
                if (cents > 1200)
                    cents -= 1200;
            }
            GenerateSteps(orwellCents);
        }

        protected void GenerateSteps(double[] orwellCents)
        {
            Array.Sort(orwellCents);
            for (int i = 0; i < nbSteps; i++)
            {
                scaleSteps[i] = new ScaleStep(i, orwellCents[i], 0);
            }
            scaleSteps[nbSteps] = new ScaleStep(nbSteps, 1200, 0);
        }

        static public void ShowUsage()
        {
            Console.WriteLine("Usage: nbSteps (9|13) method");
            Console.WriteLine(" method =");
            Console.WriteLine("  calc (3/1 div 7)");
            Console.WriteLine("  7-31 (generator is 7 steps of 31-EDO)");
            Console.WriteLine("  12-53 (generator is 12 steps of 53-EDO)");
            Console.WriteLine("  19-84 (generator is 19 steps of 84-EDO)");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int generatorNbSteps = 0;
            int edoNbSteps = 0;
            

            if (args.Length != 2)
            {
                Orwell.ShowUsage();
                return;
            }
            
            if (!int.TryParse(args[0], out int nbSteps))
            {
                Orwell.ShowUsage();
                return;
            }

            Orwell orwell = new Orwell(nbSteps);

            switch (args[1])
            {
                case "7-31":
                    generatorNbSteps = 7;
                    edoNbSteps = 31;
                    break;

                case "12-53":
                    generatorNbSteps = 12;
                    edoNbSteps = 53;
                    break;

                case "19-84":
                    generatorNbSteps = 19;
                    edoNbSteps = 84;
                    break;

                case "calc":
                    break; // nbsteps=0 indicates we calculate generator

                default:
                    Orwell.ShowUsage();
                    return;
            }

            if (edoNbSteps != 0)
                orwell.GenerateFromEDO(edoNbSteps, generatorNbSteps);
            else
                orwell.Generate();

            orwell.PlaceRatios();
            orwell.Show();
        }
    }
}

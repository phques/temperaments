
// Copyright 2021 Philippe Quesnel  
//
// This file is part of Temperaments.
//
// Temperaments is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Temperaments is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Temperaments.  If not, see <http://www.gnu.org/licenses/>.
using System;
using ScaleLib;

namespace CarlosScales
{
    // generates values for Wendy Carlos alpha, beta, gamma scales
    public class Carlos : Scale
    {
        protected double stepCents;

        // https://en.wikipedia.org/wiki/Alpha_scale
        // Carlos' α (alpha) scale arises from...taking a value for the scale degree so that
        // nine of them approximate a 3:2 perfect fifth,
        // five of them approximate a 5:4 major third,
        // and four of them approximate a 6:5 minor third.
        // In order to make the approximation as good as possible we minimize the mean square deviation.
        // alpha=(9,5,4), beta=(11,6,5), gamma=(20,11,9)
        public Carlos(int nb5Ths, int nbMaj3Thds, int nbMin3Thds, int nbSteps) : base(nbSteps)
        {
            CalcCents(nb5Ths, nbMaj3Thds, nbMin3Thds);
            Name = "Wendy Carlos scale";
        }

        protected void CalcCents(int nb5Ths, int nbMaj3Thds, int nbMin3Thds)
        {
            double a = (
                nb5Ths * Math.Log(3.0 / 2, 2) +
                nbMaj3Thds * Math.Log(5.0 / 4, 2) +
                nbMin3Thds * Math.Log(6.0 / 5, 2));

            double b = (nb5Ths * nb5Ths) + (nbMaj3Thds * nbMaj3Thds) + (nbMin3Thds * nbMin3Thds);

            stepCents = 1200 * (a / b);
        }


        public override void Generate()
        {
            for (int i = 0; i <= nbSteps; i++)
            {
                scaleSteps[i] = new ScaleStep(i, stepCents * i, 0); // no frequency
            }
        }

        public string Name { get; protected set; }

    }

    class Alpha : Carlos
    {
        public Alpha() : base(9, 5, 4, 15+2) // 15.3915 per octave
        {
            Name = "Wendy Carlos scale Alpha";
        }
    }

    class Beta : Carlos
    {
        public Beta() : base(11, 6, 5, 18+2) // 18.8 steps per octave
        {
            Name = "Wendy Carlos scale Beta";
        }
    }

    class Gamma : Carlos
    {
        public Gamma() : base(20, 11, 9, 34+2) // 34.1895 per octave.
        {
            Name = "Wendy Carlos scale Gamma";
        }
    }

    // is this one Carlos ?
    // wikipedia speaks of "The Bohlen–Pierce delta scale is based on the tritave"
    class Delta : Carlos
    {
        public Delta() : base(50, 28,31, 91+2) //
        {
            Name = "Wendy Carlos scale Delta";
        }
    }

    // trying out my own ;-)
    class Pq53EDO : Carlos
    {
        // 53 is 1200.27 !! 0.27 sharp of P8 !
        // This ends up being *almost* exactly 53-EDO !!
        // 
        public Pq53EDO() : base(31, 17, 14, 53+2) 
        {
            Name = "Wendy Carlos scale Pq53EDO";
        }
    }

    // trying out my own ;-)
    class Pq41EDO : Carlos
    {
        // 41 is 1199.75 !! 0.25 flat P8 !
        // This ends up being *almost* exactly 41-EDO !!
        // 
        public Pq41EDO() : base(24, 13, 11, 41 + 2)
        {
            Name = "Wendy Carlos scale Pq41EDO";
        }
    }

    // trying out my own ;-)
    class Pq65EDO : Carlos
    {
        // 65 is 1200.52 !! 0.52 sharp P8 !
        // This ends up being *almost* exactly 65-EDO !!
        // note that 65edo is close to 53edo
        // 
        public Pq65EDO() : base(38, 21, 17, 65+2)
        {
            Name = "Wendy Carlos scale Pq65EDO";
        }
    }

    // trying out my own ;-)
    class Pq2 : Carlos
    {
        public Pq2() : base(29, 16, 13, 100)
        {
            Name = "Wendy Carlos scale Pq2";
        }

        public override void Generate()
        {
            double f1 = 17.0 / 62 * 2;
            double f2 = 14.0 / 62 * 2;
            //double f1 = 11.0 / 20.0;
            //double f2 = 9.0 / 20;

            for (int i = 38; i <= 45; i++)
            {
                int n1 = i;
                int n2 = (int)Math.Round(i*f1);
                int n3 = (int)Math.Round(i*f2);
                CalcCents(n1, n2, n3);
                base.Generate();
                PlaceRatios();

            }
            
        }
    }

    class Gamma3va : Carlos
    {
        public Gamma3va() : base(20, 11, 9, 34*3-1) // 34.1895 per octave.
        {
            Name = "Wendy Carlos scale Gamma3va";
        }

        public override void Generate()
        {
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 34; i++)
                {
                    scaleSteps[i+34*j] = new ScaleStep(i + 34 * j, stepCents * i + 1200*j, 0); // no frequency
                }
            }  
        }

        public override void Show()
        {
            Console.WriteLine("Steps\tCents\t-- Just\tratio\tcents\terror --");

            for (int i = 0; i < scaleSteps.Length; i++)
            {
                var step = scaleSteps[i];

                //if (step != null)
                {
                    Console.Write("{0:D}\t", step.Index);
                    Console.Write("{0,7:F2}\t", step.Cents);

                    if (step.Ratio != null)
                    {
                        Console.Write("{0}\t", step.Ratio.Interval);
                        Console.Write("{0}\t", step.Ratio);
                        Console.Write("{0:F2}\t", step.Ratio.Cents);
                        Console.Write("{0:F2}\t", step.DiffJust);
                    }
                    else
                    {
                        Console.Write("\t\t\t\t");
                    }

                    Console.WriteLine("");
                }
            }
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            Carlos carlos = null;

            if (args.Length == 1)
            {
                if (args[0] == "a")
                {
                    carlos = new Alpha();
                }
                else if (args[0] == "b")
                {
                    carlos = new Beta();
                }
                else if (args[0] == "g")
                {
                    carlos = new Gamma();
                }
                else if (args[0] == "d")
                {
                    carlos = new Delta();
                }
                else if (args[0] == "pq41")
                {
                    carlos = new Pq41EDO();
                }
                else if (args[0] == "pq53")
                {
                    carlos = new Pq53EDO();
                }
                else if (args[0] == "pq65")
                {
                    carlos = new Pq65EDO();
                }
                else if (args[0] == "q")
                {
                    carlos = new Pq2();
                    carlos.Generate();
                    return;
                }
                else if (args[0] == "g3va")
                {
                    carlos = new Gamma3va();
                    carlos.Generate();
                    carlos.PlaceRatios();
                    carlos.Show();
                    return;
                }
            }
            if (carlos != null)
            {
                //Console.WriteLine("Generating {0}", carlos.Name);
                carlos.Generate();
                carlos.PlaceRatios();
                carlos.Show();
            }
            else
            {
                Console.WriteLine("Wendy Carlos scale generator, parameter: a | b | g | d | pq41 | pq53");
            }
        }
    }
}

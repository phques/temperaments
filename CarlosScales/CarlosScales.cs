
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
            double a = (
                nb5Ths * Math.Log(3.0 / 2, 2) +
                nbMaj3Thds * Math.Log(5.0 / 4, 2) +
                nbMin3Thds * Math.Log(6.0 / 5, 2));
            double b = (nb5Ths * nb5Ths) + (nbMaj3Thds * nbMaj3Thds) + (nbMin3Thds * nbMin3Thds);

            stepCents = 1200 * (a / b);
            Name = "Wendy Carlos scale";
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
            }
            if (carlos != null)
            {
                Console.WriteLine("GEnerating {0}", carlos.Name);
                carlos.Generate();
                carlos.PlaceRatios();
                carlos.Show();
            }
            else
            {
                Console.WriteLine("Wendy Carlos scale generator, parameter: a | b | g");
            }
        }
    }
}

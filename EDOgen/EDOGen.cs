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

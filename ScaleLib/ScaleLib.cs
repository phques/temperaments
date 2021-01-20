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
using System.Collections.Generic;

namespace ScaleLib
{
    public class IntervalRatio
    {
        private readonly int n1;
        private readonly int n2;
        private readonly double cents;
        private readonly string interval;
        private readonly string note;

        public IntervalRatio(int n1, int n2, string interval, string note)
        {
            this.n1 = n1;
            this.n2 = n2;
            this.interval = interval;
            this.note = note;
            this.cents = CentsFromRatio(n1, n2);
        }

        static public double CentsFromRatio(double n1, double n2)
        {
            return CentsFromRatio(n1 / n2);
        }

        static public double CentsFromRatio(double ratio)
        {
            return 1200 * Math.Log(ratio, 2);
        }

        override public string ToString()
        {
            return string.Format("{0}:{1}", n1, n2);
        }

        public double Cents => cents;
        public string Interval => interval;
        public string Note => note;
    }

    //--------------------------

    public  class ScaleStep
    {
        private readonly int index;
        private readonly double cents;
        private readonly double freq;

        public ScaleStep(int index, double cents, double freq)
        {
            this.index = index;
            this.cents = cents;
            this.freq = freq;
        }

        public int Index => index;
        public double Cents => cents;

        public double Freq => freq;

        public IntervalRatio Ratio { get; set; }
        public string ScaleNote { get; set; }

        public double DiffJust
        {
            get
            {
                if (Ratio == null)
                {
                    return double.PositiveInfinity;
                }
                return Cents - Ratio.Cents;
            }
        }
    }

    //--------------------------

    public  abstract class Scale
    {
        protected int nbSteps;
        protected ScaleStep[] scaleSteps;
        protected List<IntervalRatio> refIntervalRatios;
        protected readonly string[] keyboardNotes = {
            "C", "C#", "D", "Eb", "E", "F", "F#", "G", "Ab", "A", "Bb", "B"
        };

        public Scale(int nbSteps)
        {
            this.nbSteps = nbSteps;
            this.scaleSteps = new ScaleStep[nbSteps + 1];

            this.refIntervalRatios = new List<IntervalRatio>
            {
                new IntervalRatio(16, 15, "m2", "Db"),
                new IntervalRatio(9, 8, "M2", "D"),
                new IntervalRatio(6, 5, "m3", "Eb"),
                new IntervalRatio(5, 4, "M3", "E"),
                new IntervalRatio(4, 3, "P4", "F"),
                new IntervalRatio(11, 8, "11HTT", ""),
                new IntervalRatio(7, 5, "l7TT", ""),
                new IntervalRatio(10, 7, "g7TT", ""),
                new IntervalRatio(3, 2, "P5", "G"),
                new IntervalRatio(8, 5, "m6", "Ab"),
                new IntervalRatio(5, 3, "M6", "A"),
                new IntervalRatio(7, 4, "H7", "Bbb"),
                new IntervalRatio(9, 5, "m7", "Bb"),
                new IntervalRatio(15, 8, "M7", "B"),
                new IntervalRatio(2, 1, "P8", "C"),
            };
        }

        public abstract void Generate();

        public void PlaceRatios()
        {
            List<IntervalRatio> toPlace = new List<IntervalRatio>(refIntervalRatios);

            // always place C on 1st entry, idx = 0
            scaleSteps[0].Ratio = new IntervalRatio(1, 1, "P1", "C");

            while (toPlace.Count > 0)
            {
                // find next best candidate
                int stepIdx = -1;
                int ratioIdx = FindNextBestCandidate(toPlace, out stepIdx);
                if (stepIdx < 0)
                    break;

                // place it
                IntervalRatio ratio = toPlace[ratioIdx];
                ScaleStep scaleStep = scaleSteps[stepIdx];
                toPlace.RemoveAt(ratioIdx);
                scaleStep.Ratio = ratio;

                //## debug
                //Console.WriteLine("placing ratio {0} {1:F2} @ {2} {3:F2}", ratio, ratio.Cents, stepIdx, scaleStep.Cents);
            }

        }

        // find where to place a ratio on the scale
        private int FindNextBestCandidate(List<IntervalRatio> toPlace, out int stepIdx)
        {
            stepIdx = -1;
            int ratioIdx = -1;
            double bestPlacediff = double.PositiveInfinity;

            for (int i = 0; i < toPlace.Count; i++)
            {
                IntervalRatio ratio = toPlace[i];

                // place ratio between smaller and greater entries 
                int idxStart = 0;
                int idxEnd = 0;
                FindValidRange(ratio, out idxStart, out idxEnd);

                for (int j = idxStart; j <= idxEnd; j++)
                {
                    var checkStep = scaleSteps[j];
                    if (checkStep.Ratio == null)
                    {
                        double diff = Math.Abs(ratio.Cents - checkStep.Cents);
                        if (diff < bestPlacediff)
                        {
                            ratioIdx = i;
                            stepIdx = j;
                            bestPlacediff = diff;
                        }
                    }
                }
            }

            return ratioIdx;
        }

        private void FindValidRange(IntervalRatio ratio, out int idxStart, out int idxEnd)
        {
            idxStart = 1; // we dont place anything on step 0, it is the root, no interval
            idxEnd = scaleSteps.Length - 1;

            // start at last placed ratio entry smaller than ratio
            for (int i = scaleSteps.Length - 1; i >= 0; i--)
            {
                if (scaleSteps[i].Ratio != null && scaleSteps[i].Ratio.Cents < ratio.Cents)
                {
                    idxStart = i;
                    break;
                }
            }

            // end at 1st placed ratio entry greater than ratio
            for (int i = 0; i < scaleSteps.Length; i++)
            {
                if (scaleSteps[i].Ratio != null && scaleSteps[i].Ratio.Cents > ratio.Cents)
                {
                    idxEnd = i;
                    break;
                }
            }
        }

        public void ShowPlacedIntervals()
        {
            for (int i = 0; i < scaleSteps.Length; i++)
            {
                var step = scaleSteps[i];
                if (step.Ratio != null)
                {
                    Console.Write("{0}\t", step.Ratio.Interval);
                    Console.Write("{0}\t", step.Ratio);
                    Console.Write("{0:F2}\t", step.Ratio.Cents);
                    Console.WriteLine("{0:F2}", step.DiffJust);
                }
            }
        }

        public virtual void Show()
        {
            
            Console.WriteLine("Steps\tNote\tCents\t-- Just\tratio\tcents\terror --\tFrequency\tKbd note");

            //for (int i = 0; i < scaleSteps.Length; i++)
            for (int i = scaleSteps.Length-1; i >= 0; i--)
            {
                var step = scaleSteps[i];

                int octave = i / 12 + 1;
                int kbdKeyIdx = i % 12;
                string kbdKey = keyboardNotes[kbdKeyIdx];

                Console.Write("{0:D}\t", step.Index);

                if (step.ScaleNote != null) // not all scales have this
                    Console.Write("{0}\t", step.ScaleNote);
                else
                {
                    if (step.Ratio != null)
                        Console.Write("{0}\t", step.Ratio.Note);
                    else
                        Console.Write("\t");
                }

                Console.Write("{0,7:F2}\t", step.Cents);

                if (step.Ratio != null)
                {
                    Console.Write("{0}\t", step.Ratio.Interval);
                    Console.Write("{0}\t", step.Ratio);
                    Console.Write("{0:F2}\t", step.Ratio.Cents);
                    Console.Write("{0:F2}\t", step.DiffJust);

                    //Console.Write("\t{0,-5}\t{1,-5}\t{2,6:F2}\t{3,6:F2}", step.Ratio, step.Ratio.Interval, step.Ratio.Cents, step.DiffJust);
                }
                else
                {
                    Console.Write("\t\t\t\t");
                }

                Console.Write("{0:F2}\t", step.Freq);
                Console.Write("{0}{1}", kbdKey, octave);

                Console.WriteLine("");
            }
        }

        // This is designed for EDO scales .. might not be right for others
        public void CreateScaleNotes()
        {
            if (nbSteps < 12)
                return;

            int idxScaleStep = 0;
            while (scaleSteps[idxScaleStep] != null)
            {
                if (scaleSteps[idxScaleStep]?.Ratio?.Interval == "M2")
                    break;
                idxScaleStep++;
            }
            if (scaleSteps[idxScaleStep] == null)
                return;

            int toneSteps = idxScaleStep;
            int htoneSteps = ((nbSteps - toneSteps * 5) / 2);

            string[] notes = { "C", "D", "E", "F", "G", "A", "B", "C" };
            int[] isTones = { 1, 1, 0, 1, 1, 1, 0 };

            idxScaleStep = 0;

            for (int i = 0; i < notes.Length-1; i++)
            {
                string note = notes[i];
                string nextNote = notes[i + 1];
                bool isTone = (isTones[i] == 1);
                int nbSteps = (isTone ? toneSteps : htoneSteps);

                int nbSharps = (nbSteps - 1) / 2;
                int nbFlats = (nbSteps - 1) / 2;
                bool needsPair = ((nbSteps - 1) % 2) == 1;

                // sharps
                for (int sharps = 0; sharps <= nbSharps; sharps++)
                {
                    string n = string.Format("{0}{1} ", note, new string('#', sharps));

                    scaleSteps[idxScaleStep++].ScaleNote = n;
                    //Console.Write(n);
                }

                // "C#/Db"
                if (needsPair)
                {
                    string n = string.Format("{0}{1}/{2}{3} ",
                        note, new string('#', nbSharps+1),
                        nextNote, new string('b', nbFlats + 1));

                    scaleSteps[idxScaleStep++].ScaleNote = n;
                    //Console.Write(n);
                }

                // flats
                for (int flats = nbSharps; flats > 0; flats--)
                {
                    string n = string.Format("{0}{1} ", nextNote, new string('b', flats));

                    scaleSteps[idxScaleStep++].ScaleNote = n;
                    //Console.Write(n);
                }
            }

            scaleSteps[idxScaleStep++].ScaleNote = "C";

            //Console.WriteLine("C");
        }
    }
}

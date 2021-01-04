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

        public IntervalRatio(int n1, int n2, string interval)
        {
            this.n1 = n1;
            this.n2 = n2;
            this.interval = interval;
            this.cents = 1200 * Math.Log((double)n1 / n2, 2);
        }

        override public string ToString()
        {
            return string.Format("{0}:{1}", n1, n2);
        }

        public double Cents => cents;

        public string Interval => interval;
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

        internal IntervalRatio Ratio { get; set; }

        internal double DiffJust
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
        protected List<IntervalRatio> intervalRatios;
        protected readonly string[] keyboardNotes = {
            "C", "C#", "D", "Eb", "E", "F", "F#", "G", "Ab", "A", "Bb", "B"
        };

        public Scale(int nbSteps)
        {
            this.nbSteps = nbSteps;
            this.scaleSteps = new ScaleStep[nbSteps + 1];

            this.intervalRatios = new List<IntervalRatio>
            {
                new IntervalRatio(16, 15, "m2"),
                new IntervalRatio(9, 8, "M2"),
                new IntervalRatio(6, 5, "m3"),
                new IntervalRatio(5, 4, "M3"),
                new IntervalRatio(4, 3, "P4"),
                new IntervalRatio(11, 8, "11HTT"),
                new IntervalRatio(7, 5, "l7TT"),
                new IntervalRatio(10, 7, "g7TT"),
                new IntervalRatio(3, 2, "P5"),
                new IntervalRatio(8, 5, "m6"),
                new IntervalRatio(5, 3, "M6"),
                new IntervalRatio(7, 4, "H7"),
                new IntervalRatio(9, 5, "m7"),
                new IntervalRatio(15, 8, "M7"),
                new IntervalRatio(2, 1, "P8"),
            };
        }

        public abstract void Generate();

        public void PlaceRatios()
        {
            List<IntervalRatio> toPlace = new List<IntervalRatio>(intervalRatios);

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

        //## not good
        public void __PlaceRatio__(IntervalRatio newRatio)
        {
            List<IntervalRatio> todo = new List<IntervalRatio>
            {
                newRatio
            };

            while (todo.Count > 0)
            {
                IntervalRatio ratio = todo[0];
                todo.RemoveAt(0);

                ScaleStep scaleStepPlaced = null;
                double centsDiffPlaced = 0;

                foreach (var scaleStep in scaleSteps)
                {
                    // check if good place to put this ratio
                    double centsDiff = Math.Abs(scaleStep.Cents - ratio.Cents);

                    // potential place for this ratio
                    if (centsDiff < centsDiffPlaced || scaleStepPlaced == null)
                    {
                        // empty slot or we have a better score than ratio already there
                        if (scaleStep.Ratio == null || centsDiff < Math.Abs(scaleStep.DiffJust))
                        {
                            // debug
                            //Console.WriteLine("Placing {0} at {1}", ratio, i);

                            scaleStepPlaced = scaleStep;
                            centsDiffPlaced = centsDiff;

                            if (scaleStep.Ratio != null)
                            {
                                //Console.WriteLine("existing ratio {0} will need to be placed again", ratio);
                                todo.Add(scaleStep.Ratio);
                            }
                        }
                    }
                }

                if (scaleStepPlaced != null)
                {
                    scaleStepPlaced.Ratio = ratio;
                }
                else
                {
                    //Console.WriteLine("ratio {0} could not be placed", ratio);
                }
            }
        }
        public void __PlaceRatio__(int n1, int n2, string interval)
        {
            __PlaceRatio__(new IntervalRatio(n1, n2, interval));
        }

        public void Show()
        {
            Console.WriteLine("Steps\tCents\t-- Just\tratio\tcents\terror --\tFrequency\tKbd note");

            for (int i = 0; i < scaleSteps.Length; i++)
            {
                var step = scaleSteps[i];

                int octave = i / 12 + 1;
                int kbdKeyIdx = i % 12;
                string kbdKey = keyboardNotes[kbdKeyIdx];

                Console.Write("{0:D}\t", step.Index);
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
    }
}

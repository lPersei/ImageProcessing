using System;

namespace HistogtamV3
{
    /// <summary>
    /// Klasa dla wyrównania Histogramu
    /// </summary>
    internal class EqualizationTable
    {
        int[] Histogram = new int[256];
        float[] Probability = new float[256];
        float[] CummulativeProbability = new float[256];
        float[] MultiplyLevel = new float[256];
        public int[] Rounding = new int[256];

        public EqualizationTable(int[] Histogram)
        {
            this.Histogram = Histogram;
            Calculate();
        }

        private void Calculate()
        {
            PDF();
            CDF();
            Multiply();
            Round();
        }

        /// <summary>
        /// Rounding the elements
        /// </summary>
        private void Round()
        {
            for (int i = 0; i < 256; ++i)
            {
                Rounding[i] = (int)Math.Floor(MultiplyLevel[i]);
            }
        }

        /// <summary>
        /// Cummulative probability * L-1(256-1)
        /// </summary>
        private void Multiply()
        {
            for (int i = 0; i < 256; ++i)
            {
                MultiplyLevel[i] = CummulativeProbability[i] * 255;
            }
        }

        /// <summary>
        /// Cummulative probability table
        /// </summary>
        private void CDF()
        {
            CummulativeProbability[0] = Probability[0];
            for (int i = 1; i < 256; ++i)
            {
                CummulativeProbability[i] = CummulativeProbability[i - 1] + Probability[i];
            }
        }
        /// <summary>
        /// Probability table
        /// </summary>
        private void PDF()
        {
            float sum = 0f;
            foreach (int item in Histogram)
            {
                sum += item;
            }

            for (int i = 0; i < 256; ++i)
            {
                Probability[i] = Histogram[i] / sum;
            }
        }

        public int[] GetRound()
        {
            return Rounding;
        }
    }
}
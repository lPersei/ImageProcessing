using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistogtamV3
{
    /// <summary>
    /// Klasa dla operacij logicznych dla 2ch obracków  o tych samych rozmiarach
    /// </summary>
    public class Arithmetics
    {
        private int[,] GrayA;
        private int[,] GrayB;

        public Arithmetics(int[,] GrayA, int[,] GrayB)
        {
            this.GrayA = GrayA;
            this.GrayB = GrayB;
        }

        public Bitmap Addition()
        {
            Bitmap bmp = new Bitmap(GrayA.GetLength(0), GrayA.GetLength(1));
            Color newColor;
            int var;

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    var = (GrayA[x, y] + GrayB[x, y]) / 2;
                    newColor = Color.FromArgb(var, var, var);
                    bmp.SetPixel(x, y, newColor);
                }
            }

            return bmp;
        }

        public Bitmap Subtraction()
        {
            Bitmap bmp = new Bitmap(GrayA.GetLength(0), GrayA.GetLength(1));
            Color newColor;
            int var;

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    var = Math.Abs(GrayA[x, y] - GrayB[x, y]);
                    newColor = Color.FromArgb(var, var, var);
                    bmp.SetPixel(x, y, newColor);
                }
            }

            return bmp;
        }

        public Bitmap Difference()
        {
            Bitmap bmp = new Bitmap(GrayA.GetLength(0), GrayA.GetLength(1));
            Color newColor;
            int var;

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    var = GrayA[x, y] - GrayB[x, y];

                    if (var < 0) var = 0;

                    newColor = Color.FromArgb(var, var, var);
                    bmp.SetPixel(x, y, newColor);
                }
            }

            return bmp;
        }

        public Bitmap AND()
        {
            Bitmap bmp = new Bitmap(GrayA.GetLength(0), GrayA.GetLength(1));
            Color newColor;
            int var;

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    var = GrayA[x, y] & GrayB[x, y];

                    newColor = Color.FromArgb(var, var, var);
                    bmp.SetPixel(x, y, newColor);
                }
            }

            return bmp;
        }

        public Bitmap OR()
        {
            Bitmap bmp = new Bitmap(GrayA.GetLength(0), GrayA.GetLength(1));
            Color newColor;
            int var;

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    var = GrayA[x, y] | GrayB[x, y];

                    newColor = Color.FromArgb(var, var, var);
                    bmp.SetPixel(x, y, newColor);
                }
            }

            return bmp;
        }

        public Bitmap XOR()
        {
            Bitmap bmp = new Bitmap(GrayA.GetLength(0), GrayA.GetLength(1));
            Color newColor;
            int var;

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    var = GrayA[x, y] ^ GrayB[x, y];

                    newColor = Color.FromArgb(var, var, var);
                    bmp.SetPixel(x, y, newColor);
                }
            }

            return bmp;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.IO;
using Accord.Imaging.Filters;
using System.Windows.Forms;
using AForge.Math;

namespace HistogtamV3
{
    /// <summary>
    /// Klasa zawierająca metody i zmienne dla głównej formatki
    /// </summary>
    static class Body
    {
        public static Bitmap grayscaleImage { get; set; }//Bitmap dla szaroodcieniowego obrazku
        public static Bitmap originalImage { get; set; }//Bitmap dla originalnego obrazku

        public static Bitmap controlImageGLR = new Bitmap(256,256); //Bitmap dla kontrolki
        public static Bitmap controlImageSO = new Bitmap(256, 256); //Bitmap dla kontrolki

        public static byte[] LUT = new byte[256]; // Tabela LUT dla Cw2

        public static int[] Histogram = new int[256];//Tabela dla wartości histogramu
        public static int[,] Gray { get; set; }//Tabela z szaroodcieniowymi wartościami
        public static int[,] GrayA { get; set; }//Tabela z szaroodcieniowymi wartościami
        public static int[,] GrayB { get; set; }//Tabela z szaroodcieniowymi wartościami
        public static byte[] Bytemap { get; set; }//Tabela z szaroodcieniowymi wartościami byte
        public static int[,] GrayMask { get; set; }//Tabela z wartościami dla własnej maski

        public static Random r = new Random();//Randomizer

        /// <summary>
        /// Przypisywanie wartości dla Gray, Bytemap, LUT,Szaroodcienianie obrazku
        /// </summary>
        public static void FillBodyData()
        {
            Bytemap = new byte[grayscaleImage.Width*grayscaleImage.Height];
            grayscaleImage = MakeGray(originalImage);
            int i = 0;
            for (int x = 0; x < grayscaleImage.Width; x++)
            {
                for (int y = 0; y < grayscaleImage.Height; y++)
                {
                    Gray[x, y] = Bytemap[i] = grayscaleImage.GetPixel(x,y).R;
                    i++;
                }
            }
            
            foreach (byte item in LUT)
            {
                LUT[item] = (byte)r.Next(0, 255);
            }
        }
        /// <summary>
        /// Przypisywanie wartości dla GrayA
        /// </summary>
        /// <param name="img"></param>
        public static void FillGrayA(Bitmap img)
        {
            img = MakeGray(img);
            GrayA = new int[img.Width, img.Height];
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    GrayA[x, y] = img.GetPixel(x, y).R;
                }
            }
        }
        /// <summary>
        /// Przypisywanie wartości dla GrayB
        /// </summary>
        /// <param name="img"></param>
        public static void FillGrayB(Bitmap img)
        {
            img = MakeGray(img);
            GrayB = new int[img.Width, img.Height];
            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    GrayB[x, y] = img.GetPixel(x, y).R;
                }
            }
        }
        /// <summary>
        /// Szaroodcienianie obrazku
        /// Zwraca gotową bitmapę
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Bitmap MakeGray(Bitmap img)
        {
            ZeroHistogram(Histogram);
            byte gray;
            Color color;

            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    color = img.GetPixel(x, y);

                    gray = (byte)(.21 * color.R + .71 * color.G + .071 * color.B);

                    Histogram[gray]++;

                    img.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
            }

            return img;
        }
        /// <summary>
        /// Rozciąganie histogramu
        /// </summary>
        public static void Stretching()
        {
            Bitmap img = grayscaleImage;
            int Lmin = 0;
            int Lmax = 255;
            byte pixel;

            //minimalna niezerowa wartość
            for (int i = 0; i < 255; i++)
            {
                if (Histogram[i] == 0)
                {
                    Lmin++;
                }
                else break;

            }
            //maksymalna niezerowa wartość
            for (int i = 0; i < 255; i++)
            {
                if (Histogram[255 - i] == 0)
                {
                    Lmax--;
                }
                else break;
            }

            ZeroHistogram(Histogram);

            for (int x = 0; x < originalImage.Width; x++)
            {
                for (int y = 0; y < originalImage.Height; y++)
                {
                    pixel = (byte)(255 / (Lmax - Lmin) * (Gray[x, y] - Lmin));

                    Gray[x, y] = pixel;
                    Histogram[pixel]++;

                    img.SetPixel(x, y, Color.FromArgb(pixel, pixel, pixel));
                }
            }

            grayscaleImage = img;
        }
        /// <summary>
        /// Wyrównanie Histogramu
        /// </summary>
        public static void Equalization()
        {
            EqualizationTable equalization = new EqualizationTable(Histogram);
            Bitmap img = grayscaleImage;
            int pixel;

            ZeroHistogram(Histogram);

            for (int x = 0; x < grayscaleImage.Width; x++)
            {
                for (int y = 0; y < grayscaleImage.Height; y++)
                {
                    pixel = equalization.Rounding[Gray[x, y]];

                    Gray[x, y] = pixel;
                    Histogram[pixel]++;

                    img.SetPixel(x, y, Color.FromArgb(pixel, pixel, pixel));
                }
            }

            grayscaleImage = img;
        }
        /// <summary>
        /// Negacja obrazku
        /// </summary>
        public static void MakeNegative()
        {
            Bitmap img = grayscaleImage;
            int pixel;

            ZeroHistogram(Histogram);

            for (int x = 0; x < originalImage.Width; x++)
            {
                for (int y = 0; y < originalImage.Height; y++)
                {
                    pixel = (255 - Gray[x, y]);

                    Gray[x, y] = pixel;
                    Histogram[pixel]++;

                    img.SetPixel(x, y, Color.FromArgb(pixel, pixel, pixel));
                }
            }

            grayscaleImage = img;
        }
        /// <summary>
        /// Binaryzacja
        /// </summary>
        /// <param name="value"></param>
        public static void Binaryzation(int value)
        {
            Bitmap img = grayscaleImage;
            byte pixel;

            ZeroHistogram(Histogram);

            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    if (Gray[x, y] <= value)
                    {
                        pixel = 0;
                    }
                    else
                    {
                        pixel = 255;
                    }

                    //Gray[x, y] = pixel;
                    Histogram[pixel]++;

                    img.SetPixel(x, y, Color.FromArgb(pixel, pixel, pixel));
                }
            }

            grayscaleImage = img;
        }
        /// <summary>
        /// Binaryzacja z wyborem
        /// </summary>
        /// <param name="value"></param>
        /// <param name="exvalue"></param>
        public static void SelectBinaryzation(int value, int exvalue)
        {
            Bitmap img = grayscaleImage;
            byte pixel;

            ZeroHistogram(Histogram);

            for (int x = 0; x < img.Width; x++)
            {
                for (int y = 0; y < img.Height; y++)
                {
                    if (value <= Gray[x, y] && Gray[x, y] <= exvalue)
                    {
                        pixel = 255;
                    }
                    else
                    {
                        pixel = 0;
                    }

                    //Gray[x, y] = pixel;
                    Histogram[pixel]++;

                    img.SetPixel(x, y, Color.FromArgb(pixel, pixel, pixel));
                }
            }

            grayscaleImage = img;
        }
        /// <summary>
        /// Obniżenie poziomy szarości
        /// </summary>
        /// <param name="BNUD1"></param>
        /// <param name="BNUD2"></param>
        /// <param name="BNUD3"></param>
        /// <param name="BNUD4"></param>
        /// <param name="LNUD1"></param>
        /// <param name="LNUD2"></param>
        /// <param name="LNUD3"></param>
        /// <returns></returns>
        public static Bitmap Reduction(int BNUD1, int BNUD2, int BNUD3, int BNUD4, int LNUD1, int LNUD2, int LNUD3)
        {
            Bitmap img = grayscaleImage;
            int pixel;

            ZeroHistogram(Histogram);

            for (int x = 0; x < grayscaleImage.Width; x++)
            {
                for (int y = 0; y < grayscaleImage.Height; y++)
                {
                    if (Gray[x, y] <= BNUD1)
                    {
                        pixel = 0;
                    }
                    else if (BNUD1 < Gray[x, y] && Gray[x, y] <= BNUD2)
                    {
                        pixel = LNUD1;
                    }
                    else if (BNUD2 < Gray[x, y] && Gray[x, y] <= BNUD3)
                    {
                        pixel = LNUD2;
                    }
                    else if (BNUD3 < Gray[x, y] && Gray[x, y] <= BNUD4)
                    {
                        pixel = LNUD3;
                    }
                    else
                    {
                        pixel = 255;
                    }

                    Histogram[pixel]++;

                    img.SetPixel(x, y, Color.FromArgb(pixel, pixel, pixel));
                }
            }

            grayscaleImage = img;
            return img;
        }
        /// <summary>
        /// Operator rozciągania
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        public static void StretchingOperator(int val1, int val2)
        {
            Bitmap img = grayscaleImage;
            int pixel;
            ZeroHistogram(Histogram);
            for (int x = 0; x < grayscaleImage.Width; x++)
            {
                for (int y = 0; y < grayscaleImage.Height; y++)
                {
                    if (val1 < Gray[x, y] && Gray[x, y] <= val2)
                    {
                        pixel = (int)((Gray[x, y] - val1) * (255f / (val2 - val1)));
                    }
                    else
                    {
                        pixel = 0;
                    }

                    Histogram[pixel]++;

                    img.SetPixel(x, y, Color.FromArgb(pixel, pixel, pixel));
                }
            }

            grayscaleImage = img;
        }      
        /// <summary>
        /// Wygładzanie
        /// </summary>
        /// <param name="TAB"></param>
        /// <returns></returns>
        public static Bitmap Blur(int[,] TAB)
        {
            int pix;
            Bitmap newBmp = new Bitmap(grayscaleImage.Width, grayscaleImage.Height);
            ZeroHistogram(Histogram);

            for (int x = 1; x < grayscaleImage.Width - 1; x++)
            {
                for (int y = 1; y < grayscaleImage.Height - 1; y++)
                {
                    pix = ((TAB[0, 0] * grayscaleImage.GetPixel(x - 1, y - 1).R) +
                          (TAB[0, 1] * grayscaleImage.GetPixel(x - 1, y).R) +
                          (TAB[0, 2] * grayscaleImage.GetPixel(x - 1, y + 1).R) +
                          (TAB[1, 0] * grayscaleImage.GetPixel(x, y - 1).R) +
                          (TAB[1, 1] * grayscaleImage.GetPixel(x, y).R) +
                          (TAB[1, 2] * grayscaleImage.GetPixel(x, y + 1).R) +
                          (TAB[2, 0] * grayscaleImage.GetPixel(x + 1, y - 1).R) +
                          (TAB[2, 1] * grayscaleImage.GetPixel(x + 1, y).R) +
                          (TAB[2, 2] * grayscaleImage.GetPixel(x + 1, y + 1).R)) / (TAB[0, 0] + TAB[0, 1] + TAB[0, 2] + TAB[1, 0] + TAB[1, 1] + TAB[1, 2] + TAB[2, 0] + TAB[2, 1] + TAB[2, 2]);
                    Histogram[pix]++;

                    newBmp.SetPixel(x, y, Color.FromArgb(pix, pix, pix));
                }
            }
            grayscaleImage = newBmp;
            return newBmp;
        }
        /// <summary>
        /// Wyostrzanie
        /// </summary>
        /// <param name="TAB"></param>
        /// <returns></returns>
        public static Bitmap Sharp(int[,] TAB)
        {
            int pix;
            Bitmap newBmp = new Bitmap(grayscaleImage.Width, grayscaleImage.Height);
            ZeroHistogram(Histogram);

            for (int x = 1; x < grayscaleImage.Width - 1; x++)
            {
                for (int y = 1; y < grayscaleImage.Height - 1; y++)
                {
                    pix = (TAB[0, 0] * grayscaleImage.GetPixel(x - 1, y - 1).R) +
                          (TAB[0, 1] * grayscaleImage.GetPixel(x - 1, y).R) +
                          (TAB[0, 2] * grayscaleImage.GetPixel(x - 1, y + 1).R) +
                          (TAB[1, 0] * grayscaleImage.GetPixel(x, y - 1).R) +
                          (TAB[1, 1] * grayscaleImage.GetPixel(x, y).R) +
                          (TAB[1, 2] * grayscaleImage.GetPixel(x, y + 1).R) +
                          (TAB[2, 0] * grayscaleImage.GetPixel(x + 1, y - 1).R) +
                          (TAB[2, 1] * grayscaleImage.GetPixel(x + 1, y).R) +
                          (TAB[2, 2] * grayscaleImage.GetPixel(x + 1, y + 1).R);
                    if (pix < 0)
                    {
                        pix = 0;
                    }
                    if (pix > 255)
                    {
                        pix = 255;
                    }
                    Histogram[pix]++;
                    newBmp.SetPixel(x, y, Color.FromArgb(pix, pix, pix));
                }
            }
            grayscaleImage = newBmp;
            return newBmp;
        }
        /// <summary>
        /// Detekcja Krawędzi
        /// </summary>
        /// <param name="TAB"></param>
        /// <returns></returns>
        public static Bitmap EdgeDetection(int[,] TAB)
        {
            int pix;
            Bitmap newBmp = new Bitmap(grayscaleImage.Width, grayscaleImage.Height);
            ZeroHistogram(Histogram);
            for (int x = 1; x < grayscaleImage.Width - 1; x++)
            {
                for (int y = 1; y < grayscaleImage.Height - 1; y++)
                {
                    pix = (TAB[0, 0] * grayscaleImage.GetPixel(x - 1, y - 1).R) +
                          (TAB[0, 1] * grayscaleImage.GetPixel(x - 1, y).R) +
                          (TAB[0, 2] * grayscaleImage.GetPixel(x - 1, y + 1).R) +
                          (TAB[1, 0] * grayscaleImage.GetPixel(x, y - 1).R) +
                          (TAB[1, 1] * grayscaleImage.GetPixel(x, y).R) +
                          (TAB[1, 2] * grayscaleImage.GetPixel(x, y + 1).R) +
                          (TAB[2, 0] * grayscaleImage.GetPixel(x + 1, y - 1).R) +
                          (TAB[2, 1] * grayscaleImage.GetPixel(x + 1, y).R) +
                          (TAB[2, 2] * grayscaleImage.GetPixel(x + 1, y + 1).R);
                    if (pix < 0)
                    {
                        pix = 0;
                    }
                    if (pix > 255)
                    {
                        pix = 255;
                    }
                    Histogram[pix]++;
                    newBmp.SetPixel(x, y, Color.FromArgb(pix, pix, pix));
                }
            }
            grayscaleImage = newBmp;
            return newBmp;
        }
        /// <summary>
        /// Nakładanie własnej maski
        /// </summary>
        /// <param name="border"></param>
        /// <param name="scale"></param>
        /// <param name="TAB"></param>
        /// <returns></returns>
        public static Bitmap CustomMask(MainForm.Border border,MainForm.Scaling scale,int[,] TAB)
        {
            Bitmap newBmp = new Bitmap(grayscaleImage.Width, grayscaleImage.Height);
            GrayMask = Gray;
            border(TAB);
            newBmp = scale(newBmp);
            return newBmp;
        }
        /// <summary>
        /// Skalowanie A
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Bitmap ScaleA(Bitmap image)
        {
            int Gmin, Gmax;
            int pixel;
            var arr = GrayMask;
            Gmax = arr.Cast<int>().Max();
            Gmin = arr.Cast<int>().Min();
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    pixel = (GrayMask[x, y] - Gmin) / (Gmax - Gmin) * 255;
                    image.SetPixel(x, y, Color.FromArgb(pixel, pixel, pixel));
                }
            }
            return image;
        }
        /// <summary>
        /// Skalowanie B
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Bitmap ScaleB(Bitmap image)
        {
            int pixel;
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    if (Gray[x,y]<0)
                    {
                        pixel = 0;
                    }
                    else if (Gray[x,y] == 0)
                    {
                        pixel = 127;
                    }
                    else
                    {
                        pixel = 255;
                    }
                    image.SetPixel(x, y, Color.FromArgb(pixel, pixel, pixel));
                }
            }
            return image;
        }
        /// <summary>
        /// Skalowanie C
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static Bitmap ScaleC(Bitmap image)
        {
            int pixel;
            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    if (Gray[x, y] < 0)
                    {
                        pixel = 0;
                    }
                    else if (Gray[x,y]>=0 && Gray[x, y]<=255)
                    {
                        pixel = Gray[x, y];
                    }
                    else
                    {
                        pixel = 255;
                    }
                    image.SetPixel(x, y, Color.FromArgb(pixel, pixel, pixel));
                }
            }
            return image;
        }
        /// <summary>
        /// Skrajnie pikseli bez zmiany
        /// </summary>
        /// <param name="TAB"></param>
        /// <returns></returns>
        public static void BorderPixelsA(int[,] TAB)
        {
            int pix;

            for (int x = 1; x < originalImage.Width - 1; x++)
            {
                for (int y = 1; y < originalImage.Height - 1; y++)
                {
                    pix = (TAB[0, 0] * Gray[x - 1, y - 1] +
                          TAB[0, 1] * Gray[x, y - 1] +
                          TAB[0, 2] * Gray[x + 1, y - 1] +
                          TAB[1, 0] * Gray[x - 1, y] +
                          TAB[1, 1] * Gray[x, y] +
                          TAB[1, 2] * Gray[x + 1, y] +
                          TAB[2, 0] * Gray[x - 1, y + 1] +
                          TAB[2, 1] * Gray[x, y + 1] +
                          TAB[2, 2] * Gray[x + 1, y + 1]) / (TAB[0, 0] + TAB[0, 1] + TAB[0, 2] + TAB[1, 0] + TAB[1, 1] + TAB[1, 2] + TAB[2, 0] + TAB[2, 1] + TAB[2, 2]);

                    GrayMask[x, y] = pix;
                }
            }
        }
        /// <summary>
        /// powielenie wartości pikseli brzegowych
        /// </summary>
        /// <param name="TAB"></param>
        /// <returns></returns>
        public static void BorderPixelsB(int[,] TAB)
        {
            Bitmap bmp = new Bitmap(grayscaleImage, grayscaleImage.Width + 2, grayscaleImage.Height + 2);

            int[,] HelpTAB = new int[bmp.Width, bmp.Height];
            int pix;
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    HelpTAB[x, y] = bmp.GetPixel(x, y).R;
                }
            }
            for (int x = 0; x < bmp.Width-2; x++)
            {
                for (int y = 0; y < bmp.Height-2; y++)
                {
                    HelpTAB[x+1, y+1] = Gray[x, y];
                }
            }

            for (int x = 1; x < bmp.Width-1; x++)
            {
                for (int y = 1; y < bmp.Height-1; y++)
                {
                    pix = (TAB[0, 0] * HelpTAB[x - 1, y - 1] +
                          TAB[0, 1] * HelpTAB[x, y - 1] +
                          TAB[0, 2] * HelpTAB[x + 1, y - 1] +
                          TAB[1, 0] * HelpTAB[x - 1, y] +
                          TAB[1, 1] * HelpTAB[x, y] +
                          TAB[1, 2] * HelpTAB[x + 1, y] +
                          TAB[2, 0] * HelpTAB[x - 1, y + 1] +
                          TAB[2, 1] * HelpTAB[x, y + 1] +
                          TAB[2, 2] * HelpTAB[x + 1, y + 1]) / (TAB[0, 0] + TAB[0, 1] + TAB[0, 2] + TAB[1, 0] + TAB[1, 1] + TAB[1, 2] + TAB[2, 0] + TAB[2, 1] + TAB[2, 2]);

                    GrayMask[x-1, y-1] = pix;
                }
            }
        }

        /// <summary>
        /// Skeletonization za pomocą biblioteki Accord
        /// </summary>
        /// <returns></returns>
        public static Bitmap Thining()
        {           
            Grayscale gray = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = gray.Apply(originalImage);

            Threshold threshold = new Threshold(127);
            threshold.ApplyInPlace(grayImage);

            SimpleSkeletonization filter = new SimpleSkeletonization();
            filter.ApplyInPlace(grayImage);

            return grayImage;
        }
        /// <summary>
        /// Erozion za pomocą biblioteki Accord
        /// </summary>
        /// <returns></returns>
        public static Bitmap Erozion(short[,] mask)
        {
            Grayscale gray = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = gray.Apply(originalImage);

            Erosion filter = new Erosion(mask);
            grayImage = filter.Apply(grayImage);

            return grayImage;
        }
        /// <summary>
        /// Dilation za pomocą biblioteki Accord
        /// </summary>
        /// <returns></returns>
        public static Bitmap Dilation(short[,] mask)
        {
            Grayscale gray = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = gray.Apply(originalImage);

            Dilation filter = new Dilation(mask);
            grayImage = filter.Apply(grayImage);

            return grayImage;
        }
        /// <summary>
        /// Opening za pomocą biblioteki Accord
        /// </summary>
        /// <returns></returns>
        public static Bitmap Opening(short[,] mask)
        {
            Grayscale gray = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = gray.Apply(originalImage);

            Opening filter = new Opening(mask);
            grayImage = filter.Apply(grayImage);

            return grayImage;
        }
        /// <summary>
        /// Closing za pomocą biblioteki Accord
        /// </summary>
        /// <returns></returns>
        public static Bitmap Closing(short[,] mask)
        {
            Grayscale gray = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = gray.Apply(originalImage);

            Closing filter = new Closing(mask);
            grayImage = filter.Apply(grayImage);

            return grayImage;
        }
        /// <summary>
        /// Watershed za pomocą biblioteki Accord
        /// </summary>
        /// <returns></returns>
        public static Bitmap Watershed()
        {
            Grayscale gray = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = gray.Apply(originalImage);

            BinaryWatershed filter = new BinaryWatershed();
            grayImage = filter.Apply(grayImage);

            return grayImage;
        }
        /// <summary>
        /// Threshold za pomocą biblioteki Accord
        /// </summary>
        /// <returns></returns>
        public static Bitmap Thresh()
        {
            Grayscale gray = new Grayscale(0.2125, 0.7154, 0.0721);
            Bitmap grayImage = gray.Apply(originalImage);

            Threshold filter = new Threshold();
            grayImage = filter.Apply(grayImage);

            return grayImage;
        }
        /// <summary>
        /// Kompresja RLE
        /// </summary>
        /// <returns></returns>
        public static byte[] CompressionRLE()
        {
            byte[] TAB = new byte[Bytemap.Length * 2];
            byte count = 1;
            byte current = Bytemap[0];
            int i = 0;
            for (int x = 1; x < Bytemap.Length; ++x)
            {
                if (count == 255)
                {
                    TAB[i] = count;
                    TAB[i + 1] = current;
                    count = 1;
                    i += 2;
                    current = Bytemap[x];
                }
                else if (current == Bytemap[x])
                {
                    count++;
                }
                else
                {
                    TAB[i] = count;
                    TAB[i + 1] = current;
                    count = 1;
                    i += 2;
                    current = Bytemap[x];
                }
            }
            TAB[i] = count;
            TAB[i + 1] = current;
            i += 2;
            Array.Resize(ref TAB, i);
            return TAB;
        }
        /// <summary>
        /// Zwraca string ze Stopniem Kompresji
        /// </summary>
        /// <param name="originalSize"></param>
        /// <param name="compressedZize"></param>
        /// <returns></returns>
        public static string ShowSK(int originalSize, int compressedZize)
        {
            string myStr = "Compression rate: " + ((double)originalSize / compressedZize).ToString();
            return myStr;
        }
        /// <summary>
        /// Obliczenie nowej Bitmapy dla operacji na LUT w CW2
        /// </summary>
        /// <returns></returns>
        public static Bitmap LUTDraw()
        {
            Bitmap bmp = grayscaleImage;
            byte pix;
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    pix = LUT[bmp.GetPixel(x, y).R];
                    bmp.SetPixel(x, y, Color.FromArgb(pix, pix, pix));
                }
            }
            return bmp;
        }

        /// <summary>
        /// Metoda skrajnich pikseli C
        /// </summary>
        /// <param name="TAB"></param>
        public static void BorderPixelsC(int[,] TAB)
        {
            Bitmap bmp = new Bitmap(grayscaleImage, grayscaleImage.Width + 2, grayscaleImage.Height + 2);
            ZeroHistogram(Histogram);
            int[,] HelpTAB = new int[bmp.Width, bmp.Height];
            int pix;
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    HelpTAB[x, y] = bmp.GetPixel(x, y).R;
                }
            }
            for (int x = 0; x < bmp.Width - 2; x++)
            {
                for (int y = 0; y < bmp.Height - 2; y++)
                {
                    HelpTAB[x + 1, y + 1] = Gray[x, y];
                }
            }

            for (int x = 1; x < bmp.Width - 1; x++)
            {
                for (int y = 1; y < bmp.Height - 1; y++)
                {
                    pix = (TAB[0, 0] * HelpTAB[x - 1, y - 1] +
                          TAB[0, 1] * HelpTAB[x, y - 1] +
                          TAB[0, 2] * HelpTAB[x + 1, y - 1] +
                          TAB[1, 0] * HelpTAB[x - 1, y] +
                          TAB[1, 1] * HelpTAB[x, y] +
                          TAB[1, 2] * HelpTAB[x + 1, y] +
                          TAB[2, 0] * HelpTAB[x - 1, y + 1] +
                          TAB[2, 1] * HelpTAB[x, y + 1] +
                          TAB[2, 2] * HelpTAB[x + 1, y + 1]) / (TAB[0, 0] + TAB[0, 1] + TAB[0, 2] + TAB[1, 0] + TAB[1, 1] + TAB[1, 2] + TAB[2, 0] + TAB[2, 1] + TAB[2, 2]);
                    Histogram[pix]++;
                    GrayMask[x - 1, y - 1] = pix;
                }
            }

        }
        /// <summary>
        /// Filtr medianowy 3x3 
        /// </summary>
        /// <returns></returns>
        public static Bitmap Median3x3()
        {
            Bitmap bmp = grayscaleImage;
            int pix;
            ZeroHistogram(Histogram);
            for (int x = 1; x < grayscaleImage.Width-1; x++)
            {
                for (int y = 1; y < grayscaleImage.Height-1; y++)
                {
                    int[] sorted = new int[] {
                        Gray[x - 1, y - 1], Gray[x, y - 1], Gray[x + 1, y - 1],
                        Gray[x - 1, y],     Gray[x, y],     Gray[x + 1, y],
                        Gray[x - 1, y + 1], Gray[x, y + 1], Gray[x + 1, y + 1]
                    };
                    Array.Sort(sorted);
                    pix = sorted[sorted.Length / 2];
                    Histogram[pix]++;
                    bmp.SetPixel(x, y, Color.FromArgb(pix, pix, pix));
                }
            }
            return bmp;
        }
        /// <summary>
        /// Filtr medianowy 5x5 
        /// </summary>
        /// <returns></returns>
        public static Bitmap Median5x5()
        {
            Bitmap bmp = grayscaleImage;
            int pix;
            ZeroHistogram(Histogram);
            for (int x = 2; x < grayscaleImage.Width - 2; x++)
            {
                for (int y = 2; y < grayscaleImage.Height - 2; y++)
                {
                    int[] sorted = new int[] {
                        Gray[x - 2, y - 2], Gray[x - 1, y - 2], Gray[x, y - 2], Gray[x + 1, y - 2],Gray[x + 2, y - 2],
                        Gray[x - 2, y - 1], Gray[x - 1, y - 1], Gray[x, y - 1], Gray[x + 1, y - 1],Gray[x + 2, y - 1],
                        Gray[x - 2, y],     Gray[x - 1, y],     Gray[x, y],     Gray[x + 1, y],    Gray[x + 2, y],
                        Gray[x - 2, y + 1], Gray[x - 1, y + 1], Gray[x, y + 1], Gray[x + 1, y + 1],Gray[x + 2, y + 1],
                        Gray[x - 2, y + 2], Gray[x - 1, y + 2], Gray[x, y + 2], Gray[x + 1, y + 2],Gray[x + 2, y + 2]
                    };
                    Array.Sort(sorted);
                    pix = sorted[sorted.Length / 2];
                    Histogram[pix]++;
                    bmp.SetPixel(x, y, Color.FromArgb(pix, pix, pix));
                }
            }
            return bmp;
        }
        /// <summary>
        /// Filtr medianowy 7x7
        /// </summary>
        /// <returns></returns>
        public static Bitmap Median7x7()
        {
            Bitmap bmp = grayscaleImage;
            int pix;
            ZeroHistogram(Histogram);
            for (int x = 3; x < grayscaleImage.Width - 3; x++)
            {
                for (int y = 3; y < grayscaleImage.Height - 3; y++)
                {
                    int[] sorted = new int[] {

                        Gray[x - 3, y - 3], Gray[x - 2, y - 3], Gray[x - 1, y - 2], Gray[x, y - 3], Gray[x + 1, y - 3], Gray[x + 2, y - 3], Gray[x + 3, y - 3],
                        Gray[x - 3, y - 2], Gray[x - 2, y - 2], Gray[x - 1, y - 2], Gray[x, y - 2], Gray[x + 1, y - 2], Gray[x + 2, y - 2], Gray[x + 3, y - 2],
                        Gray[x - 3, y - 1], Gray[x - 2, y - 1], Gray[x - 1, y - 1], Gray[x, y - 1], Gray[x + 1, y - 1], Gray[x + 2, y - 1], Gray[x + 3, y - 1],
                        Gray[x - 3, y],     Gray[x - 2, y],     Gray[x - 1, y],     Gray[x, y],     Gray[x + 1, y],     Gray[x + 2, y],     Gray[x + 3, y],
                        Gray[x - 3, y + 1], Gray[x - 2, y + 1], Gray[x - 1, y + 1], Gray[x, y + 1], Gray[x + 1, y + 1], Gray[x + 2, y + 1], Gray[x + 3, y + 1],
                        Gray[x - 3, y + 2], Gray[x - 2, y + 2], Gray[x - 1, y + 2], Gray[x, y + 2], Gray[x + 1, y + 2], Gray[x + 2, y + 2], Gray[x + 3, y + 2],
                        Gray[x - 3, y + 3], Gray[x - 2, y + 3], Gray[x - 1, y + 3], Gray[x, y + 3], Gray[x + 1, y + 3], Gray[x + 2, y + 3], Gray[x + 3, y + 3]
                    };
                    Array.Sort(sorted);
                    pix = sorted[sorted.Length / 2];
                    Histogram[pix]++;
                    bmp.SetPixel(x, y, Color.FromArgb(pix, pix, pix));
                }
            }
            return bmp;
        }

        /// <summary>
        /// Filtr medianowy 3x5 
        /// </summary>
        /// <returns></returns>
        public static Bitmap Median3x5()
        {
            Bitmap bmp = grayscaleImage;
            int pix;
            ZeroHistogram(Histogram);
            for (int x = 1; x < grayscaleImage.Width - 1; x++)
            {
                for (int y = 2; y < grayscaleImage.Height - 2; y++)
                {
                    int[] sorted = new int[] {
                        Gray[x - 1, y - 2], Gray[x, y - 2], Gray[x + 1, y - 2],
                        Gray[x - 1, y - 1], Gray[x, y - 1], Gray[x + 1, y - 1],
                        Gray[x - 1, y],     Gray[x, y],     Gray[x + 1, y],   
                        Gray[x - 1, y + 1], Gray[x, y + 1], Gray[x + 1, y + 1],
                        Gray[x - 1, y + 2], Gray[x, y + 2], Gray[x + 1, y + 2]
                    };
                    Array.Sort(sorted);
                    pix = sorted[sorted.Length / 2];
                    Histogram[pix]++;
                    bmp.SetPixel(x, y, Color.FromArgb(pix, pix, pix));
                }
            }
            return bmp;
        }

        /// <summary>
        /// Filtr medianowy 5x3 
        /// </summary>
        /// <returns></returns>
        public static Bitmap Median5x3()
        {
            Bitmap bmp = grayscaleImage;
            int pix;
            ZeroHistogram(Histogram);
            for (int x = 2; x < grayscaleImage.Width - 2; x++)
            {
                for (int y = 1; y < grayscaleImage.Height - 1; y++)
                {
                    int[] sorted = new int[] {                       
                        Gray[x - 2, y - 1], Gray[x - 1, y - 1], Gray[x, y - 1], Gray[x + 1, y - 1],Gray[x + 2, y - 1],
                        Gray[x - 2, y],     Gray[x - 1, y],     Gray[x, y],     Gray[x + 1, y],    Gray[x + 2, y],
                        Gray[x - 2, y + 1], Gray[x - 1, y + 1], Gray[x, y + 1], Gray[x + 1, y + 1],Gray[x + 2, y + 1]                      
                    };
                    Array.Sort(sorted);
                    pix = sorted[sorted.Length / 2];
                    Histogram[pix]++;
                    bmp.SetPixel(x, y, Color.FromArgb(pix, pix, pix));
                }
            }
            return bmp;
        }

        /// <summary>
        /// Kompresja Huffmana z http://rosettacode.org/wiki/Rosetta_Code
        /// </summary>
        /// <returns></returns>
        public static string CompressionHuffman(int x)
        {
            
            Huffman<byte> huffman = new Huffman<byte>(Bytemap);

            List<int> encoding = huffman.Encode(Bytemap);
            List<byte> decoding = huffman.Decode(encoding);

            return ShowSK(decoding.Count * x, encoding.Count);
        }
        /// <summary>
        /// Kompresja LZW
        /// </summary>
        /// <returns></returns>
        public static string CompressionLZW()
        {
            string input = Bytemap.GetBinaryString();

            LZWEncoder encoder = new LZWEncoder();
            byte[] output = encoder.EncodeToByteList(input);

            return ShowSK(output.Length, Bytemap.Length);
        }
        /// <summary>
        /// algorytm żółwia
        /// </summary>
        /// <returns></returns>
        public static Bitmap Turtle()
        {
            Bitmap bmp = grayscaleImage;
            for (int y = 0; y < grayscaleImage.Height; y++)
            {
                for (int x = 0; x < grayscaleImage.Width-1; x++)
                {
                    if (Gray[x, y] < Gray[x+1,y])
                    {
                        bmp.SetPixel(x + 1, y, Color.FromArgb(255, 0, 0));
                    }
                    if (Gray[x, y] > Gray[x + 1, y])
                    {
                        bmp.SetPixel(x, y, Color.FromArgb(255, 0, 0));
                    }
                }
            }
            for (int x = 0; x < grayscaleImage.Width; x++)
            {
                for (int y = 0; y < grayscaleImage.Height - 1; y++)
                {
                    if (Gray[x, y] < Gray[x, y+1])
                    {
                        bmp.SetPixel(x, y+1, Color.FromArgb(255, 0, 0));
                    }
                    if (Gray[x, y] > Gray[x, y+1])
                    {
                        bmp.SetPixel(x, y, Color.FromArgb(255, 0, 0));
                    }
                }
            }
            return bmp;
        }        
        /// <summary>
        /// Wyrysowanie kontrolki dla CW2
        /// </summary>
        /// <param name="BNUD1"></param>
        /// <param name="BNUD2"></param>
        /// <returns></returns>
        public static Bitmap DrawControllforStretchingOPerator(int BNUD1,int BNUD2)
        {
            Graphics graphics = Graphics.FromImage(controlImageSO);

            Pen myPen = new Pen(Brushes.Black, 1);

            Pen myPenR = new Pen(Brushes.Red, 2);

            graphics.FillRectangle(Brushes.White, 0, 0, 255, 255);

            for (int i = 0; i <= 255; i += 51)
            {
                graphics.DrawLine(myPen, 0, i, 255, i);
                graphics.DrawLine(myPen, i, 0, i, 255);
            }

            graphics.DrawLine(myPenR, BNUD1, 255, BNUD2, 0);

            myPen.Dispose();
            myPenR.Dispose();

            graphics.Dispose();

            return controlImageSO;
        }
        /// <summary>
        /// Wyrysowanie kontrolki dla CW2
        /// </summary>
        /// <param name="BNUD1"></param>
        /// <param name="BNUD2"></param>
        /// <param name="BNUD3"></param>
        /// <param name="BNUD4"></param>
        /// <param name="LNUD1"></param>
        /// <param name="LNUD2"></param>
        /// <param name="LNUD3"></param>
        /// <returns></returns>
        public static Bitmap DrawControllforGLR(int BNUD1, int BNUD2, int BNUD3, int BNUD4, int LNUD1, int LNUD2, int LNUD3)
        {
            Graphics graphics = Graphics.FromImage(controlImageGLR);

            Pen myPen = new Pen(Brushes.Black, 1);

            Pen myPenR = new Pen(Brushes.Red, 2);
            Pen myPenG = new Pen(Brushes.Green, 2);

            graphics.FillRectangle(Brushes.White, 0, 0, 255, 255);

            for (int i = 0; i <= 255; i += 51)
            {
                graphics.DrawLine(myPen, 0, i, 255, i);
                graphics.DrawLine(myPen, i, 0, i, 255);
            }

            graphics.DrawLine(myPenR, 0, 255, BNUD1, 255);
            graphics.DrawLine(myPenG, BNUD1, 255 - LNUD1, BNUD2, 255 - LNUD1);
            graphics.DrawLine(myPenR, BNUD2, 255 - LNUD2, BNUD3, 255 - LNUD2);
            graphics.DrawLine(myPenG, BNUD3, 255 - LNUD3, BNUD4, 255 - LNUD3);
            graphics.DrawLine(myPenR, BNUD4, 1, 255, 1);

            myPen.Dispose();
            myPenG.Dispose();
            myPenR.Dispose();

            graphics.Dispose();

            return controlImageGLR;
        }        

        /// <summary>
        /// Zerowanie Histogramu
        /// </summary>
        /// <param name="hist"></param>
        public static void ZeroHistogram(int[] hist)
        {
            for (int i = 0; i < 256; i++)
            {
                hist[i] = 0;
            }
        }
    }
}

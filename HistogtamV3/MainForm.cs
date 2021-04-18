using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HistogtamV3
{
    public partial class MainForm : Form
    {
        //Delegat dla typu skrajnich pikseli
        public delegate void Border(int[,] TAB);
        //Delegat dla metody skalowania
        public delegate Bitmap Scaling(Bitmap image);
        private string Path;
        public MainForm()
        {
            InitializeComponent();
            //Fullscreen
            this.WindowState = FormWindowState.Maximized;
            //Obrazek dla kontrolki
            PBControllGLevelReduction.Image = Body.DrawControllforGLR((int)BNUD1.Value,
                                                                      (int)BNUD2.Value,
                                                                      (int)BNUD3.Value,
                                                                      (int)BNUD4.Value,
                                                                      (int)LNUD1.Value,
                                                                      (int)LNUD2.Value,
                                                                      (int)LNUD3.Value);
            //Obrazek dla kontrolki
            PBControllStretchingOperator.Image = Body.DrawControllforStretchingOPerator((int)NUD1.Value, (int)NUD2.Value);
            

        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Otwarcie Obrazku
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";
                dlg.Filter = "Image Files(*.BMP;*.JPG;*.PNG;*.TIF)|*.BMP;*.JPG;*.PNG;*.TIF";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    //Wstawianie obrazku
                    Body.originalImage = new Bitmap(dlg.FileName);
                    Path = dlg.FileName;
                    PBMainPicture.Image = Body.originalImage;
                    Body.grayscaleImage = Body.originalImage;
                    //Inicjalizacja rozmiaru tabeli z szarymi wartościami
                    Body.Gray = new int[PBMainPicture.Image.Width, PBMainPicture.Image.Height];
                    //Inicjalizacja tabel z wartościami
                    Body.FillBodyData();
                    //Rysowanie Histogramu
                    DrawHistogram(Body.Histogram);
                }
            }
            
        }
        /// <summary>
        /// Rysowanie Histogramu
        /// </summary>
        /// <param name="Histogram"></param>
        public void DrawHistogram(int[] Histogram)
        {
            chartHistogram.Series["GrayLevel"].Points.DataBindY(Histogram);
        }
        /// <summary>
        /// Sprawdzenie wartości dla Numerical UP Down na Kontrolce
        /// </summary>
        private void CheckGLR()
        {
            if (BNUD2.Value <= BNUD1.Value) BNUD2.Value++;
            else if (BNUD3.Value <= BNUD2.Value) BNUD3.Value++;
            else if (BNUD4.Value <= BNUD3.Value) BNUD4.Value++;
            else if (LNUD2.Value <= LNUD1.Value) LNUD2.Value++;
            else if (LNUD3.Value <= LNUD2.Value) LNUD3.Value++;
            else PBControllGLevelReduction.Image = Body.DrawControllforGLR((int)BNUD1.Value,
                                                                     (int)BNUD2.Value,
                                                                     (int)BNUD3.Value,
                                                                     (int)BNUD4.Value,
                                                                     (int)LNUD1.Value,
                                                                     (int)LNUD2.Value,
                                                                     (int)LNUD3.Value);
        }
        /// <summary>
        /// Sprawdzenie wartości dla Numerical UP Down na Kontrolce
        /// </summary>
        void CheckSO()
        {
            if (BNUD2.Value <= BNUD1.Value) BNUD2.Value++;
            else PBControllStretchingOperator.Image = Body.DrawControllforStretchingOPerator((int)NUD1.Value, (int)NUD2.Value);
        }

        private void BtnStretching_Click(object sender, EventArgs e)
        {
            Body.Stretching();
            PBMainPicture.Image = Body.grayscaleImage;
            DrawHistogram(Body.Histogram);
        }

        private void BtnEqualization_Click(object sender, EventArgs e)
        {
            Body.Equalization();
            PBMainPicture.Image = Body.grayscaleImage;
            DrawHistogram(Body.Histogram);
        }

        private void BtnNegative_Click(object sender, EventArgs e)
        {
            Body.MakeNegative();
            PBMainPicture.Image = Body.grayscaleImage;
            DrawHistogram(Body.Histogram);
        }

        private void TBbin_Scroll(object sender, EventArgs e)
        {
            Body.Binaryzation(TBbin.Value);
            PBMainPicture.Image = Body.grayscaleImage;
            DrawHistogram(Body.Histogram);
        }

        private void TBSBin1_Scroll(object sender, EventArgs e)
        {
            if (TBSBin1.Value >= TBSBin2.Value)
            {
                TBSBin2.Value = TBSBin1.Value;
            }
            Body.SelectBinaryzation(TBSBin1.Value, TBSBin2.Value);
            PBMainPicture.Image = Body.grayscaleImage;
            DrawHistogram(Body.Histogram);
        }

        private void TBSBin2_Scroll(object sender, EventArgs e)
        {
            if (TBSBin2.Value <= TBSBin1.Value)
            {
                TBSBin1.Value = TBSBin2.Value;
            }
            Body.SelectBinaryzation(TBSBin1.Value, TBSBin2.Value);
            PBMainPicture.Image = Body.grayscaleImage;
            DrawHistogram(Body.Histogram);
        }

        private void BtnGLevelReduction_Click(object sender, EventArgs e)
        {
            PBMainPicture.Image = Body.Reduction((int)BNUD1.Value, 
                                                 (int)BNUD2.Value, 
                                                 (int)BNUD3.Value, 
                                                 (int)BNUD4.Value, 
                                                 (int)LNUD1.Value, 
                                                 (int)LNUD2.Value, 
                                                 (int)LNUD3.Value);
            DrawHistogram(Body.Histogram);
        }

        private void LNUD3_ValueChanged(object sender, EventArgs e) => CheckGLR();

        private void LNUD2_ValueChanged(object sender, EventArgs e) => CheckGLR();

        private void LNUD1_ValueChanged(object sender, EventArgs e) => CheckGLR();

        private void BNUD1_ValueChanged(object sender, EventArgs e) => CheckGLR();

        private void BNUD2_ValueChanged(object sender, EventArgs e) => CheckGLR();

        private void BNUD3_ValueChanged(object sender, EventArgs e) => CheckGLR();

        private void BNUD4_ValueChanged(object sender, EventArgs e) => CheckGLR();

        private void BtnStretchingOperator_Click(object sender, EventArgs e)
        {
            Body.StretchingOperator((int)NUD1.Value, (int)NUD2.Value);
            PBMainPicture.Image = Body.grayscaleImage;
            DrawHistogram(Body.Histogram);
        }

        private void NUD1_ValueChanged(object sender, EventArgs e) => CheckSO();

        private void NUD2_ValueChanged(object sender, EventArgs e) => CheckSO();
        /// <summary>
        /// Otwarcie pierwszego brazku dla operacij
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnChoose1st_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";
                dlg.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Body.FillGrayA(new Bitmap(dlg.FileName));
                    TB1st.Text = dlg.FileName;
                }
            }
        }
        /// <summary>
        /// Otwarcie drugiego brazku dla operacij
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnChoose2nd_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";
                dlg.Filter = "Image Files(*.BMP;*.JPG;*.PNG)|*.BMP;*.JPG;*.PNG";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Body.FillGrayB(new Bitmap(dlg.FileName));
                    TB2nd.Text = dlg.FileName;
                }
            }
        }

        private void BtnADD_Click(object sender, EventArgs e)
        {
            Arithmetics arithmetics = new Arithmetics(Body.GrayA, Body.GrayB);
            PBMainPicture.Image = arithmetics.Addition();
        }

        private void BtnSUB_Click(object sender, EventArgs e)
        {
            Arithmetics arithmetics = new Arithmetics(Body.GrayA, Body.GrayB);
            PBMainPicture.Image = arithmetics.Subtraction();
        }

        private void BtnDIF_Click(object sender, EventArgs e)
        {
            Arithmetics arithmetics = new Arithmetics(Body.GrayA, Body.GrayB);
            PBMainPicture.Image = arithmetics.Difference();
        }

        private void BtnAND_Click(object sender, EventArgs e)
        {
            Arithmetics arithmetics = new Arithmetics(Body.GrayA, Body.GrayB);
            PBMainPicture.Image = arithmetics.AND();
        }

        private void BtnOR_Click(object sender, EventArgs e)
        {
            Arithmetics arithmetics = new Arithmetics(Body.GrayA, Body.GrayB);
            PBMainPicture.Image = arithmetics.OR();
        }

        private void BtnXOR_Click(object sender, EventArgs e)
        {
            Arithmetics arithmetics = new Arithmetics(Body.GrayA, Body.GrayB);
            PBMainPicture.Image = arithmetics.XOR();
        }

        private void BtnBlur9_Click(object sender, EventArgs e)
        {
            PBMainPicture.Image = Body.Blur(Masks.OneOverNine);
            DrawHistogram(Body.Histogram);
        }

        private void BtnBlur10_Click(object sender, EventArgs e)
        {
            PBMainPicture.Image = Body.Blur(Masks.OneOverTen);
            DrawHistogram(Body.Histogram);
        }

        private void BtnBlur16_Click(object sender, EventArgs e)
        {
            PBMainPicture.Image = Body.Blur(Masks.OneOverSixteen);
            DrawHistogram(Body.Histogram);
        }

        private void BtnSharpBasic_Click(object sender, EventArgs e)
        {
            PBMainPicture.Image = Body.Sharp(Masks.LaplacianBasic);
            DrawHistogram(Body.Histogram);
        }

        private void BtnSharpA_Click(object sender, EventArgs e)
        {
            PBMainPicture.Image = Body.Sharp(Masks.LaplacianA);
            DrawHistogram(Body.Histogram);
        }

        private void BtnSharpB_Click(object sender, EventArgs e)
        {
            PBMainPicture.Image = Body.Sharp(Masks.LaplacianB);
            DrawHistogram(Body.Histogram);
        }

        private void BtnSharpC_Click(object sender, EventArgs e)
        {
            PBMainPicture.Image = Body.Sharp(Masks.LaplacianC);
            DrawHistogram(Body.Histogram);
        }

        private void BtnSharpD_Click(object sender, EventArgs e)
        {
            PBMainPicture.Image = Body.Sharp(Masks.LaplacianD);
            DrawHistogram(Body.Histogram);
        }

        private void BtnEdgeA_Click(object sender, EventArgs e)
        {
            PBMainPicture.Image = Body.EdgeDetection(Masks.EdgeDetectionA);
            DrawHistogram(Body.Histogram);
        }

        private void BtnEdgeB_Click(object sender, EventArgs e)
        {
            PBMainPicture.Image = Body.EdgeDetection(Masks.EdgeDetectionB);
            DrawHistogram(Body.Histogram);
        }

        private void BtnEdgeC_Click(object sender, EventArgs e)
        {
            PBMainPicture.Image = Body.EdgeDetection(Masks.EdgeDetectionC);
            DrawHistogram(Body.Histogram);
        }

        private void BtnDrawCustomMask_Click(object sender, EventArgs e)
        {
            int[,] TAB = new int[,]
            {
                { (int)N1.Value, (int)N2.Value, (int)N3.Value },
                { (int)N4.Value, (int)N5.Value, (int)N6.Value },
                { (int)N7.Value, (int)N8.Value, (int)N9.Value }
            };
            Scaling scaling;
            Border border;
            //Sprawdzanie ustawień
            if (CBScaleA.Checked)
            {
                scaling = Body.ScaleA;
            }
            else if (CBScaleB.Checked)
            {
                scaling = Body.ScaleB;
            }
            else if (CBScaleC.Checked)
            {
                scaling = Body.ScaleC;
            }
            else
            {
                scaling = Body.ScaleC;
            }
            if (CBBorderA.Checked)
            {
                border = Body.BorderPixelsA;
            }
            else if (CBBorderB.Checked)
            {
                border = Body.BorderPixelsB;
            }
            else if (CBBorderC.Checked)
            {
                border = Body.BorderPixelsC;
            }
            else
            {
                border = Body.BorderPixelsB;
            }
            PBMainPicture.Image = Body.CustomMask(border, scaling, TAB);
        }

        private void BtnThining_Click(object sender, EventArgs e)
        {
            PBMainPicture.Image = Body.Thining();
        }

        private void BtnErozion_Click(object sender, EventArgs e)
        {
            if (CBRomb.Checked)
            {
                PBMainPicture.Image = Body.Erozion(Masks.Rhombus);
            }
            else
            {
                PBMainPicture.Image = Body.Erozion(Masks.Square);
            }
            
        }

        private void BtnDilation_Click(object sender, EventArgs e)
        {
            if (CBRomb.Checked)
            {
                PBMainPicture.Image = Body.Dilation(Masks.Rhombus);
            }
            else
            {
                PBMainPicture.Image = Body.Dilation(Masks.Square);
            }
            
        }

        private void BtnOpening_Click(object sender, EventArgs e)
        {
            if (CBRomb.Checked)
            {
                PBMainPicture.Image = Body.Opening(Masks.Rhombus);
            }
            else
            {
                PBMainPicture.Image = Body.Opening(Masks.Square);
            }
                
        }

        private void BtnClosing_Click(object sender, EventArgs e)
        {
            if (CBRomb.Checked)
            {
                PBMainPicture.Image = Body.Closing(Masks.Rhombus);
            }
            else
            {
                PBMainPicture.Image = Body.Closing(Masks.Square);
            }
                
        }

        private void BtnTreshold_Click(object sender, EventArgs e)
        {
            PBMainPicture.Image = Body.Thresh();
        }

        private void BtnWatershed_Click(object sender, EventArgs e)
        {
            PBMainPicture.Image = Body.Watershed();
        }

        private void BtnRLECompression_Click(object sender, EventArgs e)
        {
            label8.Text = Body.ShowSK(Body.Bytemap.Length, Body.CompressionRLE().Length);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            label9.Text = Body.CompressionHuffman(8);
        }

        private void BtnTurtle_Click(object sender, EventArgs e)
        {
            PBTurtle.Image = Body.Turtle();
        }

        private void CBSquare_CheckedChanged(object sender, EventArgs e)
        {
            if (CBRomb.Checked)
            {
                CBRomb.Checked = false;
            }
        }

        private void CBRomb_CheckedChanged(object sender, EventArgs e)
        {
            if (CBSquare.Checked)
            {
                CBSquare.Checked = false;
            }
        }

        private void NUDIndex_ValueChanged(object sender, EventArgs e)
        {
            NUDValue.Value = Body.LUT[(int)NUDIndex.Value];
        }

        private void NUDValue_ValueChanged(object sender, EventArgs e)
        {
            Body.LUT[(int)NUDIndex.Value] = (byte)NUDValue.Value;
        }

        private void BtnLUTDraw_Click(object sender, EventArgs e)
        {
            PBMainPicture.Image = Body.LUTDraw();
            DrawHistogram(Body.Histogram);
        }

        private void BtnMedian3x3_Click(object sender, EventArgs e)
        {
            PBMainPicture.Image = Body.Median3x3();
            DrawHistogram(Body.Histogram);
        }

        private void BtnMedian3x5_Click(object sender, EventArgs e)
        {
            PBMainPicture.Image = Body.Median3x5();
            DrawHistogram(Body.Histogram);
        }

        private void BtnMedian5x3_Click(object sender, EventArgs e)
        {
            PBMainPicture.Image = Body.Median5x3();
            DrawHistogram(Body.Histogram);
        }

        private void BtnMedian5x5_Click(object sender, EventArgs e)
        {
            PBMainPicture.Image = Body.Median5x5();
            DrawHistogram(Body.Histogram);
        }

        private void BtnMedian7x7_Click(object sender, EventArgs e)
        {
            PBMainPicture.Image = Body.Median7x7();
            DrawHistogram(Body.Histogram);
        }

        private void UndoAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PBMainPicture.Image = new Bitmap(Path);
            Body.originalImage = new Bitmap(Path);
            Body.FillBodyData();
            DrawHistogram(Body.Histogram);
            PBMainPicture.Image = Body.grayscaleImage;
            PBMainPicture.Invalidate();
        }
        /// <summary>
        /// Infomacja o autorze
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Aplikacja zbiorcza z ćwiczeń laboratoryjnych" + Environment.NewLine +
                            "Autor: Hryhorii Shynkovskyi" + Environment.NewLine +
                            "Prowadzący: dr inż.Marek Doros" + Environment.NewLine +
                            "Algorytmy Przetwarzania Obrazów 2019" + Environment.NewLine +
                            "Inżynieria oprogramowania grupa ID06P01");
        }

        private void CBScaleA_CheckedChanged(object sender, EventArgs e)
        {
            CBScaleB.Checked = false;
            CBScaleC.Checked = false;
        }

        private void CBScaleB_CheckedChanged(object sender, EventArgs e)
        {
            CBScaleA.Checked = false;
            CBScaleC.Checked = false;
        }

        private void CBScaleC_CheckedChanged(object sender, EventArgs e)
        {
            CBScaleA.Checked = false;
            CBScaleB.Checked = false;
        }

        private void CBBorderA_CheckedChanged(object sender, EventArgs e)
        {
            CBBorderB.Checked = false;
            CBBorderC.Checked = false;
        }

        private void CBBorderB_CheckedChanged(object sender, EventArgs e)
        {
            CBBorderA.Checked = false;
            CBBorderC.Checked = false;
        }

        private void CBBorderC_CheckedChanged(object sender, EventArgs e)
        {
            CBBorderA.Checked = false;
            CBBorderB.Checked = false;
        }

        private void BtnLZW_Click(object sender, EventArgs e)
        {
            label12.Text = Body.CompressionHuffman(7);
        }


        /* private void BtnLZW_Click(object sender, EventArgs e)
         {
             label12.Text = Body.CompressionLZW();
         }*/
    }
}

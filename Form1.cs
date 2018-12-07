using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiceConverter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static Bitmap MakeGrayscale3(Bitmap original)
        {
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);
            Graphics g = Graphics.FromImage(newBitmap);
            ColorMatrix colorMatrix = new ColorMatrix(
               new float[][]
               {
                 new float[] {.3f, .3f, .3f, 0, 0},
                 new float[] {.59f, .59f, .59f, 0, 0},
                 new float[] {.11f, .11f, .11f, 0, 0},
                 new float[] {0, 0, 0, 1, 0},
                 new float[] {0, 0, 0, 0, 1}
               });
            ImageAttributes attributes = new ImageAttributes();
            attributes.SetColorMatrix(colorMatrix);
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
            g.Dispose();
            return newBitmap;
        }

        public double getD(int x)
        {
            return Math.Round((double) x/6, 2);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.RestoreDirectory = true;
            string fileNameNoExt = "";
            string filePath = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var path = openFileDialog1.FileName;
                fileNameNoExt = System.IO.Path.GetFileNameWithoutExtension(path);
                filePath = System.IO.Path.GetDirectoryName(path);
                tbPath.Text = path;
            }
            try
            {
                Bitmap bm = (Bitmap) Bitmap.FromFile(tbPath.Text);
                Bitmap d = MakeGrayscale3(bm);
                pictureBox1.Image = d;
                string diceMap = "";
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath + "\\" + fileNameNoExt + ".txt")) {
                    for (int x = 0; x < d.Width; x++)
                    {
                        for (int y = 0; y < d.Height; y++)
                        {
                            double brightness = Math.Round(d.GetPixel(x, y).GetBrightness(), 2);
                            if (brightness <= Math.Round((double) (1 / 6),2))
                                diceMap += "6";
                            else if (brightness > getD(1) && brightness <= getD(2))
                                diceMap += "5";
                            else if (brightness > getD(2) && brightness <= getD(3))
                                diceMap += "4";
                            else if (brightness > getD(3) && brightness <= getD(4))
                                diceMap += "3";
                            else if (brightness > getD(4) && brightness <= getD(5))
                                diceMap += "2";
                            else if (brightness > getD(5))
                                diceMap += "1";
                            else
                                diceMap += "0"; //shouldn't ever happen.*/
                        }
                        file.WriteLine(diceMap);
                        diceMap = "";
                    }
                    MessageBox.Show("Complete! Dice map written to " + filePath + "\\" + fileNameNoExt + ".txt");
                }
            }
            catch(Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }
    }
}

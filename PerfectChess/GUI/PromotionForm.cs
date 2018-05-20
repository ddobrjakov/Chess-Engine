using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PerfectChess
{
    public partial class PromotionForm : Form
    {
        public PromotionForm(int Color)
        {
            InitializeComponent();

            Bitmap back = new Bitmap(this.Width, this.Height);
            Graphics G = Graphics.FromImage(back);
            LinearGradientBrush backbrush = new LinearGradientBrush(new Point(0, 0), new Point(0, back.Height), System.Drawing.Color.FromArgb(0x2c, 0x2c, 0x2c), System.Drawing.Color.FromArgb(0x1a, 0x1a, 0x1a));
            G.FillRectangle(backbrush, 0, 0, back.Width, back.Height);
            this.BackgroundImage = back;

            this.Color = Color;


            pictureBox1.Width = pictureBox1.Height = ViewSettings.SQUARESIZE;
            pictureBox2.Width = pictureBox2.Height = ViewSettings.SQUARESIZE;
            pictureBox3.Width = pictureBox3.Height = ViewSettings.SQUARESIZE;
            pictureBox4.Width = pictureBox4.Height = ViewSettings.SQUARESIZE;
            SetPiece(pictureBox1, Piece.Knight | Color, PerfectChess.Color.White);
            SetPiece(pictureBox2, Piece.Bishop | Color, PerfectChess.Color.Black);
            SetPiece(pictureBox3, Piece.Rook | Color, PerfectChess.Color.White);
            SetPiece(pictureBox4, Piece.Queen | Color, PerfectChess.Color.Black);
            //pictureBox1.Image = ViewModelConnector.PieceImage[Piece.Knight | Color];
            //pictureBox2.Image = ViewModelConnector.PieceImage[Piece.Bishop | Color];
            //pictureBox3.Image = ViewModelConnector.PieceImage[Piece.Rook | Color];
            //pictureBox4.Image = ViewModelConnector.PieceImage[Piece.Queen | Color];
        }
        private void SetPiece(PictureBox pict, int Piece, int SquareColor)
        {
            Bitmap B = new Bitmap(ViewSettings.SQUARESIZE, ViewSettings.SQUARESIZE);
            Graphics G1 = Graphics.FromImage(B);

            Bitmap SolidColorBMP = new Bitmap(ViewSettings.SQUARESIZE, ViewSettings.SQUARESIZE);
            Graphics G2 = Graphics.FromImage(SolidColorBMP);
            G2.FillRectangle(new SolidBrush(ViewModelConnector.RealColor[SquareColor]), 0, 0, SolidColorBMP.Width, SolidColorBMP.Height);

            G2.DrawImage(ViewModelConnector.PieceImage[Piece], 0, 0, ViewSettings.SQUARESIZE, ViewSettings.SQUARESIZE);
            pict.BackgroundImage = SolidColorBMP;
        }
        private int Color;
        public int PieceChosen { get; private set; } = 0;
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            PieceChosen = Piece.Knight | Color;
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            PieceChosen = Piece.Bishop | Color;
            this.Close();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            PieceChosen = Piece.Rook | Color;
            this.Close();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            PieceChosen = Piece.Queen | Color;
            this.Close();
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }
        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }
        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void pictureBox3_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }
        private void pictureBox3_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void pictureBox4_MouseEnter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Hand;
        }
        private void pictureBox4_MouseLeave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }
    }
}

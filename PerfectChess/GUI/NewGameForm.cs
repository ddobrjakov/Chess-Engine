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
    public partial class NewGameForm : Form
    {
        public NewGameForm()
        {
            InitializeComponent();

            //Bitmap back = new Bitmap(this.Width, this.Height);
            //Graphics G = Graphics.FromImage(back);
            //LinearGradientBrush backbrush = new LinearGradientBrush(new Point(0, 0), new Point(0, back.Height), System.Drawing.Color.FromArgb(0xce, 0xc5, 0xc5), System.Drawing.Color.FromArgb(0xA3, 0x8f, 0x8f));
            //G.FillRectangle(backbrush, 0, 0, back.Width, back.Height);
            //this.BackgroundImage = back;

            //groupBox1.BackColor = System.Drawing.Color.FromArgb(0, System.Drawing.Color.White);
            //groupBox2.BackColor = System.Drawing.Color.FromArgb(0, System.Drawing.Color.White);
            //groupBox1.FlatStyle = FlatStyle.Flat;
            //label1.BackColor = System.Drawing.Color.FromArgb(0, System.Drawing.Color.White);
            //label2.BackColor = System.Drawing.Color.FromArgb(0, System.Drawing.Color.White);

            buttonStart.DialogResult = DialogResult.OK;
            this.AcceptButton = buttonStart;
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void radioButtonWhite_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb.Checked)
            {
                if (rb == radioButtonWhiteHuman) WhiteHuman = true;
                else WhiteHuman = false;
            }
        }
        private void radioButtonBlack_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = sender as RadioButton;
            if (rb.Checked)
            {
                if (rb == radioButtonBlackHuman) BlackHuman = true;
                else BlackHuman = false;
            }
        }

        public bool WhiteHuman { get; private set; } = true;
        public bool BlackHuman { get; private set; } = false;
    }
}

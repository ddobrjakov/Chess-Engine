using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

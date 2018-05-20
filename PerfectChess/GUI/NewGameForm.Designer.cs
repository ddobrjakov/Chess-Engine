namespace PerfectChess
{
    partial class NewGameForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.radioButtonWhiteHuman = new System.Windows.Forms.RadioButton();
            this.radioButtonWhiteComputer = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButtonBlackHuman = new System.Windows.Forms.RadioButton();
            this.radioButtonBlackComputer = new System.Windows.Forms.RadioButton();
            this.buttonStart = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(32, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "White:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(32, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "Black:";
            // 
            // radioButtonWhiteHuman
            // 
            this.radioButtonWhiteHuman.AutoSize = true;
            this.radioButtonWhiteHuman.Checked = true;
            this.radioButtonWhiteHuman.Location = new System.Drawing.Point(10, 18);
            this.radioButtonWhiteHuman.Name = "radioButtonWhiteHuman";
            this.radioButtonWhiteHuman.Size = new System.Drawing.Size(74, 21);
            this.radioButtonWhiteHuman.TabIndex = 2;
            this.radioButtonWhiteHuman.TabStop = true;
            this.radioButtonWhiteHuman.Text = "Human";
            this.radioButtonWhiteHuman.UseVisualStyleBackColor = true;
            this.radioButtonWhiteHuman.CheckedChanged += new System.EventHandler(this.radioButtonWhite_CheckedChanged);
            // 
            // radioButtonWhiteComputer
            // 
            this.radioButtonWhiteComputer.AutoSize = true;
            this.radioButtonWhiteComputer.Location = new System.Drawing.Point(142, 18);
            this.radioButtonWhiteComputer.Name = "radioButtonWhiteComputer";
            this.radioButtonWhiteComputer.Size = new System.Drawing.Size(90, 21);
            this.radioButtonWhiteComputer.TabIndex = 3;
            this.radioButtonWhiteComputer.Text = "Computer";
            this.radioButtonWhiteComputer.UseVisualStyleBackColor = true;
            this.radioButtonWhiteComputer.CheckedChanged += new System.EventHandler(this.radioButtonWhite_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonWhiteHuman);
            this.groupBox1.Controls.Add(this.radioButtonWhiteComputer);
            this.groupBox1.Location = new System.Drawing.Point(128, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(272, 45);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonBlackHuman);
            this.groupBox2.Controls.Add(this.radioButtonBlackComputer);
            this.groupBox2.Location = new System.Drawing.Point(128, 57);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(272, 45);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            // 
            // radioButtonBlackHuman
            // 
            this.radioButtonBlackHuman.AutoSize = true;
            this.radioButtonBlackHuman.Location = new System.Drawing.Point(10, 18);
            this.radioButtonBlackHuman.Name = "radioButtonBlackHuman";
            this.radioButtonBlackHuman.Size = new System.Drawing.Size(74, 21);
            this.radioButtonBlackHuman.TabIndex = 2;
            this.radioButtonBlackHuman.Text = "Human";
            this.radioButtonBlackHuman.UseVisualStyleBackColor = true;
            this.radioButtonBlackHuman.CheckedChanged += new System.EventHandler(this.radioButtonBlack_CheckedChanged);
            // 
            // radioButtonBlackComputer
            // 
            this.radioButtonBlackComputer.AutoSize = true;
            this.radioButtonBlackComputer.Checked = true;
            this.radioButtonBlackComputer.Location = new System.Drawing.Point(142, 18);
            this.radioButtonBlackComputer.Name = "radioButtonBlackComputer";
            this.radioButtonBlackComputer.Size = new System.Drawing.Size(90, 21);
            this.radioButtonBlackComputer.TabIndex = 3;
            this.radioButtonBlackComputer.TabStop = true;
            this.radioButtonBlackComputer.Text = "Computer";
            this.radioButtonBlackComputer.UseVisualStyleBackColor = true;
            this.radioButtonBlackComputer.CheckedChanged += new System.EventHandler(this.radioButtonBlack_CheckedChanged);
            // 
            // buttonStart
            // 
            this.buttonStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonStart.Location = new System.Drawing.Point(204, 108);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(130, 36);
            this.buttonStart.TabIndex = 6;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // NewGameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 156);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "NewGameForm";
            this.Text = "Start new game";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton radioButtonWhiteHuman;
        private System.Windows.Forms.RadioButton radioButtonWhiteComputer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButtonBlackHuman;
        private System.Windows.Forms.RadioButton radioButtonBlackComputer;
        private System.Windows.Forms.Button buttonStart;
    }
}
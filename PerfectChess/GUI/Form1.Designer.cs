namespace PerfectChess
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.TestOutput = new System.Windows.Forms.RichTextBox();
            this.buttonUndo = new System.Windows.Forms.Button();
            this.newGameButton = new System.Windows.Forms.Button();
            this.buttonFlip = new System.Windows.Forms.Button();
            this.Material1 = new System.Windows.Forms.Label();
            this.Material2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TestOutput
            // 
            this.TestOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.TestOutput.Location = new System.Drawing.Point(809, 144);
            this.TestOutput.Name = "TestOutput";
            this.TestOutput.ReadOnly = true;
            this.TestOutput.Size = new System.Drawing.Size(461, 461);
            this.TestOutput.TabIndex = 0;
            this.TestOutput.Text = "";
            // 
            // buttonUndo
            // 
            this.buttonUndo.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.buttonUndo.Location = new System.Drawing.Point(809, 55);
            this.buttonUndo.Name = "buttonUndo";
            this.buttonUndo.Size = new System.Drawing.Size(205, 55);
            this.buttonUndo.TabIndex = 1;
            this.buttonUndo.Text = "Undo Move";
            this.buttonUndo.UseVisualStyleBackColor = true;
            this.buttonUndo.Click += new System.EventHandler(this.undoButton_Click);
            this.buttonUndo.MouseEnter += new System.EventHandler(this.buttonUndo_MouseEnter);
            this.buttonUndo.MouseLeave += new System.EventHandler(this.buttonUndo_MouseLeave);
            // 
            // newGameButton
            // 
            this.newGameButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.newGameButton.Location = new System.Drawing.Point(1052, 55);
            this.newGameButton.Name = "newGameButton";
            this.newGameButton.Size = new System.Drawing.Size(218, 55);
            this.newGameButton.TabIndex = 2;
            this.newGameButton.Text = "New Game";
            this.newGameButton.UseVisualStyleBackColor = true;
            this.newGameButton.Click += new System.EventHandler(this.newGameButton_Click);
            this.newGameButton.MouseEnter += new System.EventHandler(this.newGameButton_MouseEnter);
            this.newGameButton.MouseLeave += new System.EventHandler(this.newGameButton_MouseLeave);
            // 
            // buttonFlip
            // 
            this.buttonFlip.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonFlip.Location = new System.Drawing.Point(63, 674);
            this.buttonFlip.Name = "buttonFlip";
            this.buttonFlip.Size = new System.Drawing.Size(143, 33);
            this.buttonFlip.TabIndex = 3;
            this.buttonFlip.Text = "Flip the board";
            this.buttonFlip.UseVisualStyleBackColor = true;
            this.buttonFlip.Click += new System.EventHandler(this.buttonFlip_Click);
            this.buttonFlip.MouseEnter += new System.EventHandler(this.buttonFlip_MouseEnter);
            this.buttonFlip.MouseLeave += new System.EventHandler(this.buttonFlip_MouseLeave);
            // 
            // Material1
            // 
            this.Material1.AutoSize = true;
            this.Material1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.Material1.ForeColor = System.Drawing.Color.White;
            this.Material1.Location = new System.Drawing.Point(604, 674);
            this.Material1.Name = "Material1";
            this.Material1.Size = new System.Drawing.Size(23, 25);
            this.Material1.TabIndex = 4;
            this.Material1.Text = "0";
            // 
            // Material2
            // 
            this.Material2.AutoSize = true;
            this.Material2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.Material2.ForeColor = System.Drawing.Color.White;
            this.Material2.Location = new System.Drawing.Point(604, 25);
            this.Material2.Name = "Material2";
            this.Material2.Size = new System.Drawing.Size(23, 25);
            this.Material2.TabIndex = 5;
            this.Material2.Text = "0";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1436, 744);
            this.Controls.Add(this.Material2);
            this.Controls.Add(this.Material1);
            this.Controls.Add(this.buttonFlip);
            this.Controls.Add(this.newGameButton);
            this.Controls.Add(this.buttonUndo);
            this.Controls.Add(this.TestOutput);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "PerfectChess Engine";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox TestOutput;
        private System.Windows.Forms.Button buttonUndo;
        private System.Windows.Forms.Button newGameButton;
        private System.Windows.Forms.Button buttonFlip;
        private System.Windows.Forms.Label Material1;
        private System.Windows.Forms.Label Material2;
    }
}


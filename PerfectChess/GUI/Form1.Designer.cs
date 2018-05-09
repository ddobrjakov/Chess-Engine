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
            this.TestOutput = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
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
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(809, 55);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(282, 55);
            this.button1.TabIndex = 1;
            this.button1.Text = "Отменить ход";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.undoButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1436, 744);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.TestOutput);
            this.Name = "Form1";
            this.Text = "PerfectChess Engine";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox TestOutput;
        private System.Windows.Forms.Button button1;
    }
}


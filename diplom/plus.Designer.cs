namespace diplom
{
    partial class plus
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(plus));
            panel1 = new Panel();
            zField = new TextBox();
            pictureBox1 = new PictureBox();
            pictureBox2 = new PictureBox();
            buttonTask = new Button();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(zField);
            panel1.Location = new Point(12, 26);
            panel1.Name = "panel1";
            panel1.Size = new Size(515, 65);
            panel1.TabIndex = 0;
            // 
            // zField
            // 
            zField.Location = new Point(3, 19);
            zField.Name = "zField";
            zField.Size = new Size(509, 27);
            zField.TabIndex = 0;
            // 
            // pictureBox1
            // 
            pictureBox1.Cursor = Cursors.Hand;
            pictureBox1.Image = Properties.Resources.категория;
            pictureBox1.Location = new Point(15, 106);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(65, 63);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Cursor = Cursors.Hand;
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(86, 106);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(98, 63);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 2;
            pictureBox2.TabStop = false;
            // 
            // buttonTask
            // 
            buttonTask.BackColor = SystemColors.ButtonHighlight;
            buttonTask.Cursor = Cursors.Hand;
            buttonTask.FlatStyle = FlatStyle.Flat;
            buttonTask.Font = new Font("Franklin Gothic Medium", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 204);
            buttonTask.Location = new Point(253, 377);
            buttonTask.Name = "buttonTask";
            buttonTask.Size = new Size(357, 46);
            buttonTask.TabIndex = 5;
            buttonTask.Text = "Внести задачу";
            buttonTask.UseVisualStyleBackColor = false;
            buttonTask.Click += buttonTask_Click;
            // 
            // plus
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(860, 469);
            Controls.Add(buttonTask);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox1);
            Controls.Add(panel1);
            Name = "plus";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "plus";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private TextBox zField;
        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private Button buttonTask;
    }
}
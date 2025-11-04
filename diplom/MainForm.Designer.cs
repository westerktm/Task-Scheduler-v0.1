namespace diplom
{
    partial class MainForm
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
            panel1 = new Panel();
            pictureBox4 = new PictureBox();
            label2 = new Label();
            tasksPanel = new Panel();
            pictureBoxToggle = new PictureBox();
            labelToday = new Label();
            pictureBox3 = new PictureBox();
            pictureBox2 = new PictureBox();
            pictureBox1 = new PictureBox();
            label1 = new Label();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxToggle).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ButtonFace;
            panel1.Controls.Add(pictureBox4);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(tasksPanel);
            panel1.Controls.Add(pictureBoxToggle);
            panel1.Controls.Add(labelToday);
            panel1.Controls.Add(pictureBox3);
            panel1.Controls.Add(pictureBox2);
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1140, 546);
            panel1.TabIndex = 1;
            // 
            // pictureBox4
            // 
            pictureBox4.Cursor = Cursors.Hand;
            pictureBox4.Image = Properties.Resources.треугольник;
            pictureBox4.Location = new Point(257, 216);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(24, 24);
            pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox4.TabIndex = 9;
            pictureBox4.TabStop = false;
            pictureBox4.Click += pictureBox4_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Franklin Gothic Medium", 16F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label2.Location = new Point(55, 206);
            label2.Name = "label2";
            label2.Size = new Size(209, 34);
            label2.TabIndex = 8;
            label2.Text = "На этой неделе";
            // 
            // tasksPanel
            // 
            tasksPanel.BackColor = Color.Transparent;
            tasksPanel.Location = new Point(52, 143);
            tasksPanel.Name = "tasksPanel";
            tasksPanel.Size = new Size(850, 0);
            tasksPanel.TabIndex = 6;
            tasksPanel.Visible = false;
            // 
            // pictureBoxToggle
            // 
            pictureBoxToggle.Cursor = Cursors.Hand;
            pictureBoxToggle.Image = Properties.Resources.треугольник;
            pictureBoxToggle.Location = new Point(164, 106);
            pictureBoxToggle.Name = "pictureBoxToggle";
            pictureBoxToggle.Size = new Size(24, 24);
            pictureBoxToggle.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxToggle.TabIndex = 5;
            pictureBoxToggle.TabStop = false;
            pictureBoxToggle.Click += pictureBoxToggle_Click;
            // 
            // labelToday
            // 
            labelToday.AutoSize = true;
            labelToday.Font = new Font("Franklin Gothic Medium", 16F, FontStyle.Bold, GraphicsUnit.Point, 204);
            labelToday.Location = new Point(52, 96);
            labelToday.Name = "labelToday";
            labelToday.Size = new Size(118, 34);
            labelToday.TabIndex = 4;
            labelToday.Text = "Сегодня";
            // 
            // pictureBox3
            // 
            pictureBox3.Image = Properties.Resources.три_полоски;
            pictureBox3.Location = new Point(1039, 155);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(75, 62);
            pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox3.TabIndex = 3;
            pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            pictureBox2.Cursor = Cursors.Hand;
            pictureBox2.Image = Properties.Resources.avatar;
            pictureBox2.Location = new Point(1039, 87);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(75, 62);
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 2;
            pictureBox2.TabStop = false;
            pictureBox2.Click += pictureBox2_Click;
            // 
            // pictureBox1
            // 
            pictureBox1.Cursor = Cursors.Hand;
            pictureBox1.Image = Properties.Resources.плюсик;
            pictureBox1.Location = new Point(1014, 19);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(126, 62);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Franklin Gothic Medium", 18F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label1.Location = new Point(12, 19);
            label1.Name = "label1";
            label1.Size = new Size(371, 38);
            label1.TabIndex = 0;
            label1.Text = "Планировщик задач v0.1";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1140, 546);
            Controls.Add(panel1);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MainForm";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxToggle).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Panel panel1;
        private Label label1;
        private PictureBox pictureBox3;
        private PictureBox pictureBox2;
        private PictureBox pictureBox1;
        private Label labelToday;
        private PictureBox pictureBoxToggle;
        private Panel tasksPanel;
        private ListBox listBox1;
        private PictureBox pictureBox4;
        private Label label2;
        private ListBox listBox3;
        private ListBox listBox2;
    }
}
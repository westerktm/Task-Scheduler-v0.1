namespace diplom
{
    partial class RegisterForm
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
            labelLogin = new Label();
            passField2 = new TextBox();
            buttonRegister = new Button();
            passField = new TextBox();
            logField = new TextBox();
            pictureBox1 = new PictureBox();
            label1 = new Label();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ButtonFace;
            panel1.Controls.Add(labelLogin);
            panel1.Controls.Add(passField2);
            panel1.Controls.Add(buttonRegister);
            panel1.Controls.Add(passField);
            panel1.Controls.Add(logField);
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1140, 546);
            panel1.TabIndex = 1;
            // 
            // labelLogin
            // 
            labelLogin.AutoSize = true;
            labelLogin.Cursor = Cursors.Hand;
            labelLogin.Font = new Font("Segoe UI", 12F);
            labelLogin.Location = new Point(466, 492);
            labelLogin.Name = "labelLogin";
            labelLogin.Size = new Size(234, 28);
            labelLogin.TabIndex = 6;
            labelLogin.Text = "Уже есть аккаунт? Войти";
            labelLogin.UseMnemonic = false;
            labelLogin.Click += labelLogin_Click;
            // 
            // passField2
            // 
            passField2.Location = new Point(312, 369);
            passField2.Name = "passField2";
            passField2.Size = new Size(525, 27);
            passField2.TabIndex = 5;
            passField2.UseSystemPasswordChar = true;
            // 
            // buttonRegister
            // 
            buttonRegister.BackColor = SystemColors.ButtonHighlight;
            buttonRegister.Cursor = Cursors.Hand;
            buttonRegister.FlatStyle = FlatStyle.Flat;
            buttonRegister.Font = new Font("Franklin Gothic Medium", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 204);
            buttonRegister.Location = new Point(399, 430);
            buttonRegister.Name = "buttonRegister";
            buttonRegister.Size = new Size(357, 46);
            buttonRegister.TabIndex = 4;
            buttonRegister.Text = "Зарегистрироваться";
            buttonRegister.UseVisualStyleBackColor = false;
            buttonRegister.Click += buttonRegister_Click;
            // 
            // passField
            // 
            passField.Location = new Point(312, 314);
            passField.Name = "passField";
            passField.Size = new Size(525, 27);
            passField.TabIndex = 3;
            passField.UseSystemPasswordChar = true;
            // 
            // logField
            // 
            logField.Location = new Point(312, 259);
            logField.Multiline = true;
            logField.Name = "logField";
            logField.Size = new Size(525, 34);
            logField.TabIndex = 2;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.avatar;
            pictureBox1.Location = new Point(497, 77);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(152, 152);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Franklin Gothic Medium", 24F, FontStyle.Bold, GraphicsUnit.Point, 204);
            label1.Location = new Point(444, 9);
            label1.Name = "label1";
            label1.Size = new Size(256, 47);
            label1.TabIndex = 0;
            label1.Text = "Регистрация";
            label1.TextAlign = ContentAlignment.TopCenter;
            label1.Click += label1_Click;
            // 
            // RegisterForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1140, 546);
            Controls.Add(panel1);
            Name = "RegisterForm";
            Text = "RegisterForm";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Button buttonRegister;
        private TextBox passField;
        private TextBox logField;
        private PictureBox pictureBox1;
        private Label label1;
        private TextBox passField2;
        private Label labelLogin;
    }
}
namespace diplom
{
    partial class LoginForm
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
            buttonLogin = new Button();
            passField = new TextBox();
            loginField = new TextBox();
            pictureBox1 = new PictureBox();
            label1 = new Label();
            registerLabel = new Label();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = SystemColors.ButtonFace;
            panel1.Controls.Add(registerLabel);
            panel1.Controls.Add(buttonLogin);
            panel1.Controls.Add(passField);
            panel1.Controls.Add(loginField);
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(1140, 546);
            panel1.TabIndex = 0;
            // 
            // buttonLogin
            // 
            buttonLogin.BackColor = SystemColors.ButtonHighlight;
            buttonLogin.Cursor = Cursors.Hand;
            buttonLogin.FlatStyle = FlatStyle.Flat;
            buttonLogin.Font = new Font("Franklin Gothic Medium", 16.2F, FontStyle.Regular, GraphicsUnit.Point, 204);
            buttonLogin.Location = new Point(387, 410);
            buttonLogin.Name = "buttonLogin";
            buttonLogin.Size = new Size(357, 46);
            buttonLogin.TabIndex = 4;
            buttonLogin.Text = "Войти";
            buttonLogin.UseVisualStyleBackColor = false;
            buttonLogin.Click += buttonLogin_Click;
            // 
            // passField
            // 
            passField.Location = new Point(303, 306);
            passField.Name = "passField";
            passField.Size = new Size(525, 27);
            passField.TabIndex = 3;
            passField.UseSystemPasswordChar = true;
            // 
            // loginField
            // 
            loginField.Location = new Point(303, 251);
            loginField.Multiline = true;
            loginField.Name = "loginField";
            loginField.Size = new Size(525, 34);
            loginField.TabIndex = 2;
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
            label1.Size = new Size(262, 47);
            label1.TabIndex = 0;
            label1.Text = "Авторизация";
            label1.TextAlign = ContentAlignment.TopCenter;
            label1.Click += label1_Click;
            // 
            // registerLabel
            // 
            registerLabel.AutoSize = true;
            registerLabel.Font = new Font("Segoe UI", 12F);
            registerLabel.Location = new Point(444, 472);
            registerLabel.Name = "registerLabel";
            registerLabel.Size = new Size(262, 28);
            registerLabel.TabIndex = 5;
            registerLabel.Text = "Еще не зарегистрированы?";
            registerLabel.Click += label2_Click;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ButtonFace;
            ClientSize = new Size(1140, 546);
            Controls.Add(panel1);
            Name = "LoginForm";
            Text = "LoginForm";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label label1;
        private PictureBox pictureBox1;
        private Button buttonLogin;
        private TextBox passField;
        private TextBox loginField;
        private Label registerLabel;
    }
}
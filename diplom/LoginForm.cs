using MySqlConnector;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace diplom
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            this.passField.AutoSize = false;
            this.passField.Size = new Size(this.passField.Size.Width, 34);

            loginField.PlaceholderText = "Введите логин";
            passField.PlaceholderText = "Введите пароль";

            // Adaptive layout
            ConfigureAnchorsAndResize();
            this.Resize += (_, __) => ApplyLoginLayout();
            ApplyLoginLayout();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            String loginUser = loginField.Text?.Trim() ?? string.Empty;
            String passUser = passField.Text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(loginUser))
            {
                MessageBox.Show("Вы не ввели логин!");
                return;
            }
            if (string.IsNullOrEmpty(passUser))
            {
                MessageBox.Show("Вы не ввели пароль!");
                return;
            }

            string passHash = SecurityUtils.ComputeSha256(passUser);

            DB db = new DB();

            DataTable table = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand("SELECT * FROM users WHERE login = @uL AND password = @uP", db.getConnection());
            command.Parameters.Add("@uL", MySqlDbType.VarChar).Value = loginUser;
            command.Parameters.Add("@uP", MySqlDbType.VarChar).Value = passHash;

            adapter.SelectCommand = command;
            db.openConnection();
            adapter.Fill(table);
            db.closeConnection();

            if (table.Rows.Count > 0)
            {
                MessageBox.Show("Вход выполнен");
                this.Hide();
                int userId = Convert.ToInt32(table.Rows[0]["id"]);
                string login = Convert.ToString(table.Rows[0]["login"]) ?? string.Empty;
                MainForm mainForm = new MainForm(userId, login);
                mainForm.Show();
            }
            else
                MessageBox.Show("Неверный логин или пароль");
            


        }

        private void ConfigureAnchorsAndResize()
        {
            // Stretch inputs with form width
            loginField.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            passField.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            buttonLogin.Anchor = AnchorStyles.Top;
            registerLabel.Anchor = AnchorStyles.Top;
            pictureBox1.Anchor = AnchorStyles.Top;
            label1.Anchor = AnchorStyles.Top;
        }

        private void ApplyLoginLayout()
        {
            // Center horizontally for key controls
            int formWidth = this.ClientSize.Width;
            void center(Control c)
            {
                if (c == null) return;
                c.Left = Math.Max(12, (formWidth - c.Width) / 2);
            }
            center(label1);
            center(pictureBox1);
            center(loginField);
            center(passField);
            center(buttonLogin);
            center(registerLabel);
        }

        private void label2_Click(object sender, EventArgs e)
        {
            this.Hide();
            RegisterForm registerForm = new RegisterForm();
            registerForm.Show();
        }
    }
}


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
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();

            this.passField.AutoSize = false;
            this.passField.Size = new Size(this.passField.Size.Width, 34);

            this.passField2.AutoSize = false;
            this.passField2.Size = new Size(this.passField.Size.Width, 34);


            logField.PlaceholderText = "Введите логин";
            passField.PlaceholderText = "Введите пароль";
            passField2.PlaceholderText = "Повторите пароль";

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            string loginInput = logField.Text?.Trim() ?? string.Empty;
            string passwordInput = passField.Text ?? string.Empty;
            string passwordRepeatInput = passField2.Text ?? string.Empty;

            string errorMessage;
            if (!SecurityUtils.IsValidLogin(loginInput, out errorMessage))
            {
                MessageBox.Show(errorMessage);
                return;
            }
            if (!SecurityUtils.IsValidPassword(passwordInput, passwordRepeatInput, out errorMessage))
            {
                MessageBox.Show(errorMessage);
                return;
            }

            // Проверка на существующего пользователя
            if (checkUser())
                return;


            DB db = new DB();

            string passwordHash = SecurityUtils.ComputeSha256(passwordInput);

            MySqlCommand command = new MySqlCommand("INSERT INTO users (login, password) VALUES (@login, @password)", db.getConnection());
            command.Parameters.Add("@login", MySqlDbType.VarChar).Value = loginInput;
            command.Parameters.Add("@password", MySqlDbType.VarChar).Value = passwordHash;

            db.openConnection();

            if (command.ExecuteNonQuery() == 1)
                MessageBox.Show("Аккаунт был создан");
            else
                MessageBox.Show("Аккаунт не был создан");

            db.closeConnection();

        }

        public Boolean checkUser()
        {
            DB db = new DB();

            DataTable table = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter();

            MySqlCommand command = new MySqlCommand("SELECT * FROM users WHERE login = @uL", db.getConnection());
            command.Parameters.Add("@uL", MySqlDbType.VarChar).Value = (logField.Text?.Trim() ?? string.Empty);

            adapter.SelectCommand = command;

            db.openConnection();
            adapter.Fill(table);
            db.closeConnection();

            if (table.Rows.Count > 0)
            {
                MessageBox.Show("Такой логин уже есть, введите другой.");
                return true;
            }

            else
                return false;
        }

        private void labelLogin_Click(object sender, EventArgs e)
        {
            this.Hide();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }

 
    }
}

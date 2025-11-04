using System;
using System.Security.Cryptography;
using System.Text;

namespace diplom
{
    internal static class SecurityUtils
    {
        public static string ComputeSha256(string input)
        {
            if (input == null)
                input = string.Empty;

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                StringBuilder builder = new StringBuilder(hashBytes.Length * 2);
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static bool IsValidLogin(string login, out string error)
        {
            error = string.Empty;
            if (string.IsNullOrWhiteSpace(login))
            {
                error = "Вы не ввели логин!";
                return false;
            }

            string trimmed = login.Trim();
            if (trimmed.Length < 3 || trimmed.Length > 32)
            {
                error = "Длина логина должна быть от 3 до 32 символов.";
                return false;
            }

            foreach (char c in trimmed)
            {
                if (!(char.IsLetterOrDigit(c) || c == '_' || c == '-'))
                {
                    error = "Логин может содержать только буквы, цифры, '_' и '-'.";
                    return false;
                }
            }

            return true;
        }

        public static bool IsValidPassword(string password, string passwordRepeat, out string error)
        {
            error = string.Empty;
            if (string.IsNullOrEmpty(password))
            {
                error = "Вы не ввели пароль!";
                return false;
            }
            if (string.IsNullOrEmpty(passwordRepeat))
            {
                error = "Повторите указанный пароль!";
                return false;
            }
            if (!string.Equals(password, passwordRepeat, StringComparison.Ordinal))
            {
                error = "Пароли не совпадают!";
                return false;
            }

            if (password.Length < 8 || password.Length > 64)
            {
                error = "Длина пароля должна быть от 8 до 64 символов.";
                return false;
            }
            bool hasLetter = false;
            bool hasDigit = false;
            foreach (char c in password)
            {
                if (char.IsLetter(c)) hasLetter = true;
                if (char.IsDigit(c)) hasDigit = true;
                if (char.IsWhiteSpace(c))
                {
                    error = "Пароль не должен содержать пробелы.";
                    return false;
                }
            }
            if (!hasLetter || !hasDigit)
            {
                error = "Пароль должен содержать хотя бы одну букву и одну цифру.";
                return false;
            }

            return true;
        }
    }
}




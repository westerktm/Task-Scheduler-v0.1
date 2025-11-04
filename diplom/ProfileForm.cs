using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace diplom
{
    public partial class ProfileForm : Form
    {
        private MainForm? mainForm;
        private int userId;

        public ProfileForm()
        {
            InitializeComponent();
            LoadAvatar();
        }

        public ProfileForm(MainForm owner) : this()
        {
            mainForm = owner;
            userId = mainForm.UserId;
            LoadAvatar();
        }
        
        private string AvatarPath
        {
            get
            {
                var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "diplom", "avatars");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                return Path.Combine(dir, $"user_{userId}.jpg");
            }
        }
        
        private void LoadAvatar()
        {
            try
            {
                if (userId > 0 && File.Exists(AvatarPath))
                {
                    // Освобождаем старое изображение
                    var oldImage = avatarBox.Image;
                    avatarBox.Image = null;
                    oldImage?.Dispose();
                    
                    // Загружаем новое изображение
                    avatarBox.Image = new Bitmap(AvatarPath);
                }
            }
            catch (Exception ex)
            {
                // Если не удалось загрузить, используем изображение по умолчанию
                // Можно залогировать ошибку для отладки
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки аватарки: {ex.Message}");
            }
        }
        
        private void SaveAvatar(Image image)
        {
            try
            {
                if (userId <= 0)
                {
                    MessageBox.Show("Ошибка: ID пользователя не установлен", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                // Создаем директорию, если её нет
                var dir = Path.GetDirectoryName(AvatarPath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                
                // Создаем копию изображения для сохранения (чтобы избежать блокировки файла)
                using (var bitmap = new Bitmap(image.Width, image.Height))
                {
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.DrawImage(image, 0, 0, image.Width, image.Height);
                    }
                    
                    // Сохраняем изображение в формате JPEG с качеством 90
                    var jpegCodec = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders()
                        .FirstOrDefault(c => c.FormatID == System.Drawing.Imaging.ImageFormat.Jpeg.Guid);
                    
                    if (jpegCodec != null)
                    {
                        var encoderParams = new System.Drawing.Imaging.EncoderParameters(1);
                        encoderParams.Param[0] = new System.Drawing.Imaging.EncoderParameter(
                            System.Drawing.Imaging.Encoder.Quality, 90L);
                        
                        bitmap.Save(AvatarPath, jpegCodec, encoderParams);
                        encoderParams.Dispose();
                    }
                    else
                    {
                        // Если кодек не найден, используем стандартный метод
                        bitmap.Save(AvatarPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                }
                
                // Обновляем аватарку в MainForm, если она есть
                if (mainForm != null && !mainForm.IsDisposed)
                {
                    var pictureBox2 = mainForm.Controls.Find("pictureBox2", true).FirstOrDefault() as PictureBox;
                    if (pictureBox2 != null)
                    {
                        pictureBox2.Image?.Dispose();
                        if (File.Exists(AvatarPath))
                        {
                            pictureBox2.Image = new Bitmap(AvatarPath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении аватарки: {ex.Message}\nПуть: {AvatarPath}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox1_Click(object? sender, EventArgs e)
        {
            // Если есть ссылка на MainForm, используем её
            if (mainForm != null && !mainForm.IsDisposed)
            {
                mainForm.Show();
                this.Hide();
                return;
            }
            
            // Иначе пытаемся получить из Owner
            if (this.Owner is MainForm mf && !mf.IsDisposed)
            {
                mf.Show();
                this.Hide();
                return;
            }
            
            // Попытка найти открытую MainForm среди всех открытых форм
            var existingMainForm = Application.OpenForms.OfType<MainForm>().FirstOrDefault(f => !f.IsDisposed);
            if (existingMainForm != null)
            {
                existingMainForm.Show();
                this.Hide();
                return;
            }
            
            // Если MainForm не найдена, просто закрываем ProfileForm
            this.Close();
        }
        
        private void pictureBox2_Click(object? sender, EventArgs e)
        {
            if (userId <= 0)
            {
                MessageBox.Show("Ошибка: ID пользователя не установлен. Войдите в систему заново.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Изображения (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|Все файлы (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Загружаем выбранное изображение
                        Image? originalImage = null;
                        Bitmap? newImage = null;
                        
                        try
                        {
                            originalImage = Image.FromFile(openFileDialog.FileName);
                            
                            // Создаем копию изображения для PictureBox
                            newImage = new Bitmap(originalImage.Width, originalImage.Height);
                            using (var graphics = Graphics.FromImage(newImage))
                            {
                                graphics.DrawImage(originalImage, 0, 0, originalImage.Width, originalImage.Height);
                            }
                            
                            // Обновляем изображение в avatarBox
                            avatarBox.Image?.Dispose();
                            avatarBox.Image = newImage;
                            
                            // Сохраняем аватарку (создаем еще одну копию для сохранения)
                            SaveAvatar(newImage);
                            
                            MessageBox.Show($"Аватарка успешно сохранена!\nПуть: {AvatarPath}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        finally
                        {
                            originalImage?.Dispose();
                            // newImage не освобождаем, так как он используется в avatarBox
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при загрузке изображения: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}

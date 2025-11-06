using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace diplom
{
    public partial class plus : Form
    {
        private MainForm? mainForm;
        private DateTime? selectedDateTime;

        public plus()
        {
            InitializeComponent();
            CreateRoundedRectangle();
            InitializeCalendarClick();
            buttonTask.Click += buttonTask_Click;

            // Adaptive anchors
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            zField.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            pictureBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            buttonTask.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // Resize handler to keep layout centered
            this.Resize += (_, __) => ApplyPlusLayout();
            ApplyPlusLayout();
        }

        public plus(MainForm owner) : this()
        {
            mainForm = owner;
        }

        private void InitializeCalendarClick()
        {
            // Находим все PictureBox на форме
            var pictureBoxes = Controls.OfType<PictureBox>().ToList();
            
            // Если PictureBox найден, добавляем обработчик клика
            foreach (var pictureBox in pictureBoxes)
            {
                pictureBox.Cursor = Cursors.Hand; // Курсор-указатель при наведении
                pictureBox.Click += CalendarPictureBox_Click;
            }
        }

        private void CalendarPictureBox_Click(object? sender, EventArgs e)
        {
            ShowDateTimePicker();
        }

        private void ShowDateTimePicker()
        {
            // Создаем форму для выбора даты и времени
            using (var dateTimeForm = new Form())
            {
                dateTimeForm.Text = "Выберите дату и время";
                dateTimeForm.Size = new Size(350, 450);
                dateTimeForm.StartPosition = FormStartPosition.CenterParent;
                dateTimeForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                dateTimeForm.MaximizeBox = false;
                dateTimeForm.MinimizeBox = false;

                // Календарь для выбора даты - увеличиваем размер
                var monthCalendar = new MonthCalendar();
                monthCalendar.Location = new Point(15, 15);
                monthCalendar.Font = new Font("Segoe UI", 10, FontStyle.Regular); // Увеличенный шрифт для календаря
                monthCalendar.DateSelected += (s, e) => { };
                monthCalendar.MaxSelectionCount = 1;

                // Метка для времени - сдвигаем вниз
                var timeLabel = new Label();
                timeLabel.Text = "Время:";
                timeLabel.Location = new Point(15, 215);
                timeLabel.Size = new Size(60, 20);
                timeLabel.Font = new Font("Franklin Gothic Medium", 10, FontStyle.Regular);

                // DateTimePicker для выбора времени - сдвигаем вниз
                var timePicker = new DateTimePicker();
                timePicker.Format = DateTimePickerFormat.Time;
                timePicker.ShowUpDown = true;
				timePicker.Location = new Point(110, 230);
                timePicker.Size = new Size(170, 27);
                timePicker.Font = new Font("Franklin Gothic Medium", 12, FontStyle.Regular);

                // Кнопка ОК - сдвигаем вниз
                var btnOk = new Button();
                btnOk.Text = "OK";
                btnOk.DialogResult = DialogResult.OK;
				btnOk.Location = new Point(50, 272);
                btnOk.Size = new Size(80, 30);
                btnOk.Font = new Font("Franklin Gothic Medium", 10, FontStyle.Regular);

                // Кнопка Отмена - сдвигаем вниз
                var btnCancel = new Button();
                btnCancel.Text = "Отмена";
                btnCancel.DialogResult = DialogResult.Cancel;
				btnCancel.Location = new Point(145, 272);
                btnCancel.Size = new Size(80, 30);
                btnCancel.Font = new Font("Franklin Gothic Medium", 10, FontStyle.Regular);

                // Устанавливаем начальные значения
                DateTime selectedDate = DateTime.Now;
                monthCalendar.SelectionStart = selectedDate;
                monthCalendar.SelectionEnd = selectedDate;
                timePicker.Value = selectedDate;

                dateTimeForm.Controls.Add(monthCalendar);
                dateTimeForm.Controls.Add(timeLabel);
                dateTimeForm.Controls.Add(timePicker);
                dateTimeForm.Controls.Add(btnOk);
                dateTimeForm.Controls.Add(btnCancel);
                dateTimeForm.AcceptButton = btnOk;
                dateTimeForm.CancelButton = btnCancel;

                // Показываем диалог
                if (dateTimeForm.ShowDialog(this) == DialogResult.OK)
                {
                    // Объединяем выбранную дату и время
                    selectedDateTime = monthCalendar.SelectionStart.Date + timePicker.Value.TimeOfDay;
                    
                    // Показываем краткое подтверждение выбранного времени в заголовке кнопки
                    pictureBox2.Tag = selectedDateTime;
                }
            }
        }

        private void buttonTask_Click(object? sender, EventArgs e)
        {
            if (mainForm == null)
            {
                // Попробуем получить главное окно из Owner, если передали через Show(this)
                if (this.Owner is MainForm mf) {
                    mainForm = mf;
                }
            }

            var title = zField.Text?.Trim();
            // Если дата/время не установлены, попробуем взять их из Tag календарной иконки
            if (selectedDateTime == null && pictureBox2?.Tag is DateTime dtFromTag)
            {
                selectedDateTime = dtFromTag;
            }
            // Если текст пустой — останавливаемся
            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Укажите текст задачи и выберите дату/время (иконка календаря)",
                    "Недостаточно данных", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Если дата/время по-прежнему не выбраны — используем текущее время по умолчанию
            selectedDateTime ??= DateTime.Now;

            mainForm?.AddTask(title!, selectedDateTime.Value);
            this.Close();
        }

        private void CreateRoundedRectangle()
        {
            // Создаем панель
            panel1.BackColor = Color.LightGray; // Цвет фона
            panel1.Location = new Point(20, 20);
            panel1.Size = new Size(800, 80); // Увеличенный размер
            panel1.Padding = new Padding(15, 15, 15, 15);
            
            // Применяем скругление углов
            ApplyRoundedCorners(panel1, 15); // Увеличенный радиус скругления
            
            // Обработчик изменения размера для обновления скругления
            panel1.Resize += (sender, e) => ApplyRoundedCorners(panel1, 15);
            
            // Создаем поле ввода текста
            zField.BorderStyle = BorderStyle.None; // Убираем рамку
            zField.BackColor = Color.White; // Такой же фон как у панели
            zField.Font = new Font("Franklin Gothic Medium", 16, FontStyle.Regular); // Увеличенный шрифт
            zField.Dock = DockStyle.Fill; // Заполняет всю панель
            zField.PlaceholderText = "Напишите задачу"; // Текст-подсказка
            zField.Multiline = false;
            
            // Добавляем TextBox на панель
            panel1.Controls.Add(zField);
            
            // Добавляем панель на форму
            Controls.Add(panel1);
        }

        /// <summary>
        /// Применяет скругленные углы к элементу управления
        /// </summary>
        /// <param name="control">Элемент управления для скругления</param>
        /// <param name="radius">Радиус скругления в пикселях</param>
        private void ApplyRoundedCorners(Control control, int radius)
        {
            if (control == null) return;

            var rect = new Rectangle(0, 0, control.Width, control.Height);
            using var path = new GraphicsPath();
            
            int diameter = radius * 2;
            
            // Создаем путь со скругленными углами
            path.StartFigure();
            path.AddArc(new Rectangle(rect.X, rect.Y, diameter, diameter), 180, 90); // Левый верхний угол
            path.AddArc(new Rectangle(rect.Right - diameter, rect.Y, diameter, diameter), 270, 90); // Правый верхний угол
            path.AddArc(new Rectangle(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter), 0, 90); // Правый нижний угол
            path.AddArc(new Rectangle(rect.X, rect.Bottom - diameter, diameter, diameter), 90, 90); // Левый нижний угол
            path.CloseFigure();
            
            // Применяем путь к элементу управления
            control.Region = new Region(path);
        }

        private void ApplyPlusLayout()
        {
            // Stretch panel to form width with margins
            int margin = 20;
            panel1.Left = margin;
            panel1.Width = Math.Max(300, this.ClientSize.Width - margin * 2);
            // Center action button horizontally at bottom area
            buttonTask.Left = Math.Max(margin, (this.ClientSize.Width - buttonTask.Width) / 2);
            // Keep calendar/category icons near panel
            pictureBox1.Top = panel1.Bottom + 20;
            pictureBox2.Top = panel1.Bottom + 20;
        }
    }
}

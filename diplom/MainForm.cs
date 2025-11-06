using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace diplom
{
    public partial class MainForm : Form
    {
        private readonly int currentUserId;
        private readonly string currentUserLogin = string.Empty;
        
        public int UserId => currentUserId;
        public string UserLogin => currentUserLogin;
        private readonly System.Windows.Forms.Timer expandTimer = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer expandTimerWeek = new System.Windows.Forms.Timer();
        private bool isExpanded = false;
        private int animationTargetHeight = 0;
        private int animationStep = 20;
        private Control? animatedControl;
        private int expandedHeight = 120;
        private Bitmap? toggleImageOriginal;
        private Bitmap? toggleImageRotated180;
        private bool isWeekExpanded = true;
        private int animationTargetHeightWeek = 0;
        private int animationStepWeek = 20;
        private List<Control> weekControls = new List<Control>();
        private Dictionary<Control, int> weekExpandedHeights = new Dictionary<Control, int>();
        // Inline task card controls
        private Panel? taskCardPanel;
        private TextBox? taskCardTextBox;
        private Label? taskCardTimeLabel;
        private Panel? weekTasksPanel;
        private readonly List<(DateTime Time, Panel Panel)> todayTaskItems = new();
        private readonly List<(DateTime Time, Panel Panel)> weekTaskItems = new();
        private readonly List<TaskRecord> persistedTasks = new();
        private bool isLoadingFromStorage = false;
        private string TasksStoragePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "diplom", $"tasks_{currentUserId}.json");
		private readonly ToolTip taskToolTip = new ToolTip();
		private Label? emptyStateLabel;

		private class TaskRecord
        {
            public string Title { get; set; } = string.Empty;
            public DateTime DueAt { get; set; }
			public bool Completed { get; set; }
        }

		private class TaskItemMeta
		{
			public string Title { get; set; } = string.Empty;
			public DateTime DueAt { get; set; }
			public Label TitleLabel { get; set; } = null!;
			public Label TimeLabel { get; set; } = null!;
			public Panel CompleteCircle { get; set; } = null!;
			public bool Completed { get; set; }
		}

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(int userId, string login)
        {
            InitializeComponent();
            currentUserId = userId;
            currentUserLogin = login;
			// Tooltips for task actions
			taskToolTip.ShowAlways = true;
            // Configure animation
            expandTimer.Interval = 15;
            expandTimer.Tick += ExpandTimer_Tick;
            expandTimerWeek.Interval = 15;
            expandTimerWeek.Tick += ExpandTimerWeek_Tick;

            // Use only listBox1 for animation
            animatedControl = Controls.Find("listBox1", true).FirstOrDefault() as Control;
            if (animatedControl is ListBox listBox)
            {
                // Use the designed height; disable IntegralHeight so custom heights are respected
                listBox.IntegralHeight = false;
                expandedHeight = Math.Max(60, listBox.Height);
                // Start expanded and visible
                listBox.Visible = true;
                isExpanded = true;
            }
            else if (tasksPanel != null)
            {
                animatedControl = tasksPanel;
                expandedHeight = Math.Max(60, tasksPanel.Height == 0 ? 80 : tasksPanel.Height);
                tasksPanel.Visible = true;
                isExpanded = true;
            }

            // Cache toggle images: original and rotated 180°
            if (pictureBoxToggle.Image != null)
            {
                toggleImageOriginal = new Bitmap(pictureBoxToggle.Image);
                toggleImageRotated180 = new Bitmap(pictureBoxToggle.Image);
                toggleImageRotated180.RotateFlip(RotateFlipType.Rotate180FlipNone);
                pictureBoxToggle.Image = isExpanded ? toggleImageRotated180 : toggleImageOriginal;
            }

            // Create panel for "На этой неделе"
            weekTasksPanel = new Panel();
            weekTasksPanel.BackColor = Color.Transparent;
            weekTasksPanel.Location = new Point(52, 243);
            weekTasksPanel.Name = "weekTasksPanel";
            weekTasksPanel.Size = new Size(850, 0);
            weekTasksPanel.Visible = false;
            panel1.Controls.Add(weekTasksPanel);
            
            // Ensure pictureBox4 is added to panel1 (not clipped by other controls)
            if (pictureBox4.Parent != panel1)
            {
                panel1.Controls.Add(pictureBox4);
                pictureBox4.BringToFront(); // Ensure it's on top
            }

            // Register for expand/collapse animations
            weekControls.Add(weekTasksPanel);
            weekExpandedHeights[weekTasksPanel] = Math.Max(60, weekTasksPanel.Height == 0 ? 80 : weekTasksPanel.Height);
            isWeekExpanded = true;

            // Ensure DB table for tasks exists
            EnsureTasksTable();

            // Create a task card panel (rounded, with editable text and time)
            CreateTaskCardNearListBox1();

			// Adaptive anchors for main controls
			ConfigureAnchorsForAdaptiveLayout();

			// Empty state label (centered message when no tasks)
			emptyStateLabel = new Label();
			emptyStateLabel.AutoSize = false;
			// Position to fill the entire panel, text will be centered
			emptyStateLabel.Location = new Point(0, 0);
			emptyStateLabel.Size = new Size(panel1.Width, panel1.Height);
			emptyStateLabel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
			emptyStateLabel.TextAlign = ContentAlignment.MiddleCenter;
			emptyStateLabel.Font = new Font("Franklin Gothic Medium", 20F, FontStyle.Bold);
			emptyStateLabel.Text = "Все задачи выполнены";
			emptyStateLabel.Visible = false;
			panel1.Controls.Add(emptyStateLabel);
			emptyStateLabel.SendToBack(); // Keep it behind other controls (header elements will be on top)

            // Load tasks from local storage and render
            LoadTasksFromLocal();
			UpdateWeekSectionPosition(); // Update positions after loading
			UpdateEmptyStateUI();

            // Load user avatar
            LoadUserAvatar();

            // Save tasks on close
            this.FormClosing += MainForm_FormClosing;
            // Re-layout on resize to keep adaptive positions
            panel1.Resize += (_, __) =>
            {
                // Stretch task panels to panel width
                if (tasksPanel != null)
                {
                    tasksPanel.Width = Math.Max(200, panel1.Width - tasksPanel.Left - 240);
                }
                if (weekTasksPanel != null)
                {
                    weekTasksPanel.Width = Math.Max(200, panel1.Width - weekTasksPanel.Left - 240);
                }
                // Re-apply rounded region for task card panel
                ApplyRoundedRegion(taskCardPanel, 12);
                // Re-position week section and re-layout items to new widths
                UpdateWeekSectionPosition();
                if (tasksPanel != null) SortAndLayout(tasksPanel, todayTaskItems);
                if (weekTasksPanel != null) SortAndLayout(weekTasksPanel, weekTaskItems);
            };
        }

		private void ConfigureAnchorsForAdaptiveLayout()
		{
			// Header
			label1.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			// Right-side icons
			pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			pictureBox2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			pictureBox3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			// Today section header
			labelToday.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			pictureBoxToggle.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			// Task panels stretch horizontally
			if (tasksPanel != null) tasksPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			if (weekTasksPanel != null) weekTasksPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
			// Week section header
			label2.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			pictureBox4.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			// Task card panel stretches
			if (taskCardPanel != null) taskCardPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
		}

		private void UpdateEmptyStateUI()
		{
			bool hasToday = todayTaskItems.Count > 0;
			bool hasWeek = weekTaskItems.Count > 0;
			bool hasAny = hasToday || hasWeek;
			if (!hasAny)
			{
				// Hide task-related elements
				labelToday.Visible = false;
				pictureBoxToggle.Visible = false;
				label2.Visible = false;
				pictureBox4.Visible = false;
				if (tasksPanel != null) tasksPanel.Visible = false;
				if (weekTasksPanel != null) weekTasksPanel.Visible = false;
				if (taskCardPanel != null) taskCardPanel.Visible = false;
				
				// Keep header elements visible (label1, pictureBox1, pictureBox2, pictureBox3)
				// Show empty state message (it's already positioned below header elements)
				if (emptyStateLabel != null)
				{
					emptyStateLabel.Visible = true;
				}
			}
			else
			{
				// Hide empty state message
				if (emptyStateLabel != null) emptyStateLabel.Visible = false;

				// Determine which sections to show
				labelToday.Visible = hasToday;
				pictureBoxToggle.Visible = hasToday;
				if (tasksPanel != null) tasksPanel.Visible = hasToday && tasksPanel.Controls.Count > 0;

				label2.Visible = hasWeek;
				pictureBox4.Visible = hasWeek;
				if (weekTasksPanel != null) weekTasksPanel.Visible = hasWeek && weekTasksPanel.Controls.Count > 0;

				// Show input card only when there is at least one section visible (today or week)
				if (taskCardPanel != null) taskCardPanel.Visible = hasAny;

				// Reposition week section when today is hidden
				UpdateWeekSectionPosition();
			}
		}
        
        private void LoadUserAvatar()
        {
            try
            {
                var avatarPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "diplom", "avatars", $"user_{currentUserId}.jpg");
                if (File.Exists(avatarPath))
                {
                    var pictureBox2 = Controls.Find("pictureBox2", true).FirstOrDefault() as PictureBox;
                    if (pictureBox2 != null)
                    {
                        pictureBox2.Image?.Dispose();
                        pictureBox2.Image = new Bitmap(avatarPath);
                    }
                }
            }
            catch
            {
                // Если не удалось загрузить, используем изображение по умолчанию
            }
        }

        private void pictureBoxToggle_Click(object? sender, EventArgs e)
        {
            if (animatedControl == null) return;
            isExpanded = !isExpanded;
            animatedControl.Visible = true;
            animationTargetHeight = isExpanded ? expandedHeight : 0;
            animationStep = Math.Max(5, Math.Abs(animationTargetHeight - animatedControl.Height) / 4);
            RotateToggle(isExpanded);
            expandTimer.Start();
        }

        private void ExpandTimer_Tick(object? sender, EventArgs e)
        {
            if (animatedControl == null)
            {
                expandTimer.Stop();
                return;
            }

            if (animatedControl.Height == animationTargetHeight)
            {
                if (animatedControl.Height == 0)
                {
                    animatedControl.Visible = false;
                }
                expandTimer.Stop();
                return;
            }

            if (animatedControl.Height < animationTargetHeight)
            {
                animatedControl.Height = Math.Min(animatedControl.Height + animationStep, animationTargetHeight);
            }
            else
            {
                animatedControl.Height = Math.Max(animatedControl.Height - animationStep, animationTargetHeight);
            }
        }

        private void RotateToggle(bool expanded)
        {
            if (toggleImageOriginal == null || toggleImageRotated180 == null) return;
            pictureBoxToggle.Image = expanded ? toggleImageRotated180 : toggleImageOriginal;
        }

        private void pictureBox4_Click(object? sender, EventArgs e)
        {
            if (weekControls.Count == 0) return;
            isWeekExpanded = !isWeekExpanded;
            foreach (var ctrl in weekControls)
            {
                ctrl.Visible = true;
            }
            animationTargetHeightWeek = isWeekExpanded ? weekExpandedHeights.Values.DefaultIfEmpty(80).Max() : 0;
            animationStepWeek = Math.Max(5, animationTargetHeightWeek / 4);
            // Reuse same triangle image orientation rule: 180° when expanded
            RotateWeekToggle(isWeekExpanded);
            expandTimerWeek.Start();
        }

        private void ExpandTimerWeek_Tick(object? sender, EventArgs e)
        {
            bool allAtTarget = true;
            foreach (var ctrl in weekControls)
            {
                int target = isWeekExpanded ? weekExpandedHeights[ctrl] : 0;
                if (ctrl.Height == target) continue;
                allAtTarget = false;
                if (ctrl.Height < target)
                {
                    ctrl.Height = Math.Min(ctrl.Height + animationStepWeek, target);
                }
                else
                {
                    ctrl.Height = Math.Max(ctrl.Height - animationStepWeek, target);
                }
            }

            if (allAtTarget)
            {
                foreach (var ctrl in weekControls)
                {
                    if (ctrl.Height == 0) ctrl.Visible = false;
                }
                expandTimerWeek.Stop();
            }
        }

        private void RotateWeekToggle(bool expanded)
        {
            // Use pictureBox4 image state like first toggle but without caching
            if (pictureBox4.Image == null) return;
            var original = toggleImageOriginal ?? new Bitmap(pictureBox4.Image);
            var rotated = new Bitmap(pictureBox4.Image);
            rotated.RotateFlip(RotateFlipType.Rotate180FlipNone);
            pictureBox4.Image = expanded ? rotated : original;
        }

        private void CreateTaskCardNearListBox1()
        {
            var lb = Controls.Find("listBox1", true).FirstOrDefault() as ListBox;
            if (lb == null) return;

            taskCardPanel = new Panel();
            taskCardPanel.BackColor = Color.Gainsboro;
            taskCardPanel.Location = new Point(lb.Left, lb.Bottom + 12);
            taskCardPanel.Size = new Size(lb.Width, 60);
            taskCardPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            taskCardPanel.Padding = new Padding(12, 10, 12, 10);
            taskCardPanel.Resize += (_, __) => ApplyRoundedRegion(taskCardPanel, 12);
            ApplyRoundedRegion(taskCardPanel, 12);

            taskCardTextBox = new TextBox();
            taskCardTextBox.BorderStyle = BorderStyle.None;
            taskCardTextBox.Multiline = true;
            taskCardTextBox.Font = new Font("Franklin Gothic Medium", 14, FontStyle.Bold);
            taskCardTextBox.Dock = DockStyle.Fill;
            taskCardTextBox.PlaceholderText = "Введите задачу";

            taskCardTimeLabel = new Label();
            taskCardTimeLabel.AutoSize = true;
            taskCardTimeLabel.Dock = DockStyle.Right;
            taskCardTimeLabel.Font = new Font("Franklin Gothic Medium", 14, FontStyle.Bold);
            taskCardTimeLabel.Text = "18:00";
            taskCardTimeLabel.TextAlign = ContentAlignment.MiddleRight;
            taskCardTimeLabel.Padding = new Padding(0, 0, 4, 0);

            taskCardPanel.Controls.Add(taskCardTextBox);
            taskCardPanel.Controls.Add(taskCardTimeLabel);
            panel1.Controls.Add(taskCardPanel);
            taskCardPanel.BringToFront();
        }

        private static void ApplyRoundedRegion(Control? control, int radius)
        {
            if (control == null) return;
            var rect = new Rectangle(0, 0, control.Width, control.Height);
            using var path = new GraphicsPath();
            int d = radius * 2;
            path.StartFigure();
            path.AddArc(new Rectangle(rect.X, rect.Y, d, d), 180, 90);
            path.AddArc(new Rectangle(rect.Right - d, rect.Y, d, d), 270, 90);
            path.AddArc(new Rectangle(rect.Right - d, rect.Bottom - d, d, d), 0, 90);
            path.AddArc(new Rectangle(rect.X, rect.Bottom - d, d, d), 90, 90);
            path.CloseFigure();
            control.Region = new Region(path);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            plus plus = new plus(this);
            plus.Show(this);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            ProfileForm profileForm = new ProfileForm(this);
            profileForm.Show(this);
            this.Hide();
        }

        public void AddTask(string title, DateTime dateTime)
        {
            if (tasksPanel == null) return;

            // Создаем карточку задачи
            var itemPanel = new Panel
            {
                BackColor = Color.Gainsboro,
                Size = new Size(tasksPanel.Width, 60),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Padding = new Padding(12, 10, 12, 10)
            };

            var titleLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Franklin Gothic Medium", 14F, FontStyle.Bold),
                Location = new Point(15, 16),
                Text = title
            };

			var timeLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Franklin Gothic Medium", 14F, FontStyle.Bold),
                Text = dateTime.ToString("HH:mm"),
                Dock = DockStyle.Right,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 8, 0)
            };

			// Completion circle on the right
			var completeCircle = new Panel
			{
				Width = 28,
				Height = 28,
				Dock = DockStyle.Right,
				Cursor = Cursors.Hand,
				BackColor = Color.Transparent,
				Margin = new Padding(0)
			};
			completeCircle.Paint += (s, e) =>
			{
				var meta = (itemPanel.Tag as TaskItemMeta);
				bool done = meta != null && meta.Completed;
				e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
				var rect = new Rectangle(2, 2, completeCircle.Width - 4, completeCircle.Height - 4);
				using var pen = new Pen(done ? Color.Green : Color.DimGray, 2);
				if (done)
				{
					using var brush = new SolidBrush(Color.Green);
					e.Graphics.FillEllipse(brush, rect);
					using var innerPen = new Pen(Color.White, 2);
					// simple check mark
					e.Graphics.DrawLines(innerPen, new[] { new Point(rect.Left + 5, rect.Top + rect.Height / 2), new Point(rect.Left + 9, rect.Bottom - 6), new Point(rect.Right - 5, rect.Top + 6) });
				}
				else
				{
					e.Graphics.DrawEllipse(pen, rect);
				}
			};
			completeCircle.Click += (s, e) => ToggleCompleteForPanel(itemPanel);
			taskToolTip.SetToolTip(completeCircle, "Завершить задачу");

            // Добавляем справа сначала время, затем (для недели) дату слева от него
			itemPanel.Controls.Add(timeLabel);

			// Для задач не на сегодня показываем дату рядом со временем
            bool isToday = dateTime.Date == DateTime.Today;
            if (!isToday)
            {
                var dateLabel = new Label
                {
                    AutoSize = true,
                    Font = new Font("Franklin Gothic Medium", 12F, FontStyle.Regular),
                    Text = dateTime.ToString("dd.MM.yyyy"),
                    Dock = DockStyle.Right,
                    TextAlign = ContentAlignment.MiddleRight,
                    Padding = new Padding(0, 2, 12, 0)
                };
                itemPanel.Controls.Add(dateLabel);
            }

			// Добавляем круг завершения последним справа, чтобы он оказался правее даты/времени
			itemPanel.Controls.Add(completeCircle);

			itemPanel.Controls.Add(titleLabel);

            // Attach context menu for edit/delete
            var ctx = new ContextMenuStrip();
            var editItem = new ToolStripMenuItem("Редактировать");
            var deleteItem = new ToolStripMenuItem("Удалить");
            editItem.Click += (s, e) => EditTaskForPanel(itemPanel);
            deleteItem.Click += (s, e) => DeleteTaskForPanel(itemPanel);
            ctx.Items.Add(editItem);
            ctx.Items.Add(deleteItem);
            itemPanel.ContextMenuStrip = ctx;

            // Tag metadata
			itemPanel.Tag = new TaskItemMeta { Title = title, DueAt = dateTime, TitleLabel = titleLabel, TimeLabel = timeLabel, CompleteCircle = completeCircle, Completed = false };
            isToday = dateTime.Date == DateTime.Today;
            bool isThisWeek = IsInThisWeek(dateTime);

            Panel targetPanel = isToday ? tasksPanel : (weekTasksPanel ?? tasksPanel);
            targetPanel.Controls.Add(itemPanel);

            if (isToday)
            {
                todayTaskItems.Add((dateTime, itemPanel));
                SortAndLayout(targetPanel, todayTaskItems);
                targetPanel.Visible = true;
                if (animatedControl == targetPanel)
                {
                    isExpanded = true;
                    animationTargetHeight = Math.Max(expandedHeight, targetPanel.Height);
                    RotateToggle(true);
                }
                // Update week section position after today section changes
                UpdateWeekSectionPosition();
            }
			else if (isThisWeek)
			{
				weekTaskItems.Add((dateTime, itemPanel));
				if (weekTasksPanel != null)
				{
					SortAndLayout(weekTasksPanel, weekTaskItems);
					weekTasksPanel.Visible = true;
					UpdateWeekSectionPosition();
				}
			}
            else
            {
				// Пока нет отдельной секции для дальних дат — показываем в неделе
                weekTaskItems.Add((dateTime, itemPanel));
                if (weekTasksPanel != null)
                {
                    SortAndLayout(weekTasksPanel, weekTaskItems);
                    weekTasksPanel.Visible = true;
					UpdateWeekSectionPosition();
                }
            }

			// Keep in-memory list for local persistence
			persistedTasks.Add(new TaskRecord { Title = title, DueAt = dateTime, Completed = false });

			// Persist to DB for current user unless we are loading from local storage
            if (!isLoadingFromStorage)
            {
                try { SaveTaskToDb(title, dateTime); } catch { /* ignore db errors for UI flow */ }
            }

			UpdateEmptyStateUI();
        }

		private void EditTaskForPanel(Panel itemPanel)
		{
			if (itemPanel.Tag is not TaskItemMeta meta) return;
			string newTitle = meta.Title;
			DateTime newDueAt = meta.DueAt;
			if (!ShowEditTaskDialog(ref newTitle, ref newDueAt)) return;

			// Update UI labels
			meta.Title = newTitle;
			meta.DueAt = newDueAt;
			meta.TitleLabel.Text = newTitle;
			meta.TimeLabel.Text = newDueAt.ToString("HH:mm");

			// Update date label presence for non-today
			bool isToday = newDueAt.Date == DateTime.Today;
			var existingDateLabel = itemPanel.Controls
				.OfType<Label>()
				.FirstOrDefault(l => l != meta.TitleLabel && l != meta.TimeLabel && l.Dock == DockStyle.Right);
			if (!isToday)
			{
				string dateText = newDueAt.ToString("dd.MM.yyyy");
				if (existingDateLabel == null)
				{
					var dateLabel = new Label
					{
						AutoSize = true,
						Font = new Font("Franklin Gothic Medium", 12F, FontStyle.Regular),
						Text = dateText,
						Dock = DockStyle.Right,
						TextAlign = ContentAlignment.MiddleRight,
						Padding = new Padding(0, 2, 12, 0)
					};
					itemPanel.Controls.Add(dateLabel);
				}
				else
				{
					existingDateLabel.Text = dateText;
				}
			}
			else if (existingDateLabel != null)
			{
				itemPanel.Controls.Remove(existingDateLabel);
				existingDateLabel.Dispose();
			}

			// Move between lists if day changed
			MoveOrResortItem(itemPanel, meta.DueAt);
			UpdateWeekSectionPosition();

			// Update persisted tasks (match by old values; may affect duplicates)
			var existing = persistedTasks.FirstOrDefault(t => t.Title == meta.TitleLabel.Text && t.DueAt == meta.DueAt);
			if (existing != null)
			{
				existing.Title = newTitle;
				existing.DueAt = newDueAt;
			}
			else
			{
				// If not found by tag values, try loosest match by panel time label
				var loose = persistedTasks.FirstOrDefault(t => t.Title == meta.Title && t.DueAt.ToString("HH:mm") == meta.TimeLabel.Text);
				if (loose != null)
				{
					loose.Title = newTitle;
					loose.DueAt = newDueAt;
				}
			}

			SaveTasksToLocal();

			// Best-effort DB update: delete old row and insert new
			try
			{
				DeleteTaskFromDb(meta.Title, meta.DueAt);
				SaveTaskToDb(newTitle, newDueAt);
			}
			catch { }
		}

		private void ToggleCompleteForPanel(Panel itemPanel)
		{
			if (itemPanel.Tag is not TaskItemMeta meta) return;
			meta.Completed = !meta.Completed;
			// Update visuals
			meta.TitleLabel.Font = new Font(meta.TitleLabel.Font, meta.Completed ? FontStyle.Strikeout : FontStyle.Bold);
			meta.TitleLabel.ForeColor = meta.Completed ? Color.Gray : SystemColors.ControlText;
			itemPanel.BackColor = meta.Completed ? Color.LightGray : Color.Gainsboro;
			// Update tooltip text
			if (meta.CompleteCircle != null)
			{
				meta.CompleteCircle.Invalidate();
				taskToolTip.SetToolTip(meta.CompleteCircle, meta.Completed ? "Снять завершение" : "Завершить задачу");
			}

			// Update persistence
			var rec = persistedTasks.FirstOrDefault(t => t.Title == meta.Title && t.DueAt == meta.DueAt);
			if (rec != null) rec.Completed = meta.Completed;
			SaveTasksToLocal();
		}

		private void DeleteTaskForPanel(Panel itemPanel)
		{
			if (itemPanel.Tag is not TaskItemMeta meta) return;

			// Remove from panel and lists
			var host = itemPanel.Parent as Panel;
			if (host != null)
			{
				host.Controls.Remove(itemPanel);
			}
			RemoveFromLists(itemPanel);

			// Update persisted tasks
			var idx = persistedTasks.FindIndex(t => t.Title == meta.Title && t.DueAt == meta.DueAt);
			if (idx >= 0) persistedTasks.RemoveAt(idx);
			SaveTasksToLocal();

			// DB best-effort delete
			try { DeleteTaskFromDb(meta.Title, meta.DueAt); } catch { }

			// Dispose UI
			itemPanel.Dispose();

			// Relayout affected host
			if (host == tasksPanel)
			{
				SortAndLayout(tasksPanel, todayTaskItems);
				UpdateWeekSectionPosition();
			}
			else if (host == weekTasksPanel && weekTasksPanel != null)
			{
				SortAndLayout(weekTasksPanel, weekTaskItems);
				UpdateWeekSectionPosition();
			}

			UpdateEmptyStateUI();
		}

		private void MoveOrResortItem(Panel itemPanel, DateTime newDueAt)
		{
			bool isToday = newDueAt.Date == DateTime.Today;
			bool isThisWeek = IsInThisWeek(newDueAt);
			var currentHost = itemPanel.Parent as Panel;
			if (currentHost == null) return;

			// Remove from both lists
			RemoveFromLists(itemPanel);

			// Add to appropriate list and host
			Panel targetPanel = isToday ? (tasksPanel ?? currentHost) : (weekTasksPanel ?? currentHost);
			if (currentHost != targetPanel)
			{
				currentHost.Controls.Remove(itemPanel);
				targetPanel.Controls.Add(itemPanel);
			}

			if (isToday)
			{
				todayTaskItems.Add((newDueAt, itemPanel));
				SortAndLayout(targetPanel, todayTaskItems);
				targetPanel.Visible = true;
			}
			else if (isThisWeek)
			{
				weekTaskItems.Add((newDueAt, itemPanel));
				if (weekTasksPanel != null)
				{
					SortAndLayout(weekTasksPanel, weekTaskItems);
					weekTasksPanel.Visible = true;
				}
			}
			else
			{
				weekTaskItems.Add((newDueAt, itemPanel));
				if (weekTasksPanel != null)
				{
					SortAndLayout(weekTasksPanel, weekTaskItems);
					weekTasksPanel.Visible = true;
				}
			}
		}

		private void RemoveFromLists(Panel itemPanel)
		{
			int idx = todayTaskItems.FindIndex(x => x.Panel == itemPanel);
			if (idx >= 0) todayTaskItems.RemoveAt(idx);
			idx = weekTaskItems.FindIndex(x => x.Panel == itemPanel);
			if (idx >= 0) weekTaskItems.RemoveAt(idx);
		}

		private bool ShowEditTaskDialog(ref string title, ref DateTime dueAt)
		{
			using (var dlg = new Form())
			{
				dlg.Text = "Редактирование задачи";
				dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
				dlg.StartPosition = FormStartPosition.CenterParent;
				dlg.MinimizeBox = false;
				dlg.MaximizeBox = false;
				dlg.ClientSize = new Size(420, 180);

				var titleBox = new TextBox { Left = 16, Top = 20, Width = 380, Text = title };
				var timePicker = new DateTimePicker { Left = 16, Top = 60, Width = 180, Format = DateTimePickerFormat.Custom, CustomFormat = "dd.MM.yyyy HH:mm", Value = dueAt };
				var okBtn = new Button { Text = "OK", Left = 230, Width = 75, Top = 120, DialogResult = DialogResult.OK };
				var cancelBtn = new Button { Text = "Отмена", Left = 321, Width = 75, Top = 120, DialogResult = DialogResult.Cancel };
				dlg.Controls.Add(titleBox);
				dlg.Controls.Add(timePicker);
				dlg.Controls.Add(okBtn);
				dlg.Controls.Add(cancelBtn);
				dlg.AcceptButton = okBtn;
				dlg.CancelButton = cancelBtn;

				if (dlg.ShowDialog(this) == DialogResult.OK)
				{
					if (string.IsNullOrWhiteSpace(titleBox.Text)) return false;
					title = titleBox.Text.Trim();
					dueAt = timePicker.Value;
					return true;
				}
			}
			return false;
		}

		private void DeleteTaskFromDb(string title, DateTime dueAt)
		{
			DB db = new DB();
			using var cmd = new MySqlConnector.MySqlCommand("DELETE FROM tasks WHERE user_id=@uid AND title=@t AND due_at=@d LIMIT 1", db.getConnection());
			cmd.Parameters.AddWithValue("@uid", currentUserId);
			cmd.Parameters.AddWithValue("@t", title);
			cmd.Parameters.AddWithValue("@d", dueAt);
			db.openConnection();
			cmd.ExecuteNonQuery();
			db.closeConnection();
		}
        

        private void SortAndLayout(Panel hostPanel, List<(DateTime Time, Panel Panel)> items)
        {
            // Сортировка по времени
            items.Sort((a, b) => a.Time.CompareTo(b.Time));

            int y = 0;
            int spacing = 8;
            foreach (var entry in items)
            {
                entry.Panel.Location = new Point(0, y);
                entry.Panel.Width = hostPanel.Width;
                y += entry.Panel.Height + spacing;
            }

            hostPanel.Height = Math.Min(Math.Max(60, y), 400);
            if (weekExpandedHeights.ContainsKey(hostPanel))
            {
                weekExpandedHeights[hostPanel] = hostPanel.Height;
            }
            
            // Update positions of "На этой неделе" section when "Сегодня" section changes
            UpdateWeekSectionPosition();
        }
        
		private void UpdateWeekSectionPosition()
        {
            if (tasksPanel == null || label2 == null || pictureBox4 == null || weekTasksPanel == null) return;
            
			// Decide where the week section should start
			bool hasToday = todayTaskItems.Count > 0;
			int spacing = 20; // Space between sections
			int weekSectionTop;
			
			if (hasToday)
			{
				// Calculate the actual bottom of "Сегодня" section
				// Need to find the lowest point: labelToday, pictureBoxToggle, or tasksPanel
				int todaySectionBottom = 0;
				
				// Start with labelToday position
				if (labelToday != null && labelToday.Visible)
				{
					todaySectionBottom = Math.Max(todaySectionBottom, labelToday.Bottom);
				}
				
				// Check pictureBoxToggle
				if (pictureBoxToggle != null && pictureBoxToggle.Visible)
				{
					todaySectionBottom = Math.Max(todaySectionBottom, pictureBoxToggle.Bottom);
				}
				
				// Check tasksPanel (most important - it's the task list)
				if (tasksPanel != null && tasksPanel.Visible && tasksPanel.Height > 0)
				{
					todaySectionBottom = Math.Max(todaySectionBottom, tasksPanel.Bottom);
				}
				
				// If nothing was found, use default position
				if (todaySectionBottom == 0)
				{
					todaySectionBottom = labelToday != null ? labelToday.Bottom : 130;
				}
				
				// Position week section below today section with spacing
				weekSectionTop = todaySectionBottom + spacing;
			}
			else
			{
				// No today tasks: place week section where today header usually starts
				int todayHeaderTop = labelToday != null ? labelToday.Top : 96;
				weekSectionTop = todayHeaderTop;
				
				// But ensure minimum position to avoid clipping at top
				int minTop = 96; // Minimum Y position (same as labelToday default)
				if (weekSectionTop < minTop)
				{
					weekSectionTop = minTop;
				}
			}
			
			// Apply positions - use same offset as in Designer (label2 Y + 10 = pictureBox4 Y)
			label2.Location = new Point(label2.Location.X, weekSectionTop);
			// Position triangle 10 pixels below label2 (same as in Designer: label2 at 206, pictureBox4 at 216)
			int triangleTop = weekSectionTop + 10;
			pictureBox4.Location = new Point(pictureBox4.Location.X, triangleTop);
			// Ensure triangle is visible and not clipped
			pictureBox4.BringToFront();
			
			// Position weekTasksPanel below the label
			int weekPanelTop = weekSectionTop + label2.Height + 10;
			weekTasksPanel.Location = new Point(weekTasksPanel.Location.X, weekPanelTop);
        }

        private static bool IsInThisWeek(DateTime date)
        {
            DateTime today = DateTime.Today;
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7; // Monday-based week start
            DateTime weekStart = today.AddDays(-diff).Date;
            DateTime weekEnd = weekStart.AddDays(7).Date; // exclusive
            return date.Date >= weekStart && date.Date < weekEnd;
        }

        private void EnsureTasksTable()
        {
            try
            {
                DB db = new DB();
                using var cmd = new MySqlConnector.MySqlCommand(@"CREATE TABLE IF NOT EXISTS tasks (
                    id INT PRIMARY KEY AUTO_INCREMENT,
                    user_id INT NOT NULL,
                    title VARCHAR(255) NOT NULL,
                    due_at DATETIME NOT NULL,
                    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
                    INDEX idx_user_due (user_id, due_at)
                ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;", db.getConnection());
                db.openConnection();
                cmd.ExecuteNonQuery();
                db.closeConnection();
            }
            catch
            {
                // silently ignore if no DB present
            }
        }

        private void SaveTaskToDb(string title, DateTime dueAt)
        {
            DB db = new DB();
            using var cmd = new MySqlConnector.MySqlCommand("INSERT INTO tasks (user_id, title, due_at) VALUES (@uid, @t, @d)", db.getConnection());
            cmd.Parameters.AddWithValue("@uid", currentUserId);
            cmd.Parameters.AddWithValue("@t", title);
            cmd.Parameters.AddWithValue("@d", dueAt);
            db.openConnection();
            cmd.ExecuteNonQuery();
            db.closeConnection();
        }

        private void LoadTasksFromLocal()
        {
            try
            {
                var path = TasksStoragePath;
                if (!File.Exists(path)) return;
                var json = File.ReadAllText(path);
                var list = JsonSerializer.Deserialize<List<TaskRecord>>(json) ?? new List<TaskRecord>();
                if (list.Count == 0) return;
                isLoadingFromStorage = true;
                foreach (var tr in list.OrderBy(t => t.DueAt))
                {
                    AddTask(tr.Title, tr.DueAt);
                }
            }
            catch
            {
                // ignore local load errors
            }
            finally
            {
                isLoadingFromStorage = false;
            }
        }

        private void SaveTasksToLocal()
        {
            try
            {
                var dir = Path.GetDirectoryName(TasksStoragePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
                var json = JsonSerializer.Serialize(persistedTasks.OrderBy(t => t.DueAt).ToList(), new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(TasksStoragePath, json);
            }
            catch
            {
                // ignore local save errors
            }
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            SaveTasksToLocal();
        }
    }
}

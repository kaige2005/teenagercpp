using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TeenCppEdu.Services.Database;
using TeenCppEdu.Services.Logger;
using TeenCppEdu.Core.Models;

namespace TeenCppEdu.UI.Forms
{
    /// <summary>
    /// 游戏化主界面 - 学习地图
    /// </summary>
    public partial class MainForm : Form
    {
        private DatabaseService _db;
        private StudentProgress _progress;

        // 游戏化色彩
        private readonly Color ColorBgDark = Color.FromArgb(45, 52, 70);
        private readonly Color ColorBgCard = Color.FromArgb(55, 65, 90);
        private readonly Color ColorAccentGreen = Color.FromArgb(100, 200, 120);
        private readonly Color ColorAccentGold = Color.FromArgb(255, 200, 80);
        private readonly Color ColorTextLight = Color.FromArgb(240, 240, 240);
        private readonly Color ColorLocked = Color.FromArgb(100, 100, 110);

        private Label lblTitle;
        private Label lblLevel;
        private Label lblExp;
        private Label lblBadges;
        private Panel mapPanel;
        private Button btnLesson01;
        private Button btnLesson02;
        private Button btnLesson03;

        // 课程配置
        private readonly (string Id, string Number, string Title, string Icon, Point Location)[] _lessons =
        {
            ("L01", "第1课", "你好，C++！", "🚀", new Point(100, 200)),
            ("L02", "第2课", "分支魔法", "🎯", new Point(300, 180)),
            ("L03", "第3课", "数组与循环", "📊", new Point(500, 220))
        };

        public MainForm()
        {
            InitializeComponent();
            InitializeDatabase();
            InitializeUser();
            InitializeLessonButtons(); // 在 _progress 初始化后再创建按钮
        }

        private void InitializeComponent()
        {
            this.Text = "🎮 TeenC++ 编程冒险";
            this.Size = new Size(900, 700);
            this.BackColor = ColorBgDark;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Microsoft YaHei", 10);

            // 顶部信息栏
            var infoPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(35, 42, 60)
            };

            lblTitle = new Label
            {
                Text = "🎮 TeenC++ 编程冒险",
                Font = new Font("Microsoft YaHei", 18, FontStyle.Bold),
                ForeColor = ColorAccentGold,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            lblLevel = new Label
            {
                Text = "Lv.1",
                Font = new Font("Microsoft YaHei", 14, FontStyle.Bold),
                ForeColor = ColorAccentGreen,
                AutoSize = true,
                Location = new Point(20, 50)
            };

            lblExp = new Label
            {
                Text = "💎 0/500 EXP",
                Font = new Font("Microsoft YaHei", 12),
                ForeColor = ColorTextLight,
                AutoSize = true,
                Location = new Point(100, 52)
            };

            lblBadges = new Label
            {
                Text = "🏆 暂无徽章",
                Font = new Font("Microsoft YaHei", 11),
                ForeColor = ColorAccentGold,
                AutoSize = true,
                Location = new Point(400, 52)
            };

            infoPanel.Controls.AddRange(new Control[] { lblTitle, lblLevel, lblExp, lblBadges });

            // 地图面板
            mapPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ColorBgDark,
                Padding = new Padding(20)
            };

            // 课程按钮将在 InitializeUser 后由 InitializeLessonButtons 创建
            // 底部状态栏
            var statusPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = Color.FromArgb(35, 42, 60)
            };

            var lblStatus = new Label
            {
                Text = "✨ 准备好开始你的编程冒险了吗？点击第一课！",
                ForeColor = ColorTextLight,
                AutoSize = true,
                Location = new Point(20, 10)
            };
            statusPanel.Controls.Add(lblStatus);

            this.Controls.Add(mapPanel);
            this.Controls.Add(infoPanel);
            this.Controls.Add(statusPanel);
        }

        private Button CreateLessonButton(string number, string title, string icon, Point location, bool unlocked)
        {
            var btn = new Button
            {
                Size = new Size(120, 120),
                Location = location,
                FlatStyle = FlatStyle.Flat,
                BackColor = unlocked ? ColorAccentGreen : ColorLocked,
                ForeColor = unlocked ? ColorBgDark : Color.FromArgb(150, 150, 160),
                Cursor = unlocked ? Cursors.Hand : Cursors.Default,
                Enabled = unlocked,
                Tag = unlocked
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = unlocked
                ? Color.FromArgb(120, 220, 140)
                : ColorLocked;

            var lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI Emoji", 36),
                AutoSize = true,
                Location = new Point(35, 10),
                BackColor = Color.Transparent,
                ForeColor = btn.ForeColor
            };

            var lblNum = new Label
            {
                Text = number,
                Font = new Font("Microsoft YaHei", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(35, 55),
                BackColor = Color.Transparent,
                ForeColor = btn.ForeColor
            };

            var lblTitle = new Label
            {
                Text = title,
                Font = new Font("Microsoft YaHei", 9),
                AutoSize = true,
                Location = new Point(20, 85),
                BackColor = Color.Transparent,
                ForeColor = btn.ForeColor
            };

            btn.Controls.Add(lblIcon);
            btn.Controls.Add(lblNum);
            btn.Controls.Add(lblTitle);

            // 关键修复：Label子控件需要转发Click事件到父按钮
            lblIcon.Click += (s, e) => btn.PerformClick();
            lblNum.Click += (s, e) => btn.PerformClick();
            lblTitle.Click += (s, e) => btn.PerformClick();

            return btn;
        }

        private void InitializeDatabase()
        {
            _db = new DatabaseService("teen_cpp_student.db");
        }

        private void InitializeUser()
        {
            _progress = _db.GetOrCreateProgress("学员");
            UpdateUI();
        }

        private void InitializeLessonButtons()
        {
            LoggerService.Instance?.Info("初始化课程按钮...");

            // 清空地图面板
            mapPanel.Controls.Clear();
            btnLesson01 = null;
            btnLesson02 = null;
            btnLesson03 = null;

            // 确定解锁状态（默认只有第1课解锁）
            string unlockedId = _progress?.UnlockedLessonId ?? "L01";
            int maxUnlockedSequence = 1;

            // 解析已解锁的最大序号
            if (!string.IsNullOrEmpty(unlockedId) && unlockedId.StartsWith("L"))
            {
                if (int.TryParse(unlockedId.Substring(1), out int seq))
                {
                    maxUnlockedSequence = seq;
                }
            }

            LoggerService.Instance?.Info($"当前解锁状态: L{maxUnlockedSequence:00}");

            // 创建每个课程按钮
            for (int i = 0; i < _lessons.Length; i++)
            {
                var lesson = _lessons[i];
                int lessonSequence = i + 1;
                bool isUnlocked = lessonSequence <= maxUnlockedSequence;

                string icon = isUnlocked ? lesson.Icon : "🔒";
                string title = isUnlocked ? lesson.Title : "???";

                var btn = CreateLessonButton(
                    lesson.Number,
                    title,
                    icon,
                    lesson.Location,
                    isUnlocked
                );

                // 捕获变量用于事件处理（必须在循环内捕获）
                string lessonId = lesson.Id;
                bool unlocked = isUnlocked; // 关键修复：创建局部变量副本，避免闭包共享问题

                // BUG-20260322-004 修复：事件处理器中使用 btn.Tag 动态判断解锁状态
                // 而不是依赖创建时捕获的 unlocked 变量
                btn.Click += (s, e) => {
                    var currentBtn = (Button)s;
                    bool isUnlocked = (bool)(currentBtn.Tag ?? false);

                    if (isUnlocked)
                    {
                        LoggerService.Instance?.Info($"点击课程按钮: {lessonId} (unlocked={isUnlocked})");
                        OpenLesson(lessonId);
                    }
                    else
                    {
                        LoggerService.Instance?.Info($"点击锁定课程: {lessonId} (unlocked={isUnlocked})");
                        MessageBox.Show(
                            $"{lesson.Number} 还未解锁！\n\n请先完成前面的课程。",
                            "课程锁定",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                };

                if (isUnlocked)
                {
                    btn.Cursor = Cursors.Hand;
                }
                else
                {
                    btn.Cursor = Cursors.No;
                }

                switch (i)
                {
                    case 0: btnLesson01 = btn; break;
                    case 1: btnLesson02 = btn; break;
                    case 2: btnLesson03 = btn; break;
                }

                mapPanel.Controls.Add(btn);
                LoggerService.Instance?.Info($"创建课程按钮: {lesson.Id}, 解锁={isUnlocked}");
            }

            // 连接线路径（视觉效果）
            var pathLine = new Panel
            {
                Location = new Point(180, 215),
                Size = new Size(300, 4),
                BackColor = Color.FromArgb(80, 90, 110)
            };
            mapPanel.Controls.Add(pathLine);
            pathLine.SendToBack();

            LoggerService.Instance?.Info("课程按钮初始化完成");
        }

        private void UpdateUI()
        {
            lblLevel.Text = $"Lv.{_progress.Level}";
            lblExp.Text = $"💎 {_progress.Experience}/500 EXP";

            if (!string.IsNullOrEmpty(_progress.EarnedBadges))
            {
                var badges = _progress.EarnedBadges.Split(',');
                lblBadges.Text = $"🏆 {badges.Length} 个徽章";
            }

            // 刷新课程按钮解锁状态
            RefreshLessonButtons();
        }

        private void RefreshLessonButtons()
        {
            LoggerService.Instance?.Info("刷新课程按钮状态...");

            // 确定当前解锁状态
            string unlockedId = _progress?.UnlockedLessonId ?? "L01";
            int maxUnlockedSequence = 1;

            if (!string.IsNullOrEmpty(unlockedId) && unlockedId.StartsWith("L"))
            {
                if (int.TryParse(unlockedId.Substring(1), out int seq))
                {
                    maxUnlockedSequence = seq;
                }
            }

            // 只更新现有按钮的状态，不重新创建
            var buttons = new[] { btnLesson01, btnLesson02, btnLesson03 };

            for (int i = 0; i < _lessons.Length && i < buttons.Length; i++)
            {
                var btn = buttons[i];
                if (btn == null) continue;

                int lessonSequence = i + 1;
                bool shouldBeUnlocked = lessonSequence <= maxUnlockedSequence;
                bool isCurrentlyUnlocked = (bool)(btn.Tag ?? false);

                // 只在状态变化时更新
                if (shouldBeUnlocked != isCurrentlyUnlocked)
                {
                    LoggerService.Instance?.Info($"更新课程 {_lessons[i].Id}: 解锁={shouldBeUnlocked}");

                    btn.Tag = shouldBeUnlocked;
                    btn.BackColor = shouldBeUnlocked ? ColorAccentGreen : ColorLocked;
                    btn.ForeColor = shouldBeUnlocked ? ColorBgDark : Color.FromArgb(150, 150, 160);
                    btn.Enabled = shouldBeUnlocked;
                    btn.Cursor = shouldBeUnlocked ? Cursors.Hand : Cursors.No;

                    // 更新内部标签
                    var iconLabel = btn.Controls.Cast<Control>().FirstOrDefault(c => c is Label && c.Font.Size > 20) as Label;
                    var titleLabel = btn.Controls.Cast<Control>().FirstOrDefault(c => c is Label && c.Location.Y > 60) as Label;

                    if (iconLabel != null)
                        iconLabel.Text = shouldBeUnlocked ? _lessons[i].Icon : "🔒";
                    if (titleLabel != null)
                        titleLabel.Text = shouldBeUnlocked ? _lessons[i].Title : "???";
                }
            }

            LoggerService.Instance?.Info("课程按钮状态刷新完成");
        }

        private void OpenLesson(string lessonId)
        {
            LoggerService.Instance?.LogUserAction("点击课程", lessonId);

            try
            {
                LoggerService.Instance?.Info($"正在创建课程窗口: {lessonId}");
                var lessonForm = new LessonForm(lessonId, _db, _progress);
                LoggerService.Instance?.Info("课程窗口创建成功，准备显示");

                var result = lessonForm.ShowDialog();
                LoggerService.Instance?.Info($"课程窗口关闭，结果: {result}");

                if (result == DialogResult.OK)
                {
                    // 刷新进度
                    _progress = _db.GetOrCreateProgress("学员");
                    UpdateUI();
                }
            }
            catch (FileNotFoundException ex)
            {
                LoggerService.Instance?.Error($"课程文件未找到: {ex.FileName}", ex);
                var logPath = LoggerService.Instance?.GetLogPath() ?? "未知路径";
                MessageBox.Show(
                    $"无法加载课程，文件未找到:\n{ex.Message}\n\n" +
                    $"请确认课程数据已正确放置。\n\n日志文件: {logPath}",
                    "课程加载失败",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (DirectoryNotFoundException ex)
            {
                LoggerService.Instance?.Error($"课程目录未找到: {ex.Message}", ex);
                var logPath = LoggerService.Instance?.GetLogPath() ?? "未知路径";
                MessageBox.Show(
                    $"无法加载课程，目录未找到:\n{ex.Message}\n\n" +
                    $"请确认课程数据已正确放置。\n\n日志文件: {logPath}",
                    "课程加载失败",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                LoggerService.Instance?.Error($"打开课程失败: {lessonId}", ex);
                var logPath = LoggerService.Instance?.GetLogPath() ?? "未知路径";
                MessageBox.Show(
                    $"无法打开课程:\n{ex.GetType().Name}: {ex.Message}\n\n" +
                    $"堆栈:\n{ex.StackTrace}\n\n" +
                    $"日志文件: {logPath}",
                    "课程加载失败",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}

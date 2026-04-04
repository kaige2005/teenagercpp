using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TeenCppEdu.Core.Models;
using TeenCppEdu.UI.Controls;

namespace TeenCppEdu.UI.Forms
{
    /// <summary>
    /// 知识学习阶段窗体 - 整合场景、概念、测验、填空
    /// </summary>
    public partial class KnowledgePhaseForm : Form
    {
        private readonly KnowledgePhase _phase;
        private readonly string _lessonId;
        private int _currentSectionIndex = 0;
        private int _totalEarnedXp = 0;
        private readonly List<Panel> _sectionPanels = new List<Panel>();
        private Panel _contentPanel;
        private Label _lblProgress;
        private Label _lblXp;
        private Button _btnNext;
        private Button _btnPrev;

        private readonly Color ColorBgDark = Color.FromArgb(45, 52, 70);
        private readonly Color ColorBgCard = Color.FromArgb(55, 65, 90);
        private readonly Color ColorAccentGreen = Color.FromArgb(100, 200, 120);
        private readonly Color ColorAccentGold = Color.FromArgb(255, 200, 80);
        private readonly Color ColorTextLight = Color.FromArgb(240, 240, 240);

        public int TotalEarnedXp => _totalEarnedXp;
        public bool IsCompleted => _currentSectionIndex >= _phase.Sections.Count;

        public KnowledgePhaseForm(KnowledgePhase phase, string lessonId)
        {
            _phase = phase;
            _lessonId = lessonId;
            InitializeComponent();
            LoadCurrentSection();
        }

        private void InitializeComponent()
        {
            this.Text = $"📚 {_phase.Title}";
            this.Size = new Size(900, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = ColorBgDark;

            // 顶部导航栏
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(35, 42, 60)
            };

            var lblTitle = new Label
            {
                Text = $"📚 {_phase.Title}",
                Font = new Font("Microsoft YaHei", 16, FontStyle.Bold),
                ForeColor = ColorAccentGold,
                AutoSize = true,
                Location = new Point(20, 20)
            };
            headerPanel.Controls.Add(lblTitle);

            // 进度显示
            _lblProgress = new Label
            {
                Text = "1 / " + _phase.Sections.Count,
                Font = new Font("Microsoft YaHei", 12),
                ForeColor = ColorTextLight,
                AutoSize = true,
                Location = new Point(400, 24)
            };
            headerPanel.Controls.Add(_lblProgress);

            // XP显示
            _lblXp = new Label
            {
                Text = $"💎 0 / {_phase.TotalXp} XP",
                Font = new Font("Microsoft YaHei", 12, FontStyle.Bold),
                ForeColor = ColorAccentGreen,
                AutoSize = true,
                Location = new Point(550, 24)
            };
            headerPanel.Controls.Add(_lblXp);

            // 关闭按钮
            var btnClose = new Button
            {
                Text = "✕",
                Size = new Size(40, 40),
                Location = new Point(830, 15),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(255, 100, 100),
                ForeColor = Color.White,
                Font = new Font("Microsoft YaHei", 12, FontStyle.Bold)
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };
            headerPanel.Controls.Add(btnClose);

            // 底部导航栏
            var footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = Color.FromArgb(35, 42, 60)
            };

            _btnPrev = new Button
            {
                Text = "◀ 上一步",
                Size = new Size(120, 45),
                Location = new Point(20, 17),
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorBgCard,
                ForeColor = ColorTextLight,
                Font = new Font("Microsoft YaHei", 11),
                Enabled = false
            };
            _btnPrev.FlatAppearance.BorderSize = 0;
            _btnPrev.Click += (s, e) => NavigateToSection(-1);
            footerPanel.Controls.Add(_btnPrev);

            _btnNext = new Button
            {
                Text = "下一步 ▶",
                Size = new Size(160, 45),
                Location = new Point(710, 17),
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorAccentGreen,
                ForeColor = ColorBgDark,
                Font = new Font("Microsoft YaHei", 11, FontStyle.Bold)
            };
            _btnNext.FlatAppearance.BorderSize = 0;
            _btnNext.Click += OnNextClicked;
            footerPanel.Controls.Add(_btnNext);

            // 中间内容区域
            _contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ColorBgDark,
                Padding = new Padding(20),
                AutoScroll = true
            };

            this.Controls.Add(_contentPanel);
            this.Controls.Add(footerPanel);
            this.Controls.Add(headerPanel);
        }

        private void LoadCurrentSection()
        {
            _contentPanel.Controls.Clear();

            if (_currentSectionIndex >= _phase.Sections.Count)
            {
                ShowCompletionPanel();
                return;
            }

            var section = _phase.Sections.OrderBy(s => s.Order).ElementAt(_currentSectionIndex);
            Panel sectionPanel = null;

            switch (section.SectionType)
            {
                case SectionType.Scene:
                    sectionPanel = new ScenePanel(section as SceneSection);
                    break;
                case SectionType.Concept:
                    sectionPanel = new ConceptPanel(section as ConceptSection);
                    break;
                case SectionType.Quiz:
                    var quizPanel = new QuizPanel(section as QuizSection);
                    quizPanel.QuizCompleted += OnQuizCompleted;
                    sectionPanel = quizPanel;
                    _btnNext.Enabled = false; // 需要完成测验才能继续
                    break;
                case SectionType.FillBlank:
                    var fillPanel = new FillBlankPanel(section as FillBlankSection);
                    fillPanel.Completed += OnFillBlankCompleted;
                    sectionPanel = fillPanel;
                    _btnNext.Enabled = false; // 需要完成填空才能继续
                    break;
            }

            if (sectionPanel != null)
            {
                sectionPanel.Dock = DockStyle.Fill;
                _contentPanel.Controls.Add(sectionPanel);
                _sectionPanels.Add(sectionPanel);
            }

            UpdateProgress();
        }

        private void NavigateToSection(int direction)
        {
            int newIndex = _currentSectionIndex + direction;
            if (newIndex < 0 || newIndex >= _phase.Sections.Count) return;

            _currentSectionIndex = newIndex;
            LoadCurrentSection();

            // 更新按钮状态
            _btnPrev.Enabled = _currentSectionIndex > 0;
        }

        private void OnNextClicked(object sender, EventArgs e)
        {
            if (_currentSectionIndex < _phase.Sections.Count - 1)
            {
                NavigateToSection(1);
            }
            else
            {
                // 全部完成
                ShowCompletionPanel();
            }
        }

        private void OnQuizCompleted(object sender, QuizCompletedEventArgs e)
        {
            _totalEarnedXp += e.TotalXp;
            UpdateProgress();

            // 允许继续
            _btnNext.Enabled = true;

            if (e.AllCorrect)
            {
                MessageBox.Show(
                    $"🎉 太棒了！全部回答正确！\n" +
                    $"获得经验值：+{e.TotalXp} XP",
                    "测验完成",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void OnFillBlankCompleted(object sender, FillBlankCompletedEventArgs e)
        {
            _totalEarnedXp += e.TotalXp;
            UpdateProgress();

            // 允许继续（即使不完全正确也可以继续）
            _btnNext.Enabled = true;
        }

        private void UpdateProgress()
        {
            _lblProgress.Text = $"{_currentSectionIndex + 1} / {_phase.Sections.Count}";
            _lblXp.Text = $"💎 {_totalEarnedXp} / {_phase.TotalXp} XP";

            // 更新下一步按钮文本
            if (_currentSectionIndex >= _phase.Sections.Count - 1)
            {
                _btnNext.Text = "完成课程 ✓";
            }
            else
            {
                _btnNext.Text = "下一步 ▶";
            }
        }

        private void ShowCompletionPanel()
        {
            _contentPanel.Controls.Clear();
            _btnNext.Enabled = false;
            _btnPrev.Enabled = false;

            var completionPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ColorBgDark
            };

            // 恭喜图标
            var lblIcon = new Label
            {
                Text = "🎉",
                Font = new Font("Segoe UI Emoji", 72),
                AutoSize = true,
                Location = new Point(350, 80)
            };
            completionPanel.Controls.Add(lblIcon);

            // 标题
            var lblTitle = new Label
            {
                Text = "恭喜完成知识学习阶段！",
                Font = new Font("Microsoft YaHei", 20, FontStyle.Bold),
                ForeColor = ColorAccentGold,
                AutoSize = true,
                Location = new Point(220, 200)
            };
            completionPanel.Controls.Add(lblTitle);

            // 获得XP
            var lblXp = new Label
            {
                Text = $"💎 你获得了 {_totalEarnedXp} / {_phase.TotalXp} XP",
                Font = new Font("Microsoft YaHei", 16),
                ForeColor = ColorAccentGreen,
                AutoSize = true,
                Location = new Point(280, 280)
            };
            completionPanel.Controls.Add(lblXp);

            // 提示
            var lblHint = new Label
            {
                Text = "接下来请完成实践编程挑战吧！",
                Font = new Font("Microsoft YaHei", 12),
                ForeColor = ColorTextLight,
                AutoSize = true,
                Location = new Point(270, 350)
            };
            completionPanel.Controls.Add(lblHint);

            // 关闭按钮
            var btnClose = new Button
            {
                Text = "进入下一阶段 →",
                Size = new Size(180, 50),
                Location = new Point(360, 420),
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorAccentGreen,
                ForeColor = ColorBgDark,
                Font = new Font("Microsoft YaHei", 13, FontStyle.Bold)
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => {
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            completionPanel.Controls.Add(btnClose);

            _contentPanel.Controls.Add(completionPanel);
        }
    }
}

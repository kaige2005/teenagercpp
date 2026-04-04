using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TeenCppEdu.Core.Models;

namespace TeenCppEdu.UI.Controls
{
    /// <summary>
    /// 知识测验面板
    /// </summary>
    public class QuizPanel : Panel
    {
        private readonly QuizSection _section;
        private readonly List<QuestionPanel> _questionPanels = new List<QuestionPanel>();
        private readonly Color ColorBgDark = Color.FromArgb(45, 52, 70);
        private readonly Color ColorBgCard = Color.FromArgb(55, 65, 90);
        private readonly Color ColorTextLight = Color.FromArgb(240, 240, 240);
        private readonly Color ColorAccentGold = Color.FromArgb(255, 200, 80);
        private readonly Color ColorAccentGreen = Color.FromArgb(100, 200, 120);
        private readonly Color ColorAccentRed = Color.FromArgb(255, 100, 100);

        public event EventHandler<QuizCompletedEventArgs> QuizCompleted;

        // 当前累计的XP
        private int _totalEarnedXp = 0;

        public QuizPanel(QuizSection section)
        {
            _section = section;
            InitializeComponent();
        }

        public int TotalEarnedXp => _totalEarnedXp;

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = ColorBgDark;
            this.Padding = new Padding(30);
            this.AutoScroll = true;

            int y = 30;

            // 标题
            var lblTitle = new Label
            {
                Text = _section.Title,
                Font = new Font("Microsoft YaHei", 18, FontStyle.Bold),
                ForeColor = ColorAccentGold,
                AutoSize = true,
                Location = new Point(30, y),
                MaximumSize = new Size(700, 0)
            };
            this.Controls.Add(lblTitle);
            y += lblTitle.Height + 20;

            // 说明
            var lblDesc = new Label
            {
                Text = "📝 回答下列问题，检验你的学习成果！",
                Font = new Font("Microsoft YaHei", 11),
                ForeColor = ColorTextLight,
                AutoSize = true,
                Location = new Point(30, y)
            };
            this.Controls.Add(lblDesc);
            y += 50;

            // 问题列表
            foreach (var question in _section.Questions)
            {
                var qPanel = new QuestionPanel(question);
                qPanel.Location = new Point(30, y);
                qPanel.AnswerSelected += OnAnswerSelected;
                this.Controls.Add(qPanel);
                _questionPanels.Add(qPanel);
                y += qPanel.Height + 20;
            }

            // 进度显示
            var progressPanel = new Panel
            {
                Size = new Size(700, 60),
                Location = new Point(30, y + 20),
                BackColor = ColorBgCard
            };

            var lblProgress = new Label
            {
                Text = $"💎 XP: 0 / {_section.Questions.Sum(q => q.Xp)}",
                Font = new Font("Microsoft YaHei", 14, FontStyle.Bold),
                ForeColor = ColorAccentGreen,
                AutoSize = true,
                Location = new Point(20, 15),
                Name = "lblProgress"
            };
            progressPanel.Controls.Add(lblProgress);
            this.Controls.Add(progressPanel);
        }

        private void OnAnswerSelected(object sender, AnswerSelectedEventArgs e)
        {
            // 重新计算总XP
            _totalEarnedXp = _questionPanels
                .Where(qp => qp.IsCorrect)
                .Sum(qp => qp.QuestionXp);

            // 更新进度显示
            var lblProgress = this.Controls.Find("lblProgress", true).FirstOrDefault() as Label;
            if (lblProgress != null)
            {
                var totalXp = _section.Questions.Sum(q => q.Xp);
                lblProgress.Text = $"💎 XP: {_totalEarnedXp} / {totalXp}";
            }

            // 检查是否完成所有题目
            if (_questionPanels.All(qp => qp.IsAnswered))
            {
                var allCorrect = _questionPanels.All(qp => qp.IsCorrect);
                QuizCompleted?.Invoke(this, new QuizCompletedEventArgs
                {
                    TotalXp = _totalEarnedXp,
                    AllCorrect = allCorrect,
                    TotalQuestions = _questionPanels.Count
                });
            }
        }
    }

    /// <summary>
    /// 单个问题面板
    /// </summary>
    public class QuestionPanel : Panel
    {
        private readonly QuizQuestion _question;
        private readonly List<RadioButton> _options = new List<RadioButton>();
        private readonly Color ColorBgCard = Color.FromArgb(55, 65, 90);
        private readonly Color ColorAccentGreen = Color.FromArgb(100, 200, 120);
        private readonly Color ColorAccentRed = Color.FromArgb(255, 100, 100);
        private readonly Color ColorTextLight = Color.FromArgb(240, 240, 240);

        public event EventHandler<AnswerSelectedEventArgs> AnswerSelected;

        public bool IsAnswered { get; private set; }
        public bool IsCorrect { get; private set; }
        public int QuestionXp => _question.Xp;

        public QuizQuestion Question => _question;

        public QuestionPanel(QuizQuestion question)
        {
            _question = question;
            this.Size = new Size(700, CalculateHeight());
            this.BackColor = ColorBgCard;
            InitializeComponent();
        }

        private int CalculateHeight()
        {
            return 100 + _question.Options.Count * 40;
        }

        private void InitializeComponent()
        {
            int y = 15;

            // 问题文本
            var lblQuestion = new Label
            {
                Text = _question.Question,
                Font = new Font("Microsoft YaHei", 12, FontStyle.Bold),
                ForeColor = ColorTextLight,
                AutoSize = true,
                Location = new Point(20, y),
                MaximumSize = new Size(660, 0)
            };
            this.Controls.Add(lblQuestion);
            y += lblQuestion.Height + 20;

            // 选项组
            var group = new Panel
            {
                Location = new Point(20, y),
                Size = new Size(660, _question.Options.Count * 40),
                BackColor = ColorBgCard
            };

            int optY = 0;
            for (int i = 0; i < _question.Options.Count; i++)
            {
                var rb = new RadioButton
                {
                    Text = _question.Options[i],
                    Font = new Font("Microsoft YaHei", 11),
                    ForeColor = ColorTextLight,
                    AutoSize = true,
                    Location = new Point(0, optY),
                    Tag = i  // 存储选项索引
                };
                rb.CheckedChanged += OnOptionChecked;
                _options.Add(rb);
                group.Controls.Add(rb);
                optY += 40;
            }
            this.Controls.Add(group);

            // 结果显示标签
            var lblResult = new Label
            {
                Name = "lblResult",
                Text = "",
                Font = new Font("Microsoft YaHei", 11, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, y + group.Height + 10),
                Visible = false
            };
            this.Controls.Add(lblResult);
        }

        private void OnOptionChecked(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb == null || !rb.Checked) return;

            IsAnswered = true;
            int selectedIndex = (int)rb.Tag;
            IsCorrect = selectedIndex == _question.Answer;

            // 显示结果和解释
            var lblResult = this.Controls.Find("lblResult", true).FirstOrDefault() as Label;
            if (lblResult != null)
            {
                lblResult.Visible = true;
                if (IsCorrect)
                {
                    lblResult.Text = $"✓ 正确！+{_question.Xp} XP\n{_question.Explanation}";
                    lblResult.ForeColor = ColorAccentGreen;
                }
                else
                {
                    lblResult.Text = $"✗ 错误\n{_question.Explanation}";
                    lblResult.ForeColor = ColorAccentRed;
                }
            }

            // 禁用其他选项
            foreach (var opt in _options)
            {
                opt.Enabled = false;
            }

            AnswerSelected?.Invoke(this, new AnswerSelectedEventArgs
            {
                IsCorrect = IsCorrect,
                EarnedXp = IsCorrect ? _question.Xp : 0
            });
        }
    }

    public class AnswerSelectedEventArgs : EventArgs
    {
        public bool IsCorrect { get; set; }
        public int EarnedXp { get; set; }
    }

    public class QuizCompletedEventArgs : EventArgs
    {
        public int TotalXp { get; set; }
        public bool AllCorrect { get; set; }
        public int TotalQuestions { get; set; }
    }
}

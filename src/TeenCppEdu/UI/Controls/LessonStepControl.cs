using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TeenCppEdu.Core.Models;

namespace TeenCppEdu.UI.Controls
{
    /// <summary>
    /// 课程步骤说明控件
    /// </summary>
    public class LessonStepControl : Panel
    {
        private readonly List<LessonStep> _steps;
        private int _currentStep = 0;

        private readonly Color ColorActive = Color.FromArgb(100, 200, 120);
        private readonly Color ColorPending = Color.FromArgb(80, 90, 110);
        private readonly Color ColorBgDark = Color.FromArgb(45, 52, 70);

        private List<Panel> _stepPanels = new List<Panel>();

        public LessonStepControl(List<LessonStep> steps)
        {
            _steps = steps ?? new List<LessonStep>();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.BackColor = ColorBgDark;
            this.AutoScroll = true;

            // 标题
            var lblTitle = new Label
            {
                Text = "📚 学习步骤",
                Font = new Font("Microsoft YaHei", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(240, 240, 240),
                AutoSize = true,
                Location = new Point(10, 10)
            };
            this.Controls.Add(lblTitle);

            int y = 50;
            for (int i = 0; i < _steps.Count; i++)
            {
                var panel = CreateStepPanel(_steps[i], i + 1, y);
                _stepPanels.Add(panel);
                this.Controls.Add(panel);
                y += panel.Height + 10;
            }

            // 提示区域
            var hintPanel = new Panel
            {
                Location = new Point(10, y + 10),
                Size = new Size(320, 150),
                BackColor = Color.FromArgb(60, 70, 100),
                Padding = new Padding(10)
            };

            var lblHintTitle = new Label
            {
                Text = "💡 温馨提示",
                Font = new Font("Microsoft YaHei", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 200, 80),
                AutoSize = true,
                Location = new Point(10, 10)
            };

            var lblHint = new Label
            {
                Text = "1. 仔细阅读每个步骤\n" +
                       "2. 按提示修改代码\n" +
                       "3. 检查通过后会解锁下一课\n" +
                       "4. 可以用 Dev-C++ 运行你的程序",
                Font = new Font("Microsoft YaHei", 10),
                ForeColor = Color.FromArgb(220, 220, 220),
                AutoSize = true,
                Location = new Point(10, 40)
            };

            hintPanel.Controls.Add(lblHintTitle);
            hintPanel.Controls.Add(lblHint);
            this.Controls.Add(hintPanel);
        }

        private Panel CreateStepPanel(LessonStep step, int stepNumber, int y)
        {
            var panel = new Panel
            {
                Location = new Point(10, y),
                Size = new Size(320, 100),
                BackColor = stepNumber == 1 ? ColorActive : ColorPending,
                Tag = stepNumber
            };

            // 步骤编号圆圈
            var lblNum = new Label
            {
                Text = stepNumber.ToString(),
                Font = new Font("Microsoft YaHei", 14, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(30, 30, 50),
                Size = new Size(35, 35),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(10, 15)
            };

            // 步骤标题
            var lblTitle = new Label
            {
                Text = $"步骤 {stepNumber}: {step.Title}",
                Font = new Font("Microsoft YaHei", 11, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(55, 10)
            };

            // 步骤描述
            var lblDesc = new Label
            {
                Text = step.Description,
                Font = new Font("Microsoft YaHei", 9),
                ForeColor = Color.FromArgb(230, 230, 230),
                Size = new Size(260, 40),
                Location = new Point(55, 35)
            };

            // 提示（点击展开）
            var lblHint = new Label
            {
                Text = "💡 点击看提示",
                Font = new Font("Microsoft YaHei", 9, FontStyle.Underline),
                ForeColor = Color.FromArgb(200, 200, 150),
                AutoSize = true,
                Location = new Point(55, 75),
                Cursor = Cursors.Hand,
                Tag = step.Hint,
                Visible = !string.IsNullOrEmpty(step.Hint)
            };
            lblHint.Click += (s, e) =>
            {
                MessageBox.Show(step.Hint, $"步骤 {stepNumber} 提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            panel.Controls.Add(lblNum);
            panel.Controls.Add(lblTitle);
            panel.Controls.Add(lblDesc);
            panel.Controls.Add(lblHint);

            return panel;
        }

        public void SetCurrentStep(int stepNumber)
        {
            _currentStep = stepNumber;
            for (int i = 0; i < _stepPanels.Count; i++)
            {
                _stepPanels[i].BackColor = (i < stepNumber) ? ColorActive : ColorPending;
            }
        }
    }
}

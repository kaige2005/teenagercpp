using System;
using System.Drawing;
using System.Windows.Forms;
using TeenCppEdu.Core.Models;

namespace TeenCppEdu.UI.Controls
{
    /// <summary>
    /// 概念讲解面板
    /// </summary>
    public class ConceptPanel : Panel
    {
        private readonly ConceptSection _section;
        private readonly Color ColorBgDark = Color.FromArgb(45, 52, 70);
        private readonly Color ColorBgCard = Color.FromArgb(55, 65, 90);
        private readonly Color ColorTextLight = Color.FromArgb(240, 240, 240);
        private readonly Color ColorAccentGold = Color.FromArgb(255, 200, 80);
        private readonly Color ColorAccentGreen = Color.FromArgb(100, 200, 120);

        public ConceptPanel(ConceptSection section)
        {
            _section = section;
            InitializeComponent();
        }

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

            // 概念图标
            var lblIcon = new Label
            {
                Text = "📖",
                Font = new Font("Segoe UI Emoji", 36),
                AutoSize = true,
                Location = new Point(30, y)
            };
            this.Controls.Add(lblIcon);
            y += 70;

            // 解释内容
            var lblContent = new Label
            {
                Text = _section.Content,
                Font = new Font("Microsoft YaHei", 12),
                ForeColor = ColorTextLight,
                AutoSize = true,
                Location = new Point(30, y),
                MaximumSize = new Size(700, 0)
            };
            this.Controls.Add(lblContent);
            y += lblContent.Height + 30;

            // 可视化元素
            if (_section.Visual != null)
            {
                var visualPanel = CreateVisualPanel();
                visualPanel.Location = new Point(30, y);
                this.Controls.Add(visualPanel);
                y += visualPanel.Height + 30;
            }

            // 关键知识点
            if (_section.KeyPoints != null && _section.KeyPoints.Count > 0)
            {
                var keyPointsPanel = CreateKeyPointsPanel();
                keyPointsPanel.Location = new Point(30, y);
                this.Controls.Add(keyPointsPanel);
            }
        }

        private Panel CreateVisualPanel()
        {
            var panel = new Panel
            {
                Size = new Size(700, 150),
                BackColor = ColorBgCard,
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblCaption = new Label
            {
                Text = _section.Visual.Caption,
                Font = new Font("Microsoft YaHei", 11, FontStyle.Italic),
                ForeColor = Color.FromArgb(200, 200, 200),
                AutoSize = true,
                Location = new Point(10, 10)
            };
            panel.Controls.Add(lblCaption);

            // 根据类型显示不同的图示
            if (_section.Visual.Type == "array_diagram")
            {
                var diagram = CreateArrayDiagram();
                diagram.Location = new Point(10, 40);
                panel.Controls.Add(diagram);
            }

            return panel;
        }

        private Panel CreateArrayDiagram()
        {
            var panel = new Panel
            {
                Size = new Size(680, 100),
                BackColor = ColorBgCard
            };

            // 绘制数组示意图
            string[] labels = { "arr[0]", "arr[1]", "arr[2]", "arr[3]", "arr[4]" };
            string[] values = { "10", "20", "30", "40", "50" };
            int x = 20;

            for (int i = 0; i < 5; i++)
            {
                // 数组格子
                var box = new Panel
                {
                    Size = new Size(80, 80),
                    Location = new Point(x, 10),
                    BackColor = Color.FromArgb(70, 80, 110),
                    BorderStyle = BorderStyle.FixedSingle
                };

                // 索引标签
                var lblIndex = new Label
                {
                    Text = labels[i],
                    Font = new Font("Consolas", 10),
                    ForeColor = ColorAccentGold,
                    AutoSize = true,
                    Location = new Point(10, 5)
                };
                box.Controls.Add(lblIndex);

                // 值
                var lblValue = new Label
                {
                    Text = values[i],
                    Font = new Font("Consolas", 16, FontStyle.Bold),
                    ForeColor = ColorAccentGreen,
                    AutoSize = true,
                    Location = new Point(25, 35)
                };
                box.Controls.Add(lblValue);

                panel.Controls.Add(box);
                x += 90;
            }

            return panel;
        }

        private Panel CreateKeyPointsPanel()
        {
            var panel = new Panel
            {
                Size = new Size(700, _section.KeyPoints.Count * 40 + 50),
                BackColor = ColorBgCard
            };

            var lblTitle = new Label
            {
                Text = "📝 关键知识点",
                Font = new Font("Microsoft YaHei", 12, FontStyle.Bold),
                ForeColor = ColorAccentGreen,
                AutoSize = true,
                Location = new Point(15, 15)
            };
            panel.Controls.Add(lblTitle);

            int y = 50;
            foreach (var point in _section.KeyPoints)
            {
                var lblPoint = new Label
                {
                    Text = "• " + point,
                    Font = new Font("Microsoft YaHei", 11),
                    ForeColor = ColorTextLight,
                    AutoSize = true,
                    Location = new Point(20, y),
                    MaximumSize = new Size(660, 0)
                };
                panel.Controls.Add(lblPoint);
                y += 40;
            }

            return panel;
        }
    }
}

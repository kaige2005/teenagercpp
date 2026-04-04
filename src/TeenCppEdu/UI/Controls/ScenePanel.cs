using System;
using System.Drawing;
using System.Windows.Forms;
using TeenCppEdu.Core.Models;

namespace TeenCppEdu.UI.Controls
{
    /// <summary>
    /// 场景故事面板
    /// </summary>
    public class ScenePanel : Panel
    {
        private readonly SceneSection _section;
        private readonly Color ColorBgDark = Color.FromArgb(45, 52, 70);
        private readonly Color ColorTextLight = Color.FromArgb(240, 240, 240);
        private readonly Color ColorAccentGold = Color.FromArgb(255, 200, 80);

        public ScenePanel(SceneSection section)
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

            // 标题
            var lblTitle = new Label
            {
                Text = _section.Title,
                Font = new Font("Microsoft YaHei", 18, FontStyle.Bold),
                ForeColor = ColorAccentGold,
                AutoSize = true,
                Location = new Point(30, 30),
                MaximumSize = new Size(700, 0)
            };
            this.Controls.Add(lblTitle);

            // 场景图标
            var lblIcon = new Label
            {
                Text = "🎭",
                Font = new Font("Segoe UI Emoji", 48),
                AutoSize = true,
                Location = new Point(30, 80)
            };
            this.Controls.Add(lblIcon);

            // 内容
            var lblContent = new Label
            {
                Text = _section.Content,
                Font = new Font("Microsoft YaHei", 12),
                ForeColor = ColorTextLight,
                AutoSize = true,
                Location = new Point(30, 160),
                MaximumSize = new Size(700, 0)
            };
            this.Controls.Add(lblContent);
        }
    }
}

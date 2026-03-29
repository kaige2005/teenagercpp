using System;
using System.Drawing;
using System.Windows.Forms;

namespace TeenCppEdu.UI.Forms
{
    /// <summary>
    /// 导师手动放行对话框
    /// </summary>
    public partial class ManualApproveDialog : Form
    {
        private TextBox txtTeacherName;
        private TextBox txtNote;
        private Button btnConfirm;
        private Button btnCancel;

        public string TeacherName => txtTeacherName.Text.Trim();
        public string Note => txtNote.Text.Trim();

        private readonly Color ColorBgDark = Color.FromArgb(45, 52, 70);
        private readonly Color ColorAccentGreen = Color.FromArgb(100, 200, 120);
        private readonly Color ColorTextLight = Color.FromArgb(240, 240, 240);

        public ManualApproveDialog()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "🔓 导师手动放行";
            this.Size = new Size(450, 350);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = ColorBgDark;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var lblWarning = new Label
            {
                Text = "⚠️ 学生代码未通过自动检查，确定要放行吗？",
                Font = new Font("Microsoft YaHei", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 200, 100),
                AutoSize = true,
                Location = new Point(20, 20)
            };

            var lblName = new Label
            {
                Text = "导师姓名：",
                Font = new Font("Microsoft YaHei", 10),
                ForeColor = ColorTextLight,
                AutoSize = true,
                Location = new Point(20, 60)
            };

            txtTeacherName = new TextBox
            {
                Location = new Point(120, 58),
                Size = new Size(280, 25),
                Font = new Font("Microsoft YaHei", 10)
            };

            var lblNote = new Label
            {
                Text = "放行备注：",
                Font = new Font("Microsoft YaHei", 10),
                ForeColor = ColorTextLight,
                AutoSize = true,
                Location = new Point(20, 100)
            };

            txtNote = new TextBox
            {
                Location = new Point(20, 130),
                Size = new Size(380, 100),
                Font = new Font("Microsoft YaHei", 10),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };

            var lblHint = new Label
            {
                Text = "请简要说明放行的原因（如：已口头指导、特殊情况等）",
                Font = new Font("Microsoft YaHei", 9),
                ForeColor = Color.FromArgb(150, 150, 150),
                AutoSize = true,
                Location = new Point(20, 240)
            };

            btnConfirm = new Button
            {
                Text = "✓ 确认放行",
                Location = new Point(220, 270),
                Size = new Size(180, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorAccentGreen,
                ForeColor = ColorBgDark,
                Font = new Font("Microsoft YaHei", 11, FontStyle.Bold)
            };
            btnConfirm.FlatAppearance.BorderSize = 0;
            btnConfirm.Click += BtnConfirm_Click;

            btnCancel = new Button
            {
                Text = "取消",
                Location = new Point(20, 270),
                Size = new Size(100, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(100, 100, 110),
                ForeColor = ColorTextLight,
                Font = new Font("Microsoft YaHei", 11)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(lblWarning);
            this.Controls.Add(lblName);
            this.Controls.Add(txtTeacherName);
            this.Controls.Add(lblNote);
            this.Controls.Add(txtNote);
            this.Controls.Add(lblHint);
            this.Controls.Add(btnConfirm);
            this.Controls.Add(btnCancel);
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TeacherName))
            {
                MessageBox.Show("请输入导师姓名", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(Note))
            {
                var result = MessageBox.Show(
                    "没有填写备注，确定要继续放行吗？",
                    "确认",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                if (result == DialogResult.No) return;
            }

            // 最终确认
            var final = MessageBox.Show(
                $"确定以「{TeacherName}」的身份对此代码进行手动放行吗？\n\n" +
                "放行后学生将获得课程通关奖励，此操作会被记录。",
                "最终确认",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (final == DialogResult.Yes)
            {
                this.DialogResult = DialogResult.OK;
            }
        }
    }
}

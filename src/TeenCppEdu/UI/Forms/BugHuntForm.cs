using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TeenCppEdu.Core.Models;

namespace TeenCppEdu.UI.Forms
{
    /// <summary>
    /// Bug猎手挑战模式窗体
    /// </summary>
    public partial class BugHuntForm : Form
    {
        private readonly ChallengePhase _phase;
        private readonly string _lessonId;
        private int _currentHintIndex = 0;
        private bool _hasFoundBug = false;
        private int _earnedXp = 0;

        private Panel _codePanel;
        private RichTextBox _txtBuggyCode;
        private RichTextBox _txtFix;
        private Label _lblHint;
        private Label _lblStatus;
        private Label _lblXp;
        private Button _btnCheck;
        private Button _btnHint;
        private Button _btnFoundIt;

        private readonly int _targetLine; // Bug所在行号

        private readonly Color ColorBgDark = Color.FromArgb(45, 52, 70);
        private readonly Color ColorBgEditor = Color.FromArgb(30, 30, 40);
        private readonly Color ColorBgCard = Color.FromArgb(55, 65, 90);
        private readonly Color ColorAccentGreen = Color.FromArgb(100, 200, 120);
        private readonly Color ColorAccentGold = Color.FromArgb(255, 200, 80);
        private readonly Color ColorAccentRed = Color.FromArgb(255, 100, 100);
        private readonly Color ColorTextLight = Color.FromArgb(240, 240, 240);
        private readonly Color ColorBugLine = Color.FromArgb(80, 40, 40);

        public int EarnedXp => _earnedXp;
        public bool IsCompleted => _hasFoundBug;

        public BugHuntForm(ChallengePhase phase, string lessonId)
        {
            _phase = phase;
            _lessonId = lessonId;
            _targetLine = FindTargetLine();
            InitializeComponent();
        }

        private int FindTargetLine()
        {
            // 分析buggyCode找出包含关键模式的行
            var lines = _phase.BuggyCode.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                // 查找常见的Bug初始化模式
                if (lines[i].Contains("= 0") || lines[i].Contains(" int max") || lines[i].Contains(" int min"))
                {
                    return i;
                }
            }
            return 5; // 默认第6行
        }

        private void InitializeComponent()
        {
            this.Text = $"🐛 Bug猎手：{_phase.Title}";
            this.Size = new Size(1100, 800);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = ColorBgDark;

            // 顶部标题栏
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(35, 42, 60)
            };

            var lblTitle = new Label
            {
                Text = $"🐛 {_phase.Title}",
                Font = new Font("Microsoft YaHei", 16, FontStyle.Bold),
                ForeColor = ColorAccentGold,
                AutoSize = true,
                Location = new Point(20, 20)
            };
            headerPanel.Controls.Add(lblTitle);

            var lblDifficulty = new Label
            {
                Text = $"难度：{_phase.Difficulty}",
                Font = new Font("Microsoft YaHei", 11),
                ForeColor = GetDifficultyColor(_phase.Difficulty),
                AutoSize = true,
                Location = new Point(350, 24)
            };
            headerPanel.Controls.Add(lblDifficulty);

            _lblXp = new Label
            {
                Text = $"💎 0 / {_phase.RewardExp} XP",
                Font = new Font("Microsoft YaHei", 12, FontStyle.Bold),
                ForeColor = ColorAccentGreen,
                AutoSize = true,
                Location = new Point(550, 24)
            };
            headerPanel.Controls.Add(_lblXp);

            var btnClose = new Button
            {
                Text = "✕",
                Size = new Size(40, 40),
                Location = new Point(1030, 15),
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorAccentRed,
                ForeColor = Color.White
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            headerPanel.Controls.Add(btnClose);

            // 描述区域
            var descPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = ColorBgDark,
                Padding = new Padding(20)
            };

            var lblDesc = new Label
            {
                Text = _phase.Description,
                Font = new Font("Microsoft YaHei", 11),
                ForeColor = ColorTextLight,
                AutoSize = true,
                Location = new Point(20, 15),
                MaximumSize = new Size(1040, 0)
            };
            descPanel.Controls.Add(lblDesc);

            // 左侧：代码显示区
            var leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 650,
                Padding = new Padding(20),
                BackColor = ColorBgDark
            };

            var lblCodeTitle = new Label
            {
                Text = "📝 Bug代码（找出问题所在行）",
                Font = new Font("Microsoft YaHei", 12, FontStyle.Bold),
                ForeColor = ColorAccentRed,
                AutoSize = true,
                Location = new Point(20, 10)
            };
            leftPanel.Controls.Add(lblCodeTitle);

            _txtBuggyCode = new RichTextBox
            {
                Location = new Point(20, 40),
                Size = new Size(600, 450),
                BackColor = ColorBgEditor,
                ForeColor = ColorTextLight,
                Font = new Font("Consolas", 11),
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                Text = _phase.BuggyCode
            };
            _txtBuggyCode.MouseClick += OnCodeClicked;
            leftPanel.Controls.Add(_txtBuggyCode);

            // 高亮Bug行
            HighlightLine(_targetLine);

            // 右侧：提示和修复区
            var rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = ColorBgDark
            };

            // 状态显示
            _lblStatus = new Label
            {
                Text = "🔍 正在寻找Bug...",
                Font = new Font("Microsoft YaHei", 12, FontStyle.Bold),
                ForeColor = ColorAccentGold,
                AutoSize = true,
                Location = new Point(20, 10)
            };
            rightPanel.Controls.Add(_lblStatus);

            // 提示区域
            var hintPanel = new Panel
            {
                Location = new Point(20, 50),
                Size = new Size(380, 150),
                BackColor = ColorBgCard,
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblHintTitle = new Label
            {
                Text = "💡 提示",
                Font = new Font("Microsoft YaHei", 11, FontStyle.Bold),
                ForeColor = ColorAccentGold,
                AutoSize = true,
                Location = new Point(15, 12)
            };
            hintPanel.Controls.Add(lblHintTitle);

            _lblHint = new Label
            {
                Text = "点击下方按钮查看提示",
                Font = new Font("Microsoft YaHei", 10),
                ForeColor = ColorTextLight,
                AutoSize = true,
                Location = new Point(15, 45),
                MaximumSize = new Size(350, 0)
            };
            hintPanel.Controls.Add(_lblHint);

            _btnHint = new Button
            {
                Text = "显示提示",
                Size = new Size(100, 35),
                Location = new Point(255, 100),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(100, 150, 200),
                ForeColor = Color.White,
                Font = new Font("Microsoft YaHei", 10)
            };
            _btnHint.FlatAppearance.BorderSize = 0;
            _btnHint.Click += ShowNextHint;
            hintPanel.Controls.Add(_btnHint);

            rightPanel.Controls.Add(hintPanel);

            // "我发现Bug了"按钮
            _btnFoundIt = new Button
            {
                Text = "🐛 我找到Bug了！",
                Size = new Size(380, 50),
                Location = new Point(20, 220),
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorAccentRed,
                ForeColor = Color.White,
                Font = new Font("Microsoft YaHei", 13, FontStyle.Bold)
            };
            _btnFoundIt.FlatAppearance.BorderSize = 0;
            _btnFoundIt.Click += OnFoundItClicked;
            rightPanel.Controls.Add(_btnFoundIt);

            // 修复代码区
            var fixPanel = new Panel
            {
                Location = new Point(20, 290),
                Size = new Size(380, 200),
                BackColor = ColorBgCard,
                Visible = false,
                Name = "fixPanel"
            };

            var lblFixTitle = new Label
            {
                Text = "✏️ 输入修复代码",
                Font = new Font("Microsoft YaHei", 11, FontStyle.Bold),
                ForeColor = ColorAccentGreen,
                AutoSize = true,
                Location = new Point(15, 10)
            };
            fixPanel.Controls.Add(lblFixTitle);

            _txtFix = new RichTextBox
            {
                Location = new Point(15, 40),
                Size = new Size(350, 100),
                BackColor = ColorBgEditor,
                ForeColor = ColorTextLight,
                Font = new Font("Consolas", 11),
                BorderStyle = BorderStyle.FixedSingle
            };
            fixPanel.Controls.Add(_txtFix);

            _btnCheck = new Button
            {
                Text = "验证修复",
                Size = new Size(120, 40),
                Location = new Point(130, 150),
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorAccentGreen,
                ForeColor = ColorBgDark,
                Font = new Font("Microsoft YaHei", 11, FontStyle.Bold)
            };
            _btnCheck.FlatAppearance.BorderSize = 0;
            _btnCheck.Click += OnCheckClicked;
            fixPanel.Controls.Add(_btnCheck);

            rightPanel.Controls.Add(fixPanel);

            // 底部按钮面板
            var footerPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.FromArgb(35, 42, 60)
            };

            var btnSkip = new Button
            {
                Text = "跳过挑战",
                Size = new Size(120, 45),
                Location = new Point(20, 12),
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorBgCard,
                ForeColor = ColorTextLight,
                Font = new Font("Microsoft YaHei", 11)
            };
            btnSkip.FlatAppearance.BorderSize = 0;
            btnSkip.Click += (s, e) => {
                if (MessageBox.Show("确定要跳过这个挑战吗？你将不会获得经验值。", "跳过确认",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            };
            footerPanel.Controls.Add(btnSkip);

            // 组装界面
            this.Controls.Add(rightPanel);
            this.Controls.Add(leftPanel);
            this.Controls.Add(descPanel);
            this.Controls.Add(footerPanel);
            this.Controls.Add(headerPanel);
        }

        private void HighlightLine(int lineIndex)
        {
            // 设置选择范围来高亮行
            var lines = _phase.BuggyCode.Split('\n');
            int startPos = 0;
            for (int i = 0; i < lineIndex && i < lines.Length; i++)
            {
                startPos += lines[i].Length + 1; // +1 for newline
            }

            int length = lines[lineIndex].Length;
            _txtBuggyCode.Select(startPos, length);
            _txtBuggyCode.SelectionBackColor = ColorBugLine;
            _txtBuggyCode.SelectionColor = ColorAccentRed;
            _txtBuggyCode.DeselectAll();
        }

        private void OnCodeClicked(object sender, MouseEventArgs e)
        {
            // 获取点击位置对应的行号
            int charIndex = _txtBuggyCode.GetCharIndexFromPosition(e.Location);
            int lineIndex = _txtBuggyCode.GetLineFromCharIndex(charIndex);

            if (lineIndex == _targetLine)
            {
                _lblStatus.Text = "✓ 正确！你找到了Bug所在行！";
                _lblStatus.ForeColor = ColorAccentGreen;

                // 显示修复面板
                var fixPanel = this.Controls.Find("fixPanel", true).FirstOrDefault() as Panel;
                if (fixPanel != null) fixPanel.Visible = true;
            }
            else
            {
                _lblStatus.Text = "✗ 不是这一行，再仔细看看！";
                _lblStatus.ForeColor = ColorAccentRed;
            }
        }

        private void ShowNextHint(object sender, EventArgs e)
        {
            if (_currentHintIndex < _phase.Hints.Count)
            {
                _lblHint.Text = _phase.Hints[_currentHintIndex];
                _currentHintIndex++;

                if (_currentHintIndex >= _phase.Hints.Count)
                {
                    _btnHint.Enabled = false;
                    _btnHint.Text = "没有更多提示";
                }
            }
        }

        private void OnFoundItClicked(object sender, EventArgs e)
        {
            // 直接显示修复面板
            var fixPanel = this.Controls.Find("fixPanel", true).FirstOrDefault() as Panel;
            if (fixPanel != null)
            {
                fixPanel.Visible = !fixPanel.Visible;
            }
        }

        private void OnCheckClicked(object sender, EventArgs e)
        {
            string fix = _txtFix.Text.Trim();

            if (_phase.CheckFix(fix))
            {
                _hasFoundBug = true;
                _earnedXp = _phase.RewardExp;
                _lblXp.Text = $"💎 {_earnedXp} / {_phase.RewardExp} XP";

                MessageBox.Show(
                    $"🎉 太棒了！Bug修复成功！\n\n{_phase.Explanation}\n\n" +
                    $"获得经验值：+{_earnedXp} XP",
                    "挑战完成",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                _lblStatus.Text = "✗ 修复不正确，请重试！";
                _lblStatus.ForeColor = ColorAccentRed;
                MessageBox.Show("修复不正确，请仔细阅读提示后重试。", "修复失败",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private Color GetDifficultyColor(string difficulty)
        {
            switch (difficulty?.ToLower())
            {
                case "easy":
                    return ColorAccentGreen;
                case "medium":
                    return ColorAccentGold;
                case "hard":
                    return ColorAccentRed;
                default:
                    return ColorTextLight;
            }
        }
    }
}

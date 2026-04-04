using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TeenCppEdu.Core.Models;

namespace TeenCppEdu.UI.Controls
{
    /// <summary>
    /// 代码填空面板
    /// </summary>
    public class FillBlankPanel : Panel
    {
        private readonly FillBlankSection _section;
        private readonly Dictionary<string, TextBox> _blankInputs = new Dictionary<string, TextBox>();
        private readonly Dictionary<string, bool> _blankResults = new Dictionary<string, bool>();
        private readonly Color ColorBgDark = Color.FromArgb(45, 52, 70);
        private readonly Color ColorBgEditor = Color.FromArgb(30, 30, 40);
        private readonly Color ColorBgCard = Color.FromArgb(55, 65, 90);
        private readonly Color ColorTextLight = Color.FromArgb(240, 240, 240);
        private readonly Color ColorAccentGold = Color.FromArgb(255, 200, 80);
        private readonly Color ColorAccentGreen = Color.FromArgb(100, 200, 120);
        private readonly Color ColorAccentRed = Color.FromArgb(255, 100, 100);

        public event EventHandler<FillBlankCompletedEventArgs> Completed;

        private int _totalEarnedXp = 0;

        public FillBlankPanel(FillBlankSection section)
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
            y += lblTitle.Height + 15;

            // 说明
            var lblDesc = new Label
            {
                Text = _section.Description,
                Font = new Font("Microsoft YaHei", 11),
                ForeColor = ColorTextLight,
                AutoSize = true,
                Location = new Point(30, y),
                MaximumSize = new Size(700, 0)
            };
            this.Controls.Add(lblDesc);
            y += lblDesc.Height + 25;

            // 代码填空区域
            var codePanel = CreateCodePanel();
            codePanel.Location = new Point(30, y);
            this.Controls.Add(codePanel);
            y += codePanel.Height + 25;

            // 答案检查按钮
            var btnCheck = new Button
            {
                Text = "✅ 检查答案",
                Font = new Font("Microsoft YaHei", 12, FontStyle.Bold),
                Size = new Size(150, 45),
                Location = new Point(30, y),
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorAccentGreen,
                ForeColor = ColorBgDark
            };
            btnCheck.FlatAppearance.BorderSize = 0;
            btnCheck.Click += OnCheckClicked;
            this.Controls.Add(btnCheck);
            y += 65;

            // 结果显示区域
            var resultPanel = new Panel
            {
                Name = "resultPanel",
                Size = new Size(700, 80),
                Location = new Point(30, y),
                BackColor = ColorBgCard,
                Visible = false
            };
            this.Controls.Add(resultPanel);
            y += 100;

            // XP进度
            var xpPanel = new Panel
            {
                Name = "xpPanel",
                Size = new Size(700, 60),
                Location = new Point(30, y),
                BackColor = ColorBgCard,
                Visible = false
            };

            var lblXp = new Label
            {
                Name = "lblXp",
                Text = "💎 XP: 0 / " + _section.Blanks.Sum(b => b.Xp),
                Font = new Font("Microsoft YaHei", 14, FontStyle.Bold),
                ForeColor = ColorAccentGreen,
                AutoSize = true,
                Location = new Point(20, 15)
            };
            xpPanel.Controls.Add(lblXp);
            this.Controls.Add(xpPanel);
        }

        private Panel CreateCodePanel()
        {
            var panel = new Panel
            {
                Size = new Size(700, CalculateCodeHeight()),
                BackColor = ColorBgEditor,
                BorderStyle = BorderStyle.FixedSingle
            };

            // 将代码中的填空标记 ___X___ 替换为输入框
            var lines = _section.Code.Split('\n');
            int y = 10;
            int inputIndex = 0;

            foreach (var line in lines)
            {
                var linePanel = CreateCodeLine(line, ref inputIndex, 680);
                linePanel.Location = new Point(10, y);
                panel.Controls.Add(linePanel);
                y += linePanel.Height + 5;
            }

            return panel;
        }

        private int CalculateCodeHeight()
        {
            var lines = _section.Code.Split('\n');
            return lines.Length * 30 + 30;
        }

        private Panel CreateCodeLine(string line, ref int inputIndex, int maxWidth)
        {
            var panel = new Panel
            {
                AutoSize = true,
                BackColor = ColorBgEditor
            };

            // 解析填空标记 ___X___
            var matches = Regex.Matches(line, @"___([A-Z]+)___");
            if (matches.Count == 0)
            {
                // 没有填空，普通代码行
                var lbl = new Label
                {
                    Text = line,
                    Font = new Font("Consolas", 11),
                    ForeColor = ColorTextLight,
                    AutoSize = true,
                    Location = new Point(0, 0)
                };
                panel.Controls.Add(lbl);
                panel.Height = 25;
                return panel;
            }

            // 有填空，分段处理
            int x = 0;
            int lastPos = 0;
            int lineHeight = 28;

            foreach (Match match in matches)
            {
                // 填空前的普通文本
                if (match.Index > lastPos)
                {
                    var text = line.Substring(lastPos, match.Index - lastPos);
                    var lbl = new Label
                    {
                        Text = text,
                        Font = new Font("Consolas", 11),
                        ForeColor = ColorTextLight,
                        AutoSize = true,
                        Location = new Point(x, 3)
                    };
                    panel.Controls.Add(lbl);
                    x += TextRenderer.MeasureText(text, lbl.Font).Width;
                }

                // 填空输入框
                string blankId = match.Groups[1].Value;
                var blankDef = _section.Blanks.FirstOrDefault(b => b.Id == blankId);

                var txtBlank = new TextBox
                {
                    Name = $"blank_{blankId}",
                    Width = 100,
                    Height = 22,
                    Location = new Point(x, 0),
                    Font = new Font("Consolas", 11),
                    BackColor = Color.FromArgb(50, 50, 60),
                    ForeColor = ColorTextLight,
                    BorderStyle = BorderStyle.FixedSingle,
                    Tag = blankDef  // 存储BlankField引用
                };
                txtBlank.GotFocus += (s, e) => txtBlank.BackColor = Color.FromArgb(60, 60, 80);
                txtBlank.LostFocus += (s, e) => txtBlank.BackColor = Color.FromArgb(50, 50, 60);
                panel.Controls.Add(txtBlank);
                _blankInputs[blankId] = txtBlank;
                _blankResults[blankId] = false;

                x += 105;
                lastPos = match.Index + match.Length;
            }

            // 最后一个填空后的文本
            if (lastPos < line.Length)
            {
                var text = line.Substring(lastPos);
                var lbl = new Label
                {
                    Text = text,
                    Font = new Font("Consolas", 11),
                    ForeColor = ColorTextLight,
                    AutoSize = true,
                    Location = new Point(x, 3)
                };
                panel.Controls.Add(lbl);
            }

            panel.Height = lineHeight;
            return panel;
        }

        private void OnCheckClicked(object sender, EventArgs e)
        {
            _totalEarnedXp = 0;
            bool allCorrect = true;
            var results = new List<BlankResult>();

            foreach (var kvp in _blankInputs)
            {
                string blankId = kvp.Key;
                var txtBox = kvp.Value;
                var blankDef = txtBox.Tag as BlankField;

                string userAnswer = txtBox.Text.Trim();
                bool isCorrect = blankDef?.CheckAnswer(userAnswer) ?? false;
                _blankResults[blankId] = isCorrect;

                if (isCorrect)
                {
                    _totalEarnedXp += blankDef.Xp;
                    txtBox.BackColor = Color.FromArgb(40, 80, 60);
                    txtBox.ReadOnly = true;
                    txtBox.ForeColor = ColorAccentGreen;
                }
                else
                {
                    allCorrect = false;
                    txtBox.BackColor = Color.FromArgb(80, 50, 50);
                    txtBox.ForeColor = ColorAccentRed;
                }

                results.Add(new BlankResult
                {
                    Id = blankId,
                    IsCorrect = isCorrect,
                    Xp = isCorrect ? blankDef.Xp : 0,
                    Hint = blankDef?.Hint
                });
            }

            // 更新结果显示
            var resultPanel = this.Controls.Find("resultPanel", true).FirstOrDefault() as Panel;
            if (resultPanel != null)
            {
                resultPanel.Visible = true;
                resultPanel.Controls.Clear();

                var lblResult = new Label
                {
                    Text = allCorrect ?
                        $"✅ 全部正确！获得 {_totalEarnedXp} XP" :
                        $"⚠️ 部分错误，获得 {_totalEarnedXp} XP\n提示：错误的答案已标红，请修改后重试",
                    Font = new Font("Microsoft YaHei", 12, FontStyle.Bold),
                    ForeColor = allCorrect ? ColorAccentGreen : ColorAccentGold,
                    AutoSize = true,
                    Location = new Point(20, 20),
                    MaximumSize = new Size(660, 0)
                };
                resultPanel.Controls.Add(lblResult);
            }

            // 更新XP显示
            var xpPanel = this.Controls.Find("xpPanel", true).FirstOrDefault() as Panel;
            if (xpPanel != null)
            {
                xpPanel.Visible = true;
                var lblXp = xpPanel.Controls.Find("lblXp", true).FirstOrDefault() as Label;
                if (lblXp != null)
                {
                    lblXp.Text = $"💎 XP: {_totalEarnedXp} / {_section.Blanks.Sum(b => b.Xp)}";
                }
            }

            // 触发完成事件
            Completed?.Invoke(this, new FillBlankCompletedEventArgs
            {
                TotalXp = _totalEarnedXp,
                AllCorrect = allCorrect,
                Results = results
            });
        }
    }

    public class BlankResult
    {
        public string Id { get; set; }
        public bool IsCorrect { get; set; }
        public int Xp { get; set; }
        public string Hint { get; set; }
    }

    public class FillBlankCompletedEventArgs : EventArgs
    {
        public int TotalXp { get; set; }
        public bool AllCorrect { get; set; }
        public List<BlankResult> Results { get; set; }
    }
}

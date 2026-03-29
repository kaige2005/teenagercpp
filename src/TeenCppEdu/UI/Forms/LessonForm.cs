using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using TeenCppEdu.Core.Checkers;
using TeenCppEdu.Core.Models;
using TeenCppEdu.Services.Database;
using TeenCppEdu.Services.Logger;
using TeenCppEdu.Services.ProjectGenerator;
using TeenCppEdu.UI.Controls;

namespace TeenCppEdu.UI.Forms
{
    /// <summary>
    /// 课程学习界面 - 代码编辑与检查
    /// </summary>
    public partial class LessonForm : Form
    {
        private readonly string _lessonId;
        private readonly DatabaseService _db;
        private readonly StudentProgress _progress;

        private Lesson _lesson;
        private LessonCheckRules _checkRules;
        private string _templateCode;

        // UI 控件
        private RichTextBox txtCode;
        private RichTextBox txtFeedback;
        private Button btnCheck;
        private Button btnGenerate;
        private Button btnManualApprove; // 导师放行按钮
        private LessonStepControl stepControl;

        private readonly Color ColorBgDark = Color.FromArgb(45, 52, 70);
        private readonly Color ColorBgEditor = Color.FromArgb(30, 30, 40);
        private readonly Color ColorAccentGreen = Color.FromArgb(100, 200, 120);
        private readonly Color ColorAccentGold = Color.FromArgb(255, 200, 80);
        private readonly Color ColorAccentRed = Color.FromArgb(255, 100, 100);
        private readonly Color ColorTextLight = Color.FromArgb(240, 240, 240);

        public LessonForm(string lessonId, DatabaseService db, StudentProgress progress)
        {
            _lessonId = lessonId;
            _db = db;
            _progress = progress;

            LoadLessonData();
            InitializeComponent();
        }

        private void LoadLessonData()
        {
            try
            {
                // 使用应用程序目录（解决工作目录问题）
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                // 关键修复：处理不同的课程目录结构
                // 优先查找 "courses/Lxx"，如果不存在则查找 "Lxx"（兼容发布包结构）
                string courseDir = Path.Combine(baseDir, "courses", _lessonId.ToLower());
                if (!Directory.Exists(courseDir))
                {
                    courseDir = Path.Combine(baseDir, _lessonId.ToLower());
                }

                // 验证目录存在
                if (!Directory.Exists(courseDir))
                {
                    throw new DirectoryNotFoundException($"课程目录不存在: {courseDir}");
                }

                string lessonFile = Path.Combine(courseDir, "lesson.json");
                string rulesFile = Path.Combine(courseDir, "checks", "rules.json");
                string templateFile = Path.Combine(courseDir, "templates", "main.cpp");

                // 验证文件存在
                if (!File.Exists(lessonFile))
                    throw new FileNotFoundException($"课程配置文件不存在: {lessonFile}");
                if (!File.Exists(rulesFile))
                    throw new FileNotFoundException($"检查规则文件不存在: {rulesFile}");
                if (!File.Exists(templateFile))
                    throw new FileNotFoundException($"代码模板文件不存在: {templateFile}");

                // 读取课程配置
                string lessonJson = File.ReadAllText(lessonFile);
                _lesson = JsonConvert.DeserializeObject<Lesson>(lessonJson);

                // 读取检查规则
                string rulesJson = File.ReadAllText(rulesFile);
                _checkRules = JsonConvert.DeserializeObject<LessonCheckRules>(rulesJson);

                // 读取代码模板
                _templateCode = File.ReadAllText(templateFile);

                LoggerService.Instance?.LogLesson(_lessonId, "课程数据加载成功");
            }
            catch (Exception ex)
            {
                LoggerService.Instance?.Error($"加载课程数据失败: {_lessonId}", ex);
                throw;
            }
        }

        private void InitializeComponent()
        {
            this.Text = $"第{_lesson.Sequence}课：{_lesson.Title}";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = ColorBgDark;

            // 顶部标题栏
            var headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(35, 42, 60)
            };

            var lblLessonTitle = new Label
            {
                Text = $"🎯 第{_lesson.Sequence}课：{_lesson.Title}",
                Font = new Font("Microsoft YaHei", 16, FontStyle.Bold),
                ForeColor = ColorAccentGold,
                AutoSize = true,
                Location = new Point(20, 15)
            };

            var btnClose = new Button
            {
                Text = "✕",
                Size = new Size(40, 40),
                Location = new Point(1140, 10),
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorAccentRed,
                ForeColor = Color.White
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            headerPanel.Controls.Add(lblLessonTitle);
            headerPanel.Controls.Add(btnClose);

            // 左侧步骤说明面板
            var leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 350,
                Padding = new Padding(15),
                BackColor = ColorBgDark
            };

            stepControl = new LessonStepControl(_lesson.Steps);
            stepControl.Dock = DockStyle.Fill;
            leftPanel.Controls.Add(stepControl);

            // 右侧代码编辑面板
            var editorPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = ColorBgDark
            };

            // 代码编辑器
            var lblEditor = new Label
            {
                Text = "📝 C++ 代码编辑器",
                ForeColor = ColorTextLight,
                AutoSize = true,
                Location = new Point(10, 10)
            };

            txtCode = new RichTextBox
            {
                Text = _templateCode,
                Location = new Point(10, 35),
                Size = new Size(800, 450),
                BackColor = ColorBgEditor,
                ForeColor = ColorTextLight,
                Font = new Font("Consolas", 12),
                BorderStyle = BorderStyle.FixedSingle,
                AcceptsTab = true
            };

            // 反馈区域
            var lblFeedback = new Label
            {
                Text = "📋 检查结果",
                ForeColor = ColorTextLight,
                AutoSize = true,
                Location = new Point(10, 495)
            };

            txtFeedback = new RichTextBox
            {
                ReadOnly = true,
                Location = new Point(10, 520),
                Size = new Size(800, 150),
                BackColor = Color.FromArgb(40, 45, 60),
                ForeColor = ColorTextLight,
                Font = new Font("Microsoft YaHei", 10)
            };

            // 按钮面板
            var btnPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                FlowDirection = FlowDirection.RightToLeft,
                BackColor = ColorBgDark,
                Padding = new Padding(10)
            };

            btnCheck = new Button
            {
                Text = "🔍 检查代码",
                Size = new Size(150, 45),
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorAccentGreen,
                ForeColor = ColorBgDark,
                Font = new Font("Microsoft YaHei", 11, FontStyle.Bold),
                Margin = new Padding(10, 0, 0, 0)
            };
            btnCheck.FlatAppearance.BorderSize = 0;
            btnCheck.Click += BtnCheck_Click;

            btnGenerate = new Button
            {
                Text = "📂 生成项目",
                Size = new Size(150, 45),
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorAccentGold,
                ForeColor = ColorBgDark,
                Font = new Font("Microsoft YaHei", 11),
                Margin = new Padding(10, 0, 0, 0)
            };
            btnGenerate.FlatAppearance.BorderSize = 0;
            btnGenerate.Click += BtnGenerate_Click;

            btnManualApprove = new Button
            {
                Text = "🔓 导师放行",
                Size = new Size(150, 45),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(100, 150, 200),
                ForeColor = Color.White,
                Font = new Font("Microsoft YaHei", 11),
                Visible = false // 默认隐藏，检查失败才显示
            };
            btnManualApprove.FlatAppearance.BorderSize = 0;
            btnManualApprove.Click += BtnManualApprove_Click;

            btnPanel.Controls.Add(btnCheck);
            btnPanel.Controls.Add(btnGenerate);
            btnPanel.Controls.Add(btnManualApprove);

            editorPanel.Controls.Add(lblEditor);
            editorPanel.Controls.Add(txtCode);
            editorPanel.Controls.Add(lblFeedback);
            editorPanel.Controls.Add(txtFeedback);

            this.Controls.Add(editorPanel);
            this.Controls.Add(leftPanel);
            this.Controls.Add(headerPanel);
            this.Controls.Add(btnPanel);
        }

        private void BtnCheck_Click(object sender, EventArgs e)
        {
            txtFeedback.Clear();
            btnManualApprove.Visible = false;

            var engine = new CodeCheckEngine();
            var result = engine.CheckCode(txtCode.Text, _checkRules);

            DisplayResult(result);

            // 保存提交记录
            _db.SaveSubmission(_lessonId, txtCode.Text, result);
            _progress.TotalSubmissions++;
            _db.UpdateProgress(_progress);

            if (result.IsPassed)
            {
                OnLessonPassed();
            }
            else
            {
                btnManualApprove.Visible = true;
            }
        }

        private void DisplayResult(CheckResult result)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(result.Summary);
            sb.AppendLine();

            foreach (var item in result.ItemResults)
            {
                string status = item.IsPassed ? "✓" : "✗";
                string color = item.IsPassed ? "绿色" : "红色";
                sb.AppendLine($"{status} {item.RuleName} ({item.Score}/{item.MaxScore}分)");
                sb.AppendLine($"  {item.Feedback}");
                if (!string.IsNullOrEmpty(item.Details))
                {
                    sb.AppendLine($"  详情: {item.Details}");
                }
                sb.AppendLine();
            }

            if (result.IsManuallyApproved)
            {
                sb.AppendLine();
                sb.AppendLine($"📝 已由导师 {result.ApprovedBy} 手动放行");
                if (!string.IsNullOrEmpty(result.ApprovalNote))
                {
                    sb.AppendLine($"备注: {result.ApprovalNote}");
                }
            }

            txtFeedback.Text = sb.ToString();
            txtFeedback.ForeColor = result.IsPassed ? ColorAccentGreen : ColorAccentRed;
        }

        private void BtnGenerate_Click(object sender, EventArgs e)
        {
            var generator = new DevCppProjectGenerator();
            string projectName = $"Lesson{_lesson.Sequence:00}_{_progress.StudentName}";
            string outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output");

            Directory.CreateDirectory(outputPath);

            try
            {
                string projectPath = generator.GenerateProject(projectName, outputPath, txtCode.Text);

                MessageBox.Show(
                    $"项目已生成！\n\n保存位置：\n{projectPath}\n\n请用 Dev-C++ 打开 {projectName}.dev 文件查看和运行！",
                    "生成成功",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // 尝试打开文件夹
                System.Diagnostics.Process.Start("explorer.exe", projectPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"生成失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnManualApprove_Click(object sender, EventArgs e)
        {
            // 弹出导师放行对话框
            var dlg = new ManualApproveDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var engine = new CodeCheckEngine();
                var result = new CheckResult()
                {
                    ItemResults = new System.Collections.Generic.List<CheckItemResult>(),
                    Score = _checkRules.Rules.Sum(r => r.Score)
                };

                engine.ManualApprove(result, dlg.TeacherName, dlg.Note);

                // 保存记录
                _db.SaveSubmission(_lessonId, txtCode.Text, result);

                DisplayResult(result);
                OnLessonPassed();

                btnManualApprove.Visible = false;
            }
        }

        private void OnLessonPassed()
        {
            MessageBox.Show(
                $"🎉 恭喜通关！\n\n" +
                $"获得经验值：+{_lesson.RewardExp}\n" +
                $"当前等级：Lv.{_progress.Level}\n\n" +
                $"继续加油，下一课等你！",
                "通关成功",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);

            // 更新进度
            _progress.Experience += _lesson.RewardExp;
            _progress.CompletedLessons++;
            _progress.UnlockedLessonId = GetNextLessonId();

            // 经验升级计算
            if (_progress.Experience >= _progress.Level * 500)
            {
                _progress.Level++;
            }

            _db.UpdateProgress(_progress);
            _progress.UnlockedLessonId = GetNextLessonId();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private string GetNextLessonId()
        {
            int nextNum = int.Parse(_lessonId.Substring(1)) + 1;
            return $"L{nextNum:00}";
        }
    }
}

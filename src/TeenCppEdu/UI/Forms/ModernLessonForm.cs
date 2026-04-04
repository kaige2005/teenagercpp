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
    /// 现代课程学习界面 - 支持多阶段（知识+实践+挑战）
    /// </summary>
    public partial class ModernLessonForm : Form
    {
        private readonly string _lessonId;
        private readonly DatabaseService _db;
        private readonly StudentProgress _progress;

        private Lesson _lesson;
        private LessonCheckRules _checkRules;
        private string _templateCode;

        // 阶段进度
        private bool _knowledgeCompleted = false;
        private bool _practiceCompleted = false;
        private bool _challengeCompleted = false;
        private int _totalEarnedXp = 0;

        // UI 控件
        private RichTextBox txtCode;
        private RichTextBox txtFeedback;
        private Button btnCheck;
        private Button btnGenerate;
        private Button btnManualApprove;
        private LessonStepControl stepControl;
        private Panel _phasePanel;
        private Label _lblPhaseStatus;
        private Button _btnStartKnowledge;
        private Button _btnStartChallenge;

        private readonly Color ColorBgDark = Color.FromArgb(45, 52, 70);
        private readonly Color ColorBgEditor = Color.FromArgb(30, 30, 40);
        private readonly Color ColorBgCard = Color.FromArgb(55, 65, 90);
        private readonly Color ColorAccentGreen = Color.FromArgb(100, 200, 120);
        private readonly Color ColorAccentGold = Color.FromArgb(255, 200, 80);
        private readonly Color ColorAccentRed = Color.FromArgb(255, 100, 100);
        private readonly Color ColorTextLight = Color.FromArgb(240, 240, 240);

        public ModernLessonForm(string lessonId, DatabaseService db, StudentProgress progress)
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
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string courseDir = Path.Combine(baseDir, "courses", _lessonId.ToLower());
                if (!Directory.Exists(courseDir))
                {
                    courseDir = Path.Combine(baseDir, _lessonId.ToLower());
                }

                if (!Directory.Exists(courseDir))
                {
                    throw new DirectoryNotFoundException($"课程目录不存在: {courseDir}");
                }

                string lessonFile = Path.Combine(courseDir, "lesson.json");

                if (!File.Exists(lessonFile))
                    throw new FileNotFoundException($"课程配置文件不存在: {lessonFile}");

                // 读取课程配置
                string lessonJson = File.ReadAllText(lessonFile);
                _lesson = JsonConvert.DeserializeObject<Lesson>(lessonJson);

                // 新格式使用Effective路径，修正路径分隔符
                string rulesFile;
                string templateFile;

                if (_lesson.IsNewFormat)
                {
                    // 新格式使用phases中的路径
                    string effectiveRulesPath = _lesson.EffectiveCheckRulesPath?.Replace("/", "\\");
                    string effectiveTemplatePath = _lesson.EffectiveTemplatePath?.Replace("/", "\\");

                    // 移除courses/lxx/前缀如果存在
                    string prefix = $"courses/{_lessonId.ToLower()}/";
                    if (effectiveRulesPath?.StartsWith(prefix) == true)
                        effectiveRulesPath = effectiveRulesPath.Substring(prefix.Length);
                    if (effectiveTemplatePath?.StartsWith(prefix) == true)
                        effectiveTemplatePath = effectiveTemplatePath.Substring(prefix.Length);

                    rulesFile = Path.Combine(courseDir, effectiveRulesPath ?? "checks\\rules.json");
                    templateFile = Path.Combine(courseDir, effectiveTemplatePath ?? "templates\\main.cpp");
                }
                else
                {
                    // 旧格式使用固定路径
                    rulesFile = Path.Combine(courseDir, "checks", "rules.json");
                    templateFile = Path.Combine(courseDir, "templates", "main.cpp");
                }

                LoggerService.Instance?.LogLesson(_lessonId, $"规则文件路径: {rulesFile}");
                LoggerService.Instance?.LogLesson(_lessonId, $"模板文件路径: {templateFile}");

                // 如果是新格式且没有完成知识阶段，不立即加载模板
                if (_lesson.IsNewFormat && !IsPracticePhaseAvailable())
                {
                    _templateCode = null;
                    if (File.Exists(rulesFile))
                    {
                        string rulesJson = File.ReadAllText(rulesFile);
                        _checkRules = JsonConvert.DeserializeObject<LessonCheckRules>(rulesJson);
                    }
                    LoggerService.Instance?.LogLesson(_lessonId, "新课程数据加载成功（多阶段）");
                    return;
                }

                if (!File.Exists(rulesFile))
                    throw new FileNotFoundException($"检查规则文件不存在: {rulesFile}");
                if (!File.Exists(templateFile))
                    throw new FileNotFoundException($"代码模板文件不存在: {templateFile}");

                string rulesJsonText = File.ReadAllText(rulesFile);
                _checkRules = JsonConvert.DeserializeObject<LessonCheckRules>(rulesJsonText);

                // 验证规则加载成功
                if (_checkRules?.Rules == null)
                {
                    throw new InvalidOperationException($"检查规则文件格式错误: {rulesFile}");
                }

                _templateCode = File.ReadAllText(templateFile);

                LoggerService.Instance?.LogLesson(_lessonId, $"课程数据加载成功，规则数: {_checkRules.Rules.Count}");
            }
            catch (Exception ex)
            {
                LoggerService.Instance?.Error($"加载课程数据失败: {_lessonId}", ex);
                throw;
            }
        }

        private bool IsPracticePhaseAvailable()
        {
            // 从数据库读取进度
            var savedProgress = _db.GetLessonPhaseProgress(_lessonId);
            if (savedProgress != null)
            {
                _knowledgeCompleted = savedProgress.KnowledgeCompleted;
                _challengeCompleted = savedProgress.ChallengeCompleted;
                _totalEarnedXp = savedProgress.EarnedXp;
                return _knowledgeCompleted;
            }
            return !_lesson.IsNewFormat; // 旧格式直接进入实践
        }

        private void InitializeComponent()
        {
            this.Text = $"第{_lesson.Sequence}课：{_lesson.Title}";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = ColorBgDark;

            // 顶部标题栏
            var headerPanel = CreateHeaderPanel();

            // 左侧步骤面板
            var leftPanel = CreateLeftPanel();

            // 右侧主内容区
            var mainPanel = CreateMainPanel();

            this.Controls.Add(mainPanel);
            this.Controls.Add(leftPanel);
            this.Controls.Add(headerPanel);

            // 如果是新格式且未完成知识阶段，显示阶段选择
            if (_lesson.IsNewFormat && !_knowledgeCompleted)
            {
                ShowPhaseSelection();
            }
            else if (_lesson.IsNewFormat && _knowledgeCompleted && !_challengeCompleted)
            {
                // 已完成知识但可能有挑战
                if (_lesson.ChallengePhase != null)
                {
                    ShowChallengeOption();
                }
            }
            // 旧格式课程直接创建实践UI
            else if (!_lesson.IsNewFormat && _templateCode != null)
            {
                LoggerService.Instance?.LogLesson(_lessonId, "旧格式课程，直接初始化实践UI");
                CreatePracticeUI(mainPanel);
            }
        }

        private Panel CreateHeaderPanel()
        {
            var header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(35, 42, 60)
            };

            var lblTitle = new Label
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

            header.Controls.Add(lblTitle);
            header.Controls.Add(btnClose);
            return header;
        }

        private Panel CreateLeftPanel()
        {
            var leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 350,
                Padding = new Padding(15),
                BackColor = ColorBgDark
            };

            // 阶段状态显示（新格式）
            if (_lesson.IsNewFormat)
            {
                var phaseStatusPanel = new Panel
                {
                    Size = new Size(320, 120),
                    Location = new Point(15, 15),
                    BackColor = ColorBgCard,
                    Padding = new Padding(10)
                };

                var lblPhaseTitle = new Label
                {
                    Text = "📊 学习进度",
                    Font = new Font("Microsoft YaHei", 12, FontStyle.Bold),
                    ForeColor = ColorAccentGold,
                    AutoSize = true,
                    Location = new Point(10, 10)
                };
                phaseStatusPanel.Controls.Add(lblPhaseTitle);

                _lblPhaseStatus = new Label
                {
                    Text = GetPhaseStatusText(),
                    Font = new Font("Microsoft YaHei", 10),
                    ForeColor = ColorTextLight,
                    AutoSize = true,
                    Location = new Point(10, 40),
                    MaximumSize = new Size(300, 0)
                };
                phaseStatusPanel.Controls.Add(_lblPhaseStatus);

                leftPanel.Controls.Add(phaseStatusPanel);
            }

            // 步骤控件
            if (_lesson.EffectiveSteps != null && _lesson.EffectiveSteps.Count > 0)
            {
                stepControl = new LessonStepControl(_lesson.EffectiveSteps);
                stepControl.Dock = _lesson.IsNewFormat ? DockStyle.None : DockStyle.Fill;
                if (_lesson.IsNewFormat)
                {
                    stepControl.Location = new Point(15, 150);
                    stepControl.Size = new Size(320, 400);
                }
                leftPanel.Controls.Add(stepControl);
            }

            return leftPanel;
        }

        private Panel CreateMainPanel()
        {
            return new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = ColorBgDark,
                Name = "mainPanel"
            };
        }

        private void ShowPhaseSelection()
        {
            var mainPanel = this.Controls.Find("mainPanel", true).FirstOrDefault() as Panel;
            if (mainPanel == null) return;

            mainPanel.Controls.Clear();

            _phasePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = ColorBgDark
            };

            int y = 50;

            // 欢迎标题
            var lblWelcome = new Label
            {
                Text = $"{_lesson.Title}",
                Font = new Font("Microsoft YaHei", 20, FontStyle.Bold),
                ForeColor = ColorAccentGold,
                AutoSize = true,
                Location = new Point(100, y)
            };
            _phasePanel.Controls.Add(lblWelcome);
            y += 60;

            // 副标题
            if (!string.IsNullOrEmpty(_lesson.Subtitle))
            {
                var lblSubtitle = new Label
                {
                    Text = _lesson.Subtitle,
                    Font = new Font("Microsoft YaHei", 14),
                    ForeColor = ColorTextLight,
                    AutoSize = true,
                    Location = new Point(100, y)
                };
                _phasePanel.Controls.Add(lblSubtitle);
                y += 60;
            }

            // 知识点预览
            var lblKnowledgePoints = new Label
            {
                Text = "📚 本课知识点：",
                Font = new Font("Microsoft YaHei", 12, FontStyle.Bold),
                ForeColor = ColorAccentGreen,
                AutoSize = true,
                Location = new Point(100, y)
            };
            _phasePanel.Controls.Add(lblKnowledgePoints);
            y += 40;

            foreach (var kp in _lesson.KnowledgePoints)
            {
                var lblPoint = new Label
                {
                    Text = "  • " + kp,
                    Font = new Font("Microsoft YaHei", 11),
                    ForeColor = ColorTextLight,
                    AutoSize = true,
                    Location = new Point(120, y)
                };
                _phasePanel.Controls.Add(lblPoint);
                y += 30;
            }
            y += 40;

            // 第一阶段按钮
            _btnStartKnowledge = new Button
            {
                Text = $"📖 开始学习：{_lesson.KnowledgePhase?.Title}",
                Size = new Size(350, 60),
                Location = new Point(100, y),
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorAccentGold,
                ForeColor = ColorBgDark,
                Font = new Font("Microsoft YaHei", 14, FontStyle.Bold)
            };
            _btnStartKnowledge.FlatAppearance.BorderSize = 0;
            _btnStartKnowledge.Click += OnStartKnowledge;
            _phasePanel.Controls.Add(_btnStartKnowledge);

            mainPanel.Controls.Add(_phasePanel);
        }

        private void ShowChallengeOption()
        {
            var mainPanel = this.Controls.Find("mainPanel", true).FirstOrDefault() as Panel;
            if (mainPanel == null) return;

            // 实践阶段完成后显示挑战选项
            _btnStartChallenge = new Button
            {
                Text = "🐛 挑战：Bug猎手",
                Size = new Size(250, 50),
                Location = new Point(20, 560),
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorAccentRed,
                ForeColor = Color.White,
                Font = new Font("Microsoft YaHei", 12, FontStyle.Bold)
            };
            _btnStartChallenge.FlatAppearance.BorderSize = 0;
            _btnStartChallenge.Click += OnStartChallenge;
            this.Controls.Add(_btnStartChallenge);
        }

        private void OnStartKnowledge(object sender, EventArgs e)
        {
            var knowledgeForm = new KnowledgePhaseForm(_lesson.KnowledgePhase, _lessonId);
            if (knowledgeForm.ShowDialog() == DialogResult.OK)
            {
                _knowledgeCompleted = true;
                _totalEarnedXp += knowledgeForm.TotalEarnedXp;

                // 保存进度
                SavePhaseProgress();

                // 显示实践阶段
                MessageBox.Show($"🎉 知识阶段完成！获得 {knowledgeForm.TotalEarnedXp} XP\n\n接下来开始实践编程！",
                    "阶段完成", MessageBoxButtons.OK, MessageBoxIcon.Information);

                InitializePracticePhase();
            }
        }

        private void OnStartChallenge(object sender, EventArgs e)
        {
            var challengeForm = new BugHuntForm(_lesson.ChallengePhase, _lessonId);
            if (challengeForm.ShowDialog() == DialogResult.OK)
            {
                _challengeCompleted = true;
                _totalEarnedXp += challengeForm.EarnedXp;

                SavePhaseProgress();

                MessageBox.Show($"🎉 挑战完成！获得 {challengeForm.EarnedXp} XP",
                    "挑战完成", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 课程全部完成
                OnLessonFullyCompleted();
            }
        }

        private void InitializePracticePhase()
        {
            // 加载实践阶段的数据
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string courseDir = Path.Combine(baseDir, "courses", _lessonId.ToLower());
                if (!Directory.Exists(courseDir))
                {
                    courseDir = Path.Combine(baseDir, _lessonId.ToLower());
                }

                string rulesFile = Path.Combine(courseDir, _lesson.EffectiveCheckRulesPath.Replace($"courses/{_lessonId.ToLower()}/", "").Replace("/", "\\"));
                string templateFile = Path.Combine(courseDir, _lesson.EffectiveTemplatePath.Replace($"courses/{_lessonId.ToLower()}/", "").Replace("/", "\\"));

                if (File.Exists(rulesFile))
                {
                    string rulesJson = File.ReadAllText(rulesFile);
                    _checkRules = JsonConvert.DeserializeObject<LessonCheckRules>(rulesJson);
                }

                if (File.Exists(templateFile))
                {
                    _templateCode = File.ReadAllText(templateFile);
                }

                // 创建实践界面
                var mainPanel = this.Controls.Find("mainPanel", true).FirstOrDefault() as Panel;
                mainPanel.Controls.Clear();

                CreatePracticeUI(mainPanel);

                // 更新状态
                _lblPhaseStatus.Text = GetPhaseStatusText();
            }
            catch (Exception ex)
            {
                LoggerService.Instance?.Error($"加载实践阶段数据失败: {_lessonId}", ex);
                MessageBox.Show($"加载实践阶段失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreatePracticeUI(Panel container)
        {
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
                Text = _templateCode ?? "// 模板加载失败，请手动编写代码",
                Location = new Point(10, 35),
                Size = new Size(800, 450),
                BackColor = ColorBgEditor,
                ForeColor = ColorTextLight,
                Font = new Font("Consolas", 12),
                BorderStyle = BorderStyle.FixedSingle,
                AcceptsTab = true
            };

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

            // 按钮面板 - 使用Panel而非FlowLayoutPanel，避免布局冲突
            var btnPanel = new Panel
            {
                Location = new Point(10, 680),
                Size = new Size(800, 60),
                BackColor = ColorBgDark
            };

            btnCheck = new Button
            {
                Text = "🔍 检查代码",
                Size = new Size(150, 45),
                Location = new Point(490, 7),
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorAccentGreen,
                ForeColor = ColorBgDark,
                Font = new Font("Microsoft YaHei", 11, FontStyle.Bold)
            };
            btnCheck.FlatAppearance.BorderSize = 0;
            btnCheck.Click += BtnCheck_Click;

            btnGenerate = new Button
            {
                Text = "📂 生成项目",
                Size = new Size(150, 45),
                Location = new Point(330, 7),
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorAccentGold,
                ForeColor = ColorBgDark,
                Font = new Font("Microsoft YaHei", 11)
            };
            btnGenerate.FlatAppearance.BorderSize = 0;
            btnGenerate.Click += BtnGenerate_Click;

            btnManualApprove = new Button
            {
                Text = "🔓 导师放行",
                Size = new Size(150, 45),
                Location = new Point(170, 7),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(100, 150, 200),
                ForeColor = Color.White,
                Font = new Font("Microsoft YaHei", 11),
                Visible = false
            };
            btnManualApprove.FlatAppearance.BorderSize = 0;
            btnManualApprove.Click += BtnManualApprove_Click;

            btnPanel.Controls.Add(btnCheck);
            btnPanel.Controls.Add(btnGenerate);
            btnPanel.Controls.Add(btnManualApprove);

            // 调整控件高度以适应窗口
            txtCode.Height = 400;
            txtFeedback.Location = new Point(10, 445);
            txtFeedback.Height = 120;
            lblFeedback.Location = new Point(10, 420);

            container.Controls.Add(lblEditor);
            container.Controls.Add(txtCode);
            container.Controls.Add(lblFeedback);
            container.Controls.Add(txtFeedback);
            container.Controls.Add(btnPanel);
        }

        private string GetPhaseStatusText()
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"知识学习: {(_knowledgeCompleted ? "✓ 完成" : "⏳ 待完成")}");
            sb.AppendLine($"实践编程: {((_practiceCompleted || (!_lesson.IsNewFormat)) ? "✓ 完成" : "⏳ 进行中")}");
            if (_lesson.ChallengePhase != null)
            {
                sb.AppendLine($"挑战模式: {(_challengeCompleted ? "✓ 完成" : "⏳ 可选")}");
            }
            sb.AppendLine($"总XP: {_totalEarnedXp}");
            return sb.ToString();
        }

        private void SavePhaseProgress()
        {
            _db.SaveLessonPhaseProgress(new LessonPhaseProgress
            {
                LessonId = _lessonId,
                KnowledgeCompleted = _knowledgeCompleted,
                ChallengeCompleted = _challengeCompleted,
                EarnedXp = _totalEarnedXp,
                LastUpdated = DateTime.Now
            });
        }

        private void BtnCheck_Click(object sender, EventArgs e)
        {
            LoggerService.Instance?.LogLesson(_lessonId, "BtnCheck_Click: 开始检查代码");

            try
            {
                // 防御性检查：确保控件已初始化
                if (txtFeedback == null || txtCode == null)
                {
                    MessageBox.Show("UI控件未初始化，请重新打开课程。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LoggerService.Instance?.Error($"BtnCheck_Click: UI控件为空 lessonId={_lessonId}, txtFeedback={txtFeedback==null}, txtCode={txtCode==null}");
                    return;
                }

                // 防御性检查：确保检查规则已加载
                if (_checkRules == null)
                {
                    MessageBox.Show("检查规则未加载，请检查课程数据完整性。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LoggerService.Instance?.Error($"BtnCheck_Click: _checkRules为空 lessonId={_lessonId}");
                    return;
                }

                if (_checkRules.Rules == null)
                {
                    MessageBox.Show("检查规则列表为空，请检查课程数据。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LoggerService.Instance?.Error($"BtnCheck_Click: _checkRules.Rules为空 lessonId={_lessonId}");
                    return;
                }

                LoggerService.Instance?.LogLesson(_lessonId, $"BtnCheck_Click: 规则数量={_checkRules.Rules.Count}, 代码长度={txtCode.Text?.Length ?? 0}");

                txtFeedback.Clear();
                if (btnManualApprove != null)
                    btnManualApprove.Visible = false;

                var engine = new CodeCheckEngine();
                var result = engine.CheckCode(txtCode.Text, _checkRules);

                LoggerService.Instance?.LogLesson(_lessonId, $"BtnCheck_Click: 检查结果 IsPassed={result?.IsPassed}, Score={result?.Score}");

                DisplayResult(result);

                _db.SaveSubmission(_lessonId, txtCode.Text, result);
                _progress.TotalSubmissions++;
                _db.UpdateProgress(_progress);

                if (result.IsPassed)
                {
                    OnPracticePassed();
                }
                else
                {
                    if (btnManualApprove != null)
                        btnManualApprove.Visible = true;
                }
            }
            catch (Exception ex)
            {
                var errorMsg = $"检查代码时发生错误: {ex.Message}\n类型: {ex.GetType().Name}\n堆栈: {ex.StackTrace}";
                LoggerService.Instance?.Error($"BtnCheck_Click: 异常 lessonId={_lessonId}", ex);
                MessageBox.Show(errorMsg, "检查失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayResult(CheckResult result)
        {
            // 防御性检查
            if (txtFeedback == null || result == null)
            {
                LoggerService.Instance?.Error($"DisplayResult: 参数为空 txtFeedback={txtFeedback==null}, result={result==null}");
                return;
            }

            StringBuilder sb = new StringBuilder();

            sb.AppendLine(result.Summary ?? "检查完成");
            sb.AppendLine();

            foreach (var item in result.ItemResults)
            {
                string status = item.IsPassed ? "✓" : "✗";
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
            // 防御性检查
            if (txtCode == null)
            {
                MessageBox.Show("代码编辑器未初始化。", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var generator = new DevCppProjectGenerator();
            string projectName = $"Lesson{_lesson?.Sequence:00}_{_progress?.StudentName}";
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

                System.Diagnostics.Process.Start("explorer.exe", projectPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"生成失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnManualApprove_Click(object sender, EventArgs e)
        {
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

                _db.SaveSubmission(_lessonId, txtCode.Text, result);
                DisplayResult(result);
                OnPracticePassed();

                btnManualApprove.Visible = false;
            }
        }

        private void OnPracticePassed()
        {
            _practiceCompleted = true;

            int practiceXp = _lesson.IsNewFormat ? _lesson.PracticePhase?.RewardExp ?? 0 : _lesson.RewardExp;
            _totalEarnedXp += practiceXp;

            SavePhaseProgress();

            // 旧格式课程可能没有阶段状态面板
            if (_lblPhaseStatus != null)
                _lblPhaseStatus.Text = GetPhaseStatusText();

            // 如果有挑战阶段，显示选项
            if (_lesson.ChallengePhase != null && !_challengeCompleted)
            {
                if (MessageBox.Show(
                    $"🎉 实践阶段完成！获得 {practiceXp} XP\n\n" +
                    $"你可以选择挑战Bug猎手模式，额外获得 {_lesson.ChallengePhase.RewardExp} XP！\n" +
                    $"是否挑战？",
                    "实践完成",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    OnStartChallenge(null, null);
                    return;
                }
            }

            // 课程完成
            OnLessonFullyCompleted();
        }

        private void OnLessonFullyCompleted()
        {
            string badge = _lesson.IsNewFormat ? _lesson.PracticePhase.RewardBadge : _lesson.RewardBadge;
            string challengeBadge = _challengeCompleted ? _lesson.ChallengePhase?.RewardBadge : null;

            MessageBox.Show(
                $"🎉 恭喜通关！\n\n" +
                $"获得总经验值：+{_totalEarnedXp}\n" +
                $"当前等级：Lv.{_progress.Level}\n\n" +
                (badge != null ? $"🏆 获得徽章：{badge}\n" : "") +
                (challengeBadge != null ? $"🏆 挑战徽章：{challengeBadge}\n" : "") +
                $"\n继续加油，下一课等你！",
                "通关成功",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);

            // 更新进度
            _progress.Experience += _totalEarnedXp;
            _progress.CompletedLessons++;
            _progress.UnlockedLessonId = GetNextLessonId();

            if (_progress.Experience >= _progress.Level * 500)
            {
                _progress.Level++;
            }

            _db.UpdateProgress(_progress);

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

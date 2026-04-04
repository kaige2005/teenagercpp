using System;
using System.Collections.Generic;
using System.Data.SQLite;
using TeenCppEdu.Core.Models;

namespace TeenCppEdu.Services.Database
{
    /// <summary>
    /// SQLite数据库服务
    /// </summary>
    public class DatabaseService : IDisposable
    {
        private SQLiteConnection _connection;
        private readonly string _dbPath;

        public DatabaseService(string dbPath = "teen_cpp_edu.db")
        {
            _dbPath = dbPath;
            InitializeDatabase();
        }

        private void InitializeDatabase()
        {
            bool createNew = !System.IO.File.Exists(_dbPath);
            _connection = new SQLiteConnection($"Data Source={_dbPath};Version=3;");
            _connection.Open();

            if (createNew)
            {
                CreateTables();
            }
        }

        private void CreateTables()
        {
            // 学生进度表
            string createStudentProgress = @"
                CREATE TABLE IF NOT EXISTS StudentProgress (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    StudentName TEXT NOT NULL,
                    Level INTEGER DEFAULT 1,
                    Experience INTEGER DEFAULT 0,
                    UnlockedLessonId TEXT,
                    CompletedLessons INTEGER DEFAULT 0,
                    TotalSubmissions INTEGER DEFAULT 0,
                    EarnedBadges TEXT DEFAULT '',
                    FirstStudyTime TEXT DEFAULT CURRENT_TIMESTAMP,
                    LastStudyTime TEXT DEFAULT CURRENT_TIMESTAMP,
                    TotalStudyMinutes INTEGER DEFAULT 0
                );";

            // 学习记录表
            string createStudyRecord = @"
                CREATE TABLE IF NOT EXISTS StudyRecord (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    LessonId TEXT NOT NULL,
                    StartTime TEXT DEFAULT CURRENT_TIMESTAMP,
                    EndTime TEXT,
                    DurationMinutes INTEGER DEFAULT 0,
                    IsCompleted INTEGER DEFAULT 0,
                    AttemptCount INTEGER DEFAULT 0,
                    ExpGained INTEGER DEFAULT 0
                );";

            // 代码提交记录表
            string createSubmission = @"
                CREATE TABLE IF NOT EXISTS Submission (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    LessonId TEXT NOT NULL,
                    SubmitTime TEXT DEFAULT CURRENT_TIMESTAMP,
                    SourceCode TEXT,
                    CheckScore INTEGER DEFAULT 0,
                    IsPassed INTEGER DEFAULT 0,
                    IsManuallyApproved INTEGER DEFAULT 0,
                    ApprovedBy TEXT,
                    ApprovalNote TEXT,
                    Feedback TEXT
                );";

            // 课程阶段进度表（v1.2+支持）
            string createPhaseProgress = @"
                CREATE TABLE IF NOT EXISTS LessonPhaseProgress (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    LessonId TEXT NOT NULL UNIQUE,
                    KnowledgeCompleted INTEGER DEFAULT 0,
                    ChallengeCompleted INTEGER DEFAULT 0,
                    EarnedXp INTEGER DEFAULT 0,
                    LastUpdated TEXT DEFAULT CURRENT_TIMESTAMP
                );";

            ExecuteNonQuery(createStudentProgress);
            ExecuteNonQuery(createStudyRecord);
            ExecuteNonQuery(createSubmission);
            ExecuteNonQuery(createPhaseProgress);
        }

        private void ExecuteNonQuery(string sql)
        {
            using (var cmd = new SQLiteCommand(sql, _connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        // ========== 学生进度操作 ==========

        public StudentProgress GetOrCreateProgress(string studentName)
        {
            string sql = "SELECT * FROM StudentProgress WHERE StudentName = @name";
            using (var cmd = new SQLiteCommand(sql, _connection))
            {
                cmd.Parameters.AddWithValue("@name", studentName);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ReadStudentProgress(reader);
                    }
                }
            }

            // 不存在则创建
            string insertSql = @"INSERT INTO StudentProgress (StudentName, UnlockedLessonId)
                                  VALUES (@name, 'L01');";
            using (var cmd = new SQLiteCommand(insertSql, _connection))
            {
                cmd.Parameters.AddWithValue("@name", studentName);
                cmd.ExecuteNonQuery();
            }

            return GetOrCreateProgress(studentName);
        }

        public void UpdateProgress(StudentProgress progress)
        {
            string sql = @"UPDATE StudentProgress SET
                Level = @level,
                Experience = @exp,
                UnlockedLessonId = @unlocked,
                CompletedLessons = @completed,
                TotalSubmissions = @submissions,
                EarnedBadges = @badges,
                LastStudyTime = @lastTime,
                TotalStudyMinutes = @minutes
                WHERE Id = @id";

            using (var cmd = new SQLiteCommand(sql, _connection))
            {
                cmd.Parameters.AddWithValue("@level", progress.Level);
                cmd.Parameters.AddWithValue("@exp", progress.Experience);
                cmd.Parameters.AddWithValue("@unlocked", progress.UnlockedLessonId);
                cmd.Parameters.AddWithValue("@completed", progress.CompletedLessons);
                cmd.Parameters.AddWithValue("@submissions", progress.TotalSubmissions);
                cmd.Parameters.AddWithValue("@badges", progress.EarnedBadges);
                cmd.Parameters.AddWithValue("@lastTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@minutes", progress.TotalStudyMinutes);
                cmd.Parameters.AddWithValue("@id", progress.Id);
                cmd.ExecuteNonQuery();
            }
        }

        // ========== 提交记录操作 ==========

        public void SaveSubmission(string lessonId, string sourceCode, CheckResult result)
        {
            string sql = @"INSERT INTO Submission
                (LessonId, SourceCode, CheckScore, IsPassed, IsManuallyApproved, ApprovedBy, ApprovalNote, Feedback)
                VALUES (@lessonId, @code, @score, @passed, @manual, @by, @note, @feedback)";

            using (var cmd = new SQLiteCommand(sql, _connection))
            {
                cmd.Parameters.AddWithValue("@lessonId", lessonId);
                cmd.Parameters.AddWithValue("@code", sourceCode);
                cmd.Parameters.AddWithValue("@score", result.Score);
                cmd.Parameters.AddWithValue("@passed", result.IsPassed ? 1 : 0);
                cmd.Parameters.AddWithValue("@manual", result.IsManuallyApproved ? 1 : 0);
                cmd.Parameters.AddWithValue("@by", (object)result.ApprovedBy ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@note", (object)result.ApprovalNote ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@feedback", result.Summary);
                cmd.ExecuteNonQuery();
            }

            // 更新总提交次数
            UpdateTotalSubmissions();
        }

        private void UpdateTotalSubmissions()
        {
            string sql = "UPDATE StudentProgress SET TotalSubmissions = TotalSubmissions + 1";
            using (var cmd = new SQLiteCommand(sql, _connection))
            {
                cmd.ExecuteNonQuery();
            }
        }

        public List<SubmissionRecord> GetSubmissions(string lessonId = null)
        {
            var list = new List<SubmissionRecord>();
            string sql = lessonId == null
                ? "SELECT * FROM Submission ORDER BY SubmitTime DESC"
                : "SELECT * FROM Submission WHERE LessonId = @lessonId ORDER BY SubmitTime DESC";

            using (var cmd = new SQLiteCommand(sql, _connection))
            {
                if (lessonId != null)
                    cmd.Parameters.AddWithValue("@lessonId", lessonId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new SubmissionRecord
                        {
                            Id = reader.GetInt32(0),
                            LessonId = reader.GetString(1),
                            SubmitTime = DateTime.Parse(reader.GetString(2)),
                            SourceCode = reader.GetString(3),
                            CheckScore = reader.GetInt32(4),
                            IsPassed = reader.GetInt32(5) == 1,
                            IsManuallyApproved = reader.GetInt32(6) == 1,
                            ApprovedBy = reader.IsDBNull(7) ? null : (string?)reader.GetString(7),
                            ApprovalNote = reader.IsDBNull(8) ? null : (string?)reader.GetString(8),
                            Feedback = reader.IsDBNull(9) ? null : (string?)reader.GetString(9)
                        });
                    }
                }
            }
            return list;
        }

        // ========== 工具方法 ==========

        private StudentProgress ReadStudentProgress(SQLiteDataReader reader)
        {
            return new StudentProgress
            {
                Id = reader.GetInt32(0),
                StudentName = reader.GetString(1),
                Level = reader.GetInt32(2),
                Experience = reader.GetInt32(3),
                UnlockedLessonId = reader.IsDBNull(4) ? "L01" : reader.GetString(4),
                CompletedLessons = reader.GetInt32(5),
                TotalSubmissions = reader.GetInt32(6),
                EarnedBadges = reader.GetString(7),
                FirstStudyTime = DateTime.Parse(reader.GetString(8)),
                LastStudyTime = DateTime.Parse(reader.GetString(9)),
                TotalStudyMinutes = reader.GetInt32(10)
            };
        }

        // ========== 课程阶段进度操作 ==========

        public LessonPhaseProgress GetLessonPhaseProgress(string lessonId)
        {
            string sql = "SELECT * FROM LessonPhaseProgress WHERE LessonId = @lessonId";
            using (var cmd = new SQLiteCommand(sql, _connection))
            {
                cmd.Parameters.AddWithValue("@lessonId", lessonId);
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new LessonPhaseProgress
                        {
                            Id = reader.GetInt32(0),
                            LessonId = reader.GetString(1),
                            KnowledgeCompleted = reader.GetInt32(2) == 1,
                            ChallengeCompleted = reader.GetInt32(3) == 1,
                            EarnedXp = reader.GetInt32(4),
                            LastUpdated = DateTime.Parse(reader.GetString(5))
                        };
                    }
                }
            }
            return null;
        }

        public void SaveLessonPhaseProgress(LessonPhaseProgress progress)
        {
            // 检查是否存在
            var existing = GetLessonPhaseProgress(progress.LessonId);

            if (existing == null)
            {
                // 插入新记录
                string insertSql = @"
                    INSERT INTO LessonPhaseProgress (LessonId, KnowledgeCompleted, ChallengeCompleted, EarnedXp, LastUpdated)
                    VALUES (@lessonId, @knowledge, @challenge, @xp, @updated)";
                using (var cmd = new SQLiteCommand(insertSql, _connection))
                {
                    cmd.Parameters.AddWithValue("@lessonId", progress.LessonId);
                    cmd.Parameters.AddWithValue("@knowledge", progress.KnowledgeCompleted ? 1 : 0);
                    cmd.Parameters.AddWithValue("@challenge", progress.ChallengeCompleted ? 1 : 0);
                    cmd.Parameters.AddWithValue("@xp", progress.EarnedXp);
                    cmd.Parameters.AddWithValue("@updated", progress.LastUpdated.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                // 更新现有记录
                string updateSql = @"
                    UPDATE LessonPhaseProgress SET
                        KnowledgeCompleted = @knowledge,
                        ChallengeCompleted = @challenge,
                        EarnedXp = @xp,
                        LastUpdated = @updated
                    WHERE LessonId = @lessonId";
                using (var cmd = new SQLiteCommand(updateSql, _connection))
                {
                    cmd.Parameters.AddWithValue("@knowledge", progress.KnowledgeCompleted ? 1 : 0);
                    cmd.Parameters.AddWithValue("@challenge", progress.ChallengeCompleted ? 1 : 0);
                    cmd.Parameters.AddWithValue("@xp", progress.EarnedXp);
                    cmd.Parameters.AddWithValue("@updated", progress.LastUpdated.ToString("yyyy-MM-dd HH:mm:ss"));
                    cmd.Parameters.AddWithValue("@lessonId", progress.LessonId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }
    }

    public class SubmissionRecord
    {
        public int Id { get; set; }
        public string LessonId { get; set; }
        public DateTime SubmitTime { get; set; }
        public string SourceCode { get; set; }
        public int CheckScore { get; set; }
        public bool IsPassed { get; set; }
        public bool IsManuallyApproved { get; set; }
        public string ApprovedBy { get; set; }
        public string ApprovalNote { get; set; }
        public string Feedback { get; set; }
    }
}

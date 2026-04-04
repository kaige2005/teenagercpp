using System;

namespace TeenCppEdu.Core.Models
{
    /// <summary>
    /// 学生学习进度
    /// </summary>
    public class StudentProgress
    {
        public int Id { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        public string StudentName { get; set; }

        /// <summary>
        /// 当前等级
        /// </summary>
        public int Level { get; set; } = 1;

        /// <summary>
        /// 当前经验值
        /// </summary>
        public int Experience { get; set; } = 0;

        /// <summary>
        /// 解锁的最高关卡ID
        /// </summary>
        public string UnlockedLessonId { get; set; }

        /// <summary>
        /// 已通关的关卡数量
        /// </summary>
        public int CompletedLessons { get; set; } = 0;

        /// <summary>
        /// 总代码提交次数
        /// </summary>
        public int TotalSubmissions { get; set; } = 0;

        /// <summary>
        /// 获得徽章列表（逗号分隔）
        /// </summary>
        public string EarnedBadges { get; set; } = "";

        /// <summary>
        /// 首次学习时间
        /// </summary>
        public DateTime FirstStudyTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 最后学习时间
        /// </summary>
        public DateTime LastStudyTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 累计学习分钟数
        /// </summary>
        public int TotalStudyMinutes { get; set; } = 0;
    }

    /// <summary>
    /// 课程阶段进度（用于新格式课程）
    /// </summary>
    public class LessonPhaseProgress
    {
        public int Id { get; set; }
        public string LessonId { get; set; }
        public bool KnowledgeCompleted { get; set; }
        public bool ChallengeCompleted { get; set; }
        public int EarnedXp { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    /// <summary>
    /// 单次学习记录
    /// </summary>
    public class StudyRecord
    {
        public int Id { get; set; }
        public string LessonId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsCompleted { get; set; }
        public int AttemptCount { get; set; }
        public int ExpGained { get; set; }
    }
}

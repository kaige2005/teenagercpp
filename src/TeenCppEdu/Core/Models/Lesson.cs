using System;
using System.Collections.Generic;

namespace TeenCppEdu.Core.Models
{
    /// <summary>
    /// 课程关卡模型
    /// </summary>
    public class Lesson
    {
        /// <summary>
        /// 关卡唯一ID</summary>
        public string Id { get; set; }

        /// <summary>
        /// 关卡序号（第几课）
        /// </summary>
        public int Sequence { get; set; }

        /// <summary>
        /// 关卡标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 前置关卡ID（null表示第一关）
        /// </summary>
        public string PrerequisiteId { get; set; }

        /// <summary>
        /// 前置解锁条件：经验值要求
        /// </summary>
        public int RequiredExp { get; set; }

        /// <summary>
        /// 关卡知识点列表
        /// </summary>
        public List<string> KnowledgePoints { get; set; } = new List<string>();

        /// <summary>
        /// 步骤说明
        /// </summary>
        public List<LessonStep> Steps { get; set; } = new List<LessonStep>();

        /// <summary>
        /// 代码模板文件路径
        /// </summary>
        public string TemplatePath { get; set; }

        /// <summary>
        /// 检查规则配置路径
        /// </summary>
        public string CheckRulesPath { get; set; }

        /// <summary>
        /// 奖励经验值
        /// </summary>
        public int RewardExp { get; set; } = 100;

        /// <summary>
        /// 奖励徽章ID
        /// </summary>
        public string RewardBadge { get; set; }

        /// <summary>
        /// 关卡图标（游戏化元素）
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// 关卡位置坐标（在地图上的位置）
        /// </summary>
        public int MapX { get; set; }

        public int MapY { get; set; }
    }

    /// <summary>
    /// 课程步骤
    /// </summary>
    public class LessonStep
    {
        public int Order { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Hint { get; set; }
    }
}

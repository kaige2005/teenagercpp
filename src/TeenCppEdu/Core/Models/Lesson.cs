using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace TeenCppEdu.Core.Models
{
    /// <summary>
    /// 课程关卡模型 - 支持旧格式(Steps)和新格式(Phases)
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
        /// 副标题/课程主题
        /// </summary>
        [JsonProperty("subtitle")]
        public string Subtitle { get; set; }

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

        #region 旧格式兼容字段 (L01, L02)

        /// <summary>
        /// 【旧格式】步骤说明
        /// </summary>
        public List<LessonStep> Steps { get; set; } = new List<LessonStep>();

        /// <summary>
        /// 【旧格式】代码模板文件路径
        /// </summary>
        public string TemplatePath { get; set; }

        /// <summary>
        /// 【旧格式】检查规则配置路径
        /// </summary>
        public string CheckRulesPath { get; set; }

        /// <summary>
        /// 【旧格式】奖励经验值
        /// </summary>
        public int RewardExp { get; set; } = 100;

        /// <summary>
        /// 【旧格式】奖励徽章ID
        /// </summary>
        public string RewardBadge { get; set; }

        #endregion

        #region 新格式字段 (L03+)

        /// <summary>
        /// 【新格式】课程阶段列表
        /// </summary>
        [JsonProperty("phases")]
        public List<LessonPhase> Phases { get; set; } = new List<LessonPhase>();

        #endregion

        #region 通用字段

        /// <summary>
        /// 关卡图标（游戏化元素）
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// 关卡位置坐标（在地图上的位置）
        /// </summary>
        public int MapX { get; set; }

        public int MapY { get; set; }

        #endregion

        #region 便捷访问器

        /// <summary>
        /// 判断是否为新的多阶段课程格式
        /// </summary>
        [JsonIgnore]
        public bool IsNewFormat => Phases != null && Phases.Count > 0;

        /// <summary>
        /// 获取知识阶段
        /// </summary>
        [JsonIgnore]
        public KnowledgePhase KnowledgePhase =>
            Phases?.OfType<KnowledgePhase>().FirstOrDefault();

        /// <summary>
        /// 获取实践阶段
        /// </summary>
        [JsonIgnore]
        public PracticePhase PracticePhase =>
            Phases?.OfType<PracticePhase>().FirstOrDefault();

        /// <summary>
        /// 获取挑战阶段
        /// </summary>
        [JsonIgnore]
        public ChallengePhase ChallengePhase =>
            Phases?.OfType<ChallengePhase>().FirstOrDefault();

        /// <summary>
        /// 获取总经验值（新格式下各阶段之和，旧格式下为RewardExp）
        /// </summary>
        [JsonIgnore]
        public int TotalExperience
        {
            get
            {
                if (IsNewFormat)
                {
                    int total = 0;
                    if (KnowledgePhase != null)
                        total += KnowledgePhase.TotalXp;
                    if (PracticePhase != null)
                        total += PracticePhase.RewardExp;
                    if (ChallengePhase != null)
                        total += ChallengePhase.RewardExp;
                    return total;
                }
                return RewardExp;
            }
        }

        /// <summary>
        /// 获取主徽章（实践阶段或旧格式的徽章）
        /// </summary>
        [JsonIgnore]
        public string PrimaryBadge =>
            IsNewFormat ? PracticePhase?.RewardBadge : RewardBadge;

        /// <summary>
        /// 获取代码模板路径（兼容新旧格式）
        /// </summary>
        [JsonIgnore]
        public string EffectiveTemplatePath =>
            IsNewFormat ? PracticePhase?.TemplatePath : TemplatePath;

        /// <summary>
        /// 获取检查规则路径（兼容新旧格式）
        /// </summary>
        [JsonIgnore]
        public string EffectiveCheckRulesPath =>
            IsNewFormat ? PracticePhase?.CheckRulesPath : CheckRulesPath;

        /// <summary>
        /// 获取实践阶段的步骤（兼容新旧格式）
        /// </summary>
        [JsonIgnore]
        public List<LessonStep> EffectiveSteps =>
            IsNewFormat ? PracticePhase?.Steps : Steps;

        #endregion
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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace TeenCppEdu.Core.Models
{
    /// <summary>
    /// 阶段类型枚举
    /// </summary>
    public enum PhaseType
    {
        Knowledge,   // 知识学习阶段
        Practice,    // 实践编程阶段
        Challenge    // 挑战阶段（Bug猎手等）
    }

    /// <summary>
    /// 阶段基类 - 使用JsonConverter实现多态反序列化
    /// </summary>
    [JsonConverter(typeof(LessonPhaseConverter))]
    public abstract class LessonPhase
    {
        [JsonProperty("type")]
        public abstract PhaseType Type { get; }

        [JsonProperty("title")]
        public string Title { get; set; }
    }

    /// <summary>
    /// 知识学习阶段 - 包含场景、概念、测验、填空
    /// </summary>
    public class KnowledgePhase : LessonPhase
    {
        public override PhaseType Type => PhaseType.Knowledge;

        [JsonProperty("sections")]
        public List<KnowledgeSection> Sections { get; set; } = new List<KnowledgeSection>();

        [JsonProperty("totalXp")]
        public int TotalXp { get; set; }
    }

    /// <summary>
    /// 知识阶段中的小节类型
    /// </summary>
    public enum SectionType
    {
        Scene,      // 场景故事
        Concept,    // 概念讲解
        Quiz,       // 知识测验
        FillBlank   // 代码填空
    }

    /// <summary>
    /// 知识小节基类
    /// </summary>
    [JsonConverter(typeof(KnowledgeSectionConverter))]
    public abstract class KnowledgeSection
    {
        [JsonProperty("type")]
        public abstract SectionType SectionType { get; }

        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }

    /// <summary>
    /// 场景故事小节
    /// </summary>
    public class SceneSection : KnowledgeSection
    {
        public override SectionType SectionType => SectionType.Scene;
    }

    /// <summary>
    /// 概念讲解小节
    /// </summary>
    public class ConceptSection : KnowledgeSection
    {
        public override SectionType SectionType => SectionType.Concept;

        [JsonProperty("visual")]
        public VisualElement Visual { get; set; }

        [JsonProperty("keyPoints")]
        public List<string> KeyPoints { get; set; } = new List<string>();
    }

    /// <summary>
    /// 可视化元素
    /// </summary>
    public class VisualElement
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("caption")]
        public string Caption { get; set; }
    }

    /// <summary>
    /// 测验小节
    /// </summary>
    public class QuizSection : KnowledgeSection
    {
        public override SectionType SectionType => SectionType.Quiz;

        [JsonProperty("questions")]
        public List<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
    }

    /// <summary>
    /// 测验题目
    /// </summary>
    public class QuizQuestion
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("question")]
        public string Question { get; set; }

        [JsonProperty("options")]
        public List<string> Options { get; set; } = new List<string>();

        [JsonProperty("answer")]
        public int Answer { get; set; } // 正确答案的索引

        [JsonProperty("explanation")]
        public string Explanation { get; set; }

        [JsonProperty("xp")]
        public int Xp { get; set; }
    }

    /// <summary>
    /// 代码填空小节
    /// </summary>
    public class FillBlankSection : KnowledgeSection
    {
        public override SectionType SectionType => SectionType.FillBlank;

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("blanks")]
        public List<BlankField> Blanks { get; set; } = new List<BlankField>();
    }

    /// <summary>
    /// 填空字段
    /// </summary>
    public class BlankField
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("hint")]
        public string Hint { get; set; }

        [JsonProperty("answer")]
        public string Answer { get; set; }

        [JsonProperty("altAnswers")]
        public List<string> AltAnswers { get; set; } = new List<string>();

        [JsonProperty("xp")]
        public int Xp { get; set; }

        /// <summary>
        /// 检查答案是否正确（支持备选答案）
        /// </summary>
        public bool CheckAnswer(string userAnswer)
        {
            if (string.IsNullOrEmpty(userAnswer)) return false;

            string normalized = userAnswer.Trim();
            if (normalized.Equals(Answer?.Trim(), StringComparison.OrdinalIgnoreCase))
                return true;

            if (AltAnswers != null)
            {
                foreach (var alt in AltAnswers)
                {
                    if (normalized.Equals(alt?.Trim(), StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// 实践编程阶段
    /// </summary>
    public class PracticePhase : LessonPhase
    {
        public override PhaseType Type => PhaseType.Practice;

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("steps")]
        public List<LessonStep> Steps { get; set; } = new List<LessonStep>();

        [JsonProperty("templatePath")]
        public string TemplatePath { get; set; }

        [JsonProperty("checkRulesPath")]
        public string CheckRulesPath { get; set; }

        [JsonProperty("rewardExp")]
        public int RewardExp { get; set; }

        [JsonProperty("rewardBadge")]
        public string RewardBadge { get; set; }
    }

    /// <summary>
    /// 挑战阶段
    /// </summary>
    public class ChallengePhase : LessonPhase
    {
        public override PhaseType Type => PhaseType.Challenge;

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("mode")]
        public string Mode { get; set; } // bug_hunt, etc.

        [JsonProperty("difficulty")]
        public string Difficulty { get; set; }

        [JsonProperty("buggyCode")]
        public string BuggyCode { get; set; }

        [JsonProperty("hints")]
        public List<string> Hints { get; set; } = new List<string>();

        [JsonProperty("expectedFix")]
        public string ExpectedFix { get; set; }

        [JsonProperty("altFixes")]
        public List<string> AltFixes { get; set; } = new List<string>();

        [JsonProperty("explanation")]
        public string Explanation { get; set; }

        [JsonProperty("rewardExp")]
        public int RewardExp { get; set; }

        [JsonProperty("rewardBadge")]
        public string RewardBadge { get; set; }

        /// <summary>
        /// 检查修复是否正确
        /// </summary>
        public bool CheckFix(string userFix)
        {
            if (string.IsNullOrEmpty(userFix)) return false;

            string normalized = userFix.Trim();
            if (normalized.Equals(ExpectedFix?.Trim(), StringComparison.OrdinalIgnoreCase))
                return true;

            if (AltFixes != null)
            {
                foreach (var alt in AltFixes)
                {
                    if (normalized.Equals(alt?.Trim(), StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            }
            return false;
        }
    }

    #region JSON Converters

    /// <summary>
    /// LessonPhase多态反序列化转换器
    /// </summary>
    public class LessonPhaseConverter : JsonConverter<LessonPhase>
    {
        public override LessonPhase ReadJson(JsonReader reader, Type objectType, LessonPhase existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var typeToken = jsonObject["type"];

            if (typeToken == null)
                throw new JsonSerializationException("Phase must have a 'type' property");

            LessonPhase phase;
            var typeStr = typeToken.ToString().ToLower();

            switch (typeStr)
            {
                case "knowledge":
                    phase = new KnowledgePhase();
                    break;
                case "practice":
                    phase = new PracticePhase();
                    break;
                case "challenge":
                    phase = new ChallengePhase();
                    break;
                default:
                    throw new JsonSerializationException($"Unknown phase type: {typeStr}");
            }

            serializer.Populate(jsonObject.CreateReader(), phase);
            return phase;
        }

        public override void WriteJson(JsonWriter writer, LessonPhase value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }

    /// <summary>
    /// KnowledgeSection多态反序列化转换器
    /// </summary>
    public class KnowledgeSectionConverter : JsonConverter<KnowledgeSection>
    {
        public override KnowledgeSection ReadJson(JsonReader reader, Type objectType, KnowledgeSection existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var typeToken = jsonObject["type"];

            if (typeToken == null)
                throw new JsonSerializationException("Section must have a 'type' property");

            KnowledgeSection section;
            var typeStr = typeToken.ToString().ToLower();

            switch (typeStr)
            {
                case "scene":
                    section = new SceneSection();
                    break;
                case "concept":
                    section = new ConceptSection();
                    break;
                case "quiz":
                    section = new QuizSection();
                    break;
                case "fillblank":
                    section = new FillBlankSection();
                    break;
                default:
                    throw new JsonSerializationException($"Unknown section type: {typeStr}");
            }

            serializer.Populate(jsonObject.CreateReader(), section);
            return section;
        }

        public override void WriteJson(JsonWriter writer, KnowledgeSection value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }

    #endregion
}

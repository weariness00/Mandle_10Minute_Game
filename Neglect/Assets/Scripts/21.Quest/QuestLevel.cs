using System.ComponentModel;
using System.Reflection;

namespace Quest
{
    public enum QuestLevel
    {
        None = -1,
        [Description("쉬움")] Easy,
        [Description("보통")] Normal,
        [Description("어려움")] Hard,
        [Description("히든")] Hidden
    }

    static class QuestLevelExtension
    {
        public static string ToStringEx(this QuestLevel value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }
    }
}
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AutoMarket.Helpers
{
    public static class EnumExtensions
    {
        public static string ToDisplayName(this Enum value)
        {
            var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            if (member == null) return value.ToString();

            var attr = member.GetCustomAttribute<DisplayAttribute>();
            return attr?.Name ?? value.ToString();
        }
    }
}
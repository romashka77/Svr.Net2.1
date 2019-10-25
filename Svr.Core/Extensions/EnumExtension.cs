using System;
using System.ComponentModel;

namespace Svr.Core.Extensions
{
    public static class EnumExtension
    {
        public static string GetDescription(this Enum enumElement)
        {
            var type = enumElement.GetType();

            var memInfo = type.GetMember(enumElement.ToString());
            if (memInfo == null || memInfo.Length <= 0) return enumElement.ToString();
            var attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attrs != null && attrs.Length > 0)
                return ((DescriptionAttribute)attrs[0]).Description;
            return enumElement.ToString();
        }
    }
}

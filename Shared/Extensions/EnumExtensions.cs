using System;
using System.ComponentModel;
using System.Globalization;

namespace Shared.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            string description = null;

            if (!(e is Enum)) return null;
            var type = e.GetType();
            var values = Enum.GetValues(type);

            foreach (int val in values)
                if (val == e.ToInt32(CultureInfo.InvariantCulture))
                {
                    var memInfo = type.GetMember(type.GetEnumName(val) ?? throw new InvalidOperationException());
                    var descriptionAttributes = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (descriptionAttributes.Length > 0)
                        // we're only getting the first description we find
                        // others will be ignored
                        description = ((DescriptionAttribute) descriptionAttributes[0]).Description;

                    break;
                }

            return description;
        }
    }
}
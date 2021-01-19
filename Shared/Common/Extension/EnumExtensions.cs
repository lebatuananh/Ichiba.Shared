using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Shared.Common.Extension
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            try
            {
                var configName = enumValue.GetType()
                    .GetMember(enumValue.ToString())
                    .First()?
                    .GetCustomAttribute<DisplayAttribute>()?
                    .GetName();

                if (string.IsNullOrWhiteSpace(configName)) return enumValue.ToString();

                return configName;
            }
            catch
            {
                return enumValue.ToString();
            }
        }

        public static string GetConfig(this string key)
        {
            if (ConfigSetting.Configs.TryGetValue(key, out var configValue)) return configValue;

            return string.Empty;
        }

        public static string GetConfig(this Enum @enum)
        {
            return GetConfig(@enum.GetDisplayName());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Shared.Common.Extension;

namespace Shared.Common
{
    public class ConfigSetting
    {
        public static IDictionary<string, string> Configs = new Dictionary<string, string>();

        public static void Init(IConfigurationProvider configurationProvider, ICollection<string> keys)
        {
            if (configurationProvider == null) throw new ArgumentNullException(nameof(configurationProvider));

            if (keys == null) throw new ArgumentNullException(nameof(keys));

            keys.Select(m =>
            {
                if (!configurationProvider.TryGet(m, out var valueConfig)) return false;

                return Configs.TryAdd(m, valueConfig);
            }).ToList();
        }

        public static void Init<TEnum>(IConfigurationProvider configurationProvider)
            where TEnum : struct, IConvertible
        {
            var enumType = typeof(TEnum);

            if (!enumType.IsEnum) throw new ArgumentNullException(nameof(TEnum));

            var enumValues = Enum.GetValues(enumType);
            var keys = new List<string>();
            var key = string.Empty;

            foreach (Enum item in enumValues)
            {
                key = item.GetDisplayName() ?? item.ToString();

                keys.Add(key);
            }

            Init(configurationProvider, keys);
        }
    }
}
using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Laobian.Share.Setting
{
    public class CommonSetting
    {
        [SettingConfig("SEND_GRID_API_KEY", IsRequired = true)]
        public string SendGridApiKey { get; set; }

        [SettingConfig("ADMIN_USER_NAME", IsRequired = true)]
        public string AdminUserName { get; set; }

        [SettingConfig("ADMIN_USER_PASSWORD", IsRequired = true)]
        public string AdminPassword { get; set; }

        [SettingConfig("ADMIN_CHINESE_NAME", DefaultValue = "中文名")]
        public string AdminChineseName { get; set; }

        [SettingConfig("ADMIN_ENGLISH_NAME", DefaultValue = "English Name")]
        public string AdminEnglishName { get; set; }

        [SettingConfig("ADMIN_EMAIL", DefaultValue = "test@test.com")]
        public string AdminEmail { get; set; }

        [SettingConfig("COMMAND_LINE_NAME", DefaultValue = "cmd")]
        public string CommandLineName { get; set; }

        [SettingConfig("COMMAND_LINE_BEGIN_ARG", DefaultValue = "/C")]
        public string CommandLineBeginArg { get; set; }

        [SettingConfig("ENDPOINT_LOCAL_API")] public string ApiLocalEndpoint { get; set; }

        [SettingConfig("ENDPOINT_LOCAL_BLOG")] public string BlogLocalEndpoint { get; set; }

        [SettingConfig("ENDPOINT_LOCAL_ADMIN")]
        public string AdminLocalEndpoint { get; set; }

        [SettingConfig("ENDPOINT_REMOTE_API")] public string ApiRemoteEndpoint { get; set; }

        [SettingConfig("ENDPOINT_REMOTE_BLOG")]
        public string BlogRemoteEndpoint { get; set; }

        [SettingConfig("ENDPOINT_REMOTE_ADMIN")]
        public string AdminRemoteEndpoint { get; set; }

        [SettingConfig("DATA_PROTECTION_KEY_PATH", IsRequired = true)]
        public string DataProtectionKeyPath { get; set; }

        [SettingConfig("DATA_PROTECTION_APP_NAME", DefaultValue = "LAOBIAN")]
        public string DataProtectionAppName { get; set; }

        [SettingConfig("SHARED_COOKIE_NAME", IsRequired = true)]
        public string SharedCookieName { get; set; }

        public void Setup(IConfiguration config)
        {
            foreach (var propertyInfo in GetType().GetProperties())
            {
                var attr = propertyInfo.GetCustomAttribute<SettingConfigAttribute>();
                if (attr != null)
                {
                    var val = config.GetValue(propertyInfo.PropertyType, attr.Key);
                    if (val == null)
                    {
                        if (attr.IsRequired) throw new Exception($"Missing required setting: {attr.Key}");

                        val = Convert.ChangeType(attr.DefaultValue, propertyInfo.PropertyType);
                    }

                    propertyInfo.SetValue(this, Convert.ChangeType(val, propertyInfo.PropertyType));
                }
            }
        }
    }
}
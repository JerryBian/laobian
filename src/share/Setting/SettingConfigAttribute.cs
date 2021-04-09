using System;

namespace Laobian.Share.Setting
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SettingConfigAttribute : Attribute
    {
        public SettingConfigAttribute(string key)
        {
            Key = key;
        }

        public bool IsRequired { get; set; }

        public string Key { get; }

        public object DefaultValue { get; set; }
    }
}
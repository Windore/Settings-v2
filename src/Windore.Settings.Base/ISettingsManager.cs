using System;
using System.Collections.Generic;

namespace Windore.Settings.Base
{
    public interface ISettingsManager
    {
        object GetSettingValue(string category, string settingName);
        void SetSettingValue(string category, string settingName, object newValue);
        void SetSettingValueFromString(string category, string settingName, string stringValue);
        string GetSettingValueAsString(string category, string settingName) ;
        bool CheckStringValueForSetting(string category, string settingName, string stringValue, out string message);
        Type GetSettingType(string category, string settingName);
        Dictionary<string, List<string>> GetSettings();
    }
}
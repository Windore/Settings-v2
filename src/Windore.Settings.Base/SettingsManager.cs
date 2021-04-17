using System.Reflection;
using System.Collections.Generic;
using System;
using System.Text;
using System.Runtime.ExceptionServices;

namespace Windore.Settings.Base
{
    public class SettingsManager<T>
    {
        private class Category
        {
            public Dictionary<string, PropertyInfo> Settings = new Dictionary<string, PropertyInfo>();
        }

        private Dictionary<string, Category> categories = new Dictionary<string, Category>();
        private Dictionary<Type, IConvertFunction> converters;

        private T settingsObj;

        public SettingsManager() 
        {
            converters = new Dictionary<Type, IConvertFunction>();

            AddDefaultConvertFunctions();
        }

        public void SetSettingObject(T settingsObject) 
        {
            settingsObj = settingsObject;
            categories.Clear();
            VerifySettignsObject();
        }

        public void AddConvertFunction<TP>(ConvertFunction<TP> function) 
        {
            converters[typeof(TP)] = function;
        }

        public object GetSettingValue(string category, string settingName) 
        {
            PropertyInfo prop = categories[category].Settings[settingName];
            return prop.GetValue(settingsObj);
        }

        public void SetSettingValue(string category, string settingName, object newValue) 
        {
            PropertyInfo prop = categories[category].Settings[settingName];
            prop.SetValue(settingsObj, newValue);
        }

        public Dictionary<string, List<string>> GetSettings() 
        {
            var settings = new Dictionary<string, List<string>>();
            foreach (string category in categories.Keys) 
            {
                List<string> settingsInCat = new List<string>();

                foreach(string setting in categories[category].Settings.Keys) 
                {
                    settingsInCat.Add(setting);
                }

                settings[category] = settingsInCat;
            }

            return settings;
        }

        public string GenerateSettingsString() 
        {
            // Categories and settings need to be sorted alphabetically

            StringBuilder builder = new StringBuilder();

            List<string> cats = new List<string>(categories.Keys);
            cats.Sort();

            foreach(string cat in cats) 
            {   
                builder.Append(":");
                builder.Append(cat);
                builder.AppendLine(":");

                List<string> settings = new List<string>(categories[cat].Settings.Keys);
                settings.Sort();

                foreach (string setting in settings) 
                {
                    builder.Append(setting);
                    builder.Append("=");
                    builder.AppendLine(GetSettingValueAsString(cat, setting));
                }
            }

            return builder.ToString();
        }

        public void ParseSettingsString(string settingsString) 
        {
            string[] lines = settingsString.Split("\n");
            string currentCategory = string.Empty;
            int lineNum = 0;

            foreach(string line in lines) 
            {
                lineNum++;

                if (line.StartsWith('#') || string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                if (line.StartsWith(':') && line.EndsWith(':'))
                {
                    currentCategory = line.Substring(1, line.Length - 2);

                    if (!categories.ContainsKey(currentCategory)) 
                    {
                        throw new KeyNotFoundException($"The given key '{currentCategory}' was not present in the dictionary.");
                    }
                    
                    continue;
                }

                if (!line.Contains('=')) 
                {
                    throw new FormatException($"Missing a '=' at line {lineNum}.");
                }

                string[] lineDt = line.Split('=', 2);
                SetSettingValueFromString(currentCategory, lineDt[0], lineDt[1]);
            }
        }

        public string GetSettingValueAsString(string category, string settingName) 
        {
            PropertyInfo prop = categories[category].Settings[settingName];

            // ConvertToString method needs to be called dynamically
            // since the converters dict contains multiple differently typed ConverFunctions
            var func = converters[prop.PropertyType];
            return (string)func.GetType().GetMethod("ConvertToString").Invoke(func, new [] { prop.GetValue(settingsObj) });
        }

        public void SetSettingValueFromString(string category, string settingName, string stringValue) 
        {
            PropertyInfo prop = categories[category].Settings[settingName];

            // ConvertFromString method needs to be called dynamically
            // since the converters dict contains multiple differently typed ConverFunctions
            var func = converters[prop.PropertyType];

            try 
            {
                var value = func.GetType().GetMethod("ConvertFromString").Invoke(func, new [] { stringValue });
                prop.SetValue(settingsObj, value);
            }
            catch(TargetInvocationException ex) // This is used so that ArgumentExceptions from ConvertFunctions are thrown
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }
        }

        private void AddDefaultConvertFunctions() 
        {
            AddConvertFunction<int>(new ConvertFunction<int>
            (
                (num) => num.ToString(),
                (str) => 
                {
                    if (int.TryParse(str, out int num)) 
                    {
                        return num;
                    }

                    throw new ArgumentException($"Cannot parse string {str} to int");
                }
            ));

            AddConvertFunction<double>(new ConvertFunction<double>
            (
                (num) => num.ToString(),
                (str) => 
                {
                    if (double.TryParse(str, out double num)) 
                    {
                        return num;
                    }

                    throw new ArgumentException($"Cannot parse string {str} to double");
                }
            ));

            AddConvertFunction<bool>(new ConvertFunction<bool>
            (
                (bol) => bol ? "true" : "false",
                (str) => 
                {
                    if (bool.TryParse(str, out bool bol)) 
                    {
                        return bol;
                    }

                    throw new ArgumentException($"Cannot parse string {str} to bool");
                }
            ));

            // This one is obvious but needs to be defined
            AddConvertFunction<string>(new ConvertFunction<string>
            (
                (str) => str,
                (str) => str
            ));
        }

        private void VerifySettignsObject()
        {
            foreach(PropertyInfo property in typeof(T).GetProperties()) 
            {
                SettingAttribute attribute = property.GetCustomAttribute<SettingAttribute>();
                if (attribute == null) 
                {
                    continue;
                }

                VerifyProperty(property, attribute);

                // Using just the indexer doesn't work because a category might not be initialized yet
                if (!categories.ContainsKey(attribute.Category)) 
                {
                    categories.Add(attribute.Category, new Category());
                }
                categories[attribute.Category].Settings[attribute.Name] = property;

            }

            if (categories.Count == 0) 
            {
                throw new ArgumentException("Given settings object does not contain any public setting properties.");
            }
        }

        private void VerifyProperty(PropertyInfo property, SettingAttribute attribute) 
        {
            if (property.GetGetMethod() == null) 
            {
                throw new NonReadWriteSettingException($"Cannot read values from property: {property.Name}.");
            }

            if (property.GetSetMethod() == null) 
            {
                throw new NonReadWriteSettingException($"Cannot write values to property: {property.Name}.");
            }

            if (!converters.ContainsKey(property.PropertyType))
            {
                throw new InvalidCastException($"Cannot convert setting with type {property.PropertyType} to and from string");
            }

            if (attribute.Name.Contains(':') || attribute.Name.Contains('=') || attribute.Name.Contains('#')) 
            {
                throw new InvalidNameException($"Setting {attribute.Name} contains forbidden characters ':', '=' or '#'.");
            }

            if (attribute.Category.Contains(':') || attribute.Category.Contains('=') || attribute.Category.Contains('#')) 
            {
                throw new InvalidNameException($"Category {attribute.Category} contains forbidden characters ':', '=' or '#'.");
            }

            if (categories.ContainsKey(attribute.Category) && categories[attribute.Category].Settings.ContainsKey(attribute.Name))
            {
                throw new DuplicateNameInCategoryException($"Category {attribute.Category} already contains setting named {attribute.Name}.");
            }
        }
    }
}
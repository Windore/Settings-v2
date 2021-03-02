using System.Reflection;
using System.Collections.Generic;
using System;

namespace Windore.Settings.Base
{
    public class SettingsManager<T>
    {
        private Dictionary<Type, IConvertFunction> converters;

        private T settingsObj;

        public SettingsManager() 
        {
            converters = new Dictionary<Type, IConvertFunction>();

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

                    throw new ArgumentException($"Cannot parse string {str} to int");
                }
            ));

            // This one is obvious but needs to be defined
            AddConvertFunction<string>(new ConvertFunction<string>
            (
                (str) => str,
                (str) => str
            ));
        }

        public void SetSettingObject(T settingsObject) 
        {
            settingsObj = settingsObject;
            VerifySettignsObject();
        }

        public void AddConvertFunction<TP>(ConvertFunction<TP> function) 
        {
            converters[typeof(TP)] = function;
        }

        public string GenerateSettingsFile() 
        {
            return string.Empty;
        }

        private void VerifySettignsObject()
        {
            List<string> settings = new List<string>();

            foreach(PropertyInfo property in typeof(T).GetProperties()) 
            {
                SettingAttribute attribute = property.GetCustomAttribute<SettingAttribute>();
                if (attribute == null) 
                {
                    continue;
                }

                if (!property.CanWrite) 
                {
                    throw new NonWritableSettingException($"Cannot write values to property: {property.Name}.");
                }

                if (!converters.ContainsKey(property.PropertyType))
                {
                    throw new InvalidCastException($"Cannot convert setting with type {property.PropertyType} to and from string");
                }

                string val = attribute.Category + attribute.Name;

                if (settings.Contains(val))
                {
                    throw new DuplicateNameInCategoryException($"Category {attribute.Category} already contains setting named {attribute.Name}.");
                }

                settings.Add(val);
            }

            if (settings.Count == 0) 
            {
                throw new ArgumentException("Given settings object does not contain any public setting properties.");
            }
        }
    }
}
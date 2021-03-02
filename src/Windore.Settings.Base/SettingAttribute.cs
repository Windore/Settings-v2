using System;

namespace Windore.Settings.Base
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SettingAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Category { get; private set; }
        public SettingAttribute(string name, string category) {
            Name = name;
            Category = category;
        }
    }
}

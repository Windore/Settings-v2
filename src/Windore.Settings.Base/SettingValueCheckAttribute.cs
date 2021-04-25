using System;

namespace Windore.Settings.Base
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class SettingValueCheckAttribute : Attribute 
    {
        public abstract bool CheckValue(object value, out string message);
    }
}
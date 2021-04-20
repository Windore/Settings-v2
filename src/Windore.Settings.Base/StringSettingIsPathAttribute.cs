using System;
using System.IO;

namespace Windore.Settings.Base
{
    public class StringSettingIsPathAttribute : SettingValueCheckAttribute 
    {
        public override bool CheckValue(object value)
        {
            if (value is string str) 
            {
                // This way allows for somewhat testing if a given path is correct.
                // However, it's not perfect.
                FileInfo fi = null;
                try
                {
                    fi = new FileInfo(str);
                }
                catch (ArgumentException) { }
                catch (PathTooLongException) { }
                catch (NotSupportedException) { }

                if (fi != null && Path.IsPathFullyQualified(str)) 
                {
                    return true;
                }
            }
            return false;
        }
    }
}
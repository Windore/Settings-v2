using System;

namespace Windore.Settings.Base
{
    public class IntSettingValueLimitsAttribute : SettingValueCheckAttribute 
    {
        private int min;
        private int max;

        public IntSettingValueLimitsAttribute(int minValue, int maxValue) 
        {
            min = minValue;
            max = maxValue;
        }

        public override bool CheckValue(object value, out string message) 
        {
            if (value is int number) 
            {
                if (number < min) 
                {
                    message = "Given value is too small";
                    return false;
                }
                if (number > max) 
                {
                    message = "Given value is too large";
                    return false;
                }
                message = "";
                return true;
            }

            message = "Not a number";

            return false;
        }
    }
}
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

        public override bool CheckValue(object value) 
        {
            if (value is int number) 
            {
                return number > min && number < max;
            }
            return false;
        }
    }
}
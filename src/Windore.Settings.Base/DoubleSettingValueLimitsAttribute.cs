namespace Windore.Settings.Base
{
    public class DoubleSettingValueLimitsAttribute : SettingValueCheckAttribute 
    {
        private double min;
        private double max;

        public DoubleSettingValueLimitsAttribute(double minValue, double maxValue) 
        {
            min = minValue;
            max = maxValue;
        }

        public override bool CheckValue(object value) 
        {
            if (value is double number) 
            {
                return number > min && number < max;
            }
            return false;
        }
    }
}
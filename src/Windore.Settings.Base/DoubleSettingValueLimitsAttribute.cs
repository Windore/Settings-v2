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

        public override bool CheckValue(object value, out string message) 
        {
            if (value is double number) 
            {
                if (number < min) 
                {
                    message = "Given value is too small.";
                    return false;
                }
                if (number > max) 
                {
                    message = "Given value is too large.";
                    return false;
                }
                message = "";
                return true;
            }

            message = "Not a number.";

            return false;
        }
    }
}
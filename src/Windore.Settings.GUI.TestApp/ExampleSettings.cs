using Windore.Settings.Base;

namespace Windore.Settings.GUI.TestApp
{
    public class ExampleSettings
    {
        [Setting("Your name", "Basic Information")]
        public string Name { get; set; } = "";
        [Setting("Your age", "Basic Information")]
        [IntSettingValueLimits(0, 130)]
        public int Age { get; set; }
        [Setting("Your height", "Basic Information")]
        [DoubleSettingValueLimits(0, 3)]
        public double Height { get; set; }
        [Setting("Do you agree?", "Other")]
        public bool Agree { get; set; }
    }
}
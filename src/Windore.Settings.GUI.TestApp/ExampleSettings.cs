using Windore.Settings.Base;

namespace Windore.Settings.GUI.TestApp
{
    public class ExampleSettings
    {
        [Setting("Your name", "Basic Information")]
        public string Name { get; set; } = "";
        [Setting("Your age", "Basic Information")]
        public int Age { get; set; }
        [Setting("Your height", "Basic Information")]
        public double Height { get; set; }
        [Setting("Do you agree?", "Other")]
        public bool Agree { get; set; }
    }
}
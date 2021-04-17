using Avalonia.Controls;
using System;
using System.Collections.Generic;

namespace Windore.Settings.GUI
{
    // This allows list containing different generic SettingControlFunctions
    internal interface ISettingControlFunction {}

    public class SettingControlFunction<T>
    {
        private Func<string, string, UserControl> getUserControl;

        public SettingControlFunction(Func<string, string, UserControl> getUserControlFunction) 
        {
            getUserControl = getUserControlFunction;
        }

        public UserControl GetSettingControl(string category, string name) => getUserControl(category, name);
    }
}
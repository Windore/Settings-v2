using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Input;
using Windore.Settings.Base;
using System;

namespace Windore.Settings.GUI
{
    public class DefaultSettingControl : UserControl
    {
        private TextBlock nameTB;
        private TextBlock invalidInputTB;
        private TextBox input;
        private ISettingsManager manager;
        private string category;
        private string name;
        private string previousText;

        public DefaultSettingControl() 
        {
            throw new NotImplementedException("This constructor exists because AvaloniaUI requires it to exist. It's not supposed to be used");
        }

        public DefaultSettingControl(string category, string name, ISettingsManager manager)
        {
            this.category = category;
            this.name = name;
            this.manager = manager;
            InitializeComponent();

            nameTB.Text = name;
            input.Text = manager.GetSettingValueAsString(category, name);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            nameTB = this.Find<TextBlock>("settingNameTB");
            input = this.Find<TextBox>("settingInputTB");
            invalidInputTB = this.Find<TextBlock>("invalidTB");

            input.KeyUp += InputTextChanged;
        }

        private void InputTextChanged(object sender, KeyEventArgs e)
        {
            if (input.Text == previousText) return;
            previousText = input.Text;
            
            if (manager.CheckStringValueForSetting(category, name, input.Text, out string msg)) 
            {
                manager.SetSettingValueFromString(category, name, input.Text);
            }
            invalidInputTB.Text = msg;
        }
    }
}
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Windore.Settings.Base;
using System;

namespace Windore.Settings.GUI
{
    public class BoolSettingControl : UserControl
    {
        private CheckBox settingCheckBox;
        private TextBlock settingCheckBoxName;
        private ISettingsManager manager;
        private string category;
        private string name;

        public BoolSettingControl() 
        {
            throw new NotImplementedException("This constructor exists because AvaloniaUI requires it to exist. It's not supposed to be used");
        }

        public BoolSettingControl(string category, string name, ISettingsManager manager)
        {
            this.category = category;
            this.name = name;
            this.manager = manager;
            InitializeComponent();
            
            settingCheckBox.IsChecked = (bool)manager.GetSettingValue(category, name);
            settingCheckBoxName.Text = name;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            settingCheckBox = this.FindControl<CheckBox>("settingCheckBox");
            settingCheckBoxName = this.FindControl<TextBlock>("settingCheckBoxName");
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            manager.SetSettingValue(category, name, (bool)settingCheckBox.IsChecked);
        }
    }
}
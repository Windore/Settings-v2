using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Collections.Generic;
using System;
using Windore.Settings.Base;

namespace Windore.Settings.GUI
{
    public class SettingsWindow : Window
    {
        private Dictionary<Type, ISettingControlFunction> controlLookupDict = new Dictionary<Type, ISettingControlFunction>();
        private ISettingsManager manager;
        private TabControl categoryTabControl;

        public SettingsWindow()
        {
            throw new NotImplementedException("This constructor exists because AvaloniaUI requires it to exist. It's not supposed to be used");
        }

        public SettingsWindow(ISettingsManager manager)
        {
            CreateDefaultSettingControlFuncs();
            this.manager = manager;
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            CreateControls();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            categoryTabControl = this.Find<TabControl>("tabControl");
        }

        private void CreateControls() 
        {
            var settings = manager.GetSettings();

            List<TabItem> tabItems = new List<TabItem>();

            foreach (string category in settings.Keys)
            {
                List<UserControl> settingControls = CreateSettingControls(category, settings[category]);
                StackPanel categoryView = new StackPanel
                {
                    Margin = new Thickness(0, 5)
                };
                
                categoryView.Children.AddRange(settingControls);

                tabItems.Add(new TabItem() {
                    Header = category,
                    Content = new ScrollViewer() { Content= categoryView }
                });
            }

            categoryTabControl.Items = tabItems;
        }

        private List<UserControl> CreateSettingControls(string category, List<string> settings) 
        {
            List<UserControl> settingControls = new List<UserControl>();

            foreach(string setting in settings) 
            {
                Type settingType = manager.GetSettingType(category, setting);
                UserControl settingControl;

                if (!controlLookupDict.ContainsKey(settingType)) 
                {
                    settingControl = new DefaultSettingControl(category, setting, manager);
                }
                else 
                {
                    // Because the list contains different generic types which implement ISettingControlFunction
                    // reflection needs to be used to call the method. Or so I believe?
                    settingControl = (UserControl) controlLookupDict[settingType]
                        .GetType()
                        .GetMethod("GetSettingControl")
                        .Invoke(controlLookupDict[settingType], new []{category, setting});
                }

                settingControls.Add(settingControl);
            }

            return settingControls;
        }

        private void CreateDefaultSettingControlFuncs() 
        {
            controlLookupDict[typeof(bool)] = new SettingControlFunction<bool>((category, name) => 
            {
                return new BoolSettingControl(category, name, manager);
            });
        }

        public void AddSettingControlFunction<TP>(SettingControlFunction<TP> function) 
        {
            controlLookupDict[typeof(TP)] = function;
        }
    }
}
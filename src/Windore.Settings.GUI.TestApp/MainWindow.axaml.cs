using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Windore.Settings.Base;
using System.IO;
using System;

namespace Windore.Settings.GUI.TestApp
{
    public class MainWindow : Window
    {
        private SettingsManager<ExampleSettings> manager;

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            manager = new SettingsManager<ExampleSettings>();
            manager.SetSettingObject(new ExampleSettings());

            try 
            {
                manager.ParseSettingsString(File.ReadAllText(Path.GetTempPath() + "/settingstestfile.tmp"));
            }
            catch (Exception) 
            {
                Console.WriteLine("Reading test file failed. It probably just doesn't exist");
            }

            Closed += (_,__) => 
            {
                Console.WriteLine("Saving test file");
                File.WriteAllText(Path.GetTempPath() + "/settingstestfile.tmp", manager.GenerateSettingsString());
            };

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void OpenSettings(object sender, RoutedEventArgs e) 
        {
            SettingsWindow window = new SettingsWindow(manager);
            window.Show();
        }
    }
}
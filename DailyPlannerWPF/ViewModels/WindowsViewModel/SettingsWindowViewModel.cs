using DailyPlannerWPF.Infrastructure.Commands;
using DailyPlannerWPF.ViewModels.Base;
using DailyPlannerWPF.Views.Windows;
using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Input;
using System.Xml.Serialization;

namespace DailyPlannerWPF.ViewModels.WindowsViewModel
{
    [Serializable]
    public class SettingsWindowViewModel : ViewModel
    {
        internal SettingsWindow settingsWindow;

        XmlSerializer formatter = new XmlSerializer(typeof(SettingsWindowViewModel));

        public SettingsWindowViewModel()
        {
            try
            {
                

                using (FileStream fs = new FileStream("SettingsWindowVM.xml", FileMode.Open))
                {
                    SettingsWindowViewModel newSettingsWindowVM = (SettingsWindowViewModel)formatter.Deserialize(fs);
                    IsChecked = newSettingsWindowVM.IsChecked;
                }

            }
            catch { IsChecked = true; }

            if(IsGetAutorunValue("DailyPlanner") == true)
                IsChecked = true;
            else
                IsChecked = false;
        }

        private bool isChecked;
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                Set(ref isChecked, value);

                if (IsChecked == true)
                    SetAutorunValue(true);
                else
                    SetAutorunValue(false);
            }
        }
        public ICommand CloseWindowCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    using (FileStream fs = new FileStream("SettingsWindowVM.xml", FileMode.Create))
                    {
                        formatter.Serialize(fs, this);
                    }

                    settingsWindow.Close();
                });
            }
        }

        public ICommand DragMoveCommand
        {
            get
            {
                return new RelayCommand(obj =>
                {
                    settingsWindow.DragMove();
                });
            }
        }


        public bool SetAutorunValue(bool autorun)
        {
            string ExePath = Assembly.GetExecutingAssembly().Location;

            RegistryKey reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");

            try
            {
                if (autorun)
                    reg.SetValue("DailyPlanner", ExePath);
                else
                    reg.DeleteValue("DailyPlanner");

                reg.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool IsGetAutorunValue(string name)
        {

            RegistryKey reg = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run\\");

            var result = reg.GetValue(name);

            if (result == null)
                return false;
            else
                return true;
        }
    }
}

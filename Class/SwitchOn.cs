using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Windows.Devices.Radios;
using Windows.UI.Xaml.Controls;

namespace BluetoothManager.Class
{
    internal class SwitchOn : Window
    {
        public SwitchOn()
        {
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var accessLevel = await Radio.RequestAccessAsync();
                if (accessLevel != RadioAccessStatus.Allowed)
                {
                    NotifyUser("O aplicativo não tem permissão para controlar os rádios.", NotifyType.ErrorMessage);
                }
                else
                {
                    var radios = await Radio.GetRadiosAsync();
                    foreach (var radio in radios)
                    {
                        RadioSwitchList.Add(new RadioModel(radio, Dispatcher));
                    }
                }
            }
            catch (Exception ex)
            {
                NotifyUser($"Erro ao acessar rádios: {ex.Message}", NotifyType.ErrorMessage);
            }
        }


        public void NotifyUser(string strMessage, NotifyType type)
        {
            if (Dispatcher.CheckAccess())
            {
                UpdateStatus(strMessage, type);
            }
            else
            {
                Dispatcher.Invoke(() => UpdateStatus(strMessage, type));
            }
        }

        private void UpdateStatus(string strMessage, NotifyType type)
        {
            // Implement your status update logic here
        }

    }


    public enum NotifyType
    {
        ErrorMessage,
        // Add other notification types if needed
    }
}

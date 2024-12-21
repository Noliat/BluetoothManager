using System;
using System.Diagnostics;
using System.Windows.Input;
using BluetoothManager.Class.Command;

namespace BluetoothManager.Class
{
    public class BlueSettings
    {
        public ICommand OpenBluetoothSettingsCommand { get; }

        public BlueSettings()
        {
            OpenBluetoothSettingsCommand = new RelayCommand(OpenBluetoothSettings);
        }

        public void OpenBluetoothSettings(object parameter)
        {
            try
            {
                var uri = new Uri("ms-settings:bluetooth");
                Process.Start(new ProcessStartInfo(uri.ToString()) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                // Lidar com a exceção (log ou exibir uma mensagem)
                Console.WriteLine(ex.Message);
            }
        }
    }
}

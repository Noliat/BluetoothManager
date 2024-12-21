using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BluetoothManager.MainWindow;
using BluetoothManager.Class;
using Windows.Devices.Enumeration;

namespace BluetoothManager.Class
{
    internal class PairButton
    {
        private ConnectionStatusInformation ConnectionStatusInformation { get; set; }
        private MainWindow MainWindow {get;set; } = MainWindow.Current;

        private async void PairButton_Click(object sender, RoutedEventArgs e)
        {
            // Gray out the pair button and results view while pairing is in progress.
            MainWindow.Devices.IsEnabled = false;
            ConnectionStatusInformation.UpdateConnectionStatus();

            DeviceInformationDisplay selectedDeviceInfoDisp = MainWindow.Devices.SelectedItem as DeviceInformationDisplay;

            DevicePairingResult dpr = await selectedDeviceInfoDisp.DeviceInformation.Pairing.PairAsync();

            ConnectionStatusInformation.UpdateConnectionStatus();

            UpdatePairingButtons();
            MainWindow.Devices.IsEnabled = true;
        }

        private void UpdatePairingButtons()
        {
            DeviceInformationDisplay selectedDeviceInfoDisp = (DeviceInformationDisplay)MainWindow.Devices.SelectedItem;

            if (null != selectedDeviceInfoDisp &&
                selectedDeviceInfoDisp.DeviceInformation.Pairing.CanPair &&
                !selectedDeviceInfoDisp.DeviceInformation.Pairing.IsPaired) { }
        }
    }
}

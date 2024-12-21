using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth;

namespace BluetoothManager.Class
{
    public class DeviceSelectorInfo
    {
        public string DisplayName { get; set; }
        public string Selector { get; set; }
        public DeviceClass DeviceClassSelector { get; set; } = DeviceClass.All;
    }

    public enum DeviceClass
    {
        All,
        Bluetooth,
        BluetoothLE
    }

    public static class DeviceSelectorChoices
    {
        public static DeviceSelectorInfo Bluetooth =>
            new DeviceSelectorInfo() { DisplayName = "Bluetooth", Selector = "Bluetooth Selector" };

        public static DeviceSelectorInfo BluetoothUnpairedOnly =>
            new DeviceSelectorInfo() { DisplayName = "Bluetooth (unpaired)", Selector = "Bluetooth Unpaired Selector" };

        public static DeviceSelectorInfo BluetoothPairedOnly =>
            new DeviceSelectorInfo() { DisplayName = "Bluetooth (paired)", Selector = "Bluetooth Paired Selector" };

        public static DeviceSelectorInfo BluetoothLE =>
            new DeviceSelectorInfo() { DisplayName = "Bluetooth LE", Selector = "Bluetooth LE Selector" };

        public static DeviceSelectorInfo BluetoothLEUnpairedOnly =>
            new DeviceSelectorInfo() { DisplayName = "Bluetooth LE (unpaired)", Selector = "Bluetooth LE Unpaired Selector" };

        public static DeviceSelectorInfo BluetoothLEPairedOnly =>
            new DeviceSelectorInfo() { DisplayName = "Bluetooth LE (paired)", Selector = "Bluetooth LE Paired Selector" };

        public static List<DeviceSelectorInfo> DevicePickerSelectors
        {
            get
            {
                return new List<DeviceSelectorInfo>
                {
                    BluetoothPairedOnly,
                    BluetoothUnpairedOnly,
                    BluetoothLEPairedOnly,
                    BluetoothLEUnpairedOnly
                };
            }
        }

        public static List<DeviceSelectorInfo> DeviceWatcherSelectors
        {
            get
            {
                return new List<DeviceSelectorInfo>
                {
                    Bluetooth,
                    BluetoothLE
                };
            }
        }

        public static List<DeviceSelectorInfo> BackgroundDeviceWatcherSelectors
        {
            get
            {
                return new List<DeviceSelectorInfo>
                {
                    BluetoothPairedOnly,
                    BluetoothLEPairedOnly
                };
            }
        }

        public static List<DeviceSelectorInfo> PairingSelectors
        {
            get
            {
                return new List<DeviceSelectorInfo>
                {
                    Bluetooth,
                    BluetoothLE
                };
            }
        }
    }
    public class DeviceInformationDisplay : INotifyPropertyChanged
    {
        private ObservableCollection<DeviceInformationDisplay> DevicesCollection;
        public ObservableCollection<DeviceInformationDisplay> devicesCollection;

        public DeviceInformation DeviceInformation { get; private set; }
        public ConnectionStatusInformation ConnectionStatusInformation { get; private set; }

        private DeviceInformation deviceInformation;

        public DeviceInformationDisplay(DeviceInformation deviceInfo)
        {
            deviceInformation = deviceInfo;
        }

        public DeviceInformation _deviceInformatio
        {
            get => deviceInformation;
            private set
            {
                if (deviceInformation != value)
                {
                    deviceInformation = value;
                    OnPropertyChanged(nameof(DeviceInformation));
                }
            }
        }

        public string Name => deviceInformation?.Name ?? "Dispositivo desconhecido";
        public string Id => deviceInformation?.Id ?? string.Empty;
        public string ConnectionStatus { get; set; }
        public bool CanPair => deviceInformation?.Pairing.CanPair ?? false;
        public bool IsPaired => deviceInformation?.Pairing.IsPaired ?? false;
        public DeviceInformationKind Kind => deviceInformation?.Kind ?? default;
        public IReadOnlyDictionary<string, object> Properties => deviceInformation?.Properties ?? new Dictionary<string, object>();


        public string DisplayName => deviceInformation.Name;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void Update(DeviceInformationUpdate deviceInfoUpdate)
        {
            // Implement the update logic based on your requirements.
            deviceInformation.Update(deviceInfoUpdate);
            OnPropertyChanged(nameof(DeviceInformation));
        }

        public BitmapImage Icon
        {
            get
            {
                if (Properties.TryGetValue("System.Devices.Icon", out object iconPath))
                {
                    return new BitmapImage(new Uri((string)iconPath));
                }
                return null;
            }
        }
    }
}

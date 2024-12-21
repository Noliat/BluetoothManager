using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using Windows.Devices.Enumeration;

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
        // Definir Bluetooth como a escolha padrão para o DeviceWatcher
        public static DeviceSelectorInfo DefaultSelector = Bluetooth;

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
        private DeviceInformation _deviceInformation;
        private BitmapImage _glyphBitmapImage;

        public DeviceInformationDisplay(DeviceInformation deviceInfo)
        {
            _deviceInformation = deviceInfo;
        }

        public DeviceInformation DeviceInformation
        {
            get => _deviceInformation;
            private set
            {
                if (_deviceInformation != value)
                {
                    _deviceInformation = value;
                    OnPropertyChanged(nameof(DeviceInformation));
                }
            }
        }

        public DeviceInformationKind Kind => _deviceInformation.Kind;
        public string Id => _deviceInformation.Id;
        public string Name => _deviceInformation.Name;
        public bool CanPair => _deviceInformation.Pairing.CanPair;
        public bool IsPaired => _deviceInformation.Pairing.IsPaired;
        public IReadOnlyDictionary<string, object> Properties => _deviceInformation.Properties;

        public string DisplayName => _deviceInformation.Name;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void Update(DeviceInformationUpdate deviceInfoUpdate)
        {
            // Implement the update logic based on your requirements.
            _deviceInformation.Update(deviceInfoUpdate);
            OnPropertyChanged(nameof(DeviceInformation));
        }
    }
}

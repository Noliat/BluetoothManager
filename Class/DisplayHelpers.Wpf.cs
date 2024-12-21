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
using System.Data;

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
        // Removida a declaração duplicada de DevicesCollection
        public ObservableCollection<DeviceInformationDisplay> DevicesCollection { get; private set; }

        public DeviceInformation DeviceInformation { get; private set; }
        public ConnectionStatusInformation ConnectionStatusInformation { get; private set; }
        public BluetoothBatteryStatus BatteryStatus { get; private set; }

        private DeviceInformation deviceInformation;
        private string connectionStatus;
        private int? batteryLevel;

        public DeviceInformationDisplay(DeviceInformation deviceInfo)
        {
            deviceInformation = deviceInfo;
            BatteryStatus = new BluetoothBatteryStatus();
            ConnectionStatusInformation = new ConnectionStatusInformation(deviceInformation);

        }

        public DeviceInformation DeviceInformatio
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
        public string DisplayName => deviceInformation.Name;

        public bool IsConnected { get; internal set; }
        public string ConnectionStatus
        {
            get => connectionStatus;
            private set
            {
                if (connectionStatus != value)
                {
                    connectionStatus = value;
                    OnPropertyChanged(nameof(ConnectionStatus));
                }
            }
        }

        public int? BatteryLevel
        {
            get => batteryLevel;
            private set
            {
                if (batteryLevel != value)
                {
                    batteryLevel = value;
                    OnPropertyChanged(nameof(BatteryLevel));
                }
            }
        }

        public bool CanPair => deviceInformation?.Pairing.CanPair ?? false;
        public bool IsPaired => deviceInformation?.Pairing.IsPaired ?? false;
        public DeviceInformationKind Kind => deviceInformation?.Kind ?? default;
        public IReadOnlyDictionary<string, object> Properties => deviceInformation?.Properties ?? new Dictionary<string, object>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Mantido UpdateAllBatteryStatusesAsync pois atualiza o status de todos os dispositivos
        public async Task UpdateAllBatteryStatusesAsync()
        {
            if (DevicesCollection == null || DevicesCollection.Count == 0)
            {
                Console.WriteLine("A coleção de dispositivos está vazia ou não foi inicializada.");
                return;
            }

            foreach (var deviceDisplay in DevicesCollection)
            {
                if (deviceDisplay == null || deviceDisplay.DeviceInformation == null)
                {
                    Console.WriteLine("O dispositivo ou DeviceInformation é nulo.");
                    continue;
                }

                try
                {
                    await deviceDisplay.UpdateBatteryStatusAsync(deviceDisplay.DeviceInformation);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao atualizar o status da bateria: {ex.Message}");
                }
            }
        }

        public async Task UpdateBatteryStatusAsync(object device)
        {
            if (device is BluetoothLEDevice bluetoothLEDevice)
            {
                BatteryLevel = await BatteryStatus.GetBatteryLevelAsync(bluetoothLEDevice);
            }
            else if (device is BluetoothDevice bluetoothDevice)
            {
                BatteryLevel = await BatteryStatus.GetBatteryLevelAsync(bluetoothDevice);
            }

            OnPropertyChanged(nameof(BatteryLevel));
        }

        public void Update(DeviceInformationUpdate deviceInfoUpdate)
        {
            OnPropertyChanged(nameof(deviceInformation));
            OnPropertyChanged(nameof(connectionStatus));
            OnPropertyChanged(nameof(batteryLevel));
            OnPropertyChanged(nameof(Id));
            OnPropertyChanged(nameof(Name));
            OnPropertyChanged(nameof(CanPair));
            OnPropertyChanged(nameof(IsPaired));
        }

        public BitmapImage Icon
        {
            get
            {
                if (Properties.TryGetValue("System.Devices.Icon", out object iconPath))
                {
                    if (Uri.TryCreate(iconPath.ToString(), UriKind.Absolute, out Uri uriResult))
                    {
                        return new BitmapImage(uriResult);
                    }
                }
                return null;
            }
        }
    }

}

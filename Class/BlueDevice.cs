using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using BluetoothManager.Class.Command;
using Windows.System;

namespace BluetoothManager.Class
{
    public class BlueDevice : INotifyPropertyChanged
    {
        private MainWindow rootPage = MainWindow.Current;

        private ObservableCollection<BluetoothDeviceViewModel> _devices;
        public ObservableCollection<BluetoothDeviceViewModel> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                NotifyPropertyChanged();
            }
        }

        private DeviceWatcher _deviceWatcher;

        public BlueDevice()
        {
            // Inicializa a coleção de dispositivos
            Devices = new ObservableCollection<BluetoothDeviceViewModel>();

            // Configura o DeviceWatcher para monitorar mudanças nos dispositivos Bluetooth
            StartDeviceWatcher();
        }

        private void StartDeviceWatcher()
        {
            var deviceSelector = BluetoothDevice.GetDeviceSelector();
            _deviceWatcher = DeviceInformation.CreateWatcher(deviceSelector);

            _deviceWatcher.Added += DeviceWatcher_Added;
            _deviceWatcher.Updated += DeviceWatcher_Updated;
            _deviceWatcher.Removed += DeviceWatcher_Removed;

            _deviceWatcher.Start();
        }

        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            AddOrUpdateDevice(deviceInfo);
        }

        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            var device = Devices.FirstOrDefault(d => d.Id == deviceInfoUpdate.Id);
            if (device != null)
            {
                // Atualiza o status do dispositivo se ele for encontrado na lista
                UpdateDeviceStatus(device);
            }
        }

        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            var device = Devices.FirstOrDefault(d => d.Id == deviceInfoUpdate.Id);
            if (device != null)
            {
                Devices.Remove(device);
            }
        }

        public async void AddOrUpdateDevice(DeviceInformation deviceInfo)
        {
            var bluetoothDevice = await BluetoothDevice.FromIdAsync(deviceInfo.Id);
            if (bluetoothDevice != null)
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    var existingDevice = Devices.FirstOrDefault(d => d.Id == bluetoothDevice.DeviceId);
                    if (existingDevice != null)
                    {
                        // Atualizar o dispositivo existente
                        existingDevice.Name = bluetoothDevice.Name;
                        existingDevice.ConnectionStatus = bluetoothDevice.ConnectionStatus.ToString();
                    }
                    else
                    {
                        // Adicionar um novo dispositivo
                        Devices.Add(new BluetoothDeviceViewModel
                        {
                            Name = bluetoothDevice.Name,
                            Id = bluetoothDevice.DeviceId,
                            ConnectionStatus = bluetoothDevice.ConnectionStatus.ToString()
                        });
                    }
                });
            }
        }

        private async void UpdateDeviceStatus(BluetoothDeviceViewModel device)
        {
            var bluetoothDevice = await BluetoothDevice.FromIdAsync(device.Id);
            if (bluetoothDevice != null)
            {
                device.ConnectionStatus = bluetoothDevice.ConnectionStatus.ToString();
                NotifyPropertyChanged(nameof(Devices));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class BluetoothDeviceViewModel : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public string ConnectionStatus { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

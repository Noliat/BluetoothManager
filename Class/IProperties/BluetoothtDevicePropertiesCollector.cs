using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Bluetooth;
using Windows.Storage.Streams;

namespace BluetoothManager.Class.IProperties
{
    public class BluetoothDeviceInfoManager
    {
        public string DeviceName { get; private set; }
        public string DeviceId { get; private set; }
        public bool SupportsConnection { get; private set; }
        public bool SupportsBattery { get; private set; }
        public bool SupportsAudioInput { get; private set; }
        public bool SupportsAudioOutput { get; private set; }
        public bool IsJoypad { get; private set; }
        public bool IsCellPhone { get; private set; }
        public bool IsHeadset { get; private set; }
        public bool IsEarbuds { get; private set; }
        public bool IsGenericDevice { get; private set; }
        public int? BatteryLevel { get; private set; }

        private BluetoothLEDevice bluetoothLeDevice;
        private BluetoothDevice bluetoothDevice;

        // Construtor para inicializar com um dispositivo Bluetooth LE
        public BluetoothDeviceInfoManager(BluetoothLEDevice device)
        {
            DeviceName = device.Name;
            DeviceId = device.DeviceInformation.Id;
            bluetoothLeDevice = device;
            InitializeDeviceProperties();
        }

        // Construtor para inicializar com um dispositivo Bluetooth clássico
        public BluetoothDeviceInfoManager(BluetoothDevice device)
        {
            DeviceName = device.Name;
            DeviceId = device.DeviceInformation.Id;
            bluetoothDevice = device;
            InitializeDeviceProperties();
        }

        // Método para coletar e armazenar as propriedades do dispositivo
        private async void InitializeDeviceProperties()
        {
            // Verifica suporte a conexão/desconexão
            SupportsConnection = bluetoothLeDevice != null || bluetoothDevice != null;

            // Verifica se o dispositivo suporta bateria
            SupportsBattery = await SupportsBatteryAsync();

            // Verifica se o dispositivo suporta áudio de entrada/saída
            SupportsAudioInput = await SupportsAudioInputAsync();
            SupportsAudioOutput = await SupportsAudioOutputAsync();

            // Identifica o tipo de dispositivo (joypad, celular, headset, etc.)
            IdentifyDeviceType();
        }

        // Método para verificar se o dispositivo suporta bateria
        private async Task<bool> SupportsBatteryAsync()
        {
            if (bluetoothLeDevice != null)
            {
                var batteryStatus = new BluetoothBatteryStatus();
                BatteryLevel = await batteryStatus.GetBatteryLevelAsync(bluetoothLeDevice);
                return BatteryLevel.HasValue;
            }
            return false;
        }

        // Método para verificar se o dispositivo suporta áudio de entrada
        private async Task<bool> SupportsAudioInputAsync()
        {
            if (bluetoothLeDevice != null)
            {
                var services = await bluetoothLeDevice.GetGattServicesAsync();
                return services.Services.Any(service => service.Uuid == GattServiceUuids.HumanInterfaceDevice);
            }
            return false;
        }

        // Método para verificar se o dispositivo suporta áudio de saída
        private async Task<bool> SupportsAudioOutputAsync()
        {
            if (bluetoothLeDevice != null)
            {
                var services = await bluetoothLeDevice.GetGattServicesAsync();
                return services.Services.Any(service => service.Uuid == GattServiceUuids.AudioSink);
            }
            return false;
        }

        // Método para identificar o tipo de dispositivo
        private void IdentifyDeviceType()
        {
            if (DeviceName.ToLower().Contains("joypad"))
            {
                IsJoypad = true;
            }
            else if (DeviceName.ToLower().Contains("phone"))
            {
                IsCellPhone = true;
            }
            else if (DeviceName.ToLower().Contains("headset"))
            {
                IsHeadset = true;
            }
            else if (DeviceName.ToLower().Contains("earbuds"))
            {
                IsEarbuds = true;
            }
            else
            {
                IsGenericDevice = true;
            }
        }

        // Método para armazenar os GATTs dos dispositivos
        public async Task StoreGattAttributesAsync()
        {
            if (bluetoothLeDevice != null)
            {
                var gattServices = await bluetoothLeDevice.GetGattServicesAsync();
                // Armazena os serviços GATT relevantes para o dispositivo
            }
            else if (bluetoothDevice != null)
            {
                // Manipulação para dispositivos Bluetooth clássicos
            }
        }
    }

}

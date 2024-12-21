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
    public class BluetoothDevicePropertiesCollector
    {
        // Dicionário para armazenar as propriedades dos dispositivos coletadas
        private Dictionary<string, BluetoothDeviceInfo> deviceProperties = new Dictionary<string, BluetoothDeviceInfo>();

        // Coleta as propriedades do dispositivo e as armazena
        public async Task CollectDevicePropertiesAsync(BluetoothLEDevice device)
        {
            var deviceInfo = new BluetoothDeviceInfo
            {
                DeviceId = device.DeviceInformation.Id,
                Name = device.Name,
                IsConnected = device.ConnectionStatus == BluetoothConnectionStatus.Connected,
                IsPaired = device.DeviceInformation.Pairing.IsPaired,
            };

            // Verifica se o dispositivo suporta serviços GATT
            var gattServicesResult = await device.GetGattServicesAsync();
            if (gattServicesResult.Status == GattCommunicationStatus.Success)
            {
                foreach (var service in gattServicesResult.Services)
                {
                    // Verifica suporte a bateria
                    if (service.Uuid == GattServiceUuids.Battery)
                    {
                        deviceInfo.HasBattery = true;
                        deviceInfo.BatteryLevel = await GetBatteryLevelAsync(service);
                    }

                    // Verifica suporte a áudio (Entrada e/ou Saída)
                    if (service.Uuid == GattServiceUuids.AudioSource)
                    {
                        deviceInfo.SupportsAudioOutput = true;
                    }
                    if (service.Uuid == GattServiceUuids.AudioSink)
                    {
                        deviceInfo.SupportsAudioInput = true;
                    }
                }
            }

            // Determina o tipo do dispositivo com base em seu nome ou características
            deviceInfo.DeviceType = DetermineDeviceType(device.Name);

            // Armazena as propriedades coletadas
            deviceProperties[device.DeviceInformation.Id] = deviceInfo;
        }

        // Método para determinar o tipo do dispositivo baseado no nome
        private DeviceType DetermineDeviceType(string deviceName)
        {
            if (deviceName.ToLower().Contains("headset") || deviceName.ToLower().Contains("headphones"))
                return DeviceType.Headset;
            else if (deviceName.ToLower().Contains("joypad") || deviceName.ToLower().Contains("controller"))
                return DeviceType.Joypad;
            else if (deviceName.ToLower().Contains("phone") || deviceName.ToLower().Contains("cell"))
                return DeviceType.CellPhone;
            else
                return DeviceType.Generic;
        }

        // Método para obter o nível da bateria
        private async Task<int?> GetBatteryLevelAsync(GattDeviceService batteryService)
        {
            var batteryLevelCharacteristic = await batteryService.GetCharacteristicsForUuidAsync(GattCharacteristicUuids.BatteryLevel);
            if (batteryLevelCharacteristic.Status == GattCommunicationStatus.Success)
            {
                var characteristic = batteryLevelCharacteristic.Characteristics.FirstOrDefault();
                if (characteristic != null)
                {
                    var result = await characteristic.ReadValueAsync();
                    if (result.Status == GattCommunicationStatus.Success)
                    {
                        var reader = DataReader.FromBuffer(result.Value);
                        return (int?)reader.ReadByte(); // Valor da bateria entre 0-100
                    }
                }
            }
            return null;
        }

        // Método para verificar se um dispositivo suporta conectar/desconectar
        public bool CanConnectOrDisconnect(string deviceId)
        {
            if (deviceProperties.ContainsKey(deviceId))
            {
                return deviceProperties[deviceId].IsPaired;
            }
            return false;
        }

        // Método para verificar se um dispositivo possui suporte a bateria
        public bool HasBatterySupport(string deviceId)
        {
            if (deviceProperties.ContainsKey(deviceId))
            {
                return deviceProperties[deviceId].HasBattery;
            }
            return false;
        }

        // Método para obter informações completas de um dispositivo
        public BluetoothDeviceInfo GetDeviceInfo(string deviceId)
        {
            if (deviceProperties.ContainsKey(deviceId))
            {
                return deviceProperties[deviceId];
            }
            return null;
        }
    }

    // Classe que armazena as informações coletadas de um dispositivo Bluetooth
    public class BluetoothDeviceInfo
    {
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public bool IsConnected { get; set; }
        public bool IsPaired { get; set; }
        public bool HasBattery { get; set; }
        public int? BatteryLevel { get; set; }
        public bool SupportsAudioInput { get; set; }
        public bool SupportsAudioOutput { get; set; }
        public DeviceType DeviceType { get; set; }
    }

    // Enum para identificar tipos de dispositivos Bluetooth
    public enum DeviceType
    {
        Headset,
        Joypad,
        CellPhone,
        Generic
    }

}

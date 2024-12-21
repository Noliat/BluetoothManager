using Windows.Devices.Bluetooth;
using Windows.Devices.Power;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using System.Linq;

namespace BluetoothManager.Class
{
    public class BluetoothBatteryStatus
    {
        private Battery battery;
        private bool isBatterySupported = true; // Fallback para indicar se a bateria é suportada

        // Obtém o nível de bateria de um dispositivo BluetoothLE
        public async Task<int?> GetBatteryLevelAsync(BluetoothLEDevice device)
        {
            try
            {
                // Obtém o dispositivo BluetoothLE
                BluetoothLEDevice bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(device.DeviceInformation.Id);
                if (bluetoothLeDevice == null)
                {
                    isBatterySupported = false; // Se o dispositivo for nulo, não é suportado
                    return null;
                }

                // Obtém o serviço de bateria
                var batteryServiceResult = await bluetoothLeDevice.GetGattServicesForUuidAsync(GattServiceUuids.Battery);
                if (batteryServiceResult.Status == GattCommunicationStatus.Success)
                {
                    // O dispositivo suporta o serviço de bateria, agora obtenha as características
                    var batteryService = batteryServiceResult.Services.FirstOrDefault();
                    if (batteryService != null)
                    {
                        // Obtém a característica que contém o nível de bateria
                        var batteryLevelCharacteristicResult = await batteryService.GetCharacteristicsForUuidAsync(GattCharacteristicUuids.BatteryLevel);
                        if (batteryLevelCharacteristicResult.Status == GattCommunicationStatus.Success)
                        {
                            var batteryLevelCharacteristic = batteryLevelCharacteristicResult.Characteristics.FirstOrDefault();
                            if (batteryLevelCharacteristic != null)
                            {
                                // Lê o valor da característica do nível de bateria
                                var batteryLevelValue = await batteryLevelCharacteristic.ReadValueAsync();
                                if (batteryLevelValue.Status == GattCommunicationStatus.Success)
                                {
                                    var reader = DataReader.FromBuffer(batteryLevelValue.Value);
                                    byte batteryLevel = reader.ReadByte(); // Lê o nível de bateria como um valor de 0-100%
                                    return (int?)batteryLevel;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter status da bateria do dispositivo BluetoothLE: {ex.Message}");
            }

            // Caso o serviço ou característica de bateria não esteja disponível
            isBatterySupported = false;
            return null;
        }

        // Obtém o nível de bateria de um dispositivo Bluetooth clássico
        public async Task<int?> GetBatteryLevelAsync(BluetoothDevice device)
        {
            return await GetBatteryLevelInternalAsync(device.DeviceInformation.Id);
        }

        // Método interno para obter o nível de bateria
        private async Task<int?> GetBatteryLevelInternalAsync(string deviceId)
        {
            try
            {
                battery = await Battery.FromIdAsync(deviceId);
                if (battery == null)
                {
                    isBatterySupported = false; // Dispositivo não suporta relatórios de bateria
                    return null;
                }

                var batteryReport = battery.GetReport();
                if (batteryReport != null)
                {
                    var chargeLevel = batteryReport.RemainingCapacityInMilliwattHours;
                    var fullCapacity = batteryReport.FullChargeCapacityInMilliwattHours;

                    if (chargeLevel.HasValue && fullCapacity.HasValue && fullCapacity.Value > 0)
                    {
                        return (int)((chargeLevel.Value / (double)fullCapacity.Value) * 100); // Calcula o nível da bateria em %
                    }
                }

                isBatterySupported = false; // Se o relatório estiver vazio ou inválido, o dispositivo pode não suportar bateria
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter status da bateria: {ex.Message}");
                isBatterySupported = false;
            }

            return null;
        }

        // Monitoramento contínuo do status da bateria
        public async Task MonitorBatteryStatusAsync(Action<int?> onBatteryLevelChanged)
        {
            if (battery == null)
            {
                isBatterySupported = false;
                onBatteryLevelChanged(null);
                return;
            }

            // Simula uma verificação contínua do nível de bateria
            while (isBatterySupported)
            {
                var batteryLevel = await GetBatteryLevelInternalAsync(battery.DeviceId);

                // Atualiza a interface sempre que o nível de bateria mudar
                onBatteryLevelChanged(batteryLevel);

                // Aguarda um tempo antes de verificar novamente
                await Task.Delay(60000); // 1 minuto de intervalo entre verificações
            }
        }

        // Fallback para dispositivos sem suporte a relatórios de bateria
        public string GetFallbackMessage()
        {
            return isBatterySupported ? string.Empty : "Nível de bateria indisponível";
        }
    }
}

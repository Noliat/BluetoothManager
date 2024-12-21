using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Bluetooth;
using Windows.Devices.Radios;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;
using Windows.UI.Notifications;

namespace BluetoothManager.Class.BackgroundTask
{
    public sealed class BluetoothBackgroundTask : IBackgroundTask
    {
        private BackgroundTaskDeferral _deferral;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Solicita um deferral para manter o Background Task ativo
            _deferral = taskInstance.GetDeferral();

            // Insere a lógica para conectar automaticamente e monitorar dispositivos
            MonitorAndReconnectToBluetoothDevices();

            // Completa o deferral após a tarefa estar concluída
            _deferral.Complete();
        }

        private async void MonitorAndReconnectToBluetoothDevices()
        {
            // Exemplo de lógica para monitorar dispositivos emparelhados
            var selector = BluetoothLEDevice.GetDeviceSelector();
            var devices = await DeviceInformation.FindAllAsync(selector);

            foreach (var deviceInfo in devices)
            {
                var bluetoothDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);
                if (bluetoothDevice != null)
                {
                    // Verifica se o dispositivo está conectado
                    if (bluetoothDevice.ConnectionStatus == BluetoothConnectionStatus.Disconnected)
                    {
                        // Tenta reconectar
                        await bluetoothDevice.DeviceInformation.Pairing.PairAsync();

                        // Envia uma notificação ao usuário
                        SendNotification("Bluetooth reconectado", $"{bluetoothDevice.Name} foi reconectado automaticamente.");
                    }

                    // Monitoramento da bateria
                    var gattServicesResult = await bluetoothDevice.GetGattServicesAsync();
                    if (gattServicesResult.Status == GattCommunicationStatus.Success)
                    {
                        foreach (var service in gattServicesResult.Services)
                        {
                            if (service.Uuid == GattServiceUuids.Battery)
                            {
                                var batteryLevel = await GetBatteryLevelAsync(service);
                                SendNotification("Nível de Bateria", $"Bateria de {bluetoothDevice.Name}: {batteryLevel}%");
                            }
                        }
                    }
                }
            }
        }

        private async Task<int> GetBatteryLevelAsync(GattDeviceService batteryService)
        {
            // Obtendo as características de forma assíncrona
            var characteristics = await batteryService.GetCharacteristicsForUuidAsync(GattCharacteristicUuids.BatteryLevel);

            // Verifica se há características e lê a primeira delas
            if (characteristics.Status == GattCommunicationStatus.Success && characteristics.Characteristics.Count > 0)
            {
                var characteristic = characteristics.Characteristics[0];
                var result = await characteristic.ReadValueAsync();

                if (result.Status == GattCommunicationStatus.Success)
                {
                    var reader = DataReader.FromBuffer(result.Value);
                    return reader.ReadByte();
                }
            }

            return -1; // Retorna -1 se a leitura falhar
        }

        private void SendNotification(string title, string message)
        {
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            var toastTextElements = toastXml.GetElementsByTagName("text");
            toastTextElements[0].AppendChild(toastXml.CreateTextNode(title));
            toastTextElements[1].AppendChild(toastXml.CreateTextNode(message));

            var toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}

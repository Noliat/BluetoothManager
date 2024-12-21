using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BluetoothManager.Class;
using Windows.Devices.Enumeration;

namespace BluetoothManager.Class
{
    public class PairButton
    {
        private ConnectionStatusInformation ConnectionStatusInformation { get; set; }
        private MainWindow MainWindow { get; set; }

        public async Task PairButton_Click(DeviceInformationDisplay selectedDeviceInfoDisp)
        {
            // Gray out the pair button and results view while pairing is in progress.
            ConnectionStatusInformation.UpdateConnectionStatus();

            // Verifique se o dispositivo pode ser pareado
            if (selectedDeviceInfoDisp.DeviceInformation.Pairing.CanPair)
            {
                DeviceInformationDisplay deviceInfoDisp = MainWindow.Devices.SelectedItem as DeviceInformationDisplay;

                DevicePairingResult dpr = await deviceInfoDisp.DeviceInformation.Pairing.PairAsync();

                // Atualiza o status de conexão após o pareamento
                ConnectionStatusInformation.UpdateConnectionStatus();

                // Atualiza os botões de pareamento
                UpdatePairingButtons();
            }
            else
            {
                MessageBox.Show("O dispositivo não pode ser pareado.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        

        public void UpdatePairingButtons()
        {
            DeviceInformationDisplay selectedDeviceInfoDisp = (DeviceInformationDisplay)MainWindow.Devices.SelectedItem;

            if (selectedDeviceInfoDisp != null)
            {
                // Aqui você pode atualizar o conteúdo do botão de "Conectar" para "Desconectar" e vice-versa, dependendo do status do dispositivo
                var pairingInfo = selectedDeviceInfoDisp.DeviceInformation.Pairing;

                if (pairingInfo.IsPaired)
                {
                    // Atualiza o botão para "Desconectar" se o dispositivo estiver emparelhado
                }
                else
                {
                    // Atualiza o botão para "Conectar" se o dispositivo não estiver emparelhado
                }
            }
        }
    }
}

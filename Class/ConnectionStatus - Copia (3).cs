using System;
using System.ComponentModel;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace BluetoothManager.Class
{
    public class ConnectionStatusInformation : INotifyPropertyChanged
    {
        private DeviceInformation deviceInformation;
        private BluetoothConnectionStatus bluetoothConnectionStatus;

        public ConnectionStatusInformation(DeviceInformation deviceInfo)
        {
            deviceInformation = deviceInfo;
            UpdateConnectionStatus();
        }

        public DeviceInformation DeviceInformation
        {
            get => deviceInformation;
            private set
            {
                if (deviceInformation != value)
                {
                    deviceInformation = value;
                    OnPropertyChanged(nameof(DeviceInformation));
                    UpdateConnectionStatus();
                }
            }
        }

        // Propriedades para emparelhamento
        public bool CanPair => deviceInformation?.Pairing.CanPair ?? false;
        public bool IsPaired => deviceInformation?.Pairing.IsPaired ?? false;

        // Propriedade para o status de conexão
        private string connectionStatus;
        public string ConnectionStatus
        {
            get => connectionStatus;
            set
            {
                if (connectionStatus != value)
                {
                    connectionStatus = value;
                    OnPropertyChanged(nameof(ConnectionStatus));
                }
            }
        }

        // Propriedade IsConnected
        private bool isConnected;
        public bool IsConnected
        {
            get => isConnected;
            private set
            {
                if (isConnected != value)
                {
                    isConnected = value;
                    OnPropertyChanged(nameof(IsConnected));
                }
            }
        }

        // Método para atualizar o ConnectionStatus e IsConnected
        public void UpdateConnectionStatus()
        {
            if (IsPaired)
            {
                // Verifica o status de conexão Bluetooth
                if (bluetoothConnectionStatus == BluetoothConnectionStatus.Connected)
                {
                    IsConnected = true; // Dispositivo está conectado

                    // Verifica se está emparelhado e o tipo de dispositivo (entrada, saída ou ambos)
                    if (IsAudioDevice(out string audioType))
                    {
                        ConnectionStatus = $"Paired & Connected - {audioType}";
                    }
                    else
                    {
                        ConnectionStatus = "Paired & Connected - Non-audio device";
                    }
                }
                else
                {
                    IsConnected = false; // Dispositivo emparelhado mas não conectado
                    ConnectionStatus = "Paired but Disconnected";
                }
            }
            else if (CanPair)
            {
                IsConnected = false;
                ConnectionStatus = "Unpaired";
            }
            else
            {
                IsConnected = false;
                ConnectionStatus = "Disconnected";
            }
        }

        // Método para verificar o tipo de dispositivo (entrada, saída ou ambos)
        public bool IsAudioDevice(out string audioType)
        {
            audioType = "Unknown";

            if (deviceInformation.Properties.TryGetValue("System.Devices.AudioDevice.Role", out object role))
            {
                // Verifica se o dispositivo é de entrada, saída ou ambos
                switch (role.ToString())
                {
                    case "Multimedia":
                        audioType = "Audio Output (Speaker)";
                        return true;
                    case "Communications":
                        audioType = "Audio Input (Microphone)";
                        return true;
                    case "Multimedia, Communications":
                        audioType = "Audio Input/Output (Speaker and Microphone)";
                        return true;
                }
            }

            return false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

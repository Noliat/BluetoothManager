using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.UI.Xaml.Media;

namespace BluetoothManager.Class
{
    public class ConnectionStatusInformation : INotifyPropertyChanged
    {
        private BluetoothConnectionStatus bluetoothConnectionStatus;
        private DeviceInformation deviceInfo;

        public DeviceInformation DeviceInformation { get; private set; }

        // Construtor com inicialização completa
        public ConnectionStatusInformation(DeviceInformation deviceInfo, BluetoothConnectionStatus blueInfo)
        {
            this.deviceInfo = deviceInfo ?? throw new ArgumentNullException(nameof(deviceInfo));
            bluetoothConnectionStatus = blueInfo;
            this.UpdateConnectionStatus();
        }   

        // Construtor que inicializa apenas o DeviceInformation
        public ConnectionStatusInformation(DeviceInformation deviceInformation)
        {
            this.deviceInfo = deviceInformation ?? throw new ArgumentNullException(nameof(deviceInformation));
            bluetoothConnectionStatus = BluetoothConnectionStatus.Disconnected; // Define um status padrão
            this.UpdateConnectionStatus();
        }

        // Propriedade para o BluetoothConnectionStatus
        public BluetoothConnectionStatus BluetoothConnection
        {
            get => bluetoothConnectionStatus;
            private set
            {
                if (bluetoothConnectionStatus != value)
                {
                    bluetoothConnectionStatus = value;
                    this.OnPropertyChanged(nameof(BluetoothConnection));
                    this.UpdateConnectionStatus();
                }
            }
        }

        // Propriedades de emparelhamento usando DeviceInformation.Pairing
        public bool CanPair => deviceInfo.Pairing.CanPair;
        public bool IsPaired => deviceInfo.Pairing.IsPaired;

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
                    this.OnPropertyChanged(nameof(ConnectionStatus));
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
                    this.OnPropertyChanged(nameof(IsConnected));
                }
            }
        }

        public bool GetIsPaired()
        {
            return IsPaired;
        }

        public bool GetCanPair()
        {
            return CanPair;
        }

        public MainWindow MainWindow { get; private set; }
        public ObservableCollection<DeviceInformationDisplay> DevicesCollection { get; private set; }
        // Método para atualizar o ConnectionStatus e IsConnected
        public void UpdateConnectionStatus()
        {
            if ( null == deviceInfo)
            {
                ConnectionStatus = "Dispositivo não disponível";
                IsConnected = false;  // Dispositivo não disponível não pode estar conectado
                return;
            }

            // Verifica se o dispositivo está emparelhado
            // Verifica se o dispositivo está emparelhado
            else if (IsConnected)
            {
                // Verifica o status de conexão Bluetooth
                if (bluetoothConnectionStatus == BluetoothConnectionStatus.Connected)
                {
                    IsConnected = true; // Dispositivo está conectado
                    
                    // Verifica se está emparelhado e o tipo de dispositivo (entrada, saída ou ambos)
                    if (this.IsAudioDevice(out string audioType))
                    {
                        ConnectionStatus = $"{audioType}";
                    };
                }
            }
            if (IsPaired)
            {
                // Se o dispositivo pode emparelhar, mas não está emparelhado, deixa o status vazio ou um status específico para isso
                IsConnected = false;
                
                ConnectionStatus = "Emparelhado"; // Aqui você pode usar uma string mais apropriada
            }
            else if(CanPair)
            {
                IsConnected = false; // Dispositivo não está emparelhado e não pode emparelhar
                
                ConnectionStatus = "Não emparelhado";
            }
        }


        // Método para verificar o tipo de dispositivo (entrada, saída ou ambos)
        public bool IsAudioDevice(out string audioType)
        {
            audioType = string.Empty;

            // Verifica se a propriedade "System.Devices.AudioDevice.Role" está presente
            if (deviceInfo.Properties.TryGetValue("System.Devices.AudioDevice.Role", out object role))
            {
                string roleValue = role.ToString().ToLower(); // Converte para minúsculas para verificação case-insensitive

                // Faz uma verificação nos valores retornados
                switch (roleValue)
                {
                    case "multimedia":
                        audioType = "Música, conectada";
                        return true;
                    case "communications":
                        audioType = "Voz conectada";
                        return true;
                    case "multimedia, communications":
                    case "communications, multimedia": // Para garantir ambas combinações
                        audioType = "Voz e música, conectadas";
                        return true;
                    case "voice":
                        audioType = "Somente voz, conectada";
                        return true;
                    case "audio":
                        audioType = "Somente áudio, conectada";
                        return true;
                    case "multimedia, voice":
                        audioType = "Música e voz, conectadas";
                        return true;
                    default:
                        audioType = $"Emparelhado - {roleValue}"; // Se for algo diferente ou desconhecido
                        return false;
                }
            }

            // Se a propriedade não estiver presente, retorna falso
            return false;
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;
using Windows.Devices.Bluetooth;
using BluetoothManager.Class.IConverter;
using Windows.Devices.Enumeration;

namespace BluetoothManager.Class.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {

        // Construtor do ViewModel
        public MainViewModel()
        {
            BlueSettings = new BlueSettings();
            DevicesCollection = new ObservableCollection<DeviceInformationDisplay>();

            // Inicializa DeviceWatcherHelper passando o Dispatcher atual
            DeviceWatcherHelper = new DeviceWatcherHelper(DevicesCollection, Dispatcher.CurrentDispatcher);

            // Inicializa o conversor de visibilidade
            BooleanToVisibilityConverter = new BooleanToVisibilityConverter();

            // Lógica de inicialização do ConnectionStatusInformation
            InitializeConnectionStatus();

            // Instancia a classe de status de bateria
            _bluetoothBatteryStatus = new BluetoothBatteryStatus();

            // Instanciando o GridMinHeight
            _gridMinHeightConverter = new GridMinHeight();

            // Exemplo: Definindo um valor inicial para a altura da Grid
            GridHeight = 520; // ou qualquer valor de inicialização
        }

        // Evento necessário para a implementação de INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        // Propriedades do ViewModel
        private BlueSettings blueSettings;
        public BlueSettings BlueSettings
        {
            get { return blueSettings; }
            set
            {
                if (blueSettings != value)
                {
                    blueSettings = value;
                    OnPropertyChanged(nameof(BlueSettings));
                }
            }
        }


        private BluetoothBatteryStatus _batteryStatus;
        private BluetoothBatteryStatus _bluetoothBatteryStatus;
        private int _batteryLevel;
        public int BatteryLevel
        {
            get => _batteryLevel;
            set
            {
                _batteryLevel = value;
                OnPropertyChanged(nameof(BatteryLevel)); // Notifica a UI da mudança
            }
        }

        private bool _isRadioOn;

        public bool IsRadioOn
        {
            get { return _isRadioOn; }
            set
            {
                _isRadioOn = value;
                OnPropertyChanged(nameof(IsRadioOn));
            }
        }

        private ObservableCollection<DeviceInformationDisplay> devicesCollection { get; set; }
        public ObservableCollection<DeviceInformationDisplay> DevicesCollection
        {
            get { return devicesCollection; }
            set
            {
                if (devicesCollection != value)
                {
                    devicesCollection = value;
                    OnPropertyChanged(nameof(DevicesCollection));
                }
            }
        }

        private ConnectionStatusInformation connectionStatusInformation;
        public ConnectionStatusInformation ConnectionStatusInformation
        {
            get { return connectionStatusInformation; }
            set
            {
                if (connectionStatusInformation != value)
                {
                    connectionStatusInformation = value;
                    OnPropertyChanged(nameof(ConnectionStatusInformation));
                }
            }
        }

        private GridMinHeight _gridMinHeightConverter;
        private int _gridHeight;
        public int GridHeight
        {
            get { return _gridHeight; }
            set
            {
                _gridHeight = value;
                OnPropertyChanged(nameof(GridHeight));

                // Atualizando a altura mínima com base na conversão
                MinGridHeight = (double)_gridMinHeightConverter.Convert(_gridHeight, typeof(double), null, CultureInfo.InvariantCulture);
            }
        }

        private double _minGridHeight;
        public double MinGridHeight
        {
            get { return _minGridHeight; }
            set
            {
                _minGridHeight = value;
                OnPropertyChanged(nameof(MinGridHeight));
            }
        }

        private DeviceInformationDisplay deviceInfoDisp;

        public async Task OnDeviceConnectedAsync(BluetoothDevice device)
        {
            if (deviceInfoDisp != null)
            {
                await deviceInfoDisp.UpdateBatteryStatusAsync(device);
            }

            var batteryStatus = new BluetoothBatteryStatus();

            // Começa a monitorar o nível de bateria e atualiza a interface
            await batteryStatus.MonitorBatteryStatusAsync(batteryLevel =>
            {
                if (batteryLevel.HasValue)
                {
                    // Atualiza a interface com o nível de bateria
                    Console.WriteLine($"Nível de bateria: {batteryLevel}%");
                }
                else
                {
                    // Fallback para dispositivos sem suporte
                    Console.WriteLine(batteryStatus.GetFallbackMessage());
                }
            });
        }

        public BooleanToVisibilityConverter BooleanToVisibilityConverter { get; set; }

        private DeviceWatcherHelper DeviceWatcherHelper { get; set; }

        // Método para notificar mudanças de propriedades
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Método para inicializar o ConnectionStatusInformation com o primeiro dispositivo da coleção
        private void InitializeConnectionStatus()
        {
            if (DevicesCollection.Any())
            {
                var firstDevice = DevicesCollection.First();
                // Verifica se o dispositivo está disponível e cria ConnectionStatusInformation
                if (firstDevice != null && firstDevice.DeviceInformation != null)
                {
                    ConnectionStatusInformation = new ConnectionStatusInformation(firstDevice.DeviceInformation, BluetoothConnectionStatus.Disconnected);
                    OnPropertyChanged(nameof(ConnectionStatusInformation));
                }
            }
        }

        // Método chamado quando um dispositivo é selecionado
        public async void OnDeviceSelected(DeviceInformationDisplay deviceIdoDisp, BluetoothConnectionStatus connectionStatus)
        {
            if (deviceIdoDisp != null && deviceIdoDisp.DeviceInformation != null)
            {
                // Atualiza ConnectionStatusInformation com o dispositivo selecionado e seu status de conexão
                ConnectionStatusInformation = new ConnectionStatusInformation(deviceIdoDisp.DeviceInformation, connectionStatus);
                OnPropertyChanged(nameof(ConnectionStatusInformation));

                // Atualiza o status da bateria do dispositivo selecionado
                await UpdateBatteryStatusAsync(deviceIdoDisp);
            }
        }

        // Método para atualizar o nível de bateria do dispositivo selecionado
        public async Task UpdateBatteryStatusAsync(DeviceInformationDisplay deviceIdoDisp)
        {
            if (deviceIdoDisp != null)
            {
                // Verifica se o dispositivo é LE (Bluetooth Low Energy)
                if (deviceIdoDisp.Properties["System.Devices.Aep.Bluetooth.Le.IsConnectable"] is bool isBluetoothLE && isBluetoothLE)
                {
                    var bluetoothLEDevice = await BluetoothLEDevice.FromIdAsync(deviceIdoDisp.Id);
                    if (bluetoothLEDevice != null)
                    {
                        await deviceIdoDisp.UpdateBatteryStatusAsync(bluetoothLEDevice);
                    }
                }
                else
                {
                    // Caso seja um dispositivo Bluetooth clássico
                    var bluetoothDevice = await BluetoothDevice.FromIdAsync(deviceIdoDisp.Id);
                    if (bluetoothDevice != null)
                    {
                        await deviceIdoDisp.UpdateBatteryStatusAsync(bluetoothDevice);
                    }
                }
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using BluetoothManager.Class;
using BluetoothManager.Class.ViewModel;
using Hardcodet.Wpf.TaskbarNotification;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.Devices.Radios;


namespace BluetoothManager
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
        private RadioModel radioModel;
        private PosWindow ViewWindow;
        public static MainWindow Current;

        private DeviceWatcherHelper deviceWatcherHelper;
        private ObservableCollection<DeviceInformationDisplay> devicesCollection = new ObservableCollection<DeviceInformationDisplay>();


        public MainWindow()
        {
            InitializeComponent();
            ShowInTaskbar = false;

            this.DataContext = new MainViewModel();

            InitializeAction();
            Loaded += RequestAccess;
            Loaded += ListView_Loaded;

            Deactivated += MainWindow_Deactivated;
            ViewWindow = new PosWindow(this);

            // Inicia oculta
            Visibility = Visibility.Hidden;

            // Aplica as cores do sistema à janela principal
            //SystemThemeHelper.ApplySystemThemeColors(this);

            Current = this;

            // Inicialize o DeviceWatcherHelper
            deviceWatcherHelper = new DeviceWatcherHelper(new ObservableCollection<DeviceInformationDisplay>(), Dispatcher);

            deviceWatcherHelper = new DeviceWatcherHelper(devicesCollection, Dispatcher);
            deviceWatcherHelper.DeviceChanged += OnDeviceListChanged;

            _Expander = new ListViewExpander();
            Devices.SelectionChanged += _Expander.Devices_SelectionChanged;

            // Vincule a coleção de dispositivos ao ListView
            Devices.ItemsSource = deviceWatcherHelper.devicesCollection;
        }

        public void InitializeAction()
        {
            ViewModel = new BlueSettings();

            // Registrar eventos de janela para Show e Hide
            this.IsVisibleChanged += MainWindow_IsVisibleChanged;

            EnableAcrylic();
        }

        private void EnableAcrylic()
        {
            // Obter a cor do Background do Grid
            var gridBackground = ((SolidColorBrush)Grd.Background).Color;

            // Converter para formato ARGB
            int gradientColor = (gridBackground.A) |
                                (gridBackground.R) |
                                (gridBackground.G) |
                                (gridBackground.B);

            // Aplicar o efeito acrílico com a cor do Background
            AcrylicHelper.EnableAcrylic(this, gradientColor);
        }

        private void StartWatcher()
        {
            // Limpar a coleção de dispositivos antes de iniciar o watcher
            devicesCollection.Clear();

            // Seletor atualizado para excluir BLE, mantendo apenas dispositivos Bluetooth clássicos
            string bluetoothSelector = "(System.Devices.Aep.ProtocolId:={e0cbf06c-cd8b-4647-bb8a-263b43f0f974} OR " +
                                       "System.Devices.Aep.ProtocolId:={bb7bb05e-5972-42b5-94fc-76eaa7084d49}) " +
                                       "AND (System.Devices.Aep.CanPair:=System.StructuredQueryType.Boolean#True " +
                                       "OR System.Devices.Aep.IsPaired:=System.StructuredQueryType.Boolean#True)";

            // Criar o DeviceWatcher usando o seletor atualizado
            DeviceWatcher deviceWatcher = DeviceInformation.CreateWatcher(
                bluetoothSelector,
                null, // Não precisamos de propriedades adicionais neste exemplo
                DeviceInformationKind.AssociationEndpoint // Tipo específico para Bluetooth
            );

            // Iniciar o watcher para observar apenas dispositivos Bluetooth (sem BLE)
            deviceWatcherHelper.StartWatcher(deviceWatcher);
        }

        private void OnDeviceListChanged(DeviceWatcher sender, string id)
        {
            // If the item being updated is currently "selected", then update the pairing buttons
            DeviceInformationDisplay selectedDeviceInfoDisp = (DeviceInformationDisplay)Devices.SelectedItem;
            if ((selectedDeviceInfoDisp != null) && (selectedDeviceInfoDisp.Id == id))
            {
                UpdatePairingButtons();
            }
        }


        // Este método será chamado quando a visibilidade da janela mudar
        private void MainWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                // Quando a janela for exibida, inicia o DeviceWatcher
                StartWatcher();
                this.Show();
            }
            else
            {
                // Quando a janela for ocultada, para o DeviceWatcher e limpa a lista
                StopAndCleanDeviceWatcher();
                this.Hide();
            }

        }

        private void StartDeviceWatcher()
        {
            // Crie um DeviceWatcher para Bluetooth (ou outro seletor que deseje)
            // Usa o seletor padrão para Bluetooth
            DeviceWatcher deviceWatcher = DeviceInformation.CreateWatcher();
            deviceWatcherHelper.StartWatcher(deviceWatcher);
        }

        private void StopAndCleanDeviceWatcher()
        {
            // Pare o DeviceWatcher
            deviceWatcherHelper.StopWatcher();

            // Limpe a lista, mantendo apenas dispositivos pareados

            deviceWatcherHelper.devicesCollection.Clear();

        }

        // Exemplo de uso no evento Loaded
        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            var _Expander = new ListViewExpander();
            _Expander.CollapseAllItems(Devices); // MyListView é o nome da sua ListView
        }

        private ListViewExpander _Expander { get; set; }
        private void Devices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _Expander.Devices_SelectionChanged(sender, e);
        }


        private void DiscButton_Click(object sender, RoutedEventArgs e)
        {
            DisconnectButton();
        }

        private async void PairButton(object sender, MouseButtonEventArgs e)
        {
            // Verifique se o StackPanel é o sender e obtenha o DataContext associado
            StackPanel stackPanel = sender as StackPanel;
            if (stackPanel == null) return;

            // Obtenha o dispositivo diretamente do DataContext do StackPanel clicado

            if (stackPanel.DataContext is DeviceInformationDisplay deviceInfoDisp)
            {
                Devices.IsEnabled = false;

                // Verifique se o dispositivo pode ser emparelhado e inicie o emparelhamento
                if (deviceInfoDisp.DeviceInformation.Pairing.CanPair && !deviceInfoDisp.DeviceInformation.Pairing.IsPaired)
                {
                    DevicePairingResult pairingResult = await deviceInfoDisp.DeviceInformation.Pairing.PairAsync();
                    if (pairingResult.Status == DevicePairingResultStatus.Paired)
                    {
                        // Emparelhamento bem-sucedido
                        MessageBox.Show($"{deviceInfoDisp.Name} emparelhado com sucesso.");
                    }
                    else
                    {
                        // O emparelhamento falhou
                        MessageBox.Show($"Falha ao emparelhar {deviceInfoDisp.Name}.");
                    }
                }

                Devices.IsEnabled = true;
            }
        }

        private void DisconnectDevice(DeviceInformationDisplay deviceInfoDisp)
        {
            // Aqui você pode usar uma propriedade no seu ViewModel ou uma lista para controlar o estado de conexão
            // Exemplo: 
            deviceInfoDisp.IsConnected = false; // Adicione essa propriedade na sua classe DeviceInformationDisplay

            // Atualizar a interface do usuário
            UpdatePairingButtons();
        }

        private void DisconnectButton()
        {

            if (DataContext is DeviceInformationDisplay deviceInfoDisp && deviceInfoDisp.IsConnected)
            {
                Devices.IsEnabled = false;

                // Execute a lógica de desconexão
                DisconnectDevice(deviceInfoDisp);

                MessageBox.Show($"{deviceInfoDisp.Name} desconectado.");
                Devices.IsEnabled = true;
            }
        }


        private async void UnPairButton(object sender, MouseButtonEventArgs e)
        {
            Devices.IsEnabled = false;
            DeviceInformationDisplay DeviceInfoDisp = Devices.SelectedItem as DeviceInformationDisplay;

            DeviceUnpairingResult dupr = await DeviceInfoDisp.DeviceInformation.Pairing.UnpairAsync();

            UpdatePairingButtons();
            Devices.IsEnabled = true;
        }

        private void UpdatePairingButtons()
        {
            DeviceInformationDisplay deviceInfoDisp = (DeviceInformationDisplay)Devices.SelectedItem;

            if (deviceInfoDisp != null)
            {
                if (deviceInfoDisp.DeviceInformation.Pairing.CanPair && !deviceInfoDisp.DeviceInformation.Pairing.IsPaired)
                {
                    // Atualizar botão para mostrar que pode emparelhar
                    IsEnabled = true;
                }
                else
                {
                    // Atualizar botão para mostrar que não pode emparelhar
                    IsEnabled = false;
                }

                if (deviceInfoDisp.DeviceInformation.Pairing.IsPaired)
                {
                    // Atualizar botão para mostrar que já está emparelhado
                    IsEnabled = true;
                }
                else
                {
                    // Desabilitar botão se não puder emparelhar ou já estiver emparelhado
                    IsEnabled = false;
                }
            }
        }


        private BlueSettings ViewModel;
        private void TextBlock_Tapped(object sender, MouseButtonEventArgs e)
        {
            ViewModel.OpenBluetoothSettingsCommand.Execute(null);
        }

        private async void Switch_Click(object sender, RoutedEventArgs e)
        {
            // Assumindo que você tem uma instância de RadioModel na DataContext
            var radioModel = DataContext as RadioModel;
            if (radioModel != null)
            {
                var newState = radioModel.IsRadioOn ? RadioState.Off : RadioState.On;
                await radioModel.SetRadioStateAsync(newState);
            }
        }

        private async void RequestAccess(object sender, RoutedEventArgs e)
        {
            // Solicita permissão para acessar os rádios do dispositivo.
            var accessLevel = await Radio.RequestAccessAsync();
            if (accessLevel != RadioAccessStatus.Allowed)
            {
                MessageBox.Show("O aplicativo não tem permissão para controlar rádios.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                // Obtém a lista de rádios e vincula o primeiro rádio encontrado ao botão Switch.
                var radios = await Radio.GetRadiosAsync();
                var bluetoothRadio = radios.FirstOrDefault(r => r.Kind == RadioKind.Bluetooth);

                if (bluetoothRadio != null)
                {
                    // Configura o DataContext do botão Switch para o modelo de rádio encontrado.
                    Switch.DataContext = new RadioModel(bluetoothRadio);
                }
                else
                {
                    MessageBox.Show("Nenhum rádio Bluetooth foi encontrado.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Manipulador do evento Deactivated
        private void MainWindow_Deactivated(object sender, System.EventArgs e)
        {
            this.Hide();
        }


        public enum NotifyType
        {
            StatusMessage,
            ErrorMessage
        };
    }
}
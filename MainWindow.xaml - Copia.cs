using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using BluetoothManager.Class;
using InTheHand.Net.Bluetooth;
using Windows.Devices.Radios;
using Windows.UI.Xaml.Input;

namespace BluetoothManager
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : System.Windows.Window

    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeAction();
            InitializeBluetoothRadio();

            _Expander = new ListViewExpander();
            Devices.SelectionChanged += _Expander.Devices_SelectionChanged;
        }

        public void InitializeAction()
        {
            ViewModel = new BlueSettings();
            ms_settings.DataContext = ViewModel;

            radioMode = new RadioMode();
            Switch.DataContext = radioMode;

        }

        private ListViewExpander _Expander { get; set; }
        private void Devices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _Expander.Devices_SelectionChanged(sender, e);
        }

        private BlueSettings ViewModel { get; set; }
        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.OpenBluetoothSettingsCommand.Execute(null);
        }

        private RadioMode radioMode { get; set; }
        private Radio bluetoothRadio;

        private async void InitializeBluetoothRadio()
        {
            var accessLevel = await Radio.RequestAccessAsync();
            if (accessLevel != RadioAccessStatus.Allowed)
            {
                MessageBox.Show("O aplicativo não tem permissão para controlar os rádios.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var radios = await Radio.GetRadiosAsync();
            foreach (var radio in radios)
            {
                if (radio.Kind == RadioKind.Bluetooth)
                {
                    bluetoothRadio = radio;
                    break;
                }
            }

            if (bluetoothRadio == null)
            {
                MessageBox.Show("Nenhum rádio Bluetooth encontrado.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ToggleBluetoothButton_Click(object sender, RoutedEventArgs e)
        {
            if (bluetoothRadio != null)
            {
                MessageBox.Show("Nenhum rádio Bluetooth encontrado.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Switch.IsEnabled = false;
            var newState = bluetoothRadio.State != RadioState.On ? RadioState.Off : RadioState.On;

            try
            {
                await bluetoothRadio.SetStateAsync(newState);
                MessageBox.Show($"Bluetooth {(newState == RadioState.On ? "ativado" : "desativado")}.", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao alterar o estado do Bluetooth: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Switch.IsEnabled = true;
            }
        }
        public class RadioModel : INotifyPropertyChanged
        {
            private readonly Radio radio;
            private bool isEnabled;

            public RadioModel(Radio radio)
            {
                this.radio = radio;
                // Desabilita o controle de rádios de banda larga móvel
                isEnabled = (radio.Kind != RadioKind.MobileBroadband);
                radio.StateChanged += Radio_StateChanged;
            }

            private void Radio_StateChanged(Radio sender, object args)
            {
                OnPropertyChanged(nameof(IsRadioOn));
            }

            public string Name => radio.Name;

            public bool IsRadioOn => radio.State == RadioState.On;

            public bool IsEnabled
            {
                get => isEnabled;
                set
                {
                    isEnabled = value;
                    OnPropertyChanged();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public async Task SetRadioStateAsync(RadioState state)
            {
                if (radio.State != state)
                {
                    try
                    {
                        IsEnabled = false;
                        await radio.SetStateAsync(state);
                        OnPropertyChanged(nameof(IsRadioOn));
                    }
                    catch (Exception ex)
                    {
                        // Trate a exceção conforme necessário, por exemplo, exibir uma mensagem de erro.
                    }
                    finally
                    {
                        IsEnabled = true;
                    }
                }
            }
        }
    }
}

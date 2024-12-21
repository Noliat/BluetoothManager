using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using InTheHand.Net.Bluetooth.Factory;
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
            

            _Expander = new ListViewExpander();
            Devices.SelectionChanged += _Expander.Devices_SelectionChanged;
        }

        public void InitializeAction()
        {
            ViewModel = new BlueSettings();
            ms_settings.DataContext = ViewModel;
            Loaded += MainWindow_Loaded;

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

        private Radio _bluetoothRadio;
        private bool _isRadioOn;

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var accessLevel = await Radio.RequestAccessAsync();
                if (accessLevel == RadioAccessStatus.Allowed)
                {
                    MessageBox.Show("O aplicativo não tem permissão para controlar os rádios.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var radios = await Radio.GetRadiosAsync();
                _bluetoothRadio = radios.FirstOrDefault(r => r.Kind == RadioKind.Bluetooth);
                if (_bluetoothRadio != null)
                {
                    _isRadioOn = _bluetoothRadio.State == RadioState.On;
                }
                else
                {
                    MessageBox.Show("Bluetooth radio not found.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao acessar rádios: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Switch_Click(object sender, RoutedEventArgs e)
        {
            if (_bluetoothRadio != null)
            {
                try
                {
                    if (_isRadioOn)
                    {
                        await _bluetoothRadio.SetStateAsync(RadioState.Off);
                        _isRadioOn = false;
                    }
                    else
                    {
                        await _bluetoothRadio.SetStateAsync(RadioState.On);
                        _isRadioOn = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao mudar o estado do rádio: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
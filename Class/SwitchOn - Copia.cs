using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Windows.Devices.Radios;
using Windows.UI.Xaml.Controls;

namespace BluetoothManager.Class
{
    internal class SwitchOn : Window
    {
        public ObservableCollection<RadioModel> RadioSwitchList { get; set; } = new ObservableCollection<RadioModel>();

        public SwitchOn()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var accessLevel = await Radio.RequestAccessAsync();
                if (accessLevel != RadioAccessStatus.Allowed)
                {
                    NotifyUser("O aplicativo não tem permissão para controlar os rádios.", NotifyType.ErrorMessage);
                }
                else
                {
                    var radios = await Radio.GetRadiosAsync();
                    foreach (var radio in radios)
                    {
                        RadioSwitchList.Add(new RadioModel(radio, Dispatcher));
                    }
                }
            }
            catch (Exception ex)
            {
                NotifyUser($"Erro ao acessar rádios: {ex.Message}", NotifyType.ErrorMessage);
            }
        }

        private void InitializeComponent()
        {
            // Implement your WPF initialization logic here
        }

        public void NotifyUser(string strMessage, NotifyType type)
        {
            if (Dispatcher.CheckAccess())
            {
                UpdateStatus(strMessage, type);
            }
            else
            {
                Dispatcher.Invoke(() => UpdateStatus(strMessage, type));
            }
        }

        private void UpdateStatus(string strMessage, NotifyType type)
        {
            // Implement your status update logic here
        }

        private void Footer_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = ((HyperlinkButton)sender).Tag.ToString(),
                UseShellExecute = true
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Implement your button click logic here
        }
    }

    public class RadioModel : INotifyPropertyChanged
    {
        private readonly Radio radio;
        private bool isEnabled;
        private readonly Dispatcher dispatcher;

        public RadioModel(Radio radio, Dispatcher dispatcher)
        {
            this.radio = radio;
            isEnabled = (radio.Kind == RadioKind.Bluetooth);
            this.dispatcher = dispatcher;
            radio.StateChanged += Radio_StateChanged;
        }

        private async void Radio_StateChanged(Radio sender, object args)
        {
            await dispatcher.InvokeAsync(() =>
            {
                NotifyPropertyChanged(nameof(IsRadioOn));
            });
        }

        public string Name => radio.Name;

        public bool IsRadioOn
        {
            get => radio.State == RadioState.On;
            set => SetRadioState(value);
        }

        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                if (isEnabled != value)
                {
                    isEnabled = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void SetRadioState(bool isRadioOn)
        {
            if (isRadioOn != IsRadioOn)
            {
                var radioState = isRadioOn ? RadioState.On : RadioState.Off;
                IsEnabled = false;
                try
                {
                    await radio.SetStateAsync(radioState);
                }
                catch (Exception ex)
                {
                    NotifyUser($"Erro ao mudar o estado do rádio: {ex.Message}", NotifyType.ErrorMessage);
                }
                NotifyPropertyChanged(nameof(IsRadioOn));
                IsEnabled = true;
            }
        }

        private void NotifyUser(string message, NotifyType type)
        {
            dispatcher.Invoke(() => MessageBox.Show(message));
        }
    }

    public enum NotifyType
    {
        ErrorMessage,
        // Add other notification types if needed
    }
}

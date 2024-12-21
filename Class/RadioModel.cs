using BluetoothManager.Class.Command;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Devices.Radios;

namespace BluetoothManager.Class
{
    public class RadioModel : INotifyPropertyChanged
    {
        public ICommand ToggleRadioCommand { get; }

        private readonly Radio radio;
        private bool isEnabled;

        public RadioModel(Radio radio)
        {
            this.radio = radio;
            // Desabilita o controle de rádios de banda larga móvel
            isEnabled = (radio.Kind != RadioKind.MobileBroadband);
            radio.StateChanged += Radio_StateChanged;

            // Comando para alternar o estado do rádio
            ToggleRadioCommand = new RelayCommand(async _ => await ToggleRadioAsync());

        }

        private async Task ToggleRadioAsync()
        {
            var newState = IsRadioOn ? RadioState.Off : RadioState.On;
            await SetRadioStateAsync(newState);
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
                catch (Exception )
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

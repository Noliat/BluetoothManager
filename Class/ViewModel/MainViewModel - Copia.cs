using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;
using Windows.Devices.Enumeration;
using BluetoothManager;

namespace BluetoothManager.Class.ViewModel
{
    public class MainViewModel
    {
        public BlueSettings BlueSettings { get; set; }
        private DeviceWatcherHelper DeviceWatcherHelper { get; set; }
        public ObservableCollection<DeviceInformationDisplay> DevicesCollection { get; set; }
        public ConnectionStatusInformation ConnectionStatusInformation { get; set; }
        public BooleanToVisibilityConverter BooleanToVisibilityConverter{get; set;}
        public PairButton PairButton { get; set; }

        public MainViewModel()
        {
            BlueSettings = new BlueSettings();
            DevicesCollection = new ObservableCollection<DeviceInformationDisplay>();
           
            // Passa o Dispatcher atual para o DeviceWatcherHelper
            DeviceWatcherHelper = new DeviceWatcherHelper(DevicesCollection, Dispatcher.CurrentDispatcher);

            // Inicialização de ConnectionStatusInformation
            if (DevicesCollection.Any())
            {
                ConnectionStatusInformation = new ConnectionStatusInformation(DevicesCollection.First().DeviceInformation);
            }
            BooleanToVisibilityConverter = new BooleanToVisibilityConverter();

            PairButton = new PairButton();
        }
    }
}

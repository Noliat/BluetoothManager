using BluetoothManager.Class;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace BluetoothManager.Class
{
    public class BluetoothManager
    {
        public DeviceWatcherHelper deviceWatcherHelper;

        public BluetoothManager(ObservableCollection<DeviceInformationDisplay> devicesCollection, Dispatcher dispatcher)
        {
            deviceWatcherHelper = new DeviceWatcherHelper(devicesCollection, dispatcher);
        }

        public void StartBluetoothWatcher(DeviceSelectorInfo deviceSelectorInfo)
        {
            // Inicia o watcher apenas com dispositivos Bluetooth (pareados ou não)
            deviceWatcherHelper.StartWatcher(deviceSelectorInfo);
        }

        public void StopBluetoothWatcher()
        {
            deviceWatcherHelper.StopWatcher();
        }
    }
}
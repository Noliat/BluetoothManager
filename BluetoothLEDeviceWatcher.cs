using System;
using Windows.Devices.Enumeration;

namespace BluetoothApp
{
    internal class BluetoothLEDeviceWatcher
    {
        public BluetoothLEDeviceWatcher(object value, string[] requestedProperties)
        {
            Value = value;
            RequestedProperties = requestedProperties;
        }

        public object Value { get; }
        public string[] RequestedProperties { get; }
        public Action<BluetoothLEDeviceWatcher, DeviceInformation> Added { get; internal set; }
        public Action<BluetoothLEDeviceWatcher, BluetoothLEDeviceWatcherStoppedEventArgs> Stopped { get; internal set; }

        internal void Start()
        {
            // Implemente a lógica para iniciar a busca por dispositivos Bluetooth
        }

        internal void Stop()
        {
            // Implemente a lógica para parar a busca por dispositivos Bluetooth
        }
    }
}

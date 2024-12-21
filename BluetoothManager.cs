using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using System.Runtime.InteropServices;
using Windows.Devices.Bluetooth;

namespace BluetoothApp
{
    public class BluetoothManager
    {
        private MainWindow mainWindow;
        private ObservableCollection<BluetoothDevice> bluetoothDevices = new ObservableCollection<BluetoothDevice>();
        private BluetoothLEDeviceWatcher bluetoothLEDeviceWatcher;

        public BluetoothManager(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }
        // Verifica se o Bluetooth está habilitado

        // Outros membros e métodos...

        public bool IsBluetoothEnabled()
        {
            var devices = DeviceInformation.FindAllAsync(RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort)).AsTask().Result;
            return devices.Any();
        }

        // Habilita o Bluetooth
        public void EnableBluetooth() =>
            // Implementar lógica para habilitar o Bluetooth
            // Aqui está um exemplo simples para Windows:
            _ = BluetoothEnable();

        // Desabilita o Bluetooth
        public void DisableBluetooth()
        {
            // Implementar lógica para desabilitar o Bluetooth
            // Aqui está um exemplo simples para Windows:
            BluetoothDisable();
        }

        // Método interno para verificar se o Bluetooth está habilitado (exemplo para Windows)
        [DllImport("Bthprops.cpl", SetLastError = true)]
        private static extern bool BluetoothIsEnabled();

        // Método interno para habilitar o Bluetooth (exemplo para Windows)
        [DllImport("Bthprops.cpl", SetLastError = true)]
        private static extern bool BluetoothEnable();

        // Método interno para desabilitar o Bluetooth (exemplo para Windows)
        [DllImport("Bthprops.cpl", SetLastError = true)]
        private static extern bool BluetoothDisable();

        public void ConnectToDevice(BluetoothDevice device)
        {
            // Implementar lógica para se conectar ao dispositivo Bluetooth selecionado
            // Exemplo: 
            // 1. Iniciar uma conexão com o dispositivo usando APIs específicas da plataforma
            // 2. Atualizar a interface do usuário conforme necessário (por exemplo, exibir uma mensagem de conexão bem-sucedida)
            // 3. Lidar com erros e exceções, se necessário

            // Exemplo simplificado:
            mainWindow.AddLog($"Conectando-se ao dispositivo: {device.Name}");
        }

        public void DisconnectDevice()
        {
            // Implementar lógica para desconectar o dispositivo Bluetooth atualmente conectado
            // Exemplo: 
            // 1. Encerrar a conexão com o dispositivo usando APIs específicas da plataforma
            // 2. Atualizar a interface do usuário conforme necessário (por exemplo, exibir uma mensagem de desconexão)
            // 3. Lidar com erros e exceções, se necessário

            // Exemplo simplificado:
            mainWindow.AddLog("Dispositivo Bluetooth desconectado.");
        }

        // Método para obter os dispositivos Bluetooth
        public List<BluetoothDevice> GetBluetoothDevices()
        {
            // Verifica se a coleção é do tipo ObservableCollection<BluetoothDevice>
            if (mainWindow.bluetoothDevices is ObservableCollection<BluetoothDevice> collection)
            {
                // Converte a coleção para uma lista e retorna
                return collection.ToList();
            }
            else
            {
                // Caso a coleção não seja do tipo esperado, retorna uma lista vazia ou lança uma exceção, dependendo do caso.
                return new List<BluetoothDevice>();
            }
        }


        public void InitializeBluetoothWatcher()
        {
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };
            bluetoothLEDeviceWatcher = new BluetoothLEDeviceWatcher(null, requestedProperties);
            bluetoothLEDeviceWatcher.Added += BluetoothLEDeviceWatcher_Added;
            bluetoothLEDeviceWatcher.Stopped += BluetoothLEDeviceWatcher_Stopped;
            bluetoothLEDeviceWatcher.Start();
        }

        private void BluetoothLEDeviceWatcher_Added(BluetoothLEDeviceWatcher sender, DeviceInformation args)
        {
            mainWindow.AddBluetoothDevice(args.Name, args.Id);
        }

        private void BluetoothLEDeviceWatcher_Stopped(BluetoothLEDeviceWatcher sender, BluetoothLEDeviceWatcherStoppedEventArgs args)
        {
            // Handle Bluetooth LE device watcher stopped event
        }

        public override bool Equals(object obj)
        {
            return obj is BluetoothManager manager &&
                   EqualityComparer<MainWindow>.Default.Equals(mainWindow, manager.mainWindow) &&
                   EqualityComparer<ObservableCollection<BluetoothDevice>>.Default.Equals(bluetoothDevices, manager.bluetoothDevices) &&
                   EqualityComparer<BluetoothLEDeviceWatcher>.Default.Equals(bluetoothLEDeviceWatcher, manager.bluetoothLEDeviceWatcher);
        }

        internal void AddBluetoothDevice(string name, string id)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            int hashCode = -616339448;
            hashCode = hashCode * -1521134295 + EqualityComparer<MainWindow>.Default.GetHashCode(mainWindow);
            hashCode = hashCode * -1521134295 + EqualityComparer<ObservableCollection<BluetoothDevice>>.Default.GetHashCode(bluetoothDevices);
            hashCode = hashCode * -1521134295 + EqualityComparer<BluetoothLEDeviceWatcher>.Default.GetHashCode(bluetoothLEDeviceWatcher);
            return hashCode;
        }
    }
}

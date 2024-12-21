using System;
using System.Windows;

namespace BluetoothApp
{
    public class BluetoothManager
    {
        private MainWindow mainWindow;

        public BluetoothManager(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public void ToggleBluetooth()
        {
            // Implemente a lógica para ligar ou desligar o Bluetooth
            mainWindow.AddLog("Bluetooth ligado/desligado.");
        }

        public void ConnectToDevice()
        {
            // Implemente a lógica para conectar-se ao dispositivo selecionado na lista
            mainWindow.AddLog("Conectando-se ao dispositivo selecionado.");
        }
    }
}

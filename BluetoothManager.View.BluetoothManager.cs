using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Runtime.InteropServices;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using BluetoothApp;

namespace BluetoothManager.View
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
            mainWindow.ToggleBluetooth_Click(null, null);
		}

		public void ConnectToDevice()
		{
            // Implemente a lógica para conectar-se ao dispositivo selecionado na lista
            mainWindow.ConnectButton_Click(null, null);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;


namespace BluetoothManager.View
{
    public partial class MainWindow : Window
    {
        private object toggleBluetoothButton;

        public MainWindow(object toggleBluetoothButton)
        {
            this.toggleBluetoothButton = toggleBluetoothButton;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent() => throw new NotImplementedException();

        private void ToggleBluetooth_Click(object sender,
                                           RoutedEventArgs e)
        {
            // Verifica se o botão de alternância está marcado
            if ((toggleBluetoothButton as System.Windows.Controls.Primitives.ToggleButton)?.IsChecked == true)
            {
                // Código para ligar o Bluetooth
                // Por exemplo: BluetoothManager.TurnOn();
            }
            else
            {
                // Código para desligar o Bluetooth
                // Por exemplo: BluetoothManager.TurnOff();
            }

        }

        internal void AddLog(string v)
        {
            throw new NotImplementedException();
        }

        internal void ToggleBluetooth_Click(object value1, object value2)
        {
            throw new NotImplementedException();
        }

        internal void ConnectButton_Click(object value1, object value2)
        {
            throw new NotImplementedException();
        }
    }
}


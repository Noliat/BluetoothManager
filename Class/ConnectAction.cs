using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

using (link unavailable);

namespace BluetoothManager.Class
{
    internal class ConnectAction
    {
        [DllImport("BluetoothAPIs.dll", EntryPoint = "BluetoothGetDeviceSelector")]
        private static extern IntPtr BluetoothGetDeviceSelector();

        [DllImport("BluetoothAPIs.dll", EntryPoint = "BluetoothDeviceGetElement")]
        private static extern IntPtr BluetoothDeviceGetElement(IntPtr device, ref Guid serviceUuid);

        static async Task Main(string[] args)
        {
            // Obter lista de dispositivos Bluetooth pareados
            var devices = await DeviceInformation.FindAllAsync(BluetoothDevice.GetDeviceSelector());

            foreach (var device in devices)
            {
                // Verificar se o dispositivo está pareado
                if (device.Pairing.IsPaired)
                {
                    // Obter o serviço Bluetooth
                    var serviceUuid = new Guid("00001101-0000-1000-8000-00805f9b34fb"); // UUID do serviço
                    var service = (BluetoothDevice)BluetoothDeviceGetElement((link unavailable), ref serviceUuid);

                    // Conectar ao dispositivo
                    await service.ConnectAsync();

                    Console.WriteLine($"Conectado ao dispositivo: {device.Name}");
                }
            }
        }
    }
}

using System;
using System.Windows;

namespace BluetoothManager.Class
{
    public class PosWindow
    {
        public Window Window { get; set; }

        public PosWindow(Window window)
        {
            Window = window;
            window.Left = SystemParameters.WorkArea.Right - window.Width; // Posiciona a janela à direita da tela
            window.Top = SystemParameters.WorkArea.Bottom - window.Height; // Posiciona a janela na parte inferior da tela
        }
    }
}

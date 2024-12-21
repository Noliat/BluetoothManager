using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using BluetoothManager.Class.Command;

namespace BluetoothManager.Class.ViewModel
{
    public class NotifyIconViewModel
    {
        /// <summary>
        /// Mostra a janela, se nenhuma estiver aberta, ou alterna a visibilidade dela.
        /// </summary>
        private bool IsVisible = false;

        public ICommand ToggleWindowVisibilityCommand
        { 
            get
            {
                return new RelayCommand(
                    execute: (obj) =>
                    {
                        // Alterna a visibilidade da janela
                        if (IsVisible)
                        {
                            // Se a janela está visível, oculta
                            Application.Current.MainWindow.Hide();
                            IsVisible = false;
    }
                        else
                        {
                            // Se a janela está oculta, exibe e ativa
                            Application.Current.MainWindow.Show();
                            Application.Current.MainWindow.Activate(); // Garante que a janela receba o foco
                            Application.Current.MainWindow.Topmost = true;
                            IsVisible = true;
                        }
                    },
                    canExecute: (obj) => true // Sempre pode ser executado
                );
            }
        }

        /// <summary>
        /// Fecha a janela se estiver aberta.
        /// </summary>
        public ICommand CloseWindowCommand
        {
            get
            {
                return new RelayCommand(
                    execute: (obj) =>
                    {
                        if (Application.Current.MainWindow != null)
                        {
                            Application.Current.MainWindow.Close();
                            Application.Current.MainWindow = null; // Libera a referência após fechar
                        }
                    },
                    canExecute: (obj) => Application.Current.MainWindow != null
                );
            }
        }

        /// <summary>
        /// Fecha a aplicação.
        /// </summary>
        public ICommand ExitApplicationCommand
        {
            get
            {
                return new RelayCommand(
                    execute: (obj) => Application.Current.Shutdown()
                );
            }
        }

        /// <summary>
        /// Comando para abrir as configurações de Bluetooth.
        /// </summary>
        private readonly BlueSettings blueSettings;
        public ICommand OpenBluetoothSettingsCommand => blueSettings.OpenBluetoothSettingsCommand;

        public NotifyIconViewModel()
        {
            blueSettings = new BlueSettings();
        }
    }
}

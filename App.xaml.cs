using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Hardcodet.Wpf.TaskbarNotification;

namespace BluetoothManager
{
    /// <summary>
    /// Interação lógica para App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex _mutex;

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        private TaskbarIcon notifyIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

           //create the notifyicon (it's a resource declared in NotifyIconResources.xaml
            notifyIcon = (TaskbarIcon)FindResource("NotifyIcon");

            const string appName = "BluetoothManager";
            bool createdNew;
            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                // Traz a janela já existente para o primeiro plano
                BringToForeground();
                // Fecha esta nova instância
                Shutdown();
            }
        }

        private void BringToForeground()
        {
            // Obtém o nome do produto a partir do assembly
            string windowTitle = GetAssemblyProductName();

            // Encontra a janela com base no título
            IntPtr hWnd = FindWindow(null, windowTitle);

            if (hWnd != IntPtr.Zero)
            {
                // Exibe a janela caso esteja minimizada (nCmdShow = 9 para restaurar)
                ShowWindow(hWnd, 9);
                // Traz a janela para o primeiro plano
                SetForegroundWindow(hWnd);
            }
        }

        private string GetAssemblyProductName()
        {
            // Obtém o nome do produto ou título configurado no assembly
            var assembly = Assembly.GetExecutingAssembly();
            var productAttribute = (AssemblyProductAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyProductAttribute));
            return productAttribute?.Product ?? "BluetoothManager"; // Substitui "Default App Name" pelo nome padrão caso o atributo não exista
        }

        protected override void OnExit(ExitEventArgs e)
        {
            notifyIcon.Dispose(); //the icon would clean up automatically, but this is cleaner
            base.OnExit(e);
        }
    }
}


using System.Windows;
using System.Windows.Controls;
using BluetoothManager.Class;
using System.ComponentModel;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace BluetoothManager.Class
{
    public class ListViewExpander
    {
        public DeviceInformation deviceInformation;
        public ConnectionStatusInformation ConnectionStatusInformation { get; set; }
        public DeviceInformationDisplay deviceInformationDisplay;
        private BluetoothConnectionStatus BluetoothConnectionStatus { get; set; }
        private bool IsSelected = true;

        public void Devices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as ListView;
            if (listView != null)
            {
                foreach (var listViewItem in listView.Items)
                {
                    var itemContainer = listView.ItemContainerGenerator.ContainerFromItem(listViewItem) as ListViewItem;
                    if (itemContainer != null)
                    {
                        var deviceInfoDisplay = itemContainer.Content as DeviceInformationDisplay;

                        // Usa a propriedade ConnectionStatusInformation dentro de DeviceInformationDisplay
                        var connectionInfo = deviceInfoDisplay?.ConnectionStatusInformation;

                        if (itemContainer.IsSelected && ShouldExpandItem(connectionInfo))
                        {
                            ListViewItem_Expanded(itemContainer);
                            IsSelected = true;
                        }
                        else
                        {
                            ListViewItem_Collapsed(itemContainer);
                            IsSelected = false;
                        }
                    }
                }
            }
        }

        // Verifica se o item deve ser expandido baseado nas condições fornecidas
        private bool ShouldExpandItem(ConnectionStatusInformation connectionInfo)
        {
            if (connectionInfo != null)

            // Expande se o dispositivo estiver conectado ou se estiver emparelhado e for um dispositivo de áudio
            if (connectionInfo.IsConnected)
            {
                return true; // Expande se o dispositivo estiver conectado
            }
            else if (connectionInfo.IsPaired && IsAudioDevice(connectionInfo))
            {
                return true; // Expande se o dispositivo estiver emparelhado e for um dispositivo de áudio
            }

            return false; // Caso contrário, colapsa
        }

        // Verifica se o dispositivo é de áudio
        private bool IsAudioDevice(ConnectionStatusInformation connectionInfo)
        {
            // Usa o método IsAudioDevice da ConnectionStatusInformation
            return connectionInfo.IsAudioDevice(out _);
        }

        // Chamada quando o item está colapsado
        private void ListViewItem_Collapsed(ListViewItem item)
        {
            if (item != null)
            {
                item.Height = 55; // Altura colapsada
                var expandedContent = item.FindName("ExpandedContent") as UIElement;
                if (expandedContent != null)
                {
                    expandedContent.Visibility = Visibility.Collapsed;
                }
            }
        }

        // Chamada quando o item está expandido
        private void ListViewItem_Expanded(ListViewItem item)
        {
            if (item != null)
            {
                item.Height = 108; // Altura expandida
                var expandedContent = item.FindName("ExpandedContent") as UIElement;
                if (expandedContent != null)
                {
                    expandedContent.Visibility = Visibility.Visible;
                }
            }
        }

        // Método para garantir que a lista seja exibida colapsada por padrão
        public void CollapseAllItems(ListView listView)
        {
            foreach (var listViewItem in listView.Items)
            {
                var itemContainer = listView.ItemContainerGenerator.ContainerFromItem(listViewItem) as ListViewItem;
                if (itemContainer != null)
                {
                    ListViewItem_Collapsed(itemContainer); // Colapsar todos os itens ao carregar
                }
            }
        }
    }
}

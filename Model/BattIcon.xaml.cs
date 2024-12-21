using BluetoothManager.Class;
using BluetoothManager.Class.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BluetoothManager.Model
{
    /// <summary>
    /// Interação lógica para UserControl1.xam
    /// </summary>
    public partial class BattIcon : UserControl
    {
        private RadioModel radioModel;
        private MainViewModel viewModel;
        public BattIcon()
        {
            InitializeComponent();
            viewModel = new MainViewModel();
            this.DataContext = new MainViewModel(); // Defina o ViewModel
        }


    }
}

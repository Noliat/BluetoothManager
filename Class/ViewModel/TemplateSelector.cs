using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using BluetoothManager;

namespace BluetoothManager.Class.ViewModel
{
    public class TemplateSelector : DataTemplateSelector
    {
        public DataTemplate RadioOnTemplate { get; set; }
        public DataTemplate RadioOffTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var viewModel = item as MainViewModel;
            if (viewModel != null)
            {
                return viewModel.IsRadioOn ? RadioOnTemplate : RadioOffTemplate;
            }
            return base.SelectTemplate(item, container);
        }
    }
}

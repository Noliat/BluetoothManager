using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using BluetoothManager;

namespace BluetoothManager.Class.Ui
{
    internal class PressEffect
    {
        private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            Button btn = sender as Button;
            Point clickPosition = e.GetPosition(btn);

            double centerX = btn.ActualWidth / 2;
            double centerY = btn.ActualHeight / 2;

            // Define os ângulos com base na posição do clique
            double angleX = (clickPosition.X - centerX) / centerX * 5; // Controla a inclinação no eixo X
            double angleY = (clickPosition.Y - centerY) / centerY * 5; // Controla a inclinação no eixo Y

            // Aplicar a transformação de Skew baseada na posição do clique
            SkewTransform skew = new SkewTransform(angleX, angleY);
            btn.RenderTransform = skew;
        }

        private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Button btn = sender as Button;
            // Reverter a transformação ao soltar o botão
            SkewTransform skew = new SkewTransform(0, 0);
            btn.RenderTransform = skew;
        }

    }
}

using System.Windows;
using System.Windows.Media;
using Windows.UI.ViewManagement;

namespace BluetoothManager.Class
{
    public class SystemThemeHelper
    {
        public static void ApplySystemThemeColors(Window window)
        {
            var uiSettings = new UISettings();

            // Captura as cores do sistema
            var backgroundColor = uiSettings.GetColorValue(UIColorType.Background);
            var foregroundColor = uiSettings.GetColorValue(UIColorType.Foreground);
            var accentColor = uiSettings.GetColorValue(UIColorType.Accent);
            var highlightColor = uiSettings.GetColorValue(UIColorType.Highlight);
            var controlBackgroundColor = uiSettings.GetColorValue(UIColorType.ControlBackground);
            var secondaryTextColor = uiSettings.GetColorValue(UIColorType.SecondaryText); // Subtexto

            // Converte as cores do sistema para SolidColorBrush do WPF
            var backgroundBrush = new SolidColorBrush(Color.FromArgb(backgroundColor.A, backgroundColor.R, backgroundColor.G, backgroundColor.B));
            var foregroundBrush = new SolidColorBrush(Color.FromArgb(foregroundColor.A, foregroundColor.R, foregroundColor.G, foregroundColor.B));
            var accentBrush = new SolidColorBrush(Color.FromArgb(accentColor.A, accentColor.R, accentColor.G, accentColor.B));
            var highlightBrush = new SolidColorBrush(Color.FromArgb(highlightColor.A, highlightColor.R, highlightColor.G, highlightColor.B));
            var controlBackgroundBrush = new SolidColorBrush(Color.FromArgb(controlBackgroundColor.A, controlBackgroundColor.R, controlBackgroundColor.G, controlBackgroundColor.B));
            var secondaryTextBrush = new SolidColorBrush(Color.FromArgb(secondaryTextColor.A, secondaryTextColor.R, secondaryTextColor.G, secondaryTextColor.B)); // Subtexto

            // Aplica a cor de fundo da janela
            window.Background = backgroundBrush;

            // Aplica as cores aos controles da janela
            foreach (var child in LogicalTreeHelper.GetChildren(window))
            {
                if (child is FrameworkElement element)
                {
                    // Define a cor de texto
                    element.Foreground = foregroundBrush;

                    // Se o controle tiver subtexto, aplicamos a cor de subtexto
                    if (element is TextBlock textBlock && textBlock.TextWrapping != TextWrapping.NoWrap) // Exemplo de verificação de subtexto
                    {
                        textBlock.Foreground = secondaryTextBrush;
                    }

                    // Aplicar cor de destaque e borda aos botões
                    if (element is Button)
                    {
                        element.Background = accentBrush;
                        element.BorderBrush = highlightBrush;
                    }
                    // Aplicar cor de fundo e borda para outros controles
                    else if (element is Control control)
                    {
                        control.Background = controlBackgroundBrush;

                        // Verifica se o controle tem borda e aplica o brush de borda
                        if (control.BorderBrush == null) // Somente aplicar se não houver uma cor de borda já aplicada
                        {
                            control.BorderBrush = highlightBrush;
                        }
                    }
                }
            }
        }
    }
}

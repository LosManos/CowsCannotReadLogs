using System.Windows;
using System.Windows.Controls;

namespace CowsCannotReadLogs.Client.Wpf.Extensions
{
    internal static class UIELementExtensions
    {
        internal static void SetColumnRow(this UIElement me, int columnPosition, int rowPosition)
        {
            Grid.SetColumn(me, columnPosition);
            Grid.SetRow(me, rowPosition);
        }
    }
}

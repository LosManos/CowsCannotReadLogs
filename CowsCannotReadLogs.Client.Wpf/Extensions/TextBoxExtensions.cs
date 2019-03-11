using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CowsCannotReadLogs.Client.Wpf.Extensions
{
    internal static class TextBoxExtensions
    {
        internal static FormattedText GetFormattedText(this TextBox me)
        {
            var formattedText = new FormattedText(
                    me.Text,
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(me.FontFamily, me.FontStyle, me.FontWeight, me.FontStretch),
                    me.FontSize,
                    me.Foreground,
                    // 1 is not correct. Use VisualTreeHelper.GetDpi(...).
                    // See https://stackoverflow.com/a/43311364/521554.
                    1);
            return formattedText;
        }
    }
}

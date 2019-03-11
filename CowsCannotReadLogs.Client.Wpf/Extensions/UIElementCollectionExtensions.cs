using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CowsCannotReadLogs.Client.Wpf.Extensions
{
    internal static class UIElementCollectionExtensions
    {
        internal static IEnumerable<T> OfType<T>(this UIElementCollection me)
        {
            return me.Cast<UIElement>().Where(c => c.GetType() == typeof(T)).Cast<T>();
        }
    }
}

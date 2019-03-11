using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace CowsCannotReadLogs.Client.Wpf.Extensions
{
    internal static class GridExtensions
    {
        /// <summary>This method adds a row to the grid and puts the control in this row.
        /// Column is decided by columnPosition.
        /// </summary>
        /// <param name="me"></param>
        /// <param name="innerControl"></param>
        /// <param name="columnPosition"></param>
        internal static void AddAsLastRow(this Grid me, UIElement innerControl,  int columnPosition)
        {
            me.Children.Add(innerControl);
            innerControl.SetColumnRow(columnPosition, me.RowDefinitions.Count - 1);
        }

        internal static Grid AddColumnDefinition(this Grid me, ColumnDefinition columnDefinition)
        {
            me.ColumnDefinitions.Add(columnDefinition);
            return me;
        }

        internal static Grid AddColumnDefinitions(this Grid me, IEnumerable<ColumnDefinition> columnDefinitions )
        {
            foreach (var columnDefinition in columnDefinitions)
            {
                me.AddColumnDefinition(columnDefinition);
            }
            return me;
        }

        internal static Grid AddRowDefinition( this Grid me, RowDefinition rowDefinition)
        {
            me.RowDefinitions.Add(rowDefinition);
            return me;
        }

        internal static Grid AddRowDefinitions(this Grid me, IEnumerable<RowDefinition> rowDefinitions )
        {
            foreach (var rowDefinition in rowDefinitions)
            {
                me.AddRowDefinition(rowDefinition);
            }
            return me;
        }
    }
}

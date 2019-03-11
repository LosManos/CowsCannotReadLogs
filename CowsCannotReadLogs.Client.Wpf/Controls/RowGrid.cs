using CowsCannotReadLogs.TextReading;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using CowsCannotReadLogs.Client.Wpf.Extensions;
using System.Collections.Generic;
using System;

namespace CowsCannotReadLogs.Client.Wpf.Controls
{
    public class RowGrid : CowsCannotReadLogsGrid
    {
        internal static RowGrid CreateWithData(TextReader.Group group)
        {
            var row = group.Single();

            var ret = new RowGrid
            {
                Background = Brushes.Aqua
            }
            // Create columns for one less than all.
            .AddColumnDefinitions(row.Skip(1).Select(word => new ColumnDefinition { Width = GridLength.Auto }))
            // Create the last column.
            .AddColumnDefinition(new ColumnDefinition { Width = new GridLength(10, GridUnitType.Star) })
            .AddRowDefinition(new RowDefinition());

            ((RowGrid)ret).SetData(group);

            return (RowGrid)ret; // Cast is needed but I do notgit understand why.
        }

        /// <summary>This method returns a list of the widest control for every column.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<double> GetColumnWidths()
        {
            var widths = Array.CreateInstance(typeof(double), ColumnDefinitions.Count) as double[];
            for( var columnOrdinal = 0; columnOrdinal < ColumnDefinitions.Count; ++ columnOrdinal)
            {
                // Here we are looping through all controls for every column. 
                // This means elementCount*columnCount iterations. It should be faster to iterate through all controls
                // and keep a tab on which control is the widest in every column; one iterations and a Dictionary?
                // with just a few (columnCount) items.
                widths[columnOrdinal] =
                    Children
                        .Cast<TextBox>()
                        .Where(c => GetColumn(c) == columnOrdinal)
                        .Select(c => c.GetFormattedText().Width)
                        .Max();
            }
            return widths;
        }

        /// <summary>This method sets the widths for all columns.
        /// </summary>
        /// <param name="widths"></param>
        internal void SetColumnWidths(IEnumerable<double> widths)
        {
            const int Margin = 10; // 10 is a arbitrary number just to add some space around the text.
            foreach (var item in widths.SelectWithIndex())
            {
                ColumnDefinitions[item.Index].Width = new GridLength(item.Item + Margin, GridUnitType.Pixel);
            }
        }

        private void SetData(TextReader.Group group)
        {
            var row = group.Single();

            foreach (var word in row.SelectWithIndex())
            {
                var ctrl = new TextBox
                {
                    Text = word.Item,
                    Background = Brushes.Aquamarine
                };
                Children.Add(ctrl);
                ctrl.SetColumnRow(word.Index, RowDefinitions.Count - 1);
            }
        }
    }
}

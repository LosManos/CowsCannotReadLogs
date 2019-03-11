using CowsCannotReadLogs.Client.Wpf.Extensions;
using CowsCannotReadLogs.TextReading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CowsCannotReadLogs.Client.Wpf.Controls
{
    public class MainGrid : CowsCannotReadLogsGrid
    {
        private double[] _maxRowGridWidths;

        internal void AddRow(int rowNumber, TextReader.Group group)
        {
            this.AddRowDefinition(
                new RowDefinition { Height = new GridLength(10, GridUnitType.Auto) }
            );
            SetLastRowRowNumber(rowNumber);
            SetLastRowGroup(group, out var widths);

            StoreMaxOfEachItem(widths.ToList());
        }

        internal void SetWidths()
        {
            foreach( var rowGrid in Children.OfType<RowGrid>()){
                rowGrid.SetColumnWidths(_maxRowGridWidths);
            }
        }

        private IList<T> CreateListOfLength<T>(int length)
        {
            var ret = new List<T>(length);
            for (var _ = 0; _ <length; ++_)
            {
                ret.Add(default(T));
            }
            return ret;
        }

        private RowNumberLabel SetLastRowRowNumber(int rowNumber)
        {
            var label = new RowNumberLabel { Content = rowNumber };
            this.AddAsLastRow(label, 0);
            return label;
        }

        private RowGrid SetLastRowGroup(TextReader.Group group, out IEnumerable<double> widths)
        {
            var rowGrid = RowGrid.CreateWithData(group);
            this.AddAsLastRow(rowGrid, 1);

            widths = rowGrid.GetColumnWidths();

            return rowGrid;
        }

        private void StoreMaxOfEachItem(IList<double> widths)
        {
            _maxRowGridWidths = _maxRowGridWidths ?? Array.CreateInstance(typeof(double), widths.Count) as double[];
            for (var i = 0; i < widths.Count(); ++i)
            {
                _maxRowGridWidths[i] = Math.Max(widths[i], _maxRowGridWidths[i]);
            }
        }
    }
}

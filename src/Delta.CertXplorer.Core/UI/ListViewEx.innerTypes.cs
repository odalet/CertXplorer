using System;
using System.Linq;
using System.Windows.Forms;

namespace Delta.CertXplorer.UI
{
    partial class ListViewEx
    {
        public enum SortType
        {
            String,
            Numeric
        }

        public enum SortDirection
        {
            Ascending,
            Descending
        }

        public sealed class SortInformation
        {
            public SortInformation() : this(SortType.String, SortDirection.Ascending) { }
            public SortInformation(SortType type, SortDirection direction)
            {
                Type = type;
                Direction = direction;
            }

            public SortType Type { get; set; }
            public SortDirection Direction { get; set; }
        }

        public class ListViewItemComparer : System.Collections.IComparer
        {
            private readonly int columnIndex;
            private readonly bool asc;
            private readonly SortType sortType;

            public ListViewItemComparer()
                : this(0, true, SortType.String) { }
            public ListViewItemComparer(int colIndex, SortInformation sortInformation)
                : this(colIndex, sortInformation.Direction, sortInformation.Type) { }
            public ListViewItemComparer(int colIndex, SortDirection direction, SortType type)
                : this(colIndex, direction == SortDirection.Ascending, type) { }

            public ListViewItemComparer(int colIndex, bool ascending, SortType type)
            {
                columnIndex = colIndex;
                asc = ascending;
                sortType = type;
            }

            public string GetStringValue(object o)
            {
                var item = (ListViewItem)o;

                if (columnIndex == -1)
                    return item.Tag.ToString();
                if (item.SubItems.Count > columnIndex)
                    return item.SubItems[columnIndex].Text;
                return string.Empty;
            }

            public int Compare(object x, object y)
            {
                var sx = GetStringValue(x);
                var sy = GetStringValue(y);

                if (sortType == SortType.String) return CompareStrings(sx, sy);

                static double toDouble(string text)
                {
                    try
                    {
                        return string.IsNullOrEmpty(text) ? double.NaN : Convert.ToDouble(text);
                    }
                    catch { return double.NaN; }
                }

                var dx = toDouble(sx);
                var dy = toDouble(sy);
                return CompareDoubles(dx, dy);
            }

            private int CompareStrings(string sx, string sy) => asc ? string.Compare(sx, sy) : string.Compare(sy, sx);

            private int CompareDoubles(double dx, double dy)
            {
                if (double.IsNaN(dx))
                    return asc ? int.MinValue : int.MaxValue;

                if (double.IsNaN(dy))
                    return asc ? int.MaxValue : int.MinValue;

                var result = asc ? dx - dy : dy - dx;
                return (int)result;
            }
        }

        /// <summary>
        /// Stores a pair consisting of a control and its associated sub-item.
        /// </summary>
        private class EmbeddedControl
        {
            public Control Control { get; set; }
            public ListViewItem Item { get; set; }
            public int SubItemIndex { get; set; }

            public ListViewItem.ListViewSubItem SubItem
            {
                get
                {
                    if (Item == null) return null;
                    if (SubItemIndex < 0 || SubItemIndex >= Item.SubItems.Count) return null;
                    return Item.SubItems[SubItemIndex];
                }
            }
        }
    }
}

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Delta.CertXplorer.UI
{
    public partial class ListViewEx : ListView
    {
        private readonly Dictionary<int, SortInformation> sortProperties = new Dictionary<int, SortInformation>();
        private readonly SortInformation currentSortInformation = new SortInformation();

        public ListViewEx()
        {
            FullRowSelect = true;
            ControlPadding = new Padding(4);
            SortColumnOnClick = true;
        }

        public ListViewEx(IContainer container) : this() => container.Add(this);

        [Browsable(true), DefaultValue(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool SortColumnOnClick { get; set; }
        public Padding ControlPadding { get; set; }

        protected override void WndProc(ref Message m)
        {
            if (DesignMode)
            {
                base.WndProc(ref m);
                return;
            }

            const int WM_PAINT = 0x000F;

            switch (m.Msg)
            {
                case WM_PAINT:
                    if (View != View.Details)
                        break;

                    // Last column is full width
                    if (Columns.Count > 0) Columns[Columns.Count - 1].Width = -2;
                    break;
            }

            base.WndProc(ref m);
        }

        public bool SetColumnOrderType(int columnIndex, SortType sortType)
        {
            UpdateSortProperties();

            if (columnIndex == -1) currentSortInformation.Type = sortType;
            else
            {
                if (Columns.Count <= columnIndex) return false;
                else
                {
                    var info = sortProperties[columnIndex];
                    if (info == null) return false;
                    else info.Type = sortType;
                }
            }

            return true;
        }

        public void Sort(int columnIndex)
        {
            UpdateSortProperties();

            var ok = true;

            SortInformation info;
            if (columnIndex == -1)
            {
                ListViewItemSorter = new ListViewItemComparer(-1, currentSortInformation);
                info = currentSortInformation;
            }
            else
            {
                if (columnIndex < 0) ok = false;
                if (Columns.Count <= columnIndex) ok = false;

                info = sortProperties[columnIndex];
                if (info == null) ok = false;

                if (ok) ListViewItemSorter = new ListViewItemComparer(columnIndex, info);
            }

            if (ok)
            {
                Sort();
                if (info != null) info.Direction = info.Direction == SortDirection.Ascending ? 
                        SortDirection.Descending : SortDirection.Ascending;
            }
        }

        protected override void OnColumnClick(ColumnClickEventArgs e)
        {
            base.OnColumnClick(e);
            if (SortColumnOnClick) Sort(e.Column);
        }

        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            // Maybe use this to draw sort arrows on the column headers
            e.DrawDefault = true;
            base.OnDrawColumnHeader(e);
        }

        private void UpdateSortProperties()
        {
            if (sortProperties.Count >= Columns.Count) return;
            foreach (ColumnHeader ch in Columns)
            {
                if (!sortProperties.ContainsKey(ch.Index))
                    sortProperties.Add(ch.Index, new SortInformation());
            }
        }
    }
}

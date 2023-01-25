using System.Collections.ObjectModel;
using System.Globalization;
using Eto.Drawing;
using Eto.Forms;

namespace OpenTabletDriver.UX.Controls
{
    public class Sidebar : Panel
    {
        private int _suspended;
        private bool _isCreated;
        private SidebarItem? _selectedItem;
        private bool _expand;
        private Size _spacing;
        private int _itemHeight = 30;
        private int? _expandedWidth = 200;
        private int _defaultWidth = 50;

        public Collection<SidebarItem> Items { get; } = new Collection<SidebarItem>();

        public SidebarItem? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnSelectedItemChanged(EventArgs.Empty);
                }
            }
        }

        public bool Expand
        {
            get => _expand;
            set
            {
                if (_expand != value)
                {
                    _expand = value;
                    OnExpandChanged(EventArgs.Empty);
                }
            }
        }

		public Size Spacing
		{
            get => _spacing;
            set
			{
				if (_spacing != value)
				{
					_spacing = value;
                    if (Content is TableLayout table)
                        table.Spacing = _spacing;
                }
			}
		}

        public int DefaultWidth
        {
            get => _defaultWidth;
            set
            {
                if (_defaultWidth != value)
                {
                    _defaultWidth = value;
                    CreateIfNeeded(true);
                }
            }
        }

        public int ExpandedWidth
        {
            get => _expandedWidth ?? 0;
            set
            {
                if (_expandedWidth != value)
                {
                    _expandedWidth = value;
                    CreateIfNeeded(true);
                }
            }
        }

        public int ItemHeight
        {
            get => _itemHeight;
            set
            {
                if (_itemHeight != value)
                {
                    _itemHeight = value;
                    CreateIfNeeded(true);
                }
            }
        }

        public EventHandler? SelectedItemChanged;
        public EventHandler? ExpandChanged;

        public Sidebar(params SidebarItem[] items)
        {
            var itemCollection = new SidebarItemCollection(this);
            foreach (var item in items)
                itemCollection.Add(item);

            Items = itemCollection;
        }

		protected override void OnPreLoad(EventArgs e)
		{
			if (!_isCreated && _suspended <= 0)
				Create();
			base.OnPreLoad(e);
		}

		protected override void OnLoad(EventArgs e)
		{
			if (!_isCreated && _suspended <= 0)
				Create();
			base.OnLoad(e);
		}

        protected virtual void OnSelectedItemChanged(EventArgs e)
        {
            SelectedItemChanged?.Invoke(this, e);
        }

        protected virtual void OnExpandChanged(EventArgs e)
        {
            CreateIfNeeded(true);
            ExpandChanged?.Invoke(this, e);
        }

		public override void SuspendLayout()
		{
			base.SuspendLayout();
			_suspended++;
		}

		public override void ResumeLayout()
		{
			if (_suspended == 0)
				throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Must balance ResumeLayout with SuspendLayout calls"));
			_suspended--;
			base.ResumeLayout();
			CreateIfNeeded();
		}

        private void CreateIfNeeded(bool force = false)
        {
			if (_suspended > 0 || !Loaded)
			{
				if (force)
					_isCreated = false;
				return;
			}
			if (!_isCreated || force)
				Create();
        }

        private void Create()
        {
            var expanded = Expand;
            _isCreated = true;

            var tableRows = new TableRow[Items.Count + 1];

            if (expanded)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    var item = Items[i];
                    var sidebarBase = new TableCell
                    {
                        Control = new Panel
                        {
                            Content = item.Control,
                            Height = ItemHeight,
                            Width = DefaultWidth
                        }
                    };
                    var sidebarExpanded = new TableCell
                    {
                        Control = new Panel
                        {
                            Content = item.ExpandedControl,
                            Height = ItemHeight
                        }
                    };
                    var tableRow = new TableRow(sidebarBase, sidebarExpanded, null);
                    tableRows[i] = tableRow;
                }
                Width = ExpandedWidth;
            }
            else
            {
                Width = DefaultWidth;
                for (int i = 0; i < Items.Count; i++)
                {
                    var item = Items[i];
                    var sidebarBase = new TableCell
                    {
                        Control = new Panel
                        {
                            Content = item.Control,
                            Height = ItemHeight,
                            Width = DefaultWidth
                        }
                    };
                    var tableRow = new TableRow(sidebarBase);
                    tableRows[i] = tableRow;
                }
            }

            tableRows[^1] = new TableRow(new TableCell()); // prevent the last row from expanding

            var tableLayout = new TableLayout(tableRows)
            {
                Spacing = Spacing,
            };

            Content = tableLayout;
        }

        private class SidebarItemCollection : Collection<SidebarItem>
        {
            public Sidebar Parent { get; }

            public SidebarItemCollection(Sidebar sidebar)
            {
                Parent = sidebar;
            }

			protected override void InsertItem(int index, SidebarItem item)
			{
				base.InsertItem(index, item);
				Parent.CreateIfNeeded(true);
			}

			protected override void RemoveItem(int index)
			{
				base.RemoveItem(index);
				Parent.CreateIfNeeded(true);
			}

			protected override void ClearItems()
			{
				base.ClearItems();
				Parent.CreateIfNeeded(true);
			}

			protected override void SetItem(int index, SidebarItem item)
			{
				base.SetItem(index, item);
				Parent.CreateIfNeeded(true);
			}
        }
    }

    public class SidebarItem
    {
        public Control? Control { get; set; }
        public Control? ExpandedControl { get; set; }

        public int Spacing { get; set; } = 10;

        public SidebarItem(Control control, string label)
        {
            Control = control;
            ExpandedControl = new Label { Text = label, Wrap = WrapMode.None };
        }

        public SidebarItem(Control control)
        {
            Control = control;
        }

        public SidebarItem()
        {
        }

        public static implicit operator SidebarItem(Control control)
        {
            return new SidebarItem(control);
        }
    }
}

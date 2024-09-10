using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Controls
{

    public class PropertyData
    {

        public string? IconKey = null;
        public Color? IconColor;

        private int _Padding = 0;
        public int Padding
        {
            get => _Padding;
            set => _Padding = Math.Max(0, value);
        }

        public object? Tag { get; set; }
    }

    public class PropertyItem : ListViewItem
    {

        private PropertyData Data;

        public new bool UseItemStyleForSubItems
        {
            get => base.UseItemStyleForSubItems;
            private set => base.UseItemStyleForSubItems = value;
        }

        public new string ImageKey
        {
            get => base.ImageKey;
            private set => base.ImageKey = value;
        }

        public string? IconKey
        {
            get => Data.IconKey;
            set => Data.IconKey = value;
        }

        public Color? IconColor
        {
            get => Data.IconColor;
            set => Data.IconColor = value;
        }

        public int Padding
        {
            get => Data.Padding;
            set => Data.Padding = value;
        }

        public new object? Tag
        {
            get => Data.Tag;
            set => Data.Tag = value;
        }

        public PropertyItem()
        {
            UseItemStyleForSubItems = false;
            Data = new();
            base.Tag = Data;
        }
    }

    public class PropertySubItem : ListViewItem.ListViewSubItem
    {
        private PropertyData Data;

        public string? IconKey
        {
            get => Data.IconKey;
            set => Data.IconKey = value;
        }

        public Color? IconColor
        {
            get => Data.IconColor;
            set => Data.IconColor = value;
        }

        public int Padding
        {
            get => Data.Padding;
            set => Data.Padding = value;
        }

        public new object? Tag
        {
            get => Data.Tag;
            set => Data.Tag = value;
        }

        public PropertySubItem(ListViewItem? owner) : base(owner, "")
        {
            Data = new();
            base.Tag = Data;
        }
    }

    public class PropertiesList : ListView
    {

        public PropertiesList()
        {
            FullRowSelect = true;
            HeaderStyle = ColumnHeaderStyle.Nonclickable;
            MultiSelect = false;
            OwnerDraw = true;
            View = View.Details;

            DrawColumnHeader += Event_DrawColumnHeader;
            DrawItem += Event_DrawItem;
            DrawSubItem += Event_DrawSubItem;
        }

        private void Event_DrawColumnHeader(object? sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void Event_DrawItem(object? sender, DrawListViewItemEventArgs e)
        {

        }

        private void Event_DrawSubItem(object? sender, DrawListViewSubItemEventArgs e)
        {
            PropertyData data;
            Color backColor;
            if (e.ColumnIndex == 0)
            {
                data = (PropertyData)e.Item.Tag;
                backColor = e.Item.BackColor;
            }
            else
            {
                data = (PropertyData)e.SubItem.Tag;
                backColor = e.SubItem.BackColor;
            }

            var brush = new SolidBrush(backColor);
            e.Graphics.FillRectangle(brush, e.Bounds);


            Image? img = null;
            if (data.IconKey != null && SmallImageList?.Images != null)
            {
                img = SmallImageList.Images[data.IconKey];
            }

            Size imageSize = new Size(0, 0);
            if (img != null && SmallImageList != null)
            {
                imageSize = SmallImageList.ImageSize;
            }

            Size colorSize = new Size(0, 0);
            if (data.IconColor != null)
            {
                if (SmallImageList != null)
                {
                    colorSize = SmallImageList.ImageSize;
                }
                else
                {
                    colorSize = new Size(16, 16);
                }
            }

            bool selected = e.Item?.Selected ?? false;
            // optionally show selection
            if (selected)
            {
                e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
            }

            Rectangle rTgt = new(e.Bounds.Location, colorSize);
            rTgt.X += data.Padding;
            if (data.IconColor != null)
            {
                e.Graphics.FillRectangle(new SolidBrush(data.IconColor.Value), rTgt);
                rTgt.X += rTgt.Width + 2;
            }

            rTgt.Size = imageSize;
            if (img != null)
            {
                e.Graphics.DrawImage(img, rTgt);
                rTgt.X += rTgt.Width + 2;
            }

            // Optionally draw text
            if (e.SubItem != null)
            {
                e.Graphics.DrawString(e.SubItem.Text, e.SubItem.Font, selected ? SystemBrushes.HighlightText : SystemBrushes.WindowText, rTgt.X, e.Bounds.Y + 2);
            }
        }

        public PropertyData? SelectedItem
        {
            get
            {
                if (SelectedItems.Count > 0)
                {
                    return (PropertyData?)SelectedItems[0].Tag;
                }
                return null;
            }
        }

        public int? SelectedIndex
        {
            get
            {
                if (SelectedIndices.Count > 0)
                {
                    return SelectedIndices[0];
                }
                return null;
            }
            set
            {
                SelectedIndices.Clear();
                if (value != null && value >= 0 && value < Items.Count)
                {
                    SelectedIndices.Add(value.Value);
                }
            }
        }
    }
}

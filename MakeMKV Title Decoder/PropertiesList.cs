using MakeMKV_Title_Decoder.MkvToolNix.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder {

    public class PropertyData {

        public string? IconKey = null;
        public Color? IconColor;

        private int _Padding = 0;
        public int Padding {
            get => _Padding;
            set => _Padding = Math.Max(0, value);
        }

        public object? Tag { get; set; }
    }

    public class PropertyItem : ListViewItem {

        private PropertyData Data;

        public new bool UseItemStyleForSubItems { 
            get => base.UseItemStyleForSubItems;
            private set => base.UseItemStyleForSubItems = value; 
        }

        public new string ImageKey {
            get => base.ImageKey;
            private set => base.ImageKey = value;
        }

        public string? IconKey {
            get => this.Data.IconKey;
            set => this.Data.IconKey = value;
        }

        public Color? IconColor {
            get => this.Data.IconColor;
            set => this.Data.IconColor = value;
        }

        public int Padding {
            get => this.Data.Padding;
            set => this.Data.Padding = value;
        }

        public new object? Tag { 
            get => this.Data.Tag; 
            set => this.Data.Tag = value; 
        }

        public PropertyItem() {
            this.UseItemStyleForSubItems = false;
            this.Data = new();
            base.Tag = this.Data;
        }
    }

    public class PropertySubItem : ListViewItem.ListViewSubItem {
        private PropertyData Data;

        public string? IconKey {
            get => this.Data.IconKey;
            set => this.Data.IconKey = value;
        }

        public Color? IconColor {
            get => this.Data.IconColor;
            set => this.Data.IconColor = value;
        }

        public int Padding {
            get => this.Data.Padding;
            set => this.Data.Padding = value;
        }

        public new object? Tag {
            get => this.Data.Tag;
            set => this.Data.Tag = value;
        }

        public PropertySubItem(ListViewItem? owner) : base(owner, "") {
            this.Data = new();
            base.Tag = this.Data;
        }
    }

    public class PropertiesList : ListView {

        public PropertiesList() {
            this.FullRowSelect = true;
            this.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.MultiSelect = false;
            this.OwnerDraw = true;
            this.View = View.Details;

            this.DrawColumnHeader += Event_DrawColumnHeader;
            this.DrawItem += Event_DrawItem;
            this.DrawSubItem += Event_DrawSubItem;
        }

        private void Event_DrawColumnHeader(object? sender, DrawListViewColumnHeaderEventArgs e) {
            e.DrawDefault = true;
        }

        private void Event_DrawItem(object? sender, DrawListViewItemEventArgs e) {

        }

        private void Event_DrawSubItem(object? sender, DrawListViewSubItemEventArgs e) {
            PropertyData data;
            Color backColor;
            if (e.ColumnIndex == 0)
            {
                data = (PropertyData)e.Item.Tag;
                backColor = e.Item.BackColor;
            } else
            {
                data = (PropertyData)e.SubItem.Tag;
                backColor = e.SubItem.BackColor;
            }

            var brush = new SolidBrush(backColor);
            e.Graphics.FillRectangle(brush, e.Bounds);


            Image? img = null;
            if (data.IconKey != null && this.SmallImageList?.Images != null)
            {
                img = this.SmallImageList.Images[data.IconKey];
            }

            Size imageSize = new Size(0, 0);
            if (img != null && this.SmallImageList != null)
            {
                imageSize = this.SmallImageList.ImageSize;
            }

            Size colorSize = new Size(0, 0);
            if (data.IconColor != null)
            {
                if (this.SmallImageList != null)
                {
                    colorSize = this.SmallImageList.ImageSize;
                } else
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

        public PropertyData? SelectedItem {
            get
            {
                if (base.SelectedItems.Count > 0)
                {
                    return (PropertyData?)base.SelectedItems[0].Tag;
                }
                return null;
            }
        }

        public int? SelectedIndex {
            get
            {
                if (base.SelectedIndices.Count > 0)
                {
                    return base.SelectedIndices[0];
                }
                return null;
            }
            set
            {
                base.SelectedIndices.Clear();
                if (value != null && value >= 0 && value < base.Items.Count)
                {
                    base.SelectedIndices.Add(value.Value);
                }
            }
        }
    }
}

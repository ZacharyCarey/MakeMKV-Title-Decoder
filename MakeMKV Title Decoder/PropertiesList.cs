using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder {

    public class PropertyItem : ListViewItem {

        public new bool UseItemStyleForSubItems { 
            get => base.UseItemStyleForSubItems;
            private set => base.UseItemStyleForSubItems = value; 
        }

        public string IconKey {
            get => base.ImageKey;
            set => base.ImageKey = value;
        }

        public PropertyItem() {
            this.UseItemStyleForSubItems = false;
        }
    }

    public class PropertySubItem : ListViewItem.ListViewSubItem {
        public new object? Tag {
            get => base.Tag;
            private set => base.Tag = value;
        }

        public string IconKey {
            get => (string?)base.Tag ?? "";
            private set => base.Tag = value;
        }

        public PropertySubItem(ListViewItem? owner) : base(owner, "") {

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
            e.DrawDefault = true;
        }

        private void Event_DrawSubItem(object? sender, DrawListViewSubItemEventArgs e) {
            e.DrawBackground();

            Size sz = new Size(0, 0);
            if (this.SmallImageList != null)
            {
                sz = this.SmallImageList.ImageSize;
            }

            Image? img = null;
            if (e.SubItem?.Tag != null)
            {
                if (this.SmallImageList?.Images != null)
                {
                    img = this.SmallImageList.Images[(string)e.SubItem.Tag];
                }
            }

            bool selected = e.Item?.Selected ?? false; //e.ItemState.HasFlag(ListViewItemStates.Selected);
            // optionally show selection
            if (selected)
            {
                e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
            }
            if (img != null)
            {
                Rectangle rTgt = new(e.Bounds.Location, sz);
                e.Graphics.DrawImage(img, rTgt);
            }

            // Optionally draw text
            if (e.SubItem != null)
            {
                e.Graphics.DrawString(e.SubItem.Text, e.SubItem.Font, selected ? SystemBrushes.HighlightText : SystemBrushes.WindowText, e.Bounds.X + sz.Width + 2, e.Bounds.Y + 2);
            }
        }
    }
}

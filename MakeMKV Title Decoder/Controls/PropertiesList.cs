using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder.Controls
{
    public class PropertyData
    {
        public Color? BackColor { get; set; }
        public string? IconKey { get; set; }
        public Color? IconColor { get; set; }

        public int Padding { get; set; }

        public string Text { get; set; } = "";

        public bool IsSubItem { get; set; }

        public List<PropertyData> SubItems = new();

        public object? Tag = null;
    }

    public class PropertiesList : ListView
    {
        const int SubItemPadding = 25;

        public new IEnumerable<PropertyData> Items => GetItems();

        private IEnumerable<PropertyData> GetItems() {
            foreach(var item in base.Items)
            {
                yield return (PropertyData)((ListViewItem)item).Tag;
            }
        }

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
            if (e.ColumnIndex == 0)
            {
                data = (PropertyData)e.Item.Tag;
            }
            else
            {
                data = (PropertyData)e.SubItem.Tag;
            }

            Color backColor = this.BackColor;
            if (data.BackColor != null)
            {
                backColor = data.BackColor.Value;
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
            if (data.IsSubItem) rTgt.X += SubItemPadding;
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
            e.Graphics.DrawString(data.Text, this.Font, selected ? SystemBrushes.HighlightText : SystemBrushes.WindowText, rTgt.X, e.Bounds.Y + 2);
        }

        public void Add(PropertyData data, params PropertyData[] subData) {
            ListViewItem item = new();
            item.UseItemStyleForSubItems = false;
            item.Tag = data;
            data.SubItems.Clear();

            foreach(var sub in subData)
            {
                ListViewItem.ListViewSubItem subItem = new(item, null);
                subItem.Tag = sub;
                item.SubItems.Add(subItem);
                data.SubItems.Add(sub);
            }

            base.Items.Add(item);
        }

        public new void Clear() {
            base.Items.Clear();
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
            set
            {
                for (int i = 0; i < base.Items.Count; i++)
                {
                    if ((PropertyData?)base.Items[i].Tag == value)
                    {
                        this.SelectedIndex = i;
                        return;
                    }
                }
                this.SelectedIndex = -1;
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
                if (value != null && value >= 0 && value < base.Items.Count)
                {
                    SelectedIndices.Add(value.Value);
                }
            }
        }
    }
}

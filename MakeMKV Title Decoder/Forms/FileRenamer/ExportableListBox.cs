using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MakeMKV_Title_Decoder.Data;

namespace MakeMKV_Title_Decoder.Forms.FileRenamer {
    public class ExportableListItem {
        public Exportable Export;
        public string? Icon = null;

        public ExportableListItem(Exportable playlist) {
            this.Export = playlist;
        }

        public override string ToString() {
            return this.Export.Name ?? "?";
        }
    }

    public class ExportableListBox : ListBox {

        public ImageList? SmallIconList { get; set; } = null;

        public new ExportableListItem? SelectedItem {
            get
            {
                if (base.SelectedItem != null)
                {
                    return (ExportableListItem)base.SelectedItem;
                }
                return null;
            }

            set => base.SelectedItem = value;
        }

        public IEnumerable<ExportableListItem> AllItems => this.Items.Cast<ExportableListItem>();

        public ExportableListBox() {
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.DrawItem += LoadedPlaylistListBox_DrawItem;
        }

        public void Add(ExportableListItem item) {
            this.Items.Add(item);
        }

        public void Add(Exportable export) {
            this.Items.Add(new ExportableListItem(export));
        }

        public void Remove(ExportableListItem item) {
            this.Items.Remove(item);
        }

        public void Clear() {
            this.Items.Clear();
        }

        public ExportableListItem? FindItem(Exportable export) {
            foreach (var obj in this.Items)
            {
                ExportableListItem item = (ExportableListItem)obj;
                if (item.Export == export)
                {
                    return item;
                }
            }
            return null;
        }

        private void LoadedPlaylistListBox_DrawItem(object? sender, DrawItemEventArgs e) {
            Color backColor = e.BackColor;
            string text = "";
            string? icon = null;
            if (e.Index >= 0 && e.Index < this.Items.Count)
            {
                object? item = this.Items[e.Index];
                if (item != null && item is ExportableListItem itr)
                {
                    text = item.ToString() ?? "";
                    icon = itr.Icon;
                }
            }

            Image? image = null;
            Size imageSize = new Size(0, 0);
            if (this.SmallIconList?.Images != null && icon != null)
            {
                imageSize = this.SmallIconList.ImageSize;
                image = this.SmallIconList.Images[icon];
            }

            // DRAW!
            var brush = new SolidBrush(backColor);
            e.Graphics.FillRectangle(brush, e.Bounds);

            Point p = e.Bounds.Location;
            if (image != null)
            {
                e.Graphics.DrawImage(image, new Rectangle(p, imageSize));
                p.X += imageSize.Width + 2;
            }

            // Draw text, if any
            e.Graphics.DrawString(text, e.Font ?? this.Font, new SolidBrush(e.ForeColor), p.X, p.Y);

            e.DrawFocusRectangle();
        }
    }
}

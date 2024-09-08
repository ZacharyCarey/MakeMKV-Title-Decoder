using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MakeMKV_Title_Decoder.Forms.PlaylistCreator {

    public class LoadedPlaylistListItem {
        public LoadedPlaylist Playlist;
        public List<string> Icons = new();

        public LoadedPlaylistListItem(LoadedPlaylist playlist) {
            this.Playlist = playlist;
        }

        public override string ToString() {
            return this.Playlist.Name;
        }
    }

    public class LoadedPlaylistListBox : ListBox {

        public ImageList? SmallIconList { get; set; } = null;
            
        public new LoadedPlaylistListItem? SelectedItem {
            get
            {
                if (base.SelectedItem != null)
                {
                    return (LoadedPlaylistListItem)base.SelectedItem;
                }
                return null;
            }

            set => base.SelectedItem = value;
        }

        public IEnumerable<LoadedPlaylistListItem> AllItems => this.Items.Cast<LoadedPlaylistListItem>();

        public LoadedPlaylistListBox() {
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.DrawItem += LoadedPlaylistListBox_DrawItem;
        }

        public void Add(LoadedPlaylistListItem item) {
            this.Items.Add(item);
        }

        public void Add(LoadedPlaylist playlist) {
            this.Items.Add(new LoadedPlaylistListItem(playlist));
        }

        public void Remove(LoadedPlaylistListItem item) {
            this.Items.Remove(item);
        }

        public void Clear() {
            this.Items.Clear();
        }

        public LoadedPlaylistListItem? FindItem(LoadedPlaylist playlist) {
            foreach(var obj in this.Items)
            {
                LoadedPlaylistListItem item = (LoadedPlaylistListItem)obj;
                if (item.Playlist == playlist)
                {
                    return item;
                }
            }
            return null;
        }

        private void LoadedPlaylistListBox_DrawItem(object? sender, DrawItemEventArgs e) {
            Color backColor = e.BackColor;
            string text = "";
            List<string> icons = new();
            if (e.Index >= 0 && e.Index < this.Items.Count)
            {
                object? item = this.Items[e.Index];
                if (item != null && item is LoadedPlaylistListItem itr)
                {
                    text = item.ToString() ?? "";
                    icons = itr.Icons;
                }
            }

            List<Image> images = new();
            Size imageSize = new Size(0, 0);
            if (this.SmallIconList?.Images != null)
            {
                imageSize = this.SmallIconList.ImageSize;
                images = icons
                    .Select(x => this.SmallIconList.Images[x])
                    .Where(x => x != null)
                    .Cast<Image>()
                    .ToList();
            }

            // DRAW!
            var brush = new SolidBrush(backColor);
            e.Graphics.FillRectangle(brush, e.Bounds);

            Point p = e.Bounds.Location;
            foreach(var img in images)
            {
                e.Graphics.DrawImage(img, new Rectangle(p, imageSize));
                p.X += imageSize.Width + 2;
            }

            // Draw text, if any
            e.Graphics.DrawString(text, e.Font ?? this.Font, new SolidBrush(e.ForeColor), p.X, p.Y);

            e.DrawFocusRectangle();
        }
    }
}

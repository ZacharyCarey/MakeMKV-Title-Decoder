using MakeMKV_Title_Decoder.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MakeMKV_Title_Decoder
{
    public partial class VideoCompareForm : Form {

        const string KeepIconKey = "dialog-ok-apply.png";
        const string DeleteIconKey = "dialog-cancel.png";

        public readonly LoadedDisc Disc;

        public VideoCompareForm(LoadedDisc disc) {
            this.Disc = disc;

            InitializeComponent();

            this.VideoPlayerLeft.VlcViewer = VideoViewLeft;
            this.VideoPlayerLeft.LoadVLC();
            PopulateClipsList(ClipsListLeft);

            this.VideoPlayerRight.VlcViewer = VideoViewRight;
            this.VideoPlayerRight.VLC = this.VideoPlayerLeft.VLC;
            PopulateClipsList(ClipsListRight);
        }

        private void VideoCompareForm_Load(object sender, EventArgs e) {

        }

        private void VideoCompareForm_FormClosing(object sender, FormClosingEventArgs e) {
            this.VideoPlayerLeft.LoadVideo(null);
            this.VideoPlayerRight.LoadVideo(null);
        }

        private void PopulateClipsList(ListView list) {
            list.Items.Clear();
            foreach (var clip in Disc.Streams)
            {
                // TODO just create custom user control? public class ClipList : ListView
                ListViewItem keepIconItem = new("", KeepIconKey);
                keepIconItem.Tag = clip;
                list.Items.Add(keepIconItem);

                ListViewItem.ListViewSubItem sourceItem = new(keepIconItem, clip.Identity.SourceFile);
                keepIconItem.SubItems.Add(sourceItem);

                ListViewItem.ListViewSubItem renameItem = new(keepIconItem, "");
                keepIconItem.SubItems.Add(renameItem);

                RefreshClipListItem(keepIconItem, clip);
            }
        }

        private void RefreshClipListItem(ListViewItem row, LoadedStream data) {
            string name = (data.RenameData.Name ?? "");

            row.ImageKey = (string.IsNullOrWhiteSpace(name) ? DeleteIconKey : KeepIconKey);
            row.SubItems[2].Text = name;
        }

        private void SelectClip(VideoPlayer player, LoadedStream? clip) {
            if (clip == null) {
                player.LoadVideo(null);
            } else {
                string path = Path.Combine(this.Disc.Root, clip.Identity.SourceFile);
                player.LoadVideo(path);
            }
        }

        private void RefreshSelectedItem(ListView list, VideoPlayer player) {
            LoadedStream? selection = null;
            if (list.SelectedItems.Count > 0)
            {
                selection = (LoadedStream?)list.SelectedItems[0].Tag;
            }
            SelectClip(player, selection);
        }

        private void ClipsList_SelectedIndexChanged(object sender, EventArgs e) {
            RefreshSelectedItem(ClipsListLeft, VideoPlayerLeft);
            RefreshSelectedItem(ClipsListRight, VideoPlayerRight);
        }
    }
}

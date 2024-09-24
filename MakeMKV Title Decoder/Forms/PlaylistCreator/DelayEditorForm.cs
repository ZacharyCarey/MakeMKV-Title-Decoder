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
using static System.Windows.Forms.AxHost;

namespace MakeMKV_Title_Decoder.Forms.PlaylistCreator {
    public partial class DelayEditorForm : Form {

        public TrackDelay DelayInfo = new();

        public DelayEditorForm(LoadedPlaylist playlist, TrackDelay info) {
            InitializeComponent();

            // Load files from playlist
            foreach (var source in playlist.SourceFiles)
            {
                this.ClipList.Items.Add(new DropDownItem(source));
                foreach (var appended in source.AppendedFiles)
                {
                    this.ClipList.Items.Add(new DropDownItem(appended));
                }
            }

            if (info.ClipDelay == null)
            {
                this.DelayMSBox.Value = info.MillisecondDelay;
                ManualRadioBtn.Checked = true;
            } else
            {
                SetSelectedClip(info.ClipDelay);
                ClipSyncRadioBtn.Checked = true;
            }

            updateState();
            this.DialogResult = DialogResult.Cancel;
        }

        private void DelayEditorForm_Load(object sender, EventArgs e) {

        }

        private void DelayEditorForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (this.ClipSyncRadioBtn.Checked)
            {
                this.DelayInfo.ClipDelay = GetSelectedClip();
                this.DelayInfo.MillisecondDelay = 0;
            } else
            {
                this.DelayInfo.ClipDelay = null;
                this.DelayInfo.MillisecondDelay = (long)this.DelayMSBox.Value;
            }
        }

        private void SetSelectedClip(AppendedFile? file) {
            if (file == null)
            {
                this.ClipList.SelectedItem = null;
            }

            foreach (var obj in this.ClipList.Items)
            {
                if (obj != null && obj is DropDownItem item)
                {
                    if (item.File == file)
                    {
                        this.ClipList.SelectedItem = item;
                        return;
                    }
                }
            }

            this.ClipList.SelectedItem = null;
        }

        private AppendedFile? GetSelectedClip() {
            object? obj = this.ClipList.SelectedItem;
            if (obj != null && obj is DropDownItem item)
            {
                return item.File;
            } else
            {
                return null;
            }
        }

        private void updateState() {
            bool state = ClipSyncRadioBtn.Checked;
            this.ClipList.Enabled = state;
            this.DelayMSBox.Enabled = !state;

            if (state)
            {
                this.DelayInfo.ClipDelay = GetSelectedClip();
                this.DelayInfo.MillisecondDelay = 0;
            } else
            {
                this.DelayInfo.ClipDelay = null;
                this.DelayInfo.MillisecondDelay = (long)this.DelayMSBox.Value;
            }
        }

        private void ClipSyncRadioBtn_CheckedChanged(object sender, EventArgs e) {
            updateState();
        }

        private void ManualRadioBtn_CheckedChanged(object sender, EventArgs e) {
            updateState();
        }

        private void ApplyBtn_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.OK;
        }

        private void ClipList_DrawItem(object sender, DrawItemEventArgs e) {
            e.DrawBackground();
            e.DrawFocusRectangle();

            object? obj = (e.Index >= 0 ? ClipList.Items[e.Index] : null);
            if (obj != null)
            {
                Rectangle area = e.Bounds;

                if (obj is DropDownItem item)
                {
                    // Draw the colored square
                    int size = area.Height;
                    e.Graphics.FillRectangle(new SolidBrush(item.Color), area.Left, area.Top, size, size);
                    area.X += size;
                    area.Width -= size;
                }

                // Draw the name
                e.Graphics.DrawString(obj.ToString(), this.Font, new SolidBrush(this.ForeColor), area);
            }


        }

        private class DropDownItem {
            public AppendedFile File;

            public Color Color => File.Color;
            public string Name => File.Source.Rename.Name ?? "null";

            public DropDownItem(AppendedFile file) {
                this.File = file;
            }

            public override string ToString() {
                return Name;
            }
        }
    }
}

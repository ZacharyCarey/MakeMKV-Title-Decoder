using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.Data.Renames;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace MakeMKV_Title_Decoder.Forms
{
    public partial class AttachmentsForm : Form
    {
        const string KeepIconKey = "dialog-ok-apply.png";
        const string DeleteIconKey = "dialog-cancel.png";

        public readonly LoadedDisc Disc;
        Attachment? SelectedAttachment = null;

        private int NumSelectedAttachments => this.AttachmentsList.SelectedItems.Count;
        private IEnumerable<(ListViewItem Item, Attachment Stream)> AllSelectedAttachments {
            get
            {
                for (int i = 0; i < this.AttachmentsList.SelectedItems.Count; i++)
                {
                    Attachment? stream = (Attachment?)AttachmentsList.SelectedItems[i].Tag;
                    if (stream != null) yield return (AttachmentsList.SelectedItems[i], stream);
                }
            }
        }

        public AttachmentsForm(LoadedDisc disc)
        {
            this.Disc = disc;
            InitializeComponent();
            this.VideoPreview.LoadVLC();

            SelectAttachment(null);
            AttachmentsList.Items.Clear();
            foreach (var attachment in disc.RenameData.Attachments)
            {
                ListViewItem keepIconItem = new("", KeepIconKey);
                keepIconItem.Tag = attachment;
                this.AttachmentsList.Items.Add(keepIconItem);

                ListViewItem.ListViewSubItem sourceItem = new(keepIconItem, attachment.FilePath);
                keepIconItem.SubItems.Add(sourceItem);

                ListViewItem.ListViewSubItem renameItem = new(keepIconItem, attachment.Name ?? "");
                keepIconItem.SubItems.Add(renameItem);

                RefreshAttachmentsListItem(keepIconItem, attachment);
            }
        }

        private void AttachmentsForm_Load(object sender, EventArgs e)
        {

        }

        private void AttachmentsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.VideoPreview.LoadVideo(null);
        }

        private void SelectAttachment(Attachment? attachment)
        {
            this.SelectedAttachment = attachment;
            this.NameTextBox.Text = (attachment == null) ? "" : (attachment.Name ?? "");

            this.PropertiesPanel.Enabled = (attachment != null);
            if (attachment == null)
            {
                this.VideoPreview.LoadVideo(null);
            }
            else
            {
                string path = Path.Combine(this.Disc.Root, attachment.FilePath);
                this.VideoPreview.LoadVideo(path);
                this.VideoPreview.Play();
            }
        }

        private void RefreshAttachmentsListItem(ListViewItem row, Attachment data)
        {
            string name = (data.Name ?? "");

            row.ImageKey = (string.IsNullOrWhiteSpace(name) ? DeleteIconKey : KeepIconKey);
            row.SubItems[2].Text = name;
        }

        private void AttachmentsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AttachmentsList.SelectedItems.Count <= 0)
            {
                return;
            }

            Attachment? selection = (Attachment?)AttachmentsList.SelectedItems[0].Tag;
            SelectAttachment(selection);
        }

        private void ApplyBtn_Click(object sender, EventArgs e)
        {
            if (this.NumSelectedAttachments > 1)
            {
                string? name = this.NameTextBox.Text;
                if (string.IsNullOrWhiteSpace(name))
                {
                    name = null;
                }

                foreach((int index, (var item, var att)) in this.AllSelectedAttachments.WithIndex())
                {
                    string? n = name;
                    if (n != null)
                    {
                        n = n + $" {(index + 1)}";
                    }
                    att.Name = n;
                    RefreshAttachmentsListItem(item, att);
                }
            } else
            {
                Attachment? selection = this.SelectedAttachment;
                if (selection != null)
                {
                    string? name = this.NameTextBox.Text;
                    if (string.IsNullOrWhiteSpace(this.NameTextBox.Text))
                    {
                        name = null;
                    }

                    if (name != null)
                    {
                        string? err = Utils.IsValidFileName(name);
                        if (err != null)
                        {
                            MessageBox.Show("Invalid file name: " + name);
                            return;
                        }
                    }

                    selection.Name = name;
                    RefreshAttachmentsListItem(AttachmentsList.SelectedItems[0], selection);
                }
            }
        }

        private void NameTextBox_TextChanged(object sender, EventArgs e)
        {
            bool valid = (Utils.IsValidFileName(NameTextBox.Text) == null);
            if (valid)
            {
                NameTextBox.BackColor = SystemColors.Window;
            } else
            {
                NameTextBox.BackColor = Color.Red;
            }
        }
    }
}

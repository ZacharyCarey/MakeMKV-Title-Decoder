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

namespace MakeMKV_Title_Decoder.Forms
{

    public partial class CollectionsForm : Form
    {
        LoadedDisc Disc;
        Collection? SelectedCollection = null;

        public CollectionsForm(LoadedDisc disc)
        {
            this.Disc = disc;
            InitializeComponent();

            this.CollectionsListBox.Items.Clear();
            foreach (var collection in disc.RenameData.Collections)
            {
                this.CollectionsListBox.Items.Add(collection);
            }

            this.SourceList.Items.Clear();
            foreach (var attachment in disc.RenameData.Attachments)
            {
                this.SourceList.Items.Add(attachment);
            }

            this.AttachmentsListBox.Items.Clear();
            this.NameTextBox.Text = "";
        }

        private void AttachmentsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void SourceList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ApplyNameButton_Click(object sender, EventArgs e)
        {
            if (this.SelectedCollection != null)
            {
                string? err = Utils.IsValidFileName(this.NameTextBox.Text);
                if (err != null)
                {
                    MessageBox.Show("Invalid file name: " + err);
                    return;
                }

                this.SelectedCollection.Name = this.NameTextBox.Text;
                int index = this.CollectionsListBox.SelectedIndex;
                this.CollectionsListBox.Items[index] = this.CollectionsListBox.Items[index]; // Forces text to refresh
            }
        }

        private void DeleteCollectionButton_Click(object sender, EventArgs e)
        {
            if (this.SelectedCollection == null) return;

            if (MessageBox.Show("Can not be undone. Are you sure?", "Delete?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                this.Disc.RenameData.Collections.Remove(this.SelectedCollection);
                this.SelectedCollection = null;
                this.CollectionsListBox.Items.Clear();
                this.AttachmentsListBox.Items.Clear();
                this.NameTextBox.Text = "";
                foreach (var collection in this.Disc.RenameData.Collections)
                {
                    this.CollectionsListBox.Items.Add(collection);
                }
            }
        }

        private void AttachmentDeleteBtn_Click(object sender, EventArgs e)
        {
            if (this.SelectedCollection == null) return;

            if (this.AttachmentsListBox.SelectedItem != null && this.AttachmentsListBox.SelectedItem is Attachment selectedAttachment)
            {
                this.SelectedCollection.Attachments.Remove(selectedAttachment.FilePath);
                RefreshAttachments();
            }
        }

        private void SourceAddButton_Click(object sender, EventArgs e)
        {
            if (this.SelectedCollection == null) return;

            if (this.SourceList.SelectedItems.Count > 1)
            {
                for(int i = 0; i < this.SourceList.SelectedItems.Count; i++)
                {
                    if (this.SourceList.SelectedItems[i] != null && this.SourceList.SelectedItems[i] is Attachment source)
                    {
                        bool inList = false;
                        foreach(var attachment in this.SelectedCollection.Attachments)
                        {
                            if (attachment == source.FilePath)
                            {
                                inList = true;
                                break;
                            }
                        }
                        if (!inList)
                        {
                            // Attachment is not in the collection, add it
                            this.SelectedCollection.Attachments.Add(source.FilePath);
                            this.AttachmentsListBox.Items.Add(source);
                        }
                    }
                }
            } else
            {
                if (this.SourceList.SelectedItem != null && this.SourceList.SelectedItem is Attachment source)
                {
                    foreach (var attachment in this.SelectedCollection.Attachments)
                    {
                        if (attachment == source.FilePath)
                        {
                            MessageBox.Show("Attachment is already in the collection.");
                            return;
                        }
                    }

                    // Attachment is not in the collection, add it
                    this.SelectedCollection.Attachments.Add(source.FilePath);
                    this.AttachmentsListBox.Items.Add(source);
                }
            }
        }

        private void NameTextBox_TextChanged(object sender, EventArgs e)
        {
            bool valid = (Utils.IsValidFileName(NameTextBox.Text) == null);
            if ((this.SelectedCollection == null) || valid)
            {
                NameTextBox.BackColor = SystemColors.Window;
            }
            else
            {
                NameTextBox.BackColor = Color.Red;
            }
        }

        private void CollectionsListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.CollectionsListBox.SelectedItem != null && this.CollectionsListBox.SelectedItem is Collection collection)
            {
                this.SelectedCollection = collection;
                this.NameTextBox.Text = collection.Name;
                RefreshAttachments();
            }
            else
            {
                this.SelectedCollection = null;
                this.AttachmentsListBox.Items.Clear();
                this.NameTextBox.Text = "";
            }
        }

        private void RefreshAttachments()
        {
            this.AttachmentsListBox.Items.Clear();
            foreach (var attachmentPath in this.SelectedCollection.Attachments)
            {
                Attachment? attachment = null;
                foreach (var source in this.Disc.RenameData.Attachments)
                {
                    if (source.FilePath == attachmentPath)
                    {
                        attachment = source;
                        break;
                    }
                }
                if (attachment == null)
                {
                    MessageBox.Show("Critical error: failed to find atachment in loaded disc.");
                    this.SelectedCollection = null;
                    this.AttachmentsListBox.Items.Clear();
                    this.NameTextBox.Text = "";
                    this.CollectionsListBox.SelectedItem = null;
                    return;
                }

                AttachmentsListBox.Items.Add(attachment);
            }
        }

        private void NewPlaylistButton_Click(object sender, EventArgs e)
        {
            var collection = new Collection();
            this.Disc.RenameData.Collections.Add(collection);
            this.CollectionsListBox.Items.Add(collection);
        }
    }
}

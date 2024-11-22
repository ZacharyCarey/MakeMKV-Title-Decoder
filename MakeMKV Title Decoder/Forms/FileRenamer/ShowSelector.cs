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

namespace MakeMKV_Title_Decoder.Forms.FileRenamer
{
    public partial class ShowSelector : Form
    {
        private RenameData Renames;
        public ShowOutputName? Result = null;

        public ShowSelector(RenameData renameData)
        {
            this.Renames = renameData;
            InitializeComponent();

            this.DialogResult = DialogResult.None;
            foreach (var show in renameData.ShowOutputNames)
            {
                this.ShowsList.Items.Add(show);
            }

            this.DialogResult = DialogResult.Cancel;
        }

        private void AddBtn_Click(object sender, EventArgs e)
        {
            var showEditorForm = new ShowEditor(null);
            var result = showEditorForm.ShowDialog();
            if (result == DialogResult.OK && showEditorForm.Result != null)
            {
                this.Renames.ShowOutputNames.Add(showEditorForm.Result);
                this.ShowsList.Items.Add(showEditorForm.Result);
            }
        }

        private void EditBtn_Click(object sender, EventArgs e)
        {
            if (this.ShowsList.SelectedItem != null && this.ShowsList.SelectedItem is ShowOutputName selectedShow)
            {
                var showEditorForm = new ShowEditor(selectedShow);
                var result = showEditorForm.ShowDialog();
                if (result == DialogResult.OK && showEditorForm.Result != null)
                {
                    selectedShow.SetValues(showEditorForm.Result);
                }
            }
        }

        private void SelectBtn_Click(object sender, EventArgs e)
        {
            if (this.ShowsList.SelectedItem != null && this.ShowsList.SelectedItem is ShowOutputName selectedShow)
            {
                this.Result = selectedShow;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void ShowsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.DialogResult != DialogResult.OK)
            {
                this.Result = null;
            }
        }

        private void ShowSelector_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (this.ShowsList.SelectedItem != null && this.ShowsList.SelectedItem is ShowOutputName selectedShow)
            {
                var result = MessageBox.Show("Are you sure?", "Delete?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    int index = this.Renames.ShowOutputNames.IndexOf(selectedShow);
                    this.Renames.ShowOutputNames.Remove(selectedShow);
                    foreach(var playlist in this.Renames.Playlists)
                    {
                        if (playlist.OutputFile.ShowIndex == index)
                        {
                            playlist.OutputFile.ShowIndex = -1;
                        } else if(playlist.OutputFile.ShowIndex > index)
                        {
                            playlist.OutputFile.ShowIndex--;
                        }
                    }
                }
            }
        }
    }
}

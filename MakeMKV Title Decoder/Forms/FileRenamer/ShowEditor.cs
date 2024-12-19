using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.Data.Renames;
using MakeMKV_Title_Decoder.Forms.TmdbBrowser;
using MakeMKV_Title_Decoder.libs.MakeMKV.Data;
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
    public partial class ShowEditor : Form {
        public ShowOutputName? Result = null;

        public ShowEditor(ShowOutputName? initialValues, LoadedDisc disc) {
            InitializeComponent();

            TypeComboBox.Items.Clear();
            foreach (var type in Enum.GetValues<ShowType>())
            {
                TypeComboBox.Items.Add(type);
            }

            this.DialogResult = DialogResult.None;

            this.TypeComboBox.SelectedItem = initialValues?.Type;
            this.IdTextBox.Text = initialValues?.TmdbID.ToString() ?? "";
            this.ShowNameTextBox.Text = initialValues?.Name ?? "";
            this.MetadataTextBox.Text = initialValues?.MetadataFileName ?? disc.Identity.GetSafeDiscName();

            ShowNameTextBox_TextChanged(null, null);
            MetadataTextBox_TextChanged(null, null);
        }

        private void ShowNameTextBox_TextChanged(object sender, EventArgs e) {
            string? nameError = Utils.IsValidFileName(this.ShowNameTextBox.Text);
            this.ErrorText.Text = nameError ?? "";
            this.ErrorText.Visible = (nameError != null);
        }

        private bool TrySaveData() {
            if (TypeComboBox.SelectedItem == null || !(TypeComboBox.SelectedItem is ShowType))
            {
                MessageBox.Show("Please select a show type or select the TMDB page.");
                return false;
            }

            long id;
            if (!long.TryParse(IdTextBox.Text, out id))
            {
                MessageBox.Show("Invalid ID.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(this.ShowNameTextBox.Text))
            {
                MessageBox.Show("Name can't be empty.");
                return false;
            }

            string? fileNameError = Utils.IsValidFileName(this.ShowNameTextBox.Text);
            if (fileNameError != null)
            {
                MessageBox.Show("Invalid file name: " + fileNameError);
                return false;
            }

            string? metadataNameError = Utils.IsValidFileName(this.MetadataTextBox.Text);
            if (metadataNameError != null || string.IsNullOrWhiteSpace(this.MetadataTextBox.Text))
            {
                MessageBox.Show("Invalid metadata file name: " + (metadataNameError ?? "Name can't be empty."));
                return false;
            }

            this.Result = new ShowOutputName((ShowType)TypeComboBox.SelectedItem, id, this.ShowNameTextBox.Text, this.MetadataTextBox.Text);

            return true;
        }

        private void SaveBtn_Click(object sender, EventArgs e) {
            if (TrySaveData())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void CancelBtn_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void ShowEditor_FormClosing(object sender, FormClosingEventArgs e) {
            if (this.DialogResult == DialogResult.None)
            {
                var result = MessageBox.Show("Would you like to save your changes?", "Save changes?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (!TrySaveData())
                    {
                        e.Cancel = true;
                        this.Result = null;
                        return;
                    }
                } else if (result == DialogResult.No)
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Result = null;
                    return;
                } else
                {
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void TmdbBtn_Click(object sender, EventArgs e) {
            TmdbID? defaultID = null;
            long temp;
            if (this.TypeComboBox.SelectedItem != null && this.TypeComboBox.SelectedItem is ShowType type && long.TryParse(this.IdTextBox.Text, out temp))
            {
                defaultID = new TmdbID(type, temp);
            }

            var form = new TmdbBrowserForm(/*!FirstTmdb*/false, false, false, defaultID);
            //FirstTmdb = false;
            if (form.ShowDialog() == DialogResult.OK)
            {
                var ID = form.ID;
                if (ID != null)
                {
                    this.TypeComboBox.SelectedItem = ID.Type;
                    this.IdTextBox.Text = ID.ID.ToString();
                }
            }
        }

        private void MetadataTextBox_TextChanged(object sender, EventArgs e) {
            string? nameError = Utils.IsValidFileName(this.MetadataTextBox.Text);
            if (nameError == null && string.IsNullOrWhiteSpace(this.MetadataTextBox.Text))
            {
                nameError = "Name can't be null";
            }
            this.MetadataTextBox.BackColor = ((nameError != null) ? Color.Red : SystemColors.Window);
        }
    }
}

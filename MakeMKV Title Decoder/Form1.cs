using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.Data.BluRay;
using MakeMKV_Title_Decoder.Data.DVD;
using MakeMKV_Title_Decoder.Forms;
using MakeMKV_Title_Decoder.Forms.DiscBackup;
using MakeMKV_Title_Decoder.Forms.FileRenamer;
using MakeMKV_Title_Decoder.libs.MakeMKV.Data;
using PgcDemuxLib;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Utils;
using static System.Windows.Forms.Design.AxImporter;

namespace MakeMKV_Title_Decoder
{
    public partial class Form1 : Form
    {

        LoadedDisc? Disc = null;

        public Form1()
        {
            InitializeComponent();

            LoadedDiscLabel.Text = "";
            DiscNameLabel.Text = "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ConsoleLog.CreateLogger("Log.txt");
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*Dvd? dvd = null;
            using (FolderBrowserDialog openFileDialog = new FolderBrowserDialog())
            {
                openFileDialog.InitialDirectory = "C:\\Users\\Zack\\Downloads\\WILLY_WONKA";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    dvd = Dvd.ParseFolder(openFileDialog.SelectedPath);
                }
            }

            if (dvd == null)
            {
                MessageBox.Show($"Failed to parse disc data.", "Failed to read MkvToolNix", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }*/

            /*using (SaveFileDialog dialog = new())
            {
                dialog.Filter = "JSON files (*.json)|*.json";
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var path = dialog.FileName;
                    if (!dvd.SaveToFile(path))
                    {
                        MessageBox.Show("Failed to save JSON.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }*/
            /*using (FolderBrowserDialog openFileDialog = new FolderBrowserDialog())
            {
                openFileDialog.InitialDirectory = "C:\\Users\\Zack\\Downloads\\TestOutput";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    dvd.TitleSets[0].DemuxTitleCell(openFileDialog.SelectedPath, 1, 1);
                }
            }*/
        }

        private void viewInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog openFileDialog = new FolderBrowserDialog())
            {
                openFileDialog.InitialDirectory = "F:\\Video\\backup";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    LoadedDisc? disc = null;
                    try
                    {
                        disc = LoadedDisc.TryLoadDisc(openFileDialog.SelectedPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"There was an error reading the disc.: {ex.Message}", "Failed to read MkvToolNix", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (disc == null)
                    {
                        MessageBox.Show($"Failed to parse disc data.", "Failed to read MkvToolNix", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    new ViewInfoForm(disc).ShowDialog();
                }
            }
        }

        private void LoadDiscBtn_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog openFileDialog = new FolderBrowserDialog())
            {
                openFileDialog.InitialDirectory = "F:\\Video\\backup";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    LoadedDisc? parsedDisc = null;
                    try
                    {
                        parsedDisc = TaskProgressViewerForm.Run((IProgress<SimpleProgress> progress) => {
                            return LoadedDisc.TryLoadDisc(openFileDialog.SelectedPath, progress);
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"There was an error reading the disc.: {ex.Message}", "Failed to read MkvToolNix", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (parsedDisc == null)
                    {
                        MessageBox.Show($"Failed to parse disc data.", "Failed to read MkvToolNix", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    this.Disc = parsedDisc;
                }
            }

            LoadedDiscLabel.Text = this.Disc?.Root ?? "";
            DiscNameLabel.Text = this.Disc?.Identity?.GetSafeDiscName() ?? "";
        }

        private void RenameClipsBtn_Click(object sender, EventArgs e)
        {
            if (Disc == null)
            {
                MessageBox.Show("Please load a disc first.");
                return;
            }
            new ClipRenamerForm(this.Disc).ShowDialog();
        }

        private void PlaylistsBtn_Click(object sender, EventArgs e)
        {
            if (this.Disc != null)
            {
                new PlaylistCreatorForm(this.Disc).ShowDialog();
            }
        }

        private void FileRenamerBtn_Click(object sender, EventArgs e)
        {
            if (this.Disc != null)
            {
                new FileRenamerForm(this.Disc).ShowDialog();
            }
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.Disc == null)
            {
                MessageBox.Show("Please load a disc first.", "Please load disc.");
                return;
            }

            using (SaveFileDialog dialog = new())
            {
                dialog.Filter = "JSON files (*.json)|*.json";
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var path = dialog.FileName;
                    try
                    {
                        var options = new JsonSerializerOptions { WriteIndented = true, TypeInfoResolver = new DefaultJsonTypeInfoResolver() };
                        using var stream = File.Create(path);
                        JsonSerializer.Serialize(stream, this.Disc.RenameData, options);
                        stream.Flush();
                        stream.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Failed to save JSON: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Disc == null)
            {
                MessageBox.Show("Please load a disc before loading rename data.", "Please load disc");
                return;
            }

            using (OpenFileDialog dialog = new())
            {
                dialog.Filter = "JSON files (*.json)|*.json";
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    using (var stream = dialog.OpenFile())
                    {
                        RenameData? rename;

                        try
                        {
                            rename = JsonSerializer.Deserialize<RenameData>(stream);
                            Console.WriteLine("Loaded JSON file.");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Failed to read file: " + ex.Message, "Failed to read file.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        if (rename == null)
                        {
                            MessageBox.Show("Failed to read rename file.", "Failed to read file.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        if (!this.Disc.TryLoadRenameData(rename))
                        {
                            MessageBox.Show("Failed to match rename data to the disc");
                            return;
                        }
                    }
                }
            }
        }

        private void BackupBtn_Click(object sender, EventArgs e)
        {
            new DiscBakupForm().Show();
        }

        private void ExportButton_Click(object sender, EventArgs e)
        {
            // TODO using FileRename data, execute MkvToolNix to export files
            throw new NotImplementedException();
        }

        private void AttachmentsBtn_Click(object sender, EventArgs e)
        {
            if (Disc == null)
            {
                MessageBox.Show("Please load a disc first.");
                return;
            }
            new AttachmentsForm(this.Disc).ShowDialog();
        }

        private void CollectionsBtn_Click(object sender, EventArgs e)
        {
            if (Disc == null)
            {
                MessageBox.Show("Please load a disc first.");
                return;
            }
            new CollectionsForm(this.Disc).ShowDialog();
        }
    }
}

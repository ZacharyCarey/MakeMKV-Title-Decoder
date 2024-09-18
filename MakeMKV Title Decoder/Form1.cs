using JsonSerializable;
using MakeMKV_Title_Decoder.Data;
using MakeMKV_Title_Decoder.Forms.DiscBackup;
using MakeMKV_Title_Decoder.Forms.FileRenamer;
using MakeMKV_Title_Decoder.libs.MkvToolNix;
using MakeMKV_Title_Decoder.libs.MkvToolNix.Data;

namespace MakeMKV_Title_Decoder
{
	public partial class Form1 : Form
	{

		LoadedDisc? disc = null;
		RenameData renames = new();

		public Form1()
		{
			InitializeComponent();

			LoadedDiscLabel.Text = "";
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			ConsoleLog.CreateLogger("Log.txt");
		}

		private void testToolStripMenuItem_Click(object sender, EventArgs e)
		{

			//using (FolderBrowserDialog openFileDialog = new FolderBrowserDialog())
			//{
			//	openFileDialog.InitialDirectory = "F:\\Video\\backup";

			//	if (openFileDialog.ShowDialog() == DialogResult.OK)
			//	{
			//		var playlist = new Playlist
			//		{
			//			Title = "RWBY Test",
			//			Files = new() {
			//				new PlaylistFile() {
			//					Source = "BDMV\\STREAM\\00004.m2ts",
			//					Tracks = new() {
			//						new PlaylistTrack() {
			//							ID = 0,
			//							Copy = true,
			//							Sync = new(){ },
			//							Name = "Wassup",
			//							Commentary = false
			//						},
			//						new PlaylistTrack() {
			//							ID = 1,
			//							Copy = true,
			//							Sync = new(){ },
			//							Name = "IM IMMUNE",
			//							Commentary = false
			//						},
			//						new PlaylistTrack() {
			//							ID = 2,
			//							Copy = true,
			//							Sync = new(){ },
			//							Commentary = false
			//						},
			//						new PlaylistTrack() {
			//							ID = 3,
			//							Copy = false,
			//							Sync = new(){ },
			//							Commentary = false
			//						}
			//					}
			//				},
			//				new PlaylistFile() {
			//					Source = "BDMV\\STREAM\\00015.m2ts",
			//					Tracks = new() {
			//						new PlaylistTrack() {
			//							ID = 0,
			//							Copy = true,
			//							Sync = new(){ },
			//							Commentary = false,
			//							AppendedTo = new() {
			//								StreamIndex = 0,
			//								TrackIndex = 0
			//							}
			//						},
			//						new PlaylistTrack() {
			//							ID = 1,
			//							Copy = true,
			//							Sync = new(){ },
			//							Commentary = false,
			//							AppendedTo = new() {
			//								StreamIndex = 0,
			//								TrackIndex = 1
			//							}
			//						},
			//						new PlaylistTrack() {
			//							ID = 2,
			//							Copy = false,
			//							Sync = new(){ },
			//							Commentary = false
			//						},
			//						new PlaylistTrack() {
			//							ID = 3,
			//							Copy = false,
			//							Sync = new(){ },
			//							Commentary = false
			//						}
			//					},
			//				},
			//				new PlaylistFile() {
			//					Source = "BDMV\\STREAM\\00004.m2ts",
			//					Tracks = new() {
			//						new PlaylistTrack() {
			//							ID = 0,
			//							Copy = true,
			//							Sync = new(){ },
			//							Commentary = false,
			//							AppendedTo = new() {
			//								StreamIndex = 1,
			//								TrackIndex = 0
			//							}
			//						},
			//						new PlaylistTrack() {
			//							ID = 1,
			//							Copy = true,
			//							Sync = new(){ },
			//							Commentary = false,
			//							AppendedTo = new() {
			//								StreamIndex = 1,
			//								TrackIndex = 1
			//							}
			//						},
			//						new PlaylistTrack() {
			//							ID = 2,
			//							Copy = true,
			//							Sync = new(){
			//								new TrackID() {
			//									StreamIndex = 1,
			//									TrackIndex = 2
			//								}
			//							},
			//							Commentary = false,
			//							AppendedTo = new() {
			//								StreamIndex = 0,
			//								TrackIndex = 2
			//							}
			//						},
			//						new PlaylistTrack() {
			//							ID = 3,
			//							Copy = false,
			//							Sync = new(){ },
			//							Commentary = false
			//						}
			//					}
			//				}
			//			}
			//		};

			//		MkvToolNixInterface.MergeAsync(this.disc, playlist, Path.Combine(openFileDialog.SelectedPath, "Test.mkv"));
			//	}
			//}
		}

		private void viewInfoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			using (FolderBrowserDialog openFileDialog = new FolderBrowserDialog())
			{
				openFileDialog.InitialDirectory = "F:\\Video\\backup";

				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					MkvToolNixDisc? disc = null;
					try
					{
						disc = MkvToolNixDisc.OpenAsync(openFileDialog.SelectedPath);
					} catch (Exception ex)
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
					MkvToolNixDisc? parsedDisc;
					try
					{
						parsedDisc = MkvToolNixDisc.OpenAsync(openFileDialog.SelectedPath);
					} catch (Exception ex)
					{
						MessageBox.Show($"There was an error reading the disc.: {ex.Message}", "Failed to read MkvToolNix", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}

					if (parsedDisc == null)
					{
						MessageBox.Show($"Failed to parse disc data.", "Failed to read MkvToolNix", MessageBoxButtons.OK, MessageBoxIcon.Error);
						return;
					}

					LoadedDisc? loadedDisc = LoadedDisc.TryLoadDisc(parsedDisc);
					if (loadedDisc == null)
					{
						// TODO likely due to duration parsing
						MessageBox.Show("Failed to load disc data.");
						return;
					}

					this.disc = loadedDisc;
					this.renames = new();
				}
			}

			LoadedDiscLabel.Text = this.disc?.Disc?.RootPath ?? "";
		}

		private void RenameClipsBtn_Click(object sender, EventArgs e)
		{
			if (disc == null)
			{
				MessageBox.Show("Please load a disc first.");
				return;
			}
			new ClipRenamerForm(disc).ShowDialog();
			this.disc.Save(this.renames); // TODO allow canceling changes
		}

		private void PlaylistsBtn_Click(object sender, EventArgs e)
		{
			if (this.disc != null)
			{
				new PlaylistCreatorForm(this.disc, this.renames).ShowDialog();
			}
		}

		private void FileRenamerBtn_Click(object sender, EventArgs e)
		{
			if (this.disc != null)
			{
				new FileRenamerForm(this.disc.Disc, this.renames).ShowDialog();
			}
		}

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			using (SaveFileDialog dialog = new())
			{
				dialog.Filter = "JSON files (*.json)|*.json";
				dialog.RestoreDirectory = true;

				if (dialog.ShowDialog() == DialogResult.OK)
				{
					var path = dialog.FileName;
					try
					{
						File.Delete(path);
					} catch (Exception) { }

					try
					{
						Json.Write(this.renames, path);
						Console.WriteLine("Saved JSON file.");
					} catch (Exception ex)
					{
						MessageBox.Show("Failed to save JSON: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					}
				}
			}
		}

		private void loadToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.disc == null)
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
						RenameData data = new();

						try
						{
							Json.Read(stream, data);
							Console.WriteLine("Loaded JSON file.");
						} catch (Exception ex)
						{
							MessageBox.Show("Failed to read file: " + ex.Message, "Failed to read file.", MessageBoxButtons.OK, MessageBoxIcon.Error);
							return;
						}

						LoadedDisc? loadedDisc = LoadedDisc.TryLoadRenameData(this.disc.Disc, data);
						if (loadedDisc == null)
						{
							MessageBox.Show("Failed to match rename data to loaded disc");
							return;
						}

						this.disc = loadedDisc;
						this.renames = data;
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
	}
}

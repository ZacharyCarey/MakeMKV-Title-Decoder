using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MakeMKV_Title_Decoder.Forms.TmdbBrowser {

    public partial class TmdbBrowserForm : Form {
        const string TmdbHome = "https://www.themoviedb.org/";

        private static Uri? LastPage = null;

        public TmdbID? ID = null;
        private bool RequireSeason;
        private bool RequireEpisode;

        public TmdbBrowserForm(bool restoreLastPage, bool requireSeason, bool requireEpisode) {
            InitializeComponent();

            if (restoreLastPage && LastPage != null)
            {
                WebView1.Source = LastPage;
            } else {
                WebView1.Source = new Uri(TmdbHome);
            }
            
            this.DialogResult = DialogResult.None;
            this.RequireSeason = requireSeason;
            this.RequireEpisode = requireEpisode;
        }

        private void TmdbBrowserForm_Load(object sender, EventArgs e) {

        }

        private TmdbID? ParseID() {
            //https://www.themoviedb.org/movie/533535-deadpool-wolverine?language=en-US

            string uri = WebView1.Source.ToString();
            if (!uri.StartsWith(TmdbHome)) return null;
            uri = uri.Substring(TmdbHome.Length);

            string[] urls = uri.Split("/");
            if (urls.Length < 2) return null;

            ShowType type;
            switch (urls[0])
            {
                case "movie":
                    type = ShowType.Movie;
                    break;
                case "tv":
                    type = ShowType.TV;
                    break;
                default:
                    return null;
            }

            string? idText = null;
            for (int i = 0; i < urls[1].Length; i++)
            {
                if (!char.IsAsciiDigit(urls[1][i]))
                {
                    idText = urls[1].Substring(0, i);
                    break;
                }
            }
            if (idText == null) idText = urls[1];

            long id;
            if (!long.TryParse(idText, out id))
            {
                return null;
            }

            if (urls.Length < 4 || urls[2] != "season")
            {
                return new TmdbID(type, id, null, null);
            } 

            long season;
            if (!long.TryParse(urls[3], out season))
            {
                return new TmdbID(type, id, null, null);
            }

            if (urls.Length < 6 || urls[4] != "episode")
            {
                return new TmdbID(type, id, season, null);
            }

            string? episodeText = null;
            for (int i = 0; i < urls[5].Length; i++)
            {
                if (!char.IsAsciiDigit(urls[5][i]))
                {
                    episodeText = urls[5].Substring(0, i);
                    break;
                }
            }
            if (episodeText == null) episodeText = urls[5];

            long episode;
            if (!long.TryParse(episodeText, out episode))
            {
                return new TmdbID(type, id, season, null);
            }

            return new TmdbID(type, id, season, episode);
        }

        private void SelectButton_Click(object sender, EventArgs e) {
            TmdbID? id = ParseID();
            if (id == null)
            {
                MessageBox.Show("Could not find TMDB ID");
                return;
            }

            List<string> missing = new();
            if (RequireSeason && id.Season == null)
            {
                missing.Add("season");
            }

            if (RequireEpisode && id.Episode == null)
            {
                missing.Add("episode");
            }

            if (missing.Count > 0)
            {
                MessageBox.Show($"Could not find {string.Join(", ", missing)}");
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.ID = id;

            this.Close();
        }

        private void WebView1_SourceChanged(object sender, Microsoft.Web.WebView2.Core.CoreWebView2SourceChangedEventArgs e) {
            LastPage = WebView1.Source;

            TmdbID? id = ParseID();
            this.IdLabel.Text = $"ID: {id?.ID.ToString() ?? ""}";
            this.TypeLabel.Text = $"Type: {id?.Type.ToString() ?? ""}";
            this.SeasonLabel.Text = $"Season: {id?.Season.ToString() ?? ""}";
            this.EpisodeLabel.Text = $"Episode: {id?.Episode.ToString() ?? ""}";
        }
    }
}

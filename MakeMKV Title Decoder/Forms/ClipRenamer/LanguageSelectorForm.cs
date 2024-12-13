using Iso639;
using MakeMKV_Title_Decoder.Util;
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

namespace MakeMKV_Title_Decoder.Forms.ClipRenamer
{
    public partial class LanguageSelectorForm : Form
    {
        public Language? SelectedLanguage { get; private set; }

        public LanguageSelectorForm()
        {
            InitializeComponent();

            SearchTextBox_TextChanged(null, null);

            this.DialogResult = DialogResult.Cancel;
        }

        private void SelectBtn_Click(object sender, EventArgs e)
        {
            if (this.LanguageList.SelectedItem != null && this.LanguageList.SelectedItem is LanguageWrapper selectedLang)
            {
                this.SelectedLanguage = selectedLang.Language;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            IEnumerable<Language> languages;
            if (string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                List<Language> sorted = new(Language.Database.Where(lang => lang.Part2 != null));
                sorted.Sort((x, y) => x.Name.CompareTo(y.Name));
                languages = sorted;
            } else
            {

                List<(Language Lang, int Distance)> distances = new();
                //IEnumerable<Language> database = Language.FromName(SearchTextBox.Text, true).Where(lang => lang.Part2 != null);
                IEnumerable<Language> database = Language.Database.Where(lang => lang.Part2 != null);
                foreach (Language language in database) {
                    int distance = Utils.LevenshteinDistance(SearchTextBox.Text, language.Name);
                    distances.Add((language, distance));
                }
                distances.Sort((x, y) => x.Distance.CompareTo(y.Distance));
                languages = distances.Select(x => x.Lang);
            }

            this.LanguageList.Items.Clear();
            foreach(var lang in languages)
            {
                this.LanguageList.Items.Add(new LanguageWrapper(lang));
            }
        }

        // Unfortunately the language nuget package I decided on doesn't
        // override .ToString(), so I created this wrapper class to
        // do it
        private class LanguageWrapper {

            public Language Language;
            
            public LanguageWrapper(Language lang) {
                this.Language = lang;
            }

            public override string ToString() {
                return $"{Language.Name} - ({Language.Part2})";
            }
        }
    }
}

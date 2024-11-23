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

namespace MakeMKV_Title_Decoder.Forms.ClipRenamer
{
    public partial class LanguageSelectorForm : Form
    {
        public string? SelectedLanguageCode { get; private set; }

        public LanguageSelectorForm()
        {
            InitializeComponent();

            SearchTextBox_TextChanged(null, null);

            this.DialogResult = DialogResult.Cancel;
        }

        private void SelectBtn_Click(object sender, EventArgs e)
        {
            if (this.LanguageList.SelectedItem != null && this.LanguageList.SelectedItem is LanguageCode selectedLang)
            {
                this.SelectedLanguageCode = selectedLang.Code;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            IEnumerable<LanguageCode> languages;
            if (string.IsNullOrEmpty(SearchTextBox.Text))
            {
                List<LanguageCode> sorted = new(Languages.GetAllLanguages());
                sorted.Sort((x, y) => x.Name.CompareTo(y.Name));
                languages = sorted;
            } else
            {
                List<(LanguageCode Language, int Distance)> distances = new();
                foreach (LanguageCode language in Languages.GetAllLanguages()) {
                    int distance = Utils.LevenshteinDistance(SearchTextBox.Text, language.Name);
                    distances.Add((language, distance));
                }
                distances.Sort((x, y) => x.Distance.CompareTo(y.Distance));
                languages = distances.Select(x => x.Language);
            }

            this.LanguageList.Items.Clear();
            foreach(var lang in languages)
            {
                this.LanguageList.Items.Add(lang);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentificationTests {
    internal struct TitleGroup : IEnumerable<int> {
        public HashSet<int> Titles;

        public IEnumerator<int> GetEnumerator() => Titles.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Titles.GetEnumerator();

        public TitleGroup() {
            Titles = new();
        }

        public TitleGroup(IEnumerable<int> titles) {
            this.Titles = new(titles);
        }

        public void Add(int value) {
            Titles.Add(value);
        }
    }

    internal struct Feature : IEnumerable<TitleGroup> {
        public List<TitleGroup> Episodes;

        public IEnumerator<TitleGroup> GetEnumerator() => Episodes.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Episodes.GetEnumerator();

        public Feature() {
            this.Episodes = new();
        }

        public Feature(IEnumerable<TitleGroup> episodes) {
            this.Episodes = new(episodes);
        }

        public void Add(TitleGroup group) {
            this.Episodes.Add(group);
        }
    }

    internal abstract class MovieDescription {

        /// <summary>
        /// The name of the JSOn file containing scrape data.
        /// </summary>
        public abstract string ScrapeName { get; }

        /// <summary>
        /// Only one of which will be chosen. Others must be deselected
        /// </summary>
        public abstract TitleGroup AcceptableMainFeatures { get; }

        /// <summary>
        /// Only used for movies.
        /// The episodes, as they should be listed, in order
        /// </summary>
        public abstract List<List<TitleGroup>> Episodes { get; }

        /// <summary>
        /// Features that should be deselected automatically
        /// </summary>
        public abstract HashSet<int> Deselected { get; }

        /// <summary>
        /// Prefer to be deselected, but not an immediate fail as they are obvious what they are
        /// but may be difficult to discern from a bonus feature.
        /// Mostly just to keep a record of tracks
        /// </summary>
        public abstract HashSet<int> Optional { get; }

        /// <summary>
        /// Features that must stay selected and not be in the deselected titles
        /// </summary>
        public abstract HashSet<int> BonusFeatures { get; }

        // Note: Any other titles can be either checked or unchecked.

    }
}

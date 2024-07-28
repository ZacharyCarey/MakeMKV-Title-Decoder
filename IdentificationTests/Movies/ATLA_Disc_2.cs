using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentificationTests.Movies {
    internal class ATLA_Disc_2 : MovieDescription {

        public override string ScrapeName => "ATLA Disc 2.json";

        public override TitleGroup AcceptableMainFeatures => new() {
            // Only one, by far the longest
            55
        };

        public override List<List<TitleGroup>> Episodes => new() {
            new() { new(){63}, new(){75}, new(){46}, new(){76}, new() {47}, new() {77}, new() {49}, new() {78}, new() {50}, new() {79}, new() {51}, new() {80}, new() {52}, new() {81}, new() {53}}, // Episodes group #1
            new() { new() {48, 63}, new() {46, 56}, new() {47, 57}, new() {49, 58}, new() {50, 59}, new() {51, 60}, new() {52, 61}, new() {53, 62} }
        };

        public override HashSet<int> Deselected => new() {
            // Gross federal warnings that don't even have audio
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
            10, 11, 12, 13, 14, 15, 16, 16, 17, 19,
            20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
            30, 31, 32, 33, 34, 35, 36, 37, 38, 39,
            40, 41, 42, 43, 44, 45,
            54, 69, 70 // Menu splash screens? no audio

        };

        public override HashSet<int> Optional => new() {
            // More splash screens. Have audio and video, but no subtitles or number of audio tracks like main feature
            64, 65, 66, 74, // nickelodeon splash screens
            69, 70, // some foreight gov warning screen
        };

        public override HashSet<int> BonusFeatures => new() {
            // Beind the scenes
            72, 73, 82
        };

    }
}

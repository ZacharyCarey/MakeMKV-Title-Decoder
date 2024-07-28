using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentificationTests.Movies {
    internal class ATLA_DISC_1 : MovieDescription{

        public override string ScrapeName => "ATLA Disc 1.json";

        public override TitleGroup AcceptableMainFeatures => new() {
            // Only one, by far the longest
            55
        };

        public override List<List<TitleGroup>> Episodes => new() {
            new() { new() {64}, new() {79}, new() {47}, new() {80}, new() {48}, new() {81}, new() {49}, new() {82}, new() {50}, new() {83}, new() {51}, new() {84}, new() {52}, new() {85}, new() {53} },
            new() { new() {46,64}, new() {47,57}, new() {48,58}, new() {49,59}, new() {50,60}, new() {51,61}, new() {52, 62}, new() {53,63} }
        };

        public override HashSet<int> Deselected => new() {
            // Gross federal warnings that don't even have audio
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
            10, 11, 12, 13, 14, 15, 16, 16, 17, 19,
            20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
            30, 31, 32, 33, 34, 35, 36, 37, 38, 39,
            40, 41, 42, 43, 44, 45,
            54, 68, 69,// Menu splash screens? no audio
            56 // Bonus feature that can be broken into titles 72, 73, 74, 75
        };

        public override HashSet<int> Optional => new() {
            // More splash screens. Have audio and video, but no subtitles or number of audio tracks like main feature
            65, 66, 67, // Nickelodeon
            70, 71, // Some foreign gov warning
            76, // Credits
        };

        public override HashSet<int> BonusFeatures => new() {
            72, 73, 74, 75, 77
        };

    }
}

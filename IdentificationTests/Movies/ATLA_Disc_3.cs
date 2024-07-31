using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentificationTests.Movies {
    internal class ATLA_Disc_3 : MovieDescription {
        public override string ScrapeName => "ATLA Disc 3.json";

        public override TitleGroup AcceptableMainFeatures => new() {
            // Only one, by far the longest
            51
        };

        public override List<List<TitleGroup>> Episodes => new() {
            new() { new(){46, 52}, new(){47, 53}, new(){48, 54}, new(){49, 55}  } // Episodes group #1
        };

        public override HashSet<int> Deselected => new() {
            // Gross federal warnings that don't even have audio
            0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
            10, 11, 12, 13, 14, 15, 16, 16, 17, 19,
            20, 21, 22, 23, 24, 25, 26, 27, 28, 29,
            30, 31, 32, 33, 34, 35, 36, 37, 38, 39,
            40, 41, 42, 43, 44, 45,
            63, 64, // Menu splash screens? no audio

            // Main menu image, no audio
            50,
        };

        public override HashSet<int> Optional => new() {
            // More splash screens. Have audio and video, but no subtitles or numer of audio tracks like main feature
            60, 61, 62, // nickelodeon splash screens
            65, 66, // some foreight gov warning screen
        };

        public override HashSet<int> BonusFeatures => new() {
            // Commentary. This one (56) is tricky because the video segments match the main feature (52)
            56, 57, 58, 59,

            // Behind the scenes
            67, 68, 69, 70
        };
    }
}

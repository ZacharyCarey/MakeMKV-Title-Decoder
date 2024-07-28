using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentificationTests.Movies {
    internal class RWBY_VOL_1 : MovieDescription {
        public override string ScrapeName => "RWBY_VOL_1.json";

        public override TitleGroup AcceptableMainFeatures => new() {
            // Only one, by far the longest
            0
        };

        public override List<List<TitleGroup>> Episodes => new() {
            new() { new() {18}, new() {19}, new() {20}, new() {21}, new() {22}, new() {23}, new() {24}, new() {25}, new() {26}, new() {27} }
        };

        public override HashSet<int> Deselected => new() {
            // Playlist for character intros. Can be deconstructed into smaller parts
            2,

            //Playlist for bonus features. Can be deconstructed into smaller parts
            3, 4, 5,

            // Both intro variants. Can be deconstructed into smaller parts
            6,

            // Credits for every episode. Each has a 'unique' background before the intro plays.
            // Can be deconstructed into smaller parts
            7, 8, 9, 10, 11, 12, 13, 14
        };

        public override HashSet<int> Optional => new() {
            // Warnings w/ RT logo.
            15,

            // Menu background
            17,

            // Intro w/ credits overlayed
            28,

            // Credits for each episode
            14, 15, 16, 17, 18, 19, 20, 21,

            52, // intro, no credits
        };

        public override HashSet<int> BonusFeatures => new() {
            16, // More from Rooster Teeth
            37, // Behind the scenes
            38, // RWBY Cosplay
            39, // red trailer
            40, // white trailer
            41, // black trailer
            42, // yellow trailer
            43, // r1b season 11
            44, // Best of rvb
            45, // Best of RT shorts
            46, //RT animated adventires
            47, //Failed of the Weak: halo edition
            48, // Slow mo guys
            49, // A simple walk
            50, // Fan art
            51, // intro storyboard
            53, // ursa the grimm
            55, // Episode 1 storyboard
        };

    }
}

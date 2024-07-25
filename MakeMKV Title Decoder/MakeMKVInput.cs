using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder {

    internal struct TitleInfo {
        public bool Audio = false;
        public bool Video = false;
        public bool Subtitles = false;
        public bool Attachment = false;

        public TitleInfo() {

        }

        public bool MeetsRequirements(TitleInfo requirements) {
            // Requirement means we must have that value, but if requirement is false it doesnt matter what the value is
            return (!requirements.Audio || this.Audio)
                && (!requirements.Video || this.Video)
                && (!requirements.Subtitles || this.Subtitles)
                && (!requirements.Attachment || this.Attachment);
        }
    }

    internal class MakeMKVInput {
        // TODO until I can add support for scrolling
        const int MaxTitles = 56;
        const int KeyTimingInMilliseconds = 50;
        const bool verbose = false;

        readonly Size WindowSize = new Size(2560, 1440);
        readonly Point FirstTitleLocation = new Point(70, 135);
        readonly Point TitleInfoTextBoxLocation = new Point(1920, 463);
        readonly Point OutputFolderLocation = new Point(1320, 140);
        const int TitleDeltaY = 17;
        const int TitleCheckBoxX = 50;
        const int DropdownX = 29;

        Dictionary<int, int> quickLookup = new();
        int maxIndex = -1;

        public int CurrentIndex { get; private set; } = 0;

        public string ReadOutputFolder() {
            MoveMouse(OutputFolderLocation);
            ClickMouse();
            CtrlA();
            CtrlC();
            return ReadClipboard();
        }

        public void FocusMKV() {
            MoveMouse(FirstTitleLocation);
            ClickMouse();
            CurrentIndex = 0;
            if (verbose) Console.WriteLine($"Input: reset to index=0");
        }

        /*public void ResetCursor(List<Title> allTitles) {
            int resetCount = CurrentIndex + 10;
            if(verbose) Console.WriteLine($"Resetting by scrolling up {resetCount} times.");
            maxIndex = -1;
            ScrollUp(resetCount);
            ScrollDown(1); // Get off the root node
            CurrentIndex = 0;
            if (verbose) Console.WriteLine("Reset. New index=0");

            quickLookup.Clear();
            int index = -1;
            for (int i = 0; i < allTitles.Count; i++)
            {
                index++;
                quickLookup[i] = index;
                index += allTitles[i].Tracks.Count();
            }
            maxIndex = index;
        }*/

        // Special function called by the scraper to get into a known state
        public void SetTitles(List<Title> allTitles) {
            quickLookup.Clear();
            int index = -1;
            for (int i = 0; i < allTitles.Count; i++)
            {
                index++;
                quickLookup[i] = index;
                index += allTitles[i].Tracks.Count();
            }
            maxIndex = index;
            this.CurrentIndex = maxIndex;
        }

        // NOTE: Forces scroll and breaks known state
        public void ScrollDown(int count) {
            CurrentIndex += count;
            if (maxIndex >= 0) Debug.Assert(CurrentIndex >= 0 && CurrentIndex <= maxIndex, "Scrolled out of bounds.");
            if (verbose) Console.WriteLine($"Input: scroll down {count} counts. New index={CurrentIndex}");
            while (count > 0)
            {
                DownArrow();
                count--;
            }
        }

        // NOTE: Forces scroll and breaks known state
        public void ScrollUp(int count) {
            CurrentIndex -= count;
            if (maxIndex >= 0) Debug.Assert(CurrentIndex >= 0 && CurrentIndex <= maxIndex, "Scrolled out of bounds.");
            if (verbose) Console.WriteLine($"Input: scroll up {count} counts. New index={CurrentIndex}");
            while (count > 0)
            {
                UpArrow();
                count--;
            }
        }

        public void ScrollTo(Title title) {
            ScrollTo(title, 0);
        }

        private void ScrollTo(Title title, int offset) {
            ScrollTo(title.Index, offset);
        }

        private void ScrollTo(int titleIndex, int offset) {
            int index = quickLookup[titleIndex] + offset;
            if (verbose) Console.WriteLine($"Input: scroll from current={CurrentIndex} to target={index}");
            int delta = index - CurrentIndex;
            if (delta >= 0)
            {
                if (verbose) Console.WriteLine($"\tScroll down x{delta}");
                ScrollDown(delta);
            } else
            {
                if (verbose) Console.WriteLine($"\tScroll up x{-delta}");
                ScrollUp(-delta);
            }
            //currentIndex += delta;
            //Console.WriteLine($"\tnew index={currentIndex}");
        }

        // Will auto scroll to desired title
        public void ToggleAttachment(Title titleInfo) {
            int offset = titleInfo.Tracks.WithIndex().First(x => x.Value == TrackType.Attachment).Index + 1; // The +1 is to reach the first track

            //OpenDropdown();
            ScrollTo(titleInfo, offset); 
            Space();
            //ScrollUp(dist + 1);
            //CloseDropdown();

            // Sanity check
            //Debug.Assert(CurrentIndex == titleInfo.Index, "Index matching failed.");
        }

        public void ToggleTitleSelection(Title title) {
            ScrollTo(title);
            Space();
        }

        public void OpenDropdown() {
            RightArrow();
        }

        public void CloseDropdown() {
            LeftArrow();
        }

        public string CopyTitleInformation() {
            MoveMouse(TitleInfoTextBoxLocation);
            ClickMouse();
            CtrlA();
            CtrlC();
            string data = ReadClipboard();
            Tab();
            Tab();
            return data;
        }

        // Note: this leaves the input in a bit of an unknown state when finished
        public void CollapseAll() {
            ScrollTo(0, 0);
            CloseDropdown();
            for (int i = 1; i < quickLookup.Count; i++)
            {
                DownArrow();
                CloseDropdown();
            }
        }


        private void MoveMouse(Point loc) {
            Cursor.Position = loc;
        }

        private void ClickMouse() {
            mouse_event((int)MouseEventFlags.LeftDown, Cursor.Position.X, Cursor.Position.Y, 0, 0);
            Thread.Sleep(KeyTimingInMilliseconds);
            mouse_event((int)MouseEventFlags.LeftUp, Cursor.Position.X, Cursor.Position.Y, 0, 0);
            Thread.Sleep(KeyTimingInMilliseconds);
        }

        private void PressKey(string keys, bool shift, bool ctrl, bool alt) {
            string open = "";
            string close = "";
            if (shift || ctrl || alt)
            {
                open = "(";
                close = ")";
            }
            string shiftMod = (shift ? "+" : "");
            string ctrlMod = (ctrl ? "^" : "");
            string altMod = (alt ? "%" : "");
            //SetForegroundWindow(makeMKV.MainWindowHandle);
            string keyCombo = shiftMod + ctrlMod + altMod + open + keys + close;
            SendKeys.Send(keyCombo);
            Thread.Sleep(KeyTimingInMilliseconds);
        }

        private void CtrlA() {
            PressKey("a", false, true, false);
        }

        private void CtrlC() {
            PressKey("c", false, true, false);
        }

        private void LeftArrow() {
            PressKey("{LEFT}", false, false, false);
        }

        private void RightArrow() {
            PressKey("{RIGHT}", false, false, false);
        }

        private void UpArrow() {
            PressKey("{UP}", false, false, false);
        }

        private void DownArrow() {
            PressKey("{DOWN}", false, false, false);
        }

        private void Tab() {
            PressKey("{TAB}", false, false, false);
        }

        private void Space() {
            PressKey(" ", false, false, false);
        }

        private string ReadClipboard() {
            if (!Clipboard.ContainsText(TextDataFormat.Text))
            {
                throw new Exception("Clipboard did not contain any text data as expected.");
            }

            return Clipboard.GetText(TextDataFormat.Text);
        }

        [Flags]
        private enum MouseEventFlags {
            LeftDown = 0x00000002,
            LeftUp = 0x00000004,
            MiddleDown = 0x00000020,
            MiddleUp = 0x00000040,
            Move = 0x00000001,
            Absolute = 0x00008000,
            RightDown = 0x00000008,
            RightUp = 0x00000010
        }

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hWnd);
    }
}

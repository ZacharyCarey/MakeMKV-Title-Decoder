using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder {
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

        int currentIndex = 0;
        Dictionary<int, int> attachmentDistance = new();

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
            currentIndex = 0;
            if (verbose) Console.WriteLine($"Input: reset to index=0");
        }

        public void ResetCursor() {
            int resetCount = currentIndex + 10;
            if(verbose) Console.WriteLine($"Resetting by scrolling up {resetCount} times.");
            ScrollUp(resetCount);
            ScrollDown(1); // Get off the root node
            currentIndex = 0;
            if (verbose) Console.WriteLine("Reset. New index=0");
        }

        // NOTE: Forces scroll and breaks known state
        public void ScrollDown(int count) {
            currentIndex += count;
            if (verbose) Console.WriteLine($"Input: scroll down {count} counts. New index={currentIndex}");
            while (count > 0)
            {
                DownArrow();
                count--;
            }
        }

        // NOTE: Forces scroll and breaks known state
        public void ScrollUp(int count) {
            currentIndex -= count;
            if (verbose) Console.WriteLine($"Input: scroll up {count} counts. New index={currentIndex}");
            while (count > 0)
            {
                UpArrow();
                count--;
            }
        }

        public void ScrollTo(int index) {
            if (verbose) Console.WriteLine($"Input: scroll from current={currentIndex} to target={index}");
            int delta = index - currentIndex;
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

        // Expensive. Has to search for the attachment field
        public void ToggleAttachment() {
            if (!attachmentDistance.ContainsKey(currentIndex))
            {
                OpenDropdown();

                int distance = 0;
                while (true)
                {
                    DownArrow();
                    distance++;
                    string data = CopyTitleInformation();
                    if (data.StartsWith("Attachment information"))
                    {
                        Space();
                        break;
                    } else if (data.StartsWith("Title information"))
                    {
                        UpArrow();
                        distance = -1;
                        break;
                    }
                }
                attachmentDistance[currentIndex] = distance;
                LeftArrow();
                LeftArrow();
            } else
            {
                int distance = attachmentDistance[currentIndex];
                if (distance >= 0)
                {
                    OpenDropdown();
                    ScrollDown(distance);
                    Space();
                    LeftArrow();
                    LeftArrow();
                }
            }
        }

        public void ToggleTitleSelection() {
            Space();
        }

        private void OpenDropdown() {
            RightArrow();
        }

        private void CloseDropdown() {
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

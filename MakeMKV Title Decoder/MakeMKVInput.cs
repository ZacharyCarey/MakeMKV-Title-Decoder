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

        readonly Size WindowSize = new Size(2560, 1440);
        readonly Point FirstTitleLocation = new Point(70, 135);
        readonly Point TitleInfoTextBoxLocation = new Point(1920, 463);
        const int TitleDeltaY = 17;
        const int TitleCheckBoxX = 50;
        const int DropdownX = 29;

        // TODO is this needed?
        //Process makeMKV;


        public void SelectTitle(int index) {
            if (index > MaxTitles + 1)
            {
                throw new Exception($"Can't select titles past {MaxTitles} until scrolling is implemented.");
            }

            Point titleLocation = FirstTitleLocation;
            titleLocation.Y += (TitleDeltaY * index);
            MoveMouse(titleLocation);
            ClickMouse();
        }

        public void CheckUncheckTitle(int index) {
            if (index > MaxTitles + 1)
            {
                throw new Exception($"Can't select titles past {MaxTitles} until scrolling is implemented.");
            }

            Point titleLocation = FirstTitleLocation;
            titleLocation.Y += (TitleDeltaY * index);
            titleLocation.X = TitleCheckBoxX;
            MoveMouse(titleLocation);
            ClickMouse();
        }

        public void OpenDropdown(int index) {
            if (index > MaxTitles + 1)
            {
                throw new Exception($"Can't select titles past {MaxTitles} until scrolling is implemented.");
            }

            Point loc = FirstTitleLocation;
            loc.Y += (TitleDeltaY * index);
            loc.X = DropdownX;
            MoveMouse(loc);
            ClickMouse();
        }

        public string CopyTitleInformation() {
            MoveMouse(TitleInfoTextBoxLocation);
            ClickMouse();
            CtrlA();
            CtrlC();
            return ReadClipboard();
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
            SendKeys.SendWait(keyCombo);
        }

        private void CtrlA() {
            PressKey("a", false, true, false);
            Thread.Sleep(KeyTimingInMilliseconds);
        }

        private void CtrlC() {
            PressKey("c", false, true, false);
            Thread.Sleep(KeyTimingInMilliseconds);
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

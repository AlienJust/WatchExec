using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using System.Windows.Forms;
using InputManager;
using System.Runtime.InteropServices;
using System.Drawing;

namespace WatchMan
{
    public class WatchManager
    {
        bool doActionOnce;

        /// <summary>
        /// Global CpManager всегда создается с флагом doActionOnlyOnStateChange=false
        /// </summary>
        /// <param name="doActionOnlyOnStateChange">Флаг необходимости действия только по факту смены состояния</param>
        public WatchManager(bool doActionOnlyOnStateChange)
        {
            this.doActionOnce = doActionOnlyOnStateChange;

            this.bgwProceedImageAsync.DoWork += new DoWorkEventHandler(bgwProceedImageAsync_DoWork);
            this.bgwProceedImageAsync.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgwProceedImageAsync_RunWorkerCompleted);
        }
        private Keys GetKeyByStr(string key)
        {
            Keys keystroke = Keys.CapsLock;
            #region numbers
            if (key == "D0")
            {
                keystroke = Keys.D0;
            }
            else if (key == "D1")
            {
                keystroke = Keys.D1;
            }
            else if (key == "D2")
            {
                keystroke = Keys.D2;
            }
            else if (key == "D3")
            {
                keystroke = Keys.D3;
            }
            else if (key == "D4")
            {
                keystroke = Keys.D4;
            }
            else if (key == "D5")
            {
                keystroke = Keys.D5;
            }
            else if (key == "D6")
            {
                keystroke = Keys.D6;
            }
            else if (key == "D7")
            {
                keystroke = Keys.D7;
            }
            else if (key == "D8")
            {
                keystroke = Keys.D8;
            }
            else if (key == "D9")
            {
                keystroke = Keys.D9;
            }
            #endregion numbers
            #region numlock
            else if (key == "N0")
            {
                keystroke = Keys.NumPad0;
            }
            else if (key == "N1")
            {
                keystroke = Keys.NumPad1;
            }
            else if (key == "N2")
            {
                keystroke = Keys.NumPad2;
            }
            else if (key == "N3")
            {
                keystroke = Keys.NumPad3;
            }
            else if (key == "N4")
            {
                keystroke = Keys.NumPad4;
            }
            else if (key == "N5")
            {
                keystroke = Keys.NumPad5;
            }
            else if (key == "N6")
            {
                keystroke = Keys.NumPad6;
            }
            else if (key == "N7")
            {
                keystroke = Keys.NumPad7;
            }
            else if (key == "N8")
            {
                keystroke = Keys.NumPad8;
            }
            else if (key == "N9")
            {
                keystroke = Keys.NumPad9;
            }
            #endregion numlock
            #region functionals
            if (key == "F1")
            {
                keystroke = Keys.F1;
            }
            else if (key == "F2")
            {
                keystroke = Keys.F2;
            }
            else if (key == "F3")
            {
                keystroke = Keys.F3;
            }
            else if (key == "F4")
            {
                keystroke = Keys.F4;
            }
            else if (key == "F5")
            {
                keystroke = Keys.F5;
            }
            else if (key == "F6")
            {
                keystroke = Keys.F6;
            }
            else if (key == "F7")
            {
                keystroke = Keys.F7;
            }
            else if (key == "F8")
            {
                keystroke = Keys.F8;
            }
            else if (key == "F9")
            {
                keystroke = Keys.F9;
            }
            else if (key == "F10")
            {
                keystroke = Keys.F10;
            }
            else if (key == "F11")
            {
                keystroke = Keys.F11;
            }
            else if (key == "F12")
            {
                keystroke = Keys.F12;
            }
            #endregion functionals
            #region specials
            else if (key.ToUpper() == "LSHIFT")
            {
                keystroke = Keys.LShiftKey;
            }
            else if (key.ToUpper() == "RSHIFT")
            {
                keystroke = Keys.RShiftKey;
            }
            else if (key.ToUpper() == "LCTRL")
            {
                keystroke = Keys.LControlKey;
            }
            else if (key.ToUpper() == "RCTRL")
            {
                keystroke = Keys.RControlKey;
            }
            else if (key.ToUpper() == "ESC")
            {
                keystroke = Keys.Escape;
            }
            else if (key.ToUpper() == "SPACE")
            {
                keystroke = Keys.Space;
            }
            else if (key.ToUpper() == "INS")
            {
                keystroke = Keys.Insert;
            }
            else if (key.ToUpper() == "DEL")
            {
                keystroke = Keys.Delete;
            }
            #endregion specials
            return keystroke;
        }
        private Mouse.MouseKeys GetMouseButtonByStr(string key)
        {
            Mouse.MouseKeys button = Mouse.MouseKeys.Left;
            switch (key)
            {
                case "Right":
                    button = Mouse.MouseKeys.Right;
                    break;
                case "Left":
                    button = Mouse.MouseKeys.Left;
                    break;
                case "Middle":
                    button = Mouse.MouseKeys.Middle;
                    break;
            }
            return button;
        }
        //--------------------------------------------------------------
        private bool previousStateIsDiffer = true;
        private bool previousColorNotFound = false;

        private object thisLock = new object();
        private bool asyncProcessingInProgress;

        /// <summary>
        /// Потокобезопасный флаг проверки на выполнение действия в данный момент
        /// </summary>
        public bool AsyncProcessingInProgress
        {
            get
            {
                bool result = false;
                lock (thisLock)
                {
                    result = asyncProcessingInProgress;
                }
                return result;
            }
            private set
            {
                lock (thisLock)
                {
                    asyncProcessingInProgress = value;
                }
            }
        }
        public void ProceedImage(Bitmap bitmap, Rectangle bitmapRect, string traceColor, string actionSeen, string actionLost)//doActionOnlyOnStateChange - не передается, т.к. GlobalCpManager не сможет отслеживать нить для каждого отдельного источника
        {
            //требуется ли действие?
            bool colorNotFound = true;
            Color color;
            int bitmapW;
            int bitmapH;
            int deltaColor = 0;
            lock (thisLock)
            {
                string[] splittedStr = traceColor.Split('~');
                if (splittedStr.Length == 2)
                {
                    traceColor = splittedStr[0];
                    deltaColor = int.Parse(splittedStr[1]);
                }

                if (traceColor.StartsWith("#"))
                {
                    color = ColorTranslator.FromHtml(traceColor);
                }
                else
                {
                    color = Color.FromArgb(int.Parse(traceColor));
                }
                
                

                bitmapW = bitmap.Size.Width;
                bitmapH = bitmap.Size.Height;
            }
            for (int x = 1; x < bitmapW - 1; x++)//parallel can be used
            {
                for (int y = 1; y < bitmapH - 1; y++)
                {
                    Color pointColor;
                    lock (thisLock)
                    {
                        pointColor = bitmap.GetPixel(x, y);
                    }
                    if (ColorsAreTheSame(pointColor, color, deltaColor))
                    {
                        colorNotFound = false;
                        break;
                    }
                }
            }
            bool needAction = false;
            lock (thisLock)
            {
                if (doActionOnce)
                {
                    if (previousColorNotFound != colorNotFound) previousStateIsDiffer = true;
                    else previousStateIsDiffer = false;

                    previousColorNotFound = colorNotFound;
                }
                needAction = !doActionOnce || doActionOnce && previousStateIsDiffer;
            }

            if (needAction)
            {
                string action = "";
                if (colorNotFound)
                {
                    lock (thisLock)
                    {
                        action = actionLost;
                        //bitmap.Save("D:\\Scr\\Lost.bmp");
                    }
                }
                else
                {
                    lock (thisLock)
                    {
                        action = actionSeen;
                        //bitmap.Save("D:\\Scr\\Seen.bmp");
                    }
                }
                DoAction(action, bitmapRect);
            }

        }
        private bool ColorsAreTheSame(Color c1, Color c2, int dc)
        {
            bool result = false;
            if ((c1.R <= c2.R + dc && c1.G <= c2.G + dc && c1.B <= c2.B + dc) &&
                (c1.R >= c2.R - dc && c1.G >= c2.G - dc && c1.B >= c2.B - dc))
            {
                result = true;
            }
            return result;
        }
        private void DoAction(string actions, Rectangle bitmapRect)
        {
            string[] actionRaw = actions.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in actionRaw)
            {
                string[] parts = line.Split('=');
                if (parts.Length == 2)
                {
                    if (parts[0] == "delay") System.Threading.Thread.Sleep(int.Parse(parts[1]));
                    else if (parts[0] == "kDn") Keyboard.KeyDown(this.GetKeyByStr(parts[1]));
                    else if (parts[0] == "kUp") Keyboard.KeyUp(this.GetKeyByStr(parts[1]));
                    else if (parts[0] == "mAbs") // 
                    {
                        string[] coord = parts[1].Split(',');
                        int x = int.Parse(coord[0]);
                        int y = int.Parse(coord[1]);
                        InputManager.Mouse.Move(x, y);
                    }
                    else if (parts[0] == "mRel") // relative to current mouse position
                    {
                        string[] coord = parts[1].Split(',');
                        int x = int.Parse(coord[0]);
                        int y = int.Parse(coord[1]);
                        InputManager.Mouse.MoveRelative(x, y);
                    }
                    else if (parts[0] == "mRrc") // relative to rectangle center coords
                    {
                        string[] coord = parts[1].Split(',');
                        int x = bitmapRect.X + bitmapRect.Width / 2 + int.Parse(coord[0]);
                        int y = bitmapRect.Y + bitmapRect.Height / 2 + int.Parse(coord[1]);
                        InputManager.Mouse.Move(x, y);
                    }
                    else if (parts[0] == "mDn") InputManager.Mouse.ButtonDown(GetMouseButtonByStr(parts[1]));
                    else if (parts[0] == "mUp") InputManager.Mouse.ButtonUp(GetMouseButtonByStr(parts[1]));
                }
            }
        }

        BackgroundWorker bgwProceedImageAsync = new BackgroundWorker();
        public void ProceedImageAsync(Bitmap bitmap, Rectangle bitmapRect, string traceColor, string actionSeen, string actionLost)
        {
            if (!this.bgwProceedImageAsync.IsBusy)
            {
                this.AsyncProcessingInProgress = true;
                bgwProceedImageAsync.RunWorkerAsync(new BgwProceedImageAsyncParams(bitmap, bitmapRect, traceColor, actionSeen, actionLost));
            }
        }

        void bgwProceedImageAsync_DoWork(object sender, DoWorkEventArgs e)
        {
            BgwProceedImageAsyncParams p = (BgwProceedImageAsyncParams)e.Argument;
            ProceedImage(p.Bitmap, p.BitmapRect, p.TraceColor, p.ActionSeen, p.ActionLost);
        }
        void bgwProceedImageAsync_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.AsyncProcessingInProgress = false;
        }
    }
    class BgwProceedImageAsyncParams
    {
        public BgwProceedImageAsyncParams(Bitmap bitmap,  Rectangle bitmapRect, string traceColor, string actionSeen, string actionLost)
        {
            this.Bitmap = bitmap;
            this.BitmapRect = bitmapRect;
            this.TraceColor = traceColor;
            this.ActionSeen = actionSeen;
            this.ActionLost = actionLost;
        }
        public Bitmap Bitmap { get; private set; }
        public Rectangle BitmapRect { get; private set; }
        public string TraceColor { get; private set; }
        public string ActionSeen { get; private set; }
        public string ActionLost { get; private set; }
    }
}

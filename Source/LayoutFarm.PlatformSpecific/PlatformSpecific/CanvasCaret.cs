//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing; 
namespace LayoutFarm
{


    static class CanvasCaret
    {
        static Timer caretBlinkTimer;
        static Bitmap caretBmp;

        static CanvasViewport canvasViewport;

        static IntPtr caretDc;
        static int caretWidth;
        static int caretHeight;

        static bool isOdd;

        static CanvasCaret()
        {
            caretBlinkTimer = new Timer();
            caretBlinkTimer.Tick += new EventHandler(caretBlinkTimer_Tick);
            caretBlinkTimer.Interval = 500; caretWidth = 1;
            caretHeight = 16;
            caretBmp = new Bitmap(caretWidth, caretHeight);
            caretWidth = caretBmp.Width;
            caretHeight = caretBmp.Height;

            caretDc = MyWin32.CreateCompatibleDC(IntPtr.Zero);
            MyWin32.SelectObject(caretDc, caretBmp.GetHbitmap());
            MyWin32.Win32Rect r = new MyWin32.Win32Rect(0, 0, caretWidth, caretHeight);
            IntPtr brush = MyWin32.CreateSolidBrush(GraphicWin32InterOp.ColorToWin32(Color.White));
            MyWin32.FillRect(caretDc, ref r, brush);
            MyWin32.DeleteObject(brush);
        }

        static void caretBlinkTimer_Tick(object sender, EventArgs e)
        {
            if (canvasViewport != null)
            {
                canvasViewport.Caret_Blink();
            }
        }
        static void DestroyCaret(CanvasViewport viewport)
        {
            if (canvasViewport != null && canvasViewport == viewport)
            {
                caretBlinkTimer.Enabled = false; if (caretBmp != null)
                {
                    caretBmp.Dispose();
                    caretBmp = null;
                }
                if (caretDc != IntPtr.Zero)
                {
                    MyWin32.DeleteDC(caretDc);
                    caretDc = IntPtr.Zero;
                }
            }
        }
        public static void SetCaretTo(CanvasViewport viewport)
        {
            if (viewport != null && canvasViewport != viewport)
            {
                canvasViewport = viewport;
            }
            else if (viewport == null)
            {
                canvasViewport = null;
                caretBlinkTimer.Enabled = false;
            }
        }
        public static void RenderCaretBlink(IntPtr destHdc, int x, int y)
        {
            MyWin32.BitBlt(destHdc, x, y, caretWidth, caretHeight, caretDc, 0, 0, MyWin32.SRCERASE);
            isOdd = !isOdd;
        }
        public static void ForceRenderCaret(IntPtr destHdc, int x, int y)
        {
            if (!isOdd)
            {
                MyWin32.BitBlt(destHdc, x, y, caretWidth, caretHeight, caretDc, 0, 0, MyWin32.SRCERASE);
                isOdd = !isOdd;
            }
        }
        public static void ForceHideCaret(IntPtr destHdc, int x, int y)
        {
            if (isOdd)
            {
                MyWin32.BitBlt(destHdc, x, y, caretWidth, caretHeight, caretDc, 0, 0, MyWin32.SRCERASE);
                isOdd = !isOdd;
            }
        }
        public static void StartCaretBlink()
        {
            caretBlinkTimer.Enabled = true;
            isOdd = false;
        }
        public static void StopCaretBlink()
        {

            caretBlinkTimer.Enabled = false;
            isOdd = false;
        }

    }



}
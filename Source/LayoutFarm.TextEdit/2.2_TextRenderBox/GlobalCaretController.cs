//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LayoutFarm.Text
{

    static class GlobalCaretController
    {
        static bool enableCaretBlink = true;//default
        static TextEditRenderBox textEditBox;
        static bool caretRegistered = false;
        static EventHandler<EventArgs> tickHandler;
        static object caretBlinkTask = new object();

        static GlobalCaretController()
        {
            tickHandler = new EventHandler<EventArgs>(caret_TickHandler);
        }
        internal static void RegisterCaretBlink(RootGraphic gfx)
        {
            if (caretRegistered)
            {
                return;
            }
            caretRegistered = true;
            GraphicIntervalTask task = gfx.RequestGraphicInternvalTask(caretBlinkTask,
                300, tickHandler);
        }
        static void caret_TickHandler(object sender, EventArgs e)
        {
            if (textEditBox != null)
            {

                textEditBox.StateHideCaret = !textEditBox.StateHideCaret;
                //force render ?
                textEditBox.InvalidateGraphic();
            }
            else
            {

            }

        }
        public static bool EnableCaretBlink
        {
            get { return enableCaretBlink; }
            set
            {
                enableCaretBlink = value;
            }
        }
        internal static TextEditRenderBox CurrentTextEditBox
        {
            get { return textEditBox; }
            set
            {
                textEditBox = value;
            }
        }

    }

}
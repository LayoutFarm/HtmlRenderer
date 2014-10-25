//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.UI;
namespace LayoutFarm.Text
{

    static class GlobalCaretController
    {
        static bool enableCaretBlink = true;//default
        static TextEditRenderBox textEditBox;
        static bool caretRegistered = false;
        static EventHandler<GraphicsTimerTaskEventArgs> tickHandler;
        static object caretBlinkTask = new object();
        static GraphicsTimerTask task;

        static GlobalCaretController()
        {
            tickHandler = new EventHandler<GraphicsTimerTaskEventArgs>(caret_TickHandler);
        }
        internal static void RegisterCaretBlink(RootGraphic root)
        {
            if (caretRegistered)
            {
                return;
            }
            caretRegistered = true;

            task = root.RequestGraphicsIntervalTask(
                caretBlinkTask,
                TaskIntervalPlan.CaretBlink,
                300,
                tickHandler);
        }
        static void caret_TickHandler(object sender, GraphicsTimerTaskEventArgs e)
        {
            if (textEditBox != null)
            {
                textEditBox.SwapCaretState();
                //force render ?
                textEditBox.InvalidateGraphic();
                e.NeedUpdate = 1;
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
                if (textEditBox != value)//&& textEditBox != null)
                {
                    //make lost focus on current textbox
                    if (textEditBox != null)
                    {
                        //stop caret on prev element
                        textEditBox.SetCaretState(false);
                        var evlistener = textEditBox.GetController() as IEventListener;

                        textEditBox = null;

                        if (evlistener != null)
                        {
                            evlistener.ListenLostFocus(null);
                        }
                    }
                }
                textEditBox = value;
            }
        }



    }

}
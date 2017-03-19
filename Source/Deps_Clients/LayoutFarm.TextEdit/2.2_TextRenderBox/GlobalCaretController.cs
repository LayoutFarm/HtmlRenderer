//Apache2, 2014-2017, WinterDev

using System;
using LayoutFarm.RenderBoxes;
using LayoutFarm.UI;
namespace LayoutFarm.Text
{
    static class GlobalCaretController
    {
        static bool enableCaretBlink = true;//default
        static TextEditRenderBox currentTextBox;
        static EventHandler<GraphicsTimerTaskEventArgs> tickHandler;
        static object caretBlinkTask = new object();
        static GraphicsTimerTask task;
        static GlobalCaretController()
        {
            tickHandler = new EventHandler<GraphicsTimerTaskEventArgs>(caret_TickHandler);
        }
        internal static void RegisterCaretBlink(RootGraphic root)
        {
            if (!root.CaretHandleRegistered)
            {
                root.CaretHandleRegistered = true;
                task = root.SubscribeGraphicsIntervalTask(
                    caretBlinkTask,
                    TaskIntervalPlan.CaretBlink,
                    20,
                    tickHandler);
            }
        }
        static void caret_TickHandler(object sender, GraphicsTimerTaskEventArgs e)
        {
            if (currentTextBox != null)
            {
                currentTextBox.SwapCaretState();
                e.NeedUpdate = 1;
            }
            else
            {
                //Console.WriteLine("no current textbox");
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
            get { return currentTextBox; }
            set
            {
                if (currentTextBox != value)//&& textEditBox != null)
                {
                    //make lost focus on current textbox
                    if (currentTextBox != null)
                    {
                        //stop caret on prev element
                        currentTextBox.SetCaretState(false);
                        var evlistener = currentTextBox.GetController() as IEventListener;
                        currentTextBox = null;
                        if (evlistener != null)
                        {
                            evlistener.ListenLostKeyboardFocus(null);
                        }
                    }
                }
                currentTextBox = value;
            }
        }
    }
}
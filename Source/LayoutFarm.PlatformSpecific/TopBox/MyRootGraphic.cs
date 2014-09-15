//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace LayoutFarm
{

    public class MyRootGraphic : RootGraphic
    {
        List<RenderElementRequest> veReqList = new List<RenderElementRequest>();
        public MyRootGraphic(int width, int height)
            : base(width, height)
        {
#if DEBUG
            dbugCurrentGlobalVRoot = this;
            dbug_Init();
#endif
        }
#if DEBUG
        ~MyRootGraphic()
        {
            dbugHitTracker.Close();
        }
#endif
        static Point localCaretPos;
        static RenderElement caretOwner;

        public static Point GetGlobalCaretPosition()
        {
            if (caretOwner == null)
            {
                return Point.Empty;
            }
            Point caretPos = localCaretPos;
            Point globalCaret = caretOwner.GetGlobalLocation();
            caretPos.Offset(globalCaret.X, globalCaret.Y);
            return caretPos;
        }

        //public static void SetCarentPosition(Point p, RenderElement owner)
        //{
        //    caretOwner = owner;
        //    localCaretPos = p; 
        //}
        public override void SetCarentPosition(Point p, RenderElement renderE)
        {

        }



        public const int IS_SHIFT_KEYDOWN = 1 << (1 - 1);
        public const int IS_ALT_KEYDOWN = 1 << (2 - 1);
        public const int IS_CTRL_KEYDOWN = 1 << (3 - 1);


        public int VisualRequestCount
        {
            get
            {
                return veReqList.Count;
            }
        }

        public void ClearVisualRequests(TopWindowRenderBox wintop)
        {
            int j = veReqList.Count;
            for (int i = 0; i < j; ++i)
            {
                RenderElementRequest req = veReqList[i];
                switch (req.req)
                {

                    case RequestCommand.AddToWindowRoot:
                        {
                            wintop.AddChild(req.ve);

                        } break;
                    case RequestCommand.DoFocus:
                        {
                            RenderElement ve = req.ve;
                            wintop.CurrentKeyboardFocusedElement = ve;
                            ve.InvalidateGraphic();
                             
                        } break;
                    case RequestCommand.InvalidateArea:
                        {
                            Rectangle r = (Rectangle)req.parameters;
                            TopWindowRenderBox wintop2;
                            this.InvalidateGraphicArea(req.ve, ref r, out wintop2);
                        } break;

                }
            }
            veReqList.Clear();
        }
    }
}
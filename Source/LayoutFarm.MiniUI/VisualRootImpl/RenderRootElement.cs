//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


using LayoutFarm.Presentation;
namespace LayoutFarm.Presentation
{
    public class RenderRootElement 
#if DEBUG
        : dbugRootElement
#endif
    {
        List<RenderElementRequest> veReqList = new List<RenderElementRequest>();
        static Stack<VisualElementArgs> visualArgStack = new Stack<VisualElementArgs>();


        public RenderRootElement()
        {
#if DEBUG
            dbugCurrentGlobalVRoot = this;
            dbug_Init();
#endif
        }
#if DEBUG
        ~RenderRootElement()
        {
            dbugHitTracker.Close();
        }
#endif

        public static VisualElementArgs GetVisualInvalidateArgs(RootWindowRenderBox winroot)
        {
            if (visualArgStack.Count > 0)
            {
                VisualElementArgs vinv = visualArgStack.Pop();
                vinv.SetWinRoot(winroot);
                return vinv;
            }
            else
            {
                return new VisualElementArgs(winroot);
            }
        }

        public static void FreeVisualInvalidateArgs(VisualElementArgs vinv)
        {
            VisualElementArgs.ClearForReuse(vinv);
            visualArgStack.Push(vinv);
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

        public void ClearVisualRequests(RootWindowRenderBox winroot)
        {
            int j = veReqList.Count;
            for (int i = 0; i < j; ++i)
            {
                RenderElementRequest req = veReqList[i];
                switch (req.req)
                {

                    case RequestCommand.AddToWindowRoot:
                        {
                            winroot.AddChild(req.ve);

                        } break;
                    case RequestCommand.DoFocus:
                        {
                            RenderElement ve = req.ve;
                            if (ve.WinRoot != null)
                            {
                                ve.WinRoot.CurrentKeyboardFocusedElement = ve;
                                VisualElementArgs vinv = ve.GetVInv();
                                ve.InvalidateGraphic(vinv);
                                ve.FreeVInv(vinv);
                            }
                        } break;
                    case RequestCommand.InvalidateArea:
                        {
                            Rectangle r = (Rectangle)req.parameters;

                            InternalRect internalRect = InternalRect.CreateFromRect(r);
                            winroot.InvalidateGraphicArea(req.ve, internalRect);
                            InternalRect.FreeInternalRect(internalRect);

                        } break;

                }
            }
            veReqList.Clear();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;


using LayoutFarm.Presentation;




namespace LayoutFarm.Presentation
{
    public class VisualRootImpl : VisualRoot
    {
        
                        
        List<VisualElementRequest> veReqList = new List<VisualElementRequest>();
 
        static Stack<VisualElementArgs> visualArgStack = new Stack<VisualElementArgs>();

         
        public VisualRootImpl()
        {
            #if DEBUG
            dbugCurrentGlobalVRoot = this;
            dbug_Init();
#endif
        }
#if DEBUG
        ~VisualRootImpl()
        {
            dbugHitTracker.Close();
        }
#endif 

        public static VisualElementArgs GetVisualInvalidateArgs(ArtVisualRootWindow winroot)
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
        public static VisualElementArgs GetVisualInvalidateArgs(VisualRoot vsroot)
        {
            if (visualArgStack.Count > 0)
            {
                VisualElementArgs vinv = visualArgStack.Pop();
                vinv.SetVisualRoot(vsroot);
                return vinv;
            }
            else
            {
                return new VisualElementArgs(vsroot);
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
        public override void AddVisualRequest(VisualElementRequest req)
        {
            veReqList.Add(req);
        }
                                        public void ClearVisualRequests(ArtVisualRootWindow winroot)
        {
                                    int j = veReqList.Count;
            for (int i = 0; i < j; ++i)
            {
                VisualElementRequest req = veReqList[i];
                switch (req.req)
                {

                    case RequestCommand.AddToWindowRoot:
                        {
                            winroot.AddChild(req.ve);

                        } break;
                    case RequestCommand.DoFocus:
                        {
                            ArtVisualElement ve = req.ve;
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
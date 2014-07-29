using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace LayoutFarm.Presentation
{

    
    public class VisualElementArgs
    {
                                ArtVisualRootWindow winroot;
        VisualRoot visualRoot;
        
#if DEBUG
                const int VISUAL_ELEMENT = 0x0;
        const int LINE = 0x1;
        const int LAYER = 0x2;
        dbugVisualLayoutTracer debugVisualLay;
                                object dbugInitObject;
                                int dbugFlags;
        int dbugId;
        static int dbugTotalId;
#endif

        public VisualElementArgs(ArtVisualRootWindow winroot)
        {
#if DEBUG
            this.dbugId = dbugTotalId;
            dbugTotalId++;

#endif
            this.winroot = winroot;
        }
        public VisualElementArgs(VisualRoot vsroot)
        {

#if DEBUG
            this.dbugId = dbugTotalId;
            dbugTotalId++;

#endif
            this.visualRoot = vsroot;
        }


                                        public static void ClearForReuse(VisualElementArgs vinv)
        {
            vinv.winroot = null;
            vinv.IsInTopDownReArrangePhase = false;
            vinv.visualRoot = null;
            vinv.ForceReArrange = false;            
#if DEBUG
            vinv.dbugBreakOnSelectedVisuallElement = false;
            vinv.debugVisualLay = null;
            vinv.dbugInitObject = null;
            vinv.dbugFlags = 0;
#endif

        }

        public ArtVisualRootWindow WinRoot
        {
            get
            {
                                return this.winroot;
            }
        }
                                        public void SetWinRoot(ArtVisualRootWindow winroot)
        {
            this.winroot = winroot;
        }
        public void SetVisualRoot(VisualRoot vsroot)
        {
            this.visualRoot = vsroot;
        }
        public bool ForceReArrange
        {
            get;
            set;
        }

#if DEBUG
        public bool dbugBreakOnSelectedVisuallElement
        {
            get;
            set;
        }
#endif
        public bool IsInTopDownReArrangePhase
        {
            get;
            set;
        }
        public void SetFocusElement(ArtVisualElement ve)
        {
            if (winroot != null)
            {
                                winroot.CurrentKeyboardFocusedElement = ve;
            }
            else
            {
                                AddRequest(new VisualElementRequest(ve, RequestCommand.DoFocus));
            }
        }
        public void AddToWindowRootLater(ArtVisualElement ve)
        {
            AddRequest(new VisualElementRequest(
               ve, RequestCommand.AddToWindowRoot));
        }

        void AddRequest(VisualElementRequest req)
        {
            if (this.visualRoot != null)
            {
                visualRoot.AddVisualRequest(req);
            }
        }

        public void AddInvalidateRequest(ArtVisualElement ve, InternalRect rect)
        {

            if (winroot != null)
            {
                winroot.InvalidateGraphicArea(ve, rect);
            }
            else
            {
                AddRequest(new VisualElementRequest(ve, RequestCommand.InvalidateArea,
                  rect.ToRectangle()));
            }

                                                                                    
                                                                    }
                                                                                                                                
                
        


                                        
                                                        


#if DEBUG
        public void dbug_SetInitObject(ArtVisualElement initVisualElement)
        {
            this.dbugInitObject = initVisualElement;
        }
        public void dbug_SetInitObject(VisualLayer initLayer)
        {
            this.dbugInitObject = initLayer;
            dbugFlags |= LAYER;
        }
                public void debug_PushTopDownElement(ArtVisualContainerBase v)
        {
                    }
        public void debug_PopTopDownElement(ArtVisualContainerBase v)
        {
                    }
                public void dbug_EnterTopDownReCalculateContent(ArtVisualElement v)
        {

            if (debugVisualLay != null)
            {
                debugVisualLay.PushVisualElement(v);
                debugVisualLay.WriteInfo(v, ">>TOPDOWN_RECAL_CONTENT ", "-", "&");
            }
        }
        public void dbug_ExitTopDownReCalculateContent(ArtVisualElement v)
        {
            if (debugVisualLay != null)
            {
                debugVisualLay.WriteInfo(v, "<<TOPDOWN_RECAL_CONTENT ", "-", "&");
                debugVisualLay.PopVisualElement();
            }

        }
                public void dbug_WriteInfo(dbugVisitorMessage msg)
        {
            if (debugVisualLay != null)
            {
                debugVisualLay.WriteInfo(msg.text);
            }
        }

        public void dbug_WriteInfo(dbugVisitorMessage msg, ArtVisualElement ve)
        {
            if (debugVisualLay != null)
            {
                debugVisualLay.WriteInfo(msg.text, ve);
            }
        }
                public void dbug_BeginNewContext(dbugVisitorMessage msg, ArtVisualElement ve)
        {
            if (debugVisualLay != null)
            {
                debugVisualLay.BeginNewContext();                debugVisualLay.WriteInfo(msg.text, ve);            }
        }
        public void dbug_EndCurrentContext(dbugVisitorMessage msg, ArtVisualElement ve)
        {
            if (debugVisualLay != null)
            {
                debugVisualLay.WriteInfo(msg.text, ve);                debugVisualLay.EndCurrentContext();
            }
        }
                public void dbug_BeginSetElementBound(ArtVisualElement v)
        {
                                                
                                    this.dbug_BeginNewContext(dbugVisitorMessage.WITH_0, v);        }
        public void dbug_EndSetElementBound(ArtVisualElement v)
        {
            this.dbug_EndCurrentContext(dbugVisitorMessage.WITH_1, v);        }
        
                                                                                                                                                public void dbug_EnterReArrangeContent(ArtVisualElement v)
        {
            if (debugVisualLay != null)
            {
                debugVisualLay.PushVisualElement(v);
                debugVisualLay.WriteInfo(v, "ARR_INNER", ">>", "&");
            }
        }
        public void dbug_ExitReArrangeContent()
        {
            if (debugVisualLay != null)
            {
                ArtVisualElement v = (ArtVisualElement)debugVisualLay.PeekElement();
                debugVisualLay.WriteInfo(v, "ARR_INNER", "<<", "&");
                debugVisualLay.PopVisualElement();
            }
        }
                public void dbug_EnterLayerReArrangeContent(VisualLayer layer)
        {
            if (debugVisualLay != null)
            {
                debugVisualLay.PushLayerElement(layer);
                debugVisualLay.WriteInfo("..>LAYER_ARR :" + layer.ToString());
            }
        }
        public void dbug_ExitLayerReArrangeContent()
        {
            if (debugVisualLay != null)
            {
                VisualLayer layer = (VisualLayer)debugVisualLay.PeekElement();
                debugVisualLay.WriteInfo("<..LAYER_ARR :" + layer.ToString());
                debugVisualLay.PopLayerElement();

            }
        }
                public void dbug_EnterLayerReCalculateContent(VisualLayer layer)
        {
            if (debugVisualLay != null)
            {
                debugVisualLay.PushLayerElement(layer);
                debugVisualLay.WriteInfo("..>L_RECAL_TOPDOWN :" + layer.ToString());
            }
        }
        public void dbug_ExitLayerReCalculateContent()
        {
            if (debugVisualLay != null)
            {
                VisualLayer layer = (VisualLayer)debugVisualLay.PeekElement();
                debugVisualLay.WriteInfo("<..L_RECAL_TOPDOWN  :" + layer.ToString());
                debugVisualLay.PopLayerElement();
            }
        }
                public void dbug_StartLayoutTrace(dbugVisualElementLayoutMsg msg, int suffixNum)
        {

            VisualRoot visualroot = VisualRoot.dbugCurrentGlobalVRoot;
            if (visualroot == null || !visualroot.dbug_IsRecordLayoutTraceEnable)
            {
                return;
            }
            debugVisualLay = visualroot.dbug_GetLastestVisualLayoutTracer();
            switch (dbugFlags & 0x3)
            {
                case VISUAL_ELEMENT:
                    {
                        ArtVisualElement v = (ArtVisualElement)dbugInitObject;
                        debugVisualLay.WriteInfo(msg.msg + suffixNum.ToString());
                                                debugVisualLay.WriteInfo("*** init visual : " + v.dbug_FullElementDescription());
                    } break;
                case LINE:
                    {
                                                                                                                    } break;
                case LAYER:
                    {
                        VisualLayer layer = (VisualLayer)dbugInitObject;
                        debugVisualLay.WriteInfo(msg.msg + suffixNum.ToString());
                        debugVisualLay.WriteInfo("*** init layer (" + layer.dbug_layer_id + "):" + layer.ToString());
                    } break;
                default:
                    {
                        debugVisualLay.WriteInfo(msg.msg + suffixNum.ToString());
                    } break;
            }
        }
        public void dbug_StartLayoutTrace(dbugVisualElementLayoutMsg msg)
        {
            VisualRoot visualroot = VisualRoot.dbugCurrentGlobalVRoot;
            if (visualroot == null || !visualroot.dbug_IsRecordLayoutTraceEnable)
            {
                return;
            }

                                                                                                                                                                                                            
                                                            debugVisualLay = visualroot.dbug_GetLastestVisualLayoutTracer();

            switch (dbugFlags & 0x3)
            {
                case VISUAL_ELEMENT:
                    {
                        ArtVisualElement v = (ArtVisualElement)dbugInitObject;
                        debugVisualLay.WriteInfo(msg.msg);
                                                debugVisualLay.WriteInfo("*** init visual : " + v.dbug_FullElementDescription());
                    } break;
                case LINE:
                    {
                                                                                                                    } break;
                case LAYER:
                    {
                        VisualLayer layer = (VisualLayer)dbugInitObject;
                        debugVisualLay.WriteInfo(msg.msg);
                        debugVisualLay.WriteInfo("*** init layer (" + layer.dbug_layer_id + "):" + layer.ToString());
                    } break;
                default:
                    {
                        debugVisualLay.WriteInfo(msg.msg);
                    } break;
            }

        }

        public void dbug_EndLayoutTrace()
        {
            if (debugVisualLay != null)
            {

                debugVisualLay.WriteInfo("-----  END SESSION -------");
            }
        }

#endif



    }

}
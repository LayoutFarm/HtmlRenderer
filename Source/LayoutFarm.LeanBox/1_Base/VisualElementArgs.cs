//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace LayoutFarm.Presentation
{
     
    public class LayoutPhaseVisitor
    {   
        TopWindowRenderBox winroot; 
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

        public LayoutPhaseVisitor(TopWindowRenderBox winroot)
        {
#if DEBUG
            this.dbugId = dbugTotalId;
            dbugTotalId++; 
#endif
            this.winroot = winroot;
            this.ForceReArrange = true;//default
        }
       
        public static void ClearForReuse()
        {
//            vinv_winroot = null;
//            vinv_IsInTopDownReArrangePhase = false;
           
//            vinv_ForceReArrange = false;
//#if DEBUG
//            vinv_dbugBreakOnSelectedVisuallElement = false;
//            vinv_debugVisualLay = null;
//            vinv_dbugInitObject = null;
//            vinv_dbugFlags = 0;
//#endif

        }

        public TopWindowRenderBox WinRoot
        {
            get
            {
                return this.winroot;
            }
        }
        public void SetWinRoot(TopWindowRenderBox winroot)
        {
            this.winroot = winroot;
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
        public void SetFocusElement(RenderElement ve)
        {
            if (winroot != null)
            {
                winroot.CurrentKeyboardFocusedElement = ve;
            }
            else
            {
                AddRequest(new RenderElementRequest(ve, RequestCommand.DoFocus));
            }
        }
         
        void AddRequest(RenderElementRequest req)
        {
            
        } 
        public void AddInvalidateRequest(RenderElement ve, InternalRect rect)
        {

            if (winroot != null)
            {
                winroot.InvalidateGraphicArea(ve, rect);
            }
            else
            {
                AddRequest(new RenderElementRequest(ve, RequestCommand.InvalidateArea,
                  rect.ToRectangle()));
            } 
        } 
#if DEBUG
        public void dbug_SetInitObject(RenderElement initVisualElement)
        {
            this.dbugInitObject = initVisualElement;
        }
        public void dbug_SetInitObject(VisualLayer initLayer)
        {
            this.dbugInitObject = initLayer;
            dbugFlags |= LAYER;
        }
        public void debug_PushTopDownElement(RenderBoxBase v)
        {
        }
        public void debug_PopTopDownElement(RenderBoxBase v)
        {
        }
        public void dbug_EnterTopDownReCalculateContent(RenderElement v)
        {

            if (debugVisualLay != null)
            {
                debugVisualLay.PushVisualElement(v);
                debugVisualLay.WriteInfo(v, ">>TOPDOWN_RECAL_CONTENT ", "-", "&");
            }
        }
        public void dbug_ExitTopDownReCalculateContent(RenderElement v)
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

        public void dbug_WriteInfo(dbugVisitorMessage msg, RenderElement ve)
        {
            if (debugVisualLay != null)
            {
                debugVisualLay.WriteInfo(msg.text, ve);
            }
        }
        public void dbug_BeginNewContext(dbugVisitorMessage msg, RenderElement ve)
        {
            if (debugVisualLay != null)
            {
                debugVisualLay.BeginNewContext(); debugVisualLay.WriteInfo(msg.text, ve);
            }
        }
        public void dbug_EndCurrentContext(dbugVisitorMessage msg, RenderElement ve)
        {
            if (debugVisualLay != null)
            {
                debugVisualLay.WriteInfo(msg.text, ve); debugVisualLay.EndCurrentContext();
            }
        }
        public void dbug_BeginSetElementBound(RenderElement v)
        {

            this.dbug_BeginNewContext(dbugVisitorMessage.WITH_0, v);
        }
        public void dbug_EndSetElementBound(RenderElement v)
        {
            this.dbug_EndCurrentContext(dbugVisitorMessage.WITH_1, v);
        }

        public void dbug_EnterReArrangeContent(RenderElement v)
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
                RenderElement v = (RenderElement)debugVisualLay.PeekElement();
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

            RootGraphic visualroot = RootGraphic.dbugCurrentGlobalVRoot;
            if (visualroot == null || !visualroot.dbug_IsRecordLayoutTraceEnable)
            {
                return;
            }
            debugVisualLay = visualroot.dbug_GetLastestVisualLayoutTracer();
            switch (dbugFlags & 0x3)
            {
                case VISUAL_ELEMENT:
                    {
                        RenderElement v = (RenderElement)dbugInitObject;
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
            RootGraphic visualroot = RootGraphic.dbugCurrentGlobalVRoot;
            if (visualroot == null || !visualroot.dbug_IsRecordLayoutTraceEnable)
            {
                return;
            }


            debugVisualLay = visualroot.dbug_GetLastestVisualLayoutTracer();

            switch (dbugFlags & 0x3)
            {
                case VISUAL_ELEMENT:
                    {
                        RenderElement v = (RenderElement)dbugInitObject;
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
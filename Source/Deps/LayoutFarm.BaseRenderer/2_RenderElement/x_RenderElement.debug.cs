//Apache2, 2014-2017, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm
{
#if DEBUG

    public class dbugVisualElementLayoutMsg
    {
        public string msg;
        public dbugVisualElementLayoutMsg(string msg)
        {
            this.msg = msg;
        }

        public static dbugVisualElementLayoutMsg ArtVisualElement_ResumeLayout = new dbugVisualElementLayoutMsg("ArtVisualElement::ResumeLayout");
        public static dbugVisualElementLayoutMsg GridColumnCollection_Insert = new dbugVisualElementLayoutMsg("GridColumnCollection::Insert");
        public static dbugVisualElementLayoutMsg GridColumnCollection_Remove = new dbugVisualElementLayoutMsg("GridColumnCollection::Remove");
        public static dbugVisualElementLayoutMsg GrCols_MoveColumnAfter = new dbugVisualElementLayoutMsg("GrCols::MoveColumnAfter");
        public static dbugVisualElementLayoutMsg GrRows_Add = new dbugVisualElementLayoutMsg("GrRows::Add");
        public static dbugVisualElementLayoutMsg RowCollection_Remove = new dbugVisualElementLayoutMsg("RowCollection::Remove");
        public static dbugVisualElementLayoutMsg DockSpaceLayer_ArrAllDockSpaces = new dbugVisualElementLayoutMsg("DockSpaceLayer::ArrAllDockSpaces");
        public static dbugVisualElementLayoutMsg DockSpaceLayer_ArrAllFreeSpaces = new dbugVisualElementLayoutMsg("DockSpaceLayer::ArrAllFreeSpaces");
        public static dbugVisualElementLayoutMsg ArtVisualTextSurafce_EnsureCaretVisible = new dbugVisualElementLayoutMsg("ArtVisualTextSurafce::EnsureCaretVisible");
        public static dbugVisualElementLayoutMsg Clear_CAL = new dbugVisualElementLayoutMsg("ClEAR CAL : ");
        public static dbugVisualElementLayoutMsg Clear_ARR = new dbugVisualElementLayoutMsg("ClEAR ARR : ");
        public static dbugVisualElementLayoutMsg Clear_CAL_ARR = new dbugVisualElementLayoutMsg("ClEAR CAL+ARR : ");
        public static dbugVisualElementLayoutMsg Clear_ARR_CAL = new dbugVisualElementLayoutMsg("ClEAR ARR+CAL : ");
    }

    partial class RenderElement
    {
        public bool dbugNeedContentArrangement
        {
            get
            {
                return this.NeedContentArrangement;
            }
        }
        public bool dbugNeedReCalculateContentSize
        {
            get
            {
                return this.NeedReCalculateContentSize;
            }
        }
        public Rectangle dbugGetGlobalRect()
        {
            return new Rectangle(GetGlobalLocationStatic(this), Size);
        }
        public virtual void dbug_WriteOwnerLayerInfo(RootGraphic visualroot, int i)
        {
            if (this.parentLink != null)
            {
                visualroot.dbug_rootHitChainMsg.AddLast(new dbugLayoutMsg(this, new string('.', i) + " [Ly:" + i + "] " +
                      parentLink.dbugGetLinkInfo()));
            }
        }
        public virtual void dbug_WriteOwnerLineInfo(RootGraphic visualroot, int i)
        {
        }

        public string dbugGetCssBoxInfo
        {
            get
            {
                return dbug_FullElementDescription();
            }
        }

        public string dbug_FixedElementCode = null;
        public int dbug_element_code_y = 0;
        public bool dbug_hide_objIden = false;
        public readonly int dbug_obj_id = 0;
        static int dbug_totalObjectId = 0;
        public string dbug_ObjectNote;
        public void dbug_SetFixedElementCode(string staticElementCode)
        {
            dbug_FixedElementCode = staticElementCode;
        }
        public virtual void dbug_DumpVisualProps(dbugLayoutMsgWriter writer)
        {
            writer.Add(this, this.dbug_FullElementDescription());
        }


        public virtual string dbug_FullElementDescription()
        {
            string user_elem_id = null;
            string element_iden = dbug_FixedElementCode;
            if (dbug_ObjectNote != null)
            {
                element_iden += " " + dbug_ObjectNote;
            }

            if (IsBlockElement)
            {
                if (user_elem_id != null)
                {
                    return element_iden + dbug_GetBoundInfo() + "b " + " i" + dbug_obj_id + "a ,(ID=" + user_elem_id + ") " + dbug_GetLayoutInfo();
                }
                else
                {
                    return element_iden + dbug_GetBoundInfo() + "b " + " i" + dbug_obj_id + "a " + dbug_GetLayoutInfo();
                }
            }
            else
            {
                if (user_elem_id != null)
                {
                    return element_iden + dbug_GetBoundInfo() + " "
                         + " i" + dbug_obj_id + "a (ID= " + user_elem_id + ") " + dbug_GetLayoutInfo();
                }
                else
                {
                    return element_iden + dbug_GetBoundInfo() + " " + " i" + dbug_obj_id + "a " + dbug_GetLayoutInfo();
                }
            }
        }
        public RenderElement dbugParentVisualElement
        {
            get { return this.ParentRenderElement; }
        }
        public override string ToString()
        {
            return string.Empty;
        }
        public string dbug_GetBoundInfo()
        {
            Rectangle r = this.RectBounds;
            string output = "{" + r.X + "," + r.Y + "," + r.Width + "," + r.Height +
                ";dw=" + this.Width +
                ";dh=" + this.Height;
            return output;
        }
        public string dbug_GetLayoutInfo()
        {
            string info = string.Empty;
            if (!this.HasCalculatedSize)
            {
                info += "[C:" + dbug_InvalidateRecalculateSizeEpisode + "]";
            }
            else
            {
                info += "[nc:" + dbug_ValidateRecalculateSizeEpisode + "]";
            }

            if (this.dbugNeedContentArrangement)
            {
                info += "[A:" + dbug_InvalidateContentArrEpisode + "," + "na:" + dbug_ValidateContentArrEpisode + "]";
                if (this.dbug_FinishArr != this.dbug_BeginArr)
                {
                    info += "!";
                }
            }
            else
            {
                info += "[na:" + dbug_ValidateContentArrEpisode + ",A:" + dbug_ValidateContentArrEpisode + "]";
                if (this.dbug_FinishArr != this.dbug_BeginArr)
                {
                    info += "!";
                }
            }

            if (this.IsInLayoutQueue)
            {
                info += "[Q]";
            }

            return info;
        }
        public int dbug_BeginArr;
        public int dbug_FinishArr;
        public int dbug_InvalidateContentArrEpisode = 0;
        public int dbug_ValidateContentArrEpisode = 0;
        public int dbug_InvalidateRecalculateSizeEpisode = 0;
        public int dbug_ValidateRecalculateSizeEpisode = 0;
        public static int dbug_totalInvalidateContentArrEpisode = 0;
        public RootGraphic dbugVRoot
        {
            get
            {
                return RootGraphic.dbugCurrentGlobalVRoot;
            }
        }

        void debug_RecordPostDrawInfo(Canvas canvasPage)
        {
            if (dbugVRoot.dbug_ShowElementOutline)
            {
                canvasPage.DrawRectangle(Color.Red,
                    0, 0, this.Width - 1, this.Height - 1);
            }
            if (dbugVRoot.dbug_ForceShowObjectIden)
            {
                var prevColor = canvasPage.CurrentTextColor;
                canvasPage.CurrentTextColor = Color.Blue;
                canvasPage.DrawText(
                        ("<< " + dbug_FullElementDescription()).ToCharArray()
                        , 0, dbug_element_code_y);
                canvasPage.CurrentTextColor = prevColor;
            }
            else if (dbugVRoot.dbug_ShowObjectIden && !dbug_hide_objIden)
            {
                var prevColor = canvasPage.CurrentTextColor;
                canvasPage.CurrentTextColor = Color.Blue;
                canvasPage.DrawText(
                        ("<< " + dbug_FullElementDescription()).ToCharArray()
                        , 0, dbug_element_code_y);
                canvasPage.CurrentTextColor = prevColor;
            }
        }

        //-------------------------------------------------------------------------
        protected static void dbug_EnterTopDownReCalculateContent(RenderElement v)
        {
            var debugVisualLay = dbugGetLayoutTracer();
            if (debugVisualLay == null) return;
            debugVisualLay.PushVisualElement(v);
            debugVisualLay.WriteInfo(v, ">>TOPDOWN_RECAL_CONTENT ", "-", "&");
        }
        public static void dbug_ExitTopDownReCalculateContent(RenderElement v)
        {
            var debugVisualLay = dbugGetLayoutTracer();
            if (debugVisualLay == null) return;
            debugVisualLay.WriteInfo(v, "<<TOPDOWN_RECAL_CONTENT ", "-", "&");
            debugVisualLay.PopVisualElement();
        }
        public static void dbug_SetInitObject(RenderElement ve)
        {
            dbugInitObject = ve;
        }
        public static void dbug_EnterReArrangeContent(RenderElement v)
        {
            var debugVisualLay = dbugGetLayoutTracer();
            if (debugVisualLay == null) return;
            debugVisualLay.PushVisualElement(v);
            debugVisualLay.WriteInfo(v, "ARR_INNER", ">>", "&");
        }

        public static void dbug_ExitReArrangeContent(RenderElement ve)
        {
            var debugVisualLay = dbugGetLayoutTracer();
            if (debugVisualLay == null) return;
            RenderElement v = (RenderElement)debugVisualLay.PeekElement();
            debugVisualLay.WriteInfo(v, "ARR_INNER", "<<", "&");
            debugVisualLay.PopVisualElement();
        }
        public static void dbug_StartLayoutTrace(dbugVisualElementLayoutMsg m, int i)
        {
            //RootGraphic visualroot = RootGraphic.dbugCurrentGlobalVRoot;
            //if (visualroot == null || !visualroot.dbug_IsRecordLayoutTraceEnable)
            //{
            //    return;
            //}


            //debugVisualLay = visualroot.dbug_GetLastestVisualLayoutTracer();

            //switch (dbugFlags & 0x3)
            //{
            //    case VISUAL_ELEMENT:
            //        {
            //            RenderElement v = (RenderElement)dbugInitObject;
            //            debugVisualLay.WriteInfo(msg.msg);
            //            debugVisualLay.WriteInfo("*** init visual : " + v.dbug_FullElementDescription());
            //        } break;
            //    case LINE:
            //        {
            //        } break;
            //    case LAYER:
            //        {
            //            VisualLayer layer = (VisualLayer)dbugInitObject;
            //            debugVisualLay.WriteInfo(msg.msg);
            //            debugVisualLay.WriteInfo("*** init layer (" + layer.dbug_layer_id + "):" + layer.ToString());
            //        } break;
            //    default:
            //        {
            //            debugVisualLay.WriteInfo(msg.msg);
            //        } break;
            //}
        }
        public static void dbug_StartLayoutTrace(dbugVisualElementLayoutMsg m)
        {
            var debugVisualLay = dbugGetLayoutTracer();
            if (debugVisualLay == null) return;
            //switch (dbugFlags & 0x3)
            //{
            //    case VISUAL_ELEMENT:
            //        {
            //            RenderElement v = (RenderElement)dbugInitObject;
            //            debugVisualLay.WriteInfo(msg.msg);
            //            debugVisualLay.WriteInfo("*** init visual : " + v.dbug_FullElementDescription());
            //        } break;
            //    case LINE:
            //        {
            //        } break;
            //    case LAYER:
            //        {
            //            VisualLayer layer = (VisualLayer)dbugInitObject;
            //            debugVisualLay.WriteInfo(msg.msg);
            //            debugVisualLay.WriteInfo("*** init layer (" + layer.dbug_layer_id + "):" + layer.ToString());
            //        } break;
            //    default:
            //        {
            //            debugVisualLay.WriteInfo(msg.msg);
            //        } break;
            //}
        }

        public static void dbug_EndLayoutTrace()
        {
            var debugVisualLay = dbugGetLayoutTracer();
            if (debugVisualLay == null) return;
            debugVisualLay.WriteInfo("-----  END SESSION -------");
            //-------------------------------------------------------------
        }
        static dbugVisualLayoutTracer dbugGetLayoutTracer()
        {
            RootGraphic visualroot = RootGraphic.dbugCurrentGlobalVRoot;
            if (visualroot == null || !visualroot.dbug_IsRecordLayoutTraceEnable)
            {
                return null;
            }
            else
            {
                return visualroot.dbug_GetLastestVisualLayoutTracer();
            }
        }

        //-----------------------------------------------------------------
        protected static void dbug_WriteInfo(dbugVisitorMessage m)
        {
        }
        protected static void debug_PushTopDownElement(RenderElement ve)
        {
        }
        protected static void debug_PopTopDownElement(RenderElement ve)
        {
        }
        protected static void dbug_ExitReArrangeContent()
        {
        }
        //temp
        static object dbugInitObject;
#if DEBUG
        public void dbugShowRenderPart(Canvas canvasPage, Rectangle updateArea)
        {
            RootGraphic visualroot = this.dbugVRoot;
            if (visualroot.dbug_ShowRootUpdateArea)
            {
                canvasPage.FillRectangle(Color.FromArgb(50, Color.Black),
                     updateArea.Left, updateArea.Top,
                        updateArea.Width - 1, updateArea.Height - 1);
                canvasPage.FillRectangle(Color.White,
                     updateArea.Left, updateArea.Top, 5, 5);
                canvasPage.DrawRectangle(Color.Yellow,
                        updateArea.Left, updateArea.Top,
                        updateArea.Width - 1, updateArea.Height - 1);
                Color c_color = canvasPage.CurrentTextColor;
                canvasPage.CurrentTextColor = Color.White;
                canvasPage.DrawText(visualroot.dbug_RootUpdateCounter.ToString().ToCharArray(), updateArea.Left, updateArea.Top);
                if (updateArea.Height > 25)
                {
                    canvasPage.DrawText(visualroot.dbug_RootUpdateCounter.ToString().ToCharArray(), updateArea.Left, updateArea.Top + (updateArea.Height - 20));
                }
                canvasPage.CurrentTextColor = c_color;
                visualroot.dbug_RootUpdateCounter++;
            }
        }
        public RootGraphic dbugVisualRoot
        {
            get
            {
                return this.Root;
            }
        }
#endif 
    }
#endif
}
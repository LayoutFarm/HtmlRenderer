//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using System.Drawing;



namespace LayoutFarm.Presentation
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

    partial class ArtVisualElement
    {


        public string dbugGetCssBoxInfo
        {
            get
            {
                return dbug_FullElementDescription();
            }
        }

        public string dbug_FixedElementCode = null;
        public int dbug_element_code_y = 0; public bool dbug_hide_objIden = false;
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

        protected string dbug_GetBoundInfo()
        {



            Rectangle r = this.Rect;
            string output = "{" + r.X + "," + r.Y + "," + r.Width + "," + r.Height +
                ";dw=" + this.ElementDesiredWidth +
                ";dh=" + this.ElementDesiredHeight;
            return output;


        }
        protected string dbug_GetLayoutInfo()
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

         

        public VisualRoot dbugVRoot
        {
            get
            {
                return VisualRoot.dbugCurrentGlobalVRoot;
            }
        }

        void debug_RecordPostDrawInfo(ArtCanvas canvasPage)
        {
            if (dbugVRoot.dbug_ShowElementOutline)
            {
                canvasPage.DrawRectangle(Color.Red, new Rectangle(0, 0, this.Width - 1, this.Height - 1));

            }
            if (dbugVRoot.dbug_ForceShowObjectIden)
            {
                canvasPage.PushTextColor(Color.Blue);
                canvasPage.DrawText(
                        ("<< " + dbug_FullElementDescription()).ToCharArray()
                        , 0, dbug_element_code_y);
                canvasPage.PopTextColor();
            }
            else if (dbugVRoot.dbug_ShowObjectIden && !dbug_hide_objIden)
            {
                canvasPage.PushTextColor(Color.Blue);
                canvasPage.DrawText(
                        ("<< " + dbug_FullElementDescription()).ToCharArray()
                        , 0, dbug_element_code_y);
                canvasPage.PopTextColor();
            }


        }
    }
#endif
}
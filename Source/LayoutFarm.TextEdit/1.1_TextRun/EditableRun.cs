// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;

namespace LayoutFarm.Text
{

    public abstract class EditableRun : RenderElement
    { 
        TextSpanStyle spanStyle;
        public EditableRun(RootGraphic gfx, TextSpanStyle spanStyle)
            : base(gfx, 10, 10)
        {
            this.spanStyle = spanStyle;
            this.TransparentForAllEvents = true;
        }
        public TextSpanStyle SpanStyle
        {
            get
            {
                return this.spanStyle;
            }
        } 
        public void SetStyle(TextSpanStyle spanStyle)
        {   
            this.InvalidateGraphics();
            this.spanStyle = spanStyle;
            this.InvalidateGraphics();
            UpdateRunWidth();
        }
        public bool IsLineBreak
        {
            get;
            set;
        }

        public abstract int GetRunWidth(int charOffset);
        
        public abstract void UpdateRunWidth();
        public abstract string Text { get; }  
#if DEBUG
        public override string ToString()
        {

            return "[" + this.dbug_obj_id + "]" + Text;
        }
#endif
        public static void InnerTextRunTopDownReCalculateContentSize(EditableRun ve)
        {
#if DEBUG
            dbug_EnterTopDownReCalculateContent(ve);
#endif

            ve.UpdateRunWidth();

#if DEBUG
            dbug_ExitTopDownReCalculateContent(ve);
#endif
        }
        public override void TopDownReCalculateContentSize()
        {
            InnerTextRunTopDownReCalculateContentSize(this);
        } 
#if DEBUG
        public override string dbug_FullElementDescription()
        {
            string user_elem_id = null;
            if (user_elem_id != null)
            {
                return dbug_FixedElementCode + dbug_GetBoundInfo() + " "
                    + " i" + dbug_obj_id + "a " + ((EditableRun)this).Text + ",(ID " + user_elem_id + ") " + dbug_GetLayoutInfo();
            }
            else
            {
                return dbug_FixedElementCode + dbug_GetBoundInfo() + " "
                 + " i" + dbug_obj_id + "a " + ((EditableRun)this).Text + " " + dbug_GetLayoutInfo();
            }
        }
#endif

    }
}

//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using LayoutFarm;

namespace LayoutFarm
{

    public abstract class VisualTextRun : RenderElement
    {

        protected char[] mybuffer;
        public VisualTextRun(string s)
            : base(10, 10)
        {
            if (s != null && s.Length > 0)
            {

                mybuffer = s.ToCharArray();
                if (mybuffer.Length == 1 && mybuffer[0] == '\n')
                {
                    this.IsLineBreak = true;
                }
                UpdateRunWidth();
            }
            else
            {
                throw new Exception("string must be null or zero length");

            }
            this.TransparentForAllEvents = true;
        }

        public VisualTextRun(char c)
            : base(10, 10)
        {

            mybuffer = new char[] { c };
            if (c == '\n')
            {
                this.IsLineBreak = true;
            }

            this.TransparentForAllEvents = true;
            UpdateRunWidth();
        }
        public VisualTextRun(char[] mybuffer)
            : base(10, 10)
        {
            this.mybuffer = mybuffer;
            this.TransparentForAllEvents = true;
        }

        static char[] emptyline = new char[] { 'I' };

        TextRunStyle runStyle;
        bool isLineBreak;

        public TextRunStyle MyBoxStyle
        {
            get
            {
                return this.runStyle;
            }
        }

        public virtual void SetStyle(TextRunStyle beh)
        {


            if (beh == null)
            {
                return;
            }
            //------------------------------------------


            this.InvalidateGraphic();
            this.runStyle = beh;
            if (beh.positionWidth > -1)
            {
                this.SetWidth(beh.positionWidth);
            }
            if (beh.positionHeight > -1)
            {
                this.SetHeight(beh.positionHeight);
            }

            this.InvalidateGraphic();



            UpdateRunWidth();
        }
        protected void UpdateRunWidth()
        {
            Size size;
            if (IsLineBreak)
            {

                size = CalculateDrawingStringSize(emptyline);
            }
            else
            {
                size = CalculateDrawingStringSize(mybuffer);
            }

            this.SetSize(size.Width, size.Height);


            MarkHasValidCalculateSize();
        }
        public string Text
        {
            get
            {
                return new string(mybuffer);
            }
        }

        public int RunDesiredHeight
        {
            get
            {
                return this.ElementDesiredHeight;
            }
        }
        public int RunDesiredWidth
        {
            get
            {
                return this.ElementDesiredWidth;
            }
        }

#if DEBUG
        public override string dbug_FullElementDescription()
        {
            string user_elem_id = null;
            if (user_elem_id != null)
            {
                return dbug_FixedElementCode + dbug_GetBoundInfo() + " "
                    + " i" + dbug_obj_id + "a " + ((VisualTextRun)this).Text + ",(ID " + user_elem_id + ") " + dbug_GetLayoutInfo();
            }
            else
            {
                return dbug_FixedElementCode + dbug_GetBoundInfo() + " "
                 + " i" + dbug_obj_id + "a " + ((VisualTextRun)this).Text + " " + dbug_GetLayoutInfo();
            }
        }
#endif


        internal static void DrawArtVisualTextRun(VisualTextRun visualTextRun, Canvas canvasPage, InternalRect updateArea)
        {
            visualTextRun.DrawCharacters(canvasPage, updateArea, visualTextRun.mybuffer);
        }
        public override void CustomDrawToThisPage(Canvas canvasPage, InternalRect updateArea)
        {
            DrawArtVisualTextRun(this, canvasPage, updateArea);
        }

        protected bool HasStyle
        {
            get
            {
                return this.MyBoxStyle != null;
            }
        }
        void DrawCharacters(Canvas canvasPage, InternalRect updateArea, char[] textArray)
        {

            int bWidth = this.Width;
            int bHeight = this.Height;

            if (!this.HasStyle)
            {
                canvasPage.DrawText(textArray, new Rectangle(0, 0, bWidth, bHeight), 0);
            }
            else
            {
                TextRunStyle beh = this.MyBoxStyle;
                switch (canvasPage.EvaluateFontAndTextColor(beh.textFontInfo, beh.FontColor))
                {
                    case Canvas.DIFF_FONT_SAME_TEXT_COLOR:
                        {

                            canvasPage.PushFont(beh.textFontInfo);
                            canvasPage.DrawText(textArray,
                               new Rectangle(0, 0, bWidth, bHeight),
                               beh.ContentHAlign);
                            canvasPage.PopFont();

                        } break;
                    case Canvas.DIFF_FONT_DIFF_TEXT_COLOR:
                        {

                            canvasPage.PushFontInfoAndTextColor(beh.textFontInfo, beh.FontColor);
                            canvasPage.DrawText(textArray,
                               new Rectangle(0, 0, bWidth, bHeight),
                               beh.ContentHAlign);
                            canvasPage.PopFontInfoAndTextColor();

                        } break;
                    case Canvas.SAME_FONT_DIFF_TEXT_COLOR:
                        {
                            canvasPage.PushTextColor(beh.FontColor);
                            canvasPage.DrawText(textArray,
                            new Rectangle(0, 0, bWidth, bHeight),
                            beh.ContentHAlign);
                            canvasPage.PopTextColor();

                        } break;
                    default:
                        {
                            canvasPage.DrawText(textArray,
                               new Rectangle(0, 0, bWidth, bHeight),
                               beh.ContentHAlign);
                        } break;
                }
            }

        }

        Size CalculateDrawingStringSize(char[] buffer)
        {
            TextFontInfo textFontInfo = GetTextFontInfo();
            return new Size(
                textFontInfo.GetStringWidth(buffer),
                textFontInfo.FontHeight
                );
        }

        protected TextFontInfo GetTextFontInfo()
        {

            if (!HasStyle)
            {
                return LayoutFarm.Text.EditableTextFlowLayer.DefaultFontInfo;
            }
            else
            {
                TextRunStyle beh = (TextRunStyle)MyBoxStyle;
                if (beh != null && beh.textFontInfo != null)
                {
                    return beh.textFontInfo;
                }
                else
                {
                    return LayoutFarm.Text.EditableTextFlowLayer.DefaultFontInfo;
                }
            }
        }

#if DEBUG
        public override string ToString()
        {

            return "[" + this.dbug_obj_id + "]" + Text;
        }
#endif
        public static void InnerTextRunTopDownReCalculateContentSize(VisualTextRun ve)
        {
#if DEBUG
            vinv_dbug_EnterTopDownReCalculateContent(ve);
#endif

            ve.UpdateRunWidth();

#if DEBUG
            vinv_dbug_ExitTopDownReCalculateContent(ve);
#endif
        }
        public override void TopDownReCalculateContentSize()
        {
            InnerTextRunTopDownReCalculateContentSize(this);
        }
        public bool IsLineBreak
        {
            get
            {
                return isLineBreak;
            }
            set
            {
                this.isLineBreak = value;
            }
        }

    }
}

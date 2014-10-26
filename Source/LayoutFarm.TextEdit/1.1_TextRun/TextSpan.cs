//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;



namespace LayoutFarm.Text
{

    public abstract class TextSpan : RenderElement
    {

        protected char[] mybuffer;
        TextSpanSytle spanStyle;


        public TextSpan(RootGraphic gfx, string s)
            : base(gfx, 10, 10)
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

        public TextSpan(RootGraphic gfx, char c)
            : base(gfx, 10, 10)
        {

            mybuffer = new char[] { c };
            if (c == '\n')
            {
                this.IsLineBreak = true;
            }

            this.TransparentForAllEvents = true;
            UpdateRunWidth();
        }
        public TextSpan(RootGraphic gfx, char[] copyBuffer)
            : base(gfx, 10, 10)
        {
            //check line break?
            this.mybuffer = copyBuffer;
            this.TransparentForAllEvents = true;
        }
        public TextSpanSytle SpanStyle
        {
            get
            {
                return this.spanStyle;
            }
        }

        public void SetStyle(TextSpanSytle spanStyle)
        {


            if (spanStyle == null)
            {
                return;
            }
            //------------------------------------------


            this.InvalidateGraphic();
            this.spanStyle = spanStyle;
            if (spanStyle.positionWidth > -1)
            {
                this.SetWidth(spanStyle.positionWidth);
            }
            if (spanStyle.positionHeight > -1)
            {
                this.SetHeight(spanStyle.positionHeight);
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
                    + " i" + dbug_obj_id + "a " + ((TextSpan)this).Text + ",(ID " + user_elem_id + ") " + dbug_GetLayoutInfo();
            }
            else
            {
                return dbug_FixedElementCode + dbug_GetBoundInfo() + " "
                 + " i" + dbug_obj_id + "a " + ((TextSpan)this).Text + " " + dbug_GetLayoutInfo();
            }
        }
#endif


        internal static void DrawTextRun(TextSpan textspan, Canvas canvasPage, Rect updateArea)
        {
            textspan.DrawCharacters(canvasPage, updateArea, textspan.mybuffer);
        }
        public override void CustomDrawToThisPage(Canvas canvasPage, Rect updateArea)
        {
            DrawTextRun(this, canvasPage, updateArea);
        }

        protected bool HasStyle
        {
            get
            {
                return this.SpanStyle != null;
            }
        }
        void DrawCharacters(Canvas canvas, Rect updateArea, char[] textArray)
        {

            int bWidth = this.Width;
            int bHeight = this.Height;

            if (!this.HasStyle)
            {
                canvas.DrawText(textArray, new Rectangle(0, 0, bWidth, bHeight), 0);
            }
            else
            {
                TextSpanSytle beh = this.SpanStyle;
                switch (canvas.EvaluateFontAndTextColor(beh.FontInfo, beh.FontColor))
                {
                    case Canvas.DIFF_FONT_SAME_TEXT_COLOR:
                        {

                            canvas.PushFont(beh.FontInfo);
                            canvas.DrawText(textArray,
                               new Rectangle(0, 0, bWidth, bHeight),
                               beh.ContentHAlign);
                            canvas.PopFont();

                        } break;
                    case Canvas.DIFF_FONT_DIFF_TEXT_COLOR:
                        {

                            canvas.PushFontInfoAndTextColor(beh.FontInfo, beh.FontColor);
                            canvas.DrawText(textArray,
                               new Rectangle(0, 0, bWidth, bHeight),
                               beh.ContentHAlign);
                            canvas.PopFontInfoAndTextColor();

                        } break;
                    case Canvas.SAME_FONT_DIFF_TEXT_COLOR:
                        {
                            canvas.PushTextColor(beh.FontColor);
                            canvas.DrawText(textArray,
                            new Rectangle(0, 0, bWidth, bHeight),
                            beh.ContentHAlign);
                            canvas.PopTextColor();

                        } break;
                    default:
                        {
                            canvas.DrawText(textArray,
                               new Rectangle(0, 0, bWidth, bHeight),
                               beh.ContentHAlign);
                        } break;
                }
            }

        }

        Size CalculateDrawingStringSize(char[] buffer)
        {
            FontInfo fontInfo = GetFontInfo();
            return new Size(
                fontInfo.GetStringWidth(buffer),
                fontInfo.FontHeight
                );
        }

        protected FontInfo GetFontInfo()
        {

            if (!HasStyle)
            {
                return LayoutFarm.Text.EditableTextFlowLayer.DefaultFontInfo;
            }
            else
            {
                TextSpanSytle spanStyle = this.SpanStyle;
                if (spanStyle != null && spanStyle.FontInfo != null)
                {
                    return spanStyle.FontInfo;
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
        public static void InnerTextRunTopDownReCalculateContentSize(TextSpan ve)
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
            get;
            set;
        }


        //------------------------------------------------
        static readonly char[] emptyline = new char[] { 'I' };
    }
}

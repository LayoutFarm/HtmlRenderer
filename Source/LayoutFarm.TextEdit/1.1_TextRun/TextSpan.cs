// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;



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


            this.InvalidateGraphics();
            this.spanStyle = spanStyle;
            if (spanStyle.positionWidth > -1)
            {
                this.SetWidth(spanStyle.positionWidth);
            }
            if (spanStyle.positionHeight > -1)
            {
                this.SetHeight(spanStyle.positionHeight);
            }

            this.InvalidateGraphics();



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


        internal static void DrawTextRun(TextSpan textspan, Canvas canvasPage, Rectangle updateArea)
        {
            textspan.DrawCharacters(canvasPage, updateArea, textspan.mybuffer);
        }
        public override void CustomDrawToThisCanvas(Canvas canvas, Rectangle updateArea)
        {
            DrawTextRun(this, canvas, updateArea);
        }

        protected bool HasStyle
        {
            get
            {
                return this.SpanStyle != null;
            }
        }

        const int SAME_FONT_SAME_TEXT_COLOR = 0;
        const int SAME_FONT_DIFF_TEXT_COLOR = 1;
        const int DIFF_FONT_SAME_TEXT_COLOR = 2;
        const int DIFF_FONT_DIFF_TEXT_COLOR = 3;

        static int EvaluateFontAndTextColor(Canvas canvas, Font font, Color color)
        {
            var currentTextFont = canvas.CurrentFont;
            var currentTextColor = canvas.CurrentTextColor;

            if (font != null && font != currentTextFont)
            {
                if (currentTextColor != color)
                {
                    return DIFF_FONT_DIFF_TEXT_COLOR;
                }
                else
                {
                    return DIFF_FONT_SAME_TEXT_COLOR;
                }
            }
            else
            {
                if (currentTextColor != color)
                {
                    return SAME_FONT_DIFF_TEXT_COLOR;
                }
                else
                {
                    return SAME_FONT_SAME_TEXT_COLOR;
                }
            }
        }
        void DrawCharacters(Canvas canvas, Rectangle updateArea, char[] textArray)
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
                switch (EvaluateFontAndTextColor(canvas, beh.FontInfo.ResolvedFont, beh.FontColor))
                {
                    case DIFF_FONT_SAME_TEXT_COLOR:
                        {

                            var prevFont = canvas.CurrentFont;
                            canvas.CurrentFont = beh.FontInfo.ResolvedFont;
                            canvas.DrawText(textArray,
                               new Rectangle(0, 0, bWidth, bHeight),
                               beh.ContentHAlign);

                            canvas.CurrentFont = prevFont;
                        } break;
                    case DIFF_FONT_DIFF_TEXT_COLOR:
                        {
                            var prevFont = canvas.CurrentFont;
                            var prevColor = canvas.CurrentTextColor;

                            canvas.CurrentFont = beh.FontInfo.ResolvedFont;
                            canvas.CurrentTextColor = beh.FontColor;
                            canvas.DrawText(textArray,
                               new Rectangle(0, 0, bWidth, bHeight),
                               beh.ContentHAlign);

                            canvas.CurrentFont = prevFont;
                            canvas.CurrentTextColor = prevColor;

                        } break;
                    case SAME_FONT_DIFF_TEXT_COLOR:
                        {
                            var prevColor = canvas.CurrentTextColor;
                            canvas.DrawText(textArray,
                            new Rectangle(0, 0, bWidth, bHeight),
                            beh.ContentHAlign);
                            canvas.CurrentTextColor = prevColor;

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
                return LayoutFarm.Text.TextEditRenderBox.DefaultFontInfo;
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
                    return LayoutFarm.Text.TextEditRenderBox.DefaultFontInfo;
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
        public bool IsLineBreak
        {
            get;
            set;
        }


        //------------------------------------------------
        static readonly char[] emptyline = new char[] { 'I' };
    }
}

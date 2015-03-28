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
        TextSpanStyle spanStyle;

        public TextSpan(RootGraphic gfx, string s, TextSpanStyle style)
            : base(gfx, 10, 10)
        {
            this.spanStyle = style;
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

        public TextSpan(RootGraphic gfx, char c, TextSpanStyle style)
            : base(gfx, 10, 10)
        {
            this.spanStyle = style;
            mybuffer = new char[] { c };
            if (c == '\n')
            {
                this.IsLineBreak = true;
            }

            this.TransparentForAllEvents = true;
            UpdateRunWidth();
        }
        public TextSpan(RootGraphic gfx, char[] copyBuffer, TextSpanStyle style)
            : base(gfx, 10, 10)
        {
            this.spanStyle = style;
            //check line break?
            this.mybuffer = copyBuffer;
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
        public void UpdateRunWidth()
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
                return !this.SpanStyle.IsEmpty();
            }
        }

        const int SAME_FONT_SAME_TEXT_COLOR = 0;
        const int SAME_FONT_DIFF_TEXT_COLOR = 1;
        const int DIFF_FONT_SAME_TEXT_COLOR = 2;
        const int DIFF_FONT_DIFF_TEXT_COLOR = 3;

        static int EvaluateFontAndTextColor(Canvas canvas, TextSpanStyle spanStyle)
        {
            var font = spanStyle.FontInfo.ResolvedFont;
            var color = spanStyle.FontColor;
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
                TextSpanStyle style = this.SpanStyle;
                switch (EvaluateFontAndTextColor(canvas, style))
                {
                    case DIFF_FONT_SAME_TEXT_COLOR:
                        {

                            var prevFont = canvas.CurrentFont;
                            canvas.CurrentFont = style.FontInfo.ResolvedFont;
                            canvas.DrawText(textArray,
                               new Rectangle(0, 0, bWidth, bHeight),
                               style.ContentHAlign);

                            canvas.CurrentFont = prevFont;
                        } break;
                    case DIFF_FONT_DIFF_TEXT_COLOR:
                        {
                            var prevFont = canvas.CurrentFont;
                            var prevColor = canvas.CurrentTextColor;

                            canvas.CurrentFont = style.FontInfo.ResolvedFont;
                            canvas.CurrentTextColor = style.FontColor;
                            canvas.DrawText(textArray,
                               new Rectangle(0, 0, bWidth, bHeight),
                               style.ContentHAlign);

                            canvas.CurrentFont = prevFont;
                            canvas.CurrentTextColor = prevColor;

                        } break;
                    case SAME_FONT_DIFF_TEXT_COLOR:
                        {
                            var prevColor = canvas.CurrentTextColor;
                            canvas.DrawText(textArray,
                            new Rectangle(0, 0, bWidth, bHeight),
                            style.ContentHAlign);
                            canvas.CurrentTextColor = prevColor;
                        } break;
                    default:
                        {
                            canvas.DrawText(textArray,
                               new Rectangle(0, 0, bWidth, bHeight),
                               style.ContentHAlign);
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
                return this.Root.DefaultTextEditFontInfo;
            }
            else
            {
                TextSpanStyle spanStyle = this.SpanStyle;
                if (spanStyle.FontInfo != null)
                {
                    return spanStyle.FontInfo;
                }
                else
                {
                    return this.Root.DefaultTextEditFontInfo;
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

        static readonly char[] emptyline = new char[] { 'I' };

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

    }
}

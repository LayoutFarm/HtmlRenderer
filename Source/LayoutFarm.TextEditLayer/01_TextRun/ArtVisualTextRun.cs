using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using LayoutFarm.Presentation;

namespace LayoutFarm.Presentation
{

    public abstract class ArtVisualTextRun : ArtVisualElement
    {

        protected char[] mybuffer;
        public ArtVisualTextRun(string s)
            : base(10, 10, VisualElementNature.TextRun)
        {
            if (s != null && s.Length > 0)
            {

                mybuffer = s.ToCharArray();
                if (mybuffer.Length == 1 && mybuffer[0] == '\n')
                {
                    this.IsLineBreak = true;
                }
                UpdateRunWidth(null);
            }
            else
            {
                throw new Exception("string must be null or zero length");

            }
            this.TransparentForAllEvents = true;
        }

        public ArtVisualTextRun(char c)
            : base(10, 10, VisualElementNature.TextRun)
        {

            mybuffer = new char[] { c };
            if (c == '\n')
            {
                this.IsLineBreak = true;
            }

            this.TransparentForAllEvents = true;
            UpdateRunWidth(null);
        }
        public ArtVisualTextRun(char[] mybuffer)
            : base(10, 10, VisualElementNature.TextRun)
        {
            this.mybuffer = mybuffer;
            this.TransparentForAllEvents = true;
        }

        static char[] emptyline = new char[] { 'I' };

        protected void UpdateRunWidth(VisualElementArgs vinv)
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

            this.SetSize(size.Width, size.Height, vinv);


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
                    + " i" + dbug_obj_id + "a " + ((ArtVisualTextRun)this).Text + ",(ID " + user_elem_id + ") " + dbug_GetLayoutInfo();
            }
            else
            {
                return dbug_FixedElementCode + dbug_GetBoundInfo() + " "
                 + " i" + dbug_obj_id + "a " + ((ArtVisualTextRun)this).Text + " " + dbug_GetLayoutInfo();
            }
        }
#endif
        public void SetStyle(BoxStyle value, VisualElementArgs vinv)
        {
                                    this.SetBehavior(value, vinv);
                        UpdateRunWidth(vinv);
        }

                                                        internal static void DrawArtVisualTextRun(ArtVisualTextRun visualTextRun, ArtCanvas canvasPage, InternalRect updateArea)
        {
                        visualTextRun.DrawCharacters(canvasPage, updateArea, visualTextRun.mybuffer);
        }
        public override void CustomDrawToThisPage(ArtCanvas canvasPage, InternalRect updateArea)
        {
            DrawArtVisualTextRun(this, canvasPage, updateArea);
        }


        void DrawCharacters(ArtCanvas canvasPage, InternalRect updateArea, char[] textArray)
        {

            int bWidth = this.Width;
            int bHeight = this.Height;

            if (!this.HasBeh)
            {
                                canvasPage.DrawText(textArray, new Rectangle(0, 0, bWidth, bHeight), 0);
            }
            else
            {
                BoxStyle beh = (BoxStyle)this.Beh;
                                switch (canvasPage.EvaluateFontAndTextColor(beh.textFontInfo, beh.FontColor))
                {
                    case ArtCanvas.DIFF_FONT_SAME_TEXT_COLOR:
                        {

                            canvasPage.PushFont(beh.textFontInfo);
                            canvasPage.DrawText(textArray,
                               new Rectangle(0, 0, bWidth, bHeight),
                               beh.ContentHAlign);
                            canvasPage.PopFont();

                        } break;
                    case ArtCanvas.DIFF_FONT_DIFF_TEXT_COLOR:
                        {

                            canvasPage.PushFontInfoAndTextColor(beh.textFontInfo, beh.FontColor);
                            canvasPage.DrawText(textArray,
                               new Rectangle(0, 0, bWidth, bHeight),
                               beh.ContentHAlign);
                            canvasPage.PopFontInfoAndTextColor();

                        } break;
                    case ArtCanvas.SAME_FONT_DIFF_TEXT_COLOR:
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
            if (!HasBeh)
            {
                return FontManager.DefaultTextFontInfo;
            }
            else
            {
                                BoxStyle beh = (BoxStyle)Beh;
                if (beh != null && beh.textFontInfo != null)
                {
                    return beh.textFontInfo;
                                    }
                else
                {
                    return FontManager.DefaultTextFontInfo;
                }
            }
        }

        #if DEBUG
        public override string ToString()
        {

            return "[" + this.dbug_obj_id + "]" + Text;
        }
#endif
        public static void InnerTextRunTopDownReCalculateContentSize(ArtVisualTextRun ve, VisualElementArgs vinv)
        {
                                                            #if DEBUG
            vinv.dbug_EnterTopDownReCalculateContent(ve);
#endif

            ve.UpdateRunWidth(vinv);

#if DEBUG
            vinv.dbug_ExitTopDownReCalculateContent(ve);
#endif
        }
        public override void TopDownReCalculateContentSize(VisualElementArgs vinv)
        {
            InnerTextRunTopDownReCalculateContentSize(this, vinv);
        }
                        

    }
}

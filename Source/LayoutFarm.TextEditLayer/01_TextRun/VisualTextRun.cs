//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using LayoutFarm.Presentation;

namespace LayoutFarm.Presentation
{

    public abstract class VisualTextRun : RenderElement
    {

        protected char[] mybuffer;
        public VisualTextRun(string s)
            : base(10, 10, ElementNature.TextRun)
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

        public VisualTextRun(char c)
            : base(10, 10, ElementNature.TextRun)
        {

            mybuffer = new char[] { c };
            if (c == '\n')
            {
                this.IsLineBreak = true;
            }

            this.TransparentForAllEvents = true;
            UpdateRunWidth(null);
        }
        public VisualTextRun(char[] mybuffer)
            : base(10, 10, ElementNature.TextRun)
        {

            this.mybuffer = mybuffer;
            this.TransparentForAllEvents = true;
        }

        static char[] emptyline = new char[] { 'I' };
        object basicStyle;
        bool isLineBreak;

        public TextRunStyle MyBoxStyle
        {
            get
            {
                if (this.basicStyle != null)
                {
                    return (TextRunStyle)this.basicStyle;

                }
                return null;
            }
        }

        public virtual void SetStyle(TextRunStyle newbeh, LayoutPhaseVisitor vinv)
        {

            TextRunStyle beh = (TextRunStyle)newbeh;
            if (newbeh == null)
            {
                return;
            }

            if ((uiFlags & USE_ANIMATOR) == 0)
            {
                if (vinv != null)
                {
                    this.InvalidateGraphic(vinv);
                }

                this.basicStyle = beh;
                if (beh.positionWidth > -1)
                {
                    this.SetWidth(beh.positionWidth, vinv);
                }
                if (beh.positionHeight > -1)
                {
                    this.SetHeight(beh.positionHeight, vinv);
                }
                if (vinv != null)
                {
                    this.InvalidateGraphic(vinv);
                }
            }
            else
            {

            }


            UpdateRunWidth(vinv);
        }
        protected void UpdateRunWidth(LayoutPhaseVisitor vinv)
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
                    + " i" + dbug_obj_id + "a " + ((VisualTextRun)this).Text + ",(ID " + user_elem_id + ") " + dbug_GetLayoutInfo();
            }
            else
            {
                return dbug_FixedElementCode + dbug_GetBoundInfo() + " "
                 + " i" + dbug_obj_id + "a " + ((VisualTextRun)this).Text + " " + dbug_GetLayoutInfo();
            }
        }
#endif


        internal static void DrawArtVisualTextRun(VisualTextRun visualTextRun, CanvasBase canvasPage, InternalRect updateArea)
        {
            visualTextRun.DrawCharacters(canvasPage, updateArea, visualTextRun.mybuffer);
        }
        public override void CustomDrawToThisPage(CanvasBase canvasPage, InternalRect updateArea)
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
        void DrawCharacters(CanvasBase canvasPage, InternalRect updateArea, char[] textArray)
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
                    case CanvasBase.DIFF_FONT_SAME_TEXT_COLOR:
                        {

                            canvasPage.PushFont(beh.textFontInfo);
                            canvasPage.DrawText(textArray,
                               new Rectangle(0, 0, bWidth, bHeight),
                               beh.ContentHAlign);
                            canvasPage.PopFont();

                        } break;
                    case CanvasBase.DIFF_FONT_DIFF_TEXT_COLOR:
                        {

                            canvasPage.PushFontInfoAndTextColor(beh.textFontInfo, beh.FontColor);
                            canvasPage.DrawText(textArray,
                               new Rectangle(0, 0, bWidth, bHeight),
                               beh.ContentHAlign);
                            canvasPage.PopFontInfoAndTextColor();

                        } break;
                    case CanvasBase.SAME_FONT_DIFF_TEXT_COLOR:
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
                return LayoutFarm.Presentation.Text.EditableTextFlowLayer.DefaultFontInfo;
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
                    return LayoutFarm.Presentation.Text.EditableTextFlowLayer.DefaultFontInfo;
                }
            }
        }

#if DEBUG
        public override string ToString()
        {

            return "[" + this.dbug_obj_id + "]" + Text;
        }
#endif
        public static void InnerTextRunTopDownReCalculateContentSize(VisualTextRun ve, LayoutPhaseVisitor vinv)
        {
#if DEBUG
            vinv.dbug_EnterTopDownReCalculateContent(ve);
#endif

            ve.UpdateRunWidth(vinv);

#if DEBUG
            vinv.dbug_ExitTopDownReCalculateContent(ve);
#endif
        }
        public override void TopDownReCalculateContentSize(LayoutPhaseVisitor vinv)
        {
            InnerTextRunTopDownReCalculateContentSize(this, vinv);
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

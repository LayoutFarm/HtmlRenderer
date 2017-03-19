//Apache2, 2014-2017, WinterDev

using System; 
using System.Collections.Generic;
using System.Text;
using PixelFarm.Drawing;
 
using LayoutFarm.Text;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class TextBox : UIBox
    {
        TextSurfaceEventListener textSurfaceListener;
        TextEditRenderBox textEditRenderElement;
        bool _multiline;
        TextSpanStyle defaultSpanStyle;
        Color backgroundColor = Color.White;
        string userTextContent;
        public TextBox(int width, int height, bool multiline)
            : base(width, height)
        {
            this._multiline = multiline;
        }
        public void ClearText()
        {
            if (textEditRenderElement != null)
            {
                this.textEditRenderElement.ClearAllChildren();
            }
        }
        public Color BackgroundColor
        {
            get { return this.backgroundColor; }
            set
            {
                this.backgroundColor = value;
                if (textEditRenderElement != null)
                {
                    textEditRenderElement.BackgroundColor = value;
                }
            }
        }
        public TextSpanStyle DefaultSpanStyle
        {
            get { return this.defaultSpanStyle; }
            set
            {
                this.defaultSpanStyle = value;
                if (textEditRenderElement != null)
                {
                    textEditRenderElement.CurrentTextSpanStyle = value;
                     
                }
            }
        }
        public ContentTextSplitter TextSplitter
        {
            get;
            set;
        }
        public Point CaretPosition
        {
            get { return this.textEditRenderElement.CurrentCaretPos; }
        }
        public int CurrentLineCharIndex
        {
            get { return this.textEditRenderElement.CurrentLineCharIndex; }
        }
        public string Text
        {
            get
            {
                if (textEditRenderElement != null)
                {
                    StringBuilder stBuilder = new StringBuilder();
                    textEditRenderElement.CopyContentToStringBuilder(stBuilder);
                    return stBuilder.ToString();
                }
                else
                {
                    return userTextContent;
                }
            }
            set
            {
                if (textEditRenderElement == null)
                {
                    this.userTextContent = value;
                    return;
                }
                //---------------                 

                this.textEditRenderElement.ClearAllChildren();
                //convert to runs
                if (value == null)
                {
                    return;
                }
                //---------------                 
                var reader = new System.IO.StringReader(value);
                string line = reader.ReadLine();
                int lineCount = 0;
                while (line != null)
                {
                    if (lineCount > 0)
                    {
                        textEditRenderElement.SplitCurrentLineToNewLine();
                    }

                    //create textspan
                    //user can parse text line to smaller span
                    //eg. split by whitespace 

                    if (this.TextSplitter != null)
                    {
                        //parse with textsplitter 
                        var buffer = value.ToCharArray();
                        foreach (var splitBound in TextSplitter.ParseWordContent(buffer, 0, buffer.Length))
                        {
                            var startIndex = splitBound.startIndex;
                            var length = splitBound.length;
                            var splitBuffer = new char[length];
                            Array.Copy(buffer, startIndex, splitBuffer, 0, length);

                            //TODO: review
                            //this just test ***  that text box can hold freeze text run
                            //var textspan = textEditRenderElement.CreateFreezeTextRun(splitBuffer);
                            //-----------------------------------
                            //but for general 
                            var textspan = textEditRenderElement.CreateEditableTextRun(splitBuffer);
                            textEditRenderElement.AddTextRun(textspan);
                        }
                    }
                    else
                    {
                        var textspan = textEditRenderElement.CreateEditableTextRun(line);
                        textEditRenderElement.AddTextRun(textspan);
                    }


                    lineCount++;
                    line = reader.ReadLine();
                }

                this.InvalidateGraphics();
            }
        }
        public override void Focus()
        {
            //request keyboard focus
            base.Focus();
            textEditRenderElement.Focus();
        }
        public override void Blur()
        {
            base.Blur();
        }
        public void DoHome()
        {
            this.textEditRenderElement.DoHome(false);
        }
        public void DoEnd()
        {
            this.textEditRenderElement.DoEnd(false);
        }
        protected override bool HasReadyRenderElement
        {
            get { return this.textEditRenderElement != null; }
        }
        public override RenderElement CurrentPrimaryRenderElement
        {
            get { return this.textEditRenderElement; }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (textEditRenderElement == null)
            {
                var tbox = new TextEditRenderBox(rootgfx, this.Width, this.Height, _multiline);
                tbox.SetLocation(this.Left, this.Top);
                tbox.HasSpecificSize = true;
                if (this.defaultSpanStyle.IsEmpty())
                {
                    this.defaultSpanStyle = new TextSpanStyle();
                    this.defaultSpanStyle.FontInfo = rootgfx.DefaultTextEditFontInfo;
                    tbox.CurrentTextSpanStyle = this.defaultSpanStyle;
                }
                else
                {
                    tbox.CurrentTextSpanStyle = this.defaultSpanStyle;
                }
                tbox.BackgroundColor = this.backgroundColor;
                tbox.SetController(this);
                
                if (this.textSurfaceListener != null)
                {
                    tbox.TextSurfaceListener = textSurfaceListener;
                }
                this.textEditRenderElement = tbox;
                if (userTextContent != null)
                {
                    this.Text = userTextContent;
                    userTextContent = null;//clear
                }
            }
            return textEditRenderElement;
        }
        //----------------------------------------------------------------
        public bool IsMultilineTextBox
        {
            get { return this._multiline; }
        }
        public TextSurfaceEventListener TextEventListener
        {
            get { return this.textSurfaceListener; }
            set
            {
                this.textSurfaceListener = value;
                if (this.textEditRenderElement != null)
                {
                    this.textEditRenderElement.TextSurfaceListener = value;
                }
            }
        }
        public EditableRun CurrentTextSpan
        {
            get
            {
                return this.textEditRenderElement.CurrentTextRun;
            }
        }

        public void ReplaceCurrentTextRunContent(int nBackspaces, string newstr)
        {
            if (textEditRenderElement != null)
            {
                textEditRenderElement.ReplaceCurrentTextRunContent(nBackspaces, newstr);
            }
        }
        public void ReplaceCurrentLineTextRuns(IEnumerable<EditableRun> textRuns)
        {
            if (textEditRenderElement != null)
            {
                textEditRenderElement.ReplaceCurrentLineTextRuns(textRuns);
            }
        }
        public void CopyCurrentLine(StringBuilder stbuilder)
        {
            textEditRenderElement.CopyCurrentLine(stbuilder);
        }
        //---------------------------------------------------------------- 
        protected override void OnMouseLeave(UIMouseEventArgs e)
        {
            e.MouseCursorStyle = MouseCursorStyle.Arrow;
        }
        protected override void OnDoubleClick(UIMouseEventArgs e)
        {
            textEditRenderElement.OnDoubleClick(e);
            e.CancelBubbling = true;
        }
        protected override void OnKeyPress(UIKeyEventArgs e)
        {
            textEditRenderElement.OnKeyPress(e);
            e.CancelBubbling = true;
        }
        protected override void OnKeyDown(UIKeyEventArgs e)
        {
            textEditRenderElement.OnKeyDown(e);
            e.CancelBubbling = true;
        }
        protected override void OnKeyUp(UIKeyEventArgs e)
        {
            textEditRenderElement.OnKeyUp(e);
            e.CancelBubbling = true;
        }
        protected override bool OnProcessDialogKey(UIKeyEventArgs e)
        {
            if (textEditRenderElement.OnProcessDialogKey(e))
            {
                e.CancelBubbling = true;
                return true;
            }
            return false;
        }
        protected override void OnMouseDown(UIMouseEventArgs e)
        {
            this.Focus();
            e.MouseCursorStyle = MouseCursorStyle.IBeam;
            e.CancelBubbling = true;
            e.CurrentContextElement = this;
            textEditRenderElement.OnMouseDown(e);
        }
        protected override void OnLostKeyboardFocus(UIFocusEventArgs e)
        {
            base.OnLostKeyboardFocus(e);
            textEditRenderElement.Blur();
        }
        protected override void OnMouseMove(UIMouseEventArgs e)
        {
            if (e.IsDragging)
            {
                textEditRenderElement.OnDrag(e);
                e.CancelBubbling = true;
                e.MouseCursorStyle = MouseCursorStyle.IBeam;
            }
        }
        protected override void OnMouseUp(UIMouseEventArgs e)
        {
            if (e.IsDragging)
            {
                textEditRenderElement.OnDragEnd(e);
            }
            else
            {
                textEditRenderElement.OnMouseUp(e);
            }
            e.MouseCursorStyle = MouseCursorStyle.Default;
            e.CancelBubbling = true;
        }


        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "textbox");
            this.Describe(visitor);
            visitor.TextNode(this.Text);
            visitor.EndElement();
        }
    }
}
//Apache2, 2014-2017, WinterDev

using System;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    /// <summary>
    /// textbox with decoration(eg. placeholder)
    /// </summary>
    public class TextBoxContainer : EaseBox
    {
        TextBox myTextBox;
        CustomTextRun placeHolder;
        string placeHolderText = "";
        bool multiline;
        Text.TextSurfaceEventListener textEvListener;
        public TextBoxContainer(int w, int h, bool multiline)
            : base(w, h)
        {
            this.BackColor = Color.White;
            this.multiline = multiline;
        }
        public string PlaceHolderText
        {
            get { return this.placeHolderText; }
            set
            {
                this.placeHolderText = value;
                if (this.placeHolder != null)
                {
                    this.placeHolder.Text = placeHolderText;
                    this.InvalidateGraphics();
                }
            }
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (!this.HasReadyRenderElement)
            {
                //first time
                RenderElement baseRenderElement = base.GetPrimaryRenderElement(rootgfx);
                //1. add place holder first
                placeHolder = new CustomTextRun(rootgfx, this.Width - 4, this.Height - 4);
                placeHolder.Text = placeHolderText;
                placeHolder.SetLocation(1, 1);
                placeHolder.TextColor = Color.FromArgb(180, Color.LightGray);
                baseRenderElement.AddChild(placeHolder);
                //2. textbox 
                myTextBox = new TextBox(this.Width - 4, this.Height - 4, multiline);
                myTextBox.BackgroundColor = Color.Transparent;
                myTextBox.SetLocation(2, 2);
                textEvListener = new Text.TextSurfaceEventListener();
                myTextBox.TextEventListener = textEvListener;
                textEvListener.KeyDown += new EventHandler<Text.TextDomEventArgs>(textEvListener_KeyDown);
                baseRenderElement.AddChild(myTextBox);
                return baseRenderElement;
            }
            else
            {
                return base.GetPrimaryRenderElement(rootgfx);
            }
        }
        void textEvListener_KeyDown(object sender, Text.TextDomEventArgs e)
        {
            //when key up
            //check if we should show place holder
            if (!string.IsNullOrEmpty(this.placeHolderText))
            {
                var inputText = myTextBox.Text;
                if (!string.IsNullOrEmpty(inputText))
                {
                    //hide place holder                     
                    if (placeHolder.Visible)
                    {
                        this.placeHolder.SetVisible(false);
                        this.placeHolder.InvalidateGraphics();
                    }
                }
                else
                {
                    //show place holder
                    if (!placeHolder.Visible)
                    {
                        this.placeHolder.SetVisible(true);
                        this.placeHolder.InvalidateGraphics();
                    }
                }
            }
        }
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "textbox_container");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }
}
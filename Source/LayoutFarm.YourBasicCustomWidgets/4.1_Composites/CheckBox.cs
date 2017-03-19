//Apache2, 2014-2017, WinterDev

using System;
using LayoutFarm.UI;
namespace LayoutFarm.CustomWidgets
{
    public class CheckBox : EaseBox
    {
        //check icon
        ImageBox imageBox;
        bool isChecked;
        public CheckBox(int w, int h)
            : base(w, h)
        {
        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (!this.HasReadyRenderElement)
            {
                //first time
                RenderElement baseRenderElement = base.GetPrimaryRenderElement(rootgfx);
                imageBox = new ImageBox(16, 16);
                if (this.isChecked)
                {
                    imageBox.ImageBinder = ResImageList.GetImageBinder(ImageName.CheckBoxChecked);
                }
                else
                {
                    imageBox.ImageBinder = ResImageList.GetImageBinder(ImageName.CheckBoxUnChecked);
                }

                imageBox.MouseDown += (s, e) =>
                {
                    //toggle checked/unchecked
                    this.Checked = !this.Checked;
                };
                this.AddChild(imageBox);
                return baseRenderElement;
            }
            else
            {
                return base.GetPrimaryRenderElement(rootgfx);
            }
        }
        public bool Checked
        {
            get { return this.isChecked; }
            set
            {
                if (value != this.isChecked)
                {
                    this.isChecked = value;
                    //check check image too!

                    if (this.isChecked)
                    {
                        imageBox.ImageBinder = ResImageList.GetImageBinder(ImageName.CheckBoxChecked);
                    }
                    else
                    {
                        imageBox.ImageBinder = ResImageList.GetImageBinder(ImageName.CheckBoxUnChecked);
                    }



                    if (value && this.WhenChecked != null)
                    {
                        this.WhenChecked(this, EventArgs.Empty);
                    }
                }
            }
        }
        public event EventHandler WhenChecked;
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "checkbox");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }
}
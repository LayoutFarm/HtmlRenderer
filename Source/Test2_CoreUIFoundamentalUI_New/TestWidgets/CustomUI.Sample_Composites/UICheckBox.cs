// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using PixelFarm.Drawing;

using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;
namespace LayoutFarm.CustomWidgets
{
    public interface ICheckable
    {
        bool Checked { get; }

    }

    public class UICheckBox : UIPanel, ICheckable
    {
        //check icon

        UIImageBox imageBox;
        bool isChecked;
        public UICheckBox(int w, int h)
            : base(w, h)
        {

        }
        public override RenderElement GetPrimaryRenderElement(RootGraphic rootgfx)
        {
            if (!this.HasReadyRenderElement)
            {
                //first time
                RenderElement baseRenderElement = base.GetPrimaryRenderElement(rootgfx);
                imageBox = new UIImageBox(16, 16);

                if (this.isChecked)
                {
                    imageBox.Image = ResImageList.GetImage(ImageName.CheckBoxChecked);
                }
                else
                {
                    imageBox.Image = ResImageList.GetImage(ImageName.CheckBoxUnChecked);
                }

                imageBox.MouseDown += (s, e) =>
                {
                    //toggle checked/unchecked
                    this.Checked = !this.Checked;

                };
                this.AddChildBox(imageBox);
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
                        imageBox.Image = ResImageList.GetImage(ImageName.CheckBoxChecked);
                    }
                    else
                    {
                        imageBox.Image = ResImageList.GetImage(ImageName.CheckBoxUnChecked);
                    }

                    if (value && this.WhenChecked != null)
                    {
                        this.WhenChecked(this, EventArgs.Empty);
                    }
                }
            }
        }

        public event EventHandler WhenChecked;


    }


}
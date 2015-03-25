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

    public class CustomRenderBox : RenderBoxBase
    {
        Color backColor;
#if DEBUG
        public bool dbugBreak;
#endif
        public CustomRenderBox(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
            this.BackColor = Color.LightGray;

        }

        public override void ClearAllChildren()
        {

        }
        public Color BackColor
        {
            get { return this.backColor; }
            set
            {
                this.backColor = value;
                if (this.ParentLink != null)
                {
                    this.InvalidateGraphics();
                }
            }

        }
        protected override void DrawContent(Canvas canvas, Rectangle updateArea)
        {

#if DEBUG
            if (this.dbugBreak)
            {

            }
#endif
            //sample bg   
            //canvas.FillRectangle(BackColor, updateArea.Left, updateArea.Top, updateArea.Width, updateArea.Height);
            canvas.FillRectangle(BackColor, 0, 0, this.Width, this.Height);
            if (this.Layer != null)
            {
                this.Layer.DrawChildContent(canvas, updateArea);
            }

            //if (this.Layers != null)
            //{
            //    this.Layers.LayersDrawContent(canvas, updateArea);
            //}
#if DEBUG
            //canvas.dbug_DrawCrossRect(PixelFarm.Drawing.Color.Black,
            //    new Rectangle(0, 0, this.Width, this.Height));

            //canvas.dbug_DrawCrossRect(PixelFarm.Drawing.Color.Black,
            //   new Rectangle(updateArea.Left, updateArea.Top, updateArea.Width, updateArea.Height));
#endif
        }
        public PlainLayer GetExitingLayerOrCreateNew()
        {
            if (this.Layer == null)
            {
                var layer = new PlainLayer(this);
                this.Layer = layer;
                return layer;                 
            }
            return this.Layer as PlainLayer;
        }
    }



}
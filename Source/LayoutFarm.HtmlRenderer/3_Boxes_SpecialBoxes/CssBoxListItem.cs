//BSD 2014,WinterDev

using System;
using System.Collections.Generic;
using LayoutFarm.Drawing;

namespace LayoutFarm.HtmlBoxes
{

    public class CssBoxListItem : CssBox
    {
        CssBox _listItemBulletBox;
        public CssBoxListItem(object controller, Css.BoxSpec spec)
            : base(controller, spec)
        {
        }
        public CssBox BulletBox
        {
            get
            {
                return this._listItemBulletBox;
            }
            set
            {
                this._listItemBulletBox = value;
            }
        }
        protected override void PerformContentLayout(LayoutVisitor lay)
        {

            base.PerformContentLayout(lay);

            if (_listItemBulletBox != null)
            {
                //layout list item
                var prevSibling = lay.LatestSiblingBox;
                lay.LatestSiblingBox = null;//reset
                _listItemBulletBox.PerformLayout(lay);
                lay.LatestSiblingBox = prevSibling;
                var fRun = _listItemBulletBox.FirstRun;
                _listItemBulletBox.FirstRun.SetSize(fRun.Width, fRun.Height);
                _listItemBulletBox.FirstRun.SetLocation(_listItemBulletBox.SizeWidth - 5, this.ActualPaddingTop);
            }
        }
        protected override void PaintImp(BoxPainter p)
        {
            base.PaintImp(p);

        }
    }
}

namespace HtmlRenderer.RenderDom
{

    /// <summary>
    /// for special sub layer of a CssBox,
    /// (storing list-item, element shadow etc)
    /// </summary>
    class SubBoxCollection
    {
        CssBox _listItemBulletBox;
        public SubBoxCollection()
        { 
        }
        public CssBox ListItemBulletBox
        {
            get { return this._listItemBulletBox; }
            set { this._listItemBulletBox = value; }
        }
        public void PerformLayout(CssBox owner, LayoutVisitor lay)
        {   
            if (_listItemBulletBox != null)
            {
                //layout list item
                
                var prevSibling = lay.LatestSiblingBox;
                lay.LatestSiblingBox = null;//reset
                _listItemBulletBox.PerformLayout(lay);
                lay.LatestSiblingBox = prevSibling; 
                var fRun = _listItemBulletBox.FirstRun;
                _listItemBulletBox.FirstRun.SetSize(fRun.Width, fRun.Height);
                _listItemBulletBox.FirstRun.SetLocation(_listItemBulletBox.SizeWidth - 5, owner.ActualPaddingTop);
            }
        }
    }


}
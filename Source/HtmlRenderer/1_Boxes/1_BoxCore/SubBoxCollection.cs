
namespace HtmlRenderer.Dom
{

    /// <summary>
    /// for special sub layer of a CssBox,
    /// (storing list-item, element shadow etc)
    /// </summary>
    class SubBoxCollection
    {
        CssBox _listItemBox;
         
        public SubBoxCollection()
        {
             
        }
        public CssBox ListItemBox
        {
            get { return this._listItemBox; }
            set { this._listItemBox = value; }
        }
        
    }


}
// 2015,2014 ,BSD, WinterDev 
//ArthurHub 


namespace LayoutFarm.Css
{

    partial class BoxSpec
    {


        public static void InheritStyles(BoxSpec target, BoxSpec source)
        {
            //if (__aa_dbugId == 11)
            //{

            //}
            //----------------------------------------
            if (source == null)
            {
                return;
            }
            //---------------------------------------
            //only inheritable feautures
            target._fontFeats = source._fontFeats;
            target._listFeats = source._listFeats;
            //--------------------------------------- 
            target._lineHeight = source._lineHeight;
            target._textIndent = source._textIndent;
            target._actualColor = source._actualColor;
            target._emptyCells = source._emptyCells;
            //--------------------------------------- 
            target._textAlign = source._textAlign;
            target._verticalAlign = source._verticalAlign;
            target._visibility = source._visibility;
            target._whitespace = source._whitespace;
            target._wordBreak = source._wordBreak;
            target._cssDirection = source._cssDirection;
            //--------------------------------------- 

        }
        public static void CloneAllStyles(BoxSpec target, BoxSpec source)
        {
            //1.
            //=====================================
            //if (s._fontFeats.Owner == s)
            //{
            //    //this._fontFeats = s._fontFeats;
            //}
            target._fontFeats = source._fontFeats;
            target._listFeats = source._listFeats;
            //--------------------------------------- 
            target._lineHeight = source._lineHeight;
            target._textIndent = source._textIndent;
            target._actualColor = source._actualColor;
            target._emptyCells = source._emptyCells;
            //--------------------------------------- 
            target._textAlign = source._textAlign;
            target._verticalAlign = source._verticalAlign;
            target._visibility = source._visibility;
            target._whitespace = source._whitespace;
            target._wordBreak = source._wordBreak;
            target._cssDirection = source._cssDirection;
            //---------------------------------------

            //2.
            //for clone only (eg. split a box into two parts)
            //=======================================
            target._backgroundFeats = source._backgroundFeats;
            target._borderFeats = source._borderFeats;

            target._cornerFeats = source._cornerFeats;
            target._marginFeats = source._marginFeats;
            target._paddingFeats = source._paddingFeats;
            //---------------------------------------
            target._cssDisplay = source._cssDisplay;
            target._left = source._left;
            target._top = source._top;
            target._bottom = source._bottom;
            target._right = source._right;

            target._width = source._width;
            
            target._height = source._height;
            target._maxWidth = source._maxWidth;
            target._position = source._position;

            target._wordSpacing = source._wordSpacing;
            target._lineHeight = source._lineHeight;
            target._float = source._float;


            target._overflow = source._overflow;
            target._textDecoration = source._textDecoration;

            //3.
            //===================================== 
            target._cssDirection = source._cssDirection;
        }

        public static void SetVersionNumber(BoxSpec target, int versionNumber)
        {
            target._versionNum = versionNumber;
        }
        public int VersionNumber
        {
            get
            {
                return this._versionNum;
            }
        }

        public BoxSpec GetAnonVersion()
        {
            if (anonVersion != null)
            {
                return anonVersion;
            }

            this.anonVersion = new BoxSpec();
            BoxSpec.InheritStyles(anonVersion, this);
            anonVersion.Freeze();

            return anonVersion;
        }
        //---------------------------------------------------------------

        CssBorderFeature CheckBorderVersion()
        {
            return this._borderFeats = this._borderFeats.GetMyOwnVersion(this);
        }
        CssMarginFeature CheckMarginVersion()
        {
            return this._marginFeats = this._marginFeats.GetMyOwnVersion(this);
        }
        CssPaddingFeature CheckPaddingVersion()
        {
            return this._paddingFeats = this._paddingFeats.GetMyOwnVersion(this);
        }
        CssCornerFeature CheckCornerVersion()
        {
            return this._cornerFeats = this._cornerFeats.GetMyOwnVersion(this);
        }
        CssFontFeature CheckFontVersion()
        {
            return this._fontFeats = this._fontFeats.GetMyOwnVersion(this);
        }
        CssListFeature CheckListPropVersion()
        {
            return this._listFeats = this._listFeats.GetMyOwnVersion(this);
        }
        CssBackgroundFeature CheckBgVersion()
        {
            return this._backgroundFeats = this._backgroundFeats.GetMyOwnVersion(this);
        }


    }

}
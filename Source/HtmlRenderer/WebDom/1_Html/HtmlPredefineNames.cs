//BSD  2014 ,WinterCore

using System;
using System.Text;
using System.Collections;


namespace HtmlRenderer.WebDom
{
    public static class HtmlPredefineNames
    {
        static UniqueStringTable xhtmlUniqueTableTemplate = new UniqueStringTable();
        
        public const int Img = 1;
        public const int Style = 2;
        public const int Class = 3;
        public const int Link = 4;
        public const int Rel = 5;
        public const int HRef = 6;
        public const int Align = 7;
        public const int Background = 8;
        public const int BgColor = 9;
        public const int Border = 10;

        public const int Table = 11;
        public const int BorderColor = 12;
        public const int CellSpacing = 13;
        public const int CellPadding = 14;
        public const int Color = 15;
        public const int Dir = 16;
        public const int Face = 17;


        public const int Height = 18;
        public const int HSpace = 19;
        public const int NoWrap = 20;
        public const int Size = 21;
        public const int VAlign = 22;
        public const int VSpace = 23;

        public const int HR = 24;
        public const int Width = 25;
        public const int Td = 26;
        public const int Th = 27;
        public const int Src = 28;
        public const int ColSpan = 29;
        public const int RowSpan = 30;
        public const int ColGroup = 31;
        public const int Span = 32;
        public const int Col = 33;
        public const int XObj = 34;
        public const int Ui = 35;
        public const int Br = 36;
        //---------------------------------
        static HtmlPredefineNames()
        {
            string[] preDefinedStrings = new string[]{
                "img",//1
                "style",//2
                "class",//3
                "link",//4
                "rel",//5
                "href",//6
                "align",//7
                "background",//8
                "bgcolor",//9
                "border",//10                
                "table",//11
                "bordercolor",//12
                "cellspacing",//13
                "cellpadding",//14
                "color",//15
                "dir",//16
                "face",//17
                "height",//18
                "hspace",//19
                "nowrap",//20
                "size",//21
                "valign",//22
                "vspace",//23

                "hr",//24
                "width",//25
                "td",//26
                "th",//27
                "src",//28
                "colspan",//29
                "rowspan",//30
                "colGroup",//31
                "span",//32
                "col",//33
                "xobj",//34 -- extension object
                "ui",//35
                "br",//36
            };

            int j = preDefinedStrings.Length;
            for (int i = 0; i < j; ++i)
            {
                xhtmlUniqueTableTemplate.AddStringIfNotExist(preDefinedStrings[i]);
            } 
        }
        internal static UniqueStringTable CreateUniqueStringTableClone()
        {
            return xhtmlUniqueTableTemplate.Clone();
        }

    }


}
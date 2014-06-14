//BSD 2014, WinterCore
//ArthurHub 

using System;
using System.Collections.Generic;
using System.Drawing;
using HtmlRenderer.Entities;
using HtmlRenderer.Parse;
using HtmlRenderer.Utils;

namespace HtmlRenderer.Dom
{
    partial class CssTableLayoutEngine
    {



        class TableColumn
        {
            CssBox columnBox;
            public TableColumn(CssBox columnBox)
            {
                this.columnBox = columnBox;
            }

        }
        class TableColumnCollection
        {
            List<TableColumn> columns = new List<TableColumn>();
        } 
    }
}
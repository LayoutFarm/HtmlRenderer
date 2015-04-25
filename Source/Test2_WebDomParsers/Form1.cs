using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using LayoutFarm.WebDom;
using LayoutFarm.WebDom.Parser;
namespace Test2_WebDomParsers
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //test web parser
            var parser = new HtmlParser();
            var blankHtmlDoc = new LayoutFarm.WebDom.Impl.HtmlDocument();
            var snapSource = new TextSource("<html><body><div>AAA</div></body></html>".ToCharArray());
            parser.Parse(snapSource, blankHtmlDoc, blankHtmlDoc.RootNode);

        }
    }
}

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
            var snapSource = new TextSource(this.richTextBox1.Text.ToCharArray());
            parser.Parse(snapSource, blankHtmlDoc, blankHtmlDoc.RootNode);

            this.treeView2.Nodes.Clear();
            var rootNode = new TreeNode("root");
            
            DescibeNode(blankHtmlDoc.RootNode, rootNode);

            treeView2.Nodes.Add(rootNode);
            this.treeView2.ExpandAll();
        }
        void DescibeNode(DomNode node, TreeNode parentNode)
        {
            var node_info = node.ToString();
            var treeNode = new TreeNode(node_info);
            parentNode.Nodes.Add(treeNode);
            var domElt = node as DomElement;
            if (domElt != null)
            {
                var childCount = domElt.ChildrenCount;
                for (int i = 0; i < childCount; ++i)
                {
                    DescibeNode(domElt.GetChildNode(i), treeNode);
                }
            }

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            //load sample files
            LoadHtmlSamples(this.treeView1);
        }
        void LoadHtmlSamples(TreeView _samplesTreeView)
        {

            //find sample folder 
            string execFromFolder = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

#if DEBUG
            string checkFolder = "\\Source\\Test2_WebDomParsers\\bin\\Debug";
#else
            string checkFolder = "\\Source\\Test2_WebDomParsers\\bin\\Release";
#endif


            //only from debug ?
            if (!execFromFolder.EndsWith(checkFolder))
            {
                return;
            }

            int index = execFromFolder.LastIndexOf(checkFolder);
            string rootSampleFolder = execFromFolder.Substring(0, index) + "\\Source\\Test2_WebDomParsers\\SampleData";

            var root = new TreeNode("HTML Renderer");
            _samplesTreeView.Nodes.Add(root);

            string[] sampleDirs = System.IO.Directory.GetDirectories(rootSampleFolder);

            //only 1 file level (not recursive)
            foreach (string dirName in sampleDirs)
            {
                var dirNode = new TreeNode(System.IO.Path.GetFileName(dirName));
                root.Nodes.Add(dirNode);
                string[] fileNames = System.IO.Directory.GetFiles(dirName, "*.htm");
                foreach (string fname in fileNames)
                {
                    var onlyFileName = System.IO.Path.GetFileName(fname);
                    if (!onlyFileName.StartsWith("x"))
                    {
                        //for our convention: 
                        //file start with x will not show here  
                        //(it used for comment out/backup file)
                        var fileNameNode = new TreeNode(System.IO.Path.GetFileName(fname));
                        dirNode.Nodes.Add(fileNameNode);
                        fileNameNode.Tag = fname;
                    }
                }
            }
            root.ExpandAll();

            _samplesTreeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(_samplesTreeView_NodeMouseClick);
        }

        void _samplesTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //load selected file name
            var filename = e.Node.Tag as string;
            if (filename != null)
            {
                richTextBox1.Text = System.IO.File.ReadAllText(filename);
            }
        }

    }
}

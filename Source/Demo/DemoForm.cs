// "Therefore those skilled at the unorthodox
// are infinite as heaven and earth,
// inexhaustible as the great rivers.
// When they come to an end,
// they begin again,
// like the days and months;
// they die and are reborn,
// like the four seasons."
// 
// - Sun Tsu,
// "The Art of War"

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Reflection;
using System.IO;


using HtmlRenderer.Diagnostics;
using HtmlRenderer.WebDom;

using Timer = System.Threading.Timer;

namespace HtmlRenderer.Demo
{
    public partial class DemoForm : Form
    {
        #region Fields and Consts

        /// <summary>
        /// Cache for resource images
        /// </summary>
        private readonly Dictionary<string, Image> _imageCache = new Dictionary<string, Image>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// the private font used for the demo
        /// </summary>
        private readonly PrivateFontCollection _privateFont = new PrivateFontCollection();

        /// <summary>
        /// the html samples used for performance testing
        /// </summary>
        private readonly List<string> _perfTestSamples = new List<string>();

        /// <summary>
        /// the html samples to show in the demo
        /// </summary>
        private readonly Dictionary<string, string> _samples = new Dictionary<string, string>();

        /// <summary>
        /// timer to update the rendered html when html in editor changes with delay
        /// </summary>
        private readonly Timer _updateHtmlTimer;

        /// <summary>
        /// used ignore html editor updates when updating seperatly
        /// </summary>
        private bool _updateLock;

        #endregion


        /// <summary>
        /// Init.
        /// </summary>
        public DemoForm()
        {
            InitializeComponent();



            _htmlPanel.RenderError += OnRenderError;
            _htmlPanel.LinkClicked += OnLinkClicked;
            _htmlPanel.StylesheetLoad += OnStylesheetLoad;
            _htmlPanel.ImageLoad += OnImageLoad;
            //_htmlToolTip.ImageLoad += OnImageLoad; 
            //_htmlToolTip.SetToolTip(_htmlPanel, Resources.Tooltip);

            _htmlEditor.Font = new Font(FontFamily.GenericMonospace, 10);

            StartPosition = FormStartPosition.CenterScreen;
            var size = Screen.GetWorkingArea(Point.Empty);
            Size = new Size((int)(size.Width * 0.7), (int)(size.Height * 0.8));


            _updateHtmlTimer = new Timer(OnUpdateHtmlTimerTick);
            this.Text += " : " + Path.GetDirectoryName(Application.ExecutablePath);

            this._samplesTreeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(_samplesTreeView_NodeMouseClick);

        }

        void _samplesTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var filename = e.Node.Tag as string;
            if (filename == null ||
                !File.Exists(filename))
            {
                return;
            }
            //------------------------------------
            //load file
            _updateLock = true;
            Application.UseWaitCursor = true;
            _htmlPanel.Text = File.ReadAllText(filename);
            Application.UseWaitCursor = false;
            _updateLock = false;
            UpdateWebBrowserHtml();
        }

        public void PrepareSamples()
        {
            LoadCustomFonts();
            LoadSamples();

        }
        public void LoadDemo(DemoBase demoBase)
        {
            demoBase.StartDemo(this._htmlPanel);
        }
        #region Private methods

        /// <summary>
        /// Loads the tree of document samples
        /// </summary>
        private void LoadSamples()
        {


            //find sample folder 
            string execFromFolder = Path.GetDirectoryName(Application.ExecutablePath);

            //only from debug ?
            if (!execFromFolder.EndsWith("\\Source\\Demo\\bin\\Debug"))
            {
                return;
            }

            int index = execFromFolder.LastIndexOf("\\Source\\Demo\\bin\\Debug");
            string rootSampleFolder = execFromFolder.Substring(0, index) + "\\Source\\Demo\\Samples";

            var root = new TreeNode("HTML Renderer");
            _samplesTreeView.Nodes.Add(root);

            string[] sampleDirs = Directory.GetDirectories(rootSampleFolder);

            //only 1 file level (not recursive)
            foreach (string dirName in sampleDirs)
            {
                var dirNode = new TreeNode(Path.GetFileName(dirName));
                root.Nodes.Add(dirNode);
                string[] fileNames = Directory.GetFiles(dirName, "*.htm");
                foreach (string fname in fileNames)
                {
                    var onlyFileName = Path.GetFileName(fname);
                    if (!onlyFileName.StartsWith("x"))
                    {
                        //for our convention: 
                        //file start with x will not show here  
                        //(it used for comment out/backup file)
                        var fileNameNode = new TreeNode(Path.GetFileName(fname));
                        dirNode.Nodes.Add(fileNameNode);
                        fileNameNode.Tag = fname;
                    }
                }
            }
            root.ExpandAll();
            //-------------------------
        }
        public int StartAtSampleIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Load custom fonts to be used by renderer htmls
        /// </summary>
        private void LoadCustomFonts()
        {
            // load custom font font into private fonts collection
            var file = Path.GetTempFileName();
            File.WriteAllBytes(file, HtmlRenderer.Demo.Resource.CustomFont);
            _privateFont.AddFontFile(file);

            // add the fonts to renderer
            foreach (var fontFamily in _privateFont.Families)
            {
                HtmlRender.AddFontFamily(fontFamily);
            }
        }

        ///// <summary>
        ///// On tree view node click load the html to the html panel and html editor.
        ///// </summary>
        //private void OnSamplesTreeViewAfterSelect(object sender, TreeViewEventArgs e)
        //{

        //    //if (!string.IsNullOrEmpty(name))
        //    //{
        //    //    _updateLock = true;

        //    //    string html = _samples[name];

        //    //    if (!name.Contains("PerfSamples"))
        //    //        SyntaxHilight.AddColoredText(html, _htmlEditor);
        //    //    else
        //    //        _htmlEditor.Text = html;

        //    //    Application.UseWaitCursor = true;

        //    //    _htmlPanel.AvoidImagesLateLoading = !name.Contains("Many images");
        //    //    _htmlPanel.Text = html;

        //    //    //try
        //    //    //{
        //    //    //    _htmlPanel.AvoidImagesLateLoading = !name.Contains("Many images");

        //    //    //    _htmlPanel.Text = html;
        //    //    //}
        //    //    //catch (Exception ex)
        //    //    //{
        //    //    //    MessageBox.Show(ex.ToString(), "Failed to render HTML");
        //    //    //}

        //    //    Application.UseWaitCursor = false;
        //    //    _updateLock = false;

        //    //    UpdateWebBrowserHtml();
        //    //}
        //}

        /// <summary>
        /// On text change in the html editor update 
        /// </summary>
        private void OnHtmlEditorTextChanged(object sender, EventArgs e)
        {
            if (!_updateLock)
            {
                _updateHtmlTimer.Change(1000, int.MaxValue);
            }
        }

        /// <summary>
        /// Update the html renderer with text from html editor.
        /// </summary>
        private void OnUpdateHtmlTimerTick(object state)
        {
            BeginInvoke(new MethodInvoker(() =>
            {
                _updateLock = true;

                try
                {
                    _htmlPanel.Text = _htmlEditor.Text;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Failed to render HTML");
                }

                //SyntaxHilight.AddColoredText(_htmlEditor.Text, _htmlEditor);

                UpdateWebBrowserHtml();

                _updateLock = false;
            }));
        }

        /// <summary>
        /// Open the current html is external process - the default user browser.
        /// </summary>
        private void OnOpenExternalViewButtonClick(object sender, EventArgs e)
        {
            var html = _showGeneratedHtmlCB.Checked ? _htmlPanel.GetHtml() : _htmlEditor.Text;
            var tmpFile = Path.ChangeExtension(Path.GetTempFileName(), ".htm");
            File.WriteAllText(tmpFile, html);
            Process.Start(tmpFile);
        }

        /// <summary>
        /// Show\Hide the web browser viwer.
        /// </summary>
        private void OnToggleWebBrowserButton_Click(object sender, EventArgs e)
        {
            //_webBrowser.Visible = !_webBrowser.Visible;
            //_splitter.Visible = _webBrowser.Visible;
            //_toggleWebBrowserButton.Text = _webBrowser.Visible ? "Hide IE View" : "Show IE View";

            //if (_webBrowser.Visible)
            //{
            //    _webBrowser.Width = _splitContainer2.Panel2.Width / 2;
            //    UpdateWebBrowserHtml();
            //}
        }

        /// <summary>
        /// Update the html shown in the web browser
        /// </summary>
        private void UpdateWebBrowserHtml()
        {
            //if (_webBrowser.Visible)
            //{
            //    _webBrowser.DocumentText = _showGeneratedHtmlCB.Checked ? _htmlPanel.GetHtml() : GetFixedHtml();
            //}
        }

        /// <summary>
        /// Fix the raw html by replacing bridge object properties calls with path to file with the data returned from the property.
        /// </summary>
        /// <returns>fixed html</returns>
        private string GetFixedHtml()
        {
            var html = _htmlEditor.Text;
            html = Regex.Replace(html, @"src=\""(\w.*?)\""", match =>
                {
                    var img = TryLoadResourceImage(match.Groups[1].Value);
                    if (img != null)
                    {
                        var tmpFile = Path.GetTempFileName();
                        img.Save(tmpFile, ImageFormat.Jpeg);
                        return string.Format("src=\"{0}\"", tmpFile);
                    }
                    return match.Value;
                }, RegexOptions.IgnoreCase);

            html = Regex.Replace(html, @"href=\""(\w.*?)\""", match =>
            {
                var stylesheet = GetStylesheet(match.Groups[1].Value);
                if (stylesheet != null)
                {
                    var tmpFile = Path.GetTempFileName();
                    File.WriteAllText(tmpFile, stylesheet);
                    return string.Format("href=\"{0}\"", tmpFile);
                }
                return match.Value;
            }, RegexOptions.IgnoreCase);

            return html;
        }

        /// <summary>
        /// On change if to show generated html or regular update the web browser to show the new choice.
        /// </summary>
        private void OnShowGeneratedHtmlCheckedChanged(object sender, EventArgs e)
        {
            UpdateWebBrowserHtml();
        }

        /// <summary>
        /// Reload the html shown in the html editor by running coloring again.
        /// </summary>
        private void OnReloadColorsLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //SyntaxHilight.AddColoredText(_htmlEditor.Text, _htmlEditor);
        }

        /// <summary>
        /// Handle stylesheet resolve.
        /// </summary>
        private static void OnStylesheetLoad(object sender, HtmlRenderer.ContentManagers.StylesheetLoadEventArgs e)
        {
            var stylesheet = GetStylesheet(e.Src);
            if (stylesheet != null)
                e.SetStyleSheet = stylesheet;
        }

        /// <summary>
        /// Get stylesheet by given key.
        /// </summary>
        private static string GetStylesheet(string src)
        {
            if (src == "StyleSheet")
            {
                return @"h1, h2, h3 { color: navy; font-weight:normal; }
                    h1 { margin-bottom: .47em }
                    h2 { margin-bottom: .3em }
                    h3 { margin-bottom: .4em }
                    ul { margin-top: .5em }
                    ul li {margin: .25em}
                    body { font:10pt Tahoma }
		            pre  { border:solid 1px gray; background-color:#eee; padding:1em }
                    a:link { text-decoration: none; }
                    a:hover { text-decoration: underline; }
                    .gray    { color:gray; }
                    .example { background-color:#efefef; corner-radius:5px; padding:0.5em; }
                    .whitehole { background-color:white; corner-radius:10px; padding:15px; }
                    .caption { font-size: 1.1em }
                    .comment { color: green; margin-bottom: 5px; margin-left: 3px; }
                    .comment2 { color: green; }";
            }
            return null;
        }

        /// <summary>
        /// On image load in renderer set the image by event async.
        /// </summary>
        private void OnImageLoad(object sender, HtmlRenderer.ContentManagers.ImageRequestEventArgs e)
        {



            var img = TryLoadResourceImage(e.ImagSource);
            e.SetResultImage(img);

            //if (!e.Handled && htmlTag != null)
            //{
            //    string attrValue = null;
            //    if ((attrValue = htmlTag.TryGetAttribute("byevent", null)) != null)
            //    {
            //        int delay;
            //        if (int.TryParse(attrValue, out delay))
            //        {
            //            e.Handled = true;
            //            ThreadPool.QueueUserWorkItem(state =>
            //            {
            //                Thread.Sleep(delay);
            //                e.Callback("https://fbcdn-sphotos-a-a.akamaihd.net/hphotos-ak-snc7/c0.44.403.403/p403x403/318890_10151195988833836_1081776452_n.jpg");
            //            });
            //            return;
            //        }
            //        else
            //        {
            //            e.Callback("http://sphotos-a.xx.fbcdn.net/hphotos-ash4/c22.0.403.403/p403x403/263440_10152243591765596_773620816_n.jpg");
            //            return;
            //        }
            //    }
            //    else if ((attrValue = htmlTag.TryGetAttribute("byevent", null)) != null)
            //    {
            //        var split = attrValue.Split(',');
            //        var rect = new Rectangle(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]), int.Parse(split[3]));
            //        e.Callback(img ?? Resources.html32, rect);
            //        return;
            //    }
            //}

            //if (img != null)
            //{
            //    e.Callback(img);
            //}
        }

        /// <summary>
        /// Get image by resource key.
        /// </summary>
        private Image TryLoadResourceImage(string src)
        {
            Image image;
            if (!_imageCache.TryGetValue(src, out image))
            {
                switch (src.ToLower())
                {
                    case "htmlicon":
                        image = HtmlRenderer.Demo.Resource.html32;
                        break;
                    case "staricon":
                        image = HtmlRenderer.Demo.Resource.favorites32;
                        break;
                    case "fonticon":
                        image = HtmlRenderer.Demo.Resource.font32;
                        break;
                    case "commenticon":
                        image = HtmlRenderer.Demo.Resource.comment16;
                        break;
                    case "imageicon":
                        image = HtmlRenderer.Demo.Resource.image32;
                        break;
                    case "methodicon":
                        image = HtmlRenderer.Demo.Resource.method16;
                        break;
                    case "propertyicon":
                        image = HtmlRenderer.Demo.Resource.property16;
                        break;
                    case "eventicon":
                        image = HtmlRenderer.Demo.Resource.Event16;
                        break;
                }

                if (image != null)
                    _imageCache[src] = image;
            }
            return image;
        }

        /// <summary>
        /// Show error raised from html renderer.
        /// </summary>
        private static void OnRenderError(object sender, HtmlRenderErrorEventArgs e)
        {
            MessageBox.Show(e.Message + (e.Exception != null ? "\r\n" + e.Exception : null), "Error in Html Renderer", MessageBoxButtons.OK);
        }

        /// <summary>
        /// On specific link click handle it here.
        /// </summary>
        private static void OnLinkClicked(object sender, HtmlLinkClickedEventArgs e)
        {
            if (e.Link == "SayHello")
            {
                MessageBox.Show("Hello you!");
                e.Handled = true;
            }
            else if (e.Link == "ShowSampleForm")
            {
                //using (var f = new SampleForm())
                //{
                //    f.ShowDialog();
                //    e.Handled = true;
                //}
            }
        }

        /// <summary>
        /// Execute performance test by setting all sample htmls in a loop.
        /// </summary>
        private void OnRunTestButtonClick(object sender, EventArgs e)
        {
            _updateLock = true;
            _runTestButton.Text = "Running..";
            _runTestButton.Enabled = false;
            Application.DoEvents();

            GC.Collect();
#if NET_40
            AppDomain.MonitoringIsEnabled = true;
            var startMemory = AppDomain.CurrentDomain.MonitoringTotalAllocatedMemorySize;
#endif


            const int iterations = 40;

            //for (int i = 0; i < iterations; i++)
            //{                
            //    foreach (var html in _perfTestSamples)
            //    {
            //        _htmlPanel.Text = html;
            //        Application.DoEvents(); // so paint will be called
            //    }
            //}
            List<int> selectedSamples = new List<int>();
            var allTestCount = _perfTestSamples.Count;
            for (int i = 0; i < allTestCount; ++i)
            {
                selectedSamples.Add(2);
            }



            //HtmlRenderer.dbugCounter.dbugStartRecord = true;
            //HtmlRenderer.dbugCounter.dbugDrawStringCount = 0;
            long ms_total = 0;
            System.Diagnostics.Stopwatch sw = new Stopwatch();

#if DEBUG
            for (int i = 0; i < iterations; i++)
            {
                foreach (var sampleNum in selectedSamples)
                {
                    ms_total += dbugCounter.Snap(sw, () =>
                    {
                        //HtmlRenderer.dbugCounter.dbugStartRecord = true;
                        //HtmlRenderer.dbugCounter.dbugDrawStringCount = 0;
                        _htmlPanel.Text = _perfTestSamples[sampleNum];
                        Application.DoEvents(); // so paint will be called 

                    });
                }

            }
#endif
            long endMemory = 0;
            float totalMem = 0;
#if NET_40
            endMemory = AppDomain.CurrentDomain.MonitoringTotalAllocatedMemorySize;
            totalMem = (endMemory - startMemory)/1024f;
#endif
            float htmlSize = 0;
            foreach (var sample in _perfTestSamples)
            {
                htmlSize += sample.Length * 2;
            }
            htmlSize = htmlSize / 1024f;


            var msg = string.Format("{0} HTMLs ({1:N0} KB)\r\n{2} Iterations", _perfTestSamples.Count, htmlSize, iterations);
            msg += "\r\n\r\n";
            msg += string.Format("CPU:\r\nTotal: {0} msec\r\nIterationAvg: {1:N2} msec\r\nSingleAvg: {2:N2} msec",
                                   ms_total, ms_total / iterations, ms_total / (double)iterations / _perfTestSamples.Count);
            msg += "\r\n\r\n";
            msg += string.Format("Memory:\r\nTotal: {0:N0} KB\r\nIterationAvg: {1:N0} KB\r\nSingleAvg: {2:N0} KB\r\nOverhead: {3:N0}%",
                                 totalMem, totalMem / iterations, totalMem / iterations / _perfTestSamples.Count, 100 * (totalMem / iterations) / htmlSize);

            Clipboard.SetDataObject(msg);
            MessageBox.Show(msg, "Test run results");

            _updateLock = false;
            _runTestButton.Text = "Run Performance Test";
            _runTestButton.Enabled = true;
        }

        #endregion
    }
}
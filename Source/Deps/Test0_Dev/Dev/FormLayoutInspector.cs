//Apache2, 2014-2017, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using System.IO;
using LayoutFarm.UI;
using LayoutFarm.RenderBoxes;

namespace LayoutFarm.Dev
{
    public partial class FormLayoutInspector : Form
    {

#if DEBUG
        EventHandler rootDrawMsgEventHandler;
        EventHandler rootHitMsgEventHandler;
        LayoutFarm.UI.UISurfaceViewportControl vwport;
        bool pauseRecord;
#endif
        public FormLayoutInspector()
        {
            InitializeComponent();
#if DEBUG
            rootDrawMsgEventHandler = new EventHandler(artUISurfaceViewport1_dbug_VisualRootDebugMsg);
            rootHitMsgEventHandler = new EventHandler(artUISurfaceViewport1_dbug_VisualRootHitChainMsg);
            this.toolStripButton1.Click += new EventHandler(this.toolStripButton1_Click);
#endif

            listBox2.MouseDown += new MouseEventHandler(listBox2_MouseDown);
            listBox3.MouseDown += new MouseEventHandler(listBox3_MouseDown);
        }

        void listBox3_MouseDown(object sender, MouseEventArgs e)
        {

#if DEBUG
            dbugLayoutMsg msg = listBox3.SelectedItem as dbugLayoutMsg;
            if (msg == null)
            {
                return;
            }
            switch (msg.msgOwnerKind)
            {
                case dbugLayoutMsgOwnerKind.Layer:
                    {
                        RenderElementLayer layer = (RenderElementLayer)msg.owner;
                    } break;
                case dbugLayoutMsgOwnerKind.Line:
                    {

                    } break;
                case dbugLayoutMsgOwnerKind.VisualElement:
                    {
                        RenderElement ve = (RenderElement)msg.owner;
                        dbugHelper01.dbugVE_HighlightMe = ve;
                        lastestSelectVE = ve;

                        vwport.PaintMe();

                    } break;

            }
#endif
        }

        void listBox2_MouseDown(object sender, MouseEventArgs e)
        {
#if DEBUG
            dbugLayoutMsg msg = listBox2.SelectedItem as dbugLayoutMsg;
            if (msg == null)
            {
                return;
            }
            switch (msg.msgOwnerKind)
            {
                case dbugLayoutMsgOwnerKind.Layer:
                    {
                        RenderElementLayer layer =
                           (RenderElementLayer)msg.owner;
                    } break;
                case dbugLayoutMsgOwnerKind.Line:
                    {

                    } break;
                case dbugLayoutMsgOwnerKind.VisualElement:
                    {
                        RenderElement ve = (RenderElement)msg.owner;
                        dbugHelper01.dbugVE_HighlightMe = ve;
                        lastestSelectVE = ve;

                        vwport.PaintMe();

                    } break;

            }
#endif
        }

#if DEBUG
        RenderElement lastestSelectVE;
        List<dbugLayoutMsg> lastestMessages;

#endif

        void CollectList1Item(StringBuilder stBuilder)
        {
            foreach (object obj in listBox1.Items)
            {
                stBuilder.AppendLine(obj.ToString());
            }
        }
#if DEBUG
        void LoadList2NewContent(List<dbugLayoutMsg> msgs)
        {
            listBox2.Items.Clear();
            foreach (dbugLayoutMsg s in msgs)
            {
                listBox2.Items.Add(s);
            }
        }
        void LoadList3NewContent(List<dbugLayoutMsg> msgs)
        {
            listBox3.Items.Clear();
            foreach (dbugLayoutMsg s in msgs)
            {
                listBox3.Items.Add(s);
            }
        }
        void LoadList1NewContent(List<dbugLayoutMsg> msgs)
        {
            listBox1.Items.Clear();
            foreach (dbugLayoutMsg s in msgs)
            {
                listBox1.Items.Add(s);
            }
            int dumpWhen = Int32.Parse(toolStripTextBox1.Text);

            if (listBox1.Items.Count > dumpWhen)
            {

                int j = listBox1.Items.Count;

                FileStream fs = new FileStream("c:\\WImageTest\\invalidate\\lim_" + Guid.NewGuid().ToString() + ".txt", FileMode.Create);
                StreamWriter strmWriter = new StreamWriter(fs);
                strmWriter.AutoFlush = true; 

                for (int i = 0; i < j; ++i)
                {
                    strmWriter.WriteLine(listBox1.Items[i].ToString());
                }

                strmWriter.Close();
                fs.Close();
                fs.Dispose();
            }
        }
#endif
        public void Connect(LayoutFarm.UI.UISurfaceViewportControl vwport)
        {
#if DEBUG
            this.vwport = vwport;
            IdbugOutputWindow outputWin = vwport.IdebugOutputWin;
            outputWin.dbug_VisualRootDrawMsg += rootDrawMsgEventHandler;
            outputWin.dbug_VisualRootHitChainMsg += artUISurfaceViewport1_dbug_VisualRootHitChainMsg;
            outputWin.dbug_EnableAllDebugInfo();
#endif
        }
#if DEBUG
        protected override void OnClosing(CancelEventArgs e)
        {
            IdbugOutputWindow outputWin = vwport.IdebugOutputWin;
            outputWin.dbug_VisualRootDrawMsg -= rootDrawMsgEventHandler;
            outputWin.dbug_VisualRootHitChainMsg -= rootHitMsgEventHandler;
            outputWin.dbug_DisableAllDebugInfo();
            base.OnClosing(e);
        }

        void artUISurfaceViewport1_dbug_VisualRootHitChainMsg(object sender, EventArgs e)
        {
            LoadList2NewContent(this.vwport.IdebugOutputWin.dbug_rootDocHitChainMsgs);
        }
        void artUISurfaceViewport1_dbug_VisualRootDebugMsg(object sender, EventArgs e)
        {
            LoadList1NewContent(this.vwport.IdebugOutputWin.dbug_rootDocDebugMsgs);
        }
        public void TogglePauseMode()
        {
            if (!pauseRecord)
            {
                pauseRecord = true; vwport.IdebugOutputWin.dbug_VisualRootDrawMsg -= rootDrawMsgEventHandler;
                this.Text = "Pause - LayoutFarm LayoutInspector 2016";

                StringBuilder stBuilder = new StringBuilder();
                CollectList1Item(stBuilder);
                System.Windows.Forms.Clipboard.SetText(stBuilder.ToString());

            }
            else
            {
                pauseRecord = false;
                vwport.IdebugOutputWin.dbug_VisualRootDrawMsg += rootDrawMsgEventHandler;
                this.Text = "LayoutFarm LayoutInspector 2016";
            }
        }
#endif
        private void toolStripButton1_Click(object sender, EventArgs e)
        {

#if DEBUG
            int j = listBox1.Items.Count;
            StringBuilder stBuilder = new StringBuilder();
            for (int i = 0; i < j; ++i)
            {
                stBuilder.AppendLine(listBox1.Items[i].ToString());
            }

            System.Windows.Forms.Clipboard.SetText(stBuilder.ToString());
#endif
        }

        private void tlstrpDumpSelectedVisualProps_Click(object sender, EventArgs e)
        {
#if DEBUG
            if (lastestSelectVE != null)
            {
                dbugLayoutMsgWriter writer = new dbugLayoutMsgWriter();
                lastestSelectVE.dbug_DumpVisualProps(writer);
                lastestMessages = writer.allMessages;
                listBox3.Items.Clear();
                int j = lastestMessages.Count;
                for (int i = 0; i < j; ++i)
                {
                    listBox3.Items.Add(lastestMessages[i]);
                }
            }
#endif
        }

        private void tlstrpSaveSelectedVisualProps_Click(object sender, EventArgs e)
        {
#if DEBUG
            int j = lastestMessages.Count;
            if (j > 0)
            {
                FileStream fs = new FileStream("c:\\WImageTest\\layout_trace\\" + Guid.NewGuid().ToString() + ".txt", FileMode.Create);
                StreamWriter strmWriter = new StreamWriter(fs);
                for (int i = 0; i < j; ++i)
                {
                    strmWriter.WriteLine(lastestMessages[i].ToString()); 
                } 
                strmWriter.Close();
                fs.Close();
                fs.Dispose();
            }
#endif
        }

        private void tlstrpReArrange_Click(object sender, EventArgs e)
        {

#if DEBUG
            vwport.IdebugOutputWin.dbug_ReArrangeWithBreakOnSelectedNode();
#endif

        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
#if DEBUG
           vwport.PaintMeFullMode();
#endif


        }



    }
}

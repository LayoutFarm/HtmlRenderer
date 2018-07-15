//Apache2, 2014-present, WinterDev

using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using LayoutFarm.UI;
namespace LayoutFarm.DzBoardSample
{
    interface IDesignBox
    {
        UIElement CloneDesignBox();
        void Describe(DzBoxSerializer writer);
    }
    delegate UIElement UICloneHandler(DesignBox holderBox);
    abstract class DesignBox : LayoutFarm.CustomWidgets.AbstractBox, IDesignBox
    {
        UICloneHandler cloneDelegate;
        UISerializeHandler serializeDelegate;
        public DesignBox(int w, int h)
            : base(w, h)
        {
        }
        public UIElement CloneDesignBox()
        {
            if (cloneDelegate != null)
            {
                return cloneDelegate(this);
            }
            return null;
        }
        public void SetCloneDelegate(UICloneHandler cloneDelegate)
        {
            this.cloneDelegate = cloneDelegate;
        }
        public void SetSerializeDelegate(UISerializeHandler serializeDelegate)
        {
            this.serializeDelegate = serializeDelegate;
        }
        public bool HasCloneDelegate
        {
            get { return this.cloneDelegate != null; }
        }
        public virtual void Describe(DzBoxSerializer writer)
        {
            if (serializeDelegate != null)
            {
                serializeDelegate(writer, this);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
    class UIHolderBox : DesignBox
    {
        LayoutFarm.UI.AbstractRectUI targetUI;
        int borderWidth = 5;
        public UIHolderBox(int w, int h)
            : base(w, h)
        {
        }
        public int HolderBorder
        {
            get { return this.borderWidth; }
            set { this.borderWidth = value; }
        }
        public LayoutFarm.UI.AbstractRectUI TargetBox
        {
            get { return this.targetUI; }
            set
            {
                this.targetUI = value;
                if (value != null)
                {
                    this.targetUI.SetLocation(borderWidth, borderWidth);
                    this.targetUI.SetSize(this.Width - (borderWidth * 2), this.Height - (borderWidth * 2));
                }
            }
        }
        public override void SetSize(int width, int height)
        {
            base.SetSize(width, height);
            if (this.targetUI != null)
            {
                this.TargetBox = this.targetUI;
            }
        }

        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "holderbox");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }

    class DesignRectBox : LayoutFarm.CustomWidgets.AbstractBox, IDesignBox
    {
        public DesignRectBox(int w, int h)
            : base(w, h)
        {
        }
        public UIElement CloneDesignBox()
        {
            var newRectBox = new DesignRectBox(this.Width, this.Height);
            newRectBox.BackColor = this.BackColor;
            return newRectBox;
        }
        public void Describe(DzBoxSerializer writer)
        {
            DzBoxSerializerHelper.WriteElement(writer, this, "rectbox");
        }
        public override void Walk(UIVisitor visitor)
        {
            visitor.BeginElement(this, "rectbox");
            this.Describe(visitor);
            visitor.EndElement();
        }
    }


    class DesignBoardModule : CompoModule
    {
        UISelectionBox selectionBox;
        bool selectionBoxIsShown;
        RootGraphic rootgfx;
        List<LayoutFarm.CustomWidgets.AbstractBox> userBoxes = new List<CustomWidgets.AbstractBox>();
        Queue<UIControllerBox> userControllerPool = new Queue<UIControllerBox>();
        List<UIControllerBox> workingControllerBoxes = new List<UIControllerBox>();
        SelectionSet selectionSet = new SelectionSet();
        DesingBoardBackground dzBoardBg;
        List<IDesignBox> toCopyList = new List<IDesignBox>();
        DzCommandCollection dzCommandHistory;
        public DesignBoardModule()
        {
            this.dzCommandHistory = new DzCommandCollection(this);
            this.EnableUndoHistoryRecording = true;
        }
        public bool EnableUndoHistoryRecording { get; set; }
        protected override void OnStartModule()
        {
            this.rootgfx = viewport.RootGfx;
            //--------------------------------


            dzBoardBg = new DesingBoardBackground(800, 600);
            dzBoardBg.DesignBoardModule = this;
            dzBoardBg.BackColor = Color.White;
            dzBoardBg.SetLocation(0, 0);
            SetupBackgroundProperties(dzBoardBg);
            viewport.AddChild(dzBoardBg);
            //--------------------------------

            selectionBox = new UISelectionBox(1, 1);
            selectionBox.Visible = false;
            selectionBox.BackColor = Color.FromArgb(80, Color.Green);
            viewport.AddChild(selectionBox);
            SetupSelectionBoxProperties(selectionBox);
        }
        public void AddNewBox(int x, int y, int w, int h)
        {
            //add new user box
            var box1 = new DesignRectBox(w, h);
            box1.BackColor = Color.Red;
            box1.SetLocation(x, y);
            SetupActiveBoxProperties(box1);
            viewport.AddChild(box1);
            userBoxes.Add(box1);
        }

        public UIHolderBox AddExternalControl(int x, int y, AbstractRectUI uibox)
        {
            //create holder ui
            var box1 = new UIHolderBox(uibox.Width + 10, uibox.Height + 10);
            uibox.SetLocation(5, 5);
            box1.AddChild(uibox);
            box1.SetLocation(x, y);
            box1.TargetBox = uibox;
            SetupActiveBoxProperties(box1);
            viewport.AddChild(box1);
            userBoxes.Add(box1);
            return box1;
        }
        public UIHolderBox WrapWithHolderBox(int x, int y, AbstractRectUI uibox)
        {
            //create holder ui
            var box1 = new UIHolderBox(uibox.Width + 10, uibox.Height + 10);
            uibox.SetLocation(5, 5);
            box1.AddChild(uibox);
            box1.SetLocation(x, y);
            box1.TargetBox = uibox;
            SetupActiveBoxProperties(box1);
            userBoxes.Add(box1);
            return box1;
        }
        public UIHolderBox AddNewImageBox(int x, int y, int w, int h, ImageBinder imgBinder)
        {
            //add new user box
            var box1 = new UIHolderBox(w, h);
            box1.BackColor = Color.White;
            box1.SetLocation(x, y);
            var imgBox = new LayoutFarm.CustomWidgets.ImageBox(imgBinder.Image.Width, imgBinder.Image.Height);
            imgBox.ImageBinder = imgBinder;
            box1.AddChild(imgBox);
            imgBox.SetLocation(10, 10);
            box1.TargetBox = imgBox;
            box1.SetSize(imgBinder.ImageWidth + 20, imgBinder.ImageHeight + 20);
            SetupActiveBoxProperties(box1);
            viewport.AddChild(box1);
            userBoxes.Add(box1);
            return box1;
        }
        public void AddNewRect(int x, int y, int w, int h)
        {
            //add new user box
            var box1 = new GraphicShapeBox(w, h);
            box1.BackColor = Color.Red;
            box1.SetLocation(x, y);
            SetupActiveBoxProperties(box1);
            viewport.AddChild(box1);
            userBoxes.Add(box1);
        }

        static UIElement CloneTextBox(DesignBox dzBox)
        {
            UIHolderBox holderBox = dzBox as UIHolderBox;
            var originalTextBox = holderBox.TargetBox as LayoutFarm.CustomWidgets.TextBox;
            var newClone = new UIHolderBox(holderBox.Width, holderBox.Height);
            newClone.BackColor = Color.White;
            var textBox = new LayoutFarm.CustomWidgets.TextBox(newClone.Width - 10, newClone.Height - 10, true);
            textBox.BackgroundColor = Color.White;
            textBox.Text = originalTextBox.Text;
            //clone content of text box 

            newClone.SetCloneDelegate(CloneTextBox);
            newClone.SetSerializeDelegate(SerializeDzTextBox);
            newClone.AddChild(textBox);
            newClone.TargetBox = textBox;
            return newClone;
        }
        static void SerializeDzTextBox(DzBoxSerializer writer, UIElement ui)
        {
            ui.Walk(writer);
        }
        public void Save()
        {
            DzBoxSerializer dzBox = new DzBoxSerializer("workspace");
            int j = userBoxes.Count;
            for (int i = 0; i < j; ++i)
            {
                var dzElem = userBoxes[i] as IDesignBox;
                dzElem.Describe(dzBox);
            }
            dzBox.WriteToFile("d:\\WImageTest\\workspace01.xml");
        }
        public void Load()
        {
            //read 
            DzBoxDeserializer dzFileReader = new DzBoxDeserializer();
            dzFileReader.LoadFile("d:\\WImageTest\\workspace01.xml");
            dzFileReader.DrawContents(this);
        }


        public BoxSelectionMode BoxSelectionMode
        {
            get;
            set;
        }
        UIControllerBox GetFreeControllerBox()
        {
            //Console.WriteLine("<< ctrl " + userControllerPool.Count);
            UIControllerBox controlBox = null;
            if (userControllerPool.Count > 0)
            {
                controlBox = userControllerPool.Dequeue();
                //-------------------------------------------
                viewport.AddChild(controlBox);
                //register to working controller box
                workingControllerBoxes.Add(controlBox);
            }
            else
            {
                //create new one 
                //controller box 1 (red corners)
                controlBox = new UIControllerBox(40, 40);
                controlBox.DesignBoardModule = this;
                //var gridBox = new LayoutFarm.CustomWidgets.GridBox(30, 30);
                //gridBox.SetLocation(5, 5);
                //gridBox.BuildGrid(3, 3, CellSizeStyle.UniformCell);
                //controlBox.ContentBox = gridBox;

                Color c = KnownColors.FromKnownColor(KnownColor.Yellow);
                controlBox.BackColor = new Color(100, c.R, c.G, c.B);
                controlBox.SetLocation(200, 200);
                //controllerBox1.dbugTag = 3;
                controlBox.Visible = false;
                viewport.AddChild(controlBox);
                //-------------------------------------------
                //register to working controller box
                workingControllerBoxes.Add(controlBox);
            }
            return controlBox;
        }


        internal void PasteClipboardData()
        {
            //paste data from clipboard to this 
            //copy this box to a clipboard***
            //copy this content to clipboard 

            int j = toCopyList.Count;
            if (j < 1)
            {
                return;
            }
            for (int i = 0; i < j; ++i)
            {
                var toCopyUI = toCopyList[i];
                var newClone = toCopyUI.CloneDesignBox();
                //then add to 
                if (newClone == null)
                {
                    continue;
                }
                var newCloneAsRect = newClone as DesignRectBox;
                if (newCloneAsRect != null)
                {
                    SetupActiveBoxProperties(newCloneAsRect);
                    viewport.AddChild(newCloneAsRect);
                    userBoxes.Add(newCloneAsRect);
                    return;
                }
                var holderBox = newClone as UIHolderBox;
                if (holderBox != null)
                {
                    SetupActiveBoxProperties(holderBox);
                    viewport.AddChild(holderBox);
                    userBoxes.Add(holderBox);
                }

                var gfxBox = newClone as GraphicShapeBox;
                if (gfxBox != null)
                {
                    SetupActiveBoxProperties(gfxBox);
                    viewport.AddChild(gfxBox);
                    userBoxes.Add(gfxBox);
                }
            }
        }
        internal void CopyToClipBoard(IDesignBox ui)
        {
            //clear prev content
            if (ui == null) return;
            this.toCopyList.Clear();
            if (ui != null)
            {
                this.toCopyList.Add(ui);
            }
        }
        internal void RemoveTargetBox(IDesignBox ui)
        {
            var uiDesignBox = ui as LayoutFarm.CustomWidgets.AbstractBox;
            if (uiDesignBox != null)
            {
                var re = uiDesignBox.CurrentPrimaryRenderElement;
                if (re != null)
                {
                    var parent = re.ParentRenderElement;
                    parent.RemoveChild(re);
                }
                this.userBoxes.Remove(uiDesignBox);
            }
        }
        internal void UndoLastAction()
        {
            this.dzCommandHistory.UndoLastAction();
        }
        internal void ReverseLastUndoAction()
        {
            this.dzCommandHistory.ReverseLastUndoAction();
        }
        void RemoveAllControllerBoxes()
        {
            int j = this.workingControllerBoxes.Count;
            for (int i = j - 1; i >= 0; --i)
            {
                var userControllerBox = this.workingControllerBoxes[i];
                //release control
                userControllerBox.Visible = false;
                userControllerBox.ReleaseTarget();
                userControllerBox.RemoveSelf();
                userControllerPool.Enqueue(userControllerBox);
            }
            workingControllerBoxes.Clear();
            //Console.WriteLine(">> ctrl " + userControllerPool.Count);
        }

        void SetupBackgroundProperties(LayoutFarm.CustomWidgets.AbstractBox backgroundBox)
        {
            //if click on background
            backgroundBox.MouseDown += (s, e) =>
            {
                //remove all controller box
                RemoveAllControllerBoxes();
            };
            //when start drag on bg
            //just show selection box on top most
            backgroundBox.MouseDrag += (s, e) =>
            {
                //move to mouse position 
                if (!selectionBoxIsShown)
                {
                    selectionBox.LandingPoint = new Point(e.X, e.Y);
                    selectionBox.SetLocation(e.X, e.Y);
                    selectionBox.Visible = true;
                    selectionBoxIsShown = true;
                    e.SetMouseCapture(selectionBox);
                }
                else
                {
                    Point pos = selectionBox.LandingPoint;
                    int x = pos.X;
                    int y = pos.Y;
                    int w = e.X - pos.X;
                    int h = e.Y - pos.Y;
                    if (w < 0)
                    {
                        w = -w;
                        x -= w;
                    }
                    if (h < 0)
                    {
                        h = -h;
                        y -= h;
                    }
                    //set width and height
                    selectionBox.SetLocationAndSize(x, y, w, h);
                    e.SetMouseCapture(selectionBox);
                }

                e.StopPropagation();
            };
            backgroundBox.MouseUp += (s, e) =>
            {
                if (selectionBoxIsShown)
                {
                    FindSelectedUserBoxes();
                    selectionBox.Visible = false;
                    selectionBox.SetSize(1, 1);
                    selectionBoxIsShown = false;
                }
                e.StopPropagation();
            };
        }

        void FindSelectedUserBoxes()
        {
            //find users box in selected area
            int j = this.userBoxes.Count;
            var primSelectionBox = selectionBox.GetPrimaryRenderElement(rootgfx);
            var primGlobalPoint = primSelectionBox.GetGlobalLocation();
            var selectedRectArea = new Rectangle(primGlobalPoint, primSelectionBox.Size);
            this.selectionSet.Clear();
            List<CustomWidgets.AbstractBox> selectedList = new List<CustomWidgets.AbstractBox>();
            for (int i = 0; i < j; ++i)
            {
                var box = userBoxes[i];
                var primElement = box.GetPrimaryRenderElement(rootgfx);
                if (!primElement.Visible)
                {
                    continue;
                }
                //get global area 
                Point globalLocation = primElement.GetGlobalLocation();
                var userElementArea = new Rectangle(globalLocation, primElement.Size);
                if (selectedRectArea.Contains(userElementArea))
                {
                    //selected= true;
                    selectedList.Add(box);
                    //------
                    //create user controller box for the selected box 
                    UIControllerBox userControllerBox = GetFreeControllerBox();
                    userControllerBox.TargetBox = box;
                    userControllerBox.SetLocation(box.Left - 5, box.Top - 5);
                    userControllerBox.SetSize(box.Width + 10, box.Height + 10);
                    userControllerBox.Visible = true;
                    userControllerBox.Focus();
                    selectionSet.AddSelection(userControllerBox);
                }
            }
        }
        void SetupActiveBoxProperties(LayoutFarm.CustomWidgets.AbstractBox box)
        {
            //1. mouse down         
            box.MouseDown += (s, e) =>
            {
                //box.BackColor = KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);
                e.MouseCursorStyle = MouseCursorStyle.Pointer;
                //--------------------------------------------
                //request user controller for this box 

                if (this.BoxSelectionMode == DzBoardSample.BoxSelectionMode.Single)
                {
                    RemoveAllControllerBoxes();
                }

                var userControllerBox = GetFreeControllerBox();
                userControllerBox.TargetBox = box;
                //--------
                //get position relative to box
                var loca1 = box.GetGlobalLocation();
                userControllerBox.SetLocation(loca1.X - 5, loca1.Y - 5);
                userControllerBox.SetSize(box.Width + 10, box.Height + 10);
                userControllerBox.Visible = true;
                userControllerBox.Focus();
                e.SetMouseCapture(userControllerBox);
                e.StopPropagation();
                //--------------------------------------------
            };
            //2. mouse up
            box.MouseUp += (s, e) =>
            {
                e.MouseCursorStyle = MouseCursorStyle.Default;
                //box.BackColor = Color.LightGray;
                RemoveAllControllerBoxes();
                e.StopPropagation();
            };
        }

        void SetupSelectionBoxProperties(UISelectionBox selectionBox)
        {
            selectionBox.MouseUp += (s, e) =>
            {
                if (selectionBoxIsShown)
                {
                    FindSelectedUserBoxes();
                    selectionBox.Visible = false;
                    selectionBox.SetSize(1, 1);
                    selectionBoxIsShown = false;
                }
                e.StopPropagation();
            };
            selectionBox.MouseDrag += (s, e) =>
            {
                if (selectionBoxIsShown)
                {
                    Point pos = selectionBox.LandingPoint;
                    int x = pos.X;
                    int y = pos.Y;
                    //temp fix here 
                    //TODO: get global position of selected box

                    int w = (selectionBox.Left + e.X) - pos.X;
                    int h = (selectionBox.Top + e.Y) - pos.Y;
                    if (w < 0)
                    {
                        w = -w;
                        x -= w;
                    }
                    if (h < 0)
                    {
                        h = -h;
                        y -= h;
                    }
                    //set width and height
                    selectionBox.SetLocationAndSize(x, y, w, h);
                }
                e.StopPropagation();
            };
        }

        //-----------------------------------------------------------------------------------------------
        public void HistoryRecordDzElementNewPosition(AbstractRectUI dzBox, Point oldPoint, Point newPoint)
        {
            this.dzCommandHistory.AddAction(new DzSetLocationAction(dzBox, oldPoint, newPoint));
        }
        public void HistoryRecordDzElementNewBounds(AbstractRectUI dzBox, Rectangle oldRect, Rectangle newRect)
        {
            this.dzCommandHistory.AddAction(new DzSetBoundsAction(dzBox, oldRect, newRect));
        }
    }
}
//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using LayoutFarm.Drawing;


namespace LayoutFarm.UI
{
 

    

    public enum AffectedElementSideFlags
    {
        None = 0,
        Left = 1,
        Top = 1 << 1,
        Right = 1 << 2,
        Bottom = 1 << 3
    }




    //public class UISizeChangedEventArgs : UIEventArgs
    //{
    //    AffectedElementSideFlags changeFromSideFlags;
    //    static Stack<UISizeChangedEventArgs> pool = new Stack<UISizeChangedEventArgs>();
    //    private UISizeChangedEventArgs(RenderElement sourceElement, int widthDiff, int heightDiff, AffectedElementSideFlags changeFromSideFlags)
    //    {
    //        this.SourceHitElement = sourceElement;
    //        this.Location = new Point(widthDiff, heightDiff);
    //        this.changeFromSideFlags = changeFromSideFlags;
    //    }
    //    public AffectedElementSideFlags ChangeFromSideFlags
    //    {
    //        get
    //        {
    //            return changeFromSideFlags;
    //        }
    //    }
    //    public static UISizeChangedEventArgs GetFreeOne(RenderElement sourceElement, int widthDiff, int heightDiff, AffectedElementSideFlags changeFromSideFlags)
    //    {
    //        if (pool.Count > 0)
    //        {
    //            UISizeChangedEventArgs e = pool.Pop();

    //            e.Location = new Point(widthDiff, heightDiff);
    //            e.SourceHitElement = sourceElement;
    //            e.changeFromSideFlags = changeFromSideFlags;
    //            return e;
    //        }
    //        else
    //        {
    //            return new UISizeChangedEventArgs(sourceElement, widthDiff, heightDiff, changeFromSideFlags);
    //        }
    //    }
    //    public override void Clear()
    //    {
    //        base.Clear();
    //    }
    //    public static void ReleaseOne(UISizeChangedEventArgs e)
    //    {
    //        e.Clear();
    //        pool.Push(e);
    //    }
    //}

    public class UIInvalidateEventArgs : UIEventArgs
    {
        public Rectangle InvalidArea;
        public UIInvalidateEventArgs()
        {

        }
    }

    public class UICaretEventArgs : UIEventArgs
    {
        public bool Visible = false;
        public override void Clear()
        {
            Visible = false;
            base.Clear();
        }
    }
    public class UICursorEventArgs : UIEventArgs
    {

    }
    public class UIPopupEventArgs : UIEventArgs
    {
        public bool pleaseShow = false;
        public object popupWindow;
        public override void Clear()
        {
            pleaseShow = false;
            popupWindow = null;
            base.Clear();
        }
    }
   




   



}
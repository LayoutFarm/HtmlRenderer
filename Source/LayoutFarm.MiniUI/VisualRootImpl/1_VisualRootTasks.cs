//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;



namespace LayoutFarm
{
    public abstract class VisualRootTimerTask
    {
        RenderElement targetVisualElement;
        bool isInQueue; bool isEnabled;
        public VisualRootTimerTask(RenderElement targetVisualElement)
        {
            this.targetVisualElement = targetVisualElement;
        }
        public bool Enabled
        {
            get
            {
                return isEnabled;
            }

        }
        public void SetEnable(bool value, MyTopWindowRenderBox winroot)
        {
            isEnabled = value;
            if (isEnabled)
            {
                if (!isInQueue)
                {


                    if (winroot != null)
                    {
                        winroot.AddTimerTask(this);
                        isInQueue = true; winroot.EnableTaskTimer();
                    }
                }
            }
        }
        public virtual void Tick()
        {
        }
        public virtual void Reset()
        {
        }
        public bool IsInQueue
        {
            get
            {
                return isInQueue;
            }
            set
            {
                isInQueue = value;
            }
        }

    }



}
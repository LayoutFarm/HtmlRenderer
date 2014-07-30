//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;



namespace LayoutFarm.Presentation
{
    public abstract class ArtVisualRootTimerTask
    {
        ArtVisualElement targetVisualElement;
        bool isInQueue; bool isEnabled;
        public ArtVisualRootTimerTask(ArtVisualElement targetVisualElement)
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
        public void SetEnable(bool value, ArtVisualWindowImpl winroot)
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
//Apache2, 2014-2017, WinterDev

using System;
namespace LayoutFarm.UI
{
    class UIHoverMonitorTask : UITimerTask
    {
        int mouseMoveCounter = -1;
        EventHandler targetEventHandler;
        bool isEnabled;
        public UIHoverMonitorTask(EventHandler targetEventHandler)
        {
            this.targetEventHandler = targetEventHandler;
        }
        public override bool Enabled
        {
            get
            {
                return this.isEnabled;
            }
            set
            {
                this.isEnabled = value;
                if (value)
                {
                }
                else
                {
                }
            }
        }
        public override bool IsInQueue
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        public override void Reset()
        {
            mouseMoveCounter = -1;
        }
        public override void Tick()
        {
            mouseMoveCounter++;
            if (mouseMoveCounter > 1)
            {
                targetEventHandler(this, EventArgs.Empty);
            }
        }
    }
}
//BSD  2014 ,WinterDev  
using HtmlRenderer.Boxes;
using System.Collections.Generic;
using LayoutFarm;
namespace HtmlRenderer.WebDom
{


    public class HtmlEventArgs : LayoutFarm.UIEventArgs
    {
        UIEventName name;
        DomElement srcElement;
        DomElement currentContextElement;

        public HtmlEventArgs(UIEventName name)
        {
            this.name = name;
        }
        public UIEventName EventName
        {
            get { return this.name; }
        }
        public DomElement EventSourceElement
        {
            get { return this.srcElement; }
        }
        public DomElement CurrentContextElement
        {
            get { return this.currentContextElement; }
        }
        public bool IsCanceled
        {
            get;
            private set;
        }
        public void StopPropagation()
        {
            this.IsCanceled = true;
        }
        public void SetEventSourceElement(DomElement srcElement)
        {
            this.srcElement = srcElement;
        }
        public void SetCurrentContextElement(DomElement currentContextElement)
        {
            this.currentContextElement = currentContextElement;
        }
        public float X
        {
            get;
            set;
        }
        public float Y
        {
            get;
            set;
        }

    }



    public delegate void HtmlEventHandler(HtmlEventArgs e);

}
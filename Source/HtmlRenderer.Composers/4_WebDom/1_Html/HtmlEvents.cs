//BSD  2014 ,WinterDev  
using HtmlRenderer.Boxes;
using System.Collections.Generic;

namespace HtmlRenderer.WebDom
{


    public enum EventName
    {
        MouseDown,
        MouseMove,
        MouseUp,
        MouseOver,
        MouseLeave,
        Click,
        DoubleClick,

        KeyDown,
        KeyUp
    }
    public class HtmlEventArgs
    {
        EventName name;
        HtmlElement srcElement;
        HtmlElement currentContextElement;

        public HtmlEventArgs(EventName name)
        {
            this.name = name;
        }
        public EventName EventName
        {
            get { return this.name; }
        }
        public HtmlElement EventSourceElement
        {
            get { return this.srcElement; }
        }
        public HtmlElement CurrentContextElement
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
        public void SetEventSourceElement(HtmlElement srcElement)
        {
            this.srcElement = srcElement;
        }
        public void SetCurrentContextElement(HtmlElement currentContextElement)
        {
            this.currentContextElement = currentContextElement;
        }
    }

    
    
    public delegate void HtmlEventHandler(HtmlEventArgs e);

}
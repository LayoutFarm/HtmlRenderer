using System;
using HtmlRenderer.Composers;
using HtmlRenderer.WebDom;

namespace HtmlRenderer.Demo
{
    public abstract class DemoBase
    {
        public void StartDemo(HtmlPanel panel)
        {
            this.OnStartDemo(panel);
        }
        protected virtual void OnStartDemo(HtmlPanel panel)
        {
        }
    }
    public class DemoNoteAttribute : Attribute
    {
        public DemoNoteAttribute(string msg)
        {
            this.Message = msg;
        }
        public string Message { get; set; }
    }
    class DemoInfo
    {
        public readonly Type DemoType;
        public readonly string DemoNote;
        public DemoInfo(Type demoType, string demoNote)
        {
            this.DemoType = demoType;
            this.DemoNote = demoNote;
        }
        public override string ToString()
        {
            if(string.IsNullOrEmpty(DemoNote))
            {
                return this.DemoType.Name;
            }
            else
            {
                return this.DemoType.Name + " : "+ this.DemoNote;
            }             
        }
    }



}
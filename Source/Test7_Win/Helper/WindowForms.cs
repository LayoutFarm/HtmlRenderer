//Apache2, 2014-2017, WinterDev

using System;
using System.Collections.Generic;
using System.Windows.Forms;
namespace System.Windows.Forms
{
    public enum InnerViewportKind
    {
        GdiPlus,
        Skia,
        GL
    }
    public class Control
    {
        public Size Size { get; set; }
        protected virtual void OnLoad(EventArgs e) { }
        protected virtual void Dispose(bool disposing) { }
    }
    public class UserControl : Control
    {
    }
    public class Form : Control
    {
    }
    public static class Cursor
    {
        public static void Show() { }
    }
    public enum MouseButtons { Left, Right, Middle }
    public enum Keys { }
    public class KeyEventArgs : EventArgs
    {
        public int KeyValue { get; set; }
        public Keys KeyCode { get; set; }
    }
    public class KeyPressEventArgs : EventArgs
    {
        public char KeyChar { get; set; }
    }
    public class MouseEventArgs : EventArgs
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Delta { get; set; }
        public MouseButtons Button { get; set; }
    }

    public struct Size
    {
        public Size(int w, int h)
        {
            this.Width = w;
            this.Height = h;
        }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
//2014 Apache2, WinterDev
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
namespace LayoutFarm.UI
{
    public class PlainLayerElement : LayerElement
    {  

        List<UIElement> uiList = new List<UIElement>();
        VisualPlainLayer plainLayer; 

        public PlainLayerElement()
        {
        }
        public void AddUI(UIElement ui)
        {
            uiList.Add(ui);
        }
        public int Count
        {
            get { return this.uiList.Count; }
        }
        public void RemoveUI(UIElement ui)
        {
            //remove specific ui
        }
        public void RemoveAt(int index)
        {
            uiList.RemoveAt(index);
        }
        public void Clear()
        {
            this.uiList.Clear();
        }
        public UIElement GetElement(int index)
        {
            return this.uiList[index];
        }
    }
}
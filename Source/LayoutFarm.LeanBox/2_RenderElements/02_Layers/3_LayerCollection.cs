//2014 Apache2, WinterDev
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace LayoutFarm.Presentation
{
    //for mx single or multiple layer logic

    public class VisualLayerCollection
    {

        VisualLayer layer0;
        VisualLayer layer1;
        List<VisualLayer> otherLayers;
        int layerCount;
        public VisualLayerCollection()
        {

        }
        public VisualLayer GetLayer(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        return layer0;
                    }
                case 1:
                    {
                        return layer1;
                    }
                default:
                    {
                        if (otherLayers == null)
                        {
                            return null;
                        }
                        return otherLayers[index - 2];
                    }
            }
        }

        public int LayerCount
        {
            get { return this.layerCount; }
        }
        public VisualLayer Layer0
        {
            get { return this.layer0; }
        }
        public VisualLayer Layer1
        {
            get { return this.layer1; }
        }

        public void AddLayer(VisualLayer layer)
        {
            switch (layerCount)
            {
                case 0:
                    {
                        this.layer0 = layer;
                        layerCount++;
                    } break;
                case 1:
                    {
                        this.layer1 = layer;
                        layerCount++;
                    } break;
                default:
                    {
                        //more than two layer,
                        if (otherLayers == null)
                        {
                            otherLayers = new List<VisualLayer>();
                        }
                        otherLayers.Add(layer);
                        layerCount++;
                    } break;
            }
        }

        public void RemoveLayer(int layerIndex)
        {
            if (layerIndex >= this.layerCount)
            {
                //out of index
                return;
            }

            switch (layerIndex)
            {
                case 0:
                    {
                        this.layer0 = this.layer1;
                        if (this.otherLayers != null && this.otherLayers.Count > 0)
                        {
                            this.layer1 = this.otherLayers[0];
                            this.otherLayers.RemoveAt(0);
                        }
                        else
                        {
                            this.layer1 = null;
                        }
                        this.layerCount--;
                    } break;
                case 1:
                    {
                        if (this.otherLayers != null && this.otherLayers.Count > 0)
                        {
                            this.layer1 = this.otherLayers[0];
                            this.otherLayers.RemoveAt(0);
                        }
                        else
                        {
                            this.layer1 = null;
                        }
                        this.layerCount--;
                    } break;
                default:
                    {
                        this.otherLayers.RemoveAt(layerIndex - 2);
                        this.layerCount--;

                    } break;
            }

            if (layerCount <= 2)
            {
                //clear
                this.otherLayers = null;
            } 
        }
    }


}
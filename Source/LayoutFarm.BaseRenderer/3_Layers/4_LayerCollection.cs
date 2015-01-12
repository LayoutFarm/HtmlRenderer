// 2015,2014 ,Apache2, WinterDev
using System;
using System.Collections.Generic;
using PixelFarm.Drawing;
using System.Text;

namespace LayoutFarm.RenderBoxes
{
    //for mx single or multiple layer logic

    public class VisualLayerCollection
    {

        RenderElementLayer layer0;
        RenderElementLayer layer1;
        List<RenderElementLayer> otherLayers;
        int layerCount;
        public VisualLayerCollection()
        {

        }
        public RenderElementLayer GetLayer(int index)
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
        public RenderElementLayer Layer0
        {
            get { return this.layer0; }
        }
        public RenderElementLayer Layer1
        {
            get { return this.layer1; }
        }

        public void AddLayer(RenderElementLayer layer)
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
                            otherLayers = new List<RenderElementLayer>();
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

    public static class VisualLayerCollectionHelper
    {

   
        public static void ChildrenHitTestCore(this VisualLayerCollection layers, HitChain hitChain)
        {
            if (layers == null) return;


            int j = layers.LayerCount;
            switch (j)
            {
                case 0:
                    {

                    } break;
                case 1:
                    {
                        layers.Layer0.HitTestCore(hitChain);
                    } break;
                case 2:
                    {
                        if (layers.Layer1.HitTestCore(hitChain))
                        {
                            layers.Layer0.HitTestCore(hitChain);
                        }
                    } break;
                default:
                    {
                        for (int i = j - 1; i >= 0; --i)
                        {
                            var layer = layers.GetLayer(i);
                            if (!layer.HitTestCore(hitChain))
                            {
                                return;
                            }

                        }

                    } break;
            }
        }

        public static void ForceTopDownReArrangeContent(this VisualLayerCollection layers)
        {
            if (layers == null)
            {
                return;
            }
            //---------------
            int j = layers.LayerCount;
            switch (j)
            {
                case 0:
                    {

                    } break;
                case 1:
                    {
                        layers.Layer0.TopDownReArrangeContent();
                    } break;
                case 2:
                    {
                        layers.Layer1.TopDownReArrangeContent();
                        layers.Layer0.TopDownReArrangeContent();
                    } break;
                default:
                    {
                        for (int i = j - 1; i >= 0; --i)
                        {
                            layers.GetLayer(i).TopDownReArrangeContent();
                        }
                    } break;
            }
        }
        public static Size TopDownReCalculateContentSize(this VisualLayerCollection layers)
        {

            if (layers != null)
            {
                int j = layers.LayerCount;
                switch (j)
                {
                    case 0:
                        {

                        } break;
                    case 1:
                        {
                            layers.Layer0.TopDownReCalculateContentSize();
                            return layers.Layer0.PostCalculateContentSize;
                        }
                    case 2:
                        {
                            layers.Layer1.TopDownReCalculateContentSize();
                            layers.Layer0.TopDownReCalculateContentSize();
                            return layers.Layer0.PostCalculateContentSize;
                        }
                    default:
                        {
                            for (int i = j - 1; i >= 0; --i)
                            {
                                layers.GetLayer(i).TopDownReCalculateContentSize();
                            }
                            return layers.Layer0.PostCalculateContentSize;
                        }
                }
            }
            return Size.Empty;
        }

        public static void ClearAllContentInEachLayer(this VisualLayerCollection layers)
        {
            if (layers == null)
            {
                return;
            }
            //---------------
            int j = layers.LayerCount;
            switch (j)
            {
                case 0:
                    {

                    } break;
                case 1:
                    {
                        layers.Layer0.Clear();
                    } break;
                case 2:
                    {
                        layers.Layer1.Clear();
                        layers.Layer0.Clear();
                    } break;
                default:
                    {
                        for (int i = j - 1; i >= 0; --i)
                        {
                            layers.GetLayer(i).Clear();
                        }
                    } break;
            }
        }
        public static void LayersDrawContent(this VisualLayerCollection layers, Canvas canvas, Rectangle updateArea)
        {
            if (layers == null)
            {
                return;
            }
            //---------------
            int j = layers.LayerCount;
            switch (j)
            {
                case 0:
                    {

                    } break;
                case 1:
                    {
                        layers.Layer0.DrawChildContent(canvas, updateArea);
                    } break;
                case 2:
                    {
                        layers.Layer0.DrawChildContent(canvas, updateArea);
                        layers.Layer1.DrawChildContent(canvas, updateArea);

                    } break;
                default:
                    {
                        for (int i = 0; i < j; ++i)
                        {
                            layers.GetLayer(i).DrawChildContent(canvas, updateArea);
                        }
                    } break;
            }
        }

     
#if DEBUG
        public static void dbug_DumpVisualProps(this VisualLayerCollection layers, dbugLayoutMsgWriter writer)
        {

            writer.EnterNewLevel();


            int j = layers.LayerCount;
            switch (j)
            {
                case 0:
                    {

                    } break;
                case 1:
                    {
                        layers.Layer0.dbug_DumpElementProps(writer);
                    } break;
                case 2:
                    {

                        layers.Layer0.dbug_DumpElementProps(writer);
                        layers.Layer1.dbug_DumpElementProps(writer);
                    } break;
                default:
                    {
                        for (int i = 0; i < j; ++i)
                        {
                            layers.GetLayer(i).dbug_DumpElementProps(writer);
                        }
                    } break;
            }
            writer.LeaveCurrentLevel();
        }
#endif
    }

}
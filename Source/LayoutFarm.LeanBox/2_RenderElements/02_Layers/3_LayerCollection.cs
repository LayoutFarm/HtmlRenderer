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

    public static class VisualLayerCollectionHelper
    {

        public static void PrepareOriginalChildContentDrawingChain(this VisualLayerCollection layers, VisualDrawingChain chain)
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
                            layers.Layer0.PrepareDrawingChain(chain);
                            //#if DEBUG
                            //                            debug_RecordLayerInfo(layers.Layer0);
                            //#endif
                        } break;
                    case 2:
                        {
                            layers.Layer1.PrepareDrawingChain(chain);
                            layers.Layer0.PrepareDrawingChain(chain);
                            //#if DEBUG
                            //                            debug_RecordLayerInfo(layers.Layer0);
                            //#endif

                            //#if DEBUG
                            //                            debug_RecordLayerInfo(layers.Layer1);
                            //#endif
                        } break;
                    default:
                        {
                            for (int i = j - 1; i >= -1; --i)
                            {
                                var layer = layers.GetLayer(i);
                                layer.PrepareDrawingChain(chain);
                                //#if DEBUG
                                //                                debug_RecordLayerInfo(layer);
                                //#endif
                            }
                        } break;
                }
            }
        }
        public static void ChildrenHitTestCore(this VisualLayerCollection layers, HitPointChain artHitResult)
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
                        layers.Layer0.HitTestCore(artHitResult);
                    } break;
                case 2:
                    {
                        if (layers.Layer1.HitTestCore(artHitResult))
                        {
                            layers.Layer0.HitTestCore(artHitResult);
                        }
                    } break;
                default:
                    {
                        for (int i = j - 1; i >= 0; --i)
                        {
                            var layer = layers.GetLayer(i);
                            if (!layer.HitTestCore(artHitResult))
                            {
                                return;
                            }

                        }

                    } break;
            }
        }

        public static void ForceTopDownReArrangeContent(this VisualLayerCollection layers, LayoutPhaseVisitor vinv)
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
                        layers.Layer0.TopDownReArrangeContent(vinv);
                    } break;
                case 2:
                    {
                        layers.Layer1.TopDownReArrangeContent(vinv);
                        layers.Layer0.TopDownReArrangeContent(vinv);
                    } break;
                default:
                    {
                        for (int i = j - 1; i >= 0; --i)
                        {
                            layers.GetLayer(i).TopDownReArrangeContent(vinv);
                        }
                    } break;
            }
        }
        public static Size TopDownReCalculateContentSize(this VisualLayerCollection layers, LayoutPhaseVisitor vinv)
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
                            layers.Layer0.TopDownReCalculateContentSize(vinv);
                            return layers.Layer0.PostCalculateContentSize;
                        }
                    case 2:
                        {
                            layers.Layer1.TopDownReCalculateContentSize(vinv);
                            layers.Layer0.TopDownReCalculateContentSize(vinv);
                            return layers.Layer0.PostCalculateContentSize;
                        }
                    default:
                        {
                            for (int i = j - 1; i >= 0; --i)
                            {
                                layers.GetLayer(i).TopDownReCalculateContentSize(vinv);
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
        public static void LayersDrawContent(this VisualLayerCollection layers, CanvasBase canvas, InternalRect updateArea)
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
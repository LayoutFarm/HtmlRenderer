//Apache2, 2014-present, WinterDev
//MS-PL, Apache2 some parts derived from github.com/vvvv/svg 


using System.Collections.Generic;
using LayoutFarm.Composers;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.UI;
using PaintLab.Svg;

namespace LayoutFarm.Svg
{
    partial class SvgRootEventPortal
    {
        Stack<SvgHitChain> hitChainPools = new Stack<SvgHitChain>();
        HtmlElement _elementNode;
        public SvgRootEventPortal(HtmlElement elementNode)
        {
            this._elementNode = elementNode;
        }
        public CssBoxSvgRoot SvgRoot
        {
            get;
            set;
        }


        //==================================================
        SvgHitChain GetFreeHitChain()
        {
            if (hitChainPools.Count > 0)
            {
                return hitChainPools.Pop();
            }
            else
            {
                return new SvgHitChain();
            }
        }
        void ReleaseHitChain(SvgHitChain hitChain)
        {
            hitChain.Clear();
            this.hitChainPools.Push(hitChain);
        }

        public static void HitTestCore(SvgElement root, SvgHitChain chain, float x, float y)
        {
            ////1. 
            //chain.AddHit(root, x, y); 
            ////2. find hit child
            //int j = root.ChildCount;
            //for (int i = 0; i < j; ++i)
            //{ 
            //}
            //var child = root.GetFirstNode(); 
            //TODO: review here again!
            // throw new System.NotImplementedException();
            //TODO: check hit test core on svg again!
            //while (child != null)
            //{
            //    //test hit text core again
            //    var node = child.Value;
            //    if (node.HitTestCore(chain, x, y))
            //    {
            //        break;
            //    }
            //    child = child.Next;
            //}
        }

        static void ForEachOnlyEventPortalBubbleUp(UIEventArgs e, SvgHitChain hitPointChain, EventPortalAction eventPortalAction)
        {
            //only listener that need tunnel down 
            for (int i = hitPointChain.Count - 1; i >= 0; --i)
            {
                //propagate up 
                var hitInfo = hitPointChain.GetHitInfo(i);
                SvgElement svg = hitInfo.svg.GetController() as SvgElement;
                if (svg != null)
                {
                    var controller = SvgElement.UnsafeGetController(svg) as IEventPortal;
                    if (controller != null)
                    {
                        e.SetLocation((int)hitInfo.x, (int)hitInfo.y);
                        if (eventPortalAction(controller))
                        {
                            return;
                        }
                    }
                }
            }
        }

        static void ForEachEventListenerBubbleUp(UIEventArgs e, SvgHitChain hitChain, EventListenerAction listenerAction)
        {
            for (int i = hitChain.Count - 1; i >= 0; --i)
            {
                //propagate up 
                SvgHitInfo hitInfo = hitChain.GetHitInfo(i);

                IUIEventListener controller = SvgElement.UnsafeGetController(hitInfo.GetSvgElement()) as IUIEventListener;
                //switch (hitInfo.hitObjectKind)
                //{
                //    default:
                //        {
                //            continue;
                //        }
                //    case HitObjectKind.Run:
                //        {
                //            CssRun run = (CssRun)hitInfo.hitObject;
                //            controller = CssBox.UnsafeGetController(run.OwnerBox) as IEventListener;

                //        } break;
                //    case HitObjectKind.CssBox:
                //        {
                //            CssBox box = (CssBox)hitInfo.hitObject;
                //            controller = CssBox.UnsafeGetController(box) as IEventListener;
                //        } break;
                //}

                //---------------------
                if (controller != null)
                {
                    //found controller

                    e.CurrentContextElement = controller;
                    e.SetLocation((int)hitInfo.x, (int)hitInfo.y);
                    if (listenerAction())
                    {
                        return;
                    }
                }
            }
        }

        static void ForEachSvgElementBubbleUp(UIEventArgs e, SvgHitChain hitChain, EventListenerAction listenerAction)
        {
            for (int i = hitChain.Count - 1; i >= 0; --i)
            {
                //propagate up 
                var hitInfo = hitChain.GetHitInfo(i);
                //---------------------
                //hit on element  

                e.SetLocation((int)hitInfo.x, (int)hitInfo.y);
                if (listenerAction())
                {
                    return;
                }
            }
        }
        static void SetEventOrigin(UIEventArgs e, SvgHitChain hitChain)
        {
            int count = hitChain.Count;
            if (count > 0)
            {
                var hitInfo = hitChain.GetHitInfo(count - 1);
                e.ExactHitObject = hitInfo;
            }
        }
        void ClearPreviousSelection()
        {
            //TODO: add clear svg selection here
        }
    }
}
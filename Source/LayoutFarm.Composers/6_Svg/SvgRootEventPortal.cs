//Apache2, 2014-2017, WinterDev
//MS-PL, Apache2 some parts derived from github.com/vvvv/svg 


using System.Collections.Generic;
using LayoutFarm.Composers;
using LayoutFarm.HtmlBoxes;
using LayoutFarm.UI;
namespace LayoutFarm.Svg
{
    partial class SvgRootEventPortal
    {
        Stack<SvgHitChain> hitChainPools = new Stack<SvgHitChain>();
        HtmlElement elementNode;
        public SvgRootEventPortal(HtmlElement elementNode)
        {
            this.elementNode = elementNode;
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
            //1. 
            chain.AddHit(root, x, y);
            //2. find hit child
            var child = root.GetFirstNode();
            while (child != null)
            {
                var node = child.Value;
                if (node.HitTestCore(chain, x, y))
                {
                    break;
                }
                child = child.Next;
            }
        }

        static void ForEachOnlyEventPortalBubbleUp(UIEventArgs e, SvgHitChain hitPointChain, EventPortalAction eventPortalAction)
        {
            //only listener that need tunnel down 
            for (int i = hitPointChain.Count - 1; i >= 0; --i)
            {
                //propagate up 
                var hitInfo = hitPointChain.GetHitInfo(i);
                SvgElement svg = hitInfo.svg;
                if (svg != null)
                {
                    var controller = SvgElement.UnsafeGetController(hitInfo.svg) as IEventPortal;
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
                var hitInfo = hitChain.GetHitInfo(i);
                IEventListener controller = SvgElement.UnsafeGetController(hitInfo.svg) as IEventListener;
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
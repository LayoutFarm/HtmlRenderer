//MIT, 2014-present, WinterDev

using System.IO;
using PixelFarm.Drawing;
using PixelFarm.CpuBlit;
using PixelFarm.CpuBlit.VertexProcessing;
using PaintLab.Svg;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("9.4_SvgParts")]
    class Demo9_4_SvgParts : App
    {

        //QuadControllerUI _quadController = new QuadControllerUI();
        //PolygonControllerUI _quadPolygonController = new PolygonControllerUI();
        bool _hitTestOnSubPath = false;

        AppHost _appHost;

        VgVisualDoc _vgVisualDoc;

        UISprite CreateUISpriteFromVgVisualElem(VgVisualElement vgVisualElem, bool wrapWithVgVisualUse = false)
        {
            if (wrapWithVgVisualUse)
            {
                //then we create a 'svg use' element
                //and wrap the original vgVisualElement
                vgVisualElem = _vgVisualDoc.CreateVgUseVisualElement(vgVisualElem);

            }

            PixelFarm.CpuBlit.RectD org_rectD = vgVisualElem.GetRectBounds();
            org_rectD.Offset(-org_rectD.Left, -org_rectD.Bottom);
            vgVisualElem.DisableBackingImage = true;
            //-----------------------------------------             
            UISprite uiSprite = new UISprite(10, 10); //init size = (10,10), location=(0,0)       
            uiSprite.DisableBmpCache = true;



            uiSprite.LoadVg(vgVisualElem);//  



            _appHost.AddChild(uiSprite);

            var spriteEvListener = new GeneralEventListener();
            uiSprite.AttachExternalEventListener(spriteEvListener);
            spriteEvListener.MouseMove += e1 =>
            {
                if (e1.IsDragging)
                {
                    //when drag on sprie  
                    uiSprite.InvalidateOuterGraphics();
                    uiSprite.SetLocation(
                        uiSprite.Left + e1.XDiff,
                        uiSprite.Top + e1.YDiff);
                }
            };
            spriteEvListener.MouseDown += e1 =>
            {
                if (_hitTestOnSubPath)
                {
                    //find which part ...
                    VgHitInfo hitInfo = uiSprite.FindRenderElementAtPos(e1.X, e1.Y, true);

                    if (hitInfo.hitElem != null &&
                        hitInfo.hitElem.VxsPath != null)
                    {

                        PixelFarm.CpuBlit.RectD bounds = hitInfo.copyOfVxs.GetBoundingRect();


                    }
                    else
                    {
                        //_rectBoundsWidgetBox.Visible = false;
                        // _rectBoxController.Visible = false;
                    }
                }
                else
                {
                    //hit on sprite  
                    if (e1.Ctrl)
                    {
                        //test*** 
                        //
                        uiSprite.GetElementBounds(out float left, out float top, out float right, out float bottom);
                        //
                        using (Tools.BorrowRect(out SimpleRect s))
                        using (Tools.BorrowVxs(out var v1))
                        {
                            s.SetRect(left - uiSprite.ActualXOffset,
                                bottom - uiSprite.ActualYOffset,
                                right - uiSprite.ActualXOffset,
                                top - uiSprite.ActualYOffset);
                            //TODO: review here
                            //s.MakeVxs(v1);
                            //_polygonController.UpdateControlPoints(v1.CreateTrim());
                        }
                    }
                    else
                    {

                        //_rectBoundsWidgetBox.SetTarget(_uiSprite);
                        //_rectBoundsWidgetBox.SetLocationAndSize(    //blue
                        //      (int)_uiSprite.Left, (int)_uiSprite.Top,
                        //      (int)_uiSprite.Width, (int)_uiSprite.Height);
                        ////_rectBoxController.SetTarget(_uiSprite);

                        ////_rectBoxController.UpdateControllerBoxes(_rectBoundsWidgetBox);  //control 4 corners
                        //_rectBoundsWidgetBox.Visible = true;
                        ////_rectBoxController.Visible = true;

                        //UpdateTransformedShape2();
                    }
                }
            };

            return uiSprite;
        }
        protected override void OnStart(AppHost host)
        {
            _appHost = host;//** 
            string svgfile = "../Test8_HtmlRenderer.Demo/Samples/Svg/others/volcano3.svg";
            _vgVisualDoc = VgVisualDocHelper.CreateVgVisualDocFromFile(svgfile);
            //
            VgVisualElement vgVisualElem = _vgVisualDoc.VgRootElem;
            {
                UISprite wholeImgSprite = CreateUISpriteFromVgVisualElem(vgVisualElem);

            }
            // 
            if (_vgVisualDoc.TryGetVgVisualElementById("path62_larva3", out VgVisualElement vgPart))
            {
                UISprite larvaSprite = CreateUISpriteFromVgVisualElem(vgPart, true);
                larvaSprite.SetTransformation(Affine.NewScaling(4));//scale
                larvaSprite.SetLocation(100, 100);
            }
        }
    }


}
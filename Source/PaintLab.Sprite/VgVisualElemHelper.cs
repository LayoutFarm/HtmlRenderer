//MIT, 2018-present, WinterDev

using System.Collections.Generic;
using System.IO;

using PixelFarm.Drawing;
using PixelFarm.CpuBlit.VertexProcessing;
using PaintLab.Svg;

namespace LayoutFarm
{

    public static class VgVisualElemHelper
    {
        public static VgVisualElement CreateVgVisualElemFromSvgContent(string svgContent)
        {

            SvgDocBuilder docBuidler = new SvgDocBuilder();
            SvgParser parser = new SvgParser(docBuidler);
            WebLexer.TextSnapshot textSnapshot = new WebLexer.TextSnapshot(svgContent);
            parser.ParseDocument(textSnapshot);//start document parsing

            //TODO: review this step again
            VgVisualDocBuilder builder = new VgVisualDocBuilder();
            SvgDocument svgDoc = docBuidler.ResultDocument;
            //optional 
            svgDoc.OriginalContent = svgContent;
            //-------------------------------------------------------------
            VgVisualElement vgVisRootElem = builder.CreateVgVisualDoc(svgDoc, svgElem =>
            {
            }).VgRootElem;
            //
            vgVisRootElem.OwnerDocument = svgDoc;//tmp


            return vgVisRootElem;
        }
        public static VgVisualElement ReadSvgFile(string filename)
        {

            VgVisualElement vgx = CreateVgVisualElemFromSvgContent(System.IO.File.ReadAllText(filename));
            vgx.OwnerDocument.OriginalFilename = filename;
            return vgx;
        }


        static Typography.Contours.GlyphMeshStore _glyphMaskStore = null;

        static Dictionary<string, Typography.OpenFont.Typeface> s_loadedTypefaces = new Dictionary<string, Typography.OpenFont.Typeface>();


        public static VgVisualElement CreateVgVisualElementFromGlyph(string actualFontFile, float sizeInPts, char c)
        {

            if (!s_loadedTypefaces.TryGetValue(actualFontFile, out Typography.OpenFont.Typeface typeface))
            {
                //create vgrender vx from font-glyph
                //
                using (System.IO.FileStream fs = new FileStream(actualFontFile, FileMode.Open))
                {
                    Typography.OpenFont.OpenFontReader reader = new Typography.OpenFont.OpenFontReader();
                    typeface = reader.Read(fs);
                }
            }
            if (_glyphMaskStore == null)
            {
                _glyphMaskStore = new Typography.Contours.GlyphMeshStore();
                _glyphMaskStore.FlipGlyphUpward = true;
            }
            _glyphMaskStore.SetFont(typeface, sizeInPts);
            //-----------------
            VertexStore vxs = _glyphMaskStore.GetGlyphMesh(typeface.LookupIndex(c));
            var spec = new SvgPathSpec() { FillColor = Color.Red };
            VgVisualDoc renderRoot = new VgVisualDoc();
            VgVisualElement renderE = new VgVisualElement(WellknownSvgElementName.Path, spec, renderRoot);


            //offset the original vxs to (0,0) bounds
            //PixelFarm.CpuBlit.RectD bounds = vxs.GetBoundingRect();
            //Affine translate = Affine.NewTranslation(-bounds.Left, -bounds.Bottom);
            //renderE._vxsPath = vxs.CreateTrim(translate);


            PixelFarm.CpuBlit.RectD bounds = vxs.GetBoundingRect();
            Affine translate = Affine.NewTranslation(-bounds.Left, -bounds.Bottom);
            renderE.VxsPath = vxs.CreateTrim(translate);
            return renderE;
        }


    }
}
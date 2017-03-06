//MIT, 2016-2017, WinterDev 
using System;
using System.Collections.Generic;
using System.IO;
using Typography.OpenFont;
using Typography.TextLayout;
using PixelFarm.Agg;

namespace PixelFarm.Drawing.Fonts
{

    public class VxsTextPrinter : ITextPrinter
    {
        /// <summary>
        /// target canvas
        /// </summary>
        CanvasPainter canvasPainter;        
        IFontLoader _fontLoader;
        RequestFont _font;

        GlyphPathBuilder _glyphPathBuilder;
        GlyphLayout _glyphLayout = new GlyphLayout();
        Dictionary<string, GlyphPathBuilder> _cacheGlyphPathBuilders = new Dictionary<string, GlyphPathBuilder>();
        List<GlyphPlan> glyphPlanList = new List<GlyphPlan>(20);

        public VxsTextPrinter(CanvasPainter canvasPainter, IFontLoader fontLoader)
        {
            this.canvasPainter = canvasPainter;
            this._fontLoader = fontLoader;
#if DEBUG

            Typography.OpenFont.ScriptLang scLang = Typography.OpenFont.ScriptLangs.GetRegisteredScriptLang(canvasPainter.CurrentFont.ScriptCode.shortname);
            if (scLang == null)
            {
                throw new NotSupportedException("unknown script lang");
            }
#endif
            this.ScriptLang = scLang;
            ChangeFont(canvasPainter.CurrentFont);
        }
        public void ChangeFont(RequestFont font)
        {
            //1.  resolve actual font file
            this._font = font;
            string resolvedFontFilename = _fontLoader.GetFont(font.Name, InstalledFontStyle.Regular).FontPath;
            if (resolvedFontFilename != _currentFontFilename)
            {
                //switch to another font  
                //store current typeface to cache
                if (_glyphPathBuilder != null && !_cacheGlyphPathBuilders.ContainsKey(resolvedFontFilename))
                {
                    _cacheGlyphPathBuilders[_currentFontFilename] = _glyphPathBuilder;
                }
                //check if we have this in cache ?
                //if we don't have it, this _currentTypeface will set to null                   
                _cacheGlyphPathBuilders.TryGetValue(resolvedFontFilename, out _glyphPathBuilder);
            }
            this._currentFontFilename = resolvedFontFilename;

        }
        public void ChangeFontColor(Color fontColor)
        {
            //change font color

#if DEBUG
            Console.Write("please impl change font color");
#endif
        }
        public void DrawString(string text, double x, double y)
        {
            DrawString(text.ToCharArray(), x, y);
        }
        public void DrawString(char[] text, double x, double y)
        {

            //1. update current type face
            Typeface typeface = UpdateTypefaceAndGlyphBuilder();

            //2. layout glyphs with selected layout techniue 
            glyphPlanList.Clear();

            //TODO: review this again, we should use pixel?
            float fontSizePoint = _font.SizeInPoints;//font size in point unit,

            _glyphLayout.Layout(typeface, fontSizePoint, text, glyphPlanList);

            float pxScale = typeface.CalculateFromPointToPixelScale(fontSizePoint);
            int j = glyphPlanList.Count;
            float ox = canvasPainter.OriginX;
            float oy = canvasPainter.OriginY;
            for (int i = 0; i < j; ++i)
            {
                GlyphPlan glyphPlan = glyphPlanList[i];
                //-----------------------------------

                //TODO: review here ***
                //PERFORMANCE revisit here 
                //if we have create a vxs we can cache it for later use?
                //-----------------------------------  
                _glyphPathBuilder.BuildFromGlyphIndex(glyphPlan.glyphIndex, fontSizePoint);
                //-----------------------------------  
                _txToVxs.Reset();
                _glyphPathBuilder.ReadShapes(_txToVxs);

                //TODO: review here, 

                VertexStore outputVxs = _vxsPool.GetFreeVxs();
                _txToVxs.WriteOutput(outputVxs, _vxsPool, pxScale);
                canvasPainter.SetOrigin((float)(glyphPlan.x + x), (float)(glyphPlan.y + y));
                canvasPainter.Fill(outputVxs);
                _vxsPool.Release(ref outputVxs);

            }
            //restore prev origin
            canvasPainter.SetOrigin(ox, oy);
        }

        //-----------------------
        VertexStorePool _vxsPool = new VertexStorePool();
        GlyphTranslatorToVxs _txToVxs = new GlyphTranslatorToVxs();
        string _currentFontFilename = "";
        public PositionTechnique PositionTechnique
        {
            get { return _glyphLayout.PositionTechnique; }
            set { _glyphLayout.PositionTechnique = value; }
        }
        public HintTechnique HintTechnique
        {
            get;
            set;
        }
        public bool EnableLigature
        {
            get { return _glyphLayout.EnableLigature; }
            set { this._glyphLayout.EnableLigature = value; }
        }
        public Typography.OpenFont.ScriptLang ScriptLang
        {
            get
            {
                return _glyphLayout.ScriptLang;
            }
            set
            {
                _glyphLayout.ScriptLang = value;
            }
        }


        Typeface UpdateTypefaceAndGlyphBuilder()
        {
            //1. update _glyphPathBuilder for current typeface

            if (_glyphPathBuilder == null)
            {
                //TODO: review here about how to load font file and glyph builder 
                //1. read typeface ...   
                Typeface typeface = null;
                using (FileStream fs = new FileStream(_currentFontFilename, FileMode.Open, FileAccess.Read))
                {
                    var reader = new OpenFontReader();
                    typeface = reader.Read(fs);
                }
                //2. and create
                _glyphPathBuilder = new GlyphPathBuilder(typeface);
                return typeface;
            }
            else
            {
                return _glyphPathBuilder.Typeface;
            }
        }
    }




}
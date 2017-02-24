﻿//MIT, 2016-2017, WinterDev 
using System;
using System.Collections.Generic;
using System.IO;
using Typography.OpenFont;
using Typography.TextLayout;
using PixelFarm.Agg;

namespace PixelFarm.Drawing.Fonts
{

    
    public class TextPrinter : ITextPrinter
    {

        Agg.CanvasPainter canvasPainter;
        VxsTextPrinter vxsTextPrinter = new VxsTextPrinter();
        List<GlyphPlan> glyphPlanList = new List<GlyphPlan>(20);

        public TextPrinter(Agg.CanvasPainter canvasPainter)
        {
            this.canvasPainter = canvasPainter;

            vxsTextPrinter.FontFile = "d:\\WImageTest\\tahoma.ttf";
            vxsTextPrinter.ScriptLang = Typography.OpenFont.ScriptLangs.Thai;
        }


        public void DrawString(string text, double x, double y)
        {
            glyphPlanList.Clear();
            RequestFont currentFont = canvasPainter.CurrentFont;
            vxsTextPrinter.Print(currentFont.SizeInPoints, text, glyphPlanList);

            int glyphListLen = glyphPlanList.Count;

            float ox = canvasPainter.OriginX;
            float oy = canvasPainter.OriginY;

            for (int i = 0; i < glyphListLen; ++i)
            {
                GlyphPlan glyphPlan = glyphPlanList[i];
                canvasPainter.SetOrigin((float)(glyphPlan.x + x), (float)(glyphPlan.y + y));
                canvasPainter.Fill((VertexStore)glyphPlan.vxs);
            }
            canvasPainter.SetOrigin(ox, oy);

        }
    }

    class VxsTextPrinter
    {
        Typeface _currentTypeface;
        GlyphLayout _glyphLayout = new GlyphLayout();
        Dictionary<string, Typeface> _cachedTypefaces = new Dictionary<string, Typeface>();

        string _currentFontFilename = "";

        public VxsTextPrinter()
        {
            //default         
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
        public PositionTecnhique PositionTechnique
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

        public string FontFile
        {
            get { return _currentFontFilename; }
            set
            {
                if (value != _currentFontFilename)
                {
                    //switch to another font                   

                    //store current typeface to cache
                    if (_currentTypeface != null && !_cachedTypefaces.ContainsKey(value))
                    {
                        _cachedTypefaces[_currentFontFilename] = _currentTypeface;
                    }

                    //chkeck if we have this in cache ?
                    _cachedTypefaces.TryGetValue(value, out _currentTypeface);

                }
                this._currentFontFilename = value;
            }
        }
        public void Print(float size, string str, List<GlyphPlan> glyphPlanBuffer)
        {
            if (_currentTypeface == null)
            {
                OpenFontReader reader = new OpenFontReader();
                using (FileStream fs = new FileStream(_currentFontFilename, FileMode.Open))
                {
                    _currentTypeface = reader.Read(fs);
                }
            }
            //-----------
            Print(_currentTypeface, size, str, glyphPlanBuffer);
        }
        public void Print(Typeface typeface, float size, string str, List<GlyphPlan> glyphPlanBuffer)
        {
            Print(typeface, size, str.ToCharArray(), glyphPlanBuffer);
        }

        List<ushort> inputGlyphs = new List<ushort>(); //not thread safe*** 
        void Print(Typeface typeface, float size, char[] str, List<GlyphPlan> glyphPlanBuffer)
        {
            //1. layout
            _glyphLayout.Layout(typeface, size, str, glyphPlanBuffer);
            var glyphPathBuilder = new MyGlyphPathBuilder(typeface);
            int j = glyphPlanBuffer.Count;

            float pxScale = typeface.CalculateFromPointToPixelScale(size);
            for (int i = 0; i < j; ++i)
            {

                GlyphPlan glyphPlan = glyphPlanBuffer[i];
                //-----------------------------------
                //check if we static vxs/bmp for this glyph
                //if not, create and cache
                //-----------------------------------  
                glyphPathBuilder.BuildFromGlyphIndex(glyphPlan.glyphIndex, size);
                //-----------------------------------  
                var vxsBuilder = new GlyphPathBuilderVxs();
                glyphPathBuilder.ReadShapes(vxsBuilder);
                glyphPlan.vxs = vxsBuilder.GetVxs(pxScale);
            }
        }
    }

    public enum HintTechnique
    {
        /// <summary>
        /// no hinting
        /// </summary>
        None,
        /// <summary>
        /// truetype instruction
        /// </summary>
        TrueTypeInstruction,
        /// <summary>
        /// truetype instruction vertical only
        /// </summary>
        TrueTypeInstruction_VerticalOnly,
        /// <summary>
        /// custom hint
        /// </summary>
        CustomAutoFit
    }
}
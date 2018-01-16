﻿//MIT, 2014-2018, WinterDev

using System.Collections.Generic;
using Typography.TextLayout;
using Typography.Rendering;

namespace SampleWinForms.UI
{

    class VisualLine
    {

        SmallLine _line;
        DevTextPrinterBase _printer;
        public VisualLine()
        {

        }
        public void BindLine(SmallLine line)
        {
            this._line = line;
        }
        public void BindPrinter(DevTextPrinterBase printer)
        {
            _printer = printer;
        }
        public float X { get; set; }
        public float Y { get; set; }
        public void SetCharIndexFromPos(float x, float y)
        {
            _line.SetCharIndexFromPos(x, y);
        }

        public void Draw()
        {

            UnscaledGlyphPlanList glyphPlans = _line._glyphPlans;
            List<UserCodePointToGlyphIndex> userCharToGlyphIndexMap = _line._userCodePointToGlyphIndexMap;
            if (_line.ContentChanged)
            {
                //TODO: or font face/font-size change 
                //re-calculate 
                char[] textBuffer = _line._charBuffer.ToArray();
                glyphPlans.Clear();
                userCharToGlyphIndexMap.Clear();
                //read glyph plan and userCharToGlyphIndexMap                

                _printer.GenerateGlyphPlan(textBuffer, 0, textBuffer.Length, glyphPlans, userCharToGlyphIndexMap);


                _line.ContentChanged = false;
            }

            if (glyphPlans.Count > 0)
            {

                _printer.DrawFromGlyphPlans(glyphPlans, X, Y);
                //draw caret 
                //not blink in this version
                int caret_index = _line.CaretCharIndex;
                //find caret pos based on glyph plan
                //TODO: check when do gsub (glyph number may not match with user char number)                 

                if (caret_index == 0)
                {
                    _printer.DrawCaret(X, this.Y);
                }
                else
                {
                    //UserCodePointToGlyphIndex map = userCharToGlyphIndexMap[caret_index - 1];
                    //GlyphPlan p = glyphPlans[map.glyphIndexListOffset_plus1 + map.len - 2];
                    //_printer.DrawCaret(X + (p.ExactX + p.AdvanceX), this.Y);
                }
            }
            else
            {

                _printer.DrawCaret(X, this.Y);
            }
        }
    }


}
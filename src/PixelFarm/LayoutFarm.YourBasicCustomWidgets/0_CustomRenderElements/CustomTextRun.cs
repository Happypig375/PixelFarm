﻿//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm.CustomWidgets
{


    public class CustomTextRun : RenderElement
    {
        char[] textBuffer;
        Color textColor = Color.Black;
        RequestFont _font;
        RenderVxFormattedString renderVxFormattedString;
#if DEBUG
        public bool dbugBreak;
#endif
        public CustomTextRun(RootGraphic rootgfx, int width, int height)
            : base(rootgfx, width, height)
        {
            _font = rootgfx.DefaultTextEditFontInfo;
        }
        public override void ResetRootGraphics(RootGraphic rootgfx)
        {
            DirectSetRootGraphics(this, rootgfx);
        }
        public string Text
        {
            get { return new string(this.textBuffer); }
            set
            {

                if (value == null)
                {
                    this.textBuffer = null;
                }
                else
                {
                    this.textBuffer = value.ToCharArray();
                }

                renderVxFormattedString = null;
            }
        }
        public Color TextColor
        {
            get { return this.textColor; }
            set { this.textColor = value; }
        }
        public RequestFont RequestFont
        {
            get { return _font; }
            set
            {
                _font = value;
            }
        }
        public override void CustomDrawToThisCanvas(DrawBoard canvas, Rectangle updateArea)
        {
            if (this.textBuffer != null)
            {
                var prevColor = canvas.CurrentTextColor;
                canvas.CurrentTextColor = textColor;
                canvas.CurrentFont = _font;

                //for faster text drawing
                //we create a formatted-text 
                //canvas.DrawText(this.textBuffer, this.X, this.Y);
                if (renderVxFormattedString == null)
                {
                    renderVxFormattedString = canvas.CreateFormattedString(textBuffer, 0, textBuffer.Length);
                }
                canvas.DrawRenderVx(renderVxFormattedString, 0, 0); //X=0,Y=0 because  we offset the canvas to this Y before drawing this
                canvas.CurrentTextColor = prevColor;
            }
        }
    }
}
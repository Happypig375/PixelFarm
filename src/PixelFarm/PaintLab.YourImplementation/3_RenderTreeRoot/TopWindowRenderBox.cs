﻿//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
namespace LayoutFarm
{
    public class TopWindowRenderBox : RenderBoxBase
    {
        public TopWindowRenderBox(RootGraphic rootGfx, int width, int height)
            : base(rootGfx, width, height)
        {
            this.IsTopWindow = true;
            this.HasSpecificWidthAndHeight = true;
        }
        protected override void DrawBoxContent(DrawBoard canvas, Rectangle updateArea)
        {
            //TODO: implement FillRect() with no blending ... , or FastClear()

            canvas.FillRectangle(Color.White, 0, 0, this.Width, this.Height);
            this.DrawDefaultLayer(canvas, ref updateArea);
        }
    }
}
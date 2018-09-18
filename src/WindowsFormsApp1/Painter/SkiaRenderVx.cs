﻿//MIT, 2016-present, WinterDev

using PixelFarm.CpuBlit;
namespace PixelFarm.Drawing.Skia
{
    class WinGdiRenderVx : RenderVx
    {
        internal VertexStoreSnap snap;
        internal SkiaSharp.SKPath path;
        public WinGdiRenderVx(VertexStoreSnap snap)
        {
            this.snap = snap;
        }
    }
    class SkiaRenerVxFormattedString : RenderVxFormattedString
    {
        string str;
        public SkiaRenerVxFormattedString(string str)
        {
            this.str = str;
        }
        public override string OriginalString
        {
            get { return str; }
        }
    }
}
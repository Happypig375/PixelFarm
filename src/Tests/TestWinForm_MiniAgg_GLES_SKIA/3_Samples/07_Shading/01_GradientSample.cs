﻿//BSD, 2014-present, WinterDev
//MatterHackers

using System;
using System.Diagnostics;
using PixelFarm.Drawing;
using PixelFarm.Agg.VertexSource;
using PixelFarm.Agg.Transform;
using Mini;
namespace PixelFarm.Agg.Sample_Gradient
{
    [Info(OrderCode = "01_1")]
    [Info("This �sphere� is rendered with color gradients only. Initially there was an idea to compensate so called Mach Bands effect. To do so I added a gradient profile functor. Then the concept was extended to set a color profile. As a result you can render simple geometrical objects in 2D looking like 3D ones. In this example you can construct your own color profile and select the gradient function. There're not so many gradient functions in AGG, but you can easily add your own. Also, drag the �gradient� with the left mouse button, scale and rotate it with the right one.")]
    public class GradientDemo : DemoBase
    {
        Stopwatch stopwatch = new Stopwatch();
        public GradientDemo()
        {
        }

        public override void Draw(PixelFarm.Drawing.Painter p)
        {
            if (p is AggPainter)
            {
                //solid fill
                var p2 = (AggPainter)p;
                p.RenderQuality = RenderQualtity.Fast;
                p.FillColor = Drawing.Color.Red;
                p.FillRect(0, 70, 150, 120);


                p.FillColor = Color.FromArgb(80, Drawing.Color.Red);
                p.FillRect(180, 70, 150, 120);
                //-------------


                //-------------

                //var gradientBrush = new LinearGradientBrush(
                //    new PointF(0, 0),
                //    Drawing.Color.Red,
                //    new PointF(150, 0),
                //    Drawing.Color.Yellow);

                //var prevBrush = p.CurrentBrush;
                //p.CurrentBrush = gradientBrush;
                //p2.FillRect(0, 0, 150, 50);
                //p.CurrentBrush = prevBrush;



                ////------------------------------------------- 
                ////original agg's gradient fill
                //var innerGradient = new Gradients.GvcRadial();
                //SpanInterpolatorLinear linerInterpolator = new SpanInterpolatorLinear(Affine.IdentityMatrix);
                //LinearGradientColorsProvider linearColorProvider = new LinearGradientColorsProvider(Drawing.Color.Red, Drawing.Color.Yellow);
                //SpanGenGradient spanGenGradient = new SpanGenGradient(linerInterpolator,
                //    innerGradient,
                //    linearColorProvider,
                //    0, 150);
                //SimpleRect srect = new SimpleRect(0, 0, 150, 50);

                //PixelFarm.Drawing.VectorToolBox.GetFreeVxs(out var v1);
                //p2.Fill(srect.MakeVxs(v1), spanGenGradient);
                //PixelFarm.Drawing.VectorToolBox.ReleaseVxs(ref v1);
            }
        }

        public override void MouseDown(int mx, int my, bool isRightButton)
        {
        }
        public override void MouseDrag(int mx, int my)
        {
        }
        public override void MouseUp(int x, int y)
        {
        }
    }
}





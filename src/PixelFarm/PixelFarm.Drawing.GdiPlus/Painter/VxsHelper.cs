﻿//MIT, 2016-present, WinterDev

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using PixelFarm.CpuBlit;
namespace PixelFarm.Drawing.WinGdi
{
    public static class VxsHelper
    {
        static System.Drawing.Pen _pen = new System.Drawing.Pen(System.Drawing.Color.Black);
        static System.Drawing.SolidBrush _br = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
        public static System.Drawing.Drawing2D.GraphicsPath CreateGraphicsPath(VertexStore vxs)
        {
            //render vertice in store
            int vcount = vxs.Count;
            double prevX = 0;
            double prevY = 0;
            double prevMoveToX = 0;
            double prevMoveToY = 0;
            var brush_path = new System.Drawing.Drawing2D.GraphicsPath(FillMode.Winding);//*** winding for overlapped path
            for (int i = 0; i < vcount; ++i)
            {
                double x, y;
                VertexCmd cmd = vxs.GetVertex(i, out x, out y);
                switch (cmd)
                {
                    case VertexCmd.MoveTo:
                        prevMoveToX = prevX = x;
                        prevMoveToY = prevY = y;
                        brush_path.StartFigure();
                        break;
                    case VertexCmd.LineTo:
                        brush_path.AddLine((float)prevX, (float)prevY, (float)x, (float)y);
                        prevX = x;
                        prevY = y;
                        break;
                    case VertexCmd.Close:
                    case VertexCmd.CloseAndEndFigure:
                        brush_path.AddLine((float)prevX, (float)prevY, (float)prevMoveToX, (float)prevMoveToY);
                        prevMoveToX = prevX = x;
                        prevMoveToY = prevY = y;
                        brush_path.CloseFigure();
                        break;

                    case VertexCmd.NoMore:
                        i = vcount + 1;//exit from loop
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
            return brush_path;
        }
        /// <summary>
        /// we do NOT store vxsSnap
        /// </summary>
        /// <param name="vxsSnap"></param>
        /// <returns></returns>
        public static System.Drawing.Drawing2D.GraphicsPath CreateGraphicsPath(VertexStoreSnap vxsSnap)
        {
            VertexSnapIter vxsIter = vxsSnap.GetVertexSnapIter();
            double prevX = 0;
            double prevY = 0;
            double prevMoveToX = 0;
            double prevMoveToY = 0;
            var brush_path = new System.Drawing.Drawing2D.GraphicsPath(FillMode.Winding);//*** winding for overlapped path  

            for (; ; )
            {
                double x, y;
                VertexCmd cmd = vxsIter.GetNextVertex(out x, out y);
                switch (cmd)
                {
                    case PixelFarm.CpuBlit.VertexCmd.MoveTo:
                        prevMoveToX = prevX = x;
                        prevMoveToY = prevY = y;
                        brush_path.StartFigure();
                        break;
                    case PixelFarm.CpuBlit.VertexCmd.LineTo:
                        brush_path.AddLine((float)prevX, (float)prevY, (float)x, (float)y);
                        prevX = x;
                        prevY = y;
                        break;
                    case PixelFarm.CpuBlit.VertexCmd.Close:
                    case VertexCmd.CloseAndEndFigure:
                        //from current point                         
                        brush_path.AddLine((float)prevX, (float)prevY, (float)prevMoveToX, (float)prevMoveToY);
                        prevX = prevMoveToX;
                        prevY = prevMoveToY;
                        brush_path.CloseFigure();
                        break;

                    case PixelFarm.CpuBlit.VertexCmd.NoMore:
                        goto EXIT_LOOP;
                    default:
                        throw new NotSupportedException();
                }
            }
            EXIT_LOOP:
            return brush_path;
        }

        public static void FillVxsSnap(Graphics g, VertexStoreSnap vxsSnap, Color c)
        {
            using (System.Drawing.Drawing2D.GraphicsPath p = CreateGraphicsPath(vxsSnap))
            {
                _br.Color = ToDrawingColor(c);
                g.FillPath(_br, p);
            }
        }
        public static void DrawVxsSnap(Graphics g, VertexStoreSnap vxsSnap, Color c)
        {
            using (System.Drawing.Drawing2D.GraphicsPath p = CreateGraphicsPath(vxsSnap))
            {
                _pen.Color = ToDrawingColor(c);
                g.DrawPath(_pen, p);
            }
        }
        public static void FillPath(Graphics g, System.Drawing.Drawing2D.GraphicsPath p, Color c)
        {
            _br.Color = ToDrawingColor(c);
            g.FillPath(_br, p);
        }
        public static void DrawPath(Graphics g, System.Drawing.Drawing2D.GraphicsPath p, Color c)
        {
            _pen.Color = ToDrawingColor(c);
            g.DrawPath(_pen, p);
        }
        public static System.Drawing.Color ToDrawingColor(Color c)
        {
            return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
        }
    }
}
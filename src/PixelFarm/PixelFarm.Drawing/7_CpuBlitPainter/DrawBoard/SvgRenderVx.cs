﻿//----------------------------------------------------------------------------
//MIT, 2014-present, WinterDev

using System;
using System.Collections.Generic;

using PixelFarm.Drawing;
using PixelFarm.CpuBlit.VertexProcessing;

namespace PixelFarm.CpuBlit
{

    public class VxsRenderVx : RenderVx
    {
        public VertexStore _vxs;
        public VxsRenderVx(VertexStore vxs)
        {
            _vxs = vxs;

        }
        object _resolvedObject;
        public static object GetResolvedObject(VxsRenderVx vxsRenerVx)
        {
            return vxsRenerVx._resolvedObject;
        }
        public static void SetResolvedObject(VxsRenderVx vxsRenerVx, object obj)
        {
            vxsRenerVx._resolvedObject = obj;
        }

    }

    static class SimpleRectClipEvaluator
    {
        enum RectSide
        {
            None,
            Vertical,
            Horizontal
        }

        static RectSide FindRectSide(float x0, float y0, float x1, float y1)
        {
            if (x0 == x1 && y0 != y1)
            {
                return RectSide.Vertical;
            }
            else if (y0 == y1 && x0 != x1)
            {
                return RectSide.Horizontal;
            }
            return RectSide.None;
        }

        /// <summary>
        /// check if this is a simple rect
        /// </summary>
        /// <param name="vxs"></param>
        /// <returns></returns>
        public static bool EvaluateRectClip(VertexStore vxs, out RectangleF clipRect)
        {
            float x0 = 0, y0 = 0;
            float x1 = 0, y1 = 0;
            float x2 = 0, y2 = 0;
            float x3 = 0, y3 = 0;
            float x4 = 0, y4 = 0;
            clipRect = new RectangleF();

            int sideCount = 0;

            int j = vxs.Count;
            for (int i = 0; i < j; ++i)
            {
                VertexCmd cmd = vxs.GetVertex(i, out double x, out double y);
                switch (cmd)
                {
                    default: return false;
                    case VertexCmd.NoMore:
                        if (i > 6) return false;
                        break;
                    case VertexCmd.Close:
                        if (i > 5)
                        {
                            return false;
                        }
                        break;
                    case VertexCmd.LineTo:
                        {
                            switch (i)
                            {
                                case 1:
                                    x1 = (float)x;
                                    y1 = (float)y;
                                    sideCount++;
                                    break;
                                case 2:
                                    x2 = (float)x;
                                    y2 = (float)y;
                                    sideCount++;
                                    break;
                                case 3:
                                    x3 = (float)x;
                                    y3 = (float)y;
                                    sideCount++;
                                    break;
                                case 4:
                                    x4 = (float)x;
                                    y4 = (float)y;
                                    sideCount++;
                                    break;
                            }
                        }
                        break;
                    case VertexCmd.MoveTo:
                        {
                            if (i == 0)
                            {
                                x0 = (float)x;
                                y0 = (float)y;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        break;
                }
            }

            if (sideCount == 4)
            {
                RectSide s0 = FindRectSide(x0, y0, x1, y1);
                if (s0 == RectSide.None) return false;
                //
                RectSide s1 = FindRectSide(x1, y1, x2, y2);
                if (s1 == RectSide.None || s0 == s1) return false;
                //
                RectSide s2 = FindRectSide(x2, y2, x3, y3);
                if (s2 == RectSide.None || s1 == s2) return false;
                //
                RectSide s3 = FindRectSide(x3, y3, x4, y4);
                if (s3 == RectSide.None || s2 == s3) return false;
                //
                if (x4 == x0 && y4 == y0)
                {

                    if (s0 == RectSide.Horizontal)
                    {
                        clipRect = new RectangleF(x0, y0, x1 - x0, y3 - y0);
                    }
                    else
                    {
                        clipRect = new RectangleF(x0, y0, x3 - x0, y3 - y0);
                    }

                    return true;

                }
            }
            return false;

        }
    }



    public static class TempStrokeTool
    {

        [System.ThreadStatic]
        static Stack<Stroke> s_tempStrokes = new Stack<Stroke>();
        public static void GetFreeStroke(out Stroke tmpStroke)
        {
            if (s_tempStrokes.Count > 0)
            {
                tmpStroke = s_tempStrokes.Pop();
            }
            else
            {
                tmpStroke = new Stroke(1);
            }
        }
        public static void ReleaseStroke(ref Stroke s)
        {
            s.Width = 1;//reset
            s_tempStrokes.Push(s);
            s = null;
        }
    }





}
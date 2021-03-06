﻿//MIT, 2014-2016,WinterDev

using System;
using System.Collections.Generic;

using Mini;

using PixelFarm.Drawing;
using PixelFarm.DrawingGL;
using PixelFarm.CpuBlit;
namespace OpenTkEssTest
{
    [Info(OrderCode = "109")]
    [Info("T109_LionFillWithRenderVx")]
    public class T109_LionFillWithRenderVx : DemoBase
    {
        GLRenderSurface _glsx;
        SpriteShape lionShape;
        VertexStore lionVxs;
        GLPainter painter;
        List<RenderVx> lionRenderVxList = new List<RenderVx>();
        int tmpDrawVersion = 0;
        MultiPartTessResult multipartTessResult;

        protected override void OnGLSurfaceReady(GLRenderSurface glsx, GLPainter painter)
        {
            this._glsx = glsx;
            this.painter = painter;
        }
        protected override void OnReadyForInitGLShaderProgram()
        {
            //int max = Math.Max(this.Width, this.Height);

            //lionShape = new SpriteShape();
            //lionShape.ParseLion();
            ////flip this lion vertically before use with openGL
            //PixelFarm.Agg.Transform.Affine aff = PixelFarm.Agg.Transform.Affine.NewMatix(
            //     PixelFarm.Agg.Transform.AffinePlan.Scale(1, -1),
            //     PixelFarm.Agg.Transform.AffinePlan.Translate(0, 600));

            //lionVxs = new VertexStore();
            //aff.TransformToVxs(lionShape.Vxs, lionVxs);
            ////convert lion vxs to renderVx

            //////-------------
            //////version 1:
            ////int j = lionShape.NumPaths;
            ////int[] pathList = lionShape.PathIndexList;
            ////for (int i = 0; i < j; ++i)
            ////{
            ////    lionRenderVxList.Add(painter.CreateRenderVx(new VertexStoreSnap(lionVxs, pathList[i])));
            ////}
            //////------------- 
            ////version 2:
            //{
            //    tmpDrawVersion = 2;
            //    MultiPartPolygon mutiPartPolygon = new MultiPartPolygon();
            //    int j = lionShape.NumPaths;
            //    int[] pathList = lionShape.PathIndexList;
            //    //Color[] colors = lionShape.Colors;
            //    for (int i = 0; i < j; ++i)
            //    {
            //        //from lionvxs extract each part                      
            //        //fetch data and add to multipart polygon
            //        //if (i != 4) continue;
            //        //if (i > 1)
            //        //{
            //        //    break;
            //        //}
            //        mutiPartPolygon.AddVertexSnap(new VertexStoreSnap(lionVxs, pathList[i]));
            //    }
            //    //then create single render vx
            //    this.multipartTessResult = painter.CreateMultiPartTessResult(mutiPartPolygon);
            //    //create render vx for the multipart test result                 
            //}

        }
        protected override void DemoClosing()
        {
            _glsx.Dispose();
        }
        protected override void OnGLRender(object sender, EventArgs args)
        {
            _glsx.SmoothMode = SmoothMode.Smooth;
            _glsx.StrokeColor = PixelFarm.Drawing.Color.Blue;
            _glsx.ClearColorBuffer();
            //-------------------------------
            if (tmpDrawVersion == 2)
            {
                //TODO: impl this again
                //2018-08-01
                //if (multipartTessResult != null)
                //{
                //    VgRenderVx renderVx = (VgRenderVx)lionShape.GetRenderVx();
                //    int partCount = renderVx.VgCmdCount;

                //    int partIndex = 0;
                //    Color fillColor = Color.Transparent;
                //    for (int i = 0; i < partCount; ++i)
                //    {
                //        VgCmd vx = renderVx.GetVgCmd(i);
                //        switch (vx.Name)
                //        {
                //            default:
                //                break;
                //            case VgCommandName.Path:
                //                {
                //                    _glsx.FillRenderVx(fillColor, multipartTessResult, partIndex);
                //                }
                //                break;
                //            case VgCommandName.FillColor:
                //                {
                //                    fillColor = ((VgCmdFillColor)vx).Color;
                //                }
                //                break;
                //        }
                //        partIndex++;
                //    }


            }
            else
            {
                //TODO: impl this again
                //2018-08-01
                //Color fillColor = Color.Transparent;
                //int j = lionRenderVxList.Count;
                //VgRenderVx renderVx = (VgRenderVx)lionShape.GetRenderVx();
                //int partCount = renderVx.VgCmdCount;
                //int partIndex = 0;
                //for (int i = 0; i < partCount; ++i)
                //{
                //    VgCmd vx = renderVx.GetVgCmd(i);

                //    switch (vx.Name)
                //    {
                //        default:
                //            break;
                //        case VgCommandName.Path:
                //            {
                //                _glsx.FillRenderVx(fillColor, multipartTessResult, partIndex);
                //            }
                //            break;
                //        case VgCommandName.FillColor:
                //            {
                //                fillColor = ((VgCmdFillColor)vx).Color;
                //            }
                //            break;
                //    }

                //    partIndex++;
                //} 
            }
            //-------------------------------
            SwapBuffers();
        }
    }
}


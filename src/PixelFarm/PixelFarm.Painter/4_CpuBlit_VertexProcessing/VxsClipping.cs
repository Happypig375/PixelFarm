﻿//MIT, 2016-present, WinterDev

using System.Collections.Generic;
using ClipperLib;
using PixelFarm.Drawing;
namespace PixelFarm.CpuBlit.VertexProcessing
{
    public enum VxsClipperType : byte
    {
        InterSect = ClipType.ctIntersection,
        Union = ClipType.ctUnion,
        Difference = ClipType.ctDifference,
        Xor = ClipType.ctXor,
    }

    public class VxsClipper
    {
        [System.ThreadStatic]
        static Stack<VxsClipper> s_vxsClipperTools;


        List<IntPolygon> aPolys = new List<IntPolygon>();
        List<IntPolygon> bPolys = new List<IntPolygon>();
        List<IntPolygon> intersectedPolys = new List<IntPolygon>();
        Clipper clipper = new Clipper();
        PathWriter outputPathWriter = new PathWriter();


        public static void CombinePaths(
            VertexStoreSnap a,
            VertexStoreSnap b,
            VxsClipperType vxsClipType,
            bool separateIntoSmallSubPaths,
            List<VertexStore> results)
        {

            VxsClipper clipper = GetFreeVxsClipper();
            clipper.CombinePathsInternal(a, b, vxsClipType, separateIntoSmallSubPaths, results);
            ReleaseVxsClipper(ref clipper);
        }


        static VxsClipper GetFreeVxsClipper()
        {

            if (s_vxsClipperTools == null)
            {
                s_vxsClipperTools = new Stack<VxsClipper>();
            }
            //
            if (s_vxsClipperTools.Count == 0)
            {
                return new VxsClipper();
            }
            else
            {
                return s_vxsClipperTools.Pop();
            }

        }
        static void ReleaseVxsClipper(ref VxsClipper clipper)
        {
            clipper.Reset();
            s_vxsClipperTools.Push(clipper);
            clipper = null;
        }


        private VxsClipper() { }
        void Reset()
        {
            aPolys.Clear();
            bPolys.Clear();
            intersectedPolys.Clear();
            clipper.Clear();

        }
        //
        void CombinePathsInternal(
           VertexStoreSnap a,
           VertexStoreSnap b,
           VxsClipperType vxsClipType,
           bool separateIntoSmallSubPaths,
           List<VertexStore> resultList)
        {

            //prepare instance
            //reset all used fields

            ClipType clipType = (ClipType)vxsClipType;
            CreatePolygons(a, aPolys);
            CreatePolygons(b, bPolys);

            clipper.AddPaths(aPolys, PolyType.ptSubject, true);
            clipper.AddPaths(bPolys, PolyType.ptClip, true);
            clipper.Execute(clipType, intersectedPolys);

            if (separateIntoSmallSubPaths)
            {
                foreach (List<IntPoint> polygon in intersectedPolys)
                {
                    int j = polygon.Count;
                    if (j > 0)
                    {
                        //first one
                        IntPoint point = polygon[0];
                        outputPathWriter.MoveTo(point.X / 1000.0, point.Y / 1000.0);
                        //next others ...
                        if (j > 1)
                        {
                            for (int i = 1; i < j; ++i)
                            {
                                point = polygon[i];
                                outputPathWriter.LineTo(point.X / 1000.0, point.Y / 1000.0);
                            }
                        }

                        outputPathWriter.CloseFigure();
                        resultList.Add(outputPathWriter.Vxs);
                        //---
                        //clear and set an new Vxs for next operation...
                        //TODO: review here again
                        outputPathWriter.ResetWithExternalVxs(new VertexStore());
                    }
                }
            }
            else
            {
                foreach (List<IntPoint> polygon in intersectedPolys)
                {
                    int j = polygon.Count;
                    if (j > 0)
                    {
                        //first one
                        IntPoint point = polygon[0];
                        outputPathWriter.MoveTo(point.X / 1000.0, point.Y / 1000.0);
                        //next others ...
                        if (j > 1)
                        {
                            for (int i = 1; i < j; ++i)
                            {
                                point = polygon[i];
                                outputPathWriter.LineTo(point.X / 1000.0, point.Y / 1000.0);
                            }
                        }
                        outputPathWriter.CloseFigure();
                    }
                }

                //TODO: review here
                outputPathWriter.Stop();
                resultList.Add(outputPathWriter.Vxs);
            }
        }


        static void CreatePolygons(VertexStoreSnap a, List<IntPolygon> allPolys)
        {

            IntPolygon currentPoly = null;
            VertexData last = new VertexData();
            VertexData first = new VertexData();
            bool addedFirst = false;
            var snapIter = a.GetVertexSnapIter();
            double x, y;
            VertexCmd cmd = snapIter.GetNextVertex(out x, out y);
            do
            {
                if (cmd == VertexCmd.LineTo)
                {
                    if (currentPoly == null)
                    {
                        currentPoly = new IntPolygon();
                        allPolys.Add(currentPoly);
                    }
                    //
                    if (!addedFirst)
                    {
                        currentPoly.Add(new IntPoint((long)(last.x * 1000), (long)(last.y * 1000)));
                        addedFirst = true;
                        first = last;
                    }
                    currentPoly.Add(new IntPoint((long)(x * 1000), (long)(y * 1000)));
                    last = new VertexData(cmd, x, y);
                }
                else
                {
                    addedFirst = false;
                    currentPoly = new IntPolygon();
                    allPolys.Add(currentPoly);
                    if (cmd == VertexCmd.MoveTo)
                    {
                        last = new VertexData(cmd, x, y);
                    }
                    else
                    {
                        last = first;
                    }
                }
                cmd = snapIter.GetNextVertex(out x, out y);
            } while (cmd != VertexCmd.NoMore);


        }
    }
}

﻿//BSD, 2014-2018, WinterDev

/*
 * Created by SharpDevelop.
 * User: lbrubaker
 * Date: 3/26/2010
 * Time: 4:37 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using NUnit.Framework;
using Tesselate;

namespace TessTest
{
    public struct Vertex
    {
        public double m_X;
        public double m_Y;
        public Vertex(double x, double y)
        {
            m_X = x;
            m_Y = y;
        }
#if DEBUG
        public override string ToString()
        {
            return this.m_X + "," + this.m_Y;
        }
#endif

    }

    public class TessListener
    {

        List<Vertex> inputVertextList;
        List<Vertex> tempVertextList = new List<Vertex>();
        public List<Vertex> resultVertexList = new List<Vertex>();
        public TessListener()
        {
            //empty not use
            //not use first item in temp
            tempVertextList.Add(new Vertex(0, 0));
        }
        public void BeginCallBack(Tesselator.TriangleListType type)
        {

            Console.WriteLine("begin: " + type.ToString());
            //Assert.IsTrue(GetNextOutputAsString() == "B");
            //switch (type)
            //{
            //    case Tesselator.TriangleListType.Triangles:
            //        Assert.IsTrue(GetNextOutputAsString() == "TRI");
            //        break;

            //    case Tesselator.TriangleListType.TriangleFan:
            //        Assert.IsTrue(GetNextOutputAsString() == "FAN");
            //        break;

            //    case Tesselator.TriangleListType.TriangleStrip:
            //        Assert.IsTrue(GetNextOutputAsString() == "STRIP");
            //        break;

            //    default:
            //        throw new Exception("unknown TriangleListType '" + type.ToString() + "'.");
            //}
        }

        public void EndCallBack()
        {
            //Assert.IsTrue(GetNextOutputAsString() == "E");
            Console.WriteLine("end");
        }

        public void VertexCallBack(int index)
        {
            //Assert.IsTrue(GetNextOutputAsString() == "V");
            //Assert.AreEqual(GetNextOutputAsInt(), index); 
            if (index < 0)
            {
                //use data from temp store
                resultVertexList.Add(this.tempVertextList[-index]);
                Console.WriteLine("temp_v_cb:" + index + ":(" + tempVertextList[-index] + ")");
            }
            else
            {
                resultVertexList.Add(this.inputVertextList[index]);
                Console.WriteLine("v_cb:" + index + ":(" + inputVertextList[index] + ")");
            }


        }

        public void EdgeFlagCallBack(bool IsEdge)
        {
            Console.WriteLine("edge: " + IsEdge);
            //Assert.IsTrue(GetNextOutputAsString() == "F");
            //Assert.AreEqual(GetNextOutputAsBool(), IsEdge);
        }
        //public delegate void CallCombineDelegate(
        // double c1, double c2, double c3, ref CombineParameters combinePars, out int outData);
        public void CombineCallBack(double c1, double c2, double c3, ref Tesselator.CombineParameters combinePars, out int outData)
        {
            //double error = .001;
            //Assert.IsTrue(GetNextOutputAsString() == "C");
            //Assert.AreEqual(GetNextOutputAsDouble(), v0, error);
            //Assert.AreEqual(GetNextOutputAsDouble(), v1, error);
            //Assert.AreEqual(GetNextOutputAsInt(), data4[0]);
            //Assert.AreEqual(GetNextOutputAsInt(), data4[1]);
            //Assert.AreEqual(GetNextOutputAsInt(), data4[2]);
            //Assert.AreEqual(GetNextOutputAsInt(), data4[3]);
            //Assert.AreEqual(GetNextOutputAsDouble(), weight4[0], error);
            //Assert.AreEqual(GetNextOutputAsDouble(), weight4[1], error);
            //Assert.AreEqual(GetNextOutputAsDouble(), weight4[2], error);
            //Assert.AreEqual(GetNextOutputAsDouble(), weight4[3], error); 
            //here , outData = index of newly add vertext


            //----------------------------------------------------------------------
            //*** new vertext is added into user vertext list ***            
            //use negative to note that this vertext is from temporary source 

            //other implementation:
            // append to end of input list is ok if the input list can grow up ***
            //----------------------------------------------------------------------

            outData = -this.tempVertextList.Count;
            //----------------------------------------
            tempVertextList.Add(new Vertex(c1, c2));
            //----------------------------------------

        }
        public void Connect(List<Vertex> vertextList, Tesselate.Tesselator tesselator, Tesselator.WindingRuleType windingRule, bool setEdgeFlag)
        {
            this.inputVertextList = vertextList;

            tesselator.callBegin += new Tesselate.Tesselator.CallBeginDelegate(BeginCallBack);
            tesselator.callEnd += new Tesselate.Tesselator.CallEndDelegate(EndCallBack);
            tesselator.callVertex += new Tesselate.Tesselator.CallVertexDelegate(VertexCallBack);
            tesselator.callCombine += new Tesselate.Tesselator.CallCombineDelegate(CombineCallBack);
            tesselator.windingRule = windingRule;

            if (setEdgeFlag)
            {
                tesselator.callEdgeFlag += new Tesselate.Tesselator.CallEdgeFlagDelegate(EdgeFlagCallBack);
            }
        }

    }
}
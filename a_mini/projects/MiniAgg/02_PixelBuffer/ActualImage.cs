﻿//2014 BSD,WinterDev   
//----------------------------------------------------------------------------
// Anti-Grain Geometry - Version 2.4
// Copyright (C) 2002-2005 Maxim Shemanarev (http://www.antigrain.com)
//
// C# Port port by: Lars Brubaker
//                  larsbrubaker@gmail.com
// Copyright (C) 2007
//
// Permission to copy, use, modify, sell and distribute this software 
// is granted provided this copyright notice appears in all copies. 
// This software is provided "as is" without express or implied
// warranty, and with no claim as to its suitability for any purpose.
//
//----------------------------------------------------------------------------
// Contact: mcseem@antigrain.com
//          mcseemagg@yahoo.com
//          http://www.antigrain.com
//----------------------------------------------------------------------------
using System;
using System.Runtime;

using PixelFarm.Agg;
using PixelFarm.Agg.VertexSource;
using PixelFarm.VectorMath;

namespace PixelFarm.Agg.Image
{
    public enum ImageFormat
    {
        Rgba32,
        Rgba24,
        GrayScale8,
    }
    public class ActualImage : ImageBase
    {
        public ActualImage(int width, int height,
              int bitsPerPixel, IPixelBlender recieveBlender)
            : base(width, height, bitsPerPixel, recieveBlender)
        {
        }
    }

}
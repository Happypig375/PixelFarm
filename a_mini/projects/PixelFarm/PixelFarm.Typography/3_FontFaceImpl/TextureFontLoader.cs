﻿//MIT, 2014-2017, WinterDev 

using System.IO;
using System.Collections.Generic;

using PixelFarm.Agg;
using PixelFarm.Drawing;
using Typography.OpenFont;
using Typography.Rendering;
using Typography.OpenFont.Tables;
using Typography.OpenFont.Extensions;

namespace PixelFarm.Drawing.Fonts
{



    public static class TextureFontLoader
    {
        public static FontFace LoadFont(string fontfile, ScriptLang scriptLang,
            WriteDirection writeDirection,
            out SimpleFontAtlas fontAtlas)
        {
            using (FileStream fs = new FileStream(fontfile, FileMode.Open, FileAccess.Read))
            {
                var reader = new OpenFontReader();
                Typeface typeface = reader.Read(fs);
                return LoadFont(typeface, fontfile, scriptLang, writeDirection, out fontAtlas);
            }
        }
        public static FontFace LoadFont(
            Typeface typeface,
            string fontfile,
            ScriptLang scriptLang,
            WriteDirection writeDirection,
            out SimpleFontAtlas fontAtlas)
        {

            //1. read font info
            NOpenFontFace openFont = (NOpenFontFace)OpenFontLoader.LoadFont(typeface, scriptLang, writeDirection);

            //2. build texture font on the fly! OR load from prebuilt file
            //
            //2.1 test build texture on the fly


            //SimpleFontAtlasBuilder atlas1 = CreateSampleMsdfTextureFont(
            //    typeface, 16, GetGlyphIndexIter(typeface,
            //    UnicodeLangBits.BasicLatin,     //0-127 
            //    UnicodeLangBits.Thai //eg. Thai, for test with complex script, you can change to your own
            //    ));
            //SimpleFontAtlasBuilder atlas1 = CreateAggTextureFont(
            //   typeface, 16, GetGlyphIndexIter(typeface,
            //   UnicodeLangBits.BasicLatin,     //0-127 
            //   UnicodeLangBits.Thai //eg. Thai, for test with complex script, you can change to your own
            //   ));
            //SimpleFontAtlasBuilder atlas1 = CreateAggSubPixelRenderingTextureFont(
            //   typeface, 16, GetGlyphIndexIter(typeface,
            //   UnicodeLangBits.BasicLatin,     //0-127 
            //   UnicodeLangBits.Thai //eg. Thai, for test with complex script, you can change to your own
            //   ));
            SimpleFontAtlasBuilder atlas1 = CreateAggTextureFont(
              typeface, 16, GetGlyphIndexIter(typeface,
              UnicodeLangBits.BasicLatin,     //0-127 
              UnicodeLangBits.Thai //eg. Thai, for test with complex script, you can change to your own
              ));

            GlyphImage glyphImg2 = atlas1.BuildSingleImage();
            fontAtlas = atlas1.CreateSimpleFontAtlas();
            fontAtlas.TotalGlyph = glyphImg2;

            //string xmlFontFileInfo = "";
            //GlyphImage glyphImg = null; 
            //MySimpleFontAtlasBuilder atlasBuilder = new MySimpleFontAtlasBuilder();
            //SimpleFontAtlas fontAtlas = atlasBuilder.LoadFontInfo(xmlFontFileInfo);
            //glyphImg = atlasBuilder.BuildSingleImage(); //we can create a new glyph or load from prebuilt file
            //fontAtlas.TotalGlyph = glyphImg; 

            var textureFontFace = new TextureFontFace(openFont, fontAtlas);
            return textureFontFace;
        }
        static IEnumerable<ushort> GetGlyphIndexIter(Typeface typeface, params UnicodeLangBits[] rangeBits)
        {
            //temp fixed
            GlyphIndexCollector collector = new GlyphIndexCollector();
            int j = rangeBits.Length;
            for (int i = 0; i < j; ++i)
            {
                UnicodeRangeInfo rangeInfo = rangeBits[i].ToUnicodeRangeInfo();
                //get start and end bit
                int startChar = rangeInfo.StartAt;
                int startGlyphIndex = typeface.LookupIndex((char)startChar);
                while (startGlyphIndex < 1)
                {
                    startChar++;
                    startGlyphIndex = typeface.LookupIndex((char)startChar);
                }
                for (int gindex = startGlyphIndex; gindex < startGlyphIndex + 125; ++gindex)
                {
                    yield return (ushort)gindex;
                }
                //char endAt = (char)rangeInfo.EndAt;
                //for (char c = (char)rangeInfo.StartAt; c <= endAt; ++c)
                //{
                //    typeface.CollectGlyphIndexListFromSampleChar(c, collector);
                //}
            }
        }
        static IEnumerable<ushort> GetGlyphIndexIterFromSampleChar(Typeface typeface, params char[] sampleChars)
        {
            //GlyphIndexCollector collector = new GlyphIndexCollector();
            //int j = sampleChars.Length;
            //for (int i = 0; i < j; ++i)
            //{
            //    typeface.CollectGlyphIndexListFromSampleChar(sampleChars[i], collector);
            //}
            //return collector.GetGlyphIndexIter();
            int j = sampleChars.Length;
            for (int i = 0; i < j; ++i)
            {
                yield return (ushort)typeface.LookupIndex(sampleChars[i]);
            }
        }
        static IEnumerable<ushort> GetGlyphIndexIter(Typeface typeface, params char[] chars)
        {
            int j = chars.Length;
            for (int i = 0; i < j; ++i)
            {
                char c = chars[i];
                yield return (ushort)typeface.LookupIndex(c);
            }
        }

        static SimpleFontAtlasBuilder CreateSampleMsdfTextureFont(
            Typeface typeface, float sizeInPoint, IEnumerable<ushort> glyphIndexIter)
        {

            ////read type face from file
            //Typeface typeface;
            //using (var fs = new FileStream(fontfile, FileMode.Open, FileAccess.Read))
            //{
            //    var reader = new OpenFontReader();
            //    //1. read typeface from font file
            //    typeface = reader.Read(fs);
            //}
            //sample: create sample msdf texture 
            //-------------------------------------------------------------
            var builder = new GlyphPathBuilder(typeface);
            //builder.UseTrueTypeInterpreter = this.chkTrueTypeHint.Checked;
            //builder.UseVerticalHinting = this.chkVerticalHinting.Checked;
            //-------------------------------------------------------------
            var atlasBuilder = new SimpleFontAtlasBuilder();
            var msdfGlyphGen = new MsdfGlyphGen();
            atlasBuilder.TextureKind = TextureKind.Msdf;
            foreach (ushort gindex in glyphIndexIter)
            {
                //build glyph 
                builder.BuildFromGlyphIndex(gindex, sizeInPoint);
                GlyphImage glyphImage = msdfGlyphGen.CreateMsdfImage(
                    builder.GetOutputPoints(),
                    builder.GetOutputContours(),
                    builder.GetPixelScale());
                atlasBuilder.AddGlyph(gindex, glyphImage);

                //int[] buffer = glyphImage.GetImageBuffer();
                //using (var bmp = new System.Drawing.Bitmap(glyphImage.Width, glyphImage.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                //{
                //    var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, glyphImage.Width, glyphImage.Height),
                //        System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                //    System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
                //    bmp.UnlockBits(bmpdata);
                //    bmp.Save("d:\\WImageTest\\a001_xn2_" + c + ".png");
                //}
            }





            //var glyphImg2 = atlasBuilder.BuildSingleImage();
            //using (var bmp = new System.Drawing.Bitmap(glyphImg2.Width, glyphImg2.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            //{
            //    var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, glyphImg2.Width, glyphImg2.Height),
            //        System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            //    int[] intBuffer = glyphImg2.GetImageBuffer();

            //    System.Runtime.InteropServices.Marshal.Copy(intBuffer, 0, bmpdata.Scan0, intBuffer.Length);
            //    bmp.UnlockBits(bmpdata);
            //    bmp.Save("d:\\WImageTest\\a_total.png");
            //}
            //atlasBuilder.SaveFontInfo("d:\\WImageTest\\a_info.xml");

            return atlasBuilder;
        }



        static SimpleFontAtlasBuilder CreateAggTextureFont(
            Typeface typeface, float sizeInPoint, IEnumerable<ushort> glyphIndexIter)
        {

            ////read type face from file
            //Typeface typeface;
            //using (var fs = new FileStream(fontfile, FileMode.Open, FileAccess.Read))
            //{
            //    var reader = new OpenFontReader();
            //    //1. read typeface from font file
            //    typeface = reader.Read(fs);
            //}
            //sample: create sample msdf texture 
            //-------------------------------------------------------------
            var builder = new GlyphPathBuilder(typeface);
            //builder.UseTrueTypeInterpreter = this.chkTrueTypeHint.Checked;
            //builder.UseVerticalHinting = this.chkVerticalHinting.Checked;
            //-------------------------------------------------------------
            var atlasBuilder = new SimpleFontAtlasBuilder();
            atlasBuilder.TextureKind = TextureKind.AggGrayScale;
            VertexStorePool vxsPool = new VertexStorePool();
            //create agg cavnas


            foreach (ushort gindex in glyphIndexIter)
            {
                //build glyph

                builder.BuildFromGlyphIndex(gindex, sizeInPoint);
                float pxScale = builder.GetPixelScale();

                var txToVxs = new GlyphTranslatorToVxs();
                builder.ReadShapes(txToVxs);
                //
                //create new one
                var glyphVxs = new VertexStore();
                txToVxs.WriteOutput(glyphVxs, vxsPool, pxScale);
                //find bound


                RectD bounds = new Agg.RectD();
                BoundingRect.GetBoundingRect(new VertexStoreSnap(glyphVxs), ref bounds);

                //-------------------------------------------- 
                int w = (int)System.Math.Ceiling(bounds.Width);
                int h = (int)System.Math.Ceiling(bounds.Height);
                if (w < 5)
                {
                    w = 5;
                }
                if (h < 5)
                {
                    h = 5;
                }
                //translate to positive quadrant
                double dx = (bounds.Left < 0) ? -bounds.Left : 0;
                double dy = (bounds.Bottom < 0) ? -bounds.Bottom : 0;

                if (dx != 0 || dy != 0)
                {
                    Agg.Transform.Affine transformMat = Agg.Transform.Affine.NewTranslation(dx, dy);
                    VertexStore vxs2 = new VertexStore();
                    Agg.Transform.Affine.TranslateToVxs(glyphVxs, dx, dy, vxs2);
                    glyphVxs = vxs2;
                }
                //-------------------------------------------- 
                //create glyph img 
                ActualImage img = new Agg.ActualImage(w, h, PixelFormat.ARGB32);
                ImageGraphics2D imgCanvas2d = new Agg.ImageGraphics2D(img);
                AggCanvasPainter painter = new Agg.AggCanvasPainter(imgCanvas2d);
                painter.FillColor = Color.Black;
                painter.StrokeColor = Color.Black;
                painter.Fill(glyphVxs);
                //-------------------------------------------- 
                var glyphImage = new GlyphImage(w, h);
                glyphImage.TextureOffsetX = dx;
                glyphImage.TextureOffsetY = dy;
                glyphImage.SetImageBuffer(ActualImage.GetBuffer2(img), false);
                //copy data from agg canvas to glyph image
                atlasBuilder.AddGlyph(gindex, glyphImage);

                //int[] buffer = glyphImage.GetImageBuffer();
                //using (var bmp = new System.Drawing.Bitmap(glyphImage.Width, glyphImage.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                //{
                //    var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, glyphImage.Width, glyphImage.Height),
                //        System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                //    System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
                //    bmp.UnlockBits(bmpdata);
                //    bmp.Save("d:\\WImageTest\\a001_xn2_" + gindex + ".png");
                //}
            }





            //var glyphImg2 = atlasBuilder.BuildSingleImage();
            //using (var bmp = new System.Drawing.Bitmap(glyphImg2.Width, glyphImg2.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            //{
            //    var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, glyphImg2.Width, glyphImg2.Height),
            //        System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            //    int[] intBuffer = glyphImg2.GetImageBuffer();

            //    System.Runtime.InteropServices.Marshal.Copy(intBuffer, 0, bmpdata.Scan0, intBuffer.Length);
            //    bmp.UnlockBits(bmpdata);
            //    bmp.Save("d:\\WImageTest\\a_total.png");
            //}
            //atlasBuilder.SaveFontInfo("d:\\WImageTest\\a_info.xml");

            return atlasBuilder;
        }


        static SimpleFontAtlasBuilder CreateAggSubPixelRenderingTextureFont(
            Typeface typeface, float sizeInPoint, IEnumerable<ushort> glyphIndexIter)
        {

            ////read type face from file
            //Typeface typeface;
            //using (var fs = new FileStream(fontfile, FileMode.Open, FileAccess.Read))
            //{
            //    var reader = new OpenFontReader();
            //    //1. read typeface from font file
            //    typeface = reader.Read(fs);
            //}
            //sample: create sample msdf texture 
            //-------------------------------------------------------------
            var builder = new GlyphPathBuilder(typeface);
            //builder.UseTrueTypeInterpreter = this.chkTrueTypeHint.Checked;
            //builder.UseVerticalHinting = this.chkVerticalHinting.Checked;
            //-------------------------------------------------------------
            var atlasBuilder = new SimpleFontAtlasBuilder();
            atlasBuilder.TextureKind = TextureKind.AggGrayScale;
            VertexStorePool vxsPool = new VertexStorePool();
            //create agg cavnas

            foreach (ushort gindex in glyphIndexIter)
            {
                //build glyph

                builder.BuildFromGlyphIndex(gindex, sizeInPoint);
                float pxScale = builder.GetPixelScale();

                var txToVxs = new GlyphTranslatorToVxs();
                builder.ReadShapes(txToVxs);
                //
                //create new one
                var glyphVxs = new VertexStore();
                txToVxs.WriteOutput(glyphVxs, vxsPool, pxScale);
                //find bound

                //-------------------------------------------- 
                //GlyphImage glyphImg = new GlyphImage()
                RectD bounds = new Agg.RectD();
                BoundingRect.GetBoundingRect(new VertexStoreSnap(glyphVxs), ref bounds);

                //-------------------------------------------- 
                int w = (int)System.Math.Ceiling(bounds.Width);
                int h = (int)System.Math.Ceiling(bounds.Height);
                if (w < 5)
                {
                    w = 5;
                }
                if (h < 5)
                {
                    h = 5;
                }
                //translate to positive quadrant 
                double dx = (bounds.Left < 0) ? -bounds.Left : 0;
                double dy = (bounds.Bottom < 0) ? -bounds.Bottom : 0;
                w = w * 4;

                if (dx != 0 || dy != 0)
                {
                    Agg.Transform.Affine transformMat = Agg.Transform.Affine.NewTranslation(dx, dy);
                    VertexStore vxs2 = new VertexStore();
                    Agg.Transform.Affine.TranslateToVxs(glyphVxs, dx, dy, vxs2);
                    glyphVxs = vxs2;
                }
                //-------------------------------------------- 
                //create glyph img 
                ActualImage img = new Agg.ActualImage(w, h, PixelFormat.ARGB32);
                ImageGraphics2D imgCanvas2d = new Agg.ImageGraphics2D(img);
                AggCanvasPainter painter = new Agg.AggCanvasPainter(imgCanvas2d);
                painter.FillColor = Color.Black;
                painter.StrokeColor = Color.Black;
                painter.UseSubPixelRendering = true;
                painter.Clear(Color.White);
                //
                painter.Fill(glyphVxs);
                //-------------------------------------------- 
                var glyphImage = new GlyphImage(w, h);
                glyphImage.TextureOffsetX = dx;
                glyphImage.TextureOffsetY = dy;
                glyphImage.SetImageBuffer(ActualImage.GetBuffer2(img), false);
                //copy data from agg canvas to glyph image
                atlasBuilder.AddGlyph(gindex, glyphImage);

                //int[] buffer = glyphImage.GetImageBuffer();
                //using (var bmp = new System.Drawing.Bitmap(glyphImage.Width, glyphImage.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                //{
                //    var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, glyphImage.Width, glyphImage.Height),
                //        System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
                //    System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
                //    bmp.UnlockBits(bmpdata);
                //    bmp.Save("d:\\WImageTest\\a001_subpix_xn2_" + gindex + ".png");
                //}
            }
            //var glyphImg2 = atlasBuilder.BuildSingleImage();
            //using (var bmp = new System.Drawing.Bitmap(glyphImg2.Width, glyphImg2.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            //{
            //    var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, glyphImg2.Width, glyphImg2.Height),
            //        System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            //    int[] intBuffer = glyphImg2.GetImageBuffer();

            //    System.Runtime.InteropServices.Marshal.Copy(intBuffer, 0, bmpdata.Scan0, intBuffer.Length);
            //    bmp.UnlockBits(bmpdata);
            //    bmp.Save("d:\\WImageTest\\a_total.png");
            //}
            //atlasBuilder.SaveFontInfo("d:\\WImageTest\\a_info.xml");

            return atlasBuilder;
        }
        //static SimpleFontAtlasBuilder CreateSampleMsdfTextureFont(string fontfile,
        //    float sizeInPoint, UnicodeRangeInfo[] ranges)
        //{

        //    //read type face from file
        //    Typeface typeface;
        //    using (var fs = new FileStream(fontfile, FileMode.Open, FileAccess.Read))
        //    {
        //        var reader = new OpenFontReader();
        //        //1. read typeface from font file
        //        typeface = reader.Read(fs);
        //    }
        //    //sample: create sample msdf texture 
        //    //-------------------------------------------------------------
        //    var builder = new GlyphPathBuilder(typeface);
        //    //builder.UseTrueTypeInterpreter = this.chkTrueTypeHint.Checked;
        //    //builder.UseVerticalHinting = this.chkVerticalHinting.Checked;
        //    //-------------------------------------------------------------
        //    var atlasBuilder = new SimpleFontAtlasBuilder();
        //    var msdfBuilder = new MsdfGlyphGen();

        //    int rangeCount = ranges.Length;
        //    for (int r = 0; r < rangeCount; ++r)
        //    {
        //        UnicodeRangeInfo rangeInfo = ranges[r];
        //        char startAtUnicode = (char)rangeInfo.StartAt;
        //        char endAtUnicode = (char)rangeInfo.EndAt;
        //        for (char c = startAtUnicode; c <= endAtUnicode; ++c)
        //        {
        //            //build glyph
        //            ushort glyphIndex = builder.Build(c, sizeInPoint);
        //            //builder.BuildFromGlyphIndex(n, sizeInPoint);

        //            var msdfGlyphGen = new MsdfGlyphGen();
        //            GlyphImage glyphImage = msdfGlyphGen.CreateMsdfImage(
        //                builder.GetOutputPoints(),
        //                builder.GetOutputContours(),
        //                builder.GetPixelScale());

        //            atlasBuilder.AddGlyph((int)glyphIndex, glyphImage);


        //            int[] buffer = glyphImage.GetImageBuffer();
        //            using (var bmp = new System.Drawing.Bitmap(glyphImage.Width, glyphImage.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
        //            {
        //                var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, glyphImage.Width, glyphImage.Height),
        //                    System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
        //                System.Runtime.InteropServices.Marshal.Copy(buffer, 0, bmpdata.Scan0, buffer.Length);
        //                bmp.UnlockBits(bmpdata);
        //                bmp.Save("d:\\WImageTest\\a001_xn2_" + c + ".png");
        //            }
        //        }
        //    }




        //    //var glyphImg2 = atlasBuilder.BuildSingleImage();
        //    //using (var bmp = new System.Drawing.Bitmap(glyphImg2.Width, glyphImg2.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
        //    //{
        //    //    var bmpdata = bmp.LockBits(new System.Drawing.Rectangle(0, 0, glyphImg2.Width, glyphImg2.Height),
        //    //        System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
        //    //    int[] intBuffer = glyphImg2.GetImageBuffer();

        //    //    System.Runtime.InteropServices.Marshal.Copy(intBuffer, 0, bmpdata.Scan0, intBuffer.Length);
        //    //    bmp.UnlockBits(bmpdata);
        //    //    bmp.Save("d:\\WImageTest\\a_total.png");
        //    //}
        //    //atlasBuilder.SaveFontInfo("d:\\WImageTest\\a_info.xml");

        //    return atlasBuilder;
        //}
    }


}
﻿//MIT, 2014-present, WinterDev

using System;
using PixelFarm.CpuBlit;
namespace PixelFarm.Drawing.Skia
{
    static class BitmapHelper
    {
        /// <summary>
        /// copy from actual image direct to hBmpScan0
        /// </summary>
        /// <param name="actualImage"></param>
        /// <param name="hBmpScan0"></param>
        public static void CopyToWindowsBitmapSameSize(
           ActualBitmap actualImage,
           IntPtr hBmpScan0)
        {
            //1st, fast
            CpuBlit.Imaging.TempMemPtr tmp = ActualBitmap.GetBufferPtr(actualImage);
            //System.Runtime.InteropServices.Marshal.Copy(rawBuffer, 0,
            //   hBmpScan0, rawBuffer.Length);
            unsafe
            {
                NativeMemMx.memcpy((byte*)hBmpScan0, (byte*)tmp.Ptr, tmp.LengthInBytes);
            }

        }

        /////////////////////////////////////////////////////////////////////////////////////

        public static void CopyToGdiPlusBitmapSameSize(
            ActualBitmap actualImage,
            SkiaSharp.SKBitmap skBmp)
        {
            //agg store image buffer head-down
            //when copy to window bmp we here to flip 
            //style1: copy row by row *** (fastest)***
            {
                //System.GC.Collect();
                //System.Diagnostics.Stopwatch sss = new System.Diagnostics.Stopwatch();
                //sss.Start();
                //for (int i = 0; i < 1000; ++i)
                //{
                int h = skBmp.Height;
                int w = skBmp.Width;


                //BitmapData bitmapData1 = bitmap.LockBits(
                //          new Rectangle(0, 0,
                //              w,
                //              h),
                //              System.Drawing.Imaging.ImageLockMode.ReadWrite,
                //              bitmap.PixelFormat); 
                skBmp.LockPixels();
                IntPtr scan0 = skBmp.GetPixels();
                int stride = actualImage.Stride;
                //byte[] srcBuffer = ActualImage.GetBuffer(actualImage);
                unsafe
                {
                    CpuBlit.Imaging.TempMemPtr srcBufferPtr = ActualBitmap.GetBufferPtr(actualImage);
                    //fixed (byte* bufferH = &srcBuffer[0])
                    byte* bufferH = (byte*)srcBufferPtr.Ptr;
                    {
                        byte* target = (byte*)scan0;
                        int startRowAt = ((h - 1) * stride);
                        for (int y = h; y > 0; --y)
                        {
                            //byte* src = bufferH + ((y - 1) * stride);
                            //System.Runtime.InteropServices.Marshal.Copy(
                            //   srcBuffer,//src
                            //   startRowAt,
                            //   (IntPtr)target,
                            //   stride);
                            PixelFarm.CpuBlit.MemMx.memcpy(target, bufferH + startRowAt, stride);
                            startRowAt -= stride;
                            target += stride;
                        }
                    }
                    srcBufferPtr.Dispose();
                }
                skBmp.UnlockPixels();
                //}
                //sss.Stop();
                //long ms = sss.ElapsedMilliseconds;
            }
            //-----------------------------------
            //style2: copy all, then flip again
            //{
            //    System.GC.Collect();
            //    System.Diagnostics.Stopwatch sss = new System.Diagnostics.Stopwatch();
            //    sss.Start();
            //    for (int i = 0; i < 1000; ++i)
            //    {
            //        byte[] rawBuffer = ActualImage.GetBuffer(actualImage);
            //        var bmpdata = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
            //          System.Drawing.Imaging.ImageLockMode.ReadOnly,
            //         bitmap.PixelFormat);


            //        System.Runtime.InteropServices.Marshal.Copy(rawBuffer, 0,
            //            bmpdata.Scan0, rawBuffer.Length);

            //        bitmap.UnlockBits(bmpdata);
            //        bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            //    }

            //    sss.Stop();
            //    long ms = sss.ElapsedMilliseconds; 
            //}
            //-----------------------------------

            //-----------------------------------
            //style3: copy row by row + 
            //{
            //    System.GC.Collect();
            //    System.Diagnostics.Stopwatch sss = new System.Diagnostics.Stopwatch();
            //    sss.Start();
            //    for (int i = 0; i < 1000; ++i)
            //    {
            //        int h = bitmap.Height;
            //        int w = bitmap.Width;
            //        BitmapData bitmapData1 = bitmap.LockBits(
            //                  new Rectangle(0, 0,
            //                      w,
            //                      h),
            //                      System.Drawing.Imaging.ImageLockMode.ReadWrite,
            //                      bitmap.PixelFormat);
            //        IntPtr scan0 = bitmapData1.Scan0;
            //        int stride = bitmapData1.Stride;
            //        byte[] buffer = ActualImage.GetBuffer(actualImage);
            //        unsafe
            //        {
            //            fixed (byte* bufferH = &buffer[0])
            //            {
            //                byte* target = (byte*)scan0;
            //                for (int y = h; y > 0; --y)
            //                {
            //                    byte* src = bufferH + ((y - 1) * stride);
            //                    for (int n = stride - 1; n >= 0; --n)
            //                    {
            //                        *target = *src;
            //                        target++;
            //                        src++;
            //                    }
            //                }
            //            }
            //        }
            //        bitmap.UnlockBits(bitmapData1);
            //    }
            //    sss.Stop();
            //    long ms = sss.ElapsedMilliseconds;
            //} 
        }

        //public static void CopyFromGdiPlusBitmapSameSize(
        //   PixelFarm.Drawing.Skia.MySkBmp windowsBitmap,
        //   ActualImage actualImage)
        //{
        //    int h = windowsBitmap.Height;
        //    int w = windowsBitmap.Width;
        //    byte[] targetBuffer = ActualImage.GetBuffer(actualImage);
        //    SkiaSharp.SKBitmap innerBmp = windowsBitmap.internalBmp;

        //    innerBmp.LockPixels();
        //    IntPtr scan0 = innerBmp.GetPixels();
        //    int stride = actualImage.Stride;

        //    //TODO: review here 
        //    //use buffer copy

        //    unsafe
        //    {
        //        //target 
        //        int startRowAt = ((h - 1) * stride);
        //        byte* src = (byte*)scan0;
        //        for (int y = h; y > 0; --y)
        //        {
        //            // byte* target = targetH + ((y - 1) * stride);

        //            System.Runtime.InteropServices.Marshal.Copy(
        //                  (IntPtr)src,//src
        //                  targetBuffer, startRowAt, stride);
        //            startRowAt -= stride;
        //            src += stride;
        //        }

        //        //////////////////////////////////////////////////////////////////
        //        //fixed (byte* targetH = &targetBuffer[0])
        //        //{
        //        //    byte* src = (byte*)scan0;
        //        //    for (int y = h; y > 0; --y)
        //        //    {
        //        //        byte* target = targetH + ((y - 1) * stride);
        //        //        for (int n = stride - 1; n >= 0; --n)
        //        //        {
        //        //            *target = *src;
        //        //            target++;
        //        //            src++;
        //        //        }
        //        //    }
        //        //}
        //    }
        //    innerBmp.UnlockPixels();
        //}


        //public static void CopyToWindowsBitmap(ActualImage backingImageBufferByte,
        //   Bitmap windowsBitmap,
        //   RectInt rect)
        //{
        //    int offset = 0;
        //    byte[] buffer = ActualImage.GetBuffer(backingImageBufferByte);
        //    BitmapHelper.CopyToWindowsBitmap(buffer, offset,
        //        backingImageBufferByte.Stride, backingImageBufferByte.Height,
        //        backingImageBufferByte.BitDepth,
        //        windowsBitmap, rect);
        //}
        //public static void CopyToWindowsBitmap(byte[] buffer, int offset,
        //  int sBackBufferStrideInBytes, int sHeight,
        //  int bitDepth,
        //  Bitmap windowsBitmap,
        //  RectInt rect)
        //{
        //    BitmapData bitmapData1 = windowsBitmap.LockBits(
        //              new Rectangle(0, 0,
        //                  windowsBitmap.Width,
        //                  windowsBitmap.Height),
        //                  ImageLockMode.ReadWrite,
        //                  windowsBitmap.PixelFormat);
        //    int backBufferStrideInInts = sBackBufferStrideInBytes / 4;
        //    int backBufferHeight = sHeight;
        //    int backBufferHeightMinusOne = backBufferHeight - 1;
        //    int bitmapDataStride = bitmapData1.Stride;
        //    switch (bitDepth)
        //    {
        //        case 24:
        //            {
        //                throw new NotSupportedException();
        //                //unsafe
        //                //{
        //                //    byte* bitmapDataScan0 = (byte*)bitmapData1.Scan0;
        //                //    fixed (byte* pSourceFixed = &buffer[offset])
        //                //    {
        //                //        byte* pSource = pSourceFixed;
        //                //        byte* pDestBuffer = bitmapDataScan0 + bitmapDataStride * backBufferHeightMinusOne;
        //                //        for (int y = 0; y < backBufferHeight; y++)
        //                //        {
        //                //            int* pSourceInt = (int*)pSource;
        //                //            int* pDestBufferInt = (int*)pDestBuffer;
        //                //            for (int x = 0; x < backBufferStrideInInts; x++)
        //                //            {
        //                //                pDestBufferInt[x] = pSourceInt[x];
        //                //            }
        //                //            for (int x = backBufferStrideInInts * 4; x < sBackBufferStrideInBytes; x++)
        //                //            {
        //                //                pDestBuffer[x] = pSource[x];
        //                //            }
        //                //            pDestBuffer -= bitmapDataStride;
        //                //            pSource += sBackBufferStrideInBytes;
        //                //        }
        //                //    }
        //                //}
        //            }
        //            break;
        //        case 32:
        //            {
        //                unsafe
        //                {
        //                    byte* bitmapDataScan0 = (byte*)bitmapData1.Scan0;
        //                    fixed (byte* pSourceFixed = &buffer[offset])
        //                    {
        //                        byte* pSource = pSourceFixed;
        //                        byte* pDestBuffer = bitmapDataScan0 + bitmapDataStride * backBufferHeightMinusOne;
        //                        int rect_bottom = rect.Bottom;
        //                        int rect_top = rect.Top;
        //                        int rect_left = rect.Left;
        //                        int rect_right = rect.Right;
        //                        for (int y = rect_bottom; y < rect_top; y++)
        //                        {
        //                            int* pSourceInt = (int*)pSource;
        //                            pSourceInt += (sBackBufferStrideInBytes * y / 4);
        //                            int* pDestBufferInt = (int*)pDestBuffer;
        //                            pDestBufferInt -= (bitmapDataStride * y / 4);
        //                            for (int x = rect_left; x < rect_right; x++)
        //                            {
        //                                pDestBufferInt[x] = pSourceInt[x];
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            break;
        //        default:
        //            throw new NotImplementedException();
        //    }

        //    windowsBitmap.UnlockBits(bitmapData1);
        //}
    }
}

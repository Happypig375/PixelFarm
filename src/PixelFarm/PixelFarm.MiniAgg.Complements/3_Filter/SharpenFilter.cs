﻿using System;
namespace PixelFarm.Agg.Imaging
{
    public class SharpenFilterARGB
    {

        ///<summary>
        /// Sharpen kernel with the size 3x3
        ///</summary>
        public static int[,] KernelSharpen3x3 = {
                                                 { 0, -2,  0},
                                                 {-2, 11, -2},
                                                 { 0, -2,  0}
                                              };
        public void Sharpen(IImageReaderWriter img, double radius)
        {

            byte[] buffer = img.GetBuffer();
            byte[] output = new byte[buffer.Length];
            //byte[] output2 = new byte[buffer.Length];
            unsafe
            {
                fixed (byte* outputPtr = &output[0])
                fixed (byte* srcBuffer = &buffer[0])
                {
                    int* srcBuffer1 = (int*)srcBuffer;
                    int* outputBuffer1 = (int*)outputPtr;

                    Sharpen(srcBuffer1, img.Stride, img.Width, img.Height, outputBuffer1);
                }
                //ActualImage.SaveImgBufferToPngFile(output, img.Stride, img.Width + 1, img.Height + 1, "d:\\WImageTest\\test_1.png");
                img.ReplaceBuffer(output);
            }

        }
        unsafe static void Sharpen(int* srcBuffer, int stride, int width, int height, int* result)
        {
            //from https://stackoverflow.com/questions/903632/sharpen-on-a-bitmap-using-c-sharp

            int filterWidth = 3;
            int filterHeight = 3;

            // Create sharpening filter.
            //double[,] filter = new double[filterWidth, filterHeight];
            //filter[0, 0] = filter[0, 1] = filter[0, 2] =
            //filter[1, 0] = filter[1, 2] = filter[2, 0] =
            //filter[2, 1] = filter[2, 2] = -1; //periferal
            ////
            //filter[1, 1] = 9; //center 
            ////

            float[,] filter = {
                                                 { 0, 0,  0},
                                                 {-0.5f, 2f, -0.5f},
                                                 { 0, 0,  0}
                                              };
            //int[,] filter = {
            //                                     { -1, -1,  -1},
            //                                     {-1, 9, -1},
            //                                     { -1, -1,  -1}
            //                                  };
            //
            double factor = 1.0;
            double bias = 0.0;

            // Lock image bits for read/write.

            // Declare an array to hold the bytes of the bitmap.
            int bytes = stride * height;

            int rgbPos;
            // Fill the color array with the new sharpened color values.

            int width1 = stride / 4;
            for (int x = 0; x < width1; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    double red = 0.0, green = 0.0, blue = 0.0;

                    for (int filterX = 0; filterX < filterWidth; filterX++)
                    {
                        for (int filterY = 0; filterY < filterHeight; filterY++)
                        {
                            int imageX = (x - filterWidth / 2 + filterX + width1) % width1;
                            int imageY = (y - filterHeight / 2 + filterY + height) % height;

                            //rgb = imageY * wid + (4 * imageX); //rgb pos
                            rgbPos = imageY * width1 + (imageX); //rgb pos

                            int rgbValue = srcBuffer[rgbPos];

                            red += ((rgbValue >> 16) & 0xff) * filter[filterX, filterY];
                            green += ((rgbValue >> 8) & 0xff) * filter[filterX, filterY];
                            blue += (rgbValue & 0xff) * filter[filterX, filterY];
                        }
                        //to byte
                        int r = Math.Min(Math.Max((int)(factor * red + bias), 0), 255);
                        int g = Math.Min(Math.Max((int)(factor * green + bias), 0), 255);
                        int b = Math.Min(Math.Max((int)(factor * blue + bias), 0), 255);
                        unchecked
                        {
                            result[y * width1 + x] = (255 << 24) | (r << 16) | (g << 8) | b;
                        }
                    }
                }
            }
        }
    }
}
//MIT, 2014-present, WinterDev

using PixelFarm.CpuBlit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace Mini
{
    static class Program
    {
        class LocalFileStorageProvider : PixelFarm.Platforms.StorageServiceProvider
        {
            static readonly string Base = Path.Combine(Environment.CurrentDirectory, "Data");
            public override bool DataExists(string dataName)
            {
                //implement with file
                return System.IO.File.Exists(dataName);
            }
            public override byte[] ReadData(string dataName)
            {
                return System.IO.File.ReadAllBytes(dataName);
            }
            public override void SaveData(string dataName, byte[] content)
            {
                System.IO.File.WriteAllBytes(dataName, content);
            }
            public override PixelFarm.CpuBlit.ActualBitmap ReadPngBitmap(string filename)
            {
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    return PngIOStorage.Read(fs);
                }
            }
            public override void SavePngBitmap(PixelFarm.CpuBlit.ActualBitmap bmp, string filename)
            {
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    PngIOStorage.Save(bmp, fs);
                }
            }
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //PaintLab.Svg.SvgParser parser = new PaintLab.Svg.SvgParser();
            //string svgContent = System.IO.File.ReadAllText("Samples/arrow2.svg");
            //parser.ParseDocument(new LayoutFarm.WebLexer.TextSnapshot(svgContent));




            RootDemoPath.Path = @"..\Data";
            YourImplementation.TestBedStartup.Setup();

#if GL_ENABLE
            YourImplementation.BootStrapOpenGLES2.SetupDefaultValues();
#endif
            //you can use your font loader
            YourImplementation.BootStrapWinGdi.SetupDefaultValues();
            //default text breaker, this bridge between              
#if DEBUG
            //PixelFarm.Agg.ActualImage.InstallImageSaveToFileService((IntPtr imgBuffer, int stride, int width, int height, string filename) =>
            //{

            //    using (System.Drawing.Bitmap newBmp = new System.Drawing.Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            //    {
            //        PixelFarm.Agg.Imaging.BitmapHelper.CopyToGdiPlusBitmapSameSize(imgBuffer, newBmp);
            //        //save
            //        newBmp.Save(filename);
            //    }
            //});
#endif

            PixelFarm.Platforms.StorageService.RegisterProvider(new LocalFileStorageProvider());
            //Typography's TextServices
            //you can implement   Typography.TextBreak.DictionaryProvider  by your own
            //this set some essentail values for Typography Text Serice
            // 
            //2.2 Icu Text Break info
            //test Typography's custom text break,
            //check if we have that data?            
            //------------------------------------------- 
            //string typographyDir = @"brkitr_src/dictionaries";
            string icu_datadir = @"C:\Users\user\Source\Repos\Typography\Typography.TextBreak\icu62\brkitr";

            if (!System.IO.Directory.Exists(icu_datadir))
            {
                throw new System.NotSupportedException("dic");
            }
            var dicProvider = new Typography.TextBreak.IcuSimpleTextFileDictionaryProvider() { DataDir = icu_datadir };
            Typography.TextBreak.CustomBreakerBuilder.Setup(dicProvider);

            //---------------------------------------------------
            //register image loader
            Mini.DemoHelper.RegisterImageLoader(LoadImage);
            //----------------------------

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormDev());
        }
        static PixelFarm.CpuBlit.ActualBitmap LoadImage(string filename)
        {


            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(filename);


            var bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                                       System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                       System.Drawing.Imaging.PixelFormat.Format32bppArgb //lock and read as 32-argb
                                       );

            int[] imgBuffer = new int[bmpData.Width * bmp.Height];
            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, imgBuffer, 0, imgBuffer.Length);
            bmp.UnlockBits(bmpData);

            //PixelFarm.Agg.PixelFormat selectedFormat = PixelFarm.Agg.PixelFormat.ARGB32;
            //switch (bmp.PixelFormat)
            //{
            //    default:
            //        throw new NotSupportedException();
            //    //case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
            //    //    {
            //    //        bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
            //    //             System.Drawing.Imaging.ImageLockMode.ReadOnly,
            //    //             System.Drawing.Imaging.PixelFormat.Format32bppArgb //lock and read as 32-argb 
            //    //             );
            //    //        selectedFormat = PixelFarm.Agg.PixelFormat.ARGB32; //lock and read as 32-argb
            //    //        imgBuffer = new byte[bmpData.Stride * bmp.Height];
            //    //        System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, imgBuffer, 0, imgBuffer.Length);
            //    //        bmp.UnlockBits(bmpData);
            //    //    }
            //    //    break;
            //    case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
            //    case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
            //        {
            //            selectedFormat = PixelFarm.Agg.PixelFormat.ARGB32;
            //            bmpData = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
            //                System.Drawing.Imaging.ImageLockMode.ReadOnly,
            //                bmp.PixelFormat //lock and read as 32-argb
            //                );

            //            imgBuffer = new int[bmpData.Width * bmp.Height];
            //            System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, imgBuffer, 0, imgBuffer.Length);
            //            bmp.UnlockBits(bmpData);
            //        }
            //        break;
            //    case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
            //        //grey scale
            //        //selectedFormat = PixelFarm.Agg.PixelFormat.GrayScale8;
            //        throw new NotSupportedException();
            //}

            PixelFarm.CpuBlit.ActualBitmap actualImg = PixelFarm.CpuBlit.ActualBitmap.CreateFromBuffer(
                bmp.Width,
                bmp.Height,
                imgBuffer
                );
            //gdi+ load as little endian             
            actualImg.IsBigEndian = false;
            bmp.Dispose();
            return actualImg;
        }



        static class PngIOStorage
        {

            public static PixelFarm.CpuBlit.ActualBitmap Read(Stream strm)
            {

                Hjg.Pngcs.PngReader reader = new Hjg.Pngcs.PngReader(strm);
                Hjg.Pngcs.ImageInfo imgInfo = reader.ImgInfo;
                Hjg.Pngcs.ImageLine iline2 = new Hjg.Pngcs.ImageLine(imgInfo, Hjg.Pngcs.ImageLine.ESampleType.BYTE);

                int imgH = imgInfo.Rows;
                int imgW = imgInfo.Cols;
                int stride = imgInfo.BytesPerRow;
                int widthPx = imgInfo.Cols;

                int[] buffer = new int[(stride / 4) * imgH];
                bool isInverted = false;
                if (isInverted)
                {
                    //read each row 
                    //and fill the glyph image 
                    int startWriteAt = (imgW * (imgH - 1));
                    int destIndex = startWriteAt;
                    for (int row = 0; row < imgH; row++)
                    {
                        Hjg.Pngcs.ImageLine iline = reader.ReadRowByte(row);
                        byte[] scline = iline.ScanlineB;

                        int b_src = 0;
                        destIndex = startWriteAt;

                        for (int mm = 0; mm < imgW; ++mm)
                        {
                            byte b = scline[b_src];
                            byte g = scline[b_src + 1];
                            byte r = scline[b_src + 2];
                            byte a = scline[b_src + 3];
                            b_src += 4;
                            buffer[destIndex] = (b << 16) | (g << 8) | (r) | (a << 24);
                            destIndex++;
                        }
                        startWriteAt -= imgW;
                    }
                    return new ActualBitmap(imgW, imgH, buffer);
                }
                else
                {
                    //read each row 
                    //and fill the glyph image 
                    int startWriteAt = 0;
                    int destIndex = startWriteAt;
                    for (int row = 0; row < imgH; row++)
                    {
                        Hjg.Pngcs.ImageLine iline = reader.ReadRowByte(row);
                        byte[] scline = iline.ScanlineB;

                        int b_src = 0;
                        destIndex = startWriteAt;

                        for (int mm = 0; mm < imgW; ++mm)
                        {

                            byte b = scline[b_src];
                            byte g = scline[b_src + 1];
                            byte r = scline[b_src + 2];
                            byte a = scline[b_src + 3];
                            b_src += 4;
                            buffer[destIndex] = (b << 16) | (g << 8) | (r) | (a << 24);
                            destIndex++;
                        }
                        startWriteAt += imgW;
                    }
                    return new ActualBitmap(imgW, imgH, buffer);
                }


            }
            public static void Save(ActualBitmap actualImg, Stream strm)
            {
                //-------------
                unsafe
                {
                    PixelFarm.CpuBlit.Imaging.TempMemPtr tmp = ActualBitmap.GetBufferPtr(actualImg);
                    int* intBuffer = (int*)tmp.Ptr;

                    int imgW = actualImg.Width;
                    int imgH = actualImg.Height;

                    Hjg.Pngcs.ImageInfo imgInfo = new Hjg.Pngcs.ImageInfo(imgW, imgH, 8, true); //8 bits per channel with alpha
                    Hjg.Pngcs.PngWriter writer = new Hjg.Pngcs.PngWriter(strm, imgInfo);
                    Hjg.Pngcs.ImageLine iline = new Hjg.Pngcs.ImageLine(imgInfo, Hjg.Pngcs.ImageLine.ESampleType.BYTE);
                    int startReadAt = 0;

                    int imgStride = imgW * 4;

                    int srcIndex = 0;
                    int srcIndexRowHead = (tmp.LengthInBytes / 4) - imgW;

                    for (int row = 0; row < imgH; row++)
                    {
                        byte[] scanlineBuffer = iline.ScanlineB;
                        srcIndex = srcIndexRowHead;
                        for (int b = 0; b < imgStride;)
                        {
                            int srcInt = intBuffer[srcIndex];
                            srcIndex++;
                            scanlineBuffer[b] = (byte)((srcInt >> 16) & 0xff);
                            scanlineBuffer[b + 1] = (byte)((srcInt >> 8) & 0xff);
                            scanlineBuffer[b + 2] = (byte)((srcInt) & 0xff);
                            scanlineBuffer[b + 3] = (byte)((srcInt >> 24) & 0xff);
                            b += 4;
                        }
                        srcIndexRowHead -= imgW;
                        startReadAt += imgStride;
                        writer.WriteRow(iline, row);
                    }
                    writer.End();
                }


            }


        }

    }
}

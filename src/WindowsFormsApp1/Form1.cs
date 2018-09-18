using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using PixelFarm.Drawing;
using Typography.FontManagement;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            PixelFarm.Drawing.Skia.SkiaGraphicsPlatform.SetInstalledTypefaceProvider(
                InstalledTypefaceCollection.GetSharedTypefaceCollection(collection =>
                    collection.AddFontStreamSource(new FontSource())));
            var view = new SkiaSharp.Views.Desktop.SKControl
            { Dock = DockStyle.Fill };
            view.PaintSurface += (sender, e) =>
            {
                using (var board = new PixelFarm.Drawing.Skia.MySkiaDrawBoard(0, 0, 0, 0, e.Info.Width, e.Info.Height))
                {
                    board.CurrentTextColor = new PixelFarm.Drawing.Color(0, 0, 0);
                    board.CurrentFont = new RequestFont("Times New Roman", 30);
                    board.DrawText(new[] { 'a', 'b', 'c' }, 0, e.Info.Height);
                    e.Surface.Canvas.DrawBitmap(board.BackBmp, 0, 0);
                }
            };
            Controls.Add(view);
        }
        class FontSource : IFontStreamSource
        {
            public string PathName => @"C:\Windows\Fonts\times.ttf";

            public Stream ReadFontStream() => new FileStream(PathName, FileMode.Open, FileAccess.Read);
        }
    }
}

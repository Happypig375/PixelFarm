using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PixelFarm.Drawing;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var view = new SkiaSharp.Views.Desktop.SKControl();
            view.PaintSurface += (sender, e) =>
            {
                using (var board = new PixelFarm.Drawing.Skia.MySkiaDrawBoard(0, 0, 0, 0, 1000, 1000))
                {
                    board.CurrentTextColor = new PixelFarm.Drawing.Color(0, 0, 0);
                    board.CurrentFont = new RequestFont("Arial", 30);
                    board.DrawText(new[] { 'a', 'b', 'c' }, 0, 0);
                    e.Surface.Canvas.DrawBitmap(board.BackBmp, 0, 0);
                }
            };
            Controls.Add(view);
        }
    }
}

﻿//Apache2, 2014-present, WinterDev

using PixelFarm.Drawing;
using PaintLab.Svg;
using PixelFarm.CpuBlit;
using LayoutFarm.UI;

namespace LayoutFarm
{
    [DemoNote("4.1 Demo_LionSprite")]
    class Demo_LionSprite : App
    {
        VgRenderVx _renderVx;
       
        MyTestSprite _mySprite;
        AppHost _host;
        protected override void OnStart(AppHost host)
        {
            _host = host;
            _renderVx = SvgRenderVxLoader.CreateSvgRenderVxFromFile(@"Samples\lion.svg");
            _mySprite = new MyTestSprite(_renderVx);
            host.AddChild(_mySprite);

            //
            var textbox = new LayoutFarm.CustomWidgets.TextBox(100, 30, false);
            var textSpanStyle = new Text.TextSpanStyle();

            //test with various font style
            textSpanStyle.FontInfo = new PixelFarm.Drawing.RequestFont("tahoma", 18);
            textSpanStyle.FontColor = new PixelFarm.Drawing.Color(255, 0, 0);
            //set default style
            textbox.DefaultSpanStyle = textSpanStyle;

            host.AddChild(textbox);


            //var box1 = new LayoutFarm.CustomWidgets.Box(50, 50);
            //box1.BackColor = Color.Red;
            //box1.SetLocation(10, 10);
            //host.AddChild(box1);
            ////--------------------------------
            //var box2 = new LayoutFarm.CustomWidgets.Box(30, 30);
            //box2.SetLocation(50, 50);
            //host.AddChild(box2);
            ////1. mouse down         
            //box1.MouseDown += (s, e) =>
            //{
            //    box1.BackColor = KnownColors.FromKnownColor(KnownColor.DeepSkyBlue);
            //    box2.Visible = false;
            //};
            //box1.MouseUp += (s, e) =>
            //{
            //    box1.BackColor = Color.Red;
            //    box2.Visible = true;
            //}; 
        }
    }
}
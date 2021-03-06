﻿//Apache2, 2014-present, WinterDev

using System;
using LayoutFarm.UI;
namespace LayoutFarm
{
    [DemoNote("1.1 SingleButton")]
    class Demo_SingleButton : App
    {
        protected override void OnStart(AppHost host)
        {
            var sampleButton = new LayoutFarm.CustomWidgets.Box(30, 30);
            host.AddChild(sampleButton);
            int count = 0;
            sampleButton.MouseDown += new EventHandler<UIMouseEventArgs>((s, e2) =>
            {
                Console.WriteLine("click :" + (count++));
            });
        }
    }
}
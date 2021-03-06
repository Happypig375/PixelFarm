﻿//Apache2, 2014-present, WinterDev

namespace LayoutFarm.UI
{
    public static class Clipboard
    {
        static UIPlatform currentUIPlatform;
        public static void Clear()
        {
        }
        public static void SetText(string text)
        {
            //textdata = text;
            currentUIPlatform.SetClipboardData(text);
        }
        public static bool ContainUnicodeText()
        {
            return currentUIPlatform.GetClipboardData() != null;
        }
        public static string GetUnicodeText()
        {
            return currentUIPlatform.GetClipboardData();
        }

        public static void SetUIPlatform(UIPlatform uiPlatform)
        {
            currentUIPlatform = uiPlatform;
        }
    }
}
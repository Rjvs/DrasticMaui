﻿// <copyright file="PlatformExtensions.Windows.cs" company="Drastic Actions">
// Copyright (c) Drastic Actions. All rights reserved.
// </copyright>

using Microsoft.Maui.Platform;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Windows.Storage.Streams;
using WinPoint = Windows.Foundation.Point;

namespace DrasticMaui.Tools
{
    public struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public override string ToString()
        {
            return "(" + left + ", " + top + ") --> (" + right + ", " + bottom + ")";
        }
    }

    /// <summary>
    /// Windows Platform Extensions.
    /// </summary>
    public static class PlatformExtensions
    {
        public static AppWindow GetAppWindowForWinUI(this Microsoft.UI.Xaml.Window window)
        {
            var windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);

            return GetAppWindowFromWindowHandle(windowHandle);
        }

        private static AppWindow GetAppWindowFromWindowHandle(IntPtr windowHandle)
        {
            var windowId = Win32Interop.GetWindowIdFromWindow(windowHandle);
            return AppWindow.GetFromWindowId(windowId);
        }

        public static Microsoft.Maui.Graphics.Rectangle GetBoundingBox(this IView view, IMauiContext context)
            => view.ToPlatform(context).GetBoundingBox();

        public static Microsoft.Maui.Graphics.Rectangle GetBoundingBox(this FrameworkElement? nativeView)
        {
            if (nativeView == null)
                return new Microsoft.Maui.Graphics.Rectangle();

            var rootView = nativeView.XamlRoot.Content;
            if (nativeView == rootView)
            {
                if (rootView is not FrameworkElement el)
                    return new Microsoft.Maui.Graphics.Rectangle();

                return new Microsoft.Maui.Graphics.Rectangle(0, 0, el.ActualWidth, el.ActualHeight);
            }

            var topLeft = nativeView.TransformToVisual(rootView).TransformPoint(new WinPoint());
            var topRight = nativeView.TransformToVisual(rootView).TransformPoint(new WinPoint(nativeView.ActualWidth, 0));
            var bottomLeft = nativeView.TransformToVisual(rootView).TransformPoint(new WinPoint(0, nativeView.ActualHeight));
            var bottomRight = nativeView.TransformToVisual(rootView).TransformPoint(new WinPoint(nativeView.ActualWidth, nativeView.ActualHeight));

            var x1 = new[] { topLeft.X, topRight.X, bottomLeft.X, bottomRight.X }.Min();
            var x2 = new[] { topLeft.X, topRight.X, bottomLeft.X, bottomRight.X }.Max();
            var y1 = new[] { topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y }.Min();
            var y2 = new[] { topLeft.Y, topRight.Y, bottomLeft.Y, bottomRight.Y }.Max();
            return new Microsoft.Maui.Graphics.Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        public static IRandomAccessStream ToRandomAccessStream(this Stream stream)
        {
            InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream();
            using (DataWriter writer = new DataWriter(ms.GetOutputStreamAt(0)))
            {
                var array = (byte[])stream.ToByteArray();
                writer.WriteBytes(array);
                writer.StoreAsync().GetResults();
            }

            stream.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public static byte[] ToByteArray(this Stream stream)
        {
            MemoryStream ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }
    }
}

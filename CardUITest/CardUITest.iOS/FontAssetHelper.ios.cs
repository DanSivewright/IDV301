using System;
using System.IO;
using CardUITest.iOS;
using CardUITest.Services;
using Foundation;
using SkiaSharp;
using Xamarin.Forms;

[assembly: Dependency(typeof(FontAssetHelper))]
namespace CardUITest.iOS
{
    public class FontAssetHelper : IFontHelper
    {
        public SKTypeface GetSkiaTypefaceFromAssetFont(string fontName)
        {
            string fontFile = Path.Combine(NSBundle.MainBundle.BundlePath, fontName);
            SkiaSharp.SKTypeface typeFace;

            using (var asset = File.OpenRead(fontFile))
            {
                var fontStream = new MemoryStream();
                asset.CopyTo(fontStream);
                fontStream.Flush();
                fontStream.Position = 0;
                typeFace = SKTypeface.FromStream(fontStream);
            }

            return typeFace;
        }
    }
}

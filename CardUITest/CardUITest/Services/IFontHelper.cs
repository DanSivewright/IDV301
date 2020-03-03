using System;
using SkiaSharp;

namespace CardUITest.Services
{
    public interface IFontHelper
    {
        SKTypeface GetSkiaTypefaceFromAssetFont(string fontName);
    }
}

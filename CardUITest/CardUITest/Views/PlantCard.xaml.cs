using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardUITest.ViewModels;
using SkiaSharp;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SkiaSharp.Views.Forms;

namespace CardUITest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlantCard : ContentView
    {
        private Plant _viewModel;
        private readonly float _density;
        private readonly float _cardTopMargin;
        private readonly float _cornerRadius = 60f;

        //private static SKTypeface _typeface;

        // Cached skia color and paint object
        SKColor _plantColor;
        SKPaint _plantPaint;

        public PlantCard()
        {
            InitializeComponent();

            _density = (float)Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Density;
            _cardTopMargin = 400f * _density;
            _cornerRadius = 30f * _density;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            if (this.BindingContext == null) return;
            _viewModel = this.BindingContext as Plant;

            //Because we can't bind the skia drawing using the binding engine
            // We cache the paint objects when the bound character changes

            _plantColor = Color.FromHex(_viewModel.PlantColor).ToSKColor();
            _plantPaint = new SKPaint() { Color = _plantColor };

            // repaint the surfce with the new colors
            CardBackground.InvalidateSurface();
        }

        public Image MainImage
        {
            get
            {
                return PlantImage;
            }
        }

        private void CardBackground_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs args)
        {
            // seems that PaintSurface is called before the 
            // binding context is set sometimes
            if (_viewModel == null) return;

            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            canvas.DrawRoundRect(
                rect: new SKRect(0, (float)_cardTopMargin, info.Width, info.Height),
                r: new SKSize(_cornerRadius, _cornerRadius),
                paint: _plantPaint);
        }

        
    }
}
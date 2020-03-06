using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CardUITest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NavigationMenu : ContentPage
    {
        public NavigationMenu()
        {
            InitializeComponent();
        }

        //SKPaint circleColor = new SKPaint
        //{
        //    Style = SKPaintStyle.Fill,
        //    Color = SKColors.CornflowerBlue
        //};

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }



        //private void canvasView_PaintSurface(object sender, SkiaSharp.Views.Forms.SKPaintSurfaceEventArgs e)
        //{
        //    SKSurface surface = e.Surface;
        //    SKCanvas canvas = surface.Canvas;

        //    canvas.Clear();

        //    int height = e.Info.Height;
        //    int width = e.Info.Width;

        //    canvas.Translate(120, 150);
        //    canvas.Scale(30);

        //    canvas.DrawCircle(0, 0, 20, circleColor);
        //}
    }
}
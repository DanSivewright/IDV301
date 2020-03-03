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
using CardUITest.Models;

namespace CardUITest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlantCard : ContentView
    {
        private Plant _viewModel;
        private readonly float _density;
        private readonly float _cardTopMargin;
        private float _cornerRadius = 60f;
        private CardState _cardState = CardState.Collapsed;
        private float _gradientHeight = 200f;

        //private static SKTypeface _typeface;

        // Cached skia color and paint object
        SKColor _plantColor;
        SKPaint _plantPaint;
        private double _cardTopAnimPosition;
        private float _gradientTransitionY;

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
            _gradientTransitionY = float.MaxValue;

            // Setup initial values
            _cardTopAnimPosition = _cardTopMargin;

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

            // draw top hero color
            canvas.DrawRoundRect(
                rect: new SKRect(0, (float)_cardTopAnimPosition, info.Width, info.Height),
                r: new SKSize(_cornerRadius, _cornerRadius),
                paint: _plantPaint);

            // work out the gradient needs to be
            var gradientRect = new SKRect(0, _gradientTransitionY, info.Width,
                _gradientTransitionY + _gradientHeight);
            // create the gradient
            var gradientPaint = new SKPaint() { Style = SKPaintStyle.Fill };
            gradientPaint.Shader = GetGradientShader(_plantColor, SKColors.White);

            // draw the gradient
            canvas.DrawRect(gradientRect, gradientPaint);

            // draw the white bit
            SKRect bottomRect = new SKRect(0, _gradientTransitionY + _gradientHeight,
                info.Width, info.Height);
            canvas.DrawRect(bottomRect, new SKPaint() { Color = SKColors.White });

        }

        private SKShader GetGradientShader(SKColor fromColor, SKColor toColor)
        {
            return SKShader.CreateLinearGradient
                (
                    start: new SKPoint(0, _gradientTransitionY),
                    end: new SKPoint(0, _gradientTransitionY + _gradientHeight),
                    colors: new SKColor[] { fromColor, toColor },
                    colorPos: new float[] { 0, 1 },
                    SKShaderTileMode.Clamp
                );
        }

        private void ManagePlant_Tapped(object sender, EventArgs e)
        {
            // Go to state of expanded
            GoToState(CardState.Expanded);
        }

        public void GoToState(CardState cardState)
        {
            // trigger animations
            if (_cardState == cardState)
                return;

            MessagingCenter.Send<CardEvent>(new CardEvent(), cardState.ToString());

            // Handle Animations
            AnimateTransition(cardState);

            _cardState = cardState;
        }

        private void AnimateTransition(CardState cardState)
        {
            var parentAnimation = new Animation();

            if (cardState == CardState.Expanded)
            {
                parentAnimation.Add(0.00, 0.10, CreateCardAnimation(cardState));
                parentAnimation.Add(0.00, 0.50, CreateImageAnimation(cardState));
                parentAnimation.Add(0.10, 0.50, CreatePlantNameAnimation(cardState));
                parentAnimation.Add(0.15, 0.50, CreatePlantTypeAnimation(cardState));
                parentAnimation.Add(0.00, 0.25, CreateManageAnimation(cardState));
                parentAnimation.Add(0.50, 0.75, CreateGradientAnimation(cardState));
            }
            else
            {
                parentAnimation.Add(0.25, 0.45, CreateImageAnimation(cardState));
                parentAnimation.Add(0.25, 0.45, CreateCardAnimation(cardState));
                parentAnimation.Add(0.25, 0.50, CreatePlantNameAnimation(cardState));
                parentAnimation.Add(0.25, 0.50, CreatePlantTypeAnimation(cardState));
                parentAnimation.Add(0.35, 0.45, CreateManageAnimation(cardState));
                parentAnimation.Add(0.00, 0.25, CreateGradientAnimation(cardState));
            }
            parentAnimation.Commit(this, "CardExpand", 16, 2000);
        }

        private Animation CreateGradientAnimation(CardState cardState)
        {
            double start;
            double end;
            if (cardState == CardState.Expanded)
            {
                _gradientTransitionY = CardBackground.CanvasSize.Height;
                start = _gradientTransitionY;
                end = -_gradientHeight;
            }
            else
            {
                _gradientTransitionY = -_gradientHeight;
                start = _gradientTransitionY;
                end = CardBackground.CanvasSize.Height;
            }

            var gradientAnimation = new Animation(
                callback: v =>
                {
                    _gradientTransitionY = (float)v;
                    CardBackground.InvalidateSurface();
                },
                start: start,
                end: end,
                easing: Easing.Linear,
                finished: () =>
                {
                    
                }
                );

            return gradientAnimation;

        }

        private Animation CreateManageAnimation(CardState cardState)
        {
            var manageAnimStart = cardState == CardState.Expanded ? 0 : 100;
            var manageAnimSEnd = cardState == CardState.Expanded ? 100 : 0;

            var manageAnim = new Animation(
                v =>
                {
                    ManagePlantLabel.TranslationX = v;
                    ManagePlantLabel.Opacity = 1 - (v / 100);
                },
                manageAnimStart,
                manageAnimSEnd,
                Easing.SinInOut
                );
            return manageAnim;
        }

        private Animation CreatePlantTypeAnimation(CardState cardState)
        {
            var typeAnimStart = cardState == CardState.Expanded ? 0 : -120;
            var typeAnimEnd = cardState == CardState.Expanded ? -120 : 0;

            var imageAnim = new Animation(
                v =>
                {
                    PlantType.TranslationY = v;
                },
                typeAnimStart,
                typeAnimEnd,
                Easing.SpringOut
                );
            return imageAnim;
        }

        private Animation CreatePlantNameAnimation(CardState cardState)
        {
            var nameAnimStart = cardState == CardState.Expanded ? 0 : -120;
            var nameAnimEnd = cardState == CardState.Expanded ? -120 : 0;

            var imageAnim = new Animation(
                v =>
                {
                    PlantName.TranslationY = v;
                },
                nameAnimStart,
                nameAnimEnd,
                Easing.SpringOut
                );
            return imageAnim;
        }

        private Animation CreateImageAnimation(CardState cardState)
        {
            // work out where the top of the card should be
            var imageAnimStart = cardState == CardState.Expanded ? PlantImage.TranslationY : 60;
            var imageAnimEnd = cardState == CardState.Expanded ? 60 : 180;

            var imageAnim = new Animation(
                v =>
                {
                    PlantImage.TranslationY = v;
                },
                imageAnimStart,
                imageAnimEnd,
                Easing.SpringOut
                );
            return imageAnim;
        }

        private Animation CreateCardAnimation(CardState cardState)
        {
            // work out where the top of the card should be
            var cardAnimStart = cardState == CardState.Expanded ? _cardTopMargin : -_cornerRadius;
            var cardAnimEnd = cardState == CardState.Expanded ? -_cornerRadius : _cardTopMargin;

            var cardAnim = new Animation(
                v =>
                {

                    _cardTopAnimPosition = v;
                    CardBackground.InvalidateSurface();
                },
                cardAnimStart,
                cardAnimEnd,
                Easing.SinInOut
                );
            return cardAnim;
        }
    }
}
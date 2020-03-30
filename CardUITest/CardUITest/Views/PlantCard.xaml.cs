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
using CardUITest.Services;
using System.Collections.ObjectModel;

namespace CardUITest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlantCard : ContentView
    {
        private Plant _viewModel;
        public Plant _updatedViewModel;
        private readonly float _density;
        private readonly float _cardTopMargin;
        private float _cornerRadius = 60f;
        private CardState _cardState = CardState.Collapsed;
        private float _gradientHeight = 200f;

        private static SKTypeface _typeface;

        //private static SKTypeface _typeface;

        // Cached skia color and paint object
        SKColor _plantColor;
        SKPaint _plantPaint;
        private double _cardTopAnimPosition;
        private float _gradientTransitionY;

        // font and label 
        private SKPaint _plantNamePaint;
        private float _plantNamePosY;
        private float _plantNamePosX;
        private float _plantNameOffsetY;

        public PlantCard()
        {

            InitializeComponent();
            MessagingCenter.Subscribe<PlantDetailsSection, Plant>(this, "updatedViewModel", (s, a) => {
                OnBindingContextChanged();
            });

            _density = (float)Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Density;
            _cardTopMargin = 400f * _density;
            _cornerRadius = 30f * _density;

            // Load up the typeface, just once
            if (_typeface == null)
            {
                _typeface = DependencyService.Get<IFontHelper>().GetSkiaTypefaceFromAssetFont("Montserrat-Bold.ttf");
            }
        }

        public Plant viewModel
        {
            get => _viewModel;

            set
            {
                _viewModel = value;
            }
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

            _plantNamePaint = new SKPaint()
            {
                Typeface = _typeface,
                IsAntialias = true,
                Color = SKColors.White,
                TextSize = 60f * _density
            };

            _plantNamePosY = 640f * _density;
            _plantNamePosX = 40f * _density;

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
            canvas.DrawRect(bottomRect, new SKPaint() { Color = new SKColor(246, 247, 250) });

            DrawPlantName(canvas);
        }

        private void DrawPlantName(SKCanvas canvas)
        {
            // apply the gradient shader
            _plantNamePaint.Shader = GetGradientShader(SKColors.White, SKColors.Black);

            var textPos = new SKPoint(_plantNamePosX, _plantNameOffsetY + _plantNamePosY);
            canvas.DrawText(_viewModel.PlantName, textPos, _plantNamePaint);
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
            PlantDetails.InputTransparent = _cardState == CardState.Collapsed;
        }

        private void AnimateTransition(CardState cardState)
        {
            var parentAnimation = new Animation();

            if (cardState == CardState.Expanded)
            {
                parentAnimation.Add(0.00, 0.10, CreateCardAnimation(cardState));
                //parentAnimation.Add(0.00, 0.50, CreateImageAnimation(cardState));
                parentAnimation.Add(0.10, 0.50, CreatePlantNameAnimation(cardState));
                parentAnimation.Add(0.15, 0.50, CreatePlantTypeAnimation(cardState));
                parentAnimation.Add(0.00, 0.25, CreateManageAnimation(cardState));
                parentAnimation.Add(0.50, 0.75, CreateGradientAnimation(cardState));

                // Image Animation
                parentAnimation.Add(0.00, 0.50, new Animation((v) => PlantImage.TranslationY = v, PlantImage.TranslationY, 60, Easing.SpringOut));
                parentAnimation.Add(0.00, 0.10, new Animation((v) => PlantImage.Opacity = v, 1, 0, Easing.Linear));

                // Animating in the plant details section
                parentAnimation.Add(0.60, 0.85, new Animation((v) => PlantDetails.TranslationY = v, 
                    200, 0, Easing.SpringOut));
                parentAnimation.Add(0.60, 0.85, new Animation((v) => PlantDetails.Opacity = v, 
                    0, 1, Easing.Linear));

            }
            else
            {
                //parentAnimation.Add(0.25, 0.45, CreateImageAnimation(cardState));
                parentAnimation.Add(0.25, 0.45, CreateCardAnimation(cardState));
                parentAnimation.Add(0.25, 0.50, CreatePlantNameAnimation(cardState));
                parentAnimation.Add(0.25, 0.50, CreatePlantTypeAnimation(cardState));
                parentAnimation.Add(0.45, 0.50, CreateManageAnimation(cardState));
                parentAnimation.Add(0.00, 0.25, CreateGradientAnimation(cardState));

                // Image Animation
                parentAnimation.Add(0.25, 0.45, new Animation((v) => PlantImage.TranslationY = v, 60, 180, Easing.SpringOut));
                parentAnimation.Add(0.25, 0.45, new Animation((v) => PlantImage.Opacity = v, 0, 1, Easing.Linear));

                parentAnimation.Add(0.00, 0.25, new Animation((v) => PlantDetails.TranslationY = v,
                    0, 200, Easing.SpringIn));
                parentAnimation.Add(0.00, 0.25, new Animation((v) => PlantDetails.Opacity = v,
                    1, 0, Easing.Linear));
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
            var typeAnimStart = cardState == CardState.Expanded ? 0 : -490;
            var typeAnimEnd = cardState == CardState.Expanded ? -100 : 0;

            var imageAnim = new Animation(
                v =>
                {
                    PlantType.TranslationY = v;
                    PlantType.Opacity = 1 - ((-1 * v) / 100);
                },
                typeAnimStart,
                typeAnimEnd,
                Easing.SpringOut
                );
            return imageAnim;
        }

        private Animation CreatePlantNameAnimation(CardState cardState)
        {
            var nameAnimStart = cardState == CardState.Expanded ? 0 : -490;
            var nameAnimEnd = cardState == CardState.Expanded ? -490 : 0;

            var imageAnim = new Animation(
                v =>
                {
                    _plantNameOffsetY = (float)v * _density;
                    CardBackground.InvalidateSurface();
                },
                nameAnimStart,
                nameAnimEnd,
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

        private void SKCanvasView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKImageInfo info = e.Info;
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            using (SKPaint paint = new SKPaint())
            {
                SKColor shadowColor = Color.FromHex(_viewModel.PlantColor).ToSKColor();

                paint.IsDither = true;
                paint.IsAntialias = true;
                paint.Color = shadowColor;


                paint.ImageFilter = SKImageFilter.CreateDropShadow(
                    dx: 0, dy: 0,
                    sigmaX: 40, sigmaY: 40,
                    color: shadowColor,
                    shadowMode: SKDropShadowImageFilterShadowMode.DrawShadowOnly);

                var margin = info.Width / 10;
                var shadowBounds = new SKRect(margin, -40, info.Width - margin, 10);
                canvas.DrawRoundRect(shadowBounds, 10, 10, paint);

            }
        }
    }
}
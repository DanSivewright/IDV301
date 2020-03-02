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

        //private static SKTypeface _typeface;

        // Cached skia color and paint object
        SKColor _plantColor;
        SKPaint _plantPaint;
        private double _cardTopAnimPosition;

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

            canvas.DrawRoundRect(
                rect: new SKRect(0, (float)_cardTopAnimPosition, info.Width, info.Height),
                r: new SKSize(_cornerRadius, _cornerRadius),
                paint: _plantPaint);
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
            }
            else
            {
                parentAnimation.Add(0.20, 0.40, CreateImageAnimation(cardState));
                parentAnimation.Add(0.20, 0.40, CreateCardAnimation(cardState));
                parentAnimation.Add(0.20, 0.45, CreatePlantNameAnimation(cardState));
                parentAnimation.Add(0.20, 0.45, CreatePlantTypeAnimation(cardState));
                parentAnimation.Add(0.30, 0.40, CreateManageAnimation(cardState));
            }
            parentAnimation.Commit(this, "CardExpand", 16, 2000);
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
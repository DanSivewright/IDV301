using CardUITest.ViewModels;
using CardUITest.Views;
using PanCardView;
using PanCardView.EventArgs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static CardUITest.Helpers;

namespace CardUITest
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private double _plantImageTranslationY = 180;
        private double _movementFactor = 100;

        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = new CardViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MainCardView.UserInteracted += MainCardView_UserInteracted;
            MessagingCenter.Subscribe<CardEvent>(this, CardState.Expanded.ToString(), CardExpand);
        }

        private void CardExpand(CardEvent obj)
        {
            MainCardView.IsUserInteractionEnabled = false;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            MainCardView.UserInteracted -= MainCardView_UserInteracted;
            MessagingCenter.Unsubscribe<CardEvent>(this, CardState.Expanded.ToString());
        }

        private void MainCardView_UserInteracted(
            PanCardView.CardsView view,
            PanCardView.EventArgs.UserInteractedEventArgs args)
        {

            if (args.Status == PanCardView.Enums.UserInteractionStatus.Running)
            {

                // Get the card that is showing
                var card = MainCardView.CurrentView as PlantCard;

                // Work out what percentage the swipe is
                var percentFromCenter = Math.Abs(args.Diff / this.Width);

                // Adjust scale when panning
                if ((percentFromCenter > 0) && (card.Scale == 1))
                {
                    card.ScaleTo(.95, 50);
                }

                // Update elements based on swipe position
                AnimateFrontCardDuringSwipe(card, percentFromCenter);

                // Get the next card in the stack, which is coming into the view
                var nextCard = MainCardView.CurrentBackViews.First() as PlantCard;

                //Update elements based on swipe postion
                AnimateIncomingCardDuringSwipe(nextCard, percentFromCenter);
            }

            // Setting Default Values after swipe
            if (args.Status == PanCardView.Enums.UserInteractionStatus.Ended ||
                args.Status == PanCardView.Enums.UserInteractionStatus.Ending)
            {
                var card = MainCardView.CurrentView as PlantCard;
                AnimateFrontCardDuringSwipe(card, 0);
                card.ScaleTo(1, 50);
            }
        }

        private void AnimateFrontCardDuringSwipe(PlantCard card, double percentFromCenter)
        {
            // Opacity of the front card during swipe
            MainCardView.CurrentView.Opacity = LimitToRange((1 - (percentFromCenter)) * 2, 0, 1);

            //Scaling on the main card during swipe
            card.MainImage.Scale = LimitToRange((1 - (percentFromCenter) * 1.5), 0, 1);

            // y offset of image during swipe
            card.MainImage.TranslationY = _plantImageTranslationY + (_movementFactor * percentFromCenter);

            // adjust opacity of image
            card.MainImage.Opacity = LimitToRange((1 - (percentFromCenter)) * 1.5, 0, 1);
        }

        private void AnimateIncomingCardDuringSwipe(PlantCard nextCard, double percentFromCenter)
        {
            // opacity fading in
            nextCard.MainImage.Opacity = LimitToRange(percentFromCenter * 1.5, 0, 1);

            // scaling in
            nextCard.MainImage.Scale = LimitToRange(percentFromCenter * 1.1, 0, 1);

            var offset = _plantImageTranslationY + (_movementFactor * (1 - (percentFromCenter * 1.1)));
            nextCard.MainImage.TranslationY = LimitToRange(offset, _plantImageTranslationY, _plantImageTranslationY + _movementFactor);
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {

        }
    }
}

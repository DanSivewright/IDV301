using CardUITest.Models;
using CardUITest.ViewModels;
using CardUITest.Views;
using PanCardView;
using PanCardView.EventArgs;
using SkiaSharp;
using SQLite;
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

            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                // Delete all the contents of the table
                //conn.DropTable<Plant>();

                conn.CreateTable<Plant>();
                var plants = conn.Table<Plant>().ToList();

                MainCardView.ItemsSource = plants;
            }
        }

        private void CardExpand(CardEvent obj)
        {
            // Turn off swipe interaction
            MainCardView.IsUserInteractionEnabled = false;

            // Animate title out
            AnimateTitle(CardState.Expanded);
        }

        private void BackMenu_Tapped(object sender, EventArgs e)
        {
            // re-enable swiping
            MainCardView.IsUserInteractionEnabled = true;

            // Tell the card to collapse 
            ((PlantCard)MainCardView.CurrentView).GoToState(CardState.Collapsed);

            // Animate Title Back in
            AnimateTitle(CardState.Collapsed);

            
        }

        private void AnimateTitle(CardState cardState)
        {
            var TitleTranslateY = cardState == CardState.Expanded ? 0 - (PlantHeader.Height + PlantHeader.Margin.Top) : 0;
            var MenuTranslateY = cardState == CardState.Expanded ? 0 : 62;

            var escapeTranslateX = cardState == CardState.Expanded ? 0 : -40;
            var escapeOpacity = cardState == CardState.Expanded ? 1 : 0;

            var Opacity = cardState == CardState.Expanded ? 0 : 1;
      

            var animation = new Animation();

            if (cardState == CardState.Expanded)
            {
                animation.Add(0.00, 0.25, new Animation(v => PlantHeader.TranslationY = v, PlantHeader.TranslationY, TitleTranslateY));
                animation.Add(0.00, 0.25, new Animation(v => menu_container.TranslationY = v, menu_container.TranslationY, MenuTranslateY));


                animation.Add(0.00, 0.25, new Animation(v => escape_button.TranslationX = v, escape_button.TranslationX, escapeTranslateX));
                animation.Add(0.00, 0.25, new Animation(v => escape_button.Opacity = v, escape_button.Opacity, escapeOpacity));

                animation.Add(0.00, 0.25, new Animation(v => PlantHeader.Opacity = v, PlantHeader.Opacity, Opacity));
                animation.Add(0.00, 0.15, new Animation(v => menu_container.Opacity = v, menu_container.Opacity, Opacity));
            }
            else
            {
                animation.Add(0.75, 1.0, new Animation(v => PlantHeader.TranslationY = v, PlantHeader.TranslationY, TitleTranslateY));
                animation.Add(0.75, 1.0, new Animation(v => menu_container.TranslationY = v, menu_container.TranslationY, MenuTranslateY));


                animation.Add(0.00, 0.25, new Animation(v => escape_button.TranslationX = v, escape_button.TranslationX, escapeTranslateX));
                animation.Add(0.00, 0.25, new Animation(v => escape_button.Opacity = v, escape_button.Opacity, escapeOpacity));


                animation.Add(0.75, 1.0, new Animation(v => PlantHeader.Opacity = v, PlantHeader.Opacity, Opacity));
                animation.Add(0.75, 1.0, new Animation(v => menu_container.Opacity = v, menu_container.Opacity, Opacity));
            }
            animation.Commit(this, "titleAnimation", 16, 1000);
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

        async private void CreatePlant_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new CreatePlant());
        }

        async private void menu_container_OnClick(object sender, EventArgs e)
        {
            //menu_container.Source = "";
            //menu_container.ScaleTo(45, 1000, Easing.SpringOut);
            //menu_container.BackgroundColor = Xamarin.Forms.Color.Orange;
            animateMenu(CardState.Collapsed);
        }

        async private void animateMenu(CardState cardState)
        {
            await Navigation.PushModalAsync(new NavigationMenu());
        }
    }
}

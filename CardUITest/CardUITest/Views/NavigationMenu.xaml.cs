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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            navBackground.ScaleTo(100, 2000, Easing.SpringOut);
        }

        async void allPlants_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        async void createPlants_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushModalAsync(new CreatePlant());
        }

        async void navItem_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushModalAsync(new ProfilePage());
        }
    }
}
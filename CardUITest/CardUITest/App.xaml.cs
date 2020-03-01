using CardUITest.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CardUITest
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new CreatePage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

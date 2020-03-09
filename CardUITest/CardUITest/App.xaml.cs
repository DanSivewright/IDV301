using CardUITest.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CardUITest
{
    public partial class App : Application
    {

        public static string FilePath;

        public App()
        {
            InitializeComponent();

            MainPage = new MyPage();
        }
        
        public App(string filePath)
        {
            InitializeComponent();

            MainPage = new MyPage();

            FilePath = filePath;
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

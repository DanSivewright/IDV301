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
    public partial class CreatePlant : ContentPage
    {
        public CreatePlant()
        {
            InitializeComponent();
        }

        private void saveButton_Clicked(object sender, EventArgs e)
        {

        }
    }
}
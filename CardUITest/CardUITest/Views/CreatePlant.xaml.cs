using CardUITest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Lottie.Forms;

namespace CardUITest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreatePlant : ContentPage
    {
        public CreatePlant()
        {
            InitializeComponent();
        }

        async private void saveButton_Clicked(object sender, EventArgs e)
        {
            Plant plant = new Plant()
            {
                PlantName = plantName.Text,
                PlantType = plantType.Text,
                PlantColor = plantColor.Text,
            };

            // Inserting into the database
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Plant>();
                int rowsAdded = conn.Insert(plant);

            }
            await Navigation.PopModalAsync();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

    
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CardUITest.Objects.CurseWordFilter;
using CardUITest.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SQLite;
using System.Text.RegularExpressions;

namespace CardUITest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlantDetailsSection : ContentView
    {
        int selectionIndex = 0;
        List<Label> tabHeaders = new List<Label>();
        List<VisualElement> tabContents = new List<VisualElement>();
        //private List<Note> plantNotes = new List<Note>();
        private Plant _viewModel;

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            _viewModel = this.BindingContext as Plant;
        }

        public PlantDetailsSection()
        {
            InitializeComponent();

            tabHeaders.Add(Sun);
            tabHeaders.Add(Water);
            tabHeaders.Add(Notes);

            tabContents.Add(SunContent);
            tabContents.Add(WaterContent);
            tabContents.Add(NotesContent);
        }

        private async Task ShowSelection(int newTab)
        {
            // Don't do anything if the same tab is selected
            if (newTab == selectionIndex) return;

            // navigate the selection
            var selectedTab = tabHeaders[newTab];
            _ = SelectionUnderline.TranslateTo(selectedTab.Bounds.X, 0, 300, easing: Easing.SinInOut);

            // update the style of the header to show it's selcted
            var unselectedStyle = (Style)Application.Current.Resources["TabLabel"];
            var selectedStyle = (Style)Application.Current.Resources["SelectedTabLabel"];
            tabHeaders[selectionIndex].Style = unselectedStyle;
            selectedTab.Style = selectedStyle;

            // Reveal the contents
            await tabContents[selectionIndex].FadeTo(0);
            tabContents[selectionIndex].IsVisible = false;
            tabContents[newTab].IsVisible = true;
            _ = tabContents[newTab].FadeTo(1);

            selectionIndex = newTab;

            GetPlantNotes(_viewModel.Id);
        }

        async private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var tabIndex = tabHeaders.IndexOf((Label)sender);
            await ShowSelection(tabIndex);
        }

        private void GetPlantNotes(int id)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                // Declaring a list for all the notes
                List<Note> plantNotes = new List<Note>();

                conn.CreateTable<Note>();

                // Fetching all notes and pushing notes relevant to the specific plant into the list
                var notes = conn.Table<Note>().ToList();

                foreach (var pNote in notes)
                {
                    if (pNote.PlantId == id)
                    {
                        plantNotes.Add(pNote);
                    }
                }

                //Setting Item Source for the list view

                notesList.ItemsSource = plantNotes;
                noteBody.Text = "";
            }
        }

        private void UpdatePlantNotes(Note note, Plant plant)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                // Connecting to the DB and adding the note 
                conn.CreateTable<Note>();
                int rowsAdded = conn.Insert(note);

                // After adding the the note update the list
                GetPlantNotes(plant.Id);

                // Updating plant health according to note
                if(note.IsNegative == true)
                {
                    // Connecting to the plant DB
                    conn.CreateTable<Plant>();
                    int addedHealth = 10;

                    var updatePlant = new Plant();
                    updatePlant.Id = plant.Id;
                    updatePlant.PlantName = plant.PlantName;
                    updatePlant.PlantColor = plant.PlantColor;
                    updatePlant.PlantType = plant.PlantType;
                    updatePlant.Image = plant.Image;
                    updatePlant.Health = plant.Health + addedHealth;
                    updatePlant.Level = plant.Level;
                    updatePlant.Experience = plant.Experience;

                    conn.Update(updatePlant);
                }
            }
        }

        void saveNote_Clicked(System.Object sender, System.EventArgs e)
        {

            // Check for banned words in notebody
            var filteredNote = FilterBannedWords(noteBody.Text);
            bool isNegative;

            if (filteredNote.Contains("*"))
            {
                isNegative = true;
            }
            else
            {
                isNegative = false;
            }

            Note note = new Note()
            {
                NoteBody = filteredNote,
                CreatedAt = DateTime.Now,
                IsNegative = isNegative,
                PlantId = _viewModel.Id
            };

            UpdatePlantNotes(note, _viewModel);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CardUITest.Objects.CurseWordFilter;
using static CardUITest.Objects.Level;
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
        private Plant _viewModel;
        private DateTime todaysDate;

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            _viewModel = this.BindingContext as Plant;
        }

        public PlantDetailsSection()
        {
            InitializeComponent();

            todaysDate = DateTime.Today;

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

                    // variables to update plant health and 
                    int subtractedHealth = 15;

                    if (_viewModel.Health <= 10)
                    {
                        subtractedHealth = _viewModel.Health;
                    }

                    // All the negative notes for the plant are pulled 
                    // If the date is LESS than the (PREVIOUS DATE + 30 mins) update the counter
                    // If the counter is greater than 4 ( That means 4 notes in quick succession ) it has an even worse outcome on the plants health
                    var notes = conn.Table<Note>().ToList();
                    var counter = 0;
                    foreach (var pNote in notes)
                    {
                        DateTime previousDate = pNote.CreatedAt;
                        if (pNote.CreatedAt >= todaysDate && pNote.CreatedAt <= previousDate.AddMinutes(5) && pNote.PlantId == _viewModel.Id)
                        {
                            counter++;
                            if (counter > 4)
                            {
                                _viewModel.NoteMessage = "You're plant can't take it anymore, stop it!";
                                subtractedHealth = 30;
                            }
                        }
                    }

                    _viewModel.Id = plant.Id;
                    _viewModel.PlantName = plant.PlantName;
                    _viewModel.PlantColor = plant.PlantColor;
                    _viewModel.PlantType = plant.PlantType;
                    _viewModel.Image = plant.Image;
                    _viewModel.Health = plant.Health - subtractedHealth;
                    _viewModel.Level = GetLevelFromXp(plant.Experience);
                    _viewModel.Experience = plant.Experience;

                    conn.Update(_viewModel);
                }

                if (note.IsNegative == false)
                {
                    // Connecting to the plant DB
                    conn.CreateTable<Plant>();

                    // variables to update plant health and xp
                    int addedXp = 1000;
                    int addedHealth = 15;

                    if (_viewModel.Health >= 90)
                    {
                        // Calculating the remainding health and adding to get the plant to 100% health
                        addedHealth = 100 - _viewModel.Health;
                    }

                    // All the positive notes for the plant are pulled 
                    // If the date is LESS than the (PREVIOUS DATE + 30 mins) update the counter
                    // If the counter is greater than 4 ( That means 4 notes in quick succession ) it has a negative outcome on the plants health
                    var notes = conn.Table<Note>().ToList();
                    var counter = 0;
                    foreach (var pNote in notes)
                    {
                        DateTime previousDate = pNote.CreatedAt;
                        if (pNote.CreatedAt >= todaysDate && pNote.CreatedAt <= previousDate.AddMinutes(5) && pNote.PlantId == _viewModel.Id)
                        {
                            counter++;
                            if (counter > 4)
                            {
                                _viewModel.NoteMessage = "You're plant doesn't trust you anymore, you've said too many nice things";
                                addedHealth = -10;
                            }
                        }
                    }


                    _viewModel.Id = plant.Id;
                    _viewModel.PlantName = plant.PlantName;
                    _viewModel.PlantColor = plant.PlantColor;
                    _viewModel.PlantType = plant.PlantType;
                    _viewModel.Image = plant.Image;
                    _viewModel.Health = plant.Health + addedHealth;
                    _viewModel.Level = GetLevelFromXp((plant.Experience + addedXp));
                    _viewModel.Experience = plant.Experience + addedXp;

                    conn.Update(_viewModel);

                    //MessagingCenter.Send(this, "updatedViewModel", _viewModel);
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

        private void WaterButton_Clicked(object sender, EventArgs e)
        {
            Water water = new Water()
            {
                CreatedAt = DateTime.Now,
                PlantId = _viewModel.Id
            };
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Water>();
                int rowsAdded = conn.Insert(water);

                int addedXp = 1000;
                int addedHealth = 15;

                if (_viewModel.Health >= 90)
                {
                    // Calculating the remainding health and adding to get the plant to 100% health
                    addedHealth = 100 - _viewModel.Health;
                }
                else if (_viewModel.Health <= 10)
                {
                    addedHealth = -_viewModel.Health;
                }

                var waterList = conn.Table<Water>().ToList();
                var counter = 0;
                foreach (var waterObj in waterList)
                {
                    DateTime previousDate = waterObj.CreatedAt;
                    if (waterObj.CreatedAt >= todaysDate && waterObj.CreatedAt <= previousDate.AddMinutes(5) && waterObj.PlantId == _viewModel.Id)
                    {
                        counter++;
                        if (counter > 4)
                        {
                            _viewModel.WaterMessage = "Slow down! You're drowning it.";
                            addedHealth = -10;
                        }
                    }
                }

                _viewModel.Id = _viewModel.Id;
                _viewModel.PlantName = _viewModel.PlantName;
                _viewModel.PlantColor = _viewModel.PlantColor;
                _viewModel.PlantType = _viewModel.PlantType;
                _viewModel.Image = _viewModel.Image;
                _viewModel.Health = _viewModel.Health + addedHealth;
                _viewModel.Level = GetLevelFromXp((_viewModel.Experience + addedXp));
                _viewModel.Experience = _viewModel.Experience + addedXp;

                conn.Update(_viewModel);
            }
        }

        private void SunlightButton_Clicked(object sender, EventArgs e)
        {
            Sunlight sunlight = new Sunlight()
            {
                CreatedAt = DateTime.Now,
                PlantId = _viewModel.Id
            };
            using (SQLiteConnection conn = new SQLiteConnection(App.FilePath))
            {
                conn.CreateTable<Sunlight>();
                int rowsAdded = conn.Insert(sunlight);

                int addedXp = 1000;
                int addedHealth = 15;

                if (_viewModel.Health >= 90)
                {
                    // Calculating the remainding health and adding to get the plant to 100% health
                    addedHealth = 100 - _viewModel.Health;
                }
                else if (_viewModel.Health <= 10)
                {
                    addedHealth = -_viewModel.Health;
                }

                var sunlightList = conn.Table<Sunlight>().ToList();
                var counter = 0;
                foreach (var sun in sunlightList)
                {
                    DateTime previousDate = sun.CreatedAt;
                    if (sun.CreatedAt >= todaysDate && sun.CreatedAt <= previousDate.AddMinutes(5) && sun.PlantId == _viewModel.Id)
                    {
                        counter++;
                        if (counter > 4)
                        {
                            _viewModel.SunMessage = "Help! I'm burning";
                            addedHealth = -10;
                        }
                    }
                }

                _viewModel.Id = _viewModel.Id;
                _viewModel.PlantName = _viewModel.PlantName;
                _viewModel.PlantColor = _viewModel.PlantColor;
                _viewModel.PlantType = _viewModel.PlantType;
                _viewModel.Image = _viewModel.Image;
                _viewModel.Health = _viewModel.Health + addedHealth;
                _viewModel.Level = GetLevelFromXp((_viewModel.Experience + addedXp));
                _viewModel.Experience = _viewModel.Experience + addedXp;

                conn.Update(_viewModel);
            }
        }
    }
}
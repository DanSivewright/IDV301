using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CardUITest.ViewModels
{
    public class CardViewModel
    {

        ObservableCollection<Student> _students;
        public ObservableCollection<Student> Students
        {
            get
            {
                return _students
            }
            set
            {
                _students = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Api Calls and Data Handling
        private async void GetStudents()
        {
            using (var client = new HttpClient())
            {
                // Sending a GET req
                var uri = "https://localhost:44377/Student";
                var res = await client.GetStringAsync(uri);

                // Handling the res
                var StudnetList = JsonConvert.DeserializeObject<List<Student>>(res);

                Students = new ObservableCollection<Student>(StudnetList);
            }
        }


        public ObservableCollection<Plant> Plants { get; set; }
        public CardViewModel()
        {
            Plants = new ObservableCollection<Plant>()
            {
                new Plant()
                {
                     PlantName = "aloe vera",
                     PlantType = "succulent",
                     Image = "aloe_vera.png",
                     PlantColor = "#759966"
                },
                new Plant()
                {
                     PlantName = "test",
                     PlantType = "columbine",
                     Image = "aquilegia.png",
                     PlantColor = "#9c51b6"
                },
                new Plant()
                {
                     PlantName = "ella plant",
                     PlantType = "Poeplhole",
                     Image = "aquilegia.png",
                     PlantColor = "#59ABE3"
                },
                new Plant()
                {
                     PlantName = "kieran plant",
                     PlantType = "Columbine",
                     Image = "aquilegia.png",
                     PlantColor = "#1f1f1f"
                }
            };
        }
    }

    public class Plant
    {
        public string PlantName { get; set; }
        public string PlantType { get; set; }
        public string Image { get; set; }
        public string PlantColor { get; set; }
    }

    public class Student
    {
        public Object _id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string emailAddress { get; set; }
    }
}

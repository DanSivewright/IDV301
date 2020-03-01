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
    public class StudentViewModel
    {
        ObservableCollection<Student> _students;
        public ObservableCollection<Student> Students
        {
            get
            {
                return _students;
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
        

    }
    public class Student
    {
        public Object _id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string emailAddress { get; set; }
    }
}

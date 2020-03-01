using CardUITest.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CardUITest.ViewModels
{
    public class StudentViewModel
    {

        ApiServices _api = new ApiServices();

        public Object _id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string emailAddress { get; set; }
        public string Message { get; set; }


        public ICommand CreateStudentCommand
        {
            get
            {
                return new Command(async () =>
                {
                    var req = await _api.CreateAsync(firstName, lastName, emailAddress);
                    if (req)
                    {
                        Message = "Command Created";
                    }
                    else
                    {
                        Message = "Failed To Create Command";
                    }
                });
            }
        }
    }
}

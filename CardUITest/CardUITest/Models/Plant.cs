using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardUITest.Models
{
    public class Plant
    {
        [PrimaryKey]
        public int id { get; set; }
        public string PlantName { get; set; }
        public string PlantType { get; set; }
        public string Image { get; set; }
        public string PlantColor { get; set; }
    }
}

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
        public Plant()
        {
            Image = "aquilegia.png";
        }
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string PlantName { get; set; }
        public string PlantType { get; set; }
        public string Image { get; set; } 

        [MaxLength(6)]
        public string PlantColor { get; set; }
    }
}

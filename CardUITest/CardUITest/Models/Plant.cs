using SQLite;
using SQLiteNetExtensions.Attributes;
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
            Health = 50;
            Level = 0;
        }
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string PlantName { get; set; }
        public string PlantType { get; set; }
        public string Image { get; set; } 

        [MaxLength(6)]
        public string PlantColor { get; set; }
        public int Health { get; set; }
        public double Level { get; set; }
        public int Experience { get; set; }

        public string WaterMessage { get; set; }
        public string SunMessage { get; set; }
        public string NoteMessage { get; set; }

    }
}

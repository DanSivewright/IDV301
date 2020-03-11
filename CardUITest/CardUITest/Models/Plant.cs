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
            Health = 100;
            Level = 0;
        }
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string PlantName { get; set; }
        public string PlantType { get; set; }
        public string Image { get; set; } 

        [MaxLength(6)]
        public string PlantColor { get; set; }
        public int Health { get; set; }
        public int Level { get; set; }
        [OneToMany]
        public List<Note> Notes { get; set; }
    }
}

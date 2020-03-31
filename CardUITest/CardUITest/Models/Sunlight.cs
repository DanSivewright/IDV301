using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CardUITest.Models
{
    class Sunlight
    {
        [PrimaryKey, AutoIncrement]

        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public int PlantId { get; set; }
    }
}

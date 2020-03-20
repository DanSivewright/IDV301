using System;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace CardUITest.Models
{
    public class Note
    {
        public Note()
        {
            IsNegative = false;
        }
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string NoteBody { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsNegative { get; set; }
        public int PlantId { get; set; }
    }
}

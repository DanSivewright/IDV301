using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardUITest.ViewModels
{
    public class CardViewModel
    {
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
}

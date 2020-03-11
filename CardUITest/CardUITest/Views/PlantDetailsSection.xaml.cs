using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CardUITest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlantDetailsSection : ContentView
    {
        int selectionIndex = 0;
        List<Label> tabHeaders = new List<Label>();

        public PlantDetailsSection()
        {
            InitializeComponent();

            tabHeaders.Add(Sun);
            tabHeaders.Add(Water);
            tabHeaders.Add(Notes);
        }


        private void ShowSelection(int newTab)
        {
            // Don't do anything if the same tab is selected
            if (newTab == selectionIndex) return;

            // navigate the selection
            var selectedTab = tabHeaders[newTab];
            _ = SelectionUnderline.TranslateTo(selectedTab.Bounds.X, 0, 300, easing: Easing.SinInOut);

            // update the style of the header to show it's selcted
            var unselectedStyle = (Style)Application.Current.Resources["TabLabel"];
            var selectedStyle = (Style)Application.Current.Resources["SelectedTabLabel"];
            tabHeaders[selectionIndex].Style = unselectedStyle;
            selectedTab.Style = selectedStyle;

            // Reveal the contents

            selectionIndex = newTab;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var tabIndex = tabHeaders.IndexOf((Label)sender);
            ShowSelection(tabIndex);
        }
    }
}
using TD2.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TD2.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddPlaceView : ContentPage
    {
        public AddPlaceView()
        {
            InitializeComponent();
            BindingContext = new AddPlaceViewModel();
        }
    }
}
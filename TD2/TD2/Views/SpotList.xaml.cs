using TD2.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TD2
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SpotList : ContentPage
    {
        public SpotList()
        {
            BindingContext = new SpotListViewModel();
            InitializeComponent();
        }

        protected override bool OnBackButtonPressed()
        {
            ((SpotListViewModel)BindingContext).Disconnect();
            return true;
        }
    }
}
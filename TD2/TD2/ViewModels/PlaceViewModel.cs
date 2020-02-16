using Storm.Mvvm;
using System.Collections.Generic;
using TD2.Items;

namespace TD2.ViewModels
{
    class PlaceViewModel : ViewModelBase
    {
        private PlaceItem _placeItem;
        public PlaceItem PlaceItem
        {
            get => _placeItem;
            set
            {
                if (SetProperty(ref _placeItem, value) && value != null)
                {
                    _placeItem = value;
                }
            }
        }

        public override void Initialize(Dictionary<string, object> navigationParameters)
        {
            base.Initialize(navigationParameters);
            PlaceItem = GetNavigationParameter<PlaceItem>("PlaceItem");
        }
    }
}

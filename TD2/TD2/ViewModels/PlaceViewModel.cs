using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using TD2.Items;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TD2.ViewModels
{
    class PlaceViewModel : ViewModelBase
    {
        [NavigationParameter("placeItem")]
        public PlaceItem placeItem { get; set; }

        private int _Id;
        public int ID
        {
            get => _Id;
            set => SetProperty(ref _Id, value);
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private int _ImageID;
        public int ImageID
        {
            get => _ImageID;
            set => SetProperty(ref _ImageID, value);
        }

        private double _Latitude;
        public double Latitude
        {
            get => _Latitude;
            set => SetProperty(ref _Latitude, value);
        }

        private double _Longitude;
        public double Longitude
        {
            get => _Longitude;
            set => SetProperty(ref _Longitude, value);
        }

        private List<CommentItem> _Comments;
        public List<CommentItem> Comments
        {
            get => _Comments;
            set => SetProperty(ref _Comments, value);
        }

        public ICommand openMaps { get; }

        public PlaceViewModel()
        {
            openMaps = new Command(goToMaps);
        }

        private async void goToMaps(object obj)
        {
            Location location = new Location(Latitude, Longitude);
            MapLaunchOptions options = new MapLaunchOptions { Name = Title};
            await Map.OpenAsync(location, options);
        }

        public override void Initialize(Dictionary<string, object> navigationParameters)
        {
            base.Initialize(navigationParameters);
            if (placeItem != null)
            {
                ID = placeItem.Id;
                Title = placeItem.Title;
                Description = placeItem.Description;
                ImageID = placeItem.ImageId;
                Longitude = placeItem.Longitude;
                Latitude = placeItem.Latitude;
                Comments = placeItem.Comments;
            }
            else
            {
                Title = "Error";
            }
        }
    }
}

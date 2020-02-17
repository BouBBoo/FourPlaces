using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using Storm.Mvvm.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using TD2.API;
using TD2.Items;
using TD2.Views;
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

        private ObservableCollection<CommentItem> _Comments;
        public ObservableCollection<CommentItem> Comments
        {
            get => _Comments;
            set => SetProperty(ref _Comments, value);
        }

        private AddCommentView addCommentView;

        public ICommand openMaps { get; }
        public ICommand createComment { get; }

        public PlaceViewModel()
        {
            _Comments = new ObservableCollection<CommentItem>();
            openMaps = new Command(goToMaps);
            createComment = new Command(CreateComment);
        }

        private async Task<UserItem> getUserItem()
        {
            try
            {
                ApiClient apiClient = new ApiClient();
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://td-api.julienmialon.com/me");
                string token = ((LoginResult)Application.Current.Properties["token"]).AccessToken;
                request.Headers.Add("Authorization", $"Bearer {token}");
                HttpResponseMessage httpResponse = await client.SendAsync(request);
                Response<UserItem> response = await apiClient.ReadFromResponse<Response<UserItem>>(httpResponse);
                if (response.IsSuccess)
                {
                    return new UserItem(){ LastName = response.Data.LastName,
                            Email = response.Data.Email,
                            FirstName = response.Data.FirstName,
                            Id = response.Data.Id,
                            ImageId = response.Data.ImageId == null ? 1 : response.Data.ImageId};
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", e.Message, "ok");
            }
            return null;
        }

        private async void CreateComment()
        {
            addCommentView = new AddCommentView();
            await DependencyService.Get<INavigationService>().PushAsync(addCommentView, new Dictionary<string, object> {
                {"PlaceId", ID }
            });
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
                Comments = new ObservableCollection<CommentItem>(placeItem.Comments);
            }
            else
            {
                Title = "Error";
            }
        }
    }
}

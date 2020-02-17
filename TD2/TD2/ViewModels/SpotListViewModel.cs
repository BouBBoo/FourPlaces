using Newtonsoft.Json;
using Storm.Mvvm;
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
using Xamarin.Forms;

namespace TD2.ViewModels
{
    class SpotListViewModel : ViewModelBase
    {
        private readonly string URL = "https://td-api.julienmialon.com";
        public ICommand goToDetail { get; }
        public ICommand goToAddNewPlace { get; }
        public ICommand goToProfile { get; }
        public ICommand disconnect { get; }
        private ObservableCollection<PlaceItemSummary> _listPlaces;
        public ObservableCollection<PlaceItemSummary> ListPlaces
        {
            get => _listPlaces;
            set
            {
                if (SetProperty(ref _listPlaces, value) && value != null)
                {
                    _listPlaces = value;
                    OnPropertyChanged();
                }
            }
        }

        private ApiClient apiClient;
        public SpotListViewModel()
        {
            ListPlaces = new ObservableCollection<PlaceItemSummary>();
            goToDetail = new Command<int>(GoToDetailPage);
            goToAddNewPlace = new Command(GoToAddNewPlace);
            goToProfile = new Command(GoToProfile);
            disconnect = new Command(Disconnect);
            GetPlaces();
        }

        public async void Disconnect()
        {
            bool answer=  await Application.Current.MainPage.DisplayAlert("Disconnect?", "Do you really want to leave?", "Yes", "No");
            if (answer)
            {
                Application.Current.Properties["token"] = null;
                await DependencyService.Get<INavigationService>().PopAsync();
            }
        }


        private async void GoToProfile()
        {
            try
            {
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://td-api.julienmialon.com/me");
                string token = ((LoginResult)Application.Current.Properties["token"]).AccessToken;
                request.Headers.Add("Authorization", $"Bearer {token}");
                HttpResponseMessage httpResponse = await client.SendAsync(request);
                Response<UserItem> response = await apiClient.ReadFromResponse<Response<UserItem>>(httpResponse);
                if (response.IsSuccess)
                {
                    Debug.Write(response.Data.Email);
                    await DependencyService.Get<INavigationService>().PushAsync<ProfilView>(new Dictionary<string, object> {
                        { "userItem" , new UserItem(){ LastName = response.Data.LastName, 
                            Email = response.Data.Email,
                            FirstName = response.Data.FirstName,
                            Id = response.Data.Id,
                            ImageId = response.Data.ImageId == null ? 1 : response.Data.ImageId} }
                    });
                }
                else
                {
                    Debug.Write("Erreur " + response.ErrorMessage);
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", e.Message, "ok");
            }
        }

        private async void GoToAddNewPlace(object obj)
        {
            await DependencyService.Get<INavigationService>().PushAsync<AddPlaceView>();
        }

        public async void GoToDetailPage(int obj)
        {
            try
            {
                HttpResponseMessage httpResponse = await apiClient.Execute(HttpMethod.Get, URL + "/places/" + obj);
                Response<PlaceItem> response = await apiClient.ReadFromResponse<Response<PlaceItem>>(httpResponse);
                if (response.IsSuccess)
                {
                    await DependencyService.Get<INavigationService>().PushAsync<PlaceView>(new Dictionary<string, object> {
                        { "placeItem" , response.Data }
                    });
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", e.Message, "ok");
            }
            
        }

        public async void GetPlaces()
        {
            apiClient = new ApiClient();
            try
            {
                HttpResponseMessage httpResponse = await apiClient.Execute(HttpMethod.Get, URL + "/places");
                Response <ObservableCollection<PlaceItemSummary>> response = await apiClient.ReadFromResponse<Response<ObservableCollection<PlaceItemSummary>>>(httpResponse);
                if (response.IsSuccess)
                {
                    ListPlaces = response.Data;
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Erreur", e.Message, "ok");
            }
        }
        public async Task<string> GetImagePath(int id)
        {
            ApiClient apiClient = new ApiClient();
            HttpResponseMessage httpResponse = await apiClient.Execute(HttpMethod.Get, "https://td-api.julienmialon.com/images/" + id);
            Response<string> response = await apiClient.ReadFromResponse<Response<string>>(httpResponse);
            if (response.IsSuccess)
            {
                return response.Data;
            }
            else
            {
                return null;
            }
        }
    }
}

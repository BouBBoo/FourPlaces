﻿using Storm.Mvvm;
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
            GetPlaces();
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

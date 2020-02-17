using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using Storm.Mvvm.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using TD2.API;
using TD2.API.Request;
using TD2.Items;
using TD2.Views;
using Xamarin.Forms;

namespace TD2.ViewModels
{
    class AddCommentViewModel : ViewModelBase
    {
        private string _text;
        public string CommentText
        {
            get => _text;
            set
            {
                if(SetProperty(ref _text, value) && value != null) { }
            }
        }
        public bool IsSubmitted { get; set; }

        [NavigationParameter("PlaceId")]
        public int PlaceId { get; set; }

        public ICommand submitComment { get; }
        public AddCommentViewModel()
        {
            submitComment = new Command(SubmitComment);
            IsSubmitted = false;
        }

        private async void SubmitComment()
        {
            DateTime date = DateTime.Now;
            CreateCommentRequest commentItem = new CreateCommentRequest()
            {
                Text = _text
            };
            string token = ((LoginResult)Application.Current.Properties["token"]).AccessToken;
            ApiClient apiClient = new ApiClient();
            HttpResponseMessage httpResponse = await apiClient.Execute(HttpMethod.Post, "https://td-api.julienmialon.com/places/" + PlaceId + "/comments"
                , commentItem, token);
            Response<UserItem> response = await apiClient.ReadFromResponse<Response<UserItem>>(httpResponse);
            if (!response.IsSuccess)
            {
                if (response.ErrorMessage.Contains("PARAMETERS")){
                    await Application.Current.MainPage.DisplayAlert("Erreur", "There is no comment", "OK");
                } 
            }
            else 
            {
                IsSubmitted = true;
                await DependencyService.Get<INavigationService>().PopAsync();
            }

        }

        public override void Initialize(Dictionary<string, object> navigationParameters)
        {
            base.Initialize(navigationParameters);
        }
    }
}

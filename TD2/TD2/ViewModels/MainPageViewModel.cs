using Storm.Mvvm;
using Storm.Mvvm.Services;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Windows.Input;
using TD2.API;
using Xamarin.Forms;
using System.Diagnostics;

namespace TD2
{
    class MainPageViewModel : ViewModelBase
    {
        public ICommand goToCreation { get; }
        public ICommand goToLogin { get; }

        private string URL = "https://td-api.julienmialon.com/auth/login";
        private string _login;
        public string LOGIN {
            get => _login;
            set
            {
                if (SetProperty(ref _login, value) && value != null)
                {
                   
                }
            }
        }
        public string _password;
        public string PASSWORD
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value) && value != null)
                {
                    
                }
            }
        }
        public MainPageViewModel()
        {
            goToCreation = new Command(GoToAccountCreation);
            goToLogin = new Command(GoToLogin);
        }

        private async void GoToAccountCreation()
        {
            await DependencyService.Get<INavigationService>().PushAsync<AccountCreation>();
        }

        private async void GoToLogin()
        {
            if(LOGIN == " " || LOGIN == null) { await Application.Current.MainPage.DisplayAlert("Erreur", "Login non renseigné", "ok"); }
            else if (PASSWORD == " " || PASSWORD == null) { await Application.Current.MainPage.DisplayAlert("Erreur", "Mot de passe non renseigné", "ok"); }
            else
            {
                try
                {
                    ApiClient apiClient = new ApiClient();
                    HttpResponseMessage httpResponse = await apiClient.Execute(HttpMethod.Post, URL, new LoginRequest() { Email = _login, Password = _password });
                    Response<LoginResult> response = await apiClient.ReadFromResponse<Response<LoginResult>>(httpResponse);
                    if (response.IsSuccess)
                    {
                        Application.Current.Properties["token"] = response.Data;
                        await DependencyService.Get<INavigationService>().PushAsync(new SpotList());
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Erreur", response.ErrorMessage, "ok");
                    }
                }
                catch (Exception e)
                {
                    await Application.Current.MainPage.DisplayAlert("Erreur", e.Message, "ok");
                }
            }
        }
    }
}
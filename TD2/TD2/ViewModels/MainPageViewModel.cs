using Storm.Mvvm;
using Storm.Mvvm.Services;
using System;
using System.Net.Http;
using System.Windows.Input;
using TD2.API;
using Xamarin.Forms;

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
            if(LOGIN == " " || LOGIN == null) { await Application.Current.MainPage.DisplayAlert("Erreur", "Login not typed", "ok"); }
            else if (PASSWORD == " " || PASSWORD == null) { await Application.Current.MainPage.DisplayAlert("Erreur", "Password not typed", "ok"); }
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
                        LOGIN = "";
                        PASSWORD = "";
                    }
                    else
                    {
                        if (response.ErrorMessage.Contains("Credentials"))
                        {
                            await Application.Current.MainPage.DisplayAlert("Erreur", "Wrong login or password", "ok");
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Erreur", response.ErrorMessage, "ok");
                        }
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
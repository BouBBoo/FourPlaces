using Storm.Mvvm;
using Storm.Mvvm.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using TD2.API;
using Xamarin.Forms;

namespace TD2.ViewModels
{
    class AccountCreationViewModel : ViewModelBase
    {
        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                if(SetProperty(ref _email, value) && value != null) { }
            }
        }

        private string _lastname;
        public string LastName
        {
            get => _lastname;
            set
            {
                if (SetProperty(ref _lastname, value) && value != null) { }
            }
        }

        private string _firstname;
        public string FirstName
        {
            get => _firstname;
            set
            {
                if (SetProperty(ref _firstname, value) && value != null) { }
            }
        }

        private string _password;

        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value) && value != null) { }
            }
        }

        public ICommand submit { get; }

        public AccountCreationViewModel()
        {
            submit = new Command(Submit);
        }

        private async void Submit(object obj)
        {
            RegisterRequest registerRequest = new RegisterRequest()
            {
                Email = Email,
                FirstName = FirstName,
                LastName = LastName,
                Password = Password
            };
            ApiClient apiClient = new ApiClient();
            Debug.WriteLine(Password + " " + FirstName + " " + LastName + " " + Email);
            try
            {

                HttpResponseMessage httpResponse = 
                    await apiClient.Execute(HttpMethod.Post, "https://td-api.julienmialon.com/auth/register", registerRequest);
                Response<LoginResult> response = await apiClient.ReadFromResponse<Response<LoginResult>>(httpResponse);
                if (response.IsSuccess)
                {
                    Application.Current.Properties["token"] = response.Data;
                    await DependencyService.Get<INavigationService>().PopAsync();
                    await Application.Current.MainPage.DisplayAlert("Compte créé", "La création du compte a été effectué", "Ok");
                    await DependencyService.Get<INavigationService>().PushAsync<SpotList>();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Erreur response", response.ErrorMessage, "ok");
                }
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Erreur request", e.Message, "ok");
            }

        }
    }
}

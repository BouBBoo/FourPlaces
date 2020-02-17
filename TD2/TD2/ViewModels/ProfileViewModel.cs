using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TD2.API;
using TD2.API.Request;
using TD2.Items;
using TD2.Views;
using Xamarin.Forms;

namespace TD2.ViewModels
{
    class ProfileViewModel : ViewModelBase
    {
        [NavigationParameter("userItem")]
        public UserItem userItem { get; set; }

        public ProfilView profileView { get; set; }

        private string _firstname;
        public string FirstName
        {
            get => _firstname;
            set
            {
                if(SetProperty(ref _firstname, value) && value != null) { }
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

        private string _email;
        public string Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value) && value != null) { }
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

        private string _newPassword;
        public string NewPassword
        {
            get => _newPassword;
            set
            {
                if (SetProperty(ref _newPassword, value) && value != null) { }
            }
        }

        private int _imageId;
        public int ImageId
        {
            get => _imageId;
            set
            {
                if (SetProperty(ref _imageId, value)) {  }
            }
        }

        public ICommand update { get; }

        private async void Update()
        {
            try
            {
                ApiClient apiClient = new ApiClient();
                if (!userItem.LastName.Equals(LastName) || !userItem.FirstName.Equals(FirstName) || !userItem.ImageId.Equals(ImageId))
                {
                    UpdateProfileRequest updateProfileRequest = new UpdateProfileRequest();
                    updateProfileRequest.FirstName = FirstName;
                    updateProfileRequest.LastName = LastName;
                    updateProfileRequest.ImageId = ImageId;

                    HttpResponseMessage httpResponse = await apiClient.Execute(new HttpMethod("PATCH"), "https://td-api.julienmialon.com/me", updateProfileRequest, ((LoginResult)Application.Current.Properties["token"]).AccessToken);
                    Response<UserItem> response = await apiClient.ReadFromResponse<Response<UserItem>>(httpResponse);
                    if (NewPassword != null)
                    {
                        UpdatePasswordRequest updatePasswordRequest = new UpdatePasswordRequest();
                        updatePasswordRequest.OldPassword = Password;
                        updatePasswordRequest.NewPassword = NewPassword;
                        HttpResponseMessage httpResponse1 = await apiClient.Execute(new HttpMethod("PATCH"), "https://td-api.julienmialon.com/me/password", updatePasswordRequest, ((LoginResult)Application.Current.Properties["token"]).AccessToken);
                        if (httpResponse1.IsSuccessStatusCode)
                        {
                            Edit();
                            await Application.Current.MainPage.DisplayAlert("Mise à jour réussie", "La mise à jour a été effectué avec succès.", "OK");
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Erreur lors de la mise à jour", "Mise à jour mot de passe non effectué.", "OK");
                        }
                    }
                    else
                    {
                        if (response.IsSuccess)
                        {
                            Edit();
                            await Application.Current.MainPage.DisplayAlert("Mise à jour réussie", "La mise à jour a été effectué avec succès.", "OK");
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Erreur lors de la mise à jour", response.ErrorMessage, "OK");
                        }
                    }
                }
                else if (NewPassword != null)
                {
                    UpdatePasswordRequest updatePasswordRequest = new UpdatePasswordRequest();
                    updatePasswordRequest.OldPassword = Password;
                    updatePasswordRequest.NewPassword = NewPassword;
                    HttpResponseMessage httpResponse1 = await apiClient.Execute(new HttpMethod("PATCH"), "https://td-api.julienmialon.com/me/password", updatePasswordRequest, ((LoginResult)Application.Current.Properties["token"]).AccessToken);
                    if (httpResponse1.IsSuccessStatusCode)
                    {
                        Edit();
                        await Application.Current.MainPage.DisplayAlert("Mise à jour réussie", "La mise à jour a été effectué avec succès.", "OK");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Erreur lors de la mise à jour", "Mise à jour mot de passe non effectué.", "OK");
                    }
                }
                
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Error request", e.Message, "OK");
            }
        }

        public ICommand edit { get; }
        

        private void Edit()
        {
            ((Entry)profileView.FindByName("EmailEntry")).IsEnabled = !((Entry)profileView.FindByName("EmailEntry")).IsEnabled;
            ((Entry)profileView.FindByName("FirstNameEntry")).IsEnabled = !((Entry)profileView.FindByName("FirstNameEntry")).IsEnabled;
            ((Entry)profileView.FindByName("LastNameEntry")).IsEnabled = !((Entry)profileView.FindByName("LastNameEntry")).IsEnabled;
            ((StackLayout)profileView.FindByName("PasswordEntry")).IsVisible = !((StackLayout)profileView.FindByName("PasswordEntry")).IsVisible;
            ((StackLayout)profileView.FindByName("NewPasswordEntry")).IsVisible = !((StackLayout)profileView.FindByName("NewPasswordEntry")).IsVisible;
            ((Button)profileView.FindByName("Button")).IsVisible = !((Button)profileView.FindByName("Button")).IsVisible;
        }

       

        public ProfileViewModel()
        {
            update = new Command(Update);
            edit = new Command(Edit);
            
        }

        public override void Initialize(Dictionary<string, object> navigationParameters)
        {
            base.Initialize(navigationParameters);
            if (userItem != null)
            {
                FirstName = userItem.FirstName;
                LastName = userItem.LastName;
                ImageId = (int)userItem.ImageId;
                Email = userItem.Email;
            }
        }
    }
}

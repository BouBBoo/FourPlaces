using Plugin.Media;
using Plugin.Media.Abstractions;
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
                            RefreshRequest refreshRequest = new RefreshRequest();
                            refreshRequest.RefreshToken = ((LoginResult)Application.Current.Properties["token"]).RefreshToken;
                            HttpResponseMessage httpResponse2 = await apiClient.Execute(new HttpMethod("POST"), "https://td-api.julienmialon.com/auth/refresh", refreshRequest);
                            Response<LoginResult> response1 = await apiClient.ReadFromResponse<Response<LoginResult>>(httpResponse);
                            if (response1.IsSuccess)
                            {
                                Application.Current.Properties["token"] = response1.Data;
                                await Application.Current.MainPage.DisplayAlert("Mise à jour réussie", "La mise à jour a été effectué avec succès.", "OK");
                            }
                            else
                            {
                                await Application.Current.MainPage.DisplayAlert("Erreur lors de la mise à jour", "Mise à jour mot de passe non effectué.", "OK");
                            }
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
                        RefreshRequest refreshRequest = new RefreshRequest();
                        refreshRequest.RefreshToken = ((LoginResult)Application.Current.Properties["token"]).RefreshToken;
                        HttpResponseMessage httpResponse2 = await apiClient.Execute(new HttpMethod("POST"), "https://td-api.julienmialon.com/auth/refresh", refreshRequest);
                        Response<LoginResult> response1 = await apiClient.ReadFromResponse<Response<LoginResult>>(httpResponse2);
                        if (response1.IsSuccess)
                        {
                            Application.Current.Properties["token"] = response1.Data;
                            await Application.Current.MainPage.DisplayAlert("Mise à jour réussie", "La mise à jour a été effectué avec succès.", "OK");
                        }
                        else
                        {
                            await Application.Current.MainPage.DisplayAlert("Erreur lors de la mise à jour", "Mise à jour mot de passe non effectué.", "OK");
                        }
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
        public ICommand loadImage { get; }



        private void Edit()
        {
            ((Entry)profileView.FindByName("EmailEntry")).IsEnabled = !((Entry)profileView.FindByName("EmailEntry")).IsEnabled;
            ((Entry)profileView.FindByName("FirstNameEntry")).IsEnabled = !((Entry)profileView.FindByName("FirstNameEntry")).IsEnabled;
            ((Entry)profileView.FindByName("LastNameEntry")).IsEnabled = !((Entry)profileView.FindByName("LastNameEntry")).IsEnabled;
            ((StackLayout)profileView.FindByName("PasswordEntry")).IsVisible = !((StackLayout)profileView.FindByName("PasswordEntry")).IsVisible;
            ((StackLayout)profileView.FindByName("NewPasswordEntry")).IsVisible = !((StackLayout)profileView.FindByName("NewPasswordEntry")).IsVisible;
            ((Button)profileView.FindByName("Button")).IsVisible = !((Button)profileView.FindByName("Button")).IsVisible;
            ((Button)profileView.FindByName("ButtonImage")).IsVisible = !((Button)profileView.FindByName("ButtonImage")).IsVisible;
        }

        public ProfileViewModel()
        {
            update = new Command(Update);
            edit = new Command(Edit);
            loadImage = new Command(EditImage);
        }

        private async void EditImage()
        {
            string answer = await Application.Current.MainPage.DisplayActionSheet("Take a photo or load image", "Cancel", null, "Take photo", "Load image");
            string path;
            if (answer.Equals("Take photo"))
            {
                path = await OpenCamera();
            }
            else
            {
                path = await LoadPicture();
            }
            ImageId = await SubmitImageAsync(path);
        }

        private async Task<string> LoadPicture()
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await Application.Current.MainPage.DisplayAlert("No Camera", ":( No camera available.", "OK");
                return null;
            }

            var photo = await CrossMedia.Current.PickPhotoAsync();
            if (photo != null)
            {
                return photo.Path;
            }
            return null;
        }

        private async Task<string> OpenCamera()
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await Application.Current.MainPage.DisplayAlert("No Camera", ":( No camera available.", "OK");
                return null;
            }

            var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                DefaultCamera = Plugin.Media.Abstractions.CameraDevice.Front
            });
            if (photo != null)
            {
                return photo.Path;
            }
            return null;
        }

        private async Task<int> SubmitImageAsync(String PathToImage)
        {
            ApiClient apiClient = new ApiClient();
            HttpClient client = new HttpClient();
            byte[] imageData = ImageToBinary(PathToImage);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://td-api.julienmialon.com/images");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ((LoginResult)Application.Current.Properties["token"]).AccessToken);

            MultipartFormDataContent requestContent = new MultipartFormDataContent();

            var imageContent = new ByteArrayContent(imageData);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            // Le deuxième paramètre doit absolument être "file" ici sinon ça ne fonctionnera pas
            requestContent.Add(imageContent, "file", "file.jpg");

            request.Content = requestContent;

            HttpResponseMessage response = await client.SendAsync(request);

            Response<ImageItem> resp = await apiClient.ReadFromResponse<Response<ImageItem>>(response);

            if (response.IsSuccessStatusCode)
            {
                return resp.Data.Id;
            }
            return 1;
        }

        public byte[] ImageToBinary(string imagePath)
        {

            FileStream fileStream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[fileStream.Length];
            fileStream.Read(buffer, 0, (int)fileStream.Length);
            fileStream.Close();
            return buffer;
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

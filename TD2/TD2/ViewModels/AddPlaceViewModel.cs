using Newtonsoft.Json;
using Plugin.FilePicker;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Storm.Mvvm;
using Storm.Mvvm.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TD2.API;
using TD2.Items;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TD2.ViewModels
{
    class AddPlaceViewModel : ViewModelBase
    {
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                if (SetProperty(ref _title, value) && value != null) { }
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                if (SetProperty(ref _description, value) && value != null) { }
            }
        }

        private string _latitude;
        public string Latitude
        {
            get => _latitude;
            set
            {
#pragma warning disable CS0472 // Le résultat de l'expression est toujours le même, car une valeur de ce type n'est jamais égale à 'null'
                if (SetProperty(ref _latitude, value) && value != null) { }
#pragma warning restore CS0472 // Le résultat de l'expression est toujours le même, car une valeur de ce type n'est jamais égale à 'null'
            }
        }

        private string _longitude;
        public string Longitude
        {
            get => _longitude;
            set
            {
#pragma warning disable CS0472 // Le résultat de l'expression est toujours le même, car une valeur de ce type n'est jamais égale à 'null'
                if (SetProperty(ref _longitude, value) && value != null) { }
#pragma warning restore CS0472 // Le résultat de l'expression est toujours le même, car une valeur de ce type n'est jamais égale à 'null'
            }
        }

        private Image _image;
        private readonly string URL = "https://td-api.julienmialon.com";
        
        public Image Image
        {
            get => _image;
            set
            {
                if(SetProperty(ref _image, value) && value != null) { }
            }
        }

        private string _pathToImage;
        public string PathToImage
        {
            get => _pathToImage;
            set
            {
                if (SetProperty(ref _pathToImage, value) && value != null) { }
            }
        }

        public ICommand submit { get; }
        public ICommand getCoordinate { get; }
        public ICommand takePicture { get; }
        public ICommand loadPicture { get; }



        public AddPlaceViewModel()
        {
            submit = new Command(Submit);
            getCoordinate = new Command(GetCoordinate);
            takePicture = new Command(OpenCamera);
            loadPicture = new Command(LoadPicture);
            Image = new Image();
        }

        private async void LoadPicture(object obj)
        {
            await CrossMedia.Current.Initialize();
            if(!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await Application.Current.MainPage.DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            var photo = await CrossMedia.Current.PickPhotoAsync();
            if (photo != null)
            {
                Image.Source = ImageSource.FromStream(() =>
                {
                    var stream = photo.GetStream();
                    return stream;
                });
                PathToImage = photo.Path;
            }
        }

        private async void OpenCamera(object obj)
        {
            await CrossMedia.Current.Initialize();
            if(!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await Application.Current.MainPage.DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                DefaultCamera = Plugin.Media.Abstractions.CameraDevice.Front
            });
            if (photo != null)
            {
                Image.Source = ImageSource.FromStream(() =>
                {
                    var stream = photo.GetStream();
                    return stream;
                });
                PathToImage = photo.Path;
            }
        }

        private async void GetCoordinate(object obj)
        {
            try
            {
                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium);
                Xamarin.Essentials.Location location = await Geolocation.GetLocationAsync(request);
                Latitude = location.Latitude.ToString();
                Longitude = location.Longitude.ToString();
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                Debug.WriteLine(fnsEx.Message);
            }
            catch (FeatureNotEnabledException fneEx)
            {
                Debug.WriteLine(fneEx.Message);
            }
            catch (PermissionException pEx)
            {
                Debug.WriteLine(pEx.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async void Submit(object obj)
        {
            if(Title == " " || Title == null) { await Application.Current.MainPage.DisplayAlert("Error", "Nom du lieu non précisé", "OK"); }
            else if (Description == " " || Description == null) { await Application.Current.MainPage.DisplayAlert("Error", "Description du lieu non précisée", "OK"); }
            else if (Image == null || PathToImage == null) { await Application.Current.MainPage.DisplayAlert("Error", "Image du lieu non précisée", "OK"); }
            else if (Latitude == " " || Latitude == null) { await Application.Current.MainPage.DisplayAlert("Error", "Latitude du lieu non précisée", "OK"); }
            else if (Longitude == " " || Longitude == null) { await Application.Current.MainPage.DisplayAlert("Error", "Longitude du lieu non précisée", "OK"); }
            else
            {
                try { double.Parse(_latitude); } 
                catch (Exception){ await Application.Current.MainPage.DisplayAlert("Error", "Format de la latitude incorrect", "OK"); }
                try { double.Parse(_longitude); }
                catch (Exception) { await Application.Current.MainPage.DisplayAlert("Error", "Format de la longitude incorrect", "OK"); }
                try
                {
                    ApiClient apiClient = new ApiClient();
                    int idImage = await SubmitImageAsync(apiClient);
                    HttpResponseMessage httpResponse = await apiClient.Execute(HttpMethod.Post, "https://td-api.julienmialon.com/places", new CreatePlaceRequest()
                    {
                        Title = _title,
                        Description = _description,
                        ImageId = idImage,
                        Latitude = double.Parse(_latitude),
                        Longitude = double.Parse(_longitude)
                    }, ((LoginResult)Application.Current.Properties["token"]).AccessToken);
                    Response<CreatePlaceRequest> response = await apiClient.ReadFromResponse<Response<CreatePlaceRequest>>(httpResponse);

                    if (response.IsSuccess)
                    {
                        await DependencyService.Get<INavigationService>().PopAsync();
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error response", response.ErrorMessage, "OK");
                    }
                }
                catch (Exception e)
                {
                    await Application.Current.MainPage.DisplayAlert("Error request", e.Message, "OK");
                }
            }
        }

        private async Task<int> SubmitImageAsync(ApiClient apiClient)
        {

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
    }
}

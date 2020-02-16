using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using TD2.Items;
using Xamarin.Forms;

namespace TD2.ViewModels
{
    class ProfileViewModel : ViewModelBase
    {
        [NavigationParameter("userItem")]
        public UserItem userItem { get; set; }

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

        private int _imageId;
        public int ImageId
        {
            get => _imageId;
            set
            {
                if (SetProperty(ref _imageId, value)) { }
            }
        }

        public ICommand update { get; }

        private void Update()
        {
            
        }

        public ICommand edit { get; }

        private void Edit()
        {
            
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

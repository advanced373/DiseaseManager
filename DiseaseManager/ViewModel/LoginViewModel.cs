using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DiseaseManager.Model;
using DiseaseManager.ViewModel.MVVM;
using DiseaseManager.Interfaces;
using DiseaseManager.Helper;
using DiseaseManager.View;
using System.Windows.Navigation;

namespace DiseaseManager.ViewModel
{
    class LoginViewModel : ViewModelBase,IPageViewModel
    {
        #region Fields
        private string username;
        private static string password;
        private LoginDataContext dataContext;
        private ApplicationViewModel appViewModel;
        #endregion
        #region Properties
        public string Name
        {
            get { return "Login"; }
        }
        #region PasswordBox
        public static readonly DependencyProperty BoundPassword =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(LoginViewModel), new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        public static readonly DependencyProperty BindPassword = DependencyProperty.RegisterAttached(
            "BindPassword", typeof(bool), typeof(LoginViewModel), new PropertyMetadata(false, OnBindPasswordChanged));

        private static readonly DependencyProperty UpdatingPassword =
            DependencyProperty.RegisterAttached("UpdatingPassword", typeof(bool), typeof(LoginViewModel), new PropertyMetadata(false));

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox box = d as PasswordBox;

            // only handle this event when the property is attached to a PasswordBox
            // and when the BindPassword attached property has been set to true
            if (d == null || !GetBindPassword(d))
            {
                return;
            }

            // avoid recursive updating by ignoring the box's changed event
            box.PasswordChanged -= HandlePasswordChanged;

            string newPassword = (string)e.NewValue;

            if (!GetUpdatingPassword(box))
            {
                password = newPassword;
                box.Password = newPassword;
            }

            box.PasswordChanged += HandlePasswordChanged;
        }

        private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            // when the BindPassword attached property is set on a PasswordBox,
            // start listening to its PasswordChanged event

            PasswordBox box = dp as PasswordBox;

            if (box == null)
            {
                return;
            }

            bool wasBound = (bool)(e.OldValue);
            bool needToBind = (bool)(e.NewValue);

            if (wasBound)
            {
                box.PasswordChanged -= HandlePasswordChanged;
            }

            if (needToBind)
            {
                box.PasswordChanged += HandlePasswordChanged;
            }
        }

        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox box = sender as PasswordBox;

            // set a flag to indicate that we're updating the password
            SetUpdatingPassword(box, true);
            // push the new password into the BoundPassword property
            SetBoundPassword(box, box.Password);
            SetUpdatingPassword(box, false);
        }

        public static void SetBindPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(BindPassword, value);
        }

        public static bool GetBindPassword(DependencyObject dp)
        {
            return (bool)dp.GetValue(BindPassword);
        }

        public static string GetBoundPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(BoundPassword);
        }

        public static void SetBoundPassword(DependencyObject dp, string value)
        {
            password = value;
            dp.SetValue(BoundPassword, value);
        }

        private static bool GetUpdatingPassword(DependencyObject dp)
        {
            return (bool)dp.GetValue(UpdatingPassword);
        }

        private static void SetUpdatingPassword(DependencyObject dp, bool value)
        {
            dp.SetValue(UpdatingPassword, value);
        }
        #endregion
        public string Username { get => username; set { username = value; OnPropertyChanged(Username); } }

        #endregion
        #region Constructor
        public LoginViewModel(ApplicationViewModel appViewModel)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DiseaseManager.Properties.Settings.DiseasesDBConnectionString"].ToString();
            dataContext = new LoginDataContext(connectionString);
            this.appViewModel = appViewModel;
        }
        #endregion
        #region Commmands
        public ICommand Login => new Command(_ => 
        {
            if (VerifyAccounts() == true)
            {
                StartingPageViewModel p = new StartingPageViewModel(this.appViewModel,this.Username);
                appViewModel.ChangePageCommand.Execute(p);
            }
            else
            {
                MessageBox.Show("Bad");
            }
        });
        public ICommand Register => new Command(_ =>
        {
            RegisterViewModel p = new RegisterViewModel(this.appViewModel);
            appViewModel.ChangePageCommand.Execute(p);
        });
        #endregion
        #region Methods
        private bool VerifyAccounts()
        {
            
            try
            {
                var result = from account in dataContext.Logins where account.Username == this.Username select account.Password;
                if(result.Count() == 0)
                {
                    return false;
                }
                byte[] passwordFromDatabase = Convert.FromBase64String(result.First());
                return HashGenerator.VerifyPasswordHash(password, passwordFromDatabase);
            }
            catch(InvalidOperationException e)
            {
                return false;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        private void OnUpdate() => Username = string.Empty;
        #endregion
    }
}

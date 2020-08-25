using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiseaseManager.Model;
using DiseaseManager.ViewModel.MVVM;
using DiseaseManager.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Prism.Commands;
using DiseaseManager.Helper;
namespace DiseaseManager.ViewModel
{
    class RegisterViewModel : ViewModelBase, IPageViewModel
    {
        #region Members
        
        private ApplicationViewModel appViewModel;
        private LoginDataContext dataContext;
        #endregion
        #region Properties
        public string Name
        {
            get { return "Register"; }
        }
        public DelegateCommand<PasswordBox> PasswordChangedCommand { get; private set; }
        public DelegateCommand<PasswordBox> PasswordRepeatChangedCommand { get; private set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string RepeatPassword { get; set; }
        #endregion
        #region Commands
        public ICommand Register => new Command(_ =>
        {
            try
            {
                if (Username.Length < 10)
                {
                    throw new Exception("Username is too short!");
                }
                if(Password != RepeatPassword)
                {
                    throw new Exception("Passwords no match.");
                }
                if(Password.Length<10)
                {
                    throw new Exception("Password weak!");
                }
                if(VerifyIfExists() == true)
                {
                    throw new Exception("Account already exists.");
                }
                SubmitAccount();
                MessageBox.Show("Registration successful.");
                System.Threading.Thread.Sleep(500);
                GoBack.Execute(this);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });
        public ICommand GoBack => new Command(_ =>
        {
            LoginViewModel loginViewModel = new LoginViewModel(this.appViewModel);
            appViewModel.ChangePageCommand.Execute(loginViewModel);
        });
        public RegisterViewModel(ApplicationViewModel applicationViewModel)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DiseaseManager.Properties.Settings.DiseasesDBConnectionString"].ToString();
            dataContext = new LoginDataContext(connectionString);
            appViewModel = applicationViewModel;
            PasswordChangedCommand = new DelegateCommand<PasswordBox>(ExecutePasswordChangedCommand);
            PasswordRepeatChangedCommand = new DelegateCommand<PasswordBox>(ExecutePasswordRepeatChangedCommand);
        }
        private void ExecutePasswordChangedCommand(PasswordBox obj)
        {
            Password = obj.Password;
        }
        private void ExecutePasswordRepeatChangedCommand(PasswordBox obj)
        {
            RepeatPassword = obj.Password;
        }
        private bool VerifyIfExists()
        {
            var result = from account in dataContext.Logins where account.Username == this.Username select account.Password;
            if (result.Count() == 0)
            {
                return false;
            }
            return true;
        }
        private void SubmitAccount()
        {
            dataContext.Logins.InsertOnSubmit(new Login { Username = this.Username, Password = Convert.ToBase64String(HashGenerator.generateHash(this.Password)), canAcceptRequests = false });
            dataContext.SubmitChanges();
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiseaseManager.ViewModel.MVVM;
using DiseaseManager.Helper;
using DiseaseManager.Model;
using DiseaseManager.Interfaces;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.Windows.Media;
using System.Windows.Input;

namespace DiseaseManager.ViewModel
{
    class StartingPageViewModel:ViewModelBase,IPageViewModel
    {

        #region Members
        private string currentCountryPopulation;
        private string currentCountryCases;
        private string currentCountryDeaths;
        private ApplicationViewModel appViewModel;
        private StartingPageModel model;
        private string currentCountryName;
        private string username;
        #endregion
        #region Properties
        public string CurrentCountryPopulation 
        {
            get => currentCountryPopulation;
            set
            {
                currentCountryPopulation = value;
                OnPropertyChanged("CurrentCountryPopulation");
            }
        }
        public string CurrentCountryCases
        {
            get => currentCountryCases;
            set
            {
                currentCountryCases = value;
                OnPropertyChanged("CurrentCountryCases");
            }
        }
        public string CurrentCountryDeaths
        {
            get => currentCountryDeaths;
            set
            {
                currentCountryDeaths = value;
                OnPropertyChanged("CurrentCountryDeaths");
            }
        }
        public SeriesCollection countryCollection { get; }
        public Func<double, string> Formatter { get; }
        public string Username 
        { 
            get => username;
            set { username = value; OnPropertyChanged(Username); }
        }
        public List<string> Labels { get; set; }
        public ObservableCollection<string> CountriesNames { get; }
        public string CurrentCountryName 
        {
            get => currentCountryName;
            set { 
                currentCountryName = value; 
                model.CurrentCountryName=currentCountryName ;
                CurrentCountryPopulation = model.CurrentCountryPopulation;
                CurrentCountryCases = model.CurrentCountryCases;
                CurrentCountryDeaths = model.CurrentCountryDeaths;
                OnPropertyChanged(CurrentCountryName);
            } 
        }
        public string Name
        {
            get { return "Starting Page"; }
        }
        #endregion
        #region Commands
        public ICommand GoBack => new Command(_ =>
        {
            LoginViewModel p = new LoginViewModel(this.appViewModel);
            appViewModel.ChangePageCommand.Execute(p);
        });
        #endregion
        #region Constructors
        public StartingPageViewModel(ApplicationViewModel appViewModel, string username)
        {
            this.appViewModel = appViewModel;
            Labels = new List<string>();
            CountriesNames = new ObservableCollection<string>();
            var chartValues = new ChartValues<double>();
            model = new StartingPageModel("../../Repository/data.xml", CountriesNames, chartValues,Labels);
            countryCollection = new SeriesCollection();
            countryCollection.Add(new ColumnSeries { Title="Covid Cases", Values=chartValues, Fill=Brushes.Red});
            Username = username;
            Formatter = value => value.ToString("N");
        }
        #endregion
    }
}

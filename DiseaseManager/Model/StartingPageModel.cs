using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DiseaseManager.Helper;
using LiveCharts;
using LiveCharts.Wpf;

namespace DiseaseManager.Model
{
    class StartingPageModel
    {
        #region Members
        private string currentCountryPopulation;
        private string currentCountryDeaths;
        private string currentCountryCases;
        private ICollection<string> countriesNames;
        private ChartValues<double> countryRecords;
        private string xmlDocumentName;
        private XDocument xmlDocument;
        private string currentCountryName;
        private List<string> Labels;
        #endregion
        #region Constructors
        public StartingPageModel(string xmlDocumentName, ICollection<string> countriesNames, ChartValues<double> points, List<string> Labels)
        {
            currentCountryCases = "0";
            currentCountryDeaths = "0";
            currentCountryCases="0";
            this.Labels = Labels;
            this.xmlDocumentName = xmlDocumentName;
            this.countriesNames = countriesNames;
            this.countryRecords = points;
            xmlDocument = new XDocument();
            xmlDocument = XDocument.Load(xmlDocumentName);
            ProvideCountriesNames();
        }
        #endregion
        #region Properties
        public string CurrentCountryName { get => currentCountryName; set { currentCountryName = value; ProvideCountrySeries(); } }
        public  string CurrentCountryPopulation { get => currentCountryPopulation; }
        public string  CurrentCountryCases { get => currentCountryCases; }
        public string CurrentCountryDeaths { get => currentCountryDeaths; }
        #endregion
        #region Methods
        private void ProvideCountrySeries()
        {
            int sumOfCases=0,sumOfDeaths=0;
            this.countryRecords.Clear();
            var records = new ObservableCollection<Record>(from record in xmlDocument.Descendants("record")
                                                           where record.Element("countriesAndTerritories").Value == CurrentCountryName
                                                           select new Record
                                                           {
                                                               Date = DateTime.Parse(record.Element("dateRep").Value),
                                                               Cases = int.Parse(record.Element("cases").Value),
                                                               Deaths = int.Parse(record.Element("deaths").Value),
                                                               Population = int.Parse(record.Element("popData2019").Value),
                                                               Continent = record.Element("continentExp").Value
                                                           }).Distinct().OrderByDescending(Record=> Record.Date).Take(14).ToList();
            foreach (var record in records)
            {
                sumOfCases += record.Cases;
                sumOfDeaths += record.Deaths;
                this.Labels.Add(record.Date.ToString());
                this.countryRecords.Add(record.Cases);
            }
            currentCountryPopulation = records[0].Population.ToString();
            currentCountryCases = sumOfCases.ToString();
            currentCountryDeaths = sumOfDeaths.ToString();
        }
        private void ProvideCountriesNames()
        {
            var countriesNames = from record in xmlDocument.Descendants("record")
                                                       select record.Element("countriesAndTerritories").Value;
            foreach (var countryName in countriesNames.Distinct())
            {
               this.countriesNames.Add(countryName);
            }
        }
        #endregion
    }
}

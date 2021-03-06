﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DiseaseManager.ViewModel.MVVM;
using DiseaseManager.Interfaces;
using Prism.Commands;
using System.Windows.Controls;

namespace DiseaseManager.ViewModel
{
    class ApplicationViewModel:ViewModelBase
    {
        #region Members
        private ICommand _changePageCommand;
        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;
        public ApplicationViewModel()
        {
            // Add available pages
            PageViewModels.Add(new LoginViewModel(this));

            // Set starting page
            CurrentPageViewModel = PageViewModels[0];
        }
        #endregion
        #region Properties / Commands

        public ICommand ChangePageCommand
        {
            get
            {
                if (_changePageCommand == null)
                {
                    _changePageCommand = new Command(
                        p => ChangeViewModel((IPageViewModel)p),
                        p => p is IPageViewModel);
                }

                return _changePageCommand;
            }
        }

        public List<IPageViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IPageViewModel>();

                return _pageViewModels;
            }
        }

        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged("CurrentPageViewModel");
                }
            }
        }
        #endregion
        #region Methods
        private void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            CurrentPageViewModel = PageViewModels
                .FirstOrDefault(vm => vm == viewModel);
        }
        #endregion
    }
}

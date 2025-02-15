﻿using System.Windows.Input;
using DsmSuite.DsmViewer.Application.Interfaces;
using DsmSuite.DsmViewer.ViewModel.Common;

namespace DsmSuite.DsmViewer.ViewModel.Editing.Snapshot
{
    public class SnapshotMakeViewModel : ViewModelBase
    {
        private readonly IDsmApplication _application;
        private string _description;
        private string _help;

        public ICommand AcceptChangeCommand { get; }

        public SnapshotMakeViewModel(IDsmApplication application)
        {
            _application = application;

            Title = "Make snapshot";
            Help = "";

            Description = "";
            AcceptChangeCommand = new RelayCommand<object>(AcceptChangeExecute, AcceptChangeCanExecute);
        }

        public string Title { get; }

        public string Help
        {
            get { return _help; }
            private set { _help = value; RaisePropertyChanged(); }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; RaisePropertyChanged();  }
        }

        private void AcceptChangeExecute(object parameter)
        {
            _application.MakeSnapshot(Description);
        }

        private bool AcceptChangeCanExecute(object parameter)
        {
            return Description.Length > 0;
        }
    }
}

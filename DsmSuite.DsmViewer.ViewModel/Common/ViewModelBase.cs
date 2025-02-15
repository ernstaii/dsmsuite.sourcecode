﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using DsmSuite.DsmViewer.ViewModel.Properties;

namespace DsmSuite.DsmViewer.ViewModel.Common
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

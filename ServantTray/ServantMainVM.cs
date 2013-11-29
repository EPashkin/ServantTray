using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ServantTray
{
    public class ServantMainVM : INotifyPropertyChanged, IDisposable
    {
        public ServantMainVM()
        {
        }

        public string ExitTitle
        {
            get { return "Exit"; }
        }

        public ICommand ExitCommand
        {
            get
            {
                return CustomCommands.Exit;
            }
        }

        public bool ExitCanExecute()
        {
            return true;
        }

        public void OnExit()
        {
            Application.Current.Shutdown();
        }

        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region IDisposable Members
        public void Dispose()
        {
        }
        #endregion
    }
}

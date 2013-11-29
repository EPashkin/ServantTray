using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ServantTray.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class ServantMainVM : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public ServantMainVM()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
        }

        public string ExitTitle
        {
            get { return "Exit"; }
        }

        private ICommand m_ExitCommand;
        public ICommand ExitCommand
        {
            get
            {
                return m_ExitCommand ?? (m_ExitCommand = new RelayCommand<object>(OnExit));
            }
        }

        private void OnExit(object parameter)
        {
            Application.Current.Shutdown();
        }

    }
}
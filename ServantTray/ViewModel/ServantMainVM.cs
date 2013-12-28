using System.Collections.ObjectModel;
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
        private OTP_worker OTP;

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
            string target = Properties.Settings.Default.serverNode;
            string cookie = Properties.Settings.Default.serverCookie;
            OTP = new OTP_worker(target, cookie);

            var aa = new string[] { "Test1", "Test2" };
            TaskMenu.Add(null); //adding separator
            foreach (string text in aa)
            {
                TaskMenu.Add(new TaskMenuItemVM(text));
            }
            TaskMenu.Add(null); //adding separator
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

        private ObservableCollection<TaskMenuItemVM> m_TaskMenu;
        public ObservableCollection<TaskMenuItemVM> TaskMenu
        {
            get
            {
                return m_TaskMenu ?? (m_TaskMenu = new ObservableCollection<TaskMenuItemVM>());
            }
        }

        private ICommand m_TaskCommand;
        public ICommand TaskCommand
        {
            get
            {
                return m_TaskCommand ?? (m_TaskCommand = new RelayCommand<object>(OnTask));
            }
        }

        private void OnTask(object parameter)
        {
            MessageBox.Show(parameter.ToString());
        }
    }
}

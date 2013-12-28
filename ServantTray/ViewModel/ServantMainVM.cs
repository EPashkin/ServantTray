using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
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
        private readonly Dispatcher _dispatcher;

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
            _dispatcher = Dispatcher.CurrentDispatcher;

            string target = Properties.Settings.Default.serverNode;
            string cookie = Properties.Settings.Default.serverCookie;
            OTP = new OTP_worker(target, cookie);
            OnRefreshList(null);

            TaskMenu.Add(null); //adding separator
            TaskMenu.Add(null); //adding separator before exit
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
            OTP.Stop();
            Application.Current.Shutdown();
        }

        private ICommand m_RefreshListCommand;
        public ICommand RefreshListCommand
        {
            get
            {
                return m_RefreshListCommand ?? (m_RefreshListCommand = new RelayCommand<object>(OnRefreshList));
            }
        }

        private void OnRefreshList(object parameter)
        {
            OTP.GetList(OnAddTaskMenuItem);
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
            TaskMenuItemVM menuItem = parameter as TaskMenuItemVM;
            if (menuItem == null)
                return;
            string msg = string.Format("{0}{1}{2}", menuItem.Title, Environment.NewLine, menuItem.Code);
            MessageBox.Show(msg);
        }

        private void OnAddTaskMenuItem(string text, object code)
        {
            _dispatcher.Invoke(() => AddTaskMenuItem(text, code));
        }

        private void AddTaskMenuItem(string text, object code)
        {
            //last item - separator before Exit
            int count = TaskMenu.Count;
            TaskMenu.Insert(count - 1, new TaskMenuItemVM(text, code));
        }
    }
}

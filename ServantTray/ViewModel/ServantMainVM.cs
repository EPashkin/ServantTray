using System;
using System.Collections.Generic;
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
        enum StatusTypes { Disconnected = 0, Connected, HasConfirmations }
        private StatusTypes m_Status = 0;
        private StatusTypes Status
        {
            get { return m_Status; }
            set
            {
                if (m_Status == value)
                    return;
                m_Status = value;
                this.RaisePropertyChanged("ConnectedTitle");
                this.RaisePropertyChanged("TrayIconSource");
            }
        }
        private readonly Dispatcher _dispatcher;
        private readonly DispatcherTimer m_timer;

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
            OTP = new OTP_worker(target, cookie, OnConnectedChanged);

            m_timer = new DispatcherTimer(TimeSpan.FromSeconds(10),
                DispatcherPriority.Background,
                (o, args) => OnRefreshList(null),
                _dispatcher);
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
            OTP.GetList(OnGetList);
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
            TaskMenu.Remove(menuItem);
            OTP.MenuItemClicked(menuItem.Code);
        }

        private void OnGetList(IEnumerable<Tuple<String, object>> list)
        {
            _dispatcher.Invoke(() => FillTaskMenu(list));
        }

        private void FillTaskMenu(IEnumerable<Tuple<String, object>> list)
        {
            TaskMenu.Clear();
            foreach (var item in list)
            {
                TaskMenu.Add(new TaskMenuItemVM(item.Item1, item.Item2));
            }

            if (Status != StatusTypes.Disconnected)
                Status = TaskMenu.Count > 0 ? StatusTypes.HasConfirmations : StatusTypes.Connected;
        }

        public string ConnectedTitle
        {
            get
            {
                return Status != StatusTypes.Disconnected ? "Connected" : "Not connected";
            }
        }

        private void OnConnectedChanged(bool connected)
        {
            Status = connected ? StatusTypes.Connected : StatusTypes.Disconnected;
        }

        public string TrayIconSource
        {
            get
            {
                switch (Status)
                {
                    case StatusTypes.Connected:
                        return "/Icons/Connected.ico";
                    case StatusTypes.HasConfirmations:
                        return "/Icons/Bulb.ico";
                    default:
                        return "/Icons/Disconnected.ico";
                }
            }
        }
    }
}

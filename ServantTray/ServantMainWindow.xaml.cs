using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ServantTray
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.TaskIcon.TrayContextMenuOpen += TaskIcon_TrayContextMenuOpen;

            //hide Main Window
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        void TaskIcon_TrayContextMenuOpen(object sender, RoutedEventArgs e)
        {
            //hack: for enabling 'exit' menu item
            this.TaskIcon.ContextMenu.Focus();
        }

        private void ExitCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }
        private void OnExit(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

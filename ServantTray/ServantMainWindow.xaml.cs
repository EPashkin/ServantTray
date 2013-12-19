using System.Windows;

namespace ServantTray
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private OTP_worker OTP;

        public MainWindow()
        {
            InitializeComponent();

            //hide Main Window
            this.Visibility = System.Windows.Visibility.Hidden;

            string target = Properties.Settings.Default.serverNode;
            OTP = new OTP_worker(target);
        }
    }
}

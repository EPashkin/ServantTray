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
            var vm = new ServantMainVM();
            this.DataContext = vm;

            InitializeComponent();

            //hide Main Window
            this.Visibility = System.Windows.Visibility.Hidden;

            string target = "ketchup";
            OTP = new OTP_worker(target);
        }
    }
}

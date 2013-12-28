using System.Windows;

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

            //hide Main Window
            this.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}

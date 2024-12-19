using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Fragrant_World
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(Window));
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            MainFrame.Navigated += OnNavigated;
        }

        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is Page page)
            {
                Title = page.Title;
            }
        }
    }
}
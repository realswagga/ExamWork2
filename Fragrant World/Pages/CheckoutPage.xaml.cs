using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DataBaseLayer.Classes;


namespace Fragrant_World.Pages
{
    /// <summary>
    /// Interaction logic for CheckoutPage.xaml
    /// </summary>
    public partial class CheckoutPage : Page
    {
        public CheckoutPage()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(Page));
            UserNameLabel.Content = $"{UserDataBus.Surname} {UserDataBus.Name} {UserDataBus.Patronymic}";
        }

        private void GoBackImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}

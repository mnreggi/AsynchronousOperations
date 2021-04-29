using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace Asynchronous_Operations
{
    public partial class StocksExample : Window
    {
        private static string API_URL = "https://ps-async.fekberg.com/api/stocks";

        public StocksExample()
        {
            InitializeComponent();
        }
        
        
        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        
        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));

            e.Handled = true;
        }
    }
}
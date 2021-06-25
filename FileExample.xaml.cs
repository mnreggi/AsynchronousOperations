using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using Asynchronous_Operations.Services.FileExample;

namespace Asynchronous_Operations
{
    public partial class FileExample : Window
    {
        private Stopwatch stopwatch = new Stopwatch();
        CancellationTokenSource cancellationTokenSource;

        public FileExample()
        {
            InitializeComponent();
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            if(cancellationTokenSource != null)
            {
                // Already have an instance of the cancellation token source?
                // This means the button has already been pressed!
                cancellationTokenSource.Cancel();
                cancellationTokenSource = null;

                Search.Content = "Search";
                return;
            }

            try
            {
                cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Token.Register(() => { Notes.Text = "Cancellation requested"; });
                Search.Content = "Cancel"; // Button text

                BeforeLoadingPeopleData();

                var service = new PeopleService();

                var data = await service.GetPeopleFrom(CitiesIdentifier.Text, cancellationTokenSource.Token);
                PeopleGrid.ItemsSource = data;
            }
            catch (Exception ex)
            {
                Notes.Text = ex.Message;
            }
            finally
            {
                AfterLoadingPeopleData();

                cancellationTokenSource = null;
                Search.Content = "Search";
            }
        }

        private void BeforeLoadingPeopleData()
        {
            stopwatch.Restart();
            PeopleProgress.Visibility = Visibility.Visible;
            PeopleProgress.IsIndeterminate = true;
        }

        private void AfterLoadingPeopleData()
        {
            var from = string.IsNullOrEmpty(CitiesIdentifier.Text) ? "Everywhere" : CitiesIdentifier.Text;
            PeopleStatus.Text = $"Loaded people from {from} in {stopwatch.ElapsedMilliseconds} ms";
            PeopleProgress.Visibility = Visibility.Hidden;
        }
    }
}

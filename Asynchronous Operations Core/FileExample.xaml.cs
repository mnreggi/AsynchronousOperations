using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Asynchronous_Operations_Core
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
                cancellationTokenSource.Cancel();
                cancellationTokenSource = null;

                Search.Content = "Search";
                return;
            }

            try
            {
                cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.Token.Register(() => { Notes.Text = "Cancellation requested"; });
                Search.Content = "Cancel";

                BeforeLoadingPeopleData();
                
                // To dynamically add items to the UI as they arrive to the application, we need to introduce an observable collection.
                // This will be the dataSource, and as items are added to it, they will appear in the UI.
                var data = new ObservableCollection<People>();
                PeopleGrid.ItemsSource = data;

                var service = new MockStreamService();
                // var service = new StreamService();

                // Before, we used await service.GetPeopleFrom(CitiesIdentifier.Text, cancellationTokenSource.Token); because it was returning a Task
                // Now, we have an enumeration that will asynchronously allow us to retrieve items.
                // We will use it before the foreach loop.
                var enumerator = service.GetPeople(cancellationTokenSource.Token);

                await foreach (var person in enumerator.WithCancellation(cancellationTokenSource.Token))
                {
                    if (string.IsNullOrEmpty(CitiesIdentifier.Text) || !string.IsNullOrEmpty(person.City) && CitiesIdentifier.Text.Contains(person.City))
                    {
                        data.Add(person);
                    }
                }

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

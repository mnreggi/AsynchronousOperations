using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;

namespace Asynchronous_Operations
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MakeTeaExample_OnClick(object sender, RoutedEventArgs e)
        {
            var example = new MakeTea();
            example.Show();
        }

        private void PeopleExample_OnClick(object sender, RoutedEventArgs e)
        {
            var example = new FileExample();
            example.Show();
        }
        
        private async void ThisIsTest()
        {
            testNote.Text = "Hi!";
        }

        private async Task WriteSomething()
        {
            var result = await Task.Run(() => "CMP");
        }
        
        private async Task WriteSomething55()
        {
            // Part 1
            var client = new HttpClient();
            var page = await client.GetStringAsync("http://google.com");

            // Part 2 - Continuation
            if (page == "CMP")
            {
                Console.WriteLine(@"Huge Success");
            }
        }
    }
}
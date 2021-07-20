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
    }
}
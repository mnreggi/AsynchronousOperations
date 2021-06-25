
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
    }
}
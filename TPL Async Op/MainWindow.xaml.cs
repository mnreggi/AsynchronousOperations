using Newtonsoft.Json;
using StockAnalyzer.Core.Domain;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace TPL_Async_Op
{
    public partial class MainWindow : Window
    {
        private static string API_URL = "https://ps-async.fekberg.com/api/stocks";
        private Stopwatch stopwatch = new Stopwatch();
        private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            BeforeLoadingStockData();
            
            var stocks = new Dictionary<string, IEnumerable<StockPrice>>
            {
                { "MSFT", Generate("MSFT") },
                { "GOOGL", Generate("GOOGL") },
                { "BTC", Generate("BTC") },
                { "AMAZ", Generate("AMAZ") }
            };

            #region Approach 1 (Sync way)

            // var msft = Calculate(stocks["MSFT"]);
            // var google = Calculate(stocks["GOOGL"]);
            // var btc = Calculate(stocks["BTC"]);
            // var amazon = Calculate(stocks["AMAZ"]);
            //
            // Stocks.ItemsSource = new[] { msft, google, btc, amazon };
            //
            // AfterLoadingStockData();

            #endregion
            
            #region Approach 2 (Moving heavy computing to 1 thread)

            // var taskApproach2 = Task.Run(() =>
            // {
            //     var msft = Calculate(stocks["MSFT"]);
            //     var google = Calculate(stocks["GOOGL"]);
            //     var btc = Calculate(stocks["BTC"]);
            //     var amazon = Calculate(stocks["AMAZ"]);
            //     
            //     Dispatcher.Invoke(() =>
            //     {
            //         Stocks.ItemsSource = new[] { msft, google, btc, amazon };
            //     });
            // });
            // taskApproach2.ContinueWith(_ =>
            // {
            //     Dispatcher.Invoke(AfterLoadingStockData);
            // });

            #endregion
            
            #region Approach 3 (Multiple threads spawned)

            // var taskApproach2 = Task.Run(async () =>
            // {
            //     var msftTask = Task.Run(() => Calculate(stocks["MSFT"]));
            //     var googleTask = Task.Run(() => Calculate(stocks["GOOGL"]));
            //     var btcTask = Task.Run(() => Calculate(stocks["BTC"]));
            //     var amazonTask = Task.Run(() => Calculate(stocks["AMAZ"]));
            //    
            //     var allTasks = new[] { msftTask, googleTask, btcTask, amazonTask };
            //
            //     var tasksCompleted = await Task.WhenAll(allTasks);
            //     
            //     Dispatcher.Invoke(() =>
            //     {
            //         Stocks.ItemsSource = tasksCompleted;
            //     });
            // });
            // taskApproach2.ContinueWith(_ =>
            // {
            //     Dispatcher.Invoke(AfterLoadingStockData);
            // });

            #endregion
            
            #region Approach 4 (Using Parallel class)

            // var bag = new ConcurrentBag<StockCalculation>();
            // Parallel.Invoke(
            //     () => { bag.Add(Calculate(stocks["MSFT"])); },
            //     () => { bag.Add(Calculate(stocks["GOOGL"])); },
            //     () => { bag.Add(Calculate(stocks["BTC"])); },
            //     () => { bag.Add(Calculate(stocks["AMAZ"])); }
            // );
            //
            // Stocks.ItemsSource = bag;
            //
            // AfterLoadingStockData();

            #endregion
            
            #region Approach 5 (Using Parallel class + Async)
            
            // var bag = new ConcurrentBag<StockCalculation>();
            // await Task.Run(() =>
            // {
            //     Parallel.Invoke(
            //         () => { bag.Add(Calculate(stocks["MSFT"])); },
            //         () => { bag.Add(Calculate(stocks["GOOGL"])); },
            //         () => { bag.Add(Calculate(stocks["BTC"])); },
            //         () => { bag.Add(Calculate(stocks["AMAZ"])); }
            //     );
            // });
            //
            // Stocks.ItemsSource = bag;
            //
            // AfterLoadingStockData();

            #endregion
            
            #region (Using Parallel class + Async + Throwing Exception)
            
            // var bag = new ConcurrentBag<StockCalculation>();
            // try
            // {
            //     await Task.Run(() =>
            //     {
            //         try
            //         {
            //             Parallel.Invoke(
            //                 () =>
            //                 {
            //                     bag.Add(Calculate(stocks["MSFT"]));
            //                     throw new Exception("MSFT Exception");
            //                 },
            //                 () => { bag.Add(Calculate(stocks["GOOGL"])); },
            //                 () =>
            //                 {
            //                     bag.Add(Calculate(stocks["BTC"]));
            //                     throw new Exception("BTC Exception");
            //                 },
            //                 () => { bag.Add(Calculate(stocks["AMAZ"])); }
            //             );
            //         }
            //         catch (Exception exception)
            //         {
            //             Console.WriteLine(exception);
            //             throw;
            //         }
            //     });
            // }
            // catch (Exception exception)
            // {
            //     var aggregateException = exception as AggregateException;
            //     var message =
            //         $"{exception.Message} - InnerException count: {aggregateException?.InnerExceptions.Count} - {aggregateException?.InnerExceptions.Aggregate(string.Empty, (current, exceptionInnerException) => current + exceptionInnerException.Message + "\n")}";
            //     Notes.Text = message;
            // }
            //
            // Stocks.ItemsSource = bag;
            //
            // AfterLoadingStockData();

            #endregion
        }

        private IEnumerable<StockPrice> Generate(string stockIdentifier)
        {
            return Enumerable.Range(1, random.Next(10, 250))
                .Select(x => new StockPrice { 
                    Identifier = stockIdentifier, 
                    Open = random.Next(10, 1024) 
                });
        }

        private StockCalculation Calculate(IEnumerable<StockPrice> prices)
        {
            #region Start stopwatch
            var calculation = new StockCalculation();
            var watch = new Stopwatch();
            watch.Start();
            #endregion

            var end = DateTime.UtcNow.AddSeconds(4);

            // Spin a loop for a few seconds to simulate load
            while (DateTime.UtcNow < end)
            { }

            #region Return a result
            calculation.Identifier = prices.First().Identifier;
            calculation.Result = prices.Average(s => s.Open);

            watch.Stop();

            calculation.TotalSeconds = watch.Elapsed.Seconds;

            return calculation;
            #endregion
        }

        private void BeforeLoadingStockData()
        {
            stopwatch.Restart();
            StockProgress.Visibility = Visibility.Visible;
            StockProgress.IsIndeterminate = true;
        }

        private void AfterLoadingStockData()
        {
            StocksStatus.Text = $"Loaded stocks for {StockIdentifier.Text} in {stopwatch.ElapsedMilliseconds}ms";
            StockProgress.Visibility = Visibility.Hidden;
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));

            e.Handled = true;
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

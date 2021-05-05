using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Asynchronous_Operations.Services;

namespace Asynchronous_Operations
{
    public partial class MakeTea : Window
    {
        private static StringBuilder _finalMessage;
        private Stopwatch stopwatch = new Stopwatch();

        public MakeTea()
        {
            InitializeComponent();
        }
        
        private void BeforeMakingTea()
        {
            stopwatch.Restart();
            TeaProgress.Visibility = Visibility.Visible;
            TeaProgress.IsIndeterminate = true;
            MakingTeatStatus.Text = $"Making tea. Sit tight.";
        }
        
        private void AfterMakingTea()
        {
            MakingTeatStatus.Text = $"Made tea in {stopwatch.ElapsedMilliseconds}ms";
            TeaProgress.Visibility = Visibility.Hidden;
            stopwatch.Stop();
        }

        private void MakeTeaSynchronous_OnClick(object sender, RoutedEventArgs e)
        {
            BeforeMakingTea();
            _finalMessage = new StringBuilder();
            _finalMessage.AppendLine("Making tea started.");
            _finalMessage.AppendLine(MakeTeaSynchronous.MakeMeTea());

            Notes.Text = _finalMessage.ToString();
            
            AfterMakingTea();
        }

        private async void MakeTeaAsynchronous_OnClick(object sender, RoutedEventArgs e)
        {
            BeforeMakingTea();
            _finalMessage = new StringBuilder();
            _finalMessage.AppendLine("Making tea started.");
            _finalMessage.AppendLine(await MakeTeaAsync.MakeMeTeaAsync());

            Notes.Text = _finalMessage.ToString();
            
            AfterMakingTea();
        }
        
        /// <summary>
        /// This will crash the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MakeTeaAsynchronousConfigureAwaitFalse_OnClick(object sender, RoutedEventArgs e)
        {
            BeforeMakingTea();

            _finalMessage = new StringBuilder();
            _finalMessage.AppendLine("Making tea started.");
            
            // Adding ConfigureAwait false, will indicate that we don't care that the continuation be executed in a different thread.
            // Because this is a UI component, we can see that we are trying to update the UI with something, and because we are
            // in a different thread in the continuation, the application will crash.
            _finalMessage.AppendLine(await MakeTeaAsync.MakeMeTeaAsync().ConfigureAwait(false));

            Notes.Text = _finalMessage.ToString();
            AfterMakingTea();
        }

        private async void MakeTeaAsyncHeavy_OnClick(object sender, RoutedEventArgs e)
        {
            BeforeMakingTea();
            _finalMessage = new StringBuilder();
            _finalMessage.AppendLine("Making tea started.");
            _finalMessage.AppendLine(await MakeHeavyTaskTea.MakeMeHeavyTeaAsync());

            Notes.Text = _finalMessage.ToString();
            AfterMakingTea();
        }
        
        private async void MakeTeaAsyncWithException_OnClick(object sender, RoutedEventArgs e)
        {
            BeforeMakingTea();

            try
            {
                _finalMessage = new StringBuilder();
                _finalMessage.AppendLine("Making tea started.");
                _finalMessage.AppendLine(await MakeTeaAsyncWithException.MakeMeTeaAsync());

                Notes.Text = _finalMessage.ToString();
            }
            catch (Exception exception)
            {
                Notes.Text = exception.Message;
            }
            AfterMakingTea();
        }

        private async void MakeTeaAsyncWithExceptionWithoutAwait_OnClick(object sender, RoutedEventArgs e)
        {
            BeforeMakingTea();
            try
            {
                _finalMessage = new StringBuilder();
                _finalMessage.AppendLine("Making tea started.");
                MakeTeaAsyncWithException.MakeMeTeaAsyncInTryCatch();
                _finalMessage.AppendLine("After calling the method async");

                Notes.Text = _finalMessage.ToString();
            }
            catch (Exception exception)
            {
                // Why we are not seeing the exception in the Notes?
                // If we don't use the await keyword, the exception will be swallowed. await keyword will validate that the task
                // completed successfully, but in this case, there's no await keyword. Moreover, there's no continuation and that's why we 
                // can see that is not "async" and the next message is shown.
                Notes.Text = exception.Message;
            }
            AfterMakingTea();
        }

        /// <summary>
        /// This will terminate the app.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MakeTeaAsyncWithExceptionAndAsyncVoid_OnClick(object sender, RoutedEventArgs e)
        {
            BeforeMakingTea();
            try
            {
                _finalMessage = new StringBuilder();
                _finalMessage.AppendLine("Making tea started.");
                MakeTeaAsyncWithException.MakeMeTeaAsyncVoidInTryCatch();
                _finalMessage.AppendLine("After calling the method async");

                Notes.Text = _finalMessage.ToString();
            }
            catch (Exception exception)
            {
                Notes.Text = exception.Message;
            }
            AfterMakingTea();
        }
        
        /// <summary>
        /// This will "seems" that is working, but taking deeper look, we can see that actually an error was thrown. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MakeTeaAsyncUsingTPLNoDispatcher_OnClick(object sender, RoutedEventArgs e)
        {
            BeforeMakingTea();
            Task.Run(async () =>
            {
                _finalMessage = new StringBuilder();
                _finalMessage.AppendLine("Making tea started.");
                
                _finalMessage.AppendLine(await MakeTeaAsync.MakeMeTeaAsync());
                Notes.Text = _finalMessage.ToString();
            });
            AfterMakingTea();
        }

        /// <summary>
        /// This method now will work, but is not telling us how long it took to make the tea.
        /// This is because the Task.Run will be queued on the thread pool and completes instantly, because
        /// it's been executed in a different thread. Therefore the next piece of code (After) will execute.
        /// We need to introduce the concept of Continuation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MakeTeaAsyncUsingTPL_OnClick(object sender, RoutedEventArgs e)
        {
            BeforeMakingTea();
            Task.Run(async () =>
            {
                _finalMessage = new StringBuilder();
                _finalMessage.AppendLine("Making tea started.");
                
                _finalMessage.AppendLine(await MakeTeaAsync.MakeMeTeaAsync());

                Dispatcher.Invoke(() =>
                { 
                    Notes.Text = _finalMessage.ToString();
                });
            });
            AfterMakingTea();
        }
    }
}
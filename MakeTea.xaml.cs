using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Asynchronous_Operations.Services;
using Asynchronous_Operations.Services.TPL;

namespace Asynchronous_Operations
{
    public partial class MakeTea : Window
    {
        private static StringBuilder _finalMessage;
        private Stopwatch stopwatch = new Stopwatch();
        CancellationTokenSource cancellationTokenSource;

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

        /// <summary>
        /// Let's simulate a big workload here, assume that someone is knocking your door and you want to receive that person.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        
        /// <summary>
        /// This scenario will cause a Deadlock
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MakeTeaAsyncDeadlock_OnClick(object sender, RoutedEventArgs e)
        {
            BeforeMakingTea();
            try
            {
                _finalMessage = new StringBuilder();
                _finalMessage.AppendLine("Making tea started.");
                MakeTeaAsync.MakeMeTeaAsync().Wait();
                _finalMessage.AppendLine("After calling the method async");

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
        private void MakeTeaAsyncUsingTPLDispatcher_OnClick(object sender, RoutedEventArgs e)
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
        
        private async void MakeTeaAsyncAwaitUsingTPLDispatcher_OnClick(object sender, RoutedEventArgs e)
        {
            BeforeMakingTea();
            await Task.Run(async () =>
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
        
        private void MakeTeaAsyncUsingTPLDispatcherContinuation_OnClick(object sender, RoutedEventArgs e)
        {
            BeforeMakingTea();
            var makeTeaTask = Task.Run(async () =>
            {
                _finalMessage = new StringBuilder();
                _finalMessage.AppendLine("Making tea started.");
                
                _finalMessage.AppendLine(await MakeTeaAsync.MakeMeTeaAsync());

                Dispatcher.Invoke(() =>
                { 
                    Notes.Text = _finalMessage.ToString();
                });
            });
            
            makeTeaTask.ContinueWith(_ =>
            {
                Dispatcher.Invoke(AfterMakingTea);
            });
        }
        
        private void MakeTeaAsyncUsingTPLDispatcherContinuationSecondAttempt_OnClick(object sender, RoutedEventArgs e)
        {
            BeforeMakingTea();
            var makeTeaTask = Task.Run(async () =>
            {
                _finalMessage = new StringBuilder();
                _finalMessage.AppendLine("Making tea started.");
                
                _finalMessage.AppendLine(await MakeTeaAsyncTPL.MakeMeTeaAsync());

                Dispatcher.Invoke(() =>
                { 
                    Notes.Text = _finalMessage.ToString();
                });
            });
            
            makeTeaTask.ContinueWith(_ =>
            {
                Dispatcher.Invoke(AfterMakingTea);
            });
        }
        
        private void MakeTeaAsyncUsingTPLCancellationFirstAttempt_OnClick(object sender, RoutedEventArgs e)
        {
            if(cancellationTokenSource != null)
            {
                // Already have an instance of the cancellation token source?
                // This means the button has already been pressed!

                cancellationTokenSource.Cancel();
                cancellationTokenSource = null;

                TplCancellationFirstButton.Content = "TPL - Cancellation";
                return;
            }

            cancellationTokenSource = new CancellationTokenSource();
            
            TplCancellationFirstButton.Content = "Cancel";

            BeforeMakingTea();
            var makeTeaTask = Task.Run(async () =>
            {
                _finalMessage = new StringBuilder();
                _finalMessage.AppendLine("Making tea started.");
                
                _finalMessage.AppendLine(await MakeTeaAsync.MakeMeTeaAsync());

                Dispatcher.Invoke(() =>
                { 
                    Notes.Text = _finalMessage.ToString();
                });
            }, cancellationTokenSource.Token);
            
            makeTeaTask.ContinueWith(_ =>
            {
                Dispatcher.Invoke(AfterMakingTea);
                cancellationTokenSource = null;
                TplCancellationFirstButton.Content = "TPL - Cancellation";
            });
        }

        private void MakeTeaAsyncUsingTPLCancellationSecondAttempt_OnClick(object sender, RoutedEventArgs e)
        {
            if(cancellationTokenSource != null)
            {
                // Already have an instance of the cancellation token source?
                // This means the button has already been pressed!

                cancellationTokenSource.Cancel();
                cancellationTokenSource = null;

                TplCancellationSecondButton.Content = "TPL - Cancellation 2";
                return;
            }

            cancellationTokenSource = new CancellationTokenSource();
            
            TplCancellationSecondButton.Content = "Cancel";

            BeforeMakingTea();
            var makeTeaTask = Task.Run(async () =>
            {
                _finalMessage = new StringBuilder();
                _finalMessage.AppendLine("Making tea started.");
                
                _finalMessage.AppendLine(await MakeTeaAsync.MakeMeTeaAsync());

                Dispatcher.Invoke(() =>
                { 
                    Notes.Text = _finalMessage.ToString();
                });
            }, cancellationTokenSource.Token);

            makeTeaTask.ContinueWith(_ =>
            {
                Dispatcher.Invoke(AfterMakingTea);
                cancellationTokenSource = null;
                TplCancellationSecondButton.Content = "TPL - Cancellation 2";
            });
        }



        /// <summary>
        /// Step 1: UI context, click button.
        /// Step 2: MyAsyncIntToString(7) is executed. UI Context.
        /// Step 3: Hits the await Task.Delay. Fires a new thread that will have attached the context that needs to come back.
        /// Step 4: Goes back to GiveMeStringFromAnInt and execute the next line.
        /// Step 5: myTask.Result will block the Context (UI) to wait for the result.
        /// Step 6: At some point, the awaited task will finish and will wait for the Context to be free so it can run the continuation.
        /// Step 7: Deadlock. .Result blocks the context while waiting for the task to finish + the Task is waiting for the context to be released.
        /// </summary>
        /// <returns></returns>
        public string GiveMeStringFromAnInt()
        {
            var myTask = MyAsyncIntToString(7);
            return myTask.Result;
        }

        private static async Task<string> MyAsyncIntToString(int number)
        {
            await Task.Delay(1000);
            return number.ToString();
        }
    }
}
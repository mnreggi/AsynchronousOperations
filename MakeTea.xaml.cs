using System;
using System.Text;
using System.Windows;
using Asynchronous_Operations.Services;

namespace Asynchronous_Operations
{
    public partial class MakeTea : Window
    {
        private static StringBuilder _finalMessage;
        
        public MakeTea()
        {
            InitializeComponent();
        }

        private void MakeTeaSynchronous_OnClick(object sender, RoutedEventArgs e)
        {
            _finalMessage = new StringBuilder();
            _finalMessage.AppendLine("Making tea started.");
            _finalMessage.AppendLine(MakeTeaSynchronous.MakeMeTea());

            Notes.Text = _finalMessage.ToString();
        }

        private async void MakeTeaAsynchronous_OnClick(object sender, RoutedEventArgs e)
        {
            _finalMessage = new StringBuilder();
            _finalMessage.AppendLine("Making tea started.");
            _finalMessage.AppendLine(await MakeTeaAsync.MakeMeTeaAsync());

            Notes.Text = _finalMessage.ToString();
        }
        
        /// <summary>
        /// This will crash the application
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MakeTeaAsynchronousConfigureAwaitFalse_OnClick(object sender, RoutedEventArgs e)
        {
            _finalMessage = new StringBuilder();
            _finalMessage.AppendLine("Making tea started.");
            
            // Adding ConfigureAwait false, will indicate that we don't care that the continuation be executed in a different thread.
            // Because this is a UI component, we can see that we are trying to update the UI with something, and because we are
            // in a different thread in the continuation, the application will crash.
            _finalMessage.AppendLine(await MakeTeaAsync.MakeMeTeaAsync().ConfigureAwait(false));

            Notes.Text = _finalMessage.ToString();
        }

        private async void MakeTeaAsyncHeavy_OnClick(object sender, RoutedEventArgs e)
        {
            _finalMessage = new StringBuilder();
            _finalMessage.AppendLine("Making tea started.");
            _finalMessage.AppendLine(await MakeHeavyTaskTea.MakeMeHeavyTeaAsync());

            Notes.Text = _finalMessage.ToString();
        }
        
        private async void MakeTeaAsyncWithException_OnClick(object sender, RoutedEventArgs e)
        {
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
        }

        private async void MakeTeaAsyncWithExceptionWithoutAwait_OnClick(object sender, RoutedEventArgs e)
        {
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
        }

        /// <summary>
        /// This will terminate the app.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MakeTeaAsyncWithExceptionAndAsyncVoid_OnClick(object sender, RoutedEventArgs e)
        {
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
        }
    }
}
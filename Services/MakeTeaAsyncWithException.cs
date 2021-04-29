using System;
using System.Text;
using System.Threading.Tasks;

namespace Asynchronous_Operations.Services
{
    public static class MakeTeaAsyncWithException
    {
        private static StringBuilder finalMessage;
        public static async Task<string> MakeMeTeaAsync()
        {
            finalMessage = new StringBuilder();
            
            var boilingWaterAsync = TrowAnException();
	
            finalMessage.AppendLine("4) Take the cups out.");
            finalMessage.AppendLine("5) Put tea in cups.");

            var water = await boilingWaterAsync;
            
            var tea = $"6) Pour {water} in cups.";
            finalMessage.AppendLine(tea);

            return finalMessage.ToString();
        }

        public static async Task MakeMeTeaAsyncInTryCatch()
        {
            try
            {
                finalMessage = new StringBuilder();

                await TrowAnException();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        /// <summary>
        /// This will terminate the app. Because what happened was that the exception that was thrown inside the method
        /// couldn't be set on a task. This means there's no task to swallow the exception, and the exception will be thrown back to the caller.
        /// Exception occurring in an async void method can NOT be caught.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public static async void MakeMeTeaAsyncVoidInTryCatch()
        {
            try
            {
                finalMessage = new StringBuilder();

                await TrowAnException();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        private static async Task<string> TrowAnException()
        {
            await Task.Delay(1000);
            throw new Exception("This is a generated exception");
        }
    }
}
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine($"Creating client on thread: {Thread.CurrentThread.ManagedThreadId}");
            var client = new HttpClient();
            
            Console.WriteLine($"GetStringAsync on : {Thread.CurrentThread.ManagedThreadId}");
            var task = client.GetStringAsync("http://google.com");
            
            Console.WriteLine($"Looping on thread: {Thread.CurrentThread.ManagedThreadId}");
            var a = 0;
            for (int i = 0; i < 10000000; i++)
            {
                a++;
            }
            
            Console.WriteLine($"Awaiting the task on thread: {Thread.CurrentThread.ManagedThreadId}");
            var page = await task;
            
            Console.WriteLine($"Continuation on thread: {Thread.CurrentThread.ManagedThreadId}");
            var continuation = 5;
        }

        private string Result = null;
        public async Task<string> Load()
        {
            if (Result != null)
            {
                return Result;
            }
            
            Result = await Task.Run(() => "CMP");
            return Result;
        }
    }
}
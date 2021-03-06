using System.Text;
using System.Threading.Tasks;

namespace Asynchronous_Operations.Services.TPL
{
    public static class MakeTeaAsyncTPL
    {
        private static StringBuilder finalMessage;
        public static Task<string> MakeMeTeaAsync()
        {
            finalMessage = new StringBuilder();

            var boilWaterTask = Task.Run(async () =>
            {
                var waterTask = BoilWater();

                finalMessage.AppendLine("4) Take the cups out.");
                finalMessage.AppendLine("5) Put tea in cups.");

                return await waterTask;
            });

            return boilWaterTask.ContinueWith((completedTask) =>
            {
                var tea = $"6) Pour {completedTask.Result} in cups.";
                finalMessage.AppendLine(tea);

                return finalMessage.ToString();
            });
        }

        private static async Task<string> BoilWater()
        {
            finalMessage.AppendLine("1) Start the kettle.");
            finalMessage.AppendLine("2) Waiting for the kettle 4 seconds.");
            finalMessage.AppendLine("------ Delay in progress -----");

            // Will await for 4 seconds, and all the code beneath it will be executed in the continuation
            // only once the data is available. We will passed back to the MakeMeTeaAsync().
            await Task.Delay(4000);

            // When the data is available, from the UI, we will come back here and continue executing the code.
            // Followed by the rest of the MakeMeTeaAsync()
            finalMessage.AppendLine("------ Delay finished -----");
            finalMessage.AppendLine("3) Kettle finished boiling.");

            return "water";
        }
    }
}
using System.Text;
using System.Threading.Tasks;

namespace Asynchronous_Operations.Services
{
    public static class MakeHeavyTaskTea
    {
        private static StringBuilder finalMessage;

        public static async Task<string> MakeMeHeavyTeaAsync()
        {
            finalMessage = new StringBuilder();

            // Will go inside the method
            var boilingWaterAsync = BoilWater();

            finalMessage.AppendLine("4) Take the cups out.");

            // Let's put a big workload here, assume that someone is knocking your door and you want to receive that person.
            var a = 0;
            for (int i = 0; i < 1000000000; i++){
                a += i;
            }
            
            finalMessage.AppendLine("5) Put tea in cups.");

            var water = await boilingWaterAsync;
            
            var tea = $"6) Pour {water} in cups.";
            finalMessage.AppendLine(tea);

            return finalMessage.ToString();
        }

        private static async Task<string> BoilWater()
        {
            finalMessage.AppendLine("1) Start the kettle.");
            finalMessage.AppendLine("2) Waiting for the kettle 1/2 seconds.");
            finalMessage.AppendLine("------ Delay in progress -----");

            // Let's wait just for half a second.
            await Task.Delay(500);

            finalMessage.AppendLine("------ Delay finished -----");
            finalMessage.AppendLine("3) Kettle finished boiling.");

            return "water";
        }
    }
}
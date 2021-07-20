using System.Text;
using System.Threading.Tasks;

namespace Asynchronous_Operations.Services
{
    public static class MakeTeaSynchronous
    {
        // String builder to simulate operations to write into the notes.
        private static StringBuilder finalMessage;
        
        public static string MakeMeTea()
        {
            finalMessage = new StringBuilder();
            var water = BoilWater();
	
            finalMessage.AppendLine("4) Take the cups out.");
            finalMessage.AppendLine("5) Put tea in cups.");

            finalMessage.AppendLine($"6) Pour {water} in cups.");

            return finalMessage.ToString();
        }

        private static string BoilWater()
        {
            finalMessage.AppendLine("1) Start the kettle.");
            finalMessage.AppendLine("2) Waiting for the kettle 3 seconds.");
            finalMessage.AppendLine("------ Delay in progress -----");

            Task.Delay(3000).GetAwaiter().GetResult();

            finalMessage.AppendLine("------ Delay finished -----");
            finalMessage.AppendLine("3) Kettle finished boiling.");

            return "water";
        }
    }
}
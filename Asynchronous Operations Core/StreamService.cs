using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Asynchronous_Operations_Core
{
    public class StreamService : IStreamService
    {
        public async IAsyncEnumerable<People> GetPeople([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            using var stream = new StreamReader(File.OpenRead("People.csv"));
            await stream.ReadLineAsync(); // Skip the header how in the CSV

            string line;
            while ((line = await stream.ReadLineAsync()) != null)
            {
                // If a cancellation has been requested, we will break the loop
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                await Task.Delay(1, cancellationToken);
                
                yield return People.FromCSV(line);
            }
        }
    }
}
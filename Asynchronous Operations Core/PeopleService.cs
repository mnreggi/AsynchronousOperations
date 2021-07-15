using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Asynchronous_Operations_Core
{
    public interface IPeopleService
    {
        Task<IEnumerable<People>> GetPeopleFrom(string cityIdentifier, CancellationToken cancellationToken);
    }

    public class PeopleService : IPeopleService
    {
        public async Task<IEnumerable<People>> GetPeopleFrom(string cityIdentifier, CancellationToken cancellationToken)
        {
            var listPeople = new List<People>();

            using (var stream = new StreamReader(File.OpenRead("People.csv")))
            {
                await stream.ReadLineAsync(); // Skip the header how in the CSV

                string line;
                while ((line = await stream.ReadLineAsync()) != null)
                {
                    listPeople.AddRange(
                        from city 
                            in cityIdentifier.Split(',') 
                        let segments = line.Split(',') 
                        where string.IsNullOrEmpty(city) || string.Equals(segments[1].Trim(), city, StringComparison.InvariantCultureIgnoreCase) 
                        select People.FromCSV(line));
                }
            }

            if(!listPeople.Any())
            {
                throw new KeyNotFoundException($"Could not find any people from {cityIdentifier}");
            }

            await Task.Delay(5000, cancellationToken);

            return listPeople;
        }
    }
}

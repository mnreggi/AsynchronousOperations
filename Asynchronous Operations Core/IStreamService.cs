using System.Collections.Generic;
using System.Threading;

namespace Asynchronous_Operations_Core
{
    public interface IStreamService
    {
        IAsyncEnumerable<People> GetPeople(CancellationToken cancellationToken = default);
    }
}
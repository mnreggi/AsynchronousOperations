using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Asynchronous_Operations_Core
{
    
    /// <summary>
    /// This will mock the call to the file or any IO operation. We will simulate using delays that the async operation is taking longer.
    /// </summary>
    public class MockStreamService : IStreamService
    {
        // Using the async keyword here will not complain about not returning a task. It will do this under the hood.
        // Instead of using the 'return' statement we are going to use the 'yield return'
        // Here it will fakes that it takes about 500 milliseconds to retrieve an item.
        // The caller will be able to get each item after 500ms each.
        public async IAsyncEnumerable<People> GetPeople([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.Delay(500, cancellationToken);
            yield return new People() {Name = "Matias", City = "Wellington", PositiveFeedback = 10, NegativeFeedback = 1};
            
            await Task.Delay(500, cancellationToken);
            yield return new People() {Name = "Peter", City = "Tauranga", PositiveFeedback = 0, NegativeFeedback = 0};
            
            await Task.Delay(500, cancellationToken);
            yield return new People() {Name = "Sean", City = "Wellington", PositiveFeedback = 100, NegativeFeedback = 34};
            
            await Task.Delay(500, cancellationToken);
            yield return new People() {Name = "Mike", City = "Taupo", PositiveFeedback = 11, NegativeFeedback = 4};
            
            await Task.Delay(500, cancellationToken);
            yield return new People() {Name = "Paul", City = "Auckland", PositiveFeedback = 3, NegativeFeedback = 1};
            
            await Task.Delay(500, cancellationToken);
            yield return new People() {Name = "Clare", City = "Wellington", PositiveFeedback = 3, NegativeFeedback = 1};
            
            await Task.Delay(500, cancellationToken);
            yield return new People() {Name = "Ivy", City = "Auckland", PositiveFeedback = 3, NegativeFeedback = 1};
            
            await Task.Delay(500, cancellationToken);
            yield return new People() {Name = "Clare", City = "Auckland", PositiveFeedback = 3, NegativeFeedback = 1};
            
            await Task.Delay(500, cancellationToken);
            yield return new People() {Name = "Mike", City = "Wellington", PositiveFeedback = 3, NegativeFeedback = 1};
            
            await Task.Delay(500, cancellationToken);
            yield return new People() {Name = "Stefan", City = "Auckland", PositiveFeedback = 3, NegativeFeedback = 1};
        }
    }
}
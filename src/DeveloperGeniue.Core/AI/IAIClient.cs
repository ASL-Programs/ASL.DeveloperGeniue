using System.Threading;
using System.Threading.Tasks;

namespace DeveloperGeniue.Core.AI;

public interface IAIClient
{
    Task<AIResponse> GetCompletionAsync(AIRequest request, CancellationToken cancellationToken = default);
}

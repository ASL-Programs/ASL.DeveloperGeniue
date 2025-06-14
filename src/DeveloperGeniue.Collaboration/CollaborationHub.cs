using Microsoft.AspNetCore.SignalR;

namespace DeveloperGeniue.Collaboration;

public class CollaborationHub : Hub
{
    private readonly IMergeConflictResolver _resolver;

    public CollaborationHub(IMergeConflictResolver resolver)
    {
        _resolver = resolver;
    }

    public async Task ShareCodeAsync(string sessionId, string baseCode, string incomingCode)
    {
        var merged = await _resolver.ResolveAsync(baseCode, incomingCode);
        await Clients.Group(sessionId).SendAsync("ReceiveSharedCode", merged);
    }
}

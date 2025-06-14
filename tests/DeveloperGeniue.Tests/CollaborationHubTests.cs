using DeveloperGeniue.Collaboration;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Xunit;

namespace DeveloperGeniue.Tests;

public class CollaborationHubTests
{
    private class ClientProxy : IClientProxy
    {
        public string? Sent;
        public Task SendCoreAsync(string method, object?[] args, CancellationToken cancellationToken = default)
        {
            Sent = args[0]?.ToString();
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task ShareCodeUsesResolver()
    {
        var proxy = new ClientProxy();
        var clients = new HubCallerClients();
        clients.GroupProxy = proxy;
        var hub = new CollaborationHub(new SimpleMergeConflictResolver());
        typeof(Hub).GetProperty("Clients")!.SetValue(hub, clients);
        await hub.ShareCodeAsync("room", "A", "B");
        Assert.Equal("A\nB", proxy.Sent);
    }

    private class HubCallerClients : IHubCallerClients
    {
        public IClientProxy All => GroupProxy;
        public IClientProxy GroupProxy { get; set; } = new ClientProxy();
        public IClientProxy Caller => GroupProxy;
        public IClientProxy Others => GroupProxy;
        public IClientProxy AllExcept(IReadOnlyList<string> excludedConnectionIds) => GroupProxy;
        public IClientProxy Client(string connectionId) => GroupProxy;
        public IClientProxy Clients(IReadOnlyList<string> connectionIds) => GroupProxy;
        public IClientProxy Group(string groupName) => GroupProxy;
        public IClientProxy GroupExcept(string groupName, IReadOnlyList<string> connectionIds) => GroupProxy;
        public IClientProxy Groups(IReadOnlyList<string> groupNames) => GroupProxy;
        public IClientProxy OthersInGroup(string groupName) => GroupProxy;
        public IClientProxy User(string userId) => GroupProxy;
        public IClientProxy Users(IReadOnlyList<string> userIds) => GroupProxy;
    }

}


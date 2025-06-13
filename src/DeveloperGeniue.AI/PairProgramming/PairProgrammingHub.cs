using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace DeveloperGeniue.AI.PairProgramming;

/// <summary>
/// SignalR hub used for real-time pair programming sessions.
/// </summary>
public class PairProgrammingHub : Hub
{
    /// <summary>
    /// Receives a code update from a client and broadcasts it to others.
    /// </summary>
    public async Task SendCodeUpdate(string code)
    {
        await Clients.Others.SendAsync("ReceiveCodeUpdate", code);
    }
}

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

    /// <summary>
    /// Adds the caller to a collaboration session.
    /// </summary>
    public async Task JoinSession(string sessionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
    }

    /// <summary>
    /// Removes the caller from a collaboration session.
    /// </summary>
    public async Task LeaveSession(string sessionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
    }

    /// <summary>
    /// Shares the current cursor position with collaborators.
    /// </summary>
    public async Task SendCursorPosition(string sessionId, int line, int column)
    {
        await Clients.OthersInGroup(sessionId).SendAsync("ReceiveCursorPosition", line, column);
    }

    /// <summary>
    /// Sends a chat message to collaborators.
    /// </summary>
    public async Task SendChatMessage(string sessionId, string message)
    {
        await Clients.OthersInGroup(sessionId).SendAsync("ReceiveChatMessage", message);
    }
}

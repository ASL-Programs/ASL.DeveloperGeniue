using System.Threading;
using System.Threading.Tasks;
namespace DeveloperGeniue.AI.Speech;

/// <summary>
/// Basic voice command interface.
/// </summary>
public interface ISpeechInterface
{
    /// <summary>
    /// Listens for a single command and returns the recognized text.
    /// </summary>
    Task<string> ListenForCommandAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Speaks the provided text to the user.
    /// </summary>
    Task SpeakAsync(string text, CancellationToken cancellationToken = default);
}

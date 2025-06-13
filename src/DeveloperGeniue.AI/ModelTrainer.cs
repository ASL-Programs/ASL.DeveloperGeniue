using System;
using System.Threading;
using System.Threading.Tasks;
namespace DeveloperGeniue.AI;

/// <summary>
/// Provides methods to train a custom AI model.
/// </summary>
public class ModelTrainer
{
    /// <summary>
    /// Trains the model using the provided dataset path.
    /// </summary>
    /// <param name="dataPath">Path to the training data.</param>
    public void TrainModel(string dataPath)
    {
        // Placeholder for actual training logic.
        Console.WriteLine($"Training model with data at {dataPath}...");
    }

    /// <summary>
    /// Prompts the user for a training data path via voice input and trains the model.
    /// </summary>
    public async Task TrainModelWithVoiceAsync(Speech.ISpeechInterface speech, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Please specify the training data path:");
        var path = await speech.ListenForCommandAsync(cancellationToken);
        TrainModel(path);
    }
}

using Xunit;
using System;
using System.IO;
using DeveloperGeniue.AI.Speech;

namespace DeveloperGeniue.Tests;

public class SpeechInterfaceTests
{
    [Fact]
    public async Task ListenForCommandReturnsUserInput()
    {
        var input = new StringReader("hello");
        Console.SetIn(input);
        var speech = new SpeechInterface();
        var command = await speech.ListenForCommandAsync();
        Assert.Equal("hello", command);
    }
}

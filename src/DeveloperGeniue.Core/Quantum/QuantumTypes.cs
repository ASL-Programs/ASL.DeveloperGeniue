namespace DeveloperGeniue.Core;

public record QuantumCompatibilityReport(bool IsCompatible, string Notes);
public record QuantumAlgorithmSuggestions(string[] Suggestions);
public record PostQuantumSecurityReport(bool Safe, string[] Warnings);
public record QuantumSimulationResult(double EstimatedQubits, double SpeedupFactor);
public record QuantumPerformanceAnalysis(int QubitCount, double EstimatedSpeedup);

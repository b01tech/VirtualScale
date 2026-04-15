namespace VirtualScale.Api.Dtos;

public record ScaleResponse(
    decimal BruteWeight,
    decimal NetWeight,
    decimal TareWeight,
    bool IsTared,
    bool IsStable,
    int FilterLevel,
    int NumberOfCells
);

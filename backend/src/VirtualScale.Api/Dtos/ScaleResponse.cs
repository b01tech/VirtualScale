namespace VirtualScale.Api.Dtos;

public record ScaleResponse(
    decimal BruteWeight,
    decimal NetWeight,
    decimal TareWeight,
    bool IsTared,
    bool IsStable,
    int FilterLevel,
    int NumberOfCells,
    decimal CapMax,
    int Division,
    int DecimalPlaces,
    decimal ReferenceWeight,
    bool NeedsCalibrationAdjustment
);
